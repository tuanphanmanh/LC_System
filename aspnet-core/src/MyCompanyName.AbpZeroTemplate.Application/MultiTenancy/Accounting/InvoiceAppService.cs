using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using MyCompanyName.AbpZeroTemplate.Configuration;
using MyCompanyName.AbpZeroTemplate.Editions;
using MyCompanyName.AbpZeroTemplate.ExtraProperties;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Accounting.Dto;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments.Dto;

namespace MyCompanyName.AbpZeroTemplate.MultiTenancy.Accounting
{
    public class InvoiceAppService : AbpZeroTemplateAppServiceBase, IInvoiceAppService
    {
        private readonly ISubscriptionPaymentRepository _subscriptionPaymentRepository;
        private readonly IInvoiceNumberGenerator _invoiceNumberGenerator;
        private readonly EditionManager _editionManager;
        private readonly IRepository<Invoice> _invoiceRepository;

        public InvoiceAppService(
            ISubscriptionPaymentRepository subscriptionPaymentRepository,
            IInvoiceNumberGenerator invoiceNumberGenerator,
            EditionManager editionManager,
            IRepository<Invoice> invoiceRepository)
        {
            _subscriptionPaymentRepository = subscriptionPaymentRepository;
            _invoiceNumberGenerator = invoiceNumberGenerator;
            _editionManager = editionManager;
            _invoiceRepository = invoiceRepository;
        }

        public async Task<InvoiceDto> GetInvoiceInfo(EntityDto<long> input)
        {
            var paymentEntity = await _subscriptionPaymentRepository.GetPaymentWithProducts(input.Id);
            var payment = ObjectMapper.Map<SubscriptionPaymentDto>(paymentEntity);
            var invoiceNo = payment.InvoiceNo.ToString();
            
            if (string.IsNullOrEmpty(invoiceNo))
            {
                throw new Exception("There is no invoice for this payment !");
            }

            if (payment.TenantId != AbpSession.GetTenantId())
            {
                throw new UserFriendlyException(L("ThisInvoiceIsNotYours"));
            }

            var invoice = await _invoiceRepository.FirstOrDefaultAsync(b => b.InvoiceNo == invoiceNo);
            if (invoice == null)
            {
                throw new UserFriendlyException();
            }
            
            var hostAddress = await SettingManager.GetSettingValueAsync(AppSettings.HostManagement.BillingAddress);

            return new InvoiceDto
            {
                InvoiceNo = invoiceNo,
                InvoiceDate = invoice.InvoiceDate,
                TotalAmount = payment.TotalAmount,
               
                Items = payment.SubscriptionPaymentProducts,
                
                HostAddress = hostAddress.Replace("\r\n", "|").Split('|').ToList(),
                HostLegalName = await SettingManager.GetSettingValueAsync(AppSettings.HostManagement.BillingLegalName),

                TenantAddress = invoice.TenantAddress.Replace("\r\n", "|").Split('|').ToList(),
                TenantLegalName = invoice.TenantLegalName,
                TenantTaxNo = invoice.TenantTaxNo
            };
        }

        [UnitOfWork(IsolationLevel.ReadUncommitted)]
        public async Task CreateInvoice(CreateInvoiceDto input)
        {
            var payment = await _subscriptionPaymentRepository.GetAsync(input.SubscriptionPaymentId);

            if (payment.Status != SubscriptionPaymentStatus.Completed)
            {
                throw new UserFriendlyException("A membership with unpaid dues cannot have an invoice created!");
            }

            if (!string.IsNullOrEmpty(payment.InvoiceNo))
            {
                throw new UserFriendlyException("Invoice is already generated for this payment.");
            }

            var invoiceNo = await _invoiceNumberGenerator.GetNewInvoiceNumber();

            var tenantLegalName = await SettingManager.GetSettingValueAsync(AppSettings.TenantManagement.BillingLegalName);
            var tenantAddress = await SettingManager.GetSettingValueAsync(AppSettings.TenantManagement.BillingAddress);
            var tenantTaxNo = await SettingManager.GetSettingValueAsync(AppSettings.TenantManagement.BillingTaxVatNo);

            if (string.IsNullOrEmpty(tenantLegalName) || string.IsNullOrEmpty(tenantAddress))
            {
                throw new UserFriendlyException(L("InvoiceInfoIsMissingOrNotCompleted"));
            }

            await _invoiceRepository.InsertAsync(new Invoice
            {
                InvoiceNo = invoiceNo,
                InvoiceDate = Clock.Now,
                TenantLegalName = tenantLegalName,
                TenantAddress = tenantAddress,
                TenantTaxNo = tenantTaxNo
            });

            payment.InvoiceNo = invoiceNo;
        }
    }
}