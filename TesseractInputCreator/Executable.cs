using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesseractInputCreator
{
    /// <summary>
    /// The console executable of the input creator
    /// </summary>
    class Executable
    {

        #region Fields

        /// <summary>
        /// If this program is running in debug mode.
        /// </summary>
        public static bool debug = false;

        /// <summary>
        /// The directory to place all output files into.
        /// </summary>
        public static DirectoryInfo outputDirectory = null;

        /// <summary>
        /// The minimum length to make the generated string 
        /// </summary>
        public static int minStringLength = 0;

        /// <summary>
        /// The maximum length to make the generated string 
        /// </summary>
        public static int maxStringLength = 0;

        #endregion

        #region Main

        static void Main(string[] args)
        {
            ExtractArguments();
            Console.ReadLine();
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Extracts the required arguments from command line.
        /// </summary>
        private static void ExtractArguments()
        {
            if (GetFlag(Config.HELP_FLAG_ONE) || GetFlag(Config.HELP_FLAG_TWO))
            {
                WriteHelp();
                return;
            }

            // Check if we are in debug mode
            debug = GetFlag(Config.DEBUG_FLAG);

            // Extract the output directory
            ParseOutputDirectoryArgs();

            // Extract the font values
            ParseFontArgs();

            // Extract the width and height
            ParseWidthHeightArgs();

            // Extract the minimum and maximum lengths;
            ParseStringLengthArgs();

            // Extract the background and overlay;
            ParseImageArgs();

            // Extract the optional flags
            ParseFlags();
        }

        private static void WriteHelp()
        {
            Console.WriteLine(
                "Tesseract Input Creator (1.0)\n\n" +
                "The tesseract input creator is a program used to generate a series of inputs to be used for" +
                "training a tesseract OCR model.\n\n" +
                "Usage: TesseractInputCreator {-WIDTH <+int>} {-HEIGHT <+int>}\n" +
                "{-FONT <path> | -FAMILY <string> -STYLE {Regular | Bold | Italic | Underline | Strikeout}}\n"+
                "{-OUT <path>} {-LENGTH { +int | \'+int,+int\' }}\n"+
                "[-OVERLAY <path>] [-BACKGROUND <path>] [v] [h] [w] [t] [d]\n\n" +
                "See github https://github.com/DaviesCooper/TesseractInputCreator for guide on argument usage."
                );
        }

        /// <summary>
        /// Parses the flags corresponding with centering, wraping, etc.
        /// </summary>
        private static void ParseFlags()
        {
            TesseractInputFactory.verticalCentering = GetFlag(Config.VERTICAL_CENTERING_FLAG);
            TesseractInputFactory.horizontalCentering = GetFlag(Config.HORIZONTAL_CENTERING_FLAG);
            TesseractInputFactory.textWrap = GetFlag(Config.TEXT_WRAP_FLAG);
            TesseractInputFactory.tightBoxes = GetFlag(Config.TIGHT_BOXES_FLAG);
        }

        /// <summary>
        /// Parses the arguments corresponding with the overlay and background images.
        /// </summary>
        private static void ParseImageArgs()
        {
            // Get overlay
            string overlay = GetArgument(Config.OVERLAY_IMAGE_PATH_KEY);
            if (!String.IsNullOrEmpty(overlay))
            {
                // Check that the file exists
                if (File.Exists(overlay))
                    // Check that its a valid image
                    try
                    {
                        Image fromFile = Image.FromFile(overlay);
                        TesseractInputFactory.overlay = fromFile;
                    }
                    catch
                    {
                        Console.WriteLine("The supplied overlay image could not be opened. Ignoring.");
                    }
                else
                    Console.WriteLine("The supplied overlay image could not be found. Ignoring.");
            }

            // Get background
            string background = GetArgument(Config.BACKGROUND_IMAGE_PATH_KEY);
            if (!String.IsNullOrEmpty(background))
            {
                // Check that the file exists
                if (File.Exists(background))
                    // Check that its a valid image
                    try
                    {
                        Image fromFile = Image.FromFile(background);
                        TesseractInputFactory.background = fromFile;
                    }
                    catch
                    {
                        Console.WriteLine("The supplied background image could not be opened. Ignoring.");
                    }
                else
                    Console.WriteLine("The supplied background image could not be found. Ignoring.");
            }
        }

        /// <summary>
        /// Parses the arguments corresponding with the length of strings to produce.
        /// </summary>
        private static void ParseStringLengthArgs()
        {
            // Get string length value
            string stringLength = GetArgument(Config.STRING_LENGTH_KEY);
            if (String.IsNullOrEmpty(stringLength))
            {
                Console.WriteLine("String length must be specified.");
                System.Environment.Exit(-1);
            }
            string[] args = stringLength.Split(',');
            // Error if they provide three values
            if (args.Length > 2)
            {
                Console.WriteLine("String length must be one or two numbers separated by a comma.");
                System.Environment.Exit(-1);
            }
            // Parsing if they provided two values
            if (args.Length > 1)
            {
                string minLength = args[0];
                int minInt = -1;
                string maxLength = args[1];
                int maxInt = -1;
                // Error out on any wrong values
                if (!int.TryParse(minLength, out minInt) || minInt < 1)
                {
                    Console.WriteLine("minimum length must be an integer greater than 0.");
                    System.Environment.Exit(-1);
                }
                if (!int.TryParse(maxLength, out maxInt) || maxInt < 1)
                {
                    Console.WriteLine("maximum length must be an integer greater than 0.");
                    System.Environment.Exit(-1);
                }
                maxStringLength = maxInt;
                minStringLength = minInt;
                return;
            }
            // Parsing if they provided one value
            string length = args[0];
            int lengthInt = -1;
            // Error out on any wrong values
            if (!int.TryParse(length, out lengthInt) || lengthInt < 1)
            {
                Console.WriteLine("length must be an integer greater than 0.");
                System.Environment.Exit(-1);
            }
            maxStringLength = lengthInt;
            minStringLength = lengthInt;
        }

        /// <summary>
        /// Parses the arguments corresponding with the width and height of the images to produce.
        /// </summary>
        private static void ParseWidthHeightArgs()
        {
            // Get width value
            string stringWidth = GetArgument(Config.WIDTH_KEY);
            int intWidth = -1;
            if (String.IsNullOrEmpty(stringWidth))
            {
                Console.WriteLine("image width must be specified.");
                System.Environment.Exit(-1);
            }
            if (!int.TryParse(stringWidth, out intWidth))
            {
                Console.WriteLine("image width must be an integer greater than 0.");
                System.Environment.Exit(-1);
            }
            TesseractInputFactory.width = intWidth;

            // Get height value
            string stringHeight = GetArgument(Config.HEIGHT_KEY);
            int intHeight = -1;
            if (String.IsNullOrEmpty(stringHeight))
            {
                Console.WriteLine("image height must be specified.");
                System.Environment.Exit(-1);
            }
            if (!int.TryParse(stringHeight, out intHeight))
            {
                Console.WriteLine("image height must be an integer greater than 0.");
                System.Environment.Exit(-1);
            }
            TesseractInputFactory.height = intHeight;
        }

        /// <summary>
        /// Parses the arguments corresponding with where to save the output.
        /// </summary>
        private static void ParseOutputDirectoryArgs()
        {
            // Get the output directory
            string outputDir = GetArgument(Config.OUTPUT_DIRECTORY_KEY);
            if (String.IsNullOrEmpty(outputDir))
            {
                Console.WriteLine("Output directory must be specified.");
                System.Environment.Exit(-1);
            }
            outputDirectory = new DirectoryInfo(outputDir);
            if (!outputDirectory.Exists)
            {
                Console.WriteLine("Output directory does not exist.");
                System.Environment.Exit(-1);
            }
        }

        /// <summary>
        /// Parses the arguments corresponding to defining which font to use
        /// </summary>
        private static void ParseFontArgs()
        {
            // Extract font size
            string fontSize = GetArgument(Config.FONT_SIZE_KEY);
            int asInt = -1;
            // Check that it is valid
            if (String.IsNullOrEmpty(fontSize) || !int.TryParse(fontSize, out asInt))
            {
                Console.WriteLine("A font size or name is required.");
                System.Environment.Exit(-1);
            }
            if (asInt < 1)
            {
                Console.WriteLine("font size must be greater than 0.");
                System.Environment.Exit(-1);
            }

            // Now check for a custom font
            string customFont = GetArgument(Config.CUSTOM_FONT_PATH_KEY);
            // Check that it is valid
            if (!String.IsNullOrEmpty(fontSize))
            {
                // See if they passed a nonsense file
                FileInfo inf = new FileInfo(customFont);
                if (!inf.Exists)
                {
                    Console.WriteLine("Custom font file not found.");
                    System.Environment.Exit(-1);
                }
                try
                {
                    PrivateFontCollection col = new PrivateFontCollection();
                    col.AddFontFile(inf.FullName);
                    TesseractInputFactory.font = new Font(col.Families[0], asInt);
                    return;
                }
                catch
                {
                    Console.WriteLine("Invalid custom font file.");
                    System.Environment.Exit(-1);
                }
            }

            // If we get here it means no custom font was passed so we need to try to load an actual font
            string fontFamily = GetArgument(Config.FONT_FAMILY_KEY);
            string fontStyle = GetArgument(Config.FONT_STYLE_KEY);
            FontStyle style = FontStyle.Regular;

            // Check that the font specifications work
            if (String.IsNullOrEmpty(fontFamily) || String.IsNullOrEmpty(fontStyle) ||
                !Enum.TryParse<FontStyle>(fontStyle, out style))
            {
                Console.WriteLine("Invalid font specifications.");
                System.Environment.Exit(-1);
            }
            if (!IsFontInstalled(fontFamily, style))
            {
                Console.WriteLine("Specified font or font style is not installed.");
                System.Environment.Exit(-1);
            }

            // Set the font
            TesseractInputFactory.font = new Font(fontFamily, asInt, style);
        }

        /// <summary>
        /// Retrieves the value of a command line argument.
        /// Assumes that a key will be followed up immediately by value.
        /// </summary>
        private static string GetArgument(string key)
        {
            string[] args = System.Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length - 1; i++)
                if (args[i] == key)
                    return args[i + 1];
            return null;
        }

        /// <summary>
        /// Determines the presence of a command line flag.
        /// </summary>
        private static bool GetFlag(string key)
        {
            string[] args = System.Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
                if (args[i] == key)
                    return true;
            return false;
        }

        /// <summary>
        /// Checks if a specified font with a style is installed.
        /// </summary>
        /// <param name="fontName">The font's family name found on the os.</param>
        /// <param name="fontName">The style to check that the font has.</param>
        private static bool IsFontInstalled(string fontName, FontStyle style)
        {
            try
            {
                using (FontFamily family = new FontFamily(fontName))
                    return family.IsStyleAvailable(style);
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        /// <summary>
        /// Generate a random ascii character sequence (Human-readable so characters 33 to 126 inclusive)
        /// guaranteed to be at least <paramref name="minLength"/> characters and at most
        /// <paramref name="maxLength"/> characters.
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

        #endregion

    }
}
