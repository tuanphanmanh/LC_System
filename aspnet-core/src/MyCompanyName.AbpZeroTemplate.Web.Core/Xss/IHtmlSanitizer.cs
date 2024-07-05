using Abp.Dependency;

namespace MyCompanyName.AbpZeroTemplate.Web.Xss
{
    public interface IHtmlSanitizer: ITransientDependency
    {
        string Sanitize(string html);
    }
}