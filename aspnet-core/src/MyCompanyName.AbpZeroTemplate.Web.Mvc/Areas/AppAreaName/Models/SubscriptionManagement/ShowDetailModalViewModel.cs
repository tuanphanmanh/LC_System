using Abp.AutoMapper;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments.Dto;

namespace MyCompanyName.AbpZeroTemplate.Web.Areas.AppAreaName.Models.SubscriptionManagement;

[AutoMapFrom(typeof(SubscriptionPaymentProductDto))]
public class ShowDetailModalViewModel : SubscriptionPaymentProductDto
{
}