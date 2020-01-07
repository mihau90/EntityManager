using EntityManager.Models;
using Newtonsoft.Json;
using System.IO;

namespace EntityManager.ConfigManagers
{
    public class JsonConfigManager : IConfigManager
    {
        public ConfigModel Config { get; private set; }
        public string ConfigFilePath { get; private set; }

        public JsonConfigManager(string configFilePath)
        {
            this.ConfigFilePath = configFilePath;
            Config = Load();
        }

        public IConfigManager Save()
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
               return new ConfigModel();

            return model;
        }
    }
}