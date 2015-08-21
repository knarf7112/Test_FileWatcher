using System;

namespace Test_FileWatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            string s = null;
            string s2 = s;
            FileSystemListener fsl = new FileSystemListener(@"D:\temp", "*.txt");
            Console.WriteLine("開始監控");
            Console.ReadKey();
        }
    }
}
