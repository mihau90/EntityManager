using EntityManager.Models;
using Newtonsoft.Json;
using System.IO;

namespace EntityManager
{
    public class ConfigManager
    {
        public ConfigModel Config { get; private set; }
        public string ConfigFilePath { get; private set; }

        public ConfigManager(string configFilePath)
        {
            this.ConfigFilePath = configFilePath;
            Config = Load();
        }
        
        public ConfigManager Save()
        {
            string output = JsonConvert.SerializeObject(Config, Formatting.Indented);
            File.WriteAllText(ConfigFilePath, output);
            return this;
        }
        private ConfigModel Load()
        {
            if (!File.Exists(ConfigFilePath))
            {
                return new ConfigModel();
            }

            var fileContent = File.ReadAllText(ConfigFilePath);
            ConfigModel model = JsonConvert.DeserializeObject<ConfigModel>(fileContent);
            
            if (model == null)
                model = new ConfigModel();

            return model;
        }
    }
}
