using UnityEngine;
using UnityEngine.UI;
using UniverseLib.UI;

namespace TestClient.Utility;

internal static class SimpleUIFactory {
    public static void MakeBottomSeparator(GameObject parent, int height, UnityEngine.Color color) {
        GameObject separator = UIFactory.CreateUIObject("BottomSeperator", parent);
        UIFactory.SetLayoutElement(separator, minHeight: height, flexibleHeight: 0, flexibleWidth: 9999);
        separator.AddComponent<Image>().color = color;
    }
}
