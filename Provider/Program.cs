using Model;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace Publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            var delay = 2000;
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "log_queue",
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    for (int i = 0; i < 1000; i++)
                    {
                        var log = new LogModel();
                        log.Id = i;
                        log.Time = DateTime.Now.ToString("dd.MM.yyyy hh:mm:ss");
                        log.Message = string.Format("Log Message {0}", i);

                        var logMessage = JsonConvert.SerializeObject(log);
                        var body = Encoding.UTF8.GetBytes(logMessage);

                        channel.BasicPublish(exchange: "",
                                         routingKey: "log_queue",
                                         basicProperties: null,
                                         body: body);

                        Console.WriteLine("{0} has been sent to the queue.", logMessage);
                        System.Threading.Thread.Sleep(delay);
                    }
                }
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
