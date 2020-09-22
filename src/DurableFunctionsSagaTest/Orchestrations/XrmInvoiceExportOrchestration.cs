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
            // note naivety of not setting timeouts or handling errors

            var xrmInvoiceId = parameters.Item;
            var readInvoiceResult =
                await context.CallActivityAsync<ActivityResult<Invoice>>(nameof(ReadInvoiceFromXrmActivity),
                    xrmInvoiceId);
            proxy.UpdateState(TransactionState.XrmInvoiceRetrieved);


            // for our naive simulation let's assume that all we need to do now is create the invoice in the destination
            // i.e. no steps needed to check that the customer exists in that system etc
            // so call an activity function to read the full invoice and it's lines from XRM
            // and change the state 

            // again note naivety of not setting timeouts or handling errors
            var createFinanceInvoiceResult =
                await context.CallActivityAsync<ActivityResult<Invoice>>(nameof(CreateInvoiceInFinanceActivity),
                    readInvoiceResult.Item);
            proxy.UpdateState(TransactionState.FinanceInvoiceCreated);


            // then call an activity function to update status in XRM

            var finalInvoiceResult =
                await context.CallActivityAsync<ActivityResult<Invoice>>(nameof(UpdateXrmInvoiceStatusActivity),
                    createFinanceInvoiceResult.Item);

            // at the end set the state completed
            proxy.UpdateState(TransactionState.InvoiceExported);

            // and possibly return something from this.


        }

        
    }
}