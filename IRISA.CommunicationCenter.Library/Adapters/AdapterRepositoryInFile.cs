using IRISA.CommunicationCenter.Library.Loggers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace IRISA.CommunicationCenter.Library.Adapters
{
    public class AdapterRepositoryInFile : IAdapterRepository
    {
        private static readonly string _adapterFilesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Adapters");

        public virtual IEnumerable<IIccAdapter> GetAll()
        {
            return LoadAdaptersFromPath(_adapterFilesPath);
        }

        protected IEnumerable<IIccAdapter> LoadAdaptersFromPath(string adaptersPath)
        {
            if (!Directory.Exists(adaptersPath))
                throw IrisaException.Create($"مسیر ذخیره آداپتور ها موجود نیست. \r\n{adaptersPath}");

            try
            {
                List<IIccAdapter> adapters = new List<IIccAdapter>();
                Type typeFromHandle = typeof(IIccAdapter);

                foreach (var file in Directory.GetFiles(adaptersPath, "*.dll"))
                {
                    foreach (Type type in Assembly.LoadFile(file).GetTypes())
                    {
                        if
                        (
                            typeFromHandle.IsAssignableFrom(type) &&
                            typeFromHandle != type &&
                            !type.ContainsGenericParameters
                        )
                        {
                            adapters.Add((IIccAdapter)Activator.CreateInstance(type));
                        }
                    }
                }
                return adapters;
            }
            catch (Exception exception)
            {
                string message = exception.Message;
                if (exception is ReflectionTypeLoadException)
                {
                    ReflectionTypeLoadException typeLoadException = exception as ReflectionTypeLoadException;
                    if (typeLoadException?.LoaderExceptions.Count() > 0)
                    {
                        message = typeLoadException.LoaderExceptions.First().Message;
                    }
                }
                throw IrisaException.Create($"خطا هنگام لود کردن آداپتور ها. \r\n{message}");
            }
        }
    }
}
