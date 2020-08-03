using System.IO;

namespace BitStreams.Test
{
    public static class BitStreamUtils
    {
        public static BitStream FromBytes(params byte[] bytes)
        {
            return new BitStream(new MemoryStream(bytes));
        }
    }
}
