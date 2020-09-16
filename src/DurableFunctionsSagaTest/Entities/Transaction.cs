using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json;

namespace DurableFunctionsSagaTest.Entities
{

    public interface ITransaction
    {
        void StartXrmInvoiceExport(Guid xrmInvoiceId);
        Task<TransactionState> GetState();
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class Transaction : ITransaction
    {
        public Transaction()
        {
            State = TransactionState.Undefined;
        }

        [JsonProperty("state")]
        public TransactionState State { get; set; }  

        [JsonProperty("start")]
        public DateTime Start { get; set; }

        // probably going to refactor this into an object to hold (eventually) all of the Xrm Invoice value
        // ReSharper disable once StringLiteralTypo
        [JsonProperty("xrminvoiceid")]
        public Guid XrmInvoiceId { get; set; }

        // possibly will add a similar object to hold the Xero invoice information

        [FunctionName(nameof(Transaction))]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx)
            => ctx.DispatchAsync<Transaction>();

        public void StartXrmInvoiceExport(Guid xrmInvoiceId)
        {
            XrmInvoiceId = xrmInvoiceId;
            Start = DateTime.UtcNow;
            State = TransactionState.XrmInvoiceExportStarted;
        }

        public async Task<TransactionState> GetState()
        {
            return State;
        }
    }

    public enum TransactionState
    {
        Undefined,
        XrmInvoiceExportStarted

    }
}
