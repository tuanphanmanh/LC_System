using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCompanyName.AbpZeroTemplate.Authorization;
using MyCompanyName.AbpZeroTemplate.Master.Category;
using MyCompanyName.AbpZeroTemplate.Master.Category.Dto;
using MyCompanyName.AbpZeroTemplate.Master.Certificate;
using MyCompanyName.AbpZeroTemplate.Master.Certificate.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio.Rest.Api.V2010.Account;

namespace MyCompanyName.AbpZeroTemplate.Master
{
    [AbpAuthorize(AppPermissions.MasterCertificate)]
    public class MstLCCertificateAppService : AbpZeroTemplateAppServiceBase, IMstLCCertificateAppService
    {
        private readonly IRepository<MstLCCertificate, long> _certificateRepo;
        private readonly IRepository<MstLCCategory, long> _categoryRepo;
        public MstLCCertificateAppService(IRepository<MstLCCertificate, long> CertificateRepo, IRepository<MstLCCategory, long> categoryRepo)
        {
            _certificateRepo = CertificateRepo;
            _categoryRepo = categoryRepo;
        }
        [HttpGet]
        [AbpAuthorize(AppPermissions.MasterCertificate_Search)]
        public async Task<PagedResultDto<SearchCertificateOutputDto>> GetAllData(GetCertificateSearchInput input)
        {
            var result = from c in _certificateRepo.GetAll().AsNoTracking()
                                .Where(p => string.IsNullOrWhiteSpace(input.FilterText) || p.Name.Contains(input.FilterText) || p.CategoryId.ToString().Contains(input.FilterText))
                         select new SearchCertificateOutputDto()
                         {
                             Id = c.Id,
                             Name = c.Name,
                             CategoryId =  c.CategoryId,
                         };

            var pagedAndFilteredInfo = result.PageBy(input);
            return new PagedResultDto<SearchCertificateOutputDto>(
                       result.Count(),
                       pagedAndFilteredInfo.ToList()
                      );
        }

        [HttpPost]
        [AbpAuthorize(AppPermissions.MasterCertificate_CreateOrEdit)]
        public async Task CreateOrEdit(SearchCertificateOutputDto dto)
        {
            var certificateCheck = _certificateRepo.GetAll().Where(p => p.Name.Equals(dto.Name)).FirstOrDefault();
            if(_categoryRepo.GetAll().Where(a => a.Id == dto.CategoryId).FirstOrDefault() == null)
            {
                throw new UserFriendlyException("Invalid Category ID");
            }
            if (dto.Id == 0 || dto.Id.ToString() == "null")  // create New
            {
                if (certificateCheck == null)
                {
                    var newCategory = new MstLCCertificate();
                    newCategory.Name = dto.Name;
                    newCategory.CategoryId = dto.CategoryId;
                    await _certificateRepo.InsertAsync(newCategory);
                }
                else
                {
                    throw new UserFriendlyException("Certificate that already exist, create another certificate!");
                }
            }
            else // update 
            {
                var updateCategory = await _certificateRepo.FirstOrDefaultAsync(e => e.Id == dto.Id);
                if (updateCategory != null)
                {
                    updateCategory.Name = dto.Name;
                }
                else
                {
                    throw new UserFriendlyException("Can not find certificate");
                }
                await _certificateRepo.UpdateAsync(updateCategory);
            }
        }
        [HttpDelete]
        [AbpAuthorize(AppPermissions.MasterCertificate_Delete)]
        public async Task Delete(long id)
        {
            await _certificateRepo.DeleteAsync(id);
        }
    }
}
