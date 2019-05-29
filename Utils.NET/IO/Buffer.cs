using System;
using System.Collections.Generic;
using System.Text;

namespace Utils.NET.IO
{
    public class Buffer
    {
        public byte[] data;

        public int maxSize;

        public int size;

        public int RemainingLength => maxSize - size;

        public Buffer(int maxSize)
        {
            this.maxSize = maxSize;
            data = new byte[maxSize];
            size = 0;
        }

        public void AddData(byte[] data, int offset, int length)
        {
            Array.Copy(data, offset, this.data, size, length);
            size += length;
        }

        public void AddData(Array data, int offset, int length)
        {
            System.Buffer.BlockCopy(data, offset, this.data, size, length);
            size += length;
        }

        public void AddByte(byte b)
        {
            data[size] = b;
            size++;
        }

        public void Reset(int size)
        {
            //byte[] newData = new byte[size];
            //Array.Copy(data, 0, newData, 0, Math.Min(this.size, size));
            data = new byte[size];
            maxSize = size;
            this.size = 0;
        }
    }
}
