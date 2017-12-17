// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
#endregion

namespace TLAuto.Base.Extensions
{
    public static class StringExtensions
    {
        #region 下载速度转换
        /// <summary>
        /// 下载速度转换成字符串的形式
        /// </summary>
        /// <param name="speed">下载速度（字节为单位）</param>
        /// <returns></returns>
        public static string ConvertDownloadStr(this double speed)
        {
            var mStrSize = "";
            var factSize = speed;
            if (factSize < 1024.00)
            {
                mStrSize = factSize.ToString("F2") + " Byte/s";
            }
            else
            {
                if ((factSize >= 1024.00) && (factSize < 1048576))
                {
                    mStrSize = (factSize / 1024.00).ToString("F2") + " K/s";
                }
                else
                {
                    if ((factSize >= 1048576) && (factSize < 1073741824))
                    {
                        mStrSize = (factSize / 1024.00 / 1024.00).ToString("F2") + " M/s";
                    }
                    else
                    {
                        if (factSize >= 1073741824)
                        {
                            mStrSize = (factSize / 1024.00 / 1024.00 / 1024.00).ToString("F2") + " G/s";
                        }
                    }
                }
            }
            return mStrSize;
        }
        #endregion

        #region 字符串过长截取
        /// <summary>
        /// 双字节判断
        /// </summary>
        private static readonly Regex DoubleByteRegex = new Regex("[^\x00-\xff]");

        /// <summary>
        /// 大小写判断
        /// </summary>
        private static readonly Regex CaseRegex = new Regex("[A-H]|[M-Z]");

        /// <summary>
        /// 得到字符串真实长度, 1个汉字长度为2,一个大写算1.5个
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <param name="nums">字符数组</param>
        /// <returns>字符长度</returns>
        public static int GetStringLength(this string input, out char[] nums)
        {
            double num = 0;
            nums = input.ToCharArray();
            for (var i = 0; i < nums.Length; i++)
            {
                var m = DoubleByteRegex.Match(nums[i].ToString());
                if (m.Success)
                {
                    num += 2;
                }
                else
                {
                    var m2 = CaseRegex.Match(nums[i].ToString());
                    if (m2.Success)
                    {
                        num += 1.5;
                    }
                    else
                    {
                        num++;
                    }
                }
            }
            return (int)num;
        }

        /// <summary>
        /// 字符串截断
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <param name="length">长度</param>
        /// <param name="substitute">超过长度的标示，如“...”，不填就不会再后面打点</param>
        /// <returns></returns>
        public static string SubCutString(this string input, int length, string substitute = "...")
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            //初始化字符串的字符数组
            char[] arr;
            //如果字符串的真实长度小于或者等于要截取的字符串长度就直接返回源字符串
            if (GetStringLength(input, out arr) <= length)
            {
                return input;
            }
            //获取不包括substitute的长度
            if (string.IsNullOrEmpty(substitute))
            {
                length -= Encoding.UTF8.GetBytes("...").Length;
            }
            else
            {
                length -= Encoding.UTF8.GetBytes(substitute).Length;
            }
            //分析出来的字符字节长度
            double byteLength = 0;
            //拼装字符串类
            var subBuilder = new StringBuilder();
            //循环判断每个字符是属于双字节还是单字节(大写英文字母为1.5个字节)
            for (var i = 0; i < arr.Length; i++)
            {
                //判断双字节
                var doubleByteMatch = DoubleByteRegex.Match(arr[i].ToString());
                if (doubleByteMatch.Success)
                {
                    byteLength += 2;
                }
                else
                {
                    //判断是否为大写
                    var caseMatch = CaseRegex.Match(arr[i].ToString());
                    if (caseMatch.Success)
                    {
                        byteLength += 1.5;
                    }
                    else
                    {
                        byteLength++;
                    }
                }
                if (byteLength > length)
                {
                    break;
                }
                subBuilder.Append(arr[i]);
            }
            subBuilder.Append(substitute);
            return subBuilder.ToString();
        }

