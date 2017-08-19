namespace UnitOfWork
{
    public class AggregateRoot : AggregateRoot<int>, IAggregateRoot
    {
        
    }
    public class AggregateRoot<TPrimaryKey> : Entity<TPrimaryKey>, IAggregateRoot<TPrimaryKey>
    {

    }
}