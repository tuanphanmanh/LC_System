using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCompanyName.AbpZeroTemplate.Master.Document.Dto
{
    public class SearchDocumentOutputDto : EntityDto<long>
    {
        public string Name { get; set; }
    }
}
