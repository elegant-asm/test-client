using System;
using System.Text;

namespace TestClient.Utility;

internal class Random {
    private static readonly char[] chars =
        "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
    private static readonly System.Random random = new();

    public static string RandomString(int length=10) {
        if (length < 0)
            throw new ArgumentOutOfRangeException(nameof(length), "Length must be non-negative.");

        StringBuilder stringBuilder = new StringBuilder(length);
        for (int i = 0; i < length; i++) {
            int index = random.Next(0, chars.Length);
            stringBuilder.Append(chars[index]);
        }
        return stringBuilder.ToString();
    }
}
