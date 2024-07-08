using Abp.Application.Services.Dto;
using Abp.Application.Services;
using MyCompanyName.AbpZeroTemplate.Master.Contract.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MyCompanyName.AbpZeroTemplate.Master.Document.Dto;

namespace MyCompanyName.AbpZeroTemplate.Master.Document
{
    public interface IMstLCDocumentAppService : IApplicationService
    {
        Task<PagedResultDto<SearchDocumentOutputDto>> GetAllData(GetDocumentSearchInput input);
        Task CreateOrEdit(SearchDocumentOutputDto dto);
        Task Delete(long id);
    }
}
