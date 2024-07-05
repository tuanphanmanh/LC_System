using System.Text.RegularExpressions;

namespace MyCompanyName.AbpZeroTemplate.Web.Xss
{
    
    public class DefaultHtmlSanitizer : IHtmlSanitizer
    {
        public string Sanitize(string html)
        {
            return Regex.Replace(html, "<.*?>|&.*?;", string.Empty);
        }
    }
}