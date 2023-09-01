﻿using Azure.Messaging.ServiceBus;
using BirdTrading.Service.EmailAPI.Models.DTO;
using BirdTrading.Service.EmailAPI.Service;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json.Serialization;

namespace BirdTrading.Service.EmailAPI.Messaging
{
    public class AzureServiceBusConsumer : IAzureServiceBusConsumer
    {
        private readonly string serviceBusConnectionString;
        private readonly string emailCartQueue;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;

        private ServiceBusProcessor _emailCartProcessor;

        public AzureServiceBusConsumer(IConfiguration configuration, EmailService emailService)
        {

            _configuration = configuration;
            serviceBusConnectionString = _configuration.GetValue<String>("ServiceBusConnectionString");

            emailCartQueue = _configuration.GetValue<String>("TopicAndQueueNames:EmailShoppingCartQueue");

            var client = new ServiceBusClient(serviceBusConnectionString);

            _emailCartProcessor = client.CreateProcessor(emailCartQueue);
            _emailService = emailService;   
        }

        public async Task Start()
        {
            _emailCartProcessor.ProcessMessageAsync += OnEmailCartRequestReceived;
            _emailCartProcessor.ProcessErrorAsync += ErrorHandler;
            await _emailCartProcessor.StartProcessingAsync();   
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }

        private async Task OnEmailCartRequestReceived(ProcessMessageEventArgs args)
        {
            //this is where u received a message
            var message = args.Message;
            var body = Encoding.UTF8.GetString(message.Body);

            CartDTO objMessage = JsonConvert.DeserializeObject<CartDTO>(body);
            try
            {
               await  _emailService.EmailCartAndLog(objMessage);
                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task Stop()
        {
           await _emailCartProcessor.StopProcessingAsync();
           await _emailCartProcessor.DisposeAsync();
        }
    }
}
