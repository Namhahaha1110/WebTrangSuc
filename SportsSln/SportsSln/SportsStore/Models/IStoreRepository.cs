using System.Linq;
using System.Threading.Tasks;

namespace SportsStore.Models
{
    public interface IStoreRepository
    {
        IQueryable<Product> Products { get; }
        IQueryable<Category> Categories { get; }
        IQueryable<Banner> Banners { get; }
     
    
        Task CreateProduct(Product p);
        Task SaveProduct(Product p);
        Task DeleteProduct(Product p);

        Task CreateCategory(Category c);
        Task SaveCategory(Category c);
        Task DeleteCategory(Category c);

        Task CreateBanner(Banner b);
        Task SaveBanner(Banner b);
        Task DeleteBanner(Banner b);
    }
}
