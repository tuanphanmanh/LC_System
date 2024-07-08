using Abp.Application.Services.Dto;
using Abp.Application.Services;
using MyCompanyName.AbpZeroTemplate.Master.Category.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MyCompanyName.AbpZeroTemplate.Master.Certificate.Dto;

namespace MyCompanyName.AbpZeroTemplate.Master.Certificate
{
    public interface IMstLCCertificateAppService : IApplicationService
    {
        Task<PagedResultDto<SearchCertificateOutputDto>> GetAllData(GetCertificateSearchInput input);
        Task CreateOrEdit(SearchCertificateOutputDto dto);
        Task Delete(long id);
    }
}
