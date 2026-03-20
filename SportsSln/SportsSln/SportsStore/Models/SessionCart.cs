using System.Text.Json.Serialization;
using SportsStore.Infrastructure;

namespace SportsStore.Models
{
    // ==================== SESSIONCART ====================
    public class SessionCart : Cart
    {
        [System.Text.Json.Serialization.JsonIgnore]
        public ISession? Session { get; set; }

        // Lấy giỏ từ session
        public static Cart GetCart(IServiceProvider services)
        {
            ISession? session = services.GetRequiredService<IHttpContextAccessor>()
                                      .HttpContext?.Session;
            SessionCart cart = session?.GetJson<SessionCart>("Cart") ?? new SessionCart();
            cart.Session = session;
            return cart;
        }

        public override void AddItem(Product product, int quantity)
        {
            base.AddItem(product, quantity);
            Session?.SetJson("Cart", this);
        }

        public override void RemoveLine(Product product)
        {
            base.RemoveLine(product);
            Session?.SetJson("Cart", this);
        }

        public override void Clear()
        {
            base.Clear();
            Session?.Remove("Cart");
        }

        public override void UpdateQuantity(Product product, int quantity)
        {
            base.UpdateQuantity(product, quantity);
            Session?.SetJson("Cart", this);
        }
    }
}
