using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyCompanyName.AbpZeroTemplate.Common
{
    public class FormatCommon
    {
        public static string ReplaceHtmlCode(string val)
        {
            while (val.Contains(">") || val.Contains("<") || val.Contains("&lt;") || val.Contains("&lt") || val.Contains("&lt;/") || val.Contains("&lt/") || val.Contains("&gt;") || val.Contains("&gt"))
            {
                val = Regex.Replace(val, @"<|>|/|&lt;|&lt;\/|&gt;", "");
                val = Regex.Replace(val, @"\s+", " ");
            }
            return val;
        }
    }
}
