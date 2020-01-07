using EntityManager.ViewModels;

namespace EntityManager.Writers
{
    public interface IWriter
    {
        void Write(ShellViewModel model);
    }
}