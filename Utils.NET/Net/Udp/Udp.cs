using System;
using Utils.NET.Utils;

namespace Utils.NET.Net.Udp
{
    public static class Udp
    {
        public static ulong GenerateLocalSalt()
        {
            ulong a = (ulong)Rand.IntValue();
            ulong b = (ulong)Rand.IntValue();
            return (a << 32) | b;
        }

        public static ulong CreateSalt(ulong client, ulong server)
        {
            return client ^ server;
        }
    }
}
