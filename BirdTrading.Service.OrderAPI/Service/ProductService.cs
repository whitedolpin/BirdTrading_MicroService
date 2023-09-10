﻿
using BirdTrading.Service.OrderAPI.Models.DTO;
using BirdTrading.Service.OrderAPI.Service.IService;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace BirdTrading.Service.OrderAPI.Service
{
    public class ProductService : IProductService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory; 
        }

       public async Task<IEnumerable<ProductDTO>> GetProducts()
        {
            var client = _httpClientFactory.CreateClient("Product");
            var response = await client.GetAsync($"/api/product");
            var apiContent = await response.Content.ReadAsStringAsync();    
            var resp = JsonConvert.DeserializeObject<ResponseDTO>(apiContent);
            if(resp.IsSuccess)
            {
                return JsonConvert.DeserializeObject<IEnumerable<ProductDTO>>(Convert.ToString(resp.Result));
            }
            return new List<ProductDTO>();  
        }
    }
}
