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
