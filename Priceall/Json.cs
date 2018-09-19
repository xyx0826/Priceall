using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Priceall
{
    class Json
    {
        static JObject _responseObject;

        /// <summary>
        /// Parses an Evepraisal JSON response into a browsable object.
        /// </summary>
        /// <param name="evePraisalResponse">JSON response from Evepraisal.</param>
        public Json(string evePraisalResponse)
        {
            try
            {
                _responseObject = JObject.Parse(evePraisalResponse);
            }
            catch (JsonReaderException) { throw; }
        }

        /// <summary>
        /// The error message of the response. Returns empty string if error is not found.
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                try
                {
                    return _responseObject.SelectToken("error_message").ToObject<string>();
                }
                catch (NullReferenceException)
                {
                    return String.Empty;
                }
            }
        }

        /// <summary>
        /// Type of the item list identified by Evepraisal.
        /// </summary>
        public string Kind
        {
            get
            {
                return _responseObject.SelectToken("appraisal.kind")
                    .ToObject<string>();
            }
        }

        /// <summary>
        /// Buy value of the item list.
        /// </summary>
        public double BuyValue
        {
            get
            {
                return _responseObject.SelectToken("appraisal.totals.buy")
                    .ToObject<double>();
            }
        }

        /// <summary>
        /// Sell value of the item list.
        /// </summary>
        public double SellValue
        {
            get
            {
                return (double)_responseObject.SelectToken("appraisal.totals.sell");
            }
        }

        /// <summary>
        /// Total volume of the item list.
        /// </summary>
        public double Volume
        {
            get
            {
                return _responseObject.SelectToken("appraisal.totals.volume")
                    .ToObject<double>();
            }
        }

        /// <summary>
        /// Gets the formatted (prettyprinted) presentation of item list value.
        /// </summary>
        /// <param name="buyOrSell">True for sell value, false for buy value. Defaulted to buy value.</param>
        /// <returns>Pretty-printed string of the specified value.</returns>
        public string PrettyPrintValue(bool buyOrSell = false)
        {
            var value = (buyOrSell ? BuyValue : SellValue);

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
