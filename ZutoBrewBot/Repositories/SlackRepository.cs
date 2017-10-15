using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ZutoBrewBot.Configuration;
using ZutoBrewBot.Repositories.Interfaces;

namespace ZutoBrewBot.Repositories
{
    public class SlackRepository : ISlackRepository
    {
        private readonly AppSettings _appSettings;

        public SlackRepository(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public void PostMessage(string messageText)
        {
            HttpClient httpClient = new HttpClient();
            string webhookUrl = _appSettings.SlackWebhookUrl;

            var payload = new
            {
                text = messageText,
                username = _appSettings.SlackWebhookUsername
            };

            var serializedPayload = JsonConvert.SerializeObject(payload);
            HttpResponseMessage response = httpClient.PostAsync(webhookUrl,
                new StringContent(serializedPayload, Encoding.UTF8, "application/json")).Result;
        }
    }
}
