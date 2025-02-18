using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductAPI.Domain.Entities;

namespace ProductAPI.Application.DTOs.Conversions
{
    public class ProductConversion
    {
        public static Product ToEntity(ProductDTO productDTO) => new()
        {
            Id = productDTO.Id,
            Name = productDTO.Name,
            Quantity = productDTO.Quantity,
            Price = productDTO.Price
        };

        public static (ProductDTO?, IEnumerable<ProductDTO>?) FromEntity(Product product,
            IEnumerable<Product>? products)
        {
            if (product is not null || products is null)
            {
                var productDto = new ProductDTO
                (
                    product!.Id,
                    product.Name!,
                    product.Quantity,
                    product.Price

                );
                return (productDto, null);
            }

            if (product is null || products is not null)
            {
                var productsDto = products.Select(ps=> new ProductDTO(ps.Id,ps.Name!,ps.Quantity, ps.Price)).ToList();
                return (null, productsDto);
            }

            return (null, null);
        }
    }
}
