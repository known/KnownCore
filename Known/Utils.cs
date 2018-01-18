using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Known
{
    public class Utils
    {
        #region Base
        public static string NewGuid
        {
            get { return Guid.NewGuid().ToString().ToLower().Replace("-", ""); }
        }

        public static bool IsNullOrEmpty(object value)
        {
            return value == null || value == DBNull.Value || string.IsNullOrWhiteSpace(value.ToString());
        }

        public static T ConvertTo<T>(object value, T defaultValue = default(T))
        {
            if (value == null || value == DBNull.Value)
                return defaultValue;

            var valueString = value.ToString();
            var type = typeof(T);
            if (type == typeof(string))
                return (T)Convert.ChangeType(valueString, type);

            valueString = valueString.Trim();
            if (valueString.Length == 0)
                return defaultValue;

            if (type.IsEnum)
                return (T)Enum.Parse(type, valueString, true);

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                type = Nullable.GetUnderlyingType(type);

            if (type == typeof(bool) || type == typeof(bool?))
                valueString = ",1,Y,YES,TRUE,".Contains(valueString.ToUpper()) ? "True" : "False";

            try
            {
                return (T)Convert.ChangeType(valueString, type);
            }
            catch
            {
                return defaultValue;
            }
        }
        #endregion

        #region Encrypt
        public static string ToMd5(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            byte[] bytes;
            using (var md5 = MD5.Create())
            {
                bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(value));
            }

            var sb = new StringBuilder();
            foreach (var item in bytes)
            {
                sb.Append(item.ToString("x2"));
            }
            return sb.ToString();
        }

        public static string Encrypt(string value, string password = "")
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var key = new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(password));
            var des = new TripleDESCryptoServiceProvider() { Key = key, Mode = CipherMode.ECB };
            var datas = Encoding.UTF8.GetBytes(value);
            var bytes = des.CreateEncryptor().TransformFinalBlock(datas, 0, datas.Length);
            return Convert.ToBase64String(bytes);
        }

        public static string Decrypt(string value, string password = "")
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            var key = new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(password));
            var des = new TripleDESCryptoServiceProvider() { Key = key, Mode = CipherMode.ECB };
            var datas = Convert.FromBase64String(value);
            var bytes = des.CreateDecryptor().TransformFinalBlock(datas, 0, datas.Length);
            return Encoding.UTF8.GetString(bytes);
        }
        #endregion

        #region File
        public static string EnsureFile(string fileName)
        {
            var fileInfo = new FileInfo(fileName);
            if (!fileInfo.Directory.Exists)
                fileInfo.Directory.Create();

            return fileName;
        }

        public static void DeleteFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return;

            if (File.Exists(fileName))
                File.Delete(fileName);
        }

        public static string GetFileExtName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return string.Empty;

            var index = fileName.LastIndexOf('.');
            return fileName.Substring(index).ToLower();
        }
        #endregion
    }
}
