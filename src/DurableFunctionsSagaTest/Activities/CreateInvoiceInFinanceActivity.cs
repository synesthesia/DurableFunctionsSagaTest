using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DurableFunctionsSagaTest.Model.Activity;
using DurableFunctionsSagaTest.Model.Domain;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace DurableFunctionsSagaTest.Activities
{
    public class CreateInvoiceInFinanceActivity
    {
        /// <summary>
        /// A naive simulation with no error handling
        /// </summary>
        /// <param name="inputInvoice"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName(nameof(CreateInvoiceInFinanceActivity))]
        public async Task<ActivityResult<Invoice>> Run([ActivityTrigger] Invoice inputInvoice, ILogger log)
        {
            log.LogInformation($"Creating finance invoice based on input invoice Id {inputInvoice.XrmInvoiceId}.");
            // simulate waiting for a remote system
            var fInvoice = await Task.Run(async () =>
            {
                await Task.Delay(1000);
                var result = inputInvoice;
                inputInvoice.FinanceInvoiceId = Guid.NewGuid();
                return result;

            });
            // we assume for a moment that the remote system call returns our domain object
            // in a real solution there would be some mapping
            return new ActivityResult<Invoice> { Item = fInvoice, Valid = true };
        }
    }
}
