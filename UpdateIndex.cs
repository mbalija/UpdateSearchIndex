using System;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Search;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace UpdateSearchIndex
{
    public static class UpdateIndex
    {
        //private static string searchServiceName = ConfigurationManager.AppSettings["SearchServiceName"];
        //private static string searchServiceKey = ConfigurationManager.AppSettings["SearchServiceKey"];
        
        private static string searchServiceName = "ma-cosmosdbsearch";
        private static string searchServiceKey = "A3C3D5F11880F2F040FE01817DEDA4B9";

        private static SearchServiceClient serviceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(searchServiceKey));
        private static ISearchIndexClient indexClient = serviceClient.Indexes.GetClient("cosmosdb-index");// ConfigurationManager.AppSettings["SearchServiceIndexName"]);

        [FunctionName("UpdateIndex")]
        public static async Task Run([CosmosDBTrigger(
            databaseName: "TwittterLab",
            collectionName: "twitterphotos",
            ConnectionStringSetting = "DB_CONNECTION",
            LeaseCollectionName = "leases", CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> documents, ILogger log)
        /*
        {
            if (documents != null && documents.Count > 0)
            {
                log.LogInformation("Documents modified " + documents.Count);
                log.LogInformation("First document Id " + documents[0].Id);
            }
        }//*/
        //*
        {
            Console.WriteLine("Documents modified " + documents.Count);
            log.LogInformation("Documents modified " + documents.Count);
            if (documents != null && documents.Count > 0)
            {
                var batch = Microsoft.Azure.Search.Models.IndexBatch.MergeOrUpload
                (
                   documents.Select
                   (
                        // Do any transformation needed
                        doc => new IndexItem()
                        {
                            id = doc.GetPropertyValue<string>("id"),
                            caption = doc.GetPropertyValue<string>("caption"),
                            photoId = doc.GetPropertyValue<string>("photoId"),
                            url = doc.GetPropertyValue<string>("url"),
                            userId = doc.GetPropertyValue<string>("userId")
                        }
                    )
                );
                try
                {
                    await indexClient.Documents.IndexAsync(batch);
                }
                catch (IndexBatchException e)
                {
                    // Sometimes when your Search service is under load, indexing will fail for some of the documents in
                    // the batch. Depending on your application, you can take compensating actions like delaying and
                    // retrying. For this simple demo, we just log the failed document keys and continue.            
                    log.LogError(
                        string.Format("Failed to index some of the documents: {0}",
                        String.Join(", ", e.IndexingResults.Where(r => !r.Succeeded).Select(r => r.Key))));
                    log.LogError(e.Message);
                }
            }
        }
        //*/
    }
}
