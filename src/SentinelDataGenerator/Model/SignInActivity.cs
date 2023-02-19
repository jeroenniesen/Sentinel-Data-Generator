using System;
namespace SentinelDataGenerator.Model
{
	public class SignInActivity
	{
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public string MfaUsed { get; set; }
        public DateTime EventDateTime { get; set; }
        public bool IsAnomaly { get; set; }
    }
}

