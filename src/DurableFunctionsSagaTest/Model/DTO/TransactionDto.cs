using System;
using Newtonsoft.Json;

namespace DurableFunctionsSagaTest.Model.DTO
{
    public class TransactionDto
    {
        [JsonProperty("transactionId")]
        public string TransactionId { get; set; }

        [JsonProperty("start")]
        public DateTime Start { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        public TransactionDto(string transactionId, DateTime start, string state)
        {
            TransactionId = transactionId;
            Start = start;
            State = state;
        }
    }
}
