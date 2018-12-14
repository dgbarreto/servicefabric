using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using ProductService;
using WebAPI.Model;

namespace WebAPI.Controllers {
    [Route("api/products")]
    public class ProductsController : Controller {
        private readonly IProductService mService;

        public ProductsController() {
            mService = ServiceProxy.Create<IProductService>(new Uri("fabric:/ECommerce/ProductService"), new ServicePartitionKey(0));
        }

        [HttpGet("get")]
        public async Task<IEnumerable<ApiProduct>> Get() {
            IEnumerable<Product> products = await mService.GetProducts();

            return products.Select(s => new ApiProduct() {
                Description = s.Description,
                ID = s.ID,
                IsAvailable = s.IsAvailable,
                Name = s.Name,
                Price = s.Price
            });
        }

        [HttpPost("post")]
        public async Task Post([FromBody] ApiProduct product) {
            Product newProduct = new Product() {
                Description = product.Description,
                ID = Guid.NewGuid(),
                IsAvailable = product.IsAvailable,
                Name = product.Name,
                Price = product.Price
            };

            await mService.AddProduct(newProduct);
        }
    }
}