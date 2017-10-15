using Common.Logging;
using Noobot.Core;
using Noobot.Core.Configuration;
using Noobot.Core.DependencyResolution;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ZutoBrewBot.Configuration;

namespace ZutoBrewBot
{
    public class NoobotHost
    {
        private readonly IConfigReader _configReader;
        private INoobotCore _noobotCore;
        private readonly IConfiguration _configuration;

        public NoobotHost(IConfigReader configReader)
        {
            _configReader = configReader;
            _configuration = new BrewbotConfiguration();
        }

        public void Start(ILog log)
        {
            IContainerFactory containerFactory = new ContainerFactory(_configuration, _configReader, log);
            INoobotContainer container = containerFactory.CreateContainer();
            _noobotCore = container.GetNoobotCore();

            _noobotCore
                .Connect()
                .ContinueWith(task =>
                {
                    if (!task.IsCompleted || task.IsFaulted)
                    {
                        Debug.WriteLine($"Error connecting to Slack: {task.Exception}");
                    }
                })
                .GetAwaiter()
                .GetResult();
        }

        public void Stop()
        {
            _noobotCore?.Disconnect();
        }
    }
}
