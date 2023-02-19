using System;
using System.Globalization;
using System.Text.Json;
using Common.Logging.LogAnalytics;
using CsvHelper;
using Microsoft.Extensions.Configuration;
using SentinelDataGenerator.Model;

namespace SentinelDataGenerator
{
	public class Worker
	{
        private IConfiguration configuration;

        public Worker(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        internal void DoWork()
        {
            var count = 0;
            var amountUserLogs = int.Parse(configuration["AmountUserLogs"]);
            var amountAnomalyLogs = int.Parse(configuration["AmountAnomalyLogs"]);

            var signInActivities = new List<SignInActivity>();

            Console.WriteLine("=== || Generate User Logs || ====");
            while (count < amountUserLogs)
            {
                var signInActivity = new SignInActivity();
                signInActivity.IpAddress = Generators.GenerateIpAddress(false);
                signInActivity.Id = Guid.NewGuid();
                signInActivity.UserAgent = Generators.GenerateUserAgent(false);
                signInActivity.Username = Generators.GenerateUsername();
                signInActivity.EventDateTime = Generators.GenerateTime();
                signInActivity.IsAnomaly = false;
                signInActivities.Add(signInActivity);

                count++;
            }

            Console.WriteLine("=== || Generate Anomaly Logs || ====");
            count = 0; // reset count for anomaly creation
            while (count < amountAnomalyLogs)
            {
                var signInActivity = new SignInActivity();
                signInActivity.IpAddress = Generators.GenerateIpAddress(true);
                signInActivity.Id = Guid.NewGuid();
                signInActivity.UserAgent = Generators.GenerateUserAgent(true);
                signInActivity.Username = Generators.GenerateUsername();
                signInActivity.EventDateTime = Generators.GenerateTime();
                signInActivity.IsAnomaly = true;
                signInActivities.Add(signInActivity);

                Console.WriteLine("Anomaly Created");

                count++;
            }

            using (var writer = new StreamWriter("logs.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(signInActivities.OrderBy(a => a.EventDateTime).ToList());
            }

            string json = JsonSerializer.Serialize(signInActivities);
            File.WriteAllText("logs.json", json);

            string json2 = JsonSerializer.Serialize(signInActivities.FirstOrDefault());
            File.WriteAllText("single_log.json", json2);

            var laWriter = new LogAnalyticsWriter(configuration["WorkspaceId"], configuration["WorkspaceKey"], configuration["LogName"]);
            foreach (var signinActity in signInActivities)
            {
                Console.WriteLine("Sending activity to sentinel: " + signinActity.Id.ToString());
                laWriter.SendLog("Login to application", signinActity.IpAddress, signinActity.Username, signinActity.UserAgent, signinActity.EventDateTime.ToString());
            }
        }
    }
}

