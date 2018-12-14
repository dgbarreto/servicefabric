using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProductService {
    public interface IProductService : IService {
        Task<IEnumerable<Product>> GetProducts();
        Task AddProduct(Product product);
    }
}
