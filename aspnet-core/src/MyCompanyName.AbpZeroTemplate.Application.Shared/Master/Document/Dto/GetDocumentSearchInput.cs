using MyCompanyName.AbpZeroTemplate.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyCompanyName.AbpZeroTemplate.Master.Document.Dto
{
    public class GetDocumentSearchInput : PagedAndSortedInputDto
    {
        public string FilterText { get; set; }
    }
}
