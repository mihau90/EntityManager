using EntityManager.Models;

namespace EntityManager.ConfigManagers
{
    public interface IConfigManager
    {
        public ConfigModel Config { get; }
        public IConfigManager Save();
    }
}