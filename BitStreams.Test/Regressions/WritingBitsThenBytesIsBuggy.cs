using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace BitStreams.Test.Regressions
{
    public class WritingBitsThenBytesIsBuggy
    {
        [Theory]
        [InlineData(BitDirection.LsbFirst)]
        [InlineData(BitDirection.MsbFirst)]
        public void WriteBitThenBytesLsb(BitDirection direction)
        {
                BitStream stream = new BitStream(direction);
                stream.WriteBit(true);
                stream.Write(BitConverter.GetBytes((ushort)0xFFFF));
                stream.Flush();

                stream.Seek(0, SeekOrigin.Begin);

                Assert.Equal(1, stream.ReadBit());
                Span<byte> bytes = new byte[2];
                stream.Read(bytes);
                ushort us = BitConverter.ToUInt16(bytes);

                Assert.Equal(0xFFFF, us);
        }
    }
}
