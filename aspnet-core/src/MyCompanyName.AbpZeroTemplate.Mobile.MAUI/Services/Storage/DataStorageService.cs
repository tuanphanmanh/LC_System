using Abp.Dependency;
using Abp.ObjectMapping;
using MyCompanyName.AbpZeroTemplate.ApiClient;
using MyCompanyName.AbpZeroTemplate.ApiClient.Models;
using MyCompanyName.AbpZeroTemplate.Core.DataStorage;
using MyCompanyName.AbpZeroTemplate.Models.Common;
using MyCompanyName.AbpZeroTemplate.Sessions.Dto;

namespace MyCompanyName.AbpZeroTemplate.Services.Storage
{
    public class DataStorageService : IDataStorageService, ISingletonDependency
    {
        private readonly IDataStorageManager _dataStorageManager;
        private readonly IObjectMapper _objectMapper;

        public DataStorageService(
            IDataStorageManager dataStorageManager,
            IObjectMapper objectMapper)
        {
            _dataStorageManager = dataStorageManager;
            _objectMapper = objectMapper;
        }

        public async Task StoreAccessTokenAsync(string newAccessToken, string newEncryptedAccessToken)
        {
            var authenticateResult = _dataStorageManager.Retrieve<AuthenticateResultPersistanceModel>(DataStorageKey.CurrentSession_TokenInfo);

            authenticateResult.AccessToken = newAccessToken;
            authenticateResult.EncryptedAccessToken = newEncryptedAccessToken;

            await _dataStorageManager.StoreAsync(DataStorageKey.CurrentSession_TokenInfo, authenticateResult);
        }

        public AbpAuthenticateResultModel RetrieveAuthenticateResult()
        {
            var data = _dataStorageManager.Retrieve<AuthenticateResultPersistanceModel>(
                    DataStorageKey.CurrentSession_TokenInfo
            );

            return _objectMapper.Map<AbpAuthenticateResultModel>(
                data
            );
        }

        public async Task StoreAuthenticateResultAsync(AbpAuthenticateResultModel authenticateResultModel)
        {
            await _dataStorageManager.StoreAsync(
                DataStorageKey.CurrentSession_TokenInfo,
                _objectMapper.Map<AuthenticateResultPersistanceModel>(authenticateResultModel)
            );
        }

        public TenantInformation RetrieveTenantInfo()
        {
            return _objectMapper.Map<TenantInformation>(
                _dataStorageManager.Retrieve<TenantInformationPersistanceModel>(
                    DataStorageKey.CurrentSession_TenantInfo
                )
            );
        }

        public async Task StoreTenantInfoAsync(TenantInformation tenantInfo)
        {
            await _dataStorageManager.StoreAsync(
                DataStorageKey.CurrentSession_TenantInfo,
                _objectMapper.Map<TenantInformationPersistanceModel>(tenantInfo)
            );
        }

        public GetCurrentLoginInformationsOutput RetrieveLoginInfo()
        {
            return _objectMapper.Map<GetCurrentLoginInformationsOutput>(
                _dataStorageManager.Retrieve<CurrentLoginInformationPersistanceModel>(
                    DataStorageKey.CurrentSession_LoginInfo
                )
            );
        }

        public async Task StoreLoginInformationAsync(GetCurrentLoginInformationsOutput loginInfo)
        {
            await _dataStorageManager.StoreAsync(
                DataStorageKey.CurrentSession_LoginInfo,
                _objectMapper.Map<CurrentLoginInformationPersistanceModel>(
                    loginInfo
                )
            );
        }

        public void ClearSessionPersistance()
        {
            _dataStorageManager.RemoveIfExists(DataStorageKey.CurrentSession_TokenInfo);
            _dataStorageManager.RemoveIfExists(DataStorageKey.CurrentSession_TenantInfo);
            _dataStorageManager.RemoveIfExists(DataStorageKey.CurrentSession_LoginInfo);
        }
    }
}