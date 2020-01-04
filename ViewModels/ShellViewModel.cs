using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Caliburn.Micro;
using EntityManager.ConfigManagers;
using EntityManager.Models;
using EntityManager.Readers;
using EntityManager.Writers;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace EntityManager.ViewModels
{
    public class ShellViewModel : Screen
    {
        #region Private variables
        private const string AppVersion = "1.0.0";
        private const string AppName = "EntityManager";
        private Dictionary<string, string> entitiyMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        private IConfigManager configManager;
        #endregion

        #region Properties
        private string _repositoryPath = string.Empty;
        private string _tableName = string.Empty;
        private EntityModel _entity;
        private BindableCollection<PropertyModel> _properties;
        public string RepositoryPath
        {
            get { return _repositoryPath; }
            set
            {
                if (Directory.Exists(value))
                {
                    _repositoryPath = value;
                    NotifyOfPropertyChange(() => RepositoryPath);
                }
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

        #region Constructor
        public ShellViewModel()
        {
            InitializeConfiguration();
            GetEntityMapping();
            LoadInitialData();
        }
        #endregion

        #region Buttons
        public void Generate(string repositoryPath)
        {
            var sqlWriter = new SqlWriter(SqlSchemaPath);
            sqlWriter.Write(this);

            var vbaWriter = new VbaWriter(EntityFullPath);
            vbaWriter.Write(this);

            var xmlWriter = new XmlWriter(XmlPath);
            xmlWriter.Write(this);

            var objectsFactoryReader = new ObjectsFactoryReader(ObjectsFactoryPath);
            var objectsFactory = objectsFactoryReader.Read();
            objectsFactory.AddFunction(Entity.FullName);

            var objectsFactoryWriter = new ObjectsFactoryWriter(ObjectsFactoryPath);
            objectsFactoryWriter.Write(objectsFactory);
        }

        public bool CanGenerate(string repositoryPath)
        {
            return CanLoad(repositoryPath);
        }

        public void Load(string repositoryPath)
        {
            GetEntityMapping();
            MessageBox.Show("Entities loaded", MessageBoxCaption, MessageBoxButton.OK, MessageBoxImage.Information);
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
            if (MessageBoxResult.Yes == MessageBox.Show(
                "Are you sure?", $"{ AppName } { AppVersion }", MessageBoxButton.YesNo, MessageBoxImage.Question))
            {
                LoadInitialData();
            }
        }
        #endregion

        #region Public methods
        public int GetMaxTypeLenght() => Properties.Max(x => x.PropertyType.Length);
        public int GetMaxNameLenght() => Properties.Max(x => x.PropertyName.Length);
        #endregion

        #region Private methods
        private string EntitiesDirectory => $"{ RepositoryPath }\\src\\AllEntities";
        private string SqlSchemaPath => $"{ RepositoryPath }\\assets\\db_schema.sql";
        private string EntityFullPath => $"{ EntitiesDirectory }\\{ Entity.LongName }.cls";
        private string XmlPath => $"{ RepositoryPath }\\entity_mapping.xml";
        private string ObjectsFactoryPath => $"{ EntitiesDirectory }\\ObjectsFactory.bas";
        private string MessageBoxCaption => $"{ AppName } { AppVersion }";
        private void InitializeConfiguration()
        {
            string configJsonPath = $"{ Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\EntityManager.settings.json";
            configManager = new JsonConfigManager(configJsonPath);

            RepositoryPath = configManager.Config.RepositoryPath;
        }
        private void UpdateConfig()
        {
            configManager.Config.RepositoryPath = RepositoryPath;
            configManager.Save();
        }
        private void GetEntityMapping()
        {
            entitiyMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (!string.IsNullOrEmpty(RepositoryPath))
            {
                return;
            }

            var entityFiles = Directory.GetFiles(EntitiesDirectory, "*Entity.cls");

            foreach (var entityFile in entityFiles)
            {
                var entityName = Path.GetFileNameWithoutExtension(entityFile);
                var mappingValue = $"AllEntities.{ entityName }";

                entitiyMapping[entityName.DropSuffix("Entity")] = mappingValue;
                entitiyMapping[entityName] = mappingValue;
                entitiyMapping[$"AllEntities.{ entityName }"] = mappingValue;
            }
        }
        private void LoadInitialData()
        {
            TableName = string.Empty;
            Entity = new EntityModel
            {
                ShortName = string.Empty
            };
            Properties = new BindableCollection<PropertyModel>();
            Properties.Add(GetIdProperty());
        }
        private PropertyModel GetIdProperty()
        {
            return new PropertyModel()
            {
                PropertyName = "id",
                PropertyType = "String",
                ColumnName = "id",
                ColumnType = "CHAR",
                CharLimit = 38,
                IsGet = true,
                IsSet = false,
                IsOptional = true,
                PropertyDefaultValue = "AllHelpers.GUID.generate()",
                Description = "record identification number @see: AllHelpers.Guid.generate()"
            };
        }
        #endregion
    }
}