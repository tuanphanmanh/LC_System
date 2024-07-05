using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Abp.Domain.Entities.Auditing;
using Abp.MultiTenancy;
using MyCompanyName.AbpZeroTemplate.ExtraProperties;

namespace MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments
{
    [Table("AppSubscriptionPayments")]
    [MultiTenancySide(MultiTenancySides.Host)]
    public class SubscriptionPayment : FullAuditedEntity<long>, IHasExtraProperties
    {
        public ICollection<SubscriptionPaymentProduct> SubscriptionPaymentProducts { get; set; }

        public SubscriptionPaymentGatewayType Gateway { get; set; }

        public SubscriptionPaymentStatus Status { get; protected set; }

        public int TenantId { get; set; }

        public int DayCount { get; set; }

        public PaymentPeriodType? PaymentPeriodType { get; set; }

        public string ExternalPaymentId { get; set; }

        public string InvoiceNo { get; set; }

        public string SuccessUrl { get; set; }

        public string ErrorUrl { get; set; }

        public bool? IsRecurring { get; set; }

        public bool IsProrationPayment { get; set; }
        
        public ExtraPropertyDictionary ExtraProperties { get; set; }

        public void SetAsCancelled()
        {
            if (Status == SubscriptionPaymentStatus.NotPaid)
            {
                Status = SubscriptionPaymentStatus.Cancelled;
            }
        }

        public void SetAsFailed()
        {
            Status = SubscriptionPaymentStatus.Failed;
        }

        public void SetAsPaid()
        {
            if (Status == SubscriptionPaymentStatus.NotPaid)
            {
                Status = SubscriptionPaymentStatus.Paid;
            }
        }

        public void SetAsCompleted()
        {
            if (Status == SubscriptionPaymentStatus.Paid)
            {
                Status = SubscriptionPaymentStatus.Completed;
            }
        }

        public SubscriptionPayment()
        {
            Status = SubscriptionPaymentStatus.NotPaid;
            SubscriptionPaymentProducts = new List<SubscriptionPaymentProduct>();
            ExtraProperties = new ExtraPropertyDictionary();
        }

        public PaymentPeriodType GetPaymentPeriodType()
        {
            switch (DayCount)
            {
                case 30:
                    return Payments.PaymentPeriodType.Monthly;
                case 365:
                    return Payments.PaymentPeriodType.Annual;
                default:
                    throw new NotImplementedException($"PaymentPeriodType for {DayCount} day could not found");
            }
        }
        
        public bool RecurringPaymentEnabled()
        {
            return IsRecurring.HasValue && IsRecurring.Value;
        }
        
        public decimal GetTotalAmount()
        {
            return SubscriptionPaymentProducts.Sum(product => product.GetTotalAmount());
        }
    }
}