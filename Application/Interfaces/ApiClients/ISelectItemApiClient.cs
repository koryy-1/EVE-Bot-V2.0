using Domen.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.ApiClients
{
    public interface ISelectItemApiClient
    {
        public Task<SelectedItemInfo> GetSelectItemInfo();
        public Task<bool> ClickButton(string btnName);
    }
}
