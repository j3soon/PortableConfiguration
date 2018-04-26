using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace PortableConfiguration
{
    public class Config
    {
        private String _path = null;
        private SortedDictionary<String, String> _dict = new SortedDictionary<String, String>(StringComparer.OrdinalIgnoreCase);
        private Regex _keyRegex = new Regex("^[a-zA-Z_][a-zA-Z_0-9]*$");

        public delegate void DefaultConfigEventHandler();
        public event DefaultConfigEventHandler DefaultConfigEvent;

        public String this[String s]
        {
            get { return _dict[s]; }
            set
            {
                if (String.IsNullOrEmpty(s))
                    throw new ArgumentException();
                if (!_keyRegex.IsMatch(s))
                    throw new ArgumentException();
                _dict[s] = value;
            }
        }

        public static String GetAssemblyDirectory(Type type)
        {
            String codeBase = type.Assembly.CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            String path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }

        public Config(String path)
        {
            FileInfo f = new FileInfo(path);
            _path = f.FullName;
        }

        public void Load()
        {
            DefaultConfigEvent?.Invoke();
            if (!File.Exists(_path))
                File.Create(_path).Close();
            StreamReader sr = new StreamReader(_path);
            String key = "", value = "";
            //0->key
            //1->val
            int state = 0;
            int ret;
            char c;
            while (true)
            {
                ret = sr.Read();
                c = (char)ret;
                if (ret == '\r' || ret == '\n' || ret == -1)
                {
                    //Save if key isn't empty.
                    if (key != "")
                    {
                        this[key] = value;
                        state = 0;
                        key = value = "";
                    }
                    if (ret == -1)
                        break;
                }
                else if (state == 0 && c == ':')
                    state = 1;
                else if (state == 0)
                    key += c;
                else
                    value += c;
            }
            sr.Close();
        }

        public void Save()
        {
            FileInfo f = new FileInfo(_path);
            Directory.CreateDirectory(Path.GetDirectoryName(_path));
            StreamWriter sw = new StreamWriter(_path);
            foreach (KeyValuePair<String, String> pair in _dict)
                sw.WriteLine(pair.Key + ":" + pair.Value);
            sw.Flush();
            sw.Close();
        }

        public IEnumerable<KeyValuePair<String, String>> Enumerate()
        {
            foreach (KeyValuePair<String, String> pair in _dict)
                yield return pair;
        }
    }
}
