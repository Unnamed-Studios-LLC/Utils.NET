using System;
using System.Security.Cryptography;
using System.Text;
using Utils.NET.Utils;

namespace Utils.NET.Crypto
{
    public class CryptoProvider : ICryptoProvider
    {
        private SHA1 sha1;

        private SHA256 sha256;

        public CryptoProvider()
        {
            sha1 = SHA1.Create();
            sha256 = SHA256.Create();
        }

        public byte[] HashSha1(byte[] input)
        {
            return sha1.ComputeHash(input);
        }

        public string HashSha1(string input, Encoding encoding)
        {
            return StringUtils.ToHexString(HashSha1(encoding.GetBytes(input)));
        }

        public string HashSha1(string input)
        {
            return HashSha1(input, Encoding.UTF8);
        }

        public byte[] HashSha256(byte[] input)
        {
            return sha256.ComputeHash(input);
        }

        public string HashSha256(string input, Encoding encoding)
        {
            return StringUtils.ToHexString(HashSha256(encoding.GetBytes(input)));
        }

        public string HashSha256(string input)
        {
            return HashSha256(input, Encoding.UTF8);
        }
    }
}
