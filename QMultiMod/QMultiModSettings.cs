using Oculus.Newtonsoft.Json;
using System;
using System.IO;

namespace QMultiMod
{
    class QMultiModSettings
    {
        public float ScannerSpeedNormalInterval = 14f;
        public float ScannerSpeedMinimumInterval = 1f;
        public float ScannerSpeedIntervalPerModule = 3f;
        public float ScannerBlipRange = 1000f;
        public float ScannerMinRange = 600f;
        public float ScannerUpgradeAddedRange = 100f;
        public float ScannerCameraRange = 1000f;
        public float FireExtinguisherHolderRechargeValue = 0.005f;


        private static readonly string configPath = Environment.CurrentDirectory + @"\QMods\QMultiMod\config.json";
        private static readonly QMultiModSettings instance = new QMultiModSettings();


        static QMultiModSettings()
        {
        }


        private QMultiModSettings()
        {
        }


        public static QMultiModSettings Instance
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
            var userSettings = JsonConvert.DeserializeObject<QMultiModSettings>(json);

            var fields = typeof(QMultiModSettings).GetFields();

            foreach (var field in fields)
            {
                var userValue = field.GetValue(userSettings);
                field.SetValue(Instance, userValue);
            }
        }
    }
}
