using EntityManager.Models;
using EntityManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EntityManager.CodeGenerators
{
    public class VBACodeGenerator : ICodeGenerator
    {
        // Constants
        private const string CommentBlock = "\'\'";
        private const string InheritDoc = "\' @inheritdoc";
        private const string EndFunction = "End Function";

        // Private variables
        private List<string> propertiesDescription;
        private List<string> factorableDescription;
        private List<string> factorableVariables;
        private List<string> factorableAllAddInVariables;
        private List<string> propertyGetsAndSets;
        private string defaultValues;
        private int maxNameLen;
        private int maxTypeLen;

        // Properties
        private string InheritDocBlock => string.Join(Environment.NewLine, new string[] { CommentBlock, InheritDoc, CommentBlock });
        public ShellViewModel ViewModel { get; private set; }

        // Public methods
        public string Generate(ShellViewModel viewModel)
        {
            this.ViewModel = viewModel;
            maxNameLen = viewModel.GetMaxNameLenght();
            maxTypeLen = viewModel.GetMaxTypeLenght();

            GenerateVariableCodeBlock();

            var codeBlock = new List<string>()
            {
                GetAttributesCodeBlock(),
                GetInterfaceImplementationCodeBlock(),
                GetPropertiesCodeBlock(),
                GetEntityHelperCodeBlock(),
                GetClassInitializeCodeBlock(),
                GetFactorableInitializeCodeBlock(),
                GetEntityInterfaceCodeBlock(),
                GetPropertyGetsAndSets()
            };

            return string.Join(Environment.NewLine + Environment.NewLine, codeBlock);
        }

        //Private methods
        private void GenerateVariableCodeBlock()
        {
            var defaultValuesList = ViewModel.Properties.Select(x => $".Add \"{ x.PropertyName }\", { x.PropertyDefaultValue }".Indent(2).TrimEnd()).ToList();
            defaultValues = string.Join(Environment.NewLine, defaultValuesList);

            propertiesDescription = new List<string>();
            factorableDescription = new List<string>();
            factorableVariables = new List<string>();
            factorableAllAddInVariables = new List<string>();
            propertyGetsAndSets = new List<string>();
 
            foreach (var property in ViewModel.Properties)
            {
                propertiesDescription.Add(GeneratePropertiesDescription(property));
                propertyGetsAndSets.Add(GeneratePropertyGetsAndSets(property));

                if (property.IsCollection)
                {
                    continue;
                }

                factorableDescription.Add(GenerateFactorableDescription(property));

                if (property.IsAllAddInType)
                {
                    factorableAllAddInVariables.Add(GenerateFactorableAllAddInVariable(property));
                }
                else
                {
                    factorableVariables.Add(GenerateFactorableVariable(property));
                }

            }
        }
        private string GeneratePropertiesDescription(PropertyModel property)
        {
            var paddedType = property.PropertyType.PadRight(maxTypeLen);
            var paddedName = property.PropertyName.PadRight(maxNameLen);
            var description = property.Description;

            return $"\'   @var { paddedType } { paddedName } { description }".TrimEnd();
        }
        private string GenerateFactorableDescription(PropertyModel property)
        {
            var opt = (property.IsOptional) ? "[opt] " : string.Empty;
            var paddedType = property.PropertyType.PadRight(maxTypeLen, ' ');
            var paddedName = property.PropertyName.PadRight(maxNameLen, ' ');
            var description = $"{ opt }@see: Self.properties.{ property.PropertyName }";

            return $"\'   @var { paddedType } { paddedName } { description }".TrimEnd();
        }
        private string GeneratePropertyGetsAndSets(PropertyModel property)
        {
            var returnType = (property.IsCollection) ? "VBA.Collection" : property.PropertyType;
            var set = string.Empty;
            var letName = "Let";

            if (property.IsAllAddInType || property.IsCollection)
            {
                set = "Set ";
                letName = "Set";
            }

            var codeBlock = new List<string>();

            if (property.IsGet)
            {
                codeBlock.Add(CommentBlock);
                codeBlock.Add($"\' @return {property.PropertyType} @see: Self.properties.{ property.PropertyName }");
                codeBlock.Add(CommentBlock);
                codeBlock.Add($"Public Property Get { property.PropertyName }() As { returnType }");
                codeBlock.Add(string.Empty);
                codeBlock.Add($"{ set }{ property.PropertyName } = properties(\"{property.PropertyName}\")".Indent(1));
                codeBlock.Add(string.Empty);
                codeBlock.Add("End Property");
                codeBlock.Add(string.Empty);
            }

            if (property.IsSet)
            {
                codeBlock.Add(CommentBlock);
                codeBlock.Add($"\' @param { property.PropertyType } value @see: Self.properties.{ property.PropertyName }");
                codeBlock.Add(CommentBlock);
                codeBlock.Add($"Public Property { letName } { property.PropertyName } () As { returnType }");
                codeBlock.Add(string.Empty);
                codeBlock.Add($"{ set }properties(\"{ property.PropertyName }\") = { property.PropertyName }".Indent(1));
                codeBlock.Add(string.Empty);
                codeBlock.Add("End Property");
                codeBlock.Add(string.Empty);
            }

            return string.Join(Environment.NewLine, codeBlock);
        }
        private string GenerateFactorableVariable(PropertyModel property)
        {
            var opt = (property.IsOptional) ? $"If (value.Exists(\"{ property.PropertyName }\")) Then " : string.Empty;
            var set = (property.IsAllAddInType || property.IsCollection) ? "Set " : string.Empty;

            return $"{opt}{set}.Item(\"{ property.PropertyName }\") = value(\"{ property.PropertyName }\")".Indent(2);
        }
        private string GenerateFactorableAllAddInVariable(PropertyModel property)
        {
            var name = property.PropertyName;
            var type = property.PropertyType;

            return $"Set .Item(\"{ name }\") = eh.buildEntity(\"{ type }\", \"{ name }\", value, .Item(\"{ name }\"))".Indent(2);
        }
        private string GetAttributesCodeBlock()
        {
            var codeBlock = new List<string>()
            {
                "VERSION 1.0 CLASS",
                "BEGIN",
                "  MultiUse = -1  'True",
                "END",
                $"Attribute VB_Name = \"{ ViewModel.Entity.LongName }\"",
                "Attribute VB_GlobalNameSpace = False",
                "Attribute VB_Creatable = False",
                "Attribute VB_PredeclaredId = False",
                "Attribute VB_Exposed = True",
                "Option Explicit"
            };

            return string.Join(Environment.NewLine, codeBlock);
        }
        private string GetInterfaceImplementationCodeBlock()
        {
            var codeBlock = new string[]
            {
            "Implements AllInterfaces.FactorableInterface",
            "Implements AllInterfaces.EntityInterface"
            };

            return string.Join(Environment.NewLine, codeBlock);
        }
        private string GetPropertiesCodeBlock()
        {
            var codeBlock = new string[]
            {
                CommentBlock,
                "\' @var Scripting.Dictionary properties [",
                string.Join(Environment.NewLine, propertiesDescription),
                "\' ]",
                CommentBlock,
                "Private properties As Scripting.Dictionary"
            };

            return string.Join(Environment.NewLine, codeBlock);
        }
        private string GetEntityHelperCodeBlock()
        {
            var codeBlock = new string[]
            {
                "\'\'",
                "\' @var AllEntities.EntityHelper eh",
                "\'\'",
                "Private eh As AllEntities.EntityHelper"
            };

            return string.Join(Environment.NewLine, codeBlock);
        }
        private string GetClassInitializeCodeBlock()
        {
            var codeBlock = new string[]
            {
                CommentBlock,
                InheritDoc,
                CommentBlock,
                "Private Sub Class_Initialize()",
                string.Empty,
                "Set properties = New Scripting.Dictionary".Indent(1),
                "Set eh = AllHelpers.Factory.create(\"AllEntities.EntityHelper\")".Indent(1),
                string.Empty,
                "With properties".Indent(1),
                defaultValues,
                "End With".Indent(1),
                string.Empty,
                "End Sub"
            };

            return string.Join(Environment.NewLine, codeBlock);
        }
        private string GetFactorableInitializeCodeBlock()
        {
            var codeBlock = new List<string>()
            {
                CommentBlock,
                InheritDoc,
                "\'",
                "\' @param Scripting.Dictionary value [",
                string.Join(Environment.NewLine, factorableDescription),
                "\' ]",
                CommentBlock,
                "Private Function FactorableInterface_initialize(ByVal value As Scripting.Dictionary) As AllInterfaces.FactorableInterface",
                string.Empty,
                "Set FactorableInterface_initialize = Me".Indent(1),
                string.Empty,
                "With properties".Indent(1),
                string.Join(Environment.NewLine, factorableVariables),
            };

            if (factorableAllAddInVariables.Count > 0)
            {
                codeBlock.Add(string.Empty);
                codeBlock.Add(string.Join(Environment.NewLine, factorableAllAddInVariables));
            }
            codeBlock.Add("End With".Indent(1));
            codeBlock.Add(string.Empty);
            codeBlock.Add(EndFunction);

            return string.Join(Environment.NewLine, codeBlock);
        }
        private string GetEntityInterfaceCodeBlock()
        {
            var codeBlock = new List<string>()
            {
                InheritDocBlock,
                "Private Function EntityInterface_getProperty(ByVal propertyName As String) As Variant",
                string.Empty,
                "eh.getProperty Me, EntityInterface_getProperty, properties, propertyName".Indent(1),
                string.Empty,
                EndFunction,
                string.Empty,
                InheritDocBlock,
                "Private Function EntityInterface_setProperty(ByVal propertyName As String, ByVal value As Variant) As AllInterfaces.EntityInterface",
                string.Empty,
                "eh.setProperty Me, EntityInterface_setProperty, properties, propertyName, value".Indent(1),
                string.Empty,
                EndFunction,
                string.Empty,
                InheritDocBlock,
                "Private Function EntityInterface_getAllPropertyNames() As Variant",
                string.Empty,
                "EntityInterface_getAllPropertyNames = properties.Keys".Indent(1),
                string.Empty,
                EndFunction
            };

            return string.Join(Environment.NewLine, codeBlock);
        }
        private string GetPropertyGetsAndSets()
        {
            return string.Join(Environment.NewLine, propertyGetsAndSets);
        }
    }
}
