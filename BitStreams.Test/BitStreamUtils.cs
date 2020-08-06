using System.IO;

namespace BitStreams.Test
{
    public static class BitStreamUtils
    {
        public static BitStream FromBytes(BitDirection direction, params byte[] bytes)
        {
            return new BitStream(direction,new MemoryStream(bytes));
        }
    }
}
