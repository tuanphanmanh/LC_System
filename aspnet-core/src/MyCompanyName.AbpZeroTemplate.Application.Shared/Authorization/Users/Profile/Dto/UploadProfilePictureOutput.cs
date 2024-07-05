using Abp.Web.Models;

namespace MyCompanyName.AbpZeroTemplate.Authorization.Users.Profile.Dto
{
    public class UploadProfilePictureOutput : ErrorInfo
    {
        public UploadProfilePictureOutput()
        {
            
        }

        public UploadProfilePictureOutput(ErrorInfo error)
        {
            Code = error.Code;
            Details = error.Details;
            Message = error.Message;
            ValidationErrors = error.ValidationErrors;
        }
    }
}