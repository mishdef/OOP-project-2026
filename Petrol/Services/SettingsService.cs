using gsst.Interfaces;
using gsst.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace gsst.Services
{
    public class SettingsService
    {
        public static string settingsFilePath = "settings.json";

        public static SettingsModel Settings { get; set; } = new SettingsModel();

        static SettingsService()
        {
            settingsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");

            if (!File.Exists(settingsFilePath))
            {
                Settings = new SettingsModel();

                string defaultJson = JsonConvert.SerializeObject(Settings, Formatting.Indented);
                File.WriteAllText(settingsFilePath, defaultJson);
            }
            else
            {
                string jsonContent = File.ReadAllText(settingsFilePath);
                Settings = JsonConvert.DeserializeObject<SettingsModel>(jsonContent) ?? new SettingsModel();
            }
        }

        public void SaveSettings(SettingsModel settings)
        {
            Settings = settings;
            string jsonContent = JsonConvert.SerializeObject(Settings, Formatting.Indented);
            File.WriteAllText(settingsFilePath, jsonContent);
        }
    }
}
