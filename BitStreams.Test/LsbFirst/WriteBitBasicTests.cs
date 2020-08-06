using System.IO;
using Xunit;

namespace BitStreams.Test.LsbFirst
{
    public class WriteBitLsbFirstBasicTests
    {
        private readonly MemoryStream _memoryStream;
        private readonly BitStream _testObj;

        public WriteBitLsbFirstBasicTests()
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
            Assert.Equal(0b11111111, result[0]);
            Assert.Equal(0b00111010, result[1]);
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
            Assert.Equal(0b11101011, result[0]);
            Assert.Equal(0b00110010, result[1]);
            Assert.Equal(0b00000001, result[2]);
        }

        [Fact]
        public void WriteBitThenBytes2()
        {
            _testObj.WriteBit(true);
            _testObj.Write(new byte[] { 0b11111111, 0b11111111 });

            _testObj.Flush();

            var result = GetResult();
            Assert.Equal(0b11111111, result[0]);
            Assert.Equal(0b11111111, result[1]);
            Assert.Equal(0b00000001, result[2]);
        }

        [Fact]
        public void WriteBitThenBytes3()
        {
            _testObj.WriteBit(true);
            _testObj.Write(new byte[] { 0b10000000, 0b00000001 });

            _testObj.Flush();

            var result = GetResult();
            Assert.Equal(0b00000001, result[0]);
            Assert.Equal(0b00000011, result[1]);
            Assert.Equal(0b00000000, result[2]);
        }

        private byte[] GetResult()
        {
            _testObj.Flush();
            _memoryStream.Seek(0, SeekOrigin.Begin);
            return _memoryStream.ToArray();
        }
    }
}
