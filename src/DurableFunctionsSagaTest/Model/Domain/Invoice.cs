using System;
using System.Collections.Generic;
using System.Text;

namespace DurableFunctionsSagaTest.Model.Domain
{
    /// <summary>
    /// Represents an abstract view of an invoice
    /// </summary>
    public class Invoice
    {
        public Guid XrmInvoiceId { get; set; }
        public decimal NetTotal { get; set; }
    }
}
