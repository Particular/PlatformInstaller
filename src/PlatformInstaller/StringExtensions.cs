using System;
using System.Runtime.InteropServices;
using System.Security;

public static class StringExtensions
{
    public static byte[] GetBytes(this string str)
    {
        var bytes = new byte[str.Length * sizeof(char)];
        Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
        return bytes;
    }

    public static string GetString(this byte[] bytes)
    {
        var chars = new char[bytes.Length / sizeof(char)];
        Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
        return new string(chars);
    }

    public static string ToOriginalString(this SecureString value)
    {
        var bstr = Marshal.SecureStringToBSTR(value);
        try
        {
            return Marshal.PtrToStringBSTR(bstr);
        }
        finally
        {
            Marshal.FreeBSTR(bstr);
        }
    }

    public static SecureString ToSecureString(this string value)
    {
        var secureString = new SecureString();
        foreach (var ch in value)
        {
            secureString .AppendChar(ch);
        }
        return secureString;
    }

    public static string QuoteForCommandline(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;
        return value.Contains(" ")
            ? $"\"{value}\""
            : value;
    }
}
