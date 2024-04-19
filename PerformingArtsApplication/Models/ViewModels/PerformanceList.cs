using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PerformingArtsApplication.Models.ViewModels
{
    public class PerformanceList
    {
        public string PageSummary { get; set; }
        public int PageNum { get; set; }
        public IEnumerable<PerformanceDto> Performances { get; set; }
    }
}