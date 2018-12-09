namespace LogBrowser.Logic
{
    using System;

    public class Log
    {
        public DateTime DateTime { get; set; }
        public LogTypes LogType { get; set; }
        public string Source { get; set; }
        public string Message { get; set; }
    }
}
