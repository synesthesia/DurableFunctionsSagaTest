using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace DurableFunctionsSagaTest.DTO
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
