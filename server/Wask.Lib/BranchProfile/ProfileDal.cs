using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wask.Lib.Model;
using Wask.Lib.Utils;
using static System.Environment;

namespace Wask.Lib.BranchProfile
{
    public static class ProfileDal
    {
        private static List<Profile> _profiles;
        private static string _filePath;

        public static List<Profile> Profiles
        {
            get
            {
                if (_profiles == null)
                {
                    var basePath = Environment.GetFolderPath(SpecialFolder.CommonApplicationData);
                    _filePath = basePath + @"\Eze\profiles.json";
                    if (!File.Exists(_filePath))
                    {
                        Directory.CreateDirectory(basePath + @"\Eze");
                        _profiles = new List<Profile>();
                        _profiles.Add(new Profile { name = "Mainline", path = @"D:\Tfs\Oms\Mainline" });
                        File.WriteAllText(_filePath, JsonConvert.SerializeObject(_profiles));
                    }
                    else
                    {
                        JArray json = JArray.Parse(File.ReadAllText(_filePath));
                        _profiles = json.ToObject<List<Profile>>();
                    }
                }
                return _profiles;
            }
        }

        public static void Save()
        {
            File.WriteAllText(_filePath, JsonConvert.SerializeObject(Profiles));
            ServiceWatcher.Init(ServiceUtils.GetAllEzeServices());
        }
    }
}
