using Domen.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.ApiClients
{
    public interface IProbeScannerApiClient
    {
        public Task<IEnumerable<ProbeScanItem>> GetProbeScanResults();
        public Task WarpToAnomaly(ProbeScanItem probeScanItem);
    }
}
