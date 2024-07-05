using Abp.Dependency;
using Microsoft.AspNetCore.Components;
using MyCompanyName.AbpZeroTemplate.ApiClient;
using MyCompanyName.AbpZeroTemplate.Services.Storage;

namespace MyCompanyName.AbpZeroTemplate.Services.Navigation
{
    public class NavigationService : INavigationService, ISingletonDependency
    {
        private readonly IAccessTokenManager _accessTokenManager;
        private readonly IDataStorageService _dataStorageService;
        private readonly IApplicationContext _applicationContext;

        private NavigationManager _navigationManager;

        public NavigationService(IAccessTokenManager accessTokenManager,
            IApplicationContext applicationContext,
            IDataStorageService dataStorageService
            )
        {
            _accessTokenManager = accessTokenManager;
            _applicationContext = applicationContext;
            _dataStorageService = dataStorageService;
        }

        public void Initialize(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
            LoadPersistedSession();

            if (_accessTokenManager.IsUserLoggedIn)
            {
                navigationManager.NavigateTo(NavigationUrlConsts.Index);
            }
            else
            {
                navigationManager.NavigateTo(NavigationUrlConsts.Login);
            }
        }

        private void LoadPersistedSession()
        {
            _accessTokenManager.AuthenticateResult = _dataStorageService.RetrieveAuthenticateResult();
            _applicationContext.Load(_dataStorageService.RetrieveTenantInfo(), _dataStorageService.RetrieveLoginInfo());
        }

        public void NavigateTo(string uri, bool forceLoad = false, bool replace = false)
        {
            _navigationManager.NavigateTo(uri, forceLoad, replace);
        }
    }
}