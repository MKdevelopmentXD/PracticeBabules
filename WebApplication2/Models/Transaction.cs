using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string Note { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public int CategoryId { get; set; }
        public string Type { get; set; }

        public Category Category { get; set; }
    }
}
