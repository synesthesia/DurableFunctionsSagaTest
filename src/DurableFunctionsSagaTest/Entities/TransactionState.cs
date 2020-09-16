using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;

namespace DurableFunctionsSagaTest.Entities
{

    public interface ITransactionState
    {
        void StartXrmInvoiceExport(Guid xrmInvoiceId);
        Task<string> GetState();
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class TransactionState : ITransactionState
    {
        public TransactionState()
        {
            State = "Undefined";
        }

        [JsonProperty("state")]
        public string State { get; set; }  // possibly refactor to enum?

        // probably going to refactor this into an object to hold (eventually) all of the Xrm Invoice value
        // ReSharper disable once StringLiteralTypo
        [JsonProperty("xrminvoiceid")]
        public Guid XrmInvoiceId { get; set; }

        // possibly will add a similar object to hold the Xero invoice information

        [FunctionName(nameof(TransactionState))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx)
            => ctx.DispatchAsync<TransactionState>();

        public void StartXrmInvoiceExport(Guid xrmInvoiceId)
        {
            XrmInvoiceId = xrmInvoiceId;
            State = "XrmInvoiceExportStarted";

        }

        public async Task<string> GetState()
        {
            return State;
        }
    }
}
