using System.Collections.Generic;
using System.Threading.Tasks;
using Priceall.Services;

namespace Priceall.Appraisal
{
    class CeveMarketAppraisalService : IAppraisalService
    {
        public Task<AppraisalResult> AppraiseAsync(string content)
        {
            throw new System.NotImplementedException();
        }

        public AppraisalMarket GetAvailableMarkets()
        {
            throw new System.NotImplementedException();
        }

        public IReadOnlyCollection<JsonSetting> GetCustomSettings()
        {
            throw new System.NotImplementedException();
        }

        public void SetCurrentMarket(AppraisalMarket market)
        {
            throw new System.NotImplementedException();
        }
    }
}
