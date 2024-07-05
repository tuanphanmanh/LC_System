using System.Threading.Tasks;

namespace MyCompanyName.AbpZeroTemplate.Net.Emailing
{
    public interface IEmailSettingsChecker
    {
        bool EmailSettingsValid();

        Task<bool> EmailSettingsValidAsync();
    }
}