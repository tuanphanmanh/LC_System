using Abp.Events.Bus.Handlers;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Subscription;

namespace MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments
{
    public interface ISupportsRecurringPayments : 
        IEventHandler<RecurringPaymentsDisabledEventData>, 
        IEventHandler<RecurringPaymentsEnabledEventData>,
        IEventHandler<SubscriptionUpdatedEventData>,
        IEventHandler<SubscriptionCancelledEventData>
    {

    }
}
