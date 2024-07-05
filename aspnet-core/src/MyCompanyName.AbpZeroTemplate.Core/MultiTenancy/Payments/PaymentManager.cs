using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Collections.Extensions;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Subscription;

namespace MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments
{
    public class PaymentManager : IPaymentManager
    {
        private readonly ISubscriptionPaymentRepository _subscriptionPaymentRepository;

        public PaymentManager(
            ISubscriptionPaymentRepository subscriptionPaymentRepository)
        {
            _subscriptionPaymentRepository = subscriptionPaymentRepository;
        }

        public async Task<long> CreatePayment(SubscriptionPayment payment)
        {
            if (payment.RecurringPaymentEnabled())
            {
                if (payment.GetPlanId().IsNullOrEmpty())
                {
                    throw new ApplicationException($"{PaymentConsts.PlanId} is required for recurring payments!");
                }

                if (payment.GetPlanType().IsNullOrEmpty())
                {
                    throw new ApplicationException($"{PaymentConsts.PlanType} is required for recurring payments!");
                }
            }

            var paymentId = await _subscriptionPaymentRepository.InsertAndGetIdAsync(payment);
            return paymentId;
        }

        public async Task<bool> HasAnyPayment(int tenantId)
        {
            return await _subscriptionPaymentRepository.GetLastCompletedPaymentOrDefaultAsync(
                tenantId: tenantId,
                gateway: null,
                isRecurring: null) != default;
        }

        public SubscriptionPayment GetLastCompletedSubscriptionPayment(int tenantId)
        {
            return _subscriptionPaymentRepository.GetAll()
                .Where(p =>
                    p.TenantId == tenantId &&
                    p.Status == SubscriptionPaymentStatus.Completed && p.IsRecurring == true)
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();
        }

        public Task<SubscriptionPayment> GetPayment(long paymentId)
        {
            return _subscriptionPaymentRepository.GetAsync(paymentId);
        }

        public Task UpdatePayment(SubscriptionPayment payment)
        {
            return _subscriptionPaymentRepository.UpdateAsync(payment);
        }
    }
}