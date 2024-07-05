using System;
using System.ComponentModel.DataAnnotations;
using Abp.Extensions;
using Abp.Runtime.Validation;

namespace MyCompanyName.AbpZeroTemplate.Authorization.Users.Profile.Dto
{
    public class UpdateProfilePictureInput : ICustomValidate
    {
        [MaxLength(400)]
        public string FileToken { get; set; }
        
        public bool UseGravatarProfilePicture { get; set; }
        
        public long? UserId { get; set; }
        
        public void AddValidationErrors(CustomValidationContext context)
        {
            if (!UseGravatarProfilePicture && FileToken.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(FileToken));
            }
        }
    }
}