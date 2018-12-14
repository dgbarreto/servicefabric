using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;

namespace ProductService {
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class ProductService : StatefulService, IProductService {
        public ProductService(StatefulServiceContext context)
            : base(context) { }

        public async Task AddProduct(Product product) {
            var products = await this.StateManager.GetOrAddAsync<IReliableDictionary<Guid, Product>>("products");
            
            using (ITransaction transaction = this.StateManager.CreateTransaction()) {
                await products.AddOrUpdateAsync(transaction, product.ID, product, (id, value) => product);
                await transaction.CommitAsync();
            }
        }

        public async Task<IEnumerable<Product>> GetProducts() {
            var products = await this.StateManager.GetOrAddAsync<IReliableDictionary<Guid, Product>>("products");
            var output = new List<Product>();

            using (ITransaction transaction = this.StateManager.CreateTransaction()) {
                var allProducts = await products.CreateEnumerableAsync(transaction, EnumerationMode.Unordered);

                using (var enumerator = allProducts.GetAsyncEnumerator()) {
                    while (await enumerator.MoveNextAsync(CancellationToken.None)) {
                        KeyValuePair<Guid, Product> current = enumerator.Current;
                        output.Add(current.Value);
                    }
                }
            }

            return output;
        }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners() {
            return this.CreateServiceRemotingReplicaListeners();
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken) {
        }
    }
}
