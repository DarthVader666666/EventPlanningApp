﻿using Microsoft.Extensions.Configuration;
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
            Key = Encoding.UTF8.GetBytes(_configuration["AesKey"] ?? throw new ArgumentNullException("AesKey is null"));
            IV = Encoding.UTF8.GetBytes(_configuration["AesIV"] ?? throw new ArgumentNullException("IV is null"));
        }

        public string Encrypt(string? plainText)
        {
            using var aesAlg = Aes.Create();
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            
            using var memoryStream = new MemoryStream();
            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            {
                cryptoStream.Write(Encoding.UTF8.GetBytes(plainText ?? string.Empty));
            }

            return Convert.ToBase64String(memoryStream.ToArray());
        }

        public string Decrypt(string? encryptedText)
        {
            using var aesAlg = Aes.Create();
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using var memoryStream = new MemoryStream(Convert.FromBase64String(encryptedText ?? string.Empty));
            using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            using var streamReader = new StreamReader(cryptoStream);
            return streamReader.ReadToEnd();
        }
    }
}
