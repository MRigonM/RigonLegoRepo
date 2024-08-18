using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BasketController : BaseApiController
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper mapper;

        public BasketController(IBasketRepository basketRepository,IMapper mapper){
            _basketRepository = basketRepository;
            this.mapper = mapper;
        }

        [HttpGet]

        public async Task<ActionResult<CustomerBasket>> GetBasketById(string Id){

            var basket = await _basketRepository.GetBasketAsync(Id);

            return Ok(basket ?? new CustomerBasket(Id));
        }   

        [HttpPost]

        public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasketDto basket){

            var customerBasket = mapper.Map<CustomerBasketDto,CustomerBasket>(basket);

            var updateBasket = await _basketRepository.UpdateBasketAsync(customerBasket);
            
            return Ok(updateBasket);
        }
        [HttpDelete]
        public async Task DeleteBasketAsync(string Id){
            await _basketRepository.DeleteBasketAsync(Id);
        }
        
    }
}