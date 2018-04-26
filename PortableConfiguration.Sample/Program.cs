using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PortableConfiguration.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            Config config = new Config(Path.Combine(Config.GetAssemblyDirectory(typeof(Program)), "config"));
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
                    Console.WriteLine("1test is invalid config name");
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
