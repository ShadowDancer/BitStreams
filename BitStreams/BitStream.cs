using System;
using System.IO;

namespace BitStreams
{
    /// <summary>
    ///     Stream wrapper, which exposes bit read operation.
    ///     Additional byte will be read/written to stream to make this possible.
    /// </summary>
    public class BitStream : Stream
    {
        private readonly BitDirection _direction;

        /// <summary>
        ///     Amount of bits in internal buffer.
        ///     When 0 next ReadBit will fetch byte from stream
        /// </summary>
        public int BitOffset { get; private set; }

        /// <summary>
        ///     Initalizez stream wrapping internal memory stream
        /// </summary>
        /// <param name="direction"></param>
        public BitStream(BitDirection direction)
        {
            _direction = direction;
            Inner = new MemoryStream();
        }


        /// <summary>
        ///     Initializes inner stream, which this instance of bitstream will operate on
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="inner"></param>
        public BitStream(BitDirection direction, Stream inner)
        {
            _direction = direction;
            Inner = inner;
        }

        /// <summary>
        ///     Stream wrapped by this instance of BitStream
        /// </summary>
        public Stream Inner { get; }

        public void WriteBit(int i)
        {
            if (i > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(i), "Valid values are 0 and 1");
            }

            WriteBit(i != 0);
        }

        public void WriteBit(bool set)
        {
            if (WriteBitOffset == 0)
            {
                WriteBitOffset = 8;
                WriteAccumulator = 0;
            }

            WriteBitOffset -= 1;
            if (set)
            {
                int offset = _direction == BitDirection.MsbFirst ? WriteBitOffset : 7 - WriteBitOffset;
                WriteAccumulator |= (byte)(1 << offset);
            }

            if (WriteBitOffset == 0)
            {
                Write(new[] {WriteAccumulator});
            }
        }

        public int ReadBit()
        {
            if (BitOffset == 0)
            {
                int result = ReadByte();
                if (result == -1)
                {
                    return -1;
                }

                ReadAccumulator = (byte)result;
                BitOffset = 8;
            }

            BitOffset -= 1;
            var offset = _direction == BitDirection.MsbFirst ? BitOffset : 7 - BitOffset;
            return (ReadAccumulator & (1 << offset)) > 0 ? 1 : 0;
        }

        /// <inheritdoc />
        public override void Write(byte[] buffer, int offset, int count)
        {
            Write(buffer.AsSpan(offset, count));
        }

        public override void Write(ReadOnlySpan<byte> buffer)
        {
            if (WriteBitOffset == 0)
            {
                Inner.Write(buffer);
                return;
            }

            byte[] writeBuffer = new byte[buffer.Length];
            buffer.CopyTo(writeBuffer);
            byte newAccumulator;
            if (_direction == BitDirection.MsbFirst)
            {
                int offset = WriteBitOffset;
                byte mask = _mask[offset];
                byte reverseMask = (byte)(byte.MaxValue - mask);
                int bitsLeft = 8 - offset;
                newAccumulator = (byte)((writeBuffer[^1] & (reverseMask >> offset)) <<
                                                offset);
                for (int i = writeBuffer.Length - 1; i > 0; i--)
                {
                    writeBuffer[i] >>= bitsLeft;
                    writeBuffer[i] |= (byte)((writeBuffer[i - 1]) << bitsLeft);
                }

                writeBuffer[0] >>= bitsLeft;
                writeBuffer[0] |= WriteAccumulator;
            }
            else
            {
                int offset = 8 - WriteBitOffset;
                byte mask = _mask[offset];
                newAccumulator = (byte)((writeBuffer[^1] & (mask << 8 - offset)) >> (8 - offset));
                for (int i = writeBuffer.Length - 1; i > 0; i--)
                {
                    writeBuffer[i] <<= offset;
                    writeBuffer[i] |= (byte)((writeBuffer[i - 1]) >> 8 - offset);
                }

                writeBuffer[0] <<= offset;
                writeBuffer[0] |= WriteAccumulator;
            }

            WriteAccumulator = newAccumulator;
            Inner.Write(writeBuffer);
        }

        /// <inheritdoc />
        public override int Read(byte[] buffer, int offset, int count)
        {
            return Read(buffer.AsSpan(offset, count));
        }

        public override int Read(Span<byte> span)
        {
            if (span.Length == 0)
            {
                return 0;
            }

            if (BitOffset == 0)
            {
                return Inner.Read(span);
            }

            int actual = Inner.Read(span);
            if (actual == 0)
            {
                return 0;
            }

            byte newAccumulator;
            if (_direction == BitDirection.MsbFirst)
            {
                int offset = BitOffset;
                byte mask = _mask[offset];
                int bitsLeft = 8 - offset;
                newAccumulator = (byte)(span[actual - 1] & mask);
                for (int i = actual - 1; i >= 1; i++)
                {
                    span[i] = (byte)(span[i] >> offset);
                    span[i] |= (byte)((span[i] & mask) << bitsLeft);
                }

                span[0] = (byte)(span[0] >> offset);
                span[0] |= (byte)(ReadAccumulator << bitsLeft);
            }
            else
            {
                int offset = 8 - BitOffset;
                byte mask = (byte)((byte.MaxValue - _mask[offset]));
                int bitsLeft = 8 - offset;
                newAccumulator = (byte)(span[actual - 1] & mask);
                for (int i = actual - 1; i >= 1; i++)
                {
                    span[i] = (byte)(span[i] >> offset);
                    span[i] |= (byte)((span[i] & mask) << bitsLeft);
                }

                span[0] = (byte)(span[0] << bitsLeft);
                span[0] |= (byte)(ReadAccumulator >> offset);

            }
            ReadAccumulator = newAccumulator;
            return actual;
        }

        /// <summary>
        ///     Stores extra byte fetched from stream while reading
        /// </summary>
        private byte ReadAccumulator { get; set; }

        /// <summary>
        ///     Stores bits waiting for full byte to write to stream
        /// </summary>
        private byte WriteAccumulator { get; set; }

        /// <summary>
        ///     Offset in _writeAccumulator, 0 means that whole byte may be dumped into stream
        /// </summary>
        private int WriteBitOffset { get; set; }


        private static readonly byte[] _mask =
        {
            0b0000000, 0b00000001, 0b00000011, 0b00000111, 0b00001111, 0b00011111, 0b00111111, 0b01111111
        };


        /// <inheritdoc />
        public override long Seek(long offset, SeekOrigin origin)
        {
            if (origin == SeekOrigin.Current && BitOffset != 0)
            {
                throw new NotImplementedException();
            }

            return Inner.Seek(offset, origin);
        }

        /// <inheritdoc />
        public override void SetLength(long value)
        {
            Inner.SetLength(value);
        }

        /// <inheritdoc />
        public override void Flush()
        {
            if (WriteBitOffset != 0)
            {
                WriteBitOffset = 0;
                WriteByte(WriteAccumulator);
            }

            Inner.Flush();
        }

        /// <inheritdoc />
        public override bool CanRead => Inner.CanRead;

        /// <inheritdoc />
        public override bool CanSeek => Inner.CanSeek;

        /// <inheritdoc />
        public override bool CanWrite => Inner.CanWrite;

        /// <inheritdoc />
        public override long Length => Inner.Length;

        /// <inheritdoc />
        public override long Position
        {
            get => Inner.Position;
            set => Inner.Position = value;
        }
    }
}