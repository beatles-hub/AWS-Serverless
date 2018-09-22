using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AccessKeyValidator.Model
{
    public class AccessKeyMetadata
    {
        [JsonIgnore]
        public string AccessKeyId { get; set; }
        [JsonProperty]
        public DateTime CreateDate{get;set;}
        [JsonProperty]
        public string Status { get; set; }
        [JsonProperty]
        public string UserName { get; set; }
    }
}
