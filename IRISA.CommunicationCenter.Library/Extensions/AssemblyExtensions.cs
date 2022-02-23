using System.IO;
using System.Reflection;

namespace IRISA.CommunicationCenter.Library.Extensions
{
    public static class AssemblyExtensions
    {
        public static string AsssemblyFileName(this Assembly assembly)
        {
            return Path.GetFileNameWithoutExtension(assembly.Location);
        }
        public static string AssemblyVersion(this Assembly assembly)
        {
            return assembly.GetName().Version.ToString();
        }
        public static string AssemblyName(this Assembly assembly)
        {
            return assembly.GetName().Name;
        }
    }
}
