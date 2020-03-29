using App.Services.Core;
using System;
using App.Services.Core.Interfaces;

namespace App.Helper
{
    public class ConfigHelper
    {
        public const int DEFAULT_CONFIG_INT = -9999;
        public const double DEFAULT_CONFIG_DOUBLE = -9999;
        public const float DEFAULT_CONFIG_FLOAT = -9999;
        public const bool DEFAULT_CONFIG_BOOL = false;
        public const string NULL_VALUE = "NULL";
        private readonly IWebSettingService _service;

        public ConfigHelper(IWebSettingService service)
        {
            _service = service;
        }

        public string GetConfig(string name)
        {
            var setting = _service.GetByName(name);
            if (setting == null)
                return null;

            return setting.Value;
        }

        public int GetConfigAsInt(string name)
        {
            int result;
            var config = GetConfig(name);

            if (int.TryParse(config, out result))
                return result;

            return DEFAULT_CONFIG_INT;
        }

        public double GetConfigAsDouble(string name)
        {
            double result;
            var config = GetConfig(name);

            if (double.TryParse(config, out result))
                return result;

            return DEFAULT_CONFIG_DOUBLE;
        }

        public float GetConfigAsFloat(string name)
        {
            float result;
            var config = GetConfig(name);

            if (float.TryParse(config, out result))
                return result;

            return DEFAULT_CONFIG_FLOAT;
        }

        public bool GetConfigAsBool(string name)
        {
            bool result;
            var config = GetConfig(name);

            if (bool.TryParse(config, out result))
                return result;

            return DEFAULT_CONFIG_BOOL;
        }
    }
}
