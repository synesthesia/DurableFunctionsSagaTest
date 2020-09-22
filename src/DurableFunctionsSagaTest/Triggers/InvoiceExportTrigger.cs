using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;
using DurableFunctionsSagaTest.DurableEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DurableFunctionsSagaTest.Triggers
{
    public class InvoiceExportTrigger
    {
        [FunctionName(nameof(InvoiceExportTrigger))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function,  "post", Route = "triggers/xrmexportinvoice")] HttpRequest req,
            [DurableClient] IDurableEntityClient client,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            var xrmInvoiceId = (string)(data?.xrminvoiceid);

            if (xrmInvoiceId == null)
            {
                return new BadRequestErrorMessageResult("Must have xrminvoiceid");
            }

            if (!Guid.TryParse(xrmInvoiceId, out var XrmInvoiceGuid))
            {
                return new BadRequestErrorMessageResult("xrminvoiceid must be valid Guid");
            }
            
            // this will be the unique identifier for this transaction that all subsequent code can sue to retrieve state
            var transactionId = Guid.NewGuid().ToString().Replace("-", string.Empty);
            
            // create a Durable Entity to hold transaction state
            var entityId = new EntityId(nameof(Transaction), transactionId);

            await client.SignalEntityAsync<ITransaction>(entityId, proxy => proxy.StartXrmInvoiceExport(XrmInvoiceGuid));

            // we don't trigger an orchestration, that is left to the Durable Entity class representing transaction state once it has stored what it needs to

            return new OkResult();
        }
    }
}
