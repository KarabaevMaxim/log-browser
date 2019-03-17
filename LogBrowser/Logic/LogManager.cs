namespace LogBrowser.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.IO;

    public class LogManager
    {
        public LogManager()
        {
            this.Logs = new List<Log>();
        }

        public void ReadLogs()
        {
            var logStrings = File.ReadAllLines(Settings.SettingsInfo.LogFileName, Encoding.GetEncoding("windows-1251"));
            this.Logs.Clear();

            foreach (var item in logStrings)
            {
                var str = item.Split('|');
                this.Logs.Add(new Log
                {
                    DateTime = DateTime.Parse(str[0]),
                    LogType = (LogTypes)Enum.Parse(typeof(LogTypes), str[1]),
                    Source = str[2],
                    Message = str[3]
                });
            }

			this.ReverseLogs();

		}

		public void ReverseLogs()
		{
			this.Logs.Reverse();
		}

        public List<Log> Logs { get; set; }
        
    }
}
