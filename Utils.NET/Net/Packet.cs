using System;
using System.Collections.Generic;
using System.Text;
using Utils.NET.IO;

namespace Utils.NET.Net
{
    public abstract class Packet
    {
        public abstract byte Id { get; }

        public abstract void Write(BitWriter w);

        public abstract void Read(BitReader r);
    }
}