        /// <summary>
        /// 字符串截断
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <param name="length">长度</param>
        /// <param name="substitute">超过长度的标示，如“...”，不填就不会再后面打点</param>
        /// <returns></returns>
        public static string SubCutString2(this string input, int length, string substitute = "...")
        {
            var ascii = new ASCIIEncoding();
            var tempLen = 0;
            var tempString = "";
            var s = ascii.GetBytes(input);
            for (var i = 0; i < s.Length; i++)
            {
                if (s[i] == 63)
                {
                    tempLen += 2;
                }
                else
                {
                    tempLen += 1;
                }
                try
                {
                    tempString += input.Substring(i, 1);
                }
                catch
                {
                    break;
                }
                if (tempLen > length)
                {
                    break;
                }
            }
            var mybyte = Encoding.Default.GetBytes(input);
            if (mybyte.Length > length)
            {
                tempString += substitute;
            }
            return tempString;
        }

        /// <summary>
        /// 从末尾删掉指定长度的字符串
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <param name="delLength">删除长度</param>
        /// <returns></returns>
        public static string DeleteEndString(this string input, int delLength)
        {
            return input.Length > delLength
                       ? input.Substring(0, input.Length - delLength)
                       : input;
        }
        #endregion

        #region 半角全角转换
        /// <summary>
        /// 转半角的函数(SBC case)
        /// </summary>
        /// <param name="input">输入</param>
        /// <returns></returns>
        public static string ToDBC(this string input)
        {
            var c = input.ToCharArray();
            for (var i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if ((c[i] > 65535) && (c[i] < 65375))
                {
                    c[i] = (char)(c[i] - 65248);
                }
            }
            return new string(c);
        }

        /// <summary>
        /// 转全角的函数(SBC case)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToSBC(this string input)
        {
            //半角转全角：
            var c = input.ToCharArray();
            for (var i = 0; i < c.Length; i++)
            {
                if (c[i] == 32)
                {
                    c[i] = (char)12288;
                    continue;
                }
                if (c[i] < 127)
                {
                    c[i] = (char)(c[i] + 65248);
                }
            }
            return new string(c);
        }
        #endregion

        #region 获取汉字拼音首字母
        /// <summary>
        /// 在指定的字符串列表cnStr中检索符合拼音索引字符串
        /// </summary>
        /// <param name="cnStr">汉字字符串</param>
        /// <returns>相对应的汉语拼音首字母串</returns>
        public static string GetSpellCode(this string cnStr)
        {
            var strTemp = string.Empty;
            var iLen = cnStr.Length;
            for (var i = 0; i <= iLen - 1; i++)
            {
                strTemp += GetCharSpellCode(cnStr.Substring(i, 1));
            }
            return strTemp;
        }

        /// <summary>
        /// 得到一个汉字的拼音第一个字母，如果是一个英文字母则直接返回大写字母
        /// </summary>
        /// <param name="cnChar">单个汉字</param>
        /// <returns>单个大写字母</returns>
        private static string GetCharSpellCode(this string cnChar)
        {
            var zw = Encoding.Default.GetBytes(cnChar);
            //如果是字母，则直接返回 
            if (zw.Length == 1)
            {
                return cnChar.ToUpper();
            }
            //获取单一字符的字节数组
            int i1 = zw[0];
            int i2 = zw[1];
            long iCnChar = (i1 * 256) + i2;
            //expresstion 
            //table of the constant list 
            // 'A'; //45217..45252 
            // 'B'; //45253..45760 
            // 'C'; //45761..46317 
            // 'D'; //46318..46825 
            // 'E'; //46826..47009 
            // 'F'; //47010..47296 
            // 'G'; //47297..47613 

            // 'H'; //47614..48118 
            // 'J'; //48119..49061 
            // 'K'; //49062..49323 
            // 'L'; //49324..49895 
            // 'M'; //49896..50370 
            // 'N'; //50371..50613 
            // 'O'; //50614..50621 
            // 'P'; //50622..50905 
            // 'Q'; //50906..51386 

            // 'R'; //51387..51445 
            // 'S'; //51446..52217 
            // 'T'; //52218..52697 
            //没有U,V 
            // 'W'; //52698..52979 
            // 'X'; //52980..53640 
            // 'Y'; //53689..54480 
            // 'Z'; //54481..55289 

            // iCnChar match the constant 
            if ((iCnChar >= 45217) && (iCnChar <= 45252))
            {
                return "A";
            }
            if ((iCnChar >= 45253) && (iCnChar <= 45760))
            {
                return "B";
            }
            if ((iCnChar >= 45761) && (iCnChar <= 46317))
            {
                return "C";
            }
            if ((iCnChar >= 46318) && (iCnChar <= 46825))
            {
                return "D";
            }
            if ((iCnChar >= 46826) && (iCnChar <= 47009))
            {
                return "E";
            }
            if ((iCnChar >= 47010) && (iCnChar <= 47296))
            {
                return "F";
            }
            if ((iCnChar >= 47297) && (iCnChar <= 47613))
            {
                return "G";
            }
            if ((iCnChar >= 47614) && (iCnChar <= 48118))
            {
                return "H";
            }
            if ((iCnChar >= 48119) && (iCnChar <= 49061))
            {
                return "J";
            }
            if ((iCnChar >= 49062) && (iCnChar <= 49323))
            {
                return "K";
            }
            if ((iCnChar >= 49324) && (iCnChar <= 49895))
            {
                return "L";
            }
            if ((iCnChar >= 49896) && (iCnChar <= 50370))
            {
                return "M";
            }
            if ((iCnChar >= 50371) && (iCnChar <= 50613))
            {
                return "N";
            }
            if ((iCnChar >= 50614) && (iCnChar <= 50621))
            {
                return "O";
            }
            if ((iCnChar >= 50622) && (iCnChar <= 50905))
            {
                return "P";
            }
            if ((iCnChar >= 50906) && (iCnChar <= 51386))
            {
                return "Q";
            }
            if ((iCnChar >= 51387) && (iCnChar <= 51445))
            {
                return "R";
            }
            if ((iCnChar >= 51446) && (iCnChar <= 52217))
            {
                return "S";
            }
            if ((iCnChar >= 52218) && (iCnChar <= 52697))
            {
                return "T";
            }
            if ((iCnChar >= 52698) && (iCnChar <= 52979))
            {
                return "W";
            }
            if ((iCnChar >= 52980) && (iCnChar <= 53640))
            {
                return "X";
            }
            if ((iCnChar >= 53689) && (iCnChar <= 54480))
            {
                return "Y";
            }
            if ((iCnChar >= 54481) && (iCnChar <= 55289))
            {
                return "Z";
            }
            return "?";
        }
        #endregion

