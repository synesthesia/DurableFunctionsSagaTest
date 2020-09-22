using System.Threading.Tasks;
using DurableFunctionsSagaTest.Model.Activity;
using DurableFunctionsSagaTest.Model.Domain;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace DurableFunctionsSagaTest.Activities
{
    public class UpdateXrmInvoiceStatusActivity
    {
        /// <summary>
        /// A naive simulation with no error handling
        /// </summary>
        /// <param name="updatedInvoice"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName(nameof(UpdateXrmInvoiceStatusActivity))]
        public async Task<ActivityResult<Invoice>> Run([ActivityTrigger] Invoice updatedInvoice, ILogger log)
        {
            log.LogInformation($"Updating XRM invoice status for Id {updatedInvoice.XrmInvoiceId}.");
            // simulate waiting for a remote system
            var xInvoice = await Task.Run(async () =>
            {
                await Task.Delay(1000);
                var result = updatedInvoice;
                return result;
            });
            // we assume for a moment that the remote system call returns our domain object
            // in a real solution there would be some mapping
            return new ActivityResult<Invoice> { Item = xInvoice, Valid = true };
        }

    }
}