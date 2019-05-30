using System;
using System.Collections.Generic;
using System.Text;
using Utils.NET.IO;

namespace Utils.NET.Net
{
    public abstract class Packet
    {
        public abstract byte Id { get; }


        public void WritePacket(BitWriter w)
        {
            w.Write(Id);
            Write(w);
        }

        public void ReadPacket(BitReader r)
        {
            r.ReadUInt8(); // read id
            Read(r);
        }

        protected abstract void Write(BitWriter w);

        protected abstract void Read(BitReader r);
    }
}
