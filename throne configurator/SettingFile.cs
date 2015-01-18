using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace throne_configurator
{
    public class SettingFile
    {
        public List<Tuple<string, string>> Settings = new List<Tuple<string, string>>();
        private readonly string Filepath;
        public string SettingType;

        public SettingFile(string FilePath,string settingtype)
        {
            Filepath = FilePath;
            SettingType = settingtype;
            FetchKeyValuePairs();
        }

        public void FetchKeyValuePairs()
        {
            foreach (var line in File.ReadAllLines(Filepath).Where(line => line.Contains(":")))
            {
                if (line.Contains("require"))
                {
                    Settings.Add(new Tuple<string, string>("require",line.Replace("require",string.Empty).Trim()));
                }
                else
                {
                    var splittingIndex = line.IndexOf(':');
                    Settings.Add(new Tuple<string, string>(line.Substring(0, splittingIndex).Trim(),
                        line.Substring(splittingIndex + 1, line.Length - splittingIndex - 1).Trim()));
                }
            }
        }

        public void SaveFile()
        {
            using (var sw = new StreamWriter(Filepath))
            {
                sw.WriteLine(@"// Throne Configuration File");
                sw.WriteLine(@"//" + SettingType);
                foreach (var setting in Settings)
                {
                    if(setting.Item1 != "require")
                        sw.WriteLine("{0} : {1}", setting.Item1, setting.Item2);
                    else
                        sw.WriteLine("{0} {1}", setting.Item1, setting.Item2);
                }
            }
        }

    }

}
