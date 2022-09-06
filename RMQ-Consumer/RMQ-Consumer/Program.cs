using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Authentication;
using System.Text;
using EasyNetQ;
using EasyNetQ.Topology;

namespace RMQ_Consumer
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
                //定義 exchange
                var exchange = advancedBus.ExchangeDeclare("eventExchange", ExchangeType.Direct);
                //定義 queue
                var queue = advancedBus.QueueDeclare("rabbit mq test queue");
                //定義 routing key
                const string routingKey = "test";
                //使用 routing key 綁定 exchange 及 queue
                advancedBus.Bind(exchange, queue, routingKey, null);
                advancedBus.Consume(queue, (body, properties, info) =>
                {
                    Console.WriteLine($"dequeue start at:{DateTime.Now}");
                    var message = Encoding.UTF8.GetString(body.ToArray());
                    Console.WriteLine($"Got message: '{message}',{DateTime.Now}");
                });
            
                // 持續執行
                while (true)
                {
                }
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
            //     advancedBus.Consume(queue, (body, properties, info) =>
            //     {
            //         Console.WriteLine($"dequeue start at:{DateTime.Now}");
            //         var message = Encoding.UTF8.GetString(body.ToArray());
            //         Console.WriteLine($"Got message: '{message}',{DateTime.Now}");
            //     });
            //
            //     // 持續執行
            //     while (true)
            //     {
            //     }
            // }
            
            #endregion
        }
    }
}