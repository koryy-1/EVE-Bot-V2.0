using Domen.Entities;
using Domen.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IBotService
    {
        public bool IsConfigLoaded { get; }
        public void StartBotServices();
        public void StopBotServices();
        public void AuthorizeExecutor();
        public void DenyExecutorAuthorization();
        public BotState GetBotState();
        public BotStatus GetBotStatus();
        public BotConfig GetConfig();
        public void LoadConfig(BotConfig config);
    }
}
