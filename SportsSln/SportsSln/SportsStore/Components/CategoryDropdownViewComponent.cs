using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsStore.Models;

namespace SportsStore.Components
{
    public class CategoryDropdownViewComponent : ViewComponent
    {
        private readonly IStoreRepository repository;

        public CategoryDropdownViewComponent(IStoreRepository repo)
        {
            repository = repo;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await repository.Categories.OrderBy(c => c.Name).ToListAsync();
            return View(categories);
        }
    }
}
