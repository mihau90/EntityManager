using System;
using System.Linq;
using EntityManager.ViewModels;

namespace EntityManager.CodeGenerators
{
    public class SqlCodeGenerator : ICodeGenerator
    {
        public string Generate(ShellViewModel viewModel)
        {
            var columnsList = viewModel.Properties.Select(x => $"[{ x.ColumnName }] { x.ColumnDefinition },".Indent(1)).ToList();
            var lastIndex = columnsList.Count - 1;
            columnsList[lastIndex] = columnsList[lastIndex].TrimEnd(',');

            var joinedColumns = string.Join(Environment.NewLine, columnsList);

            var codeBlock = new string[]
            {
                $"CREATE TABLE { viewModel.TableName } (",
                string.Join(Environment.NewLine, joinedColumns),
                ");",
                string.Empty
            };
            
            return string.Join(Environment.NewLine, codeBlock);
        }
    }
}