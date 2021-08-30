using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesseractInputCreator
{
    public static class TesseractInputFactory
    {
        #region Fields

        /// <summary>
        /// The width of the image to generate.
        /// </summary>
        public static int width = 0;

        /// <summary>
        /// The height of the image to generate.
        /// </summary>
        public static int height = 0;

        /// <summary>
        /// Whether the generated text should be centered vertically.
        /// </summary>
        public static bool verticalCentering = false;

        /// <summary>
        /// Whether the generated text should be centered horizontally.
        /// </summary>
        public static bool horizontalCentering = false;

        /// <summary>
        /// Whether the generate text should wrap.
        /// </summary>
        public static bool textWrap = false;

        /// <summary>
        /// Whether the generated boxes should be further
        /// processed to be as tight as possible.
        /// </summary>
        /// <remarks>Currently not implemented.</remarks>
        public static bool tightBoxes = false;

        /// <summary>
        /// The background the text should be drawn upon.
        /// </summary>
        public static Image background = null;

        /// <summary>
        /// The overlay to draw ontop of the text.
        /// </summary>
        public static Image overlay = null;

        /// <summary>
        /// The font to draw the text with.
        /// </summary>
        public static Font font = null;

        #endregion

        /// <summary>
        /// Produces a new tesseract input object for use in training a tesseract model.
        /// </summary>
        /// <param name="text">The text to write to the input</param>
        /// <param name="fileName">The name to name the files</param>
        /// <returns></returns>
        public static TesseractInput NewTesseractInput(string text, string fileName)
        {
            // Set base image to use
            Image bitmap = new Bitmap(width, height);
            Graphics drawing = Graphics.FromImage(bitmap);

            //paint the background
            drawing.Clear(Color.White);
            if (background != null)
                drawing.DrawImage(background, new Point(0, 0));

            // We only dispose the graphic and not the image itself because it 
            // might be used to create other inputs.
            drawing.Dispose();

            //Create base text image
            BoxObject[] boxObjects = Generate(bitmap, text);

            // If an overlay exists, overlay it on the image
            if (overlay != null)
            {
                Graphics overlayGraphic = Graphics.FromImage(bitmap);
                overlayGraphic.DrawImage(overlay, new Point(0, 0));
                overlayGraphic.Dispose();
            }

            // Return the produced input object
            return new TesseractInput(bitmap, boxObjects, fileName);
        }

        #region Helpers 

        /// <summary>
        /// Given an image and some text draws the text on the image with the provided formatting of this class.
        /// Additionally returns the box objects of each character within the image as well.
        /// </summary>
        /// <param name="image">The image to manipulate.</param>
        /// <param name="text">The text whose characters are to have their bounds measured and be drawn to the image.</param>
        /// <returns></returns>
        private static BoxObject[] Generate(Image image, string text)
        {
            // Create Graphics object
            Graphics graphic = Graphics.FromImage(image);

            // Generate the formatter
            StringFormat format = new StringFormat();
            format.Alignment = horizontalCentering ? StringAlignment.Center : StringAlignment.Near;
            format.LineAlignment = verticalCentering ? StringAlignment.Center : StringAlignment.Near;
            format.Trimming = StringTrimming.None;
            format.FormatFlags =
                StringFormatFlags.MeasureTrailingSpaces;
            if (!textWrap)
                format.FormatFlags |= StringFormatFlags.NoWrap;
            CharacterRange[] ranges = new CharacterRange[text.Length];
            for (int i = 0; i < text.Length; i++)
            {
                ranges[i] = new CharacterRange(i, 1);
            }
            format.SetMeasurableCharacterRanges(ranges);

            // Get the rectangles
            List<RectangleF> results = CalculateCharacterBounds(text, graphic, format);

            // Draw the string
            graphic.DrawString(text, font, Brushes.Black, new RectangleF(0, 0, width, height), format);

            // generate the box objects
            List<BoxObject> retVal = new List<BoxObject>();
            for (int i = 0; i < text.Length; i++)
            {
                retVal.Add(new BoxObject(text[i], Rectangle.Ceiling(results[i])));
            }

            // Clena up memory
            graphic.Dispose();

            return retVal.ToArray();

        }

        /// <summary>
        /// For a given string and format specification, calculates the bounding rectangle of each character in the string.
        /// </summary>
        /// <param name="text">The text to calculate.</param>
        /// <param name="graphic">The graphic to perform the "rendering" with.</param>
        /// <param name="format">The format of how to "render" string.</param>
        /// <returns></returns>
        private static List<RectangleF> CalculateCharacterBounds(string text,
            Graphics graphic, StringFormat format)
        {
            List<RectangleF> result = new List<RectangleF>();

            // Find the character range.
            Region[] regions =
                graphic.MeasureCharacterRanges(
                    text, font, new RectangleF(0, 0, width, height),
                    format);

            // Convert the regions into rectangles.
            foreach (Region region in regions)
                result.Add(region.GetBounds(graphic));

            return result;
        }

        #endregion
    }

}

