using System.IO;
using EntityManager.CodeGenerators;
using EntityManager.ViewModels;

namespace EntityManager.Writers
{
    public class VbaWriter : IWriter
    {
        public string VbaFilePath { get; set; }

        public VbaWriter(string vbaFilePath)
        {
            this.VbaFilePath = vbaFilePath;
        }

        public void Write(ShellViewModel model)
        {
            var vbaCodeGenerator = new VBACodeGenerator();
            var vbaCode = vbaCodeGenerator.Generate(model);

            File.WriteAllText(VbaFilePath, vbaCode);
        }
    }
}