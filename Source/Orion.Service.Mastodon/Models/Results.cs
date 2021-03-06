﻿using System.Collections.Generic;

using Newtonsoft.Json;

namespace Orion.Service.Mastodon.Models
{
    public class Results
    {
        [JsonProperty("accounts")]
        public List<Account> Accounts { get; set; }

        [JsonProperty("statuses")]
        public List<Status> Statuses { get; set; }

        [JsonProperty("hashtags")]
        public List<string> Hashtags { get; set; }
    }
}