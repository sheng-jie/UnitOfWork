using System.Collections.Generic;
using System.Linq;

namespace UnitOfWork.ShoppingCart
{
    public class ShoppingCart : AggregateRoot
    {
        public int CustomerId { get; set; }
        public virtual Customer.Customer Customer { get; set; }

        public List<ShoppingCartLine> ShoppingCartLines { get; } = new List<ShoppingCartLine>();

        //添加
        public void AddGoods(Goods.Goods goods, int quantity)
        {
            ShoppingCartLine line = ShoppingCartLines.FirstOrDefault(p => p.Goods.Id == goods.Id);
            if (line == null)
            {
                ShoppingCartLines.Add(new ShoppingCartLine()
                {
                    Goods = goods,
                    Qty = quantity,
                    ShoppingCartId = this.Id
                });
            }
            else
            {
                line.Qty += quantity;
            }
        }

        //点击数量+号或点击数量-号或自己输入一个值
        public void ChangeItmeQty(ShoppingCartLine cartLine, int qty)
        {
            if (qty == 0)
            {
                RemoveItem(cartLine);
            }

            cartLine.Qty = qty;
        }

        //移除
        public void RemoveItem(ShoppingCartLine cartLine)
        {
            ShoppingCartLines.Remove(cartLine);
        }

        //清空
        public void Clear()
        {
            ShoppingCartLines.Clear();
        }
    }
}