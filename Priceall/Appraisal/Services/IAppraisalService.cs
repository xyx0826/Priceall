using System.Collections.Generic;
using System.Threading.Tasks;

namespace Priceall.Appraisal
{
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
        IReadOnlyCollection<AppraisalSettings> GetCustomSettings();

        Task<AppraisalResult> AppraiseAsync(string content);
    }
}
