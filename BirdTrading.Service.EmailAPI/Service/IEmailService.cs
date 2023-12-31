﻿using BirdTrading.Service.EmailAPI.Message;
using BirdTrading.Service.EmailAPI.Models.DTO;

namespace BirdTrading.Service.EmailAPI.Service
{
    public interface IEmailService
    {
        Task EmailCartAndLog(CartDTO cartDTO);
        Task RegisterUserEmailAndLog(string email);
        Task LogOrderPlaced(RewardsMessage rewardsDTO);
    }
}
