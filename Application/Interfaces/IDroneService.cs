using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IDroneService
    {
        public Task Launch();
        public Task EnsureEngage();
        public Task Scoop();
    }
}
