using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZutoBrewBot.Services.Interfaces;

namespace ZutoBrewBot.Services
{
    public class MannersMessageGenerator : IMannersMessageGenerator
    {
        private static Random _rnd;
        private static string[] _mannersMessages;

        public MannersMessageGenerator()
        {
            if (_rnd == null)
                _rnd = new Random();

            if (_mannersMessages == null)
                _mannersMessages = new string[] {
                    "Manners don't cost a penny, you know...",
                    "Ah ah aaah... You forgot to say the magic word!",
                    "Never forget that manners maketh the man...",
                    "The coffee is free, and so are good manners...",
                    "Politeness is a sign of dignity, not subservience...",
                    "Manners matter. Good looks are a bonus. Humour is a MUST...",
                    "KEEP CALM and ALWAYS SAY PLEASE"
                };
        }

        public string GetMannerResponse()
        {
            var randomIndex = _rnd.Next(0, _mannersMessages.Length - 1);
            return _mannersMessages[randomIndex];
        }
    }
}
