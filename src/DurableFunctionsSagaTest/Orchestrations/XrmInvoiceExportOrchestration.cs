using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using DurableFunctionsSagaTest.Entities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;


namespace DurableFunctionsSagaTest.Orchestrations
{
    public static class XrmInvoiceExportOrchestration
    {
        [FunctionName(nameof(XrmInvoiceExportOrchestration))]
        public static async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var transactionId = context.GetInput<string>();

            // get proxy to durable transaction state
            var entityId = new EntityId(nameof(TransactionState), transactionId);
            var proxy = context.CreateEntityProxy<ITransactionState>(entityId);

            var currentState = await proxy.GetState();
            if (currentState != "XrmInvoiceExportStarted")
            {
                throw new InvalidOperationException("Wrong state");
            }

           // add some calls her eto pull the Xrm invoice Id from the entity

           // then call an activity function to read the full invoice and it's lines from XRM
           // and update into state
           // and change the state CurrentState

           // then call activity function(s) to build the Xero invoice and any othe rrecords
           // keep updating state as you go along

           // then call an activity function to update status in XRM

           // at the end set the stat et completed

           // and possibly return something from this.


        }

        
    }
}