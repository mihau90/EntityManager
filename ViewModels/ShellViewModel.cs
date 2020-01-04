using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Caliburn.Micro;
using EntityManager.ConfigManagers;
using EntityManager.Models;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace EntityManager.ViewModels
{
    public class ShellViewModel : Screen
    {
        #region Properties
        private const string Version = "1.0.0";
        private readonly string myName = "EntityManager";
        private IConfigManager configManager;
        private string _repositoryPath = string.Empty;
        private string _tableName = string.Empty;
        private EntityModel _entity;
        private BindableCollection<PropertyModel> _properties;
        private Dictionary<string, string> _entitiyMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private string EntitiesDirectory => $"{ RepositoryPath }\\src\\AllEntities";
        private string SqlSchemaPath => $"{ RepositoryPath }\\assets\\db_schema.sql";
        private string EntityFullPath => $"{ EntitiesDirectory }\\{ Entity.LongName }.cls";
        private string XmlPath => $"{ RepositoryPath }\\entity_mapping.xml";
        private string ObjectsFactoryPath => $"{ EntitiesDirectory }\\ObjectsFactory.bas";
        public int GetMaxTypeLenght() => Properties.Max(x => x.PropertyType.Length);
        public int GetMaxNameLenght() => Properties.Max(x => x.PropertyName.Length);

        public string RepositoryPath
        {
            get { return _repositoryPath; }
            set
            {
                _repositoryPath = value;
                NotifyOfPropertyChange(() => RepositoryPath);
            }
        }
        public string TableName
        {
            get { return _tableName; }
            set
            {
                _tableName = value;
                NotifyOfPropertyChange(() => TableName);
            }
        }
        public EntityModel Entity
        {
            get { return _entity; }
            set
            {
                _entity = value;
                NotifyOfPropertyChange(() => Entity);
            }
        }

        public BindableCollection<PropertyModel> Properties
        {
            get { return _properties; }
            set
            {
                _properties = value;
                NotifyOfPropertyChange(() => Properties);
            }
        }
        #endregion

        public ShellViewModel()
        {
            string configJsonPath = $"{ Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\EntityManager.settings.json";
            configManager = new JsonConfigManager(configJsonPath);
        }

        public void Generate(string repositoryPath)
        {

        }

        public bool CanGenerate(string repositoryPath)
        {
            return true;
        }

        public void Load(string repositoryPath)
        {

        }

        public bool CanLoad(string repositoryPath)
        {
            return true;
        }

        public void AutoFillIn(string repositoryPath)
        {

        }

        public bool CanAutoFillIn(string repositoryPath)
        {
            return true;
        }

        public void PickDirectory()
        {
            var dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = RepositoryPath;
            dialog.IsFolderPicker = true;

            if (dialog.ShowDialog() != CommonFileDialogResult.Ok)
            {
                return;
            }

            RepositoryPath = dialog.FileName;

            if (Directory.Exists(RepositoryPath)
                && Directory.Exists(EntitiesDirectory))
            {
                UpdateConfig();
                return;
            }

            RepositoryPath = string.Empty;
        }

        public void Clear()
        {

        }

        private void UpdateConfig()
        {
            configManager.Config.RepositoryPath = RepositoryPath;
            configManager.Save();
        }
    }
}
