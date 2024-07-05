using Abp.Application.Services.Dto;

namespace MyCompanyName.AbpZeroTemplate.Notifications.Dto
{
    public class GetAllForLookupTableInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
    }
}