﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using ARDrone.Input.InputConfigs;
using ARDrone.Input.InputControls;
using ARDrone.Input.InputMappings;
using ARDrone.Input.Utility;

namespace ARDrone.Input
{
    public abstract class ConfigurableInput : GenericInput
    {
        protected InputMapping mapping = null;
        protected InputMapping backupMapping = null;

        protected InputConfig inputConfig = null;

        public ConfigurableInput()
            : base()
        {
            inputConfig = InputFactory.CreateConfigFor(this);
        }

        public void DetermineMapping()
        {
            mapping = GetStandardMapping();
            LoadMapping();

            backupMapping = mapping.Clone();
        }

        public void SetDefaultMapping()
        {
            mapping = GetStandardMapping();
            backupMapping = mapping.Clone();
        }

        public void CopyMappingFrom(ConfigurableInput input)
        {
            mapping = input.mapping.Clone();
            backupMapping = input.backupMapping.Clone();
        }

        protected abstract InputMapping GetStandardMapping();

        public bool LoadMapping()
        {
            try
            {
                if (mapping == null)
                    return false;

                String mappingFilePath = GetMappingFilePath();
                if (!File.Exists(mappingFilePath))
                {
                    return false;
                }

                Dictionary<String, String> mappingDictionary = DictionarySerializer.Deserialize(mappingFilePath);

                mapping.CopyMappingsFrom(mappingDictionary);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void SaveMapping()
        {
            backupMapping = mapping.Clone();

            try
            {
                if (mapping == null)
                {
                    return;
                }

                String mappingFilePath = GetMappingFilePath();

                DictionarySerializer.Serialize(mapping.Controls.Mappings, mappingFilePath);
            }
            catch (Exception e)
            {
                throw new Exception("There was an error while writing the input mapping for device \"" + DeviceName + "\": " + e.Message);
            }
        }

        private String GetMappingFilePath()
        {
            String appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            String appFolder = Path.Combine(appDataFolder, "ARDrone.NET", "mappings");

            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
            }

            String mappingPath = Path.Combine(appFolder, FilePrefix + ".xml");
            return mappingPath;
        }

        public void RevertMapping()
        {
            mapping = backupMapping.Clone();
        }

        public InputMapping Mapping
        {
            get
            {
                return mapping;
            }
        }

        public virtual String FilePrefix
        {
            get { return string.Empty; }
        }

        public InputConfig InputConfig
        {
            get
            {
                return inputConfig;
            }
        }
    }
}
