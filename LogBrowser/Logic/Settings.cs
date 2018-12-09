namespace LogBrowser.Logic
{
    using System.IO;
    using Newtonsoft.Json;

    public static class Settings
    {
        public class SettingsStore
        {
            public string LogFileName { get; set; }
        }

        public static SettingsStore SettingsInfo { get; set; }
        public const string SettingsFileName = "Settings.cfg";
        
        public static void LoadSettings()
        {
            try
            {
                SettingsInfo = JsonConvert.DeserializeObject<SettingsStore>(File.ReadAllText(SettingsFileName));
            }
            catch(FileNotFoundException)
            {
                SaveSettings();
            }
            
        }

        private static void SaveSettings()
        {
            var json = JsonConvert.SerializeObject(new SettingsStore { LogFileName = "Default.log" });
            File.WriteAllText(SettingsFileName, json);
        }
    }
}
