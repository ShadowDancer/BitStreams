namespace BitStreams
{
    public enum BitDirection
    {
        /// <summary>
        ///     Start writing bits from MSB
        /// </summary>
        /// <example>writing 1 to 00000000 equals in 10000000</example>
        MsbFirst = 0,

        /// <summary>
        ///     Start writing bits from LSB
        /// </summary>
        /// <example>writing 1 to 00000000 equals in 00000001</example>
        LsbFirst = 1
    }
}