        #region 正则表达式
        /// <summary>
        /// 指示正则表达式使用pattern参数中制定的正则表达式是否在输入字符串中找到匹配项
        /// </summary>
        /// <param name="input">要搜索匹配项的字符串</param>
        /// <param name="pattern">要匹配的正则表达式模式</param>
        /// <returns></returns>
        public static bool IsMatch(this string input, string pattern)
        {
            return !input.IsNullOrEmpty() && Regex.IsMatch(input, pattern);
        }

        /// <summary>
        /// 在指定的输入字符串中搜索pattern参数中提供的正则表达式的匹配项
        /// </summary>
        /// <param name="input">要搜索匹配项的字符串</param>
        /// <param name="pattern">要匹配的正则表达式模式</param>
        /// <returns></returns>
        public static string Match(this string input, string pattern)
        {
            return input.IsNullOrEmpty()
                       ? string.Empty
                       : Regex.Match(input, pattern).Value;
        }

        /// <summary>
        /// 在指定的输入字符串内，使用指定的替换字符串替换于指定正则表达式匹配的所有字符串
        /// </summary>
        /// <param name="input">要搜索匹配项的字符串</param>
        /// <param name="pattern">要匹配的正则表达式模式</param>
        /// <param name="replacement">替换字符串</param>
        /// <returns></returns>
        public static string Replace(this string input, string pattern, string replacement)
        {
            return input.IsNullOrEmpty()
                       ? string.Empty
                       : Regex.Replace(input, pattern, replacement);
        }

        /// <summary>
        /// 在指定的输入字符串内，使用指定的替换字符串替换于指定正则表达式匹配的所有字符串
        /// </summary>
        /// <param name="input">要搜索匹配项的字符串</param>
        /// <param name="pattern">要匹配的正则表达式模式</param>
        /// <param name="replacement">替换字符串</param>
        /// <param name="options">提供用于设置正则表达式选项的枚举值</param>
        /// <returns></returns>
        public static string Replace(this string input, string pattern, string replacement, RegexOptions options)
        {
            return input.IsNullOrEmpty()
                       ? string.Empty
                       : Regex.Replace(input, pattern, replacement, options);
        }

