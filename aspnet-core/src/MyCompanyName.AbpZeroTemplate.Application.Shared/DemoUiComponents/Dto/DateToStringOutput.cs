using System;

namespace MyCompanyName.AbpZeroTemplate.DemoUiComponents.Dto
{
    public class DateFieldOutput
    {
        public DateTime Date { get; set; }
    }
    
    public class DateRangeFieldOutput
    {
        public DateTime StartDate { get; set; }
        
        public DateTime EndDate { get; set; }
    }
}