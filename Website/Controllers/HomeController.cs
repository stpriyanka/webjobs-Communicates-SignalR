using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Website.Models;

namespace Website.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

		List<object> webJobQueueCollection = new List<object>(); 


        public async Task<ActionResult> CreateJob()
        {
			const string orgid = "4356";

			
            const string jobId = "userId="+"1234" +"orgId="+orgid;

			var viewModel = new JobStatusViewModel
			{
				JobId = jobId,
				OrgIdFromClient = orgid
			};

			webJobQueueCollection.Add(viewModel);




            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["QueueStorageConnectionString"].ConnectionString);
            var cloudQueueClient = storageAccount.CreateCloudQueueClient();

            var queue = cloudQueueClient.GetQueueReference("jobqueue");
            queue.CreateIfNotExists();

			CloudQueueMessage message = new CloudQueueMessage(JsonConvert.SerializeObject(webJobQueueCollection));
            await queue.AddMessageAsync(message);


            return RedirectToAction("JobStatus", new {jobId});
        }

        public ActionResult JobStatus(string jobId)
        {
            var viewModel = new JobStatusViewModel
            {
                JobId = jobId
            };

            return View(viewModel);
        }

        public ActionResult ProgressNotification(string jobId, string progress)
        {
            var connections = JobProgressHub.GetUserConnections(jobId);

            if (connections != null)
            {
                foreach (var connection in connections)
                {
                    // Notify the client to refresh the list of connections
                    var hubContext = GlobalHost.ConnectionManager.GetHubContext<JobProgressHub>();
                    hubContext.Clients.Clients(new[] { connection }).updateProgress(progress);
                }
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}