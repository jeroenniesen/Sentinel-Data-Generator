using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Common.Logging.LogAnalytics
{
    public class LogAnalyticsWriter
    {
        private string _WorkspaceId { get; set; }
        private string _Key { get; set; }
        private string _LogName { get; set; }
        private string _TimeStapField { get; set; }

        public LogAnalyticsWriter(string workspaceId, string key, string logName)
        {
            if (workspaceId == "")
            {
                throw new Exception("No log analytics workspace defined");
            }

            if (key == "")
            {
                throw new Exception("No log analytics key defined");
            }

            if (logName == null)
            {
                _LogName = "Generator Log";
            }
            else
            {
                _LogName = logName;
            }

            _TimeStapField = "";
            _WorkspaceId = workspaceId;
            _Key = key;
            _LogName = logName;
        }

        public void SendLog(string operation, string ipAddress, string UserName, string UserAgent, string EventDateTime)
        {
            // create record
            string json = string.Format("[{{Operation:\"{0}\",IpAddress:\"{1}\",Username:\"{2}\",UserAgent:\"{3}\",EventDateTime:\"{4}\"}}]",
                                        operation,
                                        ipAddress,
                                        UserName,
                                        UserAgent,
                                        EventDateTime);

            // Create a hash for the API signature
            var datestring = DateTime.UtcNow.ToString("r");
            var jsonBytes = Encoding.UTF8.GetBytes(json);
            string stringToHash = "POST\n" + jsonBytes.Length + "\napplication/json\n" + "x-ms-date:" + datestring + "\n/api/logs";
            string hashedString = BuildSignature(stringToHash, _Key);
            string signature = "SharedKey " + _WorkspaceId + ":" + hashedString;

            PostData(signature, datestring, json);
        }

        // Build the API signature
        private string BuildSignature(string message, string secret)
        {
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = Convert.FromBase64String(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hash = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hash);
            }
        }

        // Send a request to the POST API endpoint
        private void PostData(string signature, string date, string json)
        {
            try
            {
                string url = "https://" + _WorkspaceId + ".ods.opinsights.azure.com/api/logs?api-version=2016-04-01";

                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("Log-Type", _LogName);
                client.DefaultRequestHeaders.Add("Authorization", signature);
                client.DefaultRequestHeaders.Add("x-ms-date", date);
                client.DefaultRequestHeaders.Add("time-generated-field", _TimeStapField);

                System.Net.Http.HttpContent httpContent = new StringContent(json, Encoding.UTF8);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                Task<System.Net.Http.HttpResponseMessage> response = client.PostAsync(new Uri(url), httpContent);

                System.Net.Http.HttpContent responseContent = response.Result.Content;
                string result = responseContent.ReadAsStringAsync().Result;
                Console.WriteLine("Return Result: " + result);
            }
            catch (Exception excep)
            {
                Console.WriteLine("API Post Exception: " + excep.Message);
            }
        }
    }
}