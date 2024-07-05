using MyCompanyName.AbpZeroTemplate.ApiClient;

namespace MyCompanyName.AbpZeroTemplate.Mobile.MAUI.Core.ApiClient
{
    public class MAUIApplicationContext : ApplicationContext
    {
        private TenantInformation _currentTenant;
        public override TenantInformation CurrentTenant
        {
            get => _currentTenant;
            protected set
            {
                if (value == _currentTenant)
                {
                    return;
                }
                _currentTenant = value;
                OnTenantChange?.Invoke(this, EventArgs.Empty);
            }
        }

        public static event EventHandler OnTenantChange;
    }
}
