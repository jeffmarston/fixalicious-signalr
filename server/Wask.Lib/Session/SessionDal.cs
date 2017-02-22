using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using Wask.Lib.Model;
using static System.Environment;

namespace Wask.Lib.Sessions
{
    public static class SessionDal
    {
        private static List<Session> _sessions;
        private static string _filePath;

        public static List<Session> Sessions
        {
            get
            {
                if (_sessions == null)
                {
                    var basePath = Environment.GetFolderPath(SpecialFolder.CommonApplicationData);
                    _filePath = basePath + @"\Eze\sessions.json";
                    if (!File.Exists(_filePath))
                    {
                        Directory.CreateDirectory(basePath + @"\Eze");
                        _sessions = new List<Session>();
                        _sessions.Add(new Session { name = "BAX" });
                        _sessions.Add(new Session { name = "MARS" });
                        File.WriteAllText(_filePath, JsonConvert.SerializeObject(_sessions));
                    }
                    else
                    {
                        JArray json = JArray.Parse(File.ReadAllText(_filePath));
                        _sessions = json.ToObject<List<Session>>();
                    }
                }
                return _sessions;
            }
        }

        public static void Save()
        {
            File.WriteAllText(_filePath, JsonConvert.SerializeObject(Sessions));
        }
    }
}
