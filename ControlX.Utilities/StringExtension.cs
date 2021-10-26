namespace ControlX.Utilities;

public static class StringExtension
{
    public static byte[] GetFingerprint(this string fingerprint)
    {
        var parts = fingerprint.Split(':');
        var result = new byte[parts.Length];
        for (int i = 0; i < parts.Length; i++)
        {
            int value = Convert.ToInt32(parts[i], 16);
            result[i] = Convert.ToByte(value);
        }

        return result;
    }
}