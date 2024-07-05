using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCompanyName.AbpZeroTemplate.Master.Category.Dto
{
    public class SearchCatagoryOutputDto : EntityDto<long>
    {
        public string Name { get; set; }
    }
}
