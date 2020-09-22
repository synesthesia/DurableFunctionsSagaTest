using System;
using System.Threading.Tasks;
using DurableFunctionsSagaTest.Model.Activity;
using DurableFunctionsSagaTest.Model.Domain;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace DurableFunctionsSagaTest.Activities
{
    public class ReadInvoiceFromXrmActivity
    {
        
        [FunctionName(nameof(ReadInvoiceFromXrmActivity))]
        public async Task<ActivityResult<Invoice>> Run([ActivityTrigger] Guid xrmInvoiceid, ILogger log)
        {
            log.LogInformation($"Retrieving XRM invoice data for Id {xrmInvoiceid}.");
            // simulate waiting for a remote system
            var xInvoice = await Task.Run(async () =>
            {
                await Task.Delay(1000);
                return new Invoice
                {
                    XrmInvoiceId = xrmInvoiceid, NetTotal = new decimal(134.20)
                };
            });
            // we assume for a moment that the remote system call returns our domain object
            // in a real solution there would be some mapping
            return new ActivityResult<Invoice>{Item = xInvoice, Valid = true};
        }

    }
}