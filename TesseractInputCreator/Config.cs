using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesseractInputCreator
{
    public static class Config
    {

        #region Keys

        /// <summary>
        /// The key used to declare how many inputs to generate.
        /// </summary>
        public const string NUMBER_OF_INPUTS_KEY = "-NUM";

        /// <summary>
        /// The key used to declare the font path.
        /// </summary>
        public const string CUSTOM_FONT_PATH_KEY = "-FONT";

        /// <summary>
        /// The key used to declare the font family.
        /// </summary>
        public const string FONT_FAMILY_KEY = "-FAMILY";

        /// <summary>
        /// The key used to declare the font style.
        /// </summary>
        public const string FONT_STYLE_KEY = "-STYLE";

        /// <summary>
        /// The key used to declare the font size.
        /// </summary>
        public const string FONT_SIZE_KEY = "-SIZE";

        /// <summary>
        /// The key used to declare the output directory.
        /// </summary>
        public const string OUTPUT_DIRECTORY_KEY = "-OUT";

        /// <summary>
        /// The command line key used for setting the width of the canvas.
        /// </summary>
        public const string WIDTH_KEY = "-WIDTH";

        /// <summary>
        /// The command line key used for setting the height of the canvas.
        /// </summary>
        public const string HEIGHT_KEY = "-HEIGHT";

        /// <summary>
        /// The command line key used for setting the string text length.
        /// </summary>
        public const string STRING_LENGTH_KEY = "-LENGTH";

        /// <summary>
        /// The command line key used for setting the overlay image.
        /// </summary>
        public const string OVERLAY_IMAGE_PATH_KEY = "-OVERLAY";

        /// <summary>
        /// The command line key used for setting the background image.
        /// </summary>
        public const string BACKGROUND_IMAGE_PATH_KEY = "-BACKGROUND";
        #endregion

        #region Flags

        /// <summary>
        /// The command line flag used to inquire for help with arguments.
        /// </summary>
        public const string HELP_FLAG_ONE = "--help";

        /// <summary>
        /// The command line flag used to inquire for help with arguments.
        /// </summary>
        public const string HELP_FLAG_TWO = "--h";

        /// <summary>
        /// The command line flag used to declare if vertical centering should occur.
        /// </summary>
        public const string VERTICAL_CENTERING_FLAG = "v";

        /// <summary>
        /// The command line flag used to declare if horizontal centering should occur.
        /// </summary>
        public const string HORIZONTAL_CENTERING_FLAG = "h";

        /// <summary>
        /// The command line flag used to declare if text should wrap.
        /// </summary>
        public const string TEXT_WRAP_FLAG = "w";

        /// <summary>
        /// The command line flag used to indicate if the boxes are to be tightly wrapped
        /// </summary>
        public const string TIGHT_BOXES_FLAG = "t";

        /// <summary>
        /// The command line flag used to indicate if debug is enabled.
        /// </summary>
        public const string DEBUG_FLAG = "d";

        #endregion

    }

}
