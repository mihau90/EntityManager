using System.IO;
using System.Text.RegularExpressions;
using EntityManager.Models;

namespace EntityManager.Readers
{
    public class ObjectsFactoryReader
    {
        private ObjectsFactoryModel objectFactory;
        private string fileContent;
        public string ObjectsFactoryPath { get; private set; }

        public ObjectsFactoryReader(string objectsFactoryPath)
        {
            this.ObjectsFactoryPath = objectsFactoryPath;
        }

        public ObjectsFactoryModel Read()
        {
            using (StreamReader reader = new StreamReader(ObjectsFactoryPath))
            {
                fileContent = reader.ReadToEnd();
                reader.Close();
            }
            objectFactory = new ObjectsFactoryModel();

            ParseDescription();
            ParseFunctions();

            return objectFactory;
        }

        private void ParseDescription()
        {
            var regexPattern = @"''[\s\S]*?''";
            var regex = new Regex(regexPattern);
            var match = regex.Match(fileContent);

            objectFactory.Description = match.Value;
        }

        private void ParseFunctions()
        {
            var regexPattern = @"^Public Function (?<functionName>\w+)\(\) As AllInterfaces.FactorableInterface: *Set \1 = New (?<functionType>\w+\.\w+): *End Function";
            var regex = new Regex(regexPattern, RegexOptions.Multiline);
            var matches = regex.Matches(fileContent);

            foreach (Match match in matches)
            {
                objectFactory.Functions.Add(new FunctionModel(match.Groups["functionType"].Value));
            }
        }
    }
}