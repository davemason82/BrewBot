using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZutoBrewBot.Logging
{
    public interface IMemoryLog : ILog
    {
        string[] FullLog();
    }
}
