using BusinessLogicLayer.DTO;
using BusinessLogicLayer.ServiceContracts;
using FluentValidation;
using FluentValidation.Results;

namespace ProductsMicroservice.API.APIEndpoints;

public static class ProductAPIEndpoints
{
    public static IEndpointRouteBuilder MapProductAPIEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/products", async (IProductService productService) =>
        {
            return Results.Ok(await productService.GetProducts());
        });

        app.MapGet("/api/products/search/product-id/{productID:guid}", async (IProductService service, Guid productID) =>
        {
            ProductResponse? productResponse = await service.GetProductByCondition(p => p.ProductID == productID);
            return Results.Ok(productResponse);
        });

        app.MapGet("/api/products/search/{serachString}", async (IProductService service, string serachString) =>
        {
            List<ProductResponse?> productsByName = await service
                                                        .GetProductsByCondition(p => p.ProductName.Contains(serachString, StringComparison.OrdinalIgnoreCase)
                                                                                    && p.ProductName != null);

            List<ProductResponse?> productByCategory = await service
                                                        .GetProductsByCondition(p => p.Category.Contains(serachString, StringComparison.OrdinalIgnoreCase)
                                                                                    && p.Category != null);


            IEnumerable<ProductResponse?> products = productsByName.Union(productByCategory);

            return Results.Ok(products);


        });

        app.MapPost("/api/products", async (IProductService service, ProductAddRequest productAddRequest, IValidator<ProductAddRequest> productAddRequestValidator) =>
        {
            ValidationResult validationResult = await productAddRequestValidator.ValidateAsync(productAddRequest);
            if (!validationResult.IsValid)
            {
                var errorDictionary = validationResult.Errors
                            .GroupBy(e => e.PropertyName)  // Same field ke errors ko group karte hain
                            .ToDictionary(
                                g => g.Key,                // Property ka naam
                                g => g.Select(e => e.ErrorMessage).ToArray() // Uske saare errors
                            );

                return Results.ValidationProblem(errorDictionary);
            }

            ProductResponse? addedProductResponce = await service.AddProduct(productAddRequest);

            if (addedProductResponce != null)
            {
                return Results.Created($"/api/products/search/product-id/{addedProductResponce.ProductID}", addedProductResponce);
            }
            else
            {
                return Results.Problem("Error while adding product.");
            }

        });

        app.MapPut("/api/products", async (IProductService service,
                                            ProductUpdateRequest productUpdateRequest,
                                            IValidator<ProductUpdateRequest> productUpdateRequestValidator) =>
        {
            ValidationResult validationResult = await productUpdateRequestValidator.ValidateAsync(productUpdateRequest);
            
            if (!validationResult.IsValid)
            {
                var errorDictionary = validationResult.Errors
                            .GroupBy(e => e.PropertyName)  // Same field ke errors ko group karte hain
                            .ToDictionary(
                                g => g.Key,                // Property ka naam
                                g => g.Select(e => e.ErrorMessage).ToArray() // Uske saare errors
                            );

                return Results.ValidationProblem(errorDictionary);
            }

            ProductResponse? updatedProductResponce = await service.UpdateProduct(productUpdateRequest);

            if (updatedProductResponce != null)
            {
                return Results.Ok(updatedProductResponce);
            }
            else
            {
                return Results.Problem("Error while updating the product.");
            }

        });

        app.MapDelete("/api/products/{productId:guid}", async (IProductService service, Guid productId) =>
        {
            var isDeleted = await service.DeleteProduct(productId);

            if (isDeleted)
                return Results.Ok(isDeleted);
            else
                return Results.Problem("Error while deleting the record.");
        });


        return app;
    }
}
