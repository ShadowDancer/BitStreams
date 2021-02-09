namespace BitStreams
{
    public enum BitDirection
    {
        /// <summary>
        ///     Start writing bits from Most significant bit aka left-most bit, high-order bit.
        /// </summary>
        /// <example>writing 1 to 00000000 equals in 10000000</example>
        MsbFirst = 0,

        /// <summary>
        ///     Start writing bits from Least significant bit aka right-most bit, low-order bit.
        /// </summary>
        /// <example>writing 1 to 00000000 equals in 00000001</example>
        LsbFirst = 1
    }
}