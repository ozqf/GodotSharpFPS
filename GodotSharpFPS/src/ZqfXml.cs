using System;
using System.IO;
using System.Reflection;
using System.Xml;

namespace GodotSharpFps.src
{
    public class ZqfXml
    {
        public static void ListAllAssemblyResources(Assembly assembly)
        {
            Console.WriteLine($"--- List resources in assmebly {assembly.FullName}");
            //string str = string.Empty;
            string[] resourceNames = assembly.GetManifestResourceNames();
            foreach(string name in resourceNames)
            {
                Console.WriteLine(name);
            }
        }

        public static string ReadAssemblyEmbeddedText(string path)
        {
            Console.WriteLine($"Reading embedded text at {path}");
            string result = string.Empty;
            using (Stream stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(path))
            {
                using (StreamReader r = new StreamReader(stream))
                {
                    result = r.ReadToEnd();
                }
            }
            return result;
        }

        public static void TestReadXml(string raw)
        {
            XmlReader r = XmlReader.Create(new StringReader(raw));
            try
            {
                while (r.Read())
                {
                    switch (r.NodeType)
                    {
                        case XmlNodeType.Element:
                            Console.WriteLine(r.Name + r.AttributeCount + " attribs");
                            XmlReader sub = r.ReadSubtree();
                            break;
                    }
                    
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception reading xml");
                Console.WriteLine(ex.Message);
            }
        }
    }
}
