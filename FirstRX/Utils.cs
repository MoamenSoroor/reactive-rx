using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstRX
{
    public class Utils
    {
        public string RepeateFunction(int times = 10)
        {

            var result = Enumerable.Range(1, times).Select(v => @$"
public static void GenerateSequence{v}()
{{
    
}}
").ToList();

            return string.Join(Environment.NewLine, result);
        }



    }
}
