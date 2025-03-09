namespace TestClient.Utility;
internal static class Color {
    public static UnityEngine.Color FromRGB(int r, int g, int b, float a = 1.0f) {
        return new(r / 255f, g / 255f, b / 255f, a);
    }
}