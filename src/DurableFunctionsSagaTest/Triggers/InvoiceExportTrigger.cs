using System;
using System.IO;
using System.Threading.Tasks;
using DurableFunctionsSagaTest.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DurableFunctionsSagaTest.Triggers
{
    public static class InvoiceExportTrigger
    {
        [FunctionName(nameof(InvoiceExportTrigger))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function,  "post", Route = null)] HttpRequest req,
            [DurableClient] IDurableOrchestrationClient starter,
            [DurableClient] IDurableEntityClient client,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            var xrmInvoiceId = data?.xrminvoiceid;
            // TODO need to test for null xrmInvoiceId and return error

            // this will be the unique identifier for this transaction that all subsequent code can sue to retrieve state
            var transactionId = Guid.NewGuid().ToString().Replace("-", string.Empty);
            
            // create a Durable Entity to hold transaction state
            var entityId = new EntityId(nameof(TransactionState), transactionId);

            await client.SignalEntityAsync<ITransactionState>(entityId, proxy => proxy.StartXrmInvoiceExport(xrmInvoiceId));

            // we don't trigger an orchestration, that is left to the Durable Entity class representing transaction state once it has stored what it needs to

            return new OkResult();
        }
    }
}
