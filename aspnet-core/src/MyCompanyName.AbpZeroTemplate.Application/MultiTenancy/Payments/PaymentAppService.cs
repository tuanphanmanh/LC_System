using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using MyCompanyName.AbpZeroTemplate.Authorization;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments.Dto;
using Abp.Application.Services.Dto;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Abp.Collections.Extensions;
using Abp.Linq.Extensions;

namespace MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments
{
    public class PaymentAppService : AbpZeroTemplateAppServiceBase, IPaymentAppService
    {
        private readonly ISubscriptionPaymentRepository _subscriptionPaymentRepository;
        private readonly IPaymentGatewayStore _paymentGatewayStore;
        private readonly IPaymentManager _paymentManager;

        public PaymentAppService(
            ISubscriptionPaymentRepository subscriptionPaymentRepository,
            IPaymentGatewayStore paymentGatewayStore,
            IPaymentManager paymentManager)
        {
            _subscriptionPaymentRepository = subscriptionPaymentRepository;
            _paymentGatewayStore = paymentGatewayStore;
            _paymentManager = paymentManager;
        }

        public async Task<long> CreatePayment(CreatePaymentDto input)
        {
            var payment = new SubscriptionPayment
            {
                PaymentPeriodType = input.PaymentPeriodType,
                TenantId = input.TenantId,
                Gateway = input.SubscriptionPaymentGatewayType,
                DayCount = input.PaymentPeriodType.HasValue ? (int) input.PaymentPeriodType.Value : 0,
                IsRecurring = input.RecurringPaymentEnabled,
                SuccessUrl = input.SuccessUrl,
                ErrorUrl = input.ErrorUrl,
                ExtraProperties = input.ExtraProperties
            };

            foreach (var product in input.Products)
            {
                payment.SubscriptionPaymentProducts.Add(new SubscriptionPaymentProduct(
                    product.Description,
                    product.Amount,
                    product.Count,
                    product.Amount * product.Count,
                    product.ExtraProperties
                ));
            }

            return await _paymentManager.CreatePayment(payment);
        }

        public async Task CancelPayment(CancelPaymentDto input)
        {
            var payment = await _subscriptionPaymentRepository.GetByGatewayAndPaymentIdAsync(
                input.Gateway,
                input.PaymentId
            );

            payment.SetAsCancelled();
        }

        public async Task UpdatePayment(UpdatePaymentDto input)
        {
            var payment = await _paymentManager.GetPayment(input.PaymentId);
            payment.Gateway = Enum.Parse<SubscriptionPaymentGatewayType>(input.Gateway);
            payment.IsRecurring ??= input.IsRecurring;

            await _paymentManager.UpdatePayment(payment);
        }

        public async Task<PagedResultDto<SubscriptionPaymentListDto>> GetPaymentHistory(GetPaymentHistoryInput input)
        {
            var query = _subscriptionPaymentRepository
                .GetAllIncluding(x => x.SubscriptionPaymentProducts)
                .Where(sp => sp.TenantId == AbpSession.GetTenantId())
                .OrderBy(input.Sorting);

            var payments = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();
            var paymentsCount = query.Count();

            return new PagedResultDto<SubscriptionPaymentListDto>(
                paymentsCount,
                ObjectMapper.Map<List<SubscriptionPaymentListDto>>(payments)
            );
        }

        public List<PaymentGatewayModel> GetActiveGateways(GetActiveGatewaysInput input)
        {
            return _paymentGatewayStore.GetActiveGateways()
                .WhereIf(input.RecurringPaymentsEnabled.HasValue && input.RecurringPaymentsEnabled.Value,
                    gateway => gateway.SupportsRecurringPayments)
                .ToList();
        }

        public async Task<SubscriptionPaymentDto> GetPaymentAsync(long paymentId)
        {
            var item = await _subscriptionPaymentRepository.GetPaymentWithProducts(paymentId);
            return ObjectMapper.Map<SubscriptionPaymentDto>(item);
        }

        public async Task<SubscriptionPaymentDto> GetLastCompletedPayment()
        {
            var payment = await _subscriptionPaymentRepository.GetLastCompletedPaymentOrDefaultAsync(
                tenantId: AbpSession.GetTenantId(),
                gateway: null,
                isRecurring: null);

            return ObjectMapper.Map<SubscriptionPaymentDto>(payment);
        }

        public async Task PaymentFailed(long paymentId)
        {
            var payment = await _subscriptionPaymentRepository.GetAsync(paymentId);
            payment.SetAsFailed();
        }
        
        [AbpAuthorize(AppPermissions.Pages_Administration_Tenant_SubscriptionManagement)]
        public Task<bool> HasAnyPayment()
        {
            return _paymentManager.HasAnyPayment(AbpSession.GetTenantId());
        }
    }
}