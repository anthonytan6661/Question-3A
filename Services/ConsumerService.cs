﻿using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskConsumerAPI.Models;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace TaskConsumerAPI.Services
{
    public class ConsumerService : BackgroundService
    {
        private readonly ILogger _logger;
        private IConnection _connection;
        private IModel _channel;
        private readonly RegisterTokenContext _context;

        public ConsumerService(ILoggerFactory loggerFactory, IServiceScopeFactory factory) //, RegisterTokenContext context
        {
            this._logger = loggerFactory.CreateLogger<ConsumerService>();
            this._context = factory.CreateScope().ServiceProvider.GetRequiredService<RegisterTokenContext>();
            InitRabbitMQ();
         //   _context = context;
        
        }

    

    private void InitRabbitMQ()
        {
            var factory = new ConnectionFactory
            {

                HostName = "localhost" , 
                Port = 32001
                //HostName = Environment.GetEnvironmentVariable("RABBITMQ_HOST"),
                //Port = Convert.ToInt32(Environment.GetEnvironmentVariable("RABBITMQ_PORT"))

            };

            // create connection  
            _connection = factory.CreateConnection();

            // create channel  
            _channel = _connection.CreateModel();

            //_channel.ExchangeDeclare("demo.exchange", ExchangeType.Topic);
            _channel.QueueDeclare("RegisterQueue", false, false, false, null);
            // _channel.QueueBind("demo.queue.log", "demo.exchange", "demo.queue.*", null);
            // _channel.BasicQos(0, 1, false);

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                // received message  
                var content = System.Text.Encoding.UTF8.GetString(ea.Body.ToArray());

                // handle the received message  
                HandleMessage(content);
                _channel.BasicAck(ea.DeliveryTag, false);
            };

            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

            _channel.BasicConsume("RegisterQueue", false, consumer);
            return Task.CompletedTask;
        }

        private void HandleMessage(string content)
        {
            //save to database
            var registerToken = JsonSerializer.Deserialize<RegisterToken>(content);

            var newregister = new RegisterToken();
                newregister.Id = registerToken.Id;
                newregister.token =registerToken.token;
                newregister.CreatedTime = DateTime.UtcNow;

              _context.RegisterTokens.Add(newregister);
              _context.SaveChanges();

            // we just print this message   
         

            _logger.LogInformation($"consumer received {content}");
            Console.WriteLine($"consumer received {content}");
        }

        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e) { }
        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerRegistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerShutdown(object sender, ShutdownEventArgs e) { }
        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) { }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
