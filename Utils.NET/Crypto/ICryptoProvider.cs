using System;
using System.Text;

namespace Utils.NET.Crypto
{
    public interface ICryptoProvider
    {
        byte[] HashSha1(byte[] input);

        string HashSha1(string input, Encoding encoding);

        string HashSha1(string input);

        byte[] HashSha256(byte[] input);

        string HashSha256(string input, Encoding encoding);

        string HashSha256(string input);
    }
}
