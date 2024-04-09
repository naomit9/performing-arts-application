using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PerformingArtsApplication.Models.ViewModels
{
    public class DetailsShowcase
    {
        public ShowcaseDto SelectedShowcase { get; set; }
        public IEnumerable<PerformanceDto> ShowcasePerformances { get; set; }
        public IEnumerable<PerformanceDto> AvailablePerformances { get; set; }
    }
}