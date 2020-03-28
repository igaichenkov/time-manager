using System;
using System.Collections.Generic;

namespace TimeManager.Web.Models.WorkEntries
{
    public class ExportResultViewModel
    {
        public string UserName { get; set; }
        public DateTime? MinDate { get; set; }
        public DateTime? MaxDate { get; set; }
        public IEnumerable<WorkEntryDto> Entries { get; set; }
    }
}
