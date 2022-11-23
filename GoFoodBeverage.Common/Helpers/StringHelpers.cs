using System;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;

namespace GoFoodBeverage.Common.Helpers
{
    public static class StringHelpers
    {
        public static readonly Regex REGEX_WHITESPACE = new Regex(@"\s+");
        private static readonly string[] VietnameseSigns = new string[]
        {
            "aAeEoOuUiIdDyY",

            "áàạảãâấầậẩẫăắằặẳẵ",

            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",

            "éèẹẻẽêếềệểễ",

            "ÉÈẸẺẼÊẾỀỆỂỄ",

            "óòọỏõôốồộổỗơớờợởỡ",

            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",

            "úùụủũưứừựửữ",

            "ÚÙỤỦŨƯỨỪỰỬỮ",

            "íìịỉĩ",

            "ÍÌỊỈĨ",

            "đ",

            "Đ",

            "ýỳỵỷỹ",

            "ÝỲỴỶỸ"
        };

        public static string UserCodeGenerator(string type)
        {
            var bytes = new byte[4];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            uint random = BitConverter.ToUInt32(bytes, 0) % 100000000;

            return $"{type}{String.Format("{0:D8}", random)}";
        }

        public static string GeneratePassword(int length = 8)
        {
            string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new();

            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = validChars[random.Next(0, validChars.Length)];
            }

            return new string(chars);
        }

        public static string GenerateValidateCode(int length = 5)
        {
            string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new();
            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = validChars[random.Next(0, validChars.Length)];
            }
            return new string(chars);
        }

        public static TimeSpan? StringToTimeSpan(string input)
        {
            TimeSpan time;
            if (!TimeSpan.TryParse(input, out time))
            {
                return null;
            }

            return time;
        }

        public static string TimeSpanToString(TimeSpan input)
        {
            return input.ToString(@"hh\:mm");
        }

        public static string TimeSpanToHourMinutes(TimeSpan input)
        {
            if (input.Minutes > 30)
            {
                input = input.Add(new TimeSpan(1, 0, 0));
            }

            return String.Format("{0:%h} hours", input);
        }

        public static string FormatPriceVND(decimal? price)
        {
            return $"{string.Format("{0:#,##0}", price ?? 0)} VNĐ";
        }

        /// <summary>
        /// Created date: 2022-01-10
        /// Description: this method is used to generate the order tracking code.
        /// </summary>
        public static string GenerateOrderTrackingCode()
        {
            const string prefix = "BE";
            const int totalNumberOfCharactersToSplit = 9;
            string randomCode = $"{DateTime.Now.Ticks}";
            // For example: 637773999255854918 there are 18 characters in this string.
            // The value will be 255854918.
            randomCode = $"{prefix}{randomCode.Substring(randomCode.Length - totalNumberOfCharactersToSplit, totalNumberOfCharactersToSplit)}";
            return randomCode;

        }

        /// <summary>
        /// Get Company Id From Request Header
        /// </summary>
        /// <param name="httpContextAccessor"></param>
        /// <returns></returns>
        public static Guid GetCompanyIdFromHeader(IHttpContextAccessor httpContextAccessor)
        {
            return new Guid(httpContextAccessor.HttpContext.Request.Headers["workspace-id"]);
        }

        /// <summary>
        /// IsValidEmail
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsValidEmail(this string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// Remove Vietnamese sign (ex: Trường -> Truong
        /// </summary>
        /// <param name="str">Text input</param>
        /// <returns></returns>
        public static string RemoveSign4VietnameseString(string str)
        {
            for (int i = 1; i < VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < VietnameseSigns[i].Length; j++)
                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
            }
            return str;
        }

        #region Aes operation
        private const string SECRET_KEY = "8185f9336386738d3437f9d3050ed70e";

        public static string EncryptString(string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(SECRET_KEY);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        public static string DecryptString(string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(SECRET_KEY);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        public static string GenerateKey()
        {
            var key = Guid.NewGuid();
            return key.ToString().ToUpper();
        }
        #endregion
    }
}
