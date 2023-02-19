using System;
namespace SentinelDataGenerator
{
	public static class Generators
	{
		public static string GenerateIpAddress(bool anomaly)
        {
			if (anomaly)
			{
				return "52.10.0.5";
			}
			else
            {
				var random = new Random();
				var list = new List<string> { "40.80.122.5", "40.92.16.16", "123.6.12.162", "188.144.22.8" };
				int index = random.Next(list.Count);

				return list[index];
            }
        }

		public static string GenerateUserAgent(bool anomaly)
        {
			if (anomaly)
			{
				return "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.131 Safari/537.36";
			}
			else
			{
				var random = new Random();
				var list = new List<string> { "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.131 Safari/537.36",
											  "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4515.107 Safari/537.36",
											  "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:90.0) Gecko/20100101 Firefox/90.0",
											  "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.164 Safari/537.36" };
				int index = random.Next(list.Count);

				return list[index];
			}
		}

		public static DateTime GenerateTime() {
			var startDate = new DateTime(2019, 01, 01, 01, 01, 01);
			var endDate = new DateTime(2022, 06, 25, 23, 59, 59);

			var random = new Random();

			TimeSpan timeSpan = endDate - startDate;
			TimeSpan newSpan = new TimeSpan(0, random.Next(0, (int)timeSpan.TotalMinutes), 0);
			DateTime newDate = startDate + newSpan;

			return newDate;
		}

		public static string GenerateUsername()
		{
			var random = new Random();
			var list = new List<string> { "j.jansen", "k.vaak", "f.devries", "j.degroot" };
			int index = random.Next(list.Count);

			return list[index];
		}
	}
}

