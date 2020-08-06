using System.IO;
using Xunit;

namespace BitStreams.Test.LsbFirst
{
    public class WriteBitMsbFirstBasicTests
    {
        private readonly MemoryStream _memoryStream;
        private readonly BitStream _testObj;

        public WriteBitMsbFirstBasicTests()
        {
            _memoryStream = new MemoryStream();
            _testObj = new BitStream(BitDirection.LsbFirst, _memoryStream);
        }

        [Fact]
        public void WriteSingleBit()
        {
            _testObj.WriteBit(true);
            _testObj.Flush();

            var result = GetResult();
            Assert.Single(result);
            Assert.Equal(0b00000001, result[0]);
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
            Assert.Equal(0b01010101, result[0]);
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
            Assert.Equal(0b01111111, result[0]);
            Assert.Equal(0b11101010, result[1]);
        }

        [Fact]
        public void WriteBitThenByte()
        {
            _testObj.WriteBit(true);
            _testObj.WriteByte(0b11110101);

            _testObj.Flush();

            var result = GetResult();
            Assert.Equal(0b11101011, result[0]);
            Assert.Equal(0b00000001, result[1]);
        }

        [Fact]
        public void WriteBitThenBytes()
        {
            _testObj.WriteBit(true);
            _testObj.WriteByte(0b01110101);
            _testObj.WriteByte(0b10011001);

            _testObj.Flush();

            var result = GetResult();
            Assert.Equal(0b01110101, result[0]);
            Assert.Equal(0b11001100, result[1]);
            Assert.Equal(0b00000001, result[2]);
        }


        private byte[] GetResult()
        {
            _testObj.Flush();
            _memoryStream.Seek(0, SeekOrigin.Begin);
            return _memoryStream.ToArray();
        }
    }
}
