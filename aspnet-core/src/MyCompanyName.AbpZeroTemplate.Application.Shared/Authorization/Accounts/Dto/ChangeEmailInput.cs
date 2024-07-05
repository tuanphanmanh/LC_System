using System;
using System.Text.Json.Serialization;
using System.Web;
using Abp.Runtime.Security;
using Abp.Runtime.Validation;

namespace MyCompanyName.AbpZeroTemplate.Authorization.Accounts.Dto
{
    public class ChangeEmailInput: IShouldNormalize
    {
        [JsonIgnore]
        public long UserId { get; set; }

        [JsonIgnore]
        public string EmailAddress { get; set; }
        
        [JsonIgnore]
        public string OldEmailAddress { get; set; }

        /// <summary>
        /// Encrypted values for {TenantId}, {UserId} and {ConfirmationCode}
        /// </summary>
        public string c { get; set; }

        public void Normalize()
        {
            ResolveParameters();
        }

        protected virtual void ResolveParameters()
        {
            if (!string.IsNullOrEmpty(c))
            {
                var parameters = SimpleStringCipher.Instance.Decrypt(c);
                var query = HttpUtility.ParseQueryString(parameters);

                if (query["userId"] != null)
                {
                    UserId = Convert.ToInt32(query["userId"]);
                }

                if (query["emailAddress"] != null)
                {
                    EmailAddress = query["emailAddress"];
                }
                
                if (query["old"] != null)
                {
                    OldEmailAddress = query["old"];
                }
            }
        }
    }
}