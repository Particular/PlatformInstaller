using System;
using System.Runtime.InteropServices;

public class WindowsVersion
{
    public static Windows GetOperatingSystem()
    {
        const byte VER_NT_WORKSTATION = 1;

        var osVersionInfoEx = new OSVersionInfoEx
        {
            OSVersionInfoSize = (uint)Marshal.SizeOf(typeof(OSVersionInfoEx))
        };

        GetVersionEx(osVersionInfoEx);

        switch (Environment.OSVersion.Version.Major)
        {
            case 10:
                return Windows.Workstation10;

            case 6:
                if (Environment.OSVersion.Version.Minor == 0)
                {
                    return Windows.Unsupported; //Vista and Windows 2008 R1
                }
                if (Environment.OSVersion.Version.Minor == 1)
                {
                    return osVersionInfoEx.ProductType == VER_NT_WORKSTATION ? Windows.Workstation7 : Windows.Server2008R2;
                }
                if (Environment.OSVersion.Version.Minor == 2 || Environment.OSVersion.Version.Minor == 3)
                {
                    return osVersionInfoEx.ProductType == VER_NT_WORKSTATION ? Windows.Workstation8 :Windows.Server2012;
                }
                return Windows.Unsupported;

            default:
                return Windows.Unsupported;
        }
    }


    [DllImport("Kernel32", CharSet = CharSet.Auto)]
    static extern bool GetVersionEx([Out] [In] OSVersionInfo versionInformation);


    // ReSharper disable UnusedField.Compiler
    // ReSharper disable NotAccessedField.Local
    // ReSharper disable UnassignedField.Compiler
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    class OSVersionInfoEx : OSVersionInfo
    {
        public ushort ServicePackMajor;
        public ushort ServicePackMinor;
        public ushort SuiteMask;
        public byte ProductType;
        public byte Reserved;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    class OSVersionInfo
    {
        // ReSharper disable once NotAccessedField.Global
        public uint OSVersionInfoSize =
            (uint)Marshal.SizeOf(typeof(OSVersionInfo));

        public uint MajorVersion = 0;
        public uint MinorVersion = 0;
        public uint BuildNumber = 0;
        public uint PlatformId = 0;
        // Attribute used to indicate marshalling for String field
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string CSDVersion = null;
    }
}

public enum Windows
{
    Unsupported,
    Workstation7,
    Server2008R2,
    Workstation8,
    Server2012,
    Workstation10
}