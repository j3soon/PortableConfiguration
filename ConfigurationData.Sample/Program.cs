using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurationData.Sample
{
    class Program
    {
        public static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        static void Main(string[] args)
        {
            Config config = new Config(Path.Combine(AssemblyDirectory, "config"));
            config.DefaultConfigEvent += () =>
            {
                config["test1"] = "1";
                config["test2"] = "2";
                try
                {
                    config["1test"] = "1";
                }
                catch (Exception e)
                {
                    Console.WriteLine("Caught");
                }
            };
            config.Load();
            foreach (var pair in config.Enumerate())
                Console.WriteLine(pair.Key + ":" + pair.Value);
            config.Save();
            Console.Read();
        }
    }
}
