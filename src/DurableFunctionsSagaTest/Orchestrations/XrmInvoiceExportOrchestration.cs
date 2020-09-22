using System;
using System.Threading.Tasks;
using DurableFunctionsSagaTest.Activities;
using DurableFunctionsSagaTest.DurableEntities;
using DurableFunctionsSagaTest.Model.Activity;
using DurableFunctionsSagaTest.Model.Domain;
using DurableFunctionsSagaTest.Model.Orchestration;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;


namespace DurableFunctionsSagaTest.Orchestrations
{
    public class XrmInvoiceExportOrchestration
    {
        [FunctionName(nameof(XrmInvoiceExportOrchestration))]
        public static async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var parameters = context.GetInput<OrchestrationParameter<Guid>>();

            var transactionId = parameters.TransactionId;
            // get proxy to durable transaction state
            var entityId = new EntityId(nameof(Transaction), transactionId);
            var proxy = context.CreateEntityProxy<ITransaction>(entityId);

            var currentState = await proxy.GetState();
            if (currentState != TransactionState.XrmInvoiceExportStarted)
            {
                throw new InvalidOperationException("Wrong state");
            }


            // call an activity function to read the full invoice and it's lines from XRM
            // and change the state 

            var xrmInvoiceId = parameters.Item;
            var readInvoiceResult =
                await context.CallActivityAsync<ActivityResult<Invoice>>(nameof(ReadInvoiceFromXrmActivity),
                    xrmInvoiceId);
            proxy.UpdateState(TransactionState.XrmInvoiceRetrieved);

            // then call an activity function to read the full invoice and it's lines from XRM
            // and change the state CurrentState

            // then call activity function(s) to build the Xero invoice and any othe rrecords
            // keep updating state as you go along

            // then call an activity function to update status in XRM

            // at the end set the stat et completed

            // and possibly return something from this.


        }

        
    }
}