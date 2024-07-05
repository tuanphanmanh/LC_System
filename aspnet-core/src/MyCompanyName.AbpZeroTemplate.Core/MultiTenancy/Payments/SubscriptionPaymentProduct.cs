using System.ComponentModel.DataAnnotations.Schema;
using Abp;
using Abp.Domain.Entities.Auditing;
using Abp.MultiTenancy;
using JetBrains.Annotations;
using MyCompanyName.AbpZeroTemplate.ExtraProperties;

namespace MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments
{
    [Table("AppSubscriptionPaymentProducts")]
    [MultiTenancySide(MultiTenancySides.Host)]
    public class SubscriptionPaymentProduct : FullAuditedEntity<long>, IHasExtraProperties
    {
        public long SubscriptionPaymentId { get; set; }

        public string Description { get; set; }

        public decimal Amount { get; set; }

        public int Count { get; private set; }

        public decimal TotalAmount { get; private set; }

        public ExtraPropertyDictionary ExtraProperties { get; set; }

        protected SubscriptionPaymentProduct()
        {
            ExtraProperties = new ExtraPropertyDictionary();
        }

        public SubscriptionPaymentProduct(
            long subscriptionPaymentId,
            [NotNull] string description,
            decimal? amount = null,
            int count = 1,
            decimal? totalAmount = null,
            ExtraPropertyDictionary extraProperties = null) :
            this(description, amount, count, totalAmount, extraProperties)
        {
            SubscriptionPaymentId = subscriptionPaymentId;
        }

        public SubscriptionPaymentProduct(
            [NotNull] string description,
            decimal? amount = null,
            int count = 1,
            decimal? totalAmount = null,
            ExtraPropertyDictionary extraProperties = null)
        {
            Description = Check.NotNullOrWhiteSpace(description, nameof(description));

            Amount = amount.Value;
            Count = count;
            TotalAmount = totalAmount ?? (amount.Value * Count);
            ExtraProperties = extraProperties ?? new ExtraPropertyDictionary();
        }

        public decimal GetTotalAmount()
        {
            return Amount * Count;
        }
    }
}