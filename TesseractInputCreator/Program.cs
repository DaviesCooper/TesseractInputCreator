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


            TesseractInputFactory.font = new Font(FontFamily.GenericSansSerif, 50);
            TesseractInputFactory.verticalCentering = true;
            TesseractInputFactory.height = 500;
            TesseractInputFactory.width = 500;
            TesseractInputFactory.multiline = false;

            for (int i = 0; i < 100; i++)
            {
                Random rng = new Random(i);
                TesseractInput inp = TesseractInputFactory.NewTesseractInput(GenerateRandomString(rng, 15, 30), i);
                inp.SaveToDirectory(inf, true);
            }
        }

        /// <summary>
        /// Generate a random ascii character sequence (Human-readable so characters 33 to 126 inclusive)
        /// guaranteed to be at least <paramref name="minLength"/> characters and at most 
        /// <paramref name="maxLength"/> characters
        /// </summary>
        /// <param name="minLength">The minimum numbers of characters allowed in the string.</param>
        /// <param name="maxLength">The maximum number of characters allowed in the string.</param>
        private static string GenerateRandomString(System.Random rng, int minLength, int maxLength)
        {
            int textLength = rng.Next(minLength, maxLength + 1);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < textLength; i++)
            {
                byte charToUse = (byte)rng.Next(33, 127);
                builder.Append(Convert.ToChar(charToUse));
            }
            return builder.ToString();
        }
    }
}
