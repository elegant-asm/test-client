using System;
using System.Collections.Generic;
using System.Linq;

namespace TestClient.Utility;
internal static class ConfigSerializer {
    private const string Separator = ": ";
    public static string Serialize(Dictionary<string, string> data) {
        if (data == null || data.Count == 0)
            return string.Empty;

        int maxKeyLength = data.Keys.Max(k => k.Length);

        return string.Join(Environment.NewLine, data.Select(kvp => {
            string key = kvp.Key.PadRight(maxKeyLength);
            return $"{key}{Separator}\"{kvp.Value}\"";
        }));
    }

    public static Dictionary<string, string> Deserialize(string input) {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (string.IsNullOrWhiteSpace(input))
            return result;

        var lines = input.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines) {
            int separatorIndex = line.IndexOf(Separator, StringComparison.Ordinal);
            if (separatorIndex < 0)
                continue;

            string key = line.Substring(0, separatorIndex).Trim();
            string value = line.Substring(separatorIndex + Separator.Length).Trim().Trim('\"');

            result[key] = value;
        }

        return result;
    }
}