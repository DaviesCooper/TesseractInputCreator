using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesseractInputCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            DirectoryInfo inf = new DirectoryInfo(args[0]);
            for (int i = 0; i < 100; i++)
            {
                TesseractInput input = TesseractInputFactory.GenerateTesseractInput(5, 10, false, new Font(FontFamily.GenericSerif, 70), 500, 500, 0);
                input.SaveToDirectory(inf, true);
            }
        }
    }
}
