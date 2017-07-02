using Consumer;
using Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;

namespace LogMonitor.Controllers
{
    public class HomeController : Controller
    {
        LogReceiver _logReceiver;

        public HomeController()
        {
            _logReceiver = new LogReceiver("localhost", "log_queue");
        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult FetchLogs()
        {
            var logModels = new List<LogModel>();
            var logs = System.IO.File.ReadAllLines(HostingEnvironment.ApplicationPhysicalPath + "logs.txt");

            foreach (var log in logs)
            {
                logModels.Add(JsonConvert.DeserializeObject<LogModel>(log));
            }

            return new JsonResult
            {
                Data = logModels.OrderByDescending(o => o.Id),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}