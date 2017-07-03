using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.IO;
using System.Text;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace LogMonitor
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static IConnection connection;
        private static IModel channel;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //Use a windows service to setup queue subscriber
            this.SetupRabbitMqSubscriber();
        }

        protected void Application_End()
        {
            channel.Close(200, "Goodbye!");
            connection.Close();
        }

        private void SetupRabbitMqSubscriber()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var logFilePath = string.Format("{0}logs.txt", HostingEnvironment.ApplicationPhysicalPath);

            File.WriteAllText(logFilePath, string.Empty);
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            channel.QueueDeclare(queue: "log_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                //Insert DB instead of .txt file
                File.AppendAllText(logFilePath, message + "\n");
            };
            channel.BasicConsume(queue: "log_queue", noAck: true, consumer: consumer);
        }
    }
}
