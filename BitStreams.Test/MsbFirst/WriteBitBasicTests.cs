using System.IO;
using Xunit;

namespace BitStreams.Test.MsbFirst
{
    public class WriteBitMsbFirstBasicTests
    {
        private readonly MemoryStream _memoryStream;
        private readonly BitStream _testObj;

        public WriteBitMsbFirstBasicTests()
        {
            _memoryStream = new MemoryStream();
            _testObj = new BitStream(BitDirection.MsbFirst, _memoryStream);
        }

        [Fact]
        public void WriteSingleBit()
        {
            _testObj.WriteBit(true);
            _testObj.Flush();

            var result = GetResult();
            Assert.Single(result);
            Assert.Equal(0b10000000, result[0]);
        }

        [Fact]
        public void WriteSevenBits()
        {
            for (int i = 0; i < 7; i++)
            {
                _testObj.WriteBit(i % 2 == 0);
            }

            _testObj.Flush();

            var result = GetResult();
            Assert.Single(result);
            Assert.Equal(0b10101010, result[0]);
        }

        [Fact]
        public void WriteSevenBitsThenByte()
        {
            for (int i = 0; i < 7; i++)
            {
                _testObj.WriteBit(true);
            }

            _testObj.WriteByte(0b01110101);

            _testObj.Flush();

            var result = GetResult();
            Assert.Equal(0b11111110, result[0]);
            Assert.Equal(0b11101010, result[1]);
        }

        [Fact]
        public void WriteBitThenByte()
        {
            _testObj.WriteBit(true);
            _testObj.WriteByte(0b01110101);

            _testObj.Flush();

            var result = GetResult();
            Assert.Equal(0b10111010, result[0]);
            Assert.Equal(0b10000000, result[1]);
        }

        [Fact]
        public void WriteBitThenBytes()
        {
            _testObj.WriteBit(true);
            _testObj.WriteByte(0b01110101);
            _testObj.WriteByte(0b10011001);

            _testObj.Flush();

            var result = GetResult();
            Assert.Equal(0b10111010, result[0]);
            Assert.Equal(0b11001100, result[1]);
            Assert.Equal(0b10000000, result[2]);
        }


        private byte[] GetResult()
        {
            _testObj.Flush();
            _memoryStream.Seek(0, SeekOrigin.Begin);
            return _memoryStream.ToArray();
        }
    }
}