        /// <summary>
        /// 在指定的输入字符串内，替换开始和结束字符串中间的值
        /// </summary>
        /// <param name="input"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="replacement">替换字符串</param>
        /// <returns></returns>
        public static string ReplaceStartToEnd(this string input, string start, string end, string replacement)
        {
            return input.Replace("(?<=(" + start + "))[.\\s\\S]*?(?=(" + end + "))", replacement, RegexOptions.Multiline | RegexOptions.Singleline);
        }

        /// <summary>
        /// 获得字符串中开始和结束字符串中间得值
        /// </summary>
        /// <param name="input">字符串</param>
        /// <param name="start">开始</param>
        /// <param name="end">结束</param>
        /// <returns></returns>
        public static string GetStartToEnd(this string input, string start, string end)
        {
            var rg = new Regex("(?<=(" + start + "))[.\\s\\S]*?(?=(" + end + "))", RegexOptions.Multiline | RegexOptions.Singleline);
            return rg.Match(input).Value;
        }

        /// <summary>
        /// 验证输入字符串是否包含数字
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsMatchNumeric(this string input)
        {
            return input.IsMatch(@"^[-]?\d+[.]?\d*$");
        }

        /// <summary>
        /// 验证输入字符串是否包含字母
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsMatchLetter(this string input)
        {
            return input.IsMatch(@"^[A-Za-z]+$");
        }

        /// <summary>
        /// 验证输入字符串是否包含数字或字母
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsMatchNumericAndLetter(this string input)
        {
            return input.IsMatch(@"^[A-Za-z0-9]+$");
        }

        /// <summary>
        /// 验证输入字符串是否包含数字或字母或小数点
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsMatchNumericAndLetterAndSign(this string input)
        {
            //特殊符号可以自由添加，在\.位置后面
            return input.IsMatch(@"^[A-Za-z0-9\.]+$");
        }

        /// <summary>
        /// 验证输入字符串是否是合法IP(不包含端口号)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsMatchIpAddressWithOutPort(this string input)
        {
            return input.IsMatch(@"^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])$");
        }

        /// <summary>
        /// 判断一个字符串是否是正数型的字符串
        /// </summary>
        /// <param name="input">字符串</param>
        /// <returns>是则返回true，否则返回false</returns>
        public static bool IsUnsign(this string input)
        {
            return Regex.IsMatch(input, @"^[1-9]\d*$");
        }
        #endregion

        #region 数据类型转换和验证
        /// <summary>
        /// 指示指定的对象是否是Int32
        /// </summary>
        /// <param name="input">字符串</param>
        /// <returns></returns>
        public static bool IsInt32(this string input)
        {
            int i;
            return int.TryParse(input, out i);
        }

        /// <summary>
        /// 指示指定的对象是否是Int64（long）
        /// </summary>
        /// <param name="input">字符串</param>
        /// <returns></returns>
        public static bool IsInt64(this string input)
        {
            long i;
            return long.TryParse(input, out i);
        }

        /// <summary>
        /// 指示指定的对象是否是DateTime
        /// </summary>
        /// <param name="input">字符串</param>
        /// <returns></returns>
        public static bool IsDateTime(this string input)
        {
            DateTime dateTime;
            return DateTime.TryParse(input, out dateTime);
        }

        /// <summary>
        /// 指示指定的对象是否是Boolean
        /// </summary>
        /// <param name="input">字符串</param>
        /// <returns></returns>
        public static bool IsBoolean(this string input)
        {
            bool b;
            return bool.TryParse(input, out b);
        }

        /// <summary>
        /// 指示指定的对象是否是Decimal
        /// </summary>
        /// <param name="input">字符串</param>
        /// <returns></returns>
        public static bool IsDecimal(this string input)
        {
            decimal de;
            return decimal.TryParse(input, out de);
        }

        /// <summary>
        /// 指示指定的对象是否是Double
        /// </summary>
        /// <param name="input">字符串</param>
        /// <returns></returns>
        public static bool IsDouble(this string input)
        {
            double d;
            return double.TryParse(input, out d);
        }

        /// <summary>
        /// 将指定的Base64字符串转化为字节数组
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static byte[] FromBase64String(this string input)
        {
            return Convert.FromBase64String(input);
        }

