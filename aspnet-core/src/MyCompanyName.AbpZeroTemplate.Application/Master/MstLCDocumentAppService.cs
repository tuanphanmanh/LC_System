using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Microsoft.AspNetCore.Mvc;
using MyCompanyName.AbpZeroTemplate.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyCompanyName.AbpZeroTemplate.Master.Document;
using MyCompanyName.AbpZeroTemplate.Master.Document.Dto;
using Abp.Linq.Extensions;

namespace MyCompanyName.AbpZeroTemplate.Master
{
    [AbpAuthorize(AppPermissions.MasterDocument)]
    public class MstLCDocumentAppService : AbpZeroTemplateAppServiceBase, IMstLCDocumentAppService
    {
        private readonly IRepository<MstLCDocument, long> _documentRepo;
        public MstLCDocumentAppService(IRepository<MstLCDocument, long> documentRepo)
        {
            _documentRepo = documentRepo;
        }
        [HttpGet]
        [AbpAuthorize(AppPermissions.MasterDocument_Search)]
        public async Task<PagedResultDto<SearchDocumentOutputDto>> GetAllData(GetDocumentSearchInput input)
        {
            var result = from c in _documentRepo.GetAll().AsNoTracking()
                                .Where(p => string.IsNullOrWhiteSpace(input.FilterText) || p.Name.Contains(input.FilterText)  )
                         select new SearchDocumentOutputDto()
                         {
                             Id = c.Id,
                             Name = c.Name,
                         };

            var pagedAndFilteredInfo = result.PageBy(input);
            return new PagedResultDto<SearchDocumentOutputDto>(
                       result.Count(),
                       pagedAndFilteredInfo.ToList()
                      );
        }
        [HttpPost]
        [AbpAuthorize(AppPermissions.MasterDocument_CreateOrEdit)]
        public async Task CreateOrEdit(SearchDocumentOutputDto dto)
        {
            var documentCheck = _documentRepo.GetAll().Where(p => p.Name.Equals(dto.Name)).FirstOrDefault();

            if (dto.Id == 0 || dto.Id.ToString() == "null")   
            {
                if (documentCheck == null)
                {
                    var newContract = new MstLCDocument();
                    newContract.Name = dto.Name;
                    await _documentRepo.InsertAsync(newContract);
                }
                else
                {
                    throw new UserFriendlyException("Document that already exist, create another document!");
                }
            }
            else // update 
            {
                var updateDocument = await _documentRepo.FirstOrDefaultAsync(e => e.Id == dto.Id);
                if (updateDocument != null)
                {
                    updateDocument.Name = dto.Name;
                }
                else
                {
                    throw new UserFriendlyException("Can not find document");
                }
                await _documentRepo.UpdateAsync(updateDocument);
            }
        }
        [HttpDelete]
        [AbpAuthorize(AppPermissions.MasterDocument_Delete)]
        public async Task Delete(long id)
        {
            await _documentRepo.DeleteAsync(id);
        }
    }
}
