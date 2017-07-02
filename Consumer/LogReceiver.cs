using Model;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Consumer
{
    public class LogReceiver
    {
        private string _hostName;
        private string _queueName;

        public LogReceiver(string hostName, string queueName)
        {
            _hostName = hostName;
            _queueName = queueName;
        }

        public List<LogModel> GetLogs()
        {
            var logs = new List<LogModel>();

            var factory = new ConnectionFactory() { HostName = _hostName };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: _queueName,
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        var log = JsonConvert.DeserializeObject<LogModel>(message);
                        logs.Add(log);

                        Console.WriteLine(message);
                    };

                    channel.BasicConsume(queue: _queueName,
                                         noAck: true,
                                         consumer: consumer);
                }
            }
            return logs;
        }
    }
}
