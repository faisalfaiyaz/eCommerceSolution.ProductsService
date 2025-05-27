using DataAccessLayer.Entities;
using System.Linq.Expressions;

namespace DataAccessLayer.RepositoryContracts;

public interface IProductRepository
{
    /// <summary>
    /// it is used to get available product list from Products table
    /// </summary>
    /// <returns>list of Product</returns>
    Task<IEnumerable<Product>> GetProducts();
    Task<IEnumerable<Product?>> GetProductsByCondtion(Expression<Func<Product, bool>> condtionExpression);
    Task<Product?> GetProductByCondtion(Expression<Func<Product, bool>> condtionExpression);

    Task<Product?> AddProduct(Product product);
    Task<Product?> UpdateProduct(Product product);

    Task<bool> DeleteProduct(Guid productId);
}
