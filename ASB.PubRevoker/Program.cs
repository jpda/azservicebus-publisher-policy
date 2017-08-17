using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace ASB.PubRevoker
{
    class Program
    {
        static void Main(string[] args)
        {
            new TableRowRevoker().GetRevocationList();
            Console.WriteLine("All finished.");
            Console.ReadLine();
        }
    }

    public class TableRowRevoker
    {
        private const string TableConnection = "<table?";

        private NamespaceManager _ns;

        public TableRowRevoker()
        {
            _ns = NamespaceManager.CreateFromConnectionString("sb");
        }

        public void Go()
        {
            var tableClient = CloudStorageAccount.Parse(TableConnection).CreateCloudTableClient();
            var table = tableClient.GetTableReference("GeneratedEhSas");
            var things = new List<DynamicTableEntity>();

            TableContinuationToken token = null;

            var totalCount = 0;
            var failureCount = 0;

            do
            {
                var result = table.ExecuteQuerySegmentedAsync(new TableQuery(), token).Result;
                var results = result.Results.ToList();
                totalCount = totalCount + results.Count;

                try
                {
                    results.ForEach(x => _ns.RevokePublisher("hub", x.PartitionKey));
                }
                catch (Exception)
                {
                    failureCount++;
                }

                Console.CursorLeft = 0;
                Console.Write($"{totalCount} publishers revoked, {failureCount} failed...");

                token = result.ContinuationToken;
            } while (token != null);
        }

        public void GetRevocationList()
        {
            _ns.Settings.OperationTimeout = TimeSpan.FromDays(1);
            var things = _ns.GetRevokedPublishers("hub").ToList();
            Console.WriteLine($"Got {things.Count} revoked publishers from the API");
            foreach (var r in things)
            {
                Console.WriteLine($"Got revoked publisher {r.Name}");
            }
        }
    }
}
