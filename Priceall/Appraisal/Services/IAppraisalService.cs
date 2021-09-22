using System.Collections.Generic;
using System.Threading.Tasks;
using Priceall.Services;

namespace Priceall.Appraisal
{
    /// <summary>
    /// An online appraisal service.
    /// </summary>
    interface IAppraisalService
    {
        /// <summary>
        /// Gets all markets supported by the appraisal service.
        /// </summary>
        /// <returns>The enum flag combination of all supported markets.</returns>
        AppraisalMarket GetAvailableMarkets();

        /// <summary>
        /// Sets the current market to be used by the appraisal service.
        /// </summary>
        /// <param name="market">The market to use.</param>
        void SetCurrentMarket(AppraisalMarket market);

        /// <summary>
        /// Gets a collection of custom settings used by the appraisal service.
        /// </summary>
        /// <returns>A read-only collection of custom settings.</returns>
        IReadOnlyCollection<JsonSetting> GetCustomSettings();

        /// <summary>
        /// Asynchronously runs an appraisal.
        /// </summary>
        /// <param name="content">The content to appraise.</param>
        /// <returns>Appraisal result.</returns>
        Task<AppraisalResult> AppraiseAsync(string content);
    }
}
