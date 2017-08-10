using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace ASB.PubPolicy
{
    class Program
    {
        static void Main(string[] args)
        {
            var eps = "<SAS gen endpoint>";

            var sb = "<service bus endpoint>";
            var hub = "hub";
            var iterations = 1000000;

            Console.WriteLine("Running...");
            try
            {
                Parallel.For(0, iterations, i =>
                {
                    var sas = new SasPublisherClient(eps, $"sas-{i}-{DateTime.UtcNow.Ticks}", sb, hub);
                    //Console.WriteLine($"=== Creating publisher {i} of {iterations} ===");
                    sas.Send("hey");
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.WriteLine("All finished.");
            Console.ReadLine();
        }
    }

    public class SasPublisherClient
    {
        private readonly string _endpointService;
        private readonly string _publisherName;
        private readonly string _sbNamespace;
        private readonly string _hubName;

        public SasPublisherClient(string endpointService, string publisherName, string sbNamespace, string hubName)
        {
            _endpointService = endpointService;
            _publisherName = publisherName;
            _sbNamespace = sbNamespace;
            _hubName = hubName;
        }

        private string GetSas()
        {
            //Console.WriteLine($"Getting SAS for {_publisherName}...");
            var url = $"{_endpointService}&publisherId={_publisherName}";
            var nc = new WebClient();
            return nc.DownloadString(url).Trim('"');
        }

        public void Send(string a)
        {
            var sas = GetSas();
            //Console.WriteLine("Creating EHSender");
            var xon = ServiceBusConnectionStringBuilder.CreateUsingSharedAccessSignature(new Uri(_sbNamespace), _hubName, _publisherName, sas);
            var c = EventHubSender.CreateFromConnectionString(xon);
            c.SendAsync(new EventData(System.Text.Encoding.UTF8.GetBytes($"{_publisherName} says {a}")));
            //Console.WriteLine("Sent.");
        }
    }
}
