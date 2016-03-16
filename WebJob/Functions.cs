using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

namespace WebJob
{
	public class Functions
	{
		public static async Task ProcessQueueMessage([QueueTrigger("jobqueue")] string queue, TextWriter log)
		{


			var data = (List<JobStatusViewModel>)Newtonsoft.Json.JsonConvert.DeserializeObject(queue, typeof(List<JobStatusViewModel>));

			var jobStatusViewModel = data.FirstOrDefault();
			
			if (jobStatusViewModel != null)
			{
				string userCurrentOrdId = jobStatusViewModel.OrgIdFromClient;
				string JobId = jobStatusViewModel.JobId;


				Console.WriteLine(userCurrentOrdId);
				Console.WriteLine(JobId);

				string uiMsg = "hello. my org ID is " + userCurrentOrdId;
				
				await CommunicateProgress(JobId, uiMsg);
			}
		}

		private static async Task CommunicateProgress(string jobId, string percentage)
		{
			var httpClient = new HttpClient();

			var queryString = String.Format("?jobId={0}&progress={1}", jobId, percentage);
			var request = ConfigurationManager.AppSettings["ProgressNotificationEndpoint"] + queryString; //appsetting

			await httpClient.GetAsync(request);
		}
	}
	public class JobStatusViewModel
	{

		public string JobId { get; set; }
		public string OrgIdFromClient { get; set; }

	}
}
