using System;
using System.IO;
using System.Reflection;

namespace AbdusCo.Matbaa
{
    internal static class AssemblyExtensions
    {
        public static string ReadEmbeddedResourceAsString(this Assembly assembly, string qualifiedName)
        {
            var resourceName = $"{assembly.GetName().Name}.{qualifiedName}";
            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                throw new ApplicationException("Cannot find embedded resource");
            }

            var sr = new StreamReader(stream!);
            return sr.ReadToEnd();
        }
    }
}