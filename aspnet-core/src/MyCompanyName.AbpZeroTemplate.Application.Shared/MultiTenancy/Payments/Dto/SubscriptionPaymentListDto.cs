using System.Collections.Generic;
using System.Linq;
using Abp.Application.Services.Dto;

namespace MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments.Dto
{
    public class SubscriptionPaymentListDto : AuditedEntityDto
    {
        public string Gateway { get; set; }

        public int DayCount { get; set; }

        public string PaymentPeriodType { get; set; }

        public string ExternalPaymentId { get; set; }

        public string PayerId { get; set; }

        public string Status { get; set; }

        public int TenantId { get; set; }

        public string InvoiceNo { get; set; }

        public decimal TotalAmount
        {
            get { return SubscriptionPaymentProducts.Sum(p => p.Count * p.Amount); }
        }

        public List<SubscriptionPaymentProductDto> SubscriptionPaymentProducts { get; set; }
    }
}