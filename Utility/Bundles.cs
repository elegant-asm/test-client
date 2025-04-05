using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UniverseLib;

namespace TestClient.Utility;
internal static class Bundles {
    public static AssetBundle LoadBundle(string id) {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Resource ID cannot be null or whitespace.", nameof(id));

        using Stream stream = typeof(Plugin).Assembly.GetManifestResourceStream($"TestClient.Resources.{id}.bundle");

        if (stream != null) {
            byte[] bytes;
            using (MemoryStream ms = new()) {
                stream.CopyTo(ms);
                bytes = ms.ToArray();
            }

            AssetBundle bundle = AssetBundle.LoadFromMemory(bytes);
            return bundle ?? throw new InvalidOperationException($"Failed to load AssetBundle from resource '{id}'.");
        }

        throw new ArgumentException($"Resource '{id}' not found in assembly");
    }

    public static AssetBundle LoadBundleFromFile(string id) {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Resource ID cannot be null or whitespace.", nameof(id));

        Assembly assembly = Assembly.GetExecutingAssembly();

        string resourceName = $"TestClient.Resources.{id}.bundle";

        using (Stream stream = assembly.GetManifestResourceStream(resourceName)) {
            if (stream == null)
                throw new ArgumentException($"Resource '{resourceName}' not found in assembly.");

            string tempFilePath = Path.Combine(Path.GetTempPath(), $"{id}.bundle");

            try {
                using (FileStream fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write)) {
                    stream.CopyTo(fileStream);
                }

                AssetBundle bundle = AssetBundle.LoadFromFile(tempFilePath);
                if (bundle == null)
                    throw new InvalidOperationException($"Failed to load AssetBundle from resource '{id}'.");

                return bundle;
            } finally {
                try {
                    File.Delete(tempFilePath);
                } catch (Exception ex) {
                    Debug.LogWarning($"Failed to delete temporary file '{tempFilePath}': {ex.Message}");
                }
            }
        }
    }

    public static byte[] GetResource(string filename, string fileformat = "png") {
        Assembly assembly = Assembly.GetExecutingAssembly();
        string resourceName = $"TestClient.Resources.{filename}.{fileformat}";

        using (Stream stream = assembly.GetManifestResourceStream(resourceName)) {
            if (stream == null) {
                Plugin.Log.LogError($"Resource {resourceName} not found in project.");
                return null;
            }

            using (MemoryStream memoryStream = new MemoryStream()) {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