        /// <summary>
        /// 验证Email格式
        /// </summary>
        /// <param name="email">邮件地址</param>
        /// <returns></returns>
        public static bool IsValidEmailAddress(this string email)
        {
            return email.IsMatch(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
        }
        #endregion

        #region 帕斯卡和骆驼命名法
        /// <summary>
        /// 转换为骆驼命名法（第一个单词首字母小写。）
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <returns></returns>
        public static string ToCamel(this string input)
        {
            if (input.IsNullOrEmpty())
            {
                return input;
            }
            return input[0].ToString().ToLower() + input.Substring(1);
        }

        /// <summary>
        /// 转换为帕斯卡命名法（第一个单词首字母大写。）
        /// </summary>
        /// <param name="input">源字符串</param>
        /// <returns></returns>
        public static string ToPascal(this string input)
        {
            if (input.IsNullOrEmpty())
            {
                return input;
            }
            return input[0].ToString().ToUpper() + input.Substring(1);
        }
        #endregion

        #region 算法
        /// <summary>
        /// 计算字符串的 MD5 哈希。若字符串为空，则返回空，否则返回计算结果。
        /// </summary>
        public static string ComputeMd5Hash(this string str)
        {
            var hash = str;
            if (str != null)
            {
                var md5 = new MD5CryptoServiceProvider();
                var data = Encoding.ASCII.GetBytes(str);
                data = md5.ComputeHash(data);
                hash = "";
                for (var i = 0; i < data.Length; i++)
                {
                    hash += data[i].ToString("x2");
                }
            }
            return hash;
        }

        /// <summary>
        /// 十六进制大小比较
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>-1 a 大;0 相等;1 b 大</returns>
        public static int CompareToBase16(string a, string b)
        {
            var isAempty = string.IsNullOrEmpty(a);
            var isBempty = string.IsNullOrEmpty(b);
            if (isAempty && isBempty)
            {
                return 0;
            }

            if (isAempty)
            {
                return 1;
            }

            if (isBempty)
            {
                return -1;
            }

            if (a.ToLower().IndexOf("0x", StringComparison.InvariantCulture) >= 0)
            {
                a = a.Substring(2);
            }
            if (b.ToLower().IndexOf("0x", StringComparison.InvariantCulture) >= 0)
            {
                b = b.Substring(2);
            }
            var aa = Convert.ToInt64(a, 16);
            var bb = Convert.ToInt64(b, 16);
            if (aa > bb)
            {
                return -1;
            }
            if (aa == bb)
            {
                return 0;
            }
            return 1;
        }
        #endregion

        #region 其余扩展方法
        /// <summary>
        /// Indicates whether the specified string is null or an Empty string.
        /// </summary>
        /// <param name="s">The string to test.</param>
        /// <returns>true if the value parameter is null or an empty string (""); otherwise, false.</returns>
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        /// <summary>
        /// 从当前 System.String 对象移除所有前导空白字符和尾部空白字符。(自动判断字符串是否为空，如果为空则返回空字符串)
        /// </summary>
        /// <param name="s">字符串</param>
        /// <returns></returns>
        public static string ToTrim(this string s)
        {
            return s.IsNullOrEmpty()
                       ? string.Empty
                       : s.Trim();
        }

        /// <summary>
        /// 将指定字符串中的格式项替换为指定数组中相应参数实例的值的文本等效项
        /// </summary>
        /// <param name="format">符合格式字符串</param>
        /// <param name="args">要格式化的参数</param>
        /// <returns></returns>
        public static string FormatWith(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        /// <summary>
        /// 删除最后结尾的指定字符后的字符
        /// </summary>
        /// <param name="s">源字符串</param>
        /// <param name="strchar">分隔符</param>
        /// <returns>去掉结尾字符后的字符串</returns>
        public static string DelLastChar(this string s, string strchar)
        {
            return s.Substring(0, s.LastIndexOf(strchar));
        }

        /// <summary>
        /// 字符串比较，忽略大小写
        /// </summary>
        /// <param name="strA">字符串A</param>
        /// <param name="strB">字符串B</param>
        /// <returns>比较结果</returns>
        public static bool CompareWith(this string strA, string strB)
        {
            return string.Compare(strA, strB, StringComparison.OrdinalIgnoreCase) == 1;
        }

        /// <summary>
        /// "abc\0\0" => "abc"
        /// </summary>
        public static string TrimEndZero(this string s)
        {
            if (s == null)
            {
                return null;
            }

            var indexOf = s.IndexOf('\0');
            if (indexOf == -1)
            {
                return s;
            }

            return s.Substring(0, indexOf);
        }

        public static string Reverse(this string original)
        {
            var arr = original.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }
        #endregion
    }
}