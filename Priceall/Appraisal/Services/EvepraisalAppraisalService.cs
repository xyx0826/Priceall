using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Priceall.Appraisal
{
    class EvepraisalAppraisalService : IAppraisalService
    {
        public Task<AppraisalResult> AppraiseAsync(string content)
        {
            throw new NotImplementedException();
        }

        public AppraisalMarket GetAvailableMarkets()
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<AppraisalSettings> GetCustomSettings()
        {
            throw new NotImplementedException();
        }

        public void SetCurrentMarket(AppraisalMarket market)
        {
            throw new NotImplementedException();
        }
    }
}
