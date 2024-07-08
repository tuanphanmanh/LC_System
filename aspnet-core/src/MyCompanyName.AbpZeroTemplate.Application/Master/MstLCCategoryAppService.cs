using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using AutoMapper.Internal.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCompanyName.AbpZeroTemplate.Authorization;
using MyCompanyName.AbpZeroTemplate.Master.Category;
using MyCompanyName.AbpZeroTemplate.Master.Category.Dto;
using MyCompanyName.AbpZeroTemplate.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCompanyName.AbpZeroTemplate.Master
{
    [AbpAuthorize(AppPermissions.MasterCategory)]
    public class MstLCCategoryAppService: AbpZeroTemplateAppServiceBase, IMstLCCategoryAppService
    {
        private readonly IRepository<MstLCCategory, long> _categoryRepo;
         public MstLCCategoryAppService(IRepository<MstLCCategory, long> CategoryRepo)
            {
            _categoryRepo = CategoryRepo;
            }
        [HttpGet]
        [AbpAuthorize(AppPermissions.MasterCategory_Search)]
        public async Task<PagedResultDto<SearchCatagoryOutputDto>> GetAllData(GetCatagorySearchInput input)
        {
            var result = from c in _categoryRepo.GetAll().AsNoTracking()
                                .Where(p => string.IsNullOrWhiteSpace(input.FilterText) || p.Name.Contains(input.FilterText))
                         select new SearchCatagoryOutputDto()
                         {
                             Id = c.Id,
                             Name = c.Name,
                         };

            var pagedAndFilteredInfo = result.PageBy(input);
            return new PagedResultDto<SearchCatagoryOutputDto>(
                       result.Count(),
                       pagedAndFilteredInfo.ToList()
                      );
        }

        [HttpPost]
        [AbpAuthorize(AppPermissions.MasterCategory_CreateOrEdit)]
        public async Task CreateOrEdit(SearchCatagoryOutputDto dto)
        {
            var CategoryCheck = _categoryRepo.GetAll().Where(p => p.Name.Equals(dto.Name)).FirstOrDefault();

            if (dto.Id == 0 || dto.Id.ToString() == "null")  // create New
            {
                if (CategoryCheck == null)
                {
                    var newCategory = new MstLCCategory();
                    newCategory.Name = dto.Name;
                    await _categoryRepo.InsertAsync(newCategory);
                }
                else
                {
                    throw new UserFriendlyException("Category that already exist, create another category!");
                }
            }
            else // update 
            {
                var updateCategory = await _categoryRepo.FirstOrDefaultAsync(e => e.Id == dto.Id);
                if (updateCategory != null)
                {
                    updateCategory.Name = dto.Name;
                }
                else
                {
                    throw new UserFriendlyException("Can not find category");
                }

                await _categoryRepo.UpdateAsync(updateCategory);

            }
        }
        [HttpDelete]
        [AbpAuthorize(AppPermissions.MasterCategory_Delete)]
        public async Task Delete(long id)
        {
            await _categoryRepo.DeleteAsync(id);
        }
    }
}
