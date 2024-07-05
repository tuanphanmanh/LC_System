using Abp.Application.Services;
using Abp.Application.Services.Dto;
using MyCompanyName.AbpZeroTemplate.Master.Category.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyCompanyName.AbpZeroTemplate.Master.Category
{
    public interface IMstLCCategoryAppService : IApplicationService
    {
        Task<PagedResultDto<SearchCatagoryOutputDto>> GetAllData(GetCatagorySearchInput input);
        Task CreateOrEdit(SearchCatagoryOutputDto dto);
        Task Delete(long id);
    }
}
