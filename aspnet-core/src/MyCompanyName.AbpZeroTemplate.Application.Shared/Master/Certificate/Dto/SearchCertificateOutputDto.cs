using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCompanyName.AbpZeroTemplate.Master.Certificate.Dto
{
    public class SearchCertificateOutputDto : EntityDto<long>
    {
        public string Name { get; set; }
        public long CategoryId { get; set; }

    }
}
