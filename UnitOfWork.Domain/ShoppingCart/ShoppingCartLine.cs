namespace UnitOfWork.ShoppingCart
{
    public class ShoppingCartLine : Entity
    {
        public int ShoppingCartId { get; set; }
        public virtual UnitOfWork.ShoppingCart.ShoppingCart ShoppingCart { get; set; }

        public int GoodsId { get; set; }

        public virtual Goods.Goods Goods { get; set; }

        public int Qty { get; set; }
    }
}