using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCompanyName.AbpZeroTemplate.Master.Contract.Dto
{
    public class SearchContractOutputDto : EntityDto<long>
    {
        public string Name { get; set; }
    }
}
