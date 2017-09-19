using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Xsl;
using ITGlobal.CommandLine;

namespace xsltator
{
    class Program
    {
        private static string xsltPath, xmlPath, htmlPath;

        static void Main(string[] args)
        {
            if(!SetupCLI(args))
                return;
            
            using (CLI.WithForeground(ConsoleColor.Green))
            {
                Console.WriteLine($"Input xslt is:\t\t{xsltPath}");
                Console.WriteLine($"Input xml is:\t\t{xmlPath}");
                Console.WriteLine($"Output html is:\t\t{htmlPath}");
            }

            Console.WriteLine("Loading xslt...");
            var xslt = new XslCompiledTransform();
            using (var reader = new StreamReader(xsltPath))
            {
                using (var xmlReader = new XmlTextReader(reader))
                {
                    xslt.Load(xmlReader);
                }
            }

            Console.WriteLine("Loading xml...");
            var xml = new XmlDocument();
            using (var reader = new StreamReader(xmlPath))
            {
                xml.Load(reader);
            }

            Console.WriteLine("Transforming...");
            var writer = new XmlTextWriter(htmlPath, Encoding.UTF8);
            xslt.Transform(xml, writer);

            Console.WriteLine("Done. Press any...");
            Console.ReadKey();
        }

        /// <summary>
        /// Initialize command line args parser
        /// </summary>
        /// <param name="args"></param>
        private static bool SetupCLI(string[] args)
        {
            ICommandParser cli = null;
            try
            {
                cli = CLI.Parser().Title("xsltator").Version("1.0.0").HelpText("Utility for applying an xslt to an xml");
                var xslt = cli.Parameter<string>("xslt").Required(true).HelpText("Xslt file path");
                var xml = cli.Parameter<string>("xml").Required(true).HelpText("Xml file path");
                var html = cli.Parameter<string>("html").Required(false).HelpText("Optional output html file path");

                cli.Parse(args);
                
                xsltPath = xslt.Value;
                xmlPath = xml.Value;

                htmlPath = html.IsSet ? html.Value : Path.ChangeExtension(xmlPath, "html");
            }
            catch (Exception exception)
            {
                using (CLI.WithColors(ConsoleColor.White, ConsoleColor.Red))
                {
                    Console.WriteLine($"\nError while reading command line:\t{exception.Message}\n");
                    using (CLI.WithForeground(ConsoleColor.Black))
                    {
                        Console.WriteLine("Press any...");
                        Console.ReadKey();
                        return false;
                    }
                }
            }

            return true;
        }

    }
}
