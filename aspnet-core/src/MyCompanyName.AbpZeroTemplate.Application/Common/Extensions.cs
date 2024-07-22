using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace tmss.Common
{
    public static class Extensions
    {
        public static string ToFriendlyErrorMsg(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";
            if (value.IndexOf("unique") > 0)
            {
                return "Data is duplicated!";
            }
            else if (value.IndexOf("DELETE statement") > 0)
            {
                return "This data is in use. Can not delete!";
            }
            else if (value.IndexOf("truncated") > 0)
            {
                return "Dữ liệu vượt quá kích thước cho phép!";
            }
            return value;
        }
        public static void CopyProperties(this object source, object destination)
        {
            // If any this null throw an exception
            if (source == null || destination == null)
                throw new Exception("Source or/and Destination Objects are null");
            // Getting the Types of the objects
            Type typeDest = destination.GetType();
            Type typeSrc = source.GetType();
            // Collect all the valid properties to map
            var results = from srcProp in typeSrc.GetProperties()
                          let targetProperty = typeDest.GetProperty(srcProp.Name)
                          where srcProp.CanRead
                          && targetProperty != null
                          && (targetProperty.GetSetMethod(true) != null && !targetProperty.GetSetMethod(true).IsPrivate)
                          && (targetProperty.GetSetMethod().Attributes & MethodAttributes.Static) == 0
                          && targetProperty.PropertyType.IsAssignableFrom(srcProp.PropertyType)
                          select new { sourceProperty = srcProp, targetProperty = targetProperty };
            //map the properties
            foreach (var props in results)
            {
                props.targetProperty.SetValue(destination, props.sourceProperty.GetValue(source, null), null);
            }
        }
        public static string ConvertPriceToWordsVnd(long price)
        {
            if (price == 0)
                return "Không đồng";

            string[] ones = { "", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string[] onesGroup = { "", "mốt", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string[] tens = { "", "mười", "hai mươi", "ba mươi", "bốn mươi", "năm mươi", "sáu mươi", "bảy mươi", "tám mươi", "chín mươi" };
            string[] hundreds = { "", "một", "hai trăm", "ba trăm", "bốn trăm", "năm trăm", "sáu trăm", "bảy trăm", "tám trăm", "chín trăm" };

            string result = "";
            int billion = (int)(price / 1000000000);
            int million = (int)((price % 1000000000) / 1000000);
            int thousand = (int)((price % 1000000) / 1000);
            int remainder = (int)(price % 1000);

            if (billion > 0)
            {
                result += $"{hundreds[billion]} tỷ ";
            }
            if (million > 0)
            {
                if (billion > 0 && million < 100) result += "linh ";
                result += $"{ConvertGroupVnd(million)} triệu ";
            }
            if (thousand > 0)
            {
                if (million > 0 && thousand < 100) result += "linh ";
                result += $"{ConvertGroupVnd(thousand)} nghìn ";
            }
            if (remainder > 0)
            {
                if (thousand > 0 && remainder < 100) result += "linh ";
                result += ConvertGroupVnd(remainder);
            }

            result = result.Trim();
            return result;
        }

        static string ConvertGroupVnd(int number)
        {
            string[] ones = { "", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string[] onesGroup = { "", "mốt", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string[] tens = { "", "mười", "hai mươi", "ba mươi", "bốn mươi", "năm mươi", "sáu mươi", "bảy mươi", "tám mươi", "chín mươi" };
            string[] hundreds = { "", "một trăm", "hai trăm", "ba trăm", "bốn trăm", "năm trăm", "sáu trăm", "bảy trăm", "tám trăm", "chín trăm" };

            int hundred = number / 100;
            int ten = (number % 100) / 10;
            int one = number % 10;

            string result = "";

            if (hundred > 0)
            {
                result += $"{hundreds[hundred]} ";
            }
            if (ten > 1)
            {
                result += $"{tens[ten]} ";
                if (one == 1) result += "mốt ";
                else if (one > 1) result += $"{onesGroup[one]} ";
            }
            else if (ten == 1)
            {
                result += "mười ";
                if (one == 1) result += "một ";
                else if (one > 1) result += $"{ones[one]} ";
            }
            else
            {
                if (one > 0) result += $"{ones[one]} ";
            }

            return result.Trim();
        }

        public static string ConvertPriceToVietnameseText(long price)
        {
            if (price == 0)
                return "Không đồng";

            string[] ones = { "", "một", "hai", "ba", "bốn", "lăm", "sáu", "bảy", "tám", "chín" };
            string[] teens = { "mười", "mười một", "mười hai", "mười ba", "mười bốn", "mười lăm", "mười sáu", "mười bảy", "mười tám", "mười chín" };
            string[] tens = { "", "", "hai mươi", "ba mươi", "bốn mươi", "năm mươi", "sáu mươi", "bảy mươi", "tám mươi", "chín mươi" };
            string[] groups = { "", "nghìn", "triệu", "tỷ" };

            int groupIndex = 0;
            string result = "";

            while (price > 0)
            {
                int group = (int)(price % 1000);
                price /= 1000;

                int unit = group % 10;
                int ten = (group % 100) / 10;
                int hundred = group / 100;

                string groupText = "";

                if (hundred > 0)
                {
                    groupText += ones[hundred] + " trăm ";
                    if (ten == 0 && unit > 0)
                    {
                        groupText += "linh ";
                    }
                }


                if (ten > 1)
                {
                    groupText += tens[ten];
                    if (unit > 0)
                        groupText += " " + ones[unit];
                }
                else if (ten == 1)
                {
                    groupText += teens[unit];
                }
                else if (unit > 0)
                {
                    groupText += ones[unit];
                }

                if (!string.IsNullOrEmpty(groupText))
                {
                    //if (groupIndex > 0 && group > 0 && price % 1000 == 0)
                    //{
                    //    groupText += " lẻ ";
                    //}
                    //else
                    //{
                    //    groupText += " ";
                    //}

                    groupText += " " + groups[groupIndex];
                    if (!string.IsNullOrEmpty(result))
                        groupText += " ";
                    result = groupText + result;
                }

                groupIndex++;
            }

            return result.Trim();

        }

        public static string ConvertPriceToWordsUsd(decimal price)
        {
            string[] unitsMap = { "", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten",
                            "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
            string[] tensMap = { "", "", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

            if (price == 0)
                return "zero";

            // Split the decimal value into dollars and cents
            int dollars = (int)Math.Floor(price);
            int cents = (int)Math.Round((price - dollars) * 100);

            string result = "";

            if (dollars > 0)
            {
                // Convert dollars part to words
                result += NumberToWordsUsd(dollars) + " dollars";
            }

            if (cents > 0)
            {
                // If there are cents, add them to the result
                result += " and " + NumberToWordsUsd(cents) + " cents";
            }

            return result;
        }

        static string NumberToWordsUsd(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWordsUsd(Math.Abs(number));

            string[] unitsMap = { "", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten",
                            "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
            string[] tensMap = { "", "", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWordsUsd(number / 1000000) + " million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWordsUsd(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWordsUsd(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += " " + unitsMap[number % 10];
                }
            }

            return words.Trim();
        }
    }
}
