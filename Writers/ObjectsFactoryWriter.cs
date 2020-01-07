using System;
using System.IO;
using System.Linq;
using EntityManager.Models;

namespace EntityManager.Writers
{
    public class ObjectsFactoryWriter
    {
        public string ObjectsFactoryPath { get; private set; }

        public ObjectsFactoryWriter(string objectsFactoryPath)
        {
            this.ObjectsFactoryPath = objectsFactoryPath;
        }

        public void Write(ObjectsFactoryModel objectFactory)
        {
            var maxFunctionNameLenght = objectFactory.GetMaxFunctionNameLenght();
            var maxFunctionDeclarationLenght = objectFactory.GetMaxFunctionDeclarationLenght();
            var sortedList = objectFactory.Functions.OrderBy(x => x.Name).ToList();
            var unduplicatedList = sortedList.GroupBy(x => x.Name).Select(x => x.First()).ToList();
            var vbaFunctionsList = sortedList.Select(f => f.ToVba(maxFunctionNameLenght, maxFunctionDeclarationLenght)).ToList();
            
            var codeBlock = new string[]
            {
                $"Attribute VB_Name = \"{ objectFactory.VbName }\"",
                objectFactory.Description,
                string.Empty,
                "Option Explicit",
                string.Empty,
                string.Join(Environment.NewLine, vbaFunctionsList)
            };

            File.WriteAllText(ObjectsFactoryPath, string.Join(Environment.NewLine, codeBlock));
        }
    }
}