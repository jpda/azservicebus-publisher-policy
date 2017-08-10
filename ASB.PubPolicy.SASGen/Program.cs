using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus;

namespace ASB.PubPolicy.SASGen
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }

    public static class Generator
    {
        private const string Namespace = "sb://jpd-pp.servicebus.windows.net";
        private const string EventHubName = "hub";
        private const string PrimaryKey = "XnbXTS0FRCu4q8hGbvRvCuOUeJCq3f3wbRcKNKs5ofk=";

        public static string GetSas(string publisherId)
        {
            return SharedAccessSignatureTokenProvider.GetPublisherSharedAccessSignature(new Uri(Namespace), EventHubName, publisherId, "Send", PrimaryKey, new TimeSpan(0, 5, 0));
        }
    }
}
