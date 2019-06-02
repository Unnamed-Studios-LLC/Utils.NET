using System;
namespace Utils.NET.IO.Tbon
{
    public abstract class TToken
    {
        public abstract bool IsValue { get; }

        public abstract bool IsObject { get; }

        public abstract T Value<T>();
    }
}
