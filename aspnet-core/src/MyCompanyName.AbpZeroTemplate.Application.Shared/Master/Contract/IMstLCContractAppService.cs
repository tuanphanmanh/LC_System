using Abp.Application.Services.Dto;
using Abp.Application.Services;
using MyCompanyName.AbpZeroTemplate.Master.Contract.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyCompanyName.AbpZeroTemplate.Master.Contract
{ 
    public interface IMstLCContractAppService : IApplicationService
    {
        Task<PagedResultDto<SearchContractOutputDto>> GetAllData(GetContractSearchInput input);
        Task CreateOrEdit(SearchContractOutputDto dto);
        Task Delete(long id);
    }
}
