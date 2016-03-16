using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace Website
{
    public class JobProgressHub : Hub
    {
        private readonly static ConnectionMapping<string> Connections = new ConnectionMapping<string>();

        public override Task OnConnected()
        {
            string jobid = GetJobId();


            Connections.Add(jobid, Context.ConnectionId);

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string jobid = GetJobId();

            Connections.Remove(jobid, Context.ConnectionId);

            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            string jobId = GetJobId();

            if (!Connections.GetConnections(jobId).Contains(Context.ConnectionId))
            {
                Connections.Add(jobId, Context.ConnectionId);
            }

            return base.OnReconnected();
        }

        private string GetJobId()
        {
	        string x = Context.QueryString["jobId"];
            return x;
        }

        public static IEnumerable<string> GetUserConnections(string jobId)
        {
            return Connections.GetConnections(jobId);
        }
    }
}