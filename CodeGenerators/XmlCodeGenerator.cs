using EntityManager.Models;
using EntityManager.ViewModels;
using System;
using System.Collections.Generic;

namespace EntityManager.CodeGenerators
{
    public class XMLCodeGenerator : ICodeGenerator
    {
        private List<string> properties;
        private List<string> indexes;
        private List<string> relations;

        public ShellViewModel ViewModel { get; private set; }

        public string Generate(ShellViewModel viewModel)
        {
            this.ViewModel = viewModel;
            GenerateVariableCodeBlock();

            var entityName = viewModel.Entity.FullName;

            var codeBlock = new List<string>
            {
                $"<entity name=\"{ entityName }\" table=\"{ viewModel.TableName }\" repository=\"{ viewModel.Entity.Repository }\" >".Indent(1),
                "<id name=\"id\" type=\"String\" column=\"id\" generator=\"AllHelpers.Guid.generate\" />".Indent(2)
            };

            //Properties
            if (properties.Count > 0)
            {
                codeBlock.Add("<properties>".Indent(2));
                codeBlock.Add(string.Join(Environment.NewLine, properties));
                codeBlock.Add("</properties>".Indent(2));
            }

            ////Relations
            if (relations.Count > 0)
            {
                codeBlock.Add("<relations>".Indent(2));
                codeBlock.Add(string.Join(Environment.NewLine, relations));
                codeBlock.Add("</relations>".Indent(2));
            }

            codeBlock.Add("</entity>".Indent(1));
            return string.Join(Environment.NewLine, codeBlock);
        }

        private void GenerateVariableCodeBlock()
        {
            properties = new List<string>();
            indexes = new List<string>();
            relations = new List<string>();

            foreach (var property in ViewModel.Properties)
            {
                switch (property.Relation)
                {
                    case Enums.RelationType.None:
                        properties.Add(GenerateProperty(property));
                        break;

                    //case Enums.RelationType.OneToOne:
                    //    break;

                    case Enums.RelationType.OneToMany:
                        relations.Add(GenerateRelation(property));
                        break;

                    case Enums.RelationType.ManyToOne:
                        properties.Add(GenerateProperty(property));
                        relations.Add(GenerateRelation(property));
                        break;

                    //case Enums.RelationType.ManyToMany:
                    //    break;

                    default:
                        break;
                }
            }
        }
        private string GenerateProperty(PropertyModel property)
        {
            var propertyType = (property.IsAllAddInType) ? "String" : property.PropertyType;

            return
                ($"<property " +
                $"name=\"{ property.PropertyName }\" " +
                $"column=\"{ property.ColumnName }\" " +
                $"type=\"{ propertyType }\" " +
                $"column-definition=\"{ property.ColumnDefinition }\" />").Indent(3);
        }
        private string GenerateRelation(PropertyModel property)
        {
            string targetEntity;
            string relationType;
            string propertyName;
            string sourceColumn;
            string targetColumn;

            switch (property.Relation)
            {
                //case Enums.RelationType.None:
                //    return "";
                //case Enums.RelationType.OneToOne:
                //    return "";

                case Enums.RelationType.OneToMany:
                    relationType = property.Relation.ToString();
                    propertyName = property.PropertyName;
                    targetEntity = property.PropertyType.DropSuffix("[]");
                    targetColumn = property.ColumnName;
                    sourceColumn = "id";
                    break;

                case Enums.RelationType.ManyToOne:
                    relationType = property.Relation.ToString();
                    propertyName = property.PropertyName;
                    targetEntity = property.PropertyType;
                    targetColumn = "id";
                    sourceColumn = property.ColumnName;
                    break;

                //case Enums.RelationType.ManyToMany:
                //    break;

                default:
                    return "";
            }

            return
                ($"<relation type=\"{ relationType }\" " +
                $"property=\"{ propertyName }\" " +
                $"target-entity=\"{ targetEntity }\" " +
                $"target-column=\"{ targetColumn }\" " +
                $"source-column=\"{ sourceColumn }\" />").Indent(3);
        }
    }
}
