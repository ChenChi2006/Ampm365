using System;
using PosPrint;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string rst1;
                int rst2 = OpenCashBox.OpenCashBox.openCashBox(out rst1);
                Console.WriteLine(rst1);
                Console.WriteLine(rst2);
            }
            catch (Exception)
            {
                Console.WriteLine("直接调用openCashBox失败");
            }
            try
            {
                string rst2 = new OpenCashBox_QS().openCashBox();
                Console.WriteLine(rst2);
            }
            catch (Exception)
            {
                Console.WriteLine("调用封装后的openCashBox失败");
            }
            Console.ReadKey();//防止闪退 
        }
    }
}
