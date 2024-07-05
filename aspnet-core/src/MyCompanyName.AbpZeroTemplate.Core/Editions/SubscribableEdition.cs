using System;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Editions;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments;

namespace MyCompanyName.AbpZeroTemplate.Editions
{
    /// <summary>
    /// Extends <see cref="Edition"/> to add subscription features.
    /// </summary>
    public class SubscribableEdition : Edition
    {
        /// <summary>
        /// The edition that will assigned after expire date
        /// </summary>
        public int? ExpiringEditionId { get; set; }

        public decimal? MonthlyPrice { get; set; }

        public decimal? AnnualPrice { get; set; }

        public int? TrialDayCount { get; set; }

        /// <summary>
        /// The account will be taken an action (termination of tenant account) after the specified days when the subscription is expired.
        /// </summary>
        public int? WaitingDayAfterExpire { get; set; }

        [NotMapped]
        public bool IsFree => !MonthlyPrice.HasValue && !AnnualPrice.HasValue;

        public bool HasTrial()
        {
            if (IsFree)
            {
                return false;
            }

            return TrialDayCount.HasValue && TrialDayCount.Value > 0;
        }

        public decimal GetPaymentAmount(PaymentPeriodType? paymentPeriodType)
        {
            var amount = GetPaymentAmountOrNull(paymentPeriodType);
            return amount ?? 0;
        }

        public decimal? GetPaymentAmountOrNull(PaymentPeriodType? paymentPeriodType)
        {
            switch (paymentPeriodType)
            {
                case PaymentPeriodType.Monthly:
                    return MonthlyPrice;
                case PaymentPeriodType.Annual:
                    return AnnualPrice;
                default:
                    return null;
            }
        }

        public string GetPlanId(PaymentPeriodType paymentPeriodType)
        {
            return Name + "_" + paymentPeriodType + "_" + AbpZeroTemplateConsts.Currency;
        }
    }
}