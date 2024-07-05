using System.Collections.Generic;

namespace MyCompanyName.AbpZeroTemplate.Authorization.Users.Profile.Dto
{
    public class GenerateGoogleAuthenticatorKeyOutput
    {
        public string QrCodeSetupImageUrl { get; set; }
        public string GoogleAuthenticatorKey { get; set; }
    }
}
