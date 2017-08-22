# 1. 引言
> Maintains a list of objects affected by a business transaction and coordinates the writing out of changes and the resolution of concurrency problems.
 *[Unit of Work](https://martinfowler.com/eaaCatalog/unitOfWork.html) --Martin Fowler*

Unit Of Work模式，由马丁大叔提出，是一种数据访问模式。UOW模式的作用是在业务用例的操作中跟踪对象的所有更改（增加、删除和更新），并将所有更改的对象保存在其维护的列表中。在业务用例的终点，通过事务，**一次性提交所有更改**，以确保数据的完整性和有效性。总而言之，UOW协调这些对象的持久化及并发问题。

# 2. UOW的本质
通过以上的介绍，我们可以总结出实现UOW的几个要点：
1. UOW跟踪变化
2. UOW维护了一个变更列表
3. UOW将跟踪到的已变更的对象保存到变更列表中
4. UOW借助事务一次性提交变更列表中的所有更改
5. UOW处理并发

而对于这些要点，EF中的DBContext已经实现了。

# 3. EF中的UOW

每个`DbContext`类型实例都有一个`ChangeTracker`用来跟踪记录实体的变化。当调用`SaveChanges`时，所有的更改将通过事务一次性提交到数据库。

我们直接看个EF Core的测试用例：
```
public ApplicationDbContext InMemorySqliteTestDbContext
{
    get
    {
        // In-memory database only exists while the connection is open
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new ApplicationDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }
}

[Fact]
public void Test_Ef_Implemented_Uow()
{
    //新增用户
    var user = new ApplicationUser()
    {
        UserName = "shengjie",
        Email = "ysjshengjie@qq.com"
    };

    InMemorySqliteTestDbContext.Users.Add(user);

    //创建用户对应客户
    var customer = new Customer()
    {
        ApplicationUser = user,
        NickName = "圣杰"
    };

    InMemorySqliteTestDbContext.Customers.Add(customer);

    //添加地址
    var address = new Address("广东省", "深圳市", "福田区", "下沙街道", "圣杰", "135****9309");

    InMemorySqliteTestDbContext.Addresses.Add(address);

    //修改客户对象的派送地址
    customer.AddShippingAddress(address);

    InMemoryTestDbContext.Entry(customer).State = EntityState.Modified;

    //保存
    var changes = InMemorySqliteTestDbContext.SaveChanges();

    Assert.Equal(3, changes);

    var savedCustomer = InMemorySqliteTestDbContext.Customers
        .FirstOrDefault(c => c.NickName == "圣杰");

    Assert.Equal("shengjie", savedCustomer.ApplicationUser.UserName);

    Assert.Equal(customer.ApplicationUserId, savedCustomer.ApplicationUserId);

    Assert.Equal(1, savedCustomer.ShippingAddresses.Count);
}
```
首先这个用例是绿色通过的。该测试用例中我们添加了一个User，并为User创建对应的Customer，同时为Customer添加一条Address。从代码中我们可以看出仅做了一次保存，新增加的User、Customer、Address对象都成功持久化到了内存数据库中。从而证明EF Core是实现了Uow模式的。但很显然应用程序与基础设施层高度耦合，那如何解耦呢？继续往下看。

# 4. DDD中的UOW
那既然EF Core已经实现了Uow模式，我们还有必要自行实现一套Uow模式吗？这就视具体情况而定了，如果你的项目简单的增删改查就搞定了的，就不用折腾了。

在DDD中，我们会借助仓储模式来实现领域对象的持久化。仓储只关注于单一聚合的持久化，而业务用例却常常会涉及多个聚合的更改，为了确保业务用例的一致型，我们需要引入事务管理，而事务管理是应用服务层的关注点。我们如何在应用服务层来管理事务呢？借助UOW。这样就形成了一条链：Uow->仓储-->聚合-->实体和值对象。即Uow负责管理仓储处理事务，仓储管理单一聚合，聚合又由实体和值对象组成。

下面我们就先来定义实体和值对象，这里我们使用层超类型。

## 4.1. 定义实体
```
    /// <summary>
    /// A shortcut of <see cref="IEntity{TPrimaryKey}"/> for most used primary key type (<see cref="int"/>).
    /// </summary>
    public interface IEntity : IEntity<int>
    {

    }

    /// <summary>
    /// Defines interface for base entity type. All entities in the system must implement this interface.
    /// </summary>
    /// <typeparam name="TPrimaryKey">Type of the primary key of the entity</typeparam>
    public interface IEntity<TPrimaryKey>
    {
        /// <summary>
        /// Unique identifier for this entity.
        /// </summary>
        TPrimaryKey Id { get; set; }
    }
```

## 4.2. 定义聚合
```
namespace UnitOfWork
{
    public interface IAggregateRoot : IAggregateRoot<int>, IEntity
    {

    }

    public interface IAggregateRoot<TPrimaryKey> : IEntity<TPrimaryKey>
    {

    }
}
```

## 4.3. 定义泛型仓储
```
namespace UnitOfWork
{
    public interface IRepository<TEntity> : IRepository<TEntity, int>
        where TEntity : class, IEntity, IAggregateRoot
    {

    }

    public interface IRepository<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>, IAggregateRoot<TPrimaryKey>
    {        
        IQueryable<TEntity> GetAll();

        TEntity Get(TPrimaryKey id);

        TEntity FirstOrDefault(TPrimaryKey id);

        TEntity Insert(TEntity entity);
        
        TEntity Update(TEntity entity);

        void Delete(TEntity entity);

        void Delete(TPrimaryKey id);
    }
}
```
因为仓储是管理聚合的，所以我们需要限制泛型参数为实现`IAggregateRoot`的类。

## 4.4. 实现泛型仓储
```
amespace UnitOfWork.Repositories
{
    public class EfCoreRepository<TEntity>
        : EfCoreRepository<TEntity, int>, IRepository<TEntity>
        where TEntity : class, IEntity, IAggregateRoot
    {
        public EfCoreRepository(UnitOfWorkDbContext dbDbContext) : base(dbDbContext)
        {
        }
    }

    public class EfCoreRepository<TEntity, TPrimaryKey>
        : IRepository<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>, IAggregateRoot<TPrimaryKey>
    {
        private readonly UnitOfWorkDbContext _dbContext;

        public virtual DbSet<TEntity> Table => _dbContext.Set<TEntity>();

        public EfCoreRepository(UnitOfWorkDbContext dbDbContext)
        {
            _dbContext = dbDbContext;
        }

        public IQueryable<TEntity> GetAll()
        {
            return Table.AsQueryable();
        }

        public TEntity Insert(TEntity entity)
        {
            var newEntity = Table.Add(entity).Entity;
            _dbContext.SaveChanges();
            return newEntity;
        }

        public TEntity Update(TEntity entity)
        {
            AttachIfNot(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;

            _dbContext.SaveChanges();

            return entity;
        }

        public void Delete(TEntity entity)
        {
            AttachIfNot(entity);
            Table.Remove(entity);

           _dbContext.SaveChanges();
        }

        public void Delete(TPrimaryKey id)
        {
            var entity = GetFromChangeTrackerOrNull(id);
            if (entity != null)
            {
                Delete(entity);
                return;
            }

            entity = FirstOrDefault(id);
            if (entity != null)
            {
                Delete(entity);
                return;
            }
        }

        protected virtual void AttachIfNot(TEntity entity)
        {
            var entry = _dbContext.ChangeTracker.Entries().FirstOrDefault(ent => ent.Entity == entity);
            if (entry != null)
            {
                return;
            }

            Table.Attach(entity);
        }

        private TEntity GetFromChangeTrackerOrNull(TPrimaryKey id)
        {
            var entry = _dbContext.ChangeTracker.Entries()
                .FirstOrDefault(
                    ent =>
                        ent.Entity is TEntity &&
                        EqualityComparer<TPrimaryKey>.Default.Equals(id, ((TEntity)ent.Entity).Id)
                );

            return entry?.Entity as TEntity;
        }
    }
}
```
因为我们直接使用EF Core进行持久化，所以我们直接通过构造函数初始化DbContex实例。同时，我们注意到`Insert、Update、Delete`方法都显式的调用了`SaveChanges`方法。

至此，我们完成了从实体到聚合再到仓储的定义和实现，万事俱备，只欠Uow。

## 4.5. 实现UOW
通过第3节的说明我们已经知道，EF Core已经实现了UOW模式。而为了确保领域层透明的进行持久化，我们对其进行了更高一层的抽象，实现了仓储模式。但这似乎引入了另外一个问题，因为仓储是管理单一聚合的，每次做增删改时都显式的提交了更改（调用了SaveChanges），在处理多个聚合时，就无法利用DbContext进行批量提交了。那该如何是好？一不做二不休，我们再对其进行一层抽象，抽离保存接口，这也就是Uow的核心接口方法。
我们抽离`SaveChanges`方法，定义`IUnitOfWork`接口。
```
namespace UnitOfWork
{
    public interface IUnitOfWork
    {
        int SaveChanges();
    }
}
```
因为我们是基于EFCore实现Uow的，所以我们只需要依赖DbContex，就可以实现批量提交。实现也很简单：
```
namespace UnitOfWork
{
    public class UnitOfWork<TDbContext> : IUnitOfWork where TDbContext : DbContext
    {
        private readonly TDbContext _dbContext;

        public UnitOfWork(TDbContext context)
        {
            _dbContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }
    }
}
```
既然Uow接手保存操作，自然我们需要：**注释掉EfCoreRepository中Insert、Update、Delete方法中的显式保存调用`_dbContext.SaveChanges();`**。

那如何确保操作多个仓储时，最终能够一次性提交所有呢？

**确保Uow和仓储共用同一个DbContex即可**。这个时候我们就可以借助依赖注入。

## 4.6. 依赖注入
我们直接使用.net core 提供的依赖注入，依次注入DbContext、UnitOfWork和Repository。
```
//注入DbContext
services.AddDbContext<UnitOfWorkDbContext>(
    options =>options.UseSqlServer(
    Configuration.GetConnectionString("DefaultConnection")));

//注入Uow依赖
services.AddScoped<IUnitOfWork, UnitOfWork<UnitOfWorkDbContext>>();

//注入泛型仓储
services.AddTransient(typeof(IRepository<>), typeof(EfCoreRepository<>));
services.AddTransient(typeof(IRepository<,>), typeof(EfCoreRepository<,>));
```
这里我们限定了DbContext和UnitOfWork的生命周期为`Scoped`，从而确保每次请求共用同一个对象。如何理解呢？就是**整个调用链**上的需要注入的同类型对象，使用是同一个类型实例。
## 4.7. 使用UOW
下面我们就来实际看一看如何使用UOW，我们定义一个应用服务：
```
namespace UnitOfWork.Customer
{
    public class CustomerAppService : ICustomerAppService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<ShoppingCart.ShoppingCart> _shoppingCartRepository;

        public CustomerAppService(IRepository<ShoppingCart> shoppingCartRepository, 
            IRepository<Customer> customerRepository, IUnitOfWork unitOfWork)
        {
            _shoppingCartRepository = shoppingCartRepository;
            _customerRepository = customerRepository;
            _unitOfWork = unitOfWork;
        }

        public void CreateCustomer(Customer customer)
        {
            _customerRepository.Insert(customer);//创建客户

            var cart = new ShoppingCart.ShoppingCart() {CustomerId = customer.Id};
            _shoppingCartRepository.Insert(cart);//创建购物车
            _unitOfWork.SaveChanges();
        }

        //....
    }
}
```
通过以上案例，我们可以看出，我们只需要通过构造函数依赖注入需要的仓储和Uow即可完成对多个仓储的持久化操作。

# 5. 最后

对于Uow模式，有很多种实现方式，大多过于复杂抽象。EF和EF Core本身已经实现了Uow模式，所以在实现时，我们应避免不必要的抽象来降低系统的复杂度。

最后，重申一下：
**Uow模式是用来管理仓储处理事务的，仓储用来解耦的（领域层与基础设施层）。而基于EF实现Uow模式的关键：确保Uow和Reopository之间共享同一个DbContext实例。**

最后附上基于.Net Core和EF Core实现的源码： [GitHub--UnitOfWork](https://github.com/yanshengjie/UnitOfWork)
