using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Authentication;
using System.Text;
using EasyNetQ;
using EasyNetQ.Topology;

namespace RMQ_Producer
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            // #region TCP

            const string connectionString = "host=localhost; username=guest; password=guest; virtualHost=Test";
            using (var advancedBus = RabbitHutch
                       .CreateBus(connectionString).Advanced)
            {
                //定義 exchange (延時)
                var exchangeDelay = advancedBus.ExchangeDeclare("eventExchangeDelay",
                    cfg => cfg.AsDelayedExchange(ExchangeType.Direct));

                //定義 exchange (一般)
                var exchangeNormal = advancedBus.ExchangeDeclare("eventExchangeNormal", ExchangeType.Direct);

                //定義 queue
                var queue = advancedBus.QueueDeclare("rabbit mq test queue");

                //定義 routing key
                const string routingKey = "test";

                //使用 routing key 綁定 exchange 及 queue (延時)
                advancedBus.Bind(exchangeDelay, queue, routingKey, null);

                //使用 routing key 綁定 exchange 及 queue (一般)
                advancedBus.Bind(exchangeNormal, queue, routingKey, null);

                // publish 訊息，DeliveryMode 是用來設定 message persist (1:non-persistent;2:persistent)
                // (延時)
                var msgHeaders = new MessageProperties
                {
                    DeliveryMode = 2
                };
                msgHeaders.Headers.Add("x-delay", 5000); // 毫秒
                //發送的訊息
                var messageDelay = $"Hello World-{Guid.NewGuid()}-Delay";
                var bodyDelay = Encoding.UTF8.GetBytes(messageDelay);
                advancedBus.Publish(exchangeDelay, routingKey, false, msgHeaders, bodyDelay);

                // (一般)
                var message = $"Hello World-{Guid.NewGuid()}-Normal";
                var body = Encoding.UTF8.GetBytes(message);
                advancedBus.Publish(exchangeNormal, routingKey, false, new MessageProperties { DeliveryMode = 2 },
                    body);

                Console.WriteLine($"Send Message：{message},{DateTime.Now}");
            }

            // #endregion

            #region TLS

            // var connection = new ConnectionConfiguration
            // {
            //     Name = "rabbitMQ TLS Test",
            //     UserName = "guest",
            //     Password = "guest",
            //     VirtualHost = "Test",
            // };
            //
            // var host = new HostConfiguration
            // {
            //     Host = "localhost",
            //     Port = 5671,
            //     Ssl =
            //     {
            //         Version = SslProtocols.Tls12,
            //         Enabled = true,
            //         ServerName = "RabbitSSL",
            //         CertPath = "",
            //         CertPassphrase = "rabbit",
            //         AcceptablePolicyErrors = SslPolicyErrors.RemoteCertificateNameMismatch |
            //                                  SslPolicyErrors.RemoteCertificateChainErrors
            //     }
            // };
            //
            // connection.Hosts = new List<HostConfiguration> {host};
            //
            // using (var advancedBus = RabbitHutch.CreateBus(connection, x => { }).Advanced)
            // {
            //     //定義 exchange
            //     var exchange = advancedBus.ExchangeDeclare("eventExchange", ExchangeType.Direct);
            //     //定義 queue
            //     var queue = advancedBus.QueueDeclare("rabbit mq test queue");
            //     //定義 routing key
            //     const string routingKey = "test";
            //     //使用 routing key 綁定 exchange 及 queue
            //     advancedBus.Bind(exchange, queue, routingKey, null);
            //
            //     var message = $"Hello World-{Guid.NewGuid()}";
            //     var body = Encoding.UTF8.GetBytes(message);
            //     // publish 訊息，DeliveryMode 是用來設定 message persist (1:non-persistent;2:persistent)
            //     advancedBus.Publish(exchange, routingKey, false, new MessageProperties {DeliveryMode = 2}, body);
            //     Console.WriteLine($"Send Message：{message},{DateTime.Now}");
            // }

            #endregion
        }
    }
}