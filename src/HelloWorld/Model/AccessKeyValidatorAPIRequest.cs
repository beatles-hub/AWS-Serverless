using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AccessKeyValidator.Model
{
    public class AccessKeyValidatorAPIRequest
    {
        [JsonProperty]
        public string AccessKeyID { get; set; }
    }
}
