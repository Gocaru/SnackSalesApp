using ApiECommerce.Entities;

namespace ApiECommerce.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetCategories();
    }
}
