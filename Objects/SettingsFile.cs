using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StructGen.Objects
{
    public class SettingsFile<T> where T : class, new()
    {
        public T? data;

        private string file_path;

        public SettingsFile(string file_path)
        {
            this.file_path = file_path;
        }

        public bool Load()
        {
            try
            {
                if (!File.Exists(file_path))
                {
                    data = new T();
                }
                else
                {
                    data = JsonConvert.DeserializeObject<T>(File.ReadAllText(file_path));
                }
            }
            catch (Exception)
            {
                return false;
            }

            return Save();
        }

        public bool Save()
        {
            try
            {
                if(file_path != null && file_path != string.Empty)
                {
                    string? directoryName = Path.GetDirectoryName(file_path);
                    if (!Directory.Exists(directoryName) && directoryName != null)
                    {
                        Directory.CreateDirectory(directoryName);
                    }

                    File.WriteAllText(file_path, JsonConvert.SerializeObject(data, Formatting.Indented));
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}
