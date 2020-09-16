using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DurableFunctionsSagaTest.DTO;
using DurableFunctionsSagaTest.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace DurableFunctionsSagaTest.Triggers
{
    public class GetTransactions
    {
        [FunctionName(nameof(GetTransactions))]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "transactions")] HttpRequest req,
            [DurableClient] IDurableEntityClient client,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");


            var transactions = new List<TransactionDto>();
            using CancellationTokenSource source = new CancellationTokenSource();
            var token = source.Token;

            var query = new EntityQuery
            {
                PageSize = 100,
                FetchState = true,
                EntityName = nameof(Transaction)
            };

            do
            {
                var result = await client.ListEntitiesAsync(query, token);
                if (null == result?.Entities)
                    break;

                foreach (var e in result.Entities)
                {
                    if (null == e.State)
                        continue;

                    try
                    {
                        var c = e.State.ToObject<Transaction>();
                        var state = Enum.GetName(typeof(TransactionState), c.State);
                        transactions.Add(new TransactionDto(e.EntityId.EntityKey, c.Start, state));
                    }
                    catch
                    {
                        // logging
                    }
                }

                query.ContinuationToken = result.ContinuationToken;
            }
            while (query.ContinuationToken != null);

            return new OkObjectResult(transactions.OrderByDescending(c => c.Start));
        }
    }
}
