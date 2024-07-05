using MyCompanyName.AbpZeroTemplate.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCompanyName.AbpZeroTemplate.Master.Category.Dto
{
    public class GetCatagorySearchInput : PagedAndSortedInputDto
    {
        public string FilterText { get; set; }
    }
}
