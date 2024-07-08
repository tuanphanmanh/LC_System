using MyCompanyName.AbpZeroTemplate.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCompanyName.AbpZeroTemplate.Master.Contract.Dto
{
    public class GetContractSearchInput : PagedAndSortedInputDto
    {
        public string FilterText { get; set; }
    }
}
