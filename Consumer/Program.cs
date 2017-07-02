using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var logReceiver = new LogReceiver("localhost", "log_queue");
            logReceiver.GetLogs();

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
