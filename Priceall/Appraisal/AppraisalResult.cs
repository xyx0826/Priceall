using Priceall.Helpers;
using System.Windows.Data;

namespace Priceall.Appraisal
{
    /// <summary>
    /// An appraisal result.
    /// </summary>
    class AppraisalResult
    {
        /// <summary>
        /// The outcome of the appraisal.
        /// </summary>
        public AppraisalStatus Status;

        /// <summary>
        /// The error message, if any, of the appraisal.
        /// </summary>
        public string Message;

        /// <summary>
        /// The identified type of the appraisal content.
        /// </summary>
        public string Kind;

        /// <summary>
        /// The total buy value of the appraisal.
        /// </summary>
        public double BuyValue;

        /// <summary>
        /// The total sell value of the appraisal.
        /// </summary>
        public double SellValue;

        /// <summary>
        /// The total item volume of the appraisal.
        /// </summary>
        public double Volume;

        public AppraisalResult(AppraisalStatus status, string message = "")
        {
            Status = status;
            Message = message;
        }

        /// <summary>
        /// The formatted buy value of the appraisal.
        /// </summary>
        public string FormattedBuyValue => BuyValue.ToPrettyPrint();

        /// <summary>
        /// The formatted sell value of the appraisal.
        /// </summary>
        public string FormattedSellValue => SellValue.ToPrettyPrint();
    }

    /// <summary>
    /// The status of an appraisal result.
    /// </summary>
    enum AppraisalStatus
    {
        Successful,
        ContentError,
        NetworkError,
        InternalError
    }
}
