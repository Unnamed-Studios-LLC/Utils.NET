using System;
using System.IO;

namespace Utils.NET.IO.Tbon
{
    public class TbonReader
    {
        /// <summary>
        /// Reader used to get text from the stream
        /// </summary>
        private TextReader reader;

        public TbonReader(TextReader reader)
        {
            this.reader = reader;
        }


    }
}
