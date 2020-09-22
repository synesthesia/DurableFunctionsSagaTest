using System;
using System.Collections.Generic;
using System.Text;

namespace DurableFunctionsSagaTest.Model.Orchestration
{
    public class OrchestrationParameter<T>
    {
        public string TransactionId { get; set; }
        public T Item { get; set; }

    }
}
