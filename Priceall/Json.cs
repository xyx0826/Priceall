using Newtonsoft.Json.Linq;
using System;

namespace Priceall
{
    class Json
    {
        static JObject _responseObject;

        public Json(string praisalResponse)
        {
            _responseObject = JObject.Parse(praisalResponse);
        }

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

        public string Kind
        {
            get
            {
                return _responseObject.SelectToken("appraisal.kind")
                    .ToObject<string>();
            }
        }

        public double BuyValue
        {
            get
            {
                return _responseObject.SelectToken("appraisal.totals.buy")
                    .ToObject<double>();
            }
        }

        public double SellValue
        {
            get
            {
                return (double)_responseObject.SelectToken("appraisal.totals.sell");
            }
        }

        public string PrettyPrintedValue
        {
            get
            {
                if (SellValue < 1000)   // do not preformat if below 1k // 不到1k就不格式化
                {
                    return SellValue.ToString("F2") + " ISK";
                }
                else if (SellValue < 1000000)    // format in k's if below 1mil // 100w以下用千表示
                {
                    return (SellValue / 1000).ToString("F2") + " K";
                }
                else if (SellValue < 1000000000) // format in mils if below 1bil // 10e以下用百万表示
                {
                    return (SellValue / 1000000).ToString("F2") + " Mil";
                }
                else // format in bils if higher // 超出10e用十亿表示
                {
                    return (SellValue / 1000000000).ToString("F2") + " Bil";
                }
            }
        }

        public double Volume
        {
            get
            {
                return _responseObject.SelectToken("appraisal.totals.volume")
                    .ToObject<double>();
            }
        }
    }
}
