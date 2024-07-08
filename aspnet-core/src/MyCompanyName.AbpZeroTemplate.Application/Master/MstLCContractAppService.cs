using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using MyCompanyName.AbpZeroTemplate.Authorization;
using MyCompanyName.AbpZeroTemplate.Master.Certificate.Dto;
using MyCompanyName.AbpZeroTemplate.Master.Certificate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyCompanyName.AbpZeroTemplate.Master.Contract;
using Microsoft.EntityFrameworkCore;
using MyCompanyName.AbpZeroTemplate.Master.Contract.Dto;
using Abp.Linq.Extensions;

namespace MyCompanyName.AbpZeroTemplate.Master
{
    [AbpAuthorize(AppPermissions.MasterContract)]
    public class MstLCContractAppService : AbpZeroTemplateAppServiceBase, IMstLCContractAppService
    {
        private readonly IRepository<MstLCContract, long> _contractRepo;
        public MstLCContractAppService(IRepository<MstLCContract, long> contractRepo)
        {
            _contractRepo = contractRepo;
        }
        [HttpGet]
        [AbpAuthorize(AppPermissions.MasterContract_Search)]
        public async Task<PagedResultDto<SearchContractOutputDto>> GetAllData(GetContractSearchInput input)
        {
            var result = from c in _contractRepo.GetAll().AsNoTracking()
                                .Where(p => string.IsNullOrWhiteSpace(input.FilterText) || p.Name.Contains(input.FilterText))
                         select new SearchContractOutputDto()
                         {
                             Id = c.Id,
                             Name = c.Name,
                         };

            var pagedAndFilteredInfo = result.PageBy(input);
            return new PagedResultDto<SearchContractOutputDto>(
                       result.Count(),
                       pagedAndFilteredInfo.ToList()
                      );
        }

        [HttpPost]
        [AbpAuthorize(AppPermissions.MasterContract_CreateOrEdit)]
        public async Task CreateOrEdit(SearchContractOutputDto dto)
        {
            var contractCheck = _contractRepo.GetAll().Where(p => p.Name.Equals(dto.Name)).FirstOrDefault();

            if (dto.Id == 0 || dto.Id.ToString() == "null")  // create New
            {
                if (contractCheck == null)
                {
                    var newContract = new MstLCContract();
                    newContract.Name = dto.Name;
                    await _contractRepo.InsertAsync(newContract);
                }
                else
                {
                    throw new UserFriendlyException("Contract that already exist, create another contract!");
                }
            }
            else // update 
            {
                var updateCategory = await _contractRepo.FirstOrDefaultAsync(e => e.Id == dto.Id);
                if (updateCategory != null)
                {
                    updateCategory.Name = dto.Name;
                }
                else
                {
                    throw new UserFriendlyException("Can not find contract");
                }
                await _contractRepo.UpdateAsync(updateCategory);
            }
        }
        [HttpDelete]
        [AbpAuthorize(AppPermissions.MasterContract_Delete)]
        public async Task Delete(long id)
        {
            await _contractRepo.DeleteAsync(id);
        }
    }
}
