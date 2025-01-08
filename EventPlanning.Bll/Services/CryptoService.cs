using EventPlanning.Bll.Enums;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace EventPlanning.Bll.Services
{
    public class CryptoService
    {
        private readonly IConfiguration _configuration;
        private readonly byte[] Key;
        private readonly byte[] IV;

        public CryptoService(IConfiguration configuration)
        {
            _configuration = configuration;
            Key = Encoding.UTF8.GetBytes(_configuration["EncryptionKey"] ?? throw new ArgumentNullException("EncryptionKey is null"));
            IV = Encoding.UTF8.GetBytes(_configuration["IV"] ?? throw new ArgumentNullException("IV is null"));
        }

        public string Encrypt(string text)
        {
            return GetText(CryptType.Encrypt, text);
        }

        public string Decrypt(string text)
        {
            string res = null;

            try
            {
                res = GetText(CryptType.Decrypt, text);
            }
            catch (Exception ex)
            { 
                
            }

            return res;
        }

        private string GetText(CryptType cryptType, string text)
        {
            using var aesAlg = Aes.Create();
            //aesAlg.BlockSize = 128;

            aesAlg.Key = Key;
            aesAlg.IV = IV;

            var crypt = cryptType switch
            {
                CryptType.Encrypt => aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV),
                CryptType.Decrypt => aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV),
                _ => throw new ArgumentException("Wrong CryptType.")
            };

            using var memoryStream = new MemoryStream();
            using (var cryptoStream = new CryptoStream(memoryStream, crypt, CryptoStreamMode.Write))
            {
                var bytes = Encoding.UTF8.GetBytes(text);
                cryptoStream.Write(bytes);
            }

            var result = Convert.ToBase64String(memoryStream.ToArray());

            return result;
        }
    }
}
