using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using UserActor.Interfaces;
using WebAPI.Model;

namespace WebAPI.Controllers {
    [Route("api/basket")]
    public class BasketController : Controller {
        [HttpGet("get/{userId}")]
        public async Task<ApiBasket> Get(string userId) {
            IUserActor actor = GetActor(userId);

            Dictionary<Guid, int> products = await actor.GetBasket();

            return new ApiBasket() {
                UserId = userId,
                Items = products.Select(s => new ApiBasketItem {
                    ProductId = s.Key.ToString(),
                    Quantity = s.Value
                }).ToArray()
            };
        }

        [HttpPost("add/{userId}")]
        public async Task Add(string userId, [FromBody] ApiBasketAddRequest request) {
            IUserActor actor = GetActor(userId);

            await actor.AddToBasket(request.ProductId, request.Quantity);
        }

        [HttpDelete("delete/{userId}")]
        public async Task Delete(string userId) {
            IUserActor actor = GetActor(userId);

            await actor.ClearBasket();
        }

        private IUserActor GetActor(string userId) {
            return ActorProxy.Create<IUserActor>(new ActorId(userId), new Uri("fabric:/ECommerce/UserActorService"));
        }
    }
}