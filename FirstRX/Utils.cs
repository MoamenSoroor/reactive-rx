using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstRX
{
    public static class Utils
    {
        public static string RepeateFunction(int times = 10)
        {

            var result = Enumerable.Range(1, times).Select(v => @$"
public static void GenerateSequence{v}()
{{
    
}}
").ToList();

            return string.Join(Environment.NewLine, result);
        }



        public static void RenameNamespaces(string path = @"D:\Projects\CSharpProjects\ReactiveProgramming\FirstRX\SequenceBasics\")
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            var files = dir.GetFiles("*.cs");

            files.ToList().ForEach(file =>
            {
                var content = File.ReadAllText(file.FullName)
                    .Replace("namespace FirstRX", "namespace FirstRX.SequenceBasics");
                var newdir = Path.Combine(file.DirectoryName, "test");
                Directory.CreateDirectory(newdir);
                File.WriteAllText($"{Path.Combine(newdir, file.Name)}", content);

            });

            Console.WriteLine("done.");



        }





    }
}
