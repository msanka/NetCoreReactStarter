using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace native_api_proxy_module.Utilities
{
    public interface ICryptoHelper
    {
        string encrypt(string valueToEncrypt);
        string decrypt(string valueToDecrypt);
    }

    public class CryptoHelper : ICryptoHelper
    {
        private IConfiguration _config;
        private string _symmetricKey = "symmetricKey";
        private string _rijndaelManagedPaddingMode = "PKCS7";
        private bool _isLegacyJWT = false;
        
        public CryptoHelper(IServiceProvider serviceProvider)
        {
            _config = serviceProvider.GetInstance<IConfiguration>(); 

            if (_config != null)
            {
                _symmetricKey = _config["AppDataSecurity:symmetricKey"];
                _rijndaelManagedPaddingMode = _config["AppDataSecurity:rijndaelManagedPaddingMode"];
                _isLegacyJWT = string.IsNullOrWhiteSpace(_config["AppDataSecurity:isLegacyJWT"]) ? false : Boolean.Parse(_config["AppDataSecurity:isLegacyJWT"]);
            }
        }

        public string encrypt(string plainText)
        {
            if (_isLegacyJWT)
            {
                return EncryptLegacy(plainText, _symmetricKey);
            }
            else
            {
                if (String.IsNullOrWhiteSpace(plainText))
                {
                    throw new ArgumentNullException("plainText", "parameter 'plainText' cannot be null or empty");
                }

                var plainBytes = Encoding.UTF8.GetBytes(plainText);
                RijndaelManaged rijndaelManaged = GetCryptor(_symmetricKey);
                byte[] bytes = encrypt(plainBytes, rijndaelManaged);
                return Convert.ToBase64String(bytes);
            }
        }
        public string decrypt(string encryptedText)
        {
            if (_isLegacyJWT)
            {
                return DecryptLegacy(encryptedText, _symmetricKey);
            }
            else
            {
                if (encryptedText == null)
                {
                    throw new ArgumentNullException("encryptedText", "parameter 'encryptedText' cannot be null or empty");
                }
                if (encryptedText.Trim() == "")
                {
                    return string.Empty;
                }
                
                var encryptedBytes = Convert.FromBase64String(encryptedText);
                RijndaelManaged rijndaelManaged = GetCryptor(_symmetricKey);
                byte[] bytes = decrypt(encryptedBytes, rijndaelManaged);
                return Encoding.UTF8.GetString(bytes);
            }
        }


        #region Helper Methods

        private RijndaelManaged GetCryptor(String secretKey)
        {
            var keyBytes = new byte[16];
            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
            Array.Copy(secretKeyBytes, keyBytes, Math.Min(keyBytes.Length, secretKeyBytes.Length));
            PaddingMode paddingMode = (PaddingMode)Enum.Parse(typeof(PaddingMode), _rijndaelManagedPaddingMode);
            /* 
            switch (padding)
            {
                case "1":
                    paddingMode = PaddingMode.None;
                    break;
                case "2":
                    paddingMode = PaddingMode.PKCS7;
                    break;
                case "3":
                    paddingMode = PaddingMode.Zeros;
                    break;
                case "4":
                    paddingMode = PaddingMode.ANSIX923;
                    break;
                case "5":
                    paddingMode = PaddingMode.ISO10126;
                    break;
                default:
                    paddingMode = PaddingMode.PKCS7;
                    break;
            }
            */

            return new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                Padding = paddingMode,
                KeySize = 256,
                BlockSize = 128,
                Key = keyBytes,
                IV = keyBytes
            };
        }

        private byte[] encrypt(byte[] plainBytes, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateEncryptor()
                .TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        }

        private byte[] decrypt(byte[] encryptedData, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateDecryptor()
                .TransformFinalBlock(encryptedData, 0, encryptedData.Length);
        }

        private string EncryptLegacy(string text, string symmetricKey)
        {
            DeriveBytes rgb = new Rfc2898DeriveBytes(symmetricKey, Encoding.Unicode.GetBytes(symmetricKey));
            var algorithm = Aes.Create();
            algorithm.Key = rgb.GetBytes(algorithm.KeySize >> 3);
            algorithm.IV = rgb.GetBytes(algorithm.BlockSize >> 3);
            ICryptoTransform transform = algorithm.CreateEncryptor();
            using (MemoryStream buffer = new MemoryStream())
            {
                using (CryptoStream stream = new CryptoStream(buffer, transform, CryptoStreamMode.Write))
                {
                    using (StreamWriter writer = new StreamWriter(stream, Encoding.Unicode))
                    {
                        writer.Write(text);
                    }
                }
                return Convert.ToBase64String(buffer.ToArray());
            }
        }

        private string DecryptLegacy(string text, string symmetricKey)
        {
            DeriveBytes rgb = new Rfc2898DeriveBytes(symmetricKey, Encoding.Unicode.GetBytes(symmetricKey));
            var algorithm = Aes.Create();
            algorithm.Key = rgb.GetBytes(algorithm.KeySize >> 3);
            algorithm.IV = rgb.GetBytes(algorithm.BlockSize >> 3);
            ICryptoTransform transform = algorithm.CreateDecryptor();
            byte[] non64Text = Convert.FromBase64String(text);
            using (MemoryStream buffer = new MemoryStream(non64Text))
            {
                using (CryptoStream stream = new CryptoStream(buffer, transform, CryptoStreamMode.Read))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.Unicode))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }

        #endregion

    }
}