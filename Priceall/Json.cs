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

        public float BuyValue
        {
            get
            {
                return _responseObject.SelectToken("appraisal.totals.buy")
                    .ToObject<float>();
            }
        }

        public float SellValue
        {
            get
            {
                return _responseObject.SelectToken("appraisal.totals.sell")
                    .ToObject<float>();
            }
        }

        public float Volume
        {
            get
            {
                return _responseObject.SelectToken("appraisal.totals.volume")
                    .ToObject<float>();
            }
        }
    }
}
