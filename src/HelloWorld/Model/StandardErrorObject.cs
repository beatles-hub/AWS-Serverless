using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace AccessKeyValidator.Model
{
    public class StandardErrorObject
    {
        [JsonProperty(PropertyName = "Error Message")]
        private string mErrorString;

        public void setError(string ErrorString)
        {
            this.mErrorString = ErrorString;
        }

        public string getError()
        {
            return this.mErrorString;
        }
    }
}
