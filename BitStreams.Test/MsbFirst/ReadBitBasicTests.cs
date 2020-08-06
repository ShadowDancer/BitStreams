using Xunit;

namespace BitStreams.Test.MsbFirst
{
    public class ReadBitBasicTests
    {
        private const int _firstByte = 0b00101101;
        private const int _secondByte = 0b11011101;
        private const int _thirdByte = 0b10110101;

        private readonly BitStream _testObj;

        public ReadBitBasicTests()
        {
            _testObj = BitStreamUtils.FromBytes(BitDirection.MsbFirst, _firstByte, _secondByte, _thirdByte);
        }

        [Fact]
        public void ReadByteBitByBit()
        {
            byte result = 0;
            for (int i = 7; i >= 0; i--)
            {
                result |= (byte)(_testObj.ReadBit() << i);
            }

            Assert.Equal(_firstByte, result);
        }

        [Fact]
        public void Read7BitsThenByte_ReturnsCorrectByte()
        {
            for (int i = 6; i >= 0; i--)
            {
                _testObj.ReadBit();
            }

            int result = _testObj.ReadByte();

            Assert.Equal(0b11101110, result);
        }

        [Fact]
        public void ReadBitThenByte_ReturnsCorrectByte()
        {
            for (int i = 0; i >= 0; i--)
            {
                _testObj.ReadBit();
            }

            int result = _testObj.ReadByte();

            Assert.Equal(0b01011011, result);
        }

        [Fact]
        public void ReadBitThenBytes_ReturnsCorrectBytes()
        {
            for (int i = 0; i >= 0; i--)
            {
                _testObj.ReadBit();
            }

            int result = _testObj.ReadByte();
            int result2 = _testObj.ReadByte();

            Assert.Equal(0b01011011, result);
            Assert.Equal(0b10111011, result2);
        }
        [Fact]
        public void Read3BitsThenByte_ReturnsCorrectByte()
        {
            for (int i = 3; i >= 0; i--)
            {
                _testObj.ReadBit();
            }

            int result = _testObj.ReadByte();

            Assert.Equal(0b11011101, result);
        }

        [Fact]
        public void ReadBitOnEmptyBuffer_ReturnsEOS()
        {
            _testObj.ReadByte();
            _testObj.ReadByte();
            _testObj.ReadByte();

            int result = _testObj.ReadBit();

            Assert.Equal(-1, result);
        }

    }
}
