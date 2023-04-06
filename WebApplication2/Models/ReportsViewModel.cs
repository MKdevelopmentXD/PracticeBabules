using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Models
{
    public class ReportsViewModel
    {
        public string ReportType { get; set; }
        public DateTime Month { get; set; }
        public List<CategoryViewModel> Expenses { get; set; }
        public List<CategoryViewModel> Revenues { get; set; }
    }
}
