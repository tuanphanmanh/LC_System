using System;
using System.Linq;
using MyCompanyName.AbpZeroTemplate.ExtraProperties;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments;

namespace MyCompanyName.AbpZeroTemplate.MultiTenancy.Subscription
{
    public static class SubscriptionPaymentExtensions
    {
        public static int GetTargetEditionId(this SubscriptionPayment payment)
        {
            var product = payment.SubscriptionPaymentProducts.First();
            return product.GetProperty(PaymentConsts.TargetEditionId, 0);
        }

        public static string GetPlanId(this SubscriptionPayment payment)
        {
            if (!payment.IsRecurring.HasValue || !payment.IsRecurring.Value)
            {
                throw new ApplicationException("Only recurring payment has plan name!");
            }

            return payment.GetProperty(Payments.PaymentConsts.PlanId, "");
        }
        
        public static decimal GetPlanAmount(this SubscriptionPayment payment)
        {
            if (!payment.IsRecurring.HasValue || !payment.IsRecurring.Value)
            {
                throw new ApplicationException("Only recurring payment has plan name!");
            }

            return payment.GetProperty<decimal>(Payments.PaymentConsts.PlanAmount, 0);
        }

        public static string GetPlanType(this SubscriptionPayment payment)
        {
            if (!payment.IsRecurring.HasValue || !payment.IsRecurring.Value)
            {
                throw new ApplicationException("Only recurring payment has plan type!");
            }

            return payment.GetProperty(Payments.PaymentConsts.PlanType, "");
        }
        
        public static int? GetEditionId(this SubscriptionPayment payment)
        {
            var product = payment.SubscriptionPaymentProducts.First();
            var editionId = product.GetProperty(PaymentConsts.EditionId, 0);
            return editionId > 0 ? editionId : null;
        }
    }
}