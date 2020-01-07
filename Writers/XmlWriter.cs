using System;
using System.IO;
using System.Text.RegularExpressions;
using EntityManager.CodeGenerators;
using EntityManager.ViewModels;

namespace EntityManager.Writers
{
    public class XmlWriter : IWriter
    {
        public string XmlFilePath { get; private set; }

        public XmlWriter(string xmlFilePath)
        {
            this.XmlFilePath = xmlFilePath;
        }

        public void Write(ShellViewModel model)
        {
            string fileContents;
            using (StreamReader reader = new StreamReader(XmlFilePath))
            {
                fileContents = reader.ReadToEnd();
                reader.Close();
            }

            var xmlCodeGenerator = new XMLCodeGenerator();
            var xmlCode = xmlCodeGenerator.Generate(model);

            var entityDefinitionPattern = $@" *<entity name=.{ model.Entity.FullName }.[\s\S]*?</entity>";
            var regex = new Regex(entityDefinitionPattern);
            var entityExists = regex.IsMatch(fileContents);

            if (entityExists)
            {
                fileContents = regex.Replace(fileContents, xmlCode);
            }
            else
            {
                fileContents = fileContents.Replace("</entities>", $"{ xmlCode }{ Environment.NewLine }</entities>");
            }

            // remove blank lines
            fileContents = Regex.Replace(fileContents, @"^\s*$[\r\n]*", string.Empty, RegexOptions.Multiline);
            fileContents = Regex.Replace(fileContents, @" *</entity>", $"</entity>{ Environment.NewLine }".Indent(1), RegexOptions.Multiline);

            File.WriteAllText(XmlFilePath, fileContents);
        }
    }
}
