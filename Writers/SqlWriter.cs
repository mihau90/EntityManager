using System;
using System.IO;
using System.Text.RegularExpressions;
using EntityManager.CodeGenerators;
using EntityManager.ViewModels;

namespace EntityManager.Writers
{
    public class SqlWriter : IWriter
    {
        public string SqlFilePath { get; private set; }

        public SqlWriter(string sqlFilePath)
        {
            this.SqlFilePath = sqlFilePath;
        }

        public void Write(ShellViewModel model)
        {
            string fileContents;
            using (StreamReader reader = new StreamReader(SqlFilePath))
            {
                fileContents = reader.ReadToEnd();
                reader.Close();
            }

            var sqlCodeGenerator = new SqlCodeGenerator();
            var sqlCode = sqlCodeGenerator.Generate(model);

            var tableDefinitionPattern = $@"CREATE TABLE { model.TableName } *\([\s\S]*?\);";
            var regex = new Regex(tableDefinitionPattern);
            var tableExists = regex.IsMatch(fileContents);

            if (tableExists)
            {
                fileContents = regex.Replace(fileContents, sqlCode);
            }
            else
            {
                fileContents += sqlCode;
            }

            // remove blank lines
            fileContents = Regex.Replace(fileContents, @"^\s*$[\r\n]*", string.Empty, RegexOptions.Multiline);
            fileContents = Regex.Replace(fileContents, @"\);.*", $");{ Environment.NewLine }", RegexOptions.Multiline);

            File.WriteAllText(SqlFilePath, fileContents);
        }
    }
}