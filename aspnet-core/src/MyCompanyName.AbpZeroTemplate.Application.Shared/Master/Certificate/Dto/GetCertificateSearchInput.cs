using MyCompanyName.AbpZeroTemplate.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCompanyName.AbpZeroTemplate.Master.Certificate.Dto
{
    public class GetCertificateSearchInput : PagedAndSortedInputDto
    {
        public string FilterText { get; set; }
    }
}
