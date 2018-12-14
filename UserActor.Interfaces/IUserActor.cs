using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;

namespace UserActor.Interfaces {
    public interface IUserActor : IActor {
        Task AddToBasket(Guid productID, int quantity);
        Task<Dictionary<Guid, int>> GetBasket();
        Task ClearBasket();
    }
}
