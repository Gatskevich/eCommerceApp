﻿using eCommerce.SharedLibrary.DTOs.Responses;
using ProductApi.Domain.Entities;

namespace ProductApi.Application.DTOs.Conversions
{
    public static class ProductConversion
    {
        public static Product ToEntity(ProductDTO product) => new()
        {
            Id = product.Id,
            Name = product.Name,
            Quantity = product.Quantity,
            Price = product.Price
        };

        public static (ProductDTO?, IEnumerable<ProductDTO>?) FromEntity(Product product, IEnumerable<Product>? products)
        {
            // Return a single
            if (product is not null || products is null)
            {
                var singleProduct = new ProductDTO
                    (
                    product!.Id,
                    product.Name!,
                    product.Quantity,
                    product.Price
                    );
                return (singleProduct, null);
            }

            // Return a list 
            if (products is not null || product is null)
            {
                var _products = products!.Select(p => new ProductDTO
                    (
                    p.Id,
                    p.Name!,
                    p.Quantity,
                    p.Price
                    )).ToList();

                return (null, _products);
            }

            // Return null
            return (null, null);
        }
    }
}
