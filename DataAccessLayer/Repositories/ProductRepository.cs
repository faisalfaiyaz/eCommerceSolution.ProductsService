using DataAccessLayer.Context;
using DataAccessLayer.Entities;
using DataAccessLayer.RepositoryContracts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataAccessLayer.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _db;

    public ProductRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Product?> AddProduct(Product product)
    {
        await _db.Products.AddAsync(product);
        await _db.SaveChangesAsync();

        return product;
    }

    public async Task<bool> DeleteProduct(Guid productId)
    {
        Product productFromDb = await _db.Products.FindAsync(productId);
        _db.Products.Remove(productFromDb);
        int affectedRows = await _db.SaveChangesAsync();

        return affectedRows > 0;
    }

    public async Task<Product?> GetProductByCondtion(Expression<Func<Product, bool>> condtionExpression)
    {
        return await _db.Products.FirstOrDefaultAsync(condtionExpression);
    }

    public async Task<IEnumerable<Product>> GetProducts()
    {
        return await _db.Products.ToListAsync();

    }

    public async Task<IEnumerable<Product?>> GetProductsByCondtion(Expression<Func<Product, bool>> condtionExpression)
    {
        return await _db.Products.Where(condtionExpression).ToListAsync();
    }

    public async Task<Product?> UpdateProduct(Product product)
    {
        Product? existingProduct = await _db.Products.FirstOrDefaultAsync(p => p.ProductId == product.ProductId);

        if (existingProduct == null)
        {
            return existingProduct;
        }

        existingProduct.ProductName = product.ProductName;
        existingProduct.UnitPrice = product.UnitPrice;
        existingProduct.QuantityInStock = product.QuantityInStock;
        existingProduct.Category = product.Category;

        await _db.SaveChangesAsync();

        return existingProduct;

    }
}
