using EntityManager.ViewModels;

namespace EntityManager.CodeGenerators
{
    public interface ICodeGenerator
    {
        string Generate(ShellViewModel viewModel);
    }
}
