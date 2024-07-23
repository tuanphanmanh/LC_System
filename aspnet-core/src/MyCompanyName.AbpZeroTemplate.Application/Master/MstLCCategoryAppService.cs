using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore;
using Abp.Linq.Extensions;
using Abp.UI;
using AutoMapper.Internal.Mappers;
using GemBox.Spreadsheet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCompanyName.AbpZeroTemplate.Authorization;
using MyCompanyName.AbpZeroTemplate.Common;
using MyCompanyName.AbpZeroTemplate.Master.Category;
using MyCompanyName.AbpZeroTemplate.Master.Category.Dto;
using MyCompanyName.AbpZeroTemplate.Storage;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tmss.Common;

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
                                orderby c.Name
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
            if (dto.Id == 0 || dto.Id == null)  // create New
            {
                if (CategoryCheck == null)
                {
                    var newCategory = new MstLCCategory();
                    newCategory.Name = FormatCommon.ReplaceHtmlCode(dto.Name);
                    await _categoryRepo.InsertAsync(newCategory);
                }
                else
                {
                    throw new UserFriendlyException(400, L("Category that already exist, create another category!"));
                }
            }
            else // update 
            {
                var updateCategory = await _categoryRepo.FirstOrDefaultAsync(e => e.Id == dto.Id);
                if (updateCategory != null)
                {
                    updateCategory.Name = FormatCommon.ReplaceHtmlCode(dto.Name);
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
            var deleteCate = _categoryRepo.GetAll().Where(p => p.Id == id).FirstOrDefault();
            if(deleteCate != null)
            {
                await _categoryRepo.DeleteAsync(deleteCate);
            }
            else
            {
                throw new UserFriendlyException("Can not find category for delete");
            }
        }

        public async Task<byte[]> MstNpuBrandExportExcel(InputMstLCCategoryExportDto input)
        {
            var brandList = from item in _categoryRepo.GetAll().AsNoTracking()
                            where ((string.IsNullOrWhiteSpace(input.CategoryCode) || item.Name.Contains(input.CategoryCode)))
                            select new SearchCatagoryOutputDto
                            {
                                Id = item.Id,
                                Name = item.Name,
                            };
            var result = brandList.ToList();
            SpreadsheetInfo.SetLicense("EF21-1FW1-HWZF-CLQH");
            var xlWorkBook = new ExcelFile();
            var v_worksheet = xlWorkBook.Worksheets.Add("Book1");

            var v_list_export_excel = result.ToList();
            List<string> list = new List<string>();
            list.Add("CharacteristicsValue");
            list.Add("CharacteristicsValueDescription");
            list.Add("ManufPlanName");
            list.Add("ProductGroupName");

            List<string> listHeader = new List<string>();
            listHeader.Add("Characteristics Value");
            listHeader.Add("Characteristics Value Description");
            listHeader.Add("Manuf Plan Name");
            listHeader.Add("Product Group Name");

            string[] properties = list.ToArray();
            string[] p_header = listHeader.ToArray();
            Commons.FillExcel(v_list_export_excel, v_worksheet, 1, 0, properties, p_header);

            var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".xlsx");
            xlWorkBook.Save(tempFile);
            var tempFile2 = Commons.SetAutoFit(tempFile, p_header.Length);
            byte[] fileByte = await File.ReadAllBytesAsync(tempFile2);
            File.Delete(tempFile);
            File.Delete(tempFile2);
            return fileByte;
        }
    }
}
