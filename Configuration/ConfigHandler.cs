using TestClient.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TestClient;
using UnityEngine;
using UniverseLib;

namespace TestClient.Configuration;
internal static class ConfigHandler {
    private static bool _initialized = false;

    private static readonly string _configFileName = "config.era";
    private static string _configFilePath;
    private static Dictionary<string, string> _loadedConfigs = [];

    public static readonly Dictionary<string, object> Configs = [];

    private static void SetupIO() {
        try {
            string assemblyPath = Assembly.GetExecutingAssembly().Location;
            string assemblyDirectory = Path.GetDirectoryName(assemblyPath);
            string pluginFolderPath = Path.Combine(assemblyDirectory, MyPluginInfo.PLUGIN_GUID);
            if (!Directory.Exists(pluginFolderPath))
                Directory.CreateDirectory(pluginFolderPath);

            _configFilePath = Path.Combine(pluginFolderPath, _configFileName);
            if (!File.Exists(_configFilePath)) {
                string serializedData = ConfigSerializer.Serialize(_loadedConfigs);
                File.WriteAllText(_configFilePath, serializedData);
            } else {
                _loadedConfigs = ConfigSerializer.Deserialize(File.ReadAllText(_configFilePath));
            }
        } catch (Exception ex) {
            Plugin.Log.LogError($"Error during IO setup: {ex.Message}");
            throw;
        }
        _initialized = true;
    }

    public static void Init() {
        SetupIO();
        Plugin.Log.LogMessage("ConfigHandler setup complete.");
    }

    private static void CheckInitialization() {
        if (!_initialized)
            throw new Exception("ConfigHandler is not initialized!");
    }

    public static bool LoadConfig<T>(Config<T> config) {
        CheckInitialization();
        if (_loadedConfigs.ContainsKey(config.Name)) {
            config.SetValue(StringToConfigValue<T>(_loadedConfigs[config.Name]));
            return true;
        }
        return false;
        //else
        //    Save();
    }

    public static T StringToConfigValue<T>(string value) {
        return (T)StringToConfigValue(value, typeof(T));
    }
    public static object StringToConfigValue(string value, Type elementType) {
        if (elementType == typeof(KeyCode))
            return (KeyCode)Enum.Parse(typeof(KeyCode), value);
        else if (elementType == typeof(bool))
            return bool.Parse(value);
        else if (elementType == typeof(int))
            return int.Parse(value);
        else if (elementType == typeof(float))
            return float.Parse(value);
        else if (elementType.IsEnum)
            return Enum.Parse(elementType, value);
        return value;
    }

    public static void Save() {
        CheckInitialization();
        File.WriteAllText(_configFilePath, ConfigsToString());
    }

    private static string ConfigsToString() {
        Dictionary<string, string> configs = [];
        foreach (var config in Configs.Values) {
            var actualType = config.GetActualType();
            configs.Add((string)actualType.GetProperty("Name").GetValue(config), actualType.GetProperty("Value").GetValue(config).ToString());
        }
        return ConfigSerializer.Serialize(configs);
    }
}

public class Config<T> {
    public T Value { get; private set; }
    public string Name { get; private set; }
    public bool Cached = false;

    public Config(string name, T defaultValue, bool saveable) {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Configuration name cannot be null or empty.", nameof(name));

        if (ConfigHandler.Configs.ContainsKey(name))
            throw new ArgumentException($"A configuration with the name '{name}' is already registered.", nameof(name));

        Name = name;
        Value = defaultValue;

        if (saveable) {
            ConfigHandler.Configs.Add(name, this);
            Cached = ConfigHandler.LoadConfig(this);
        }
    }

    public void SetValue(T newValue) {
        Value = newValue;
        ConfigHandler.Save();
    }
}