using Oculus.Newtonsoft.Json;
using System;
using System.IO;

namespace BetterScannerRoom
{
    class BSRSettings
    {
        public float ScannerSpeedNormalInterval = 14f;
        public float ScannerSpeedMinimumInterval = 1f;
        public float ScannerSpeedIntervalPerModule = 3f;
        public float ScannerBlipRange = 1000f;
        public float ScannerMinRange = 600f;
        public float ScannerUpgradeAddedRange = 100f;
        public float ScannerCameraRange = 1000f;


        private static readonly string configPath = Environment.CurrentDirectory + @"\QMods\BetterScannerRoom\config.json";
        private static readonly BSRSettings instance = new BSRSettings();


        static BSRSettings()
        {
        }


        private BSRSettings()
        {
        }


        public static BSRSettings Instance
        {
            get
            {
                return instance;
            }
        }


        public static void Load()
        {
            if (!File.Exists(configPath))
            {
                File.WriteAllText(configPath, JsonConvert.SerializeObject(Instance, Formatting.Indented));
                return;
            }

            var json = File.ReadAllText(configPath);
            var userSettings = JsonConvert.DeserializeObject<BSRSettings>(json);

            var fields = typeof(BSRSettings).GetFields();

            foreach (var field in fields)
            {
                var userValue = field.GetValue(userSettings);
                field.SetValue(Instance, userValue);
            }
        }
    }
}
