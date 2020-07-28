namespace Priceall.Helpers
{
    static class ValueHelper
    {
        /// <summary>
        /// Gets the formatted (pretty-printed) presentation of currency value.
        /// </summary>
        /// <param name="value">The value to format.</param>
        /// <returns>Pretty-printed string of the specified value.</returns>
        public static string ToPrettyPrint(this double value)
        {
            if (value < 1000)   // do not preformat if below 1k // 不到1k就不格式化
            {
                return value.ToString("F2") + " ISK";
            }
            else if (value < 1000000)    // format in k's if below 1mil // 100w以下用千表示
            {
                return (value / 1000).ToString("F2") + " K";
            }
            else if (value < 1000000000) // format in mils if below 1bil // 10e以下用百万表示
            {
                return (value / 1000000).ToString("F2") + " Mil";
            }
            else // format in bils if higher // 超出10e用十亿表示
            {
                return (value / 1000000000).ToString("F2") + " Bil";
            }
        }
    }
}
