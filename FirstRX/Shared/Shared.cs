using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstRX
{

    public static class OutputFiles
    {
        public const string Output1 = "output.txt";
        public const string Output2 = "output2.txt";
        public const string Output3 = "output3.txt";
        public const string Output4 = "output4.txt";
    }

    public class OutputFileObserver<T> : IObserver<T>
    {
        private readonly string fileName;

        public OutputFileObserver(string fileName = "output.txt")
        {
            this.fileName = fileName;
            File.Delete(this.fileName); 
        }

        public void OnCompleted()
        {
            using StreamWriter writer = File.AppendText(this.fileName);
            writer.WriteLine("Sequence Completed");

        }

        public void OnError(Exception error)
        {
            using StreamWriter writer = File.AppendText(this.fileName);
            writer.WriteLine("Sequence Faulted with {0}", error.ToString());
        }

        public void OnNext(T value)
        {
            using StreamWriter writer = File.AppendText(this.fileName);
            writer.WriteLine("Received value {0}", value);
        }
    }
}
