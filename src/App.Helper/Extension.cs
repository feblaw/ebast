using App.Domain.Models.Core;
using MimeKit.Cryptography;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;

namespace App.Helper
{
    public static class Extension
    {
        public static string ToSlug(this string str)
        {
            return str.ToLower().Replace(" ", "_");
        }

        public static string GetFileExtension(this string str)
        {
            return Path.GetExtension(str);
        }

        public static string GetFilename(this string str)
        {
            return Path.GetFileName(str);
        }

        public static string GetFilenameWithoutExtension(this string str)
        {
            return Path.GetFileNameWithoutExtension(str);
        }

        public static string Truncate(this string str, int max)
        {
            if (str == null)
            {
                return "";
            }
            else if (str.Length < max)
            {
                return str;
            }

            return str.Substring(0, max) + "...";
        }

        public static string GetMD5Hash(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }
            byte[] input = Encoding.UTF8.GetBytes(text);
            byte[] output = MD5.Create().ComputeHash(input);
            return Convert.ToBase64String(output);
        }

        public static string GetMD5Hash(this string text, string salt)
        {
            var concatString = text + salt;
            return GetMD5Hash(concatString);
        }

        public static Attachment ConvertToAttachment(this string json)
        {
            try
            {
                var attachment = JsonConvert.DeserializeObject<Attachment>(json);

                return attachment;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string ConvertToString(this Attachment att)
        {
            return JsonConvert.SerializeObject(att);
        }

        public static string RandomColor()
        {
            var random = new Random();
            var color = String.Format("#{0:X6}", random.Next(0x1000000));
            return color.ToString();
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public static int MonthDistance(DateTime first,DateTime last)
        {
            return Math.Abs((first.Month - last.Month) + 12 * (first.Year - last.Year));
        }
    }
}
