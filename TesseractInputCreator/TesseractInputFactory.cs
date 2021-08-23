﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesseractInputCreator
{
    public static class TesseractInputFactory
    {

        #region Constructor

        /// <summary>
        /// Generates a basic Tesseract input object maintaining the required constraints.
        /// </summary>
        /// <param name="minLength">The minimum acceptable string length.</param>
        /// <param name="maxLength">The maximum acceptable string length.</param>
        /// <param name="multiLine">Whether the string needs to be broken up (Produces more "square" inputs.</param>
        /// <param name="fontToUse">Which font to use for the text.</param>
        /// <param name="pixelWidth">The width of the generated image.</param>
        /// <param name="pixelHeight">The height of the generated image.</param>
        /// <param name="margin">The margin of whitespace to place around the image.</param>
        /// <param name="background">What image should be used as a background. Null indicates no image i.e. white canvas.<para/>
        /// The background is assumed to have the same dimensions as specified by pixel width and height</param>
        /// <param name="overlay">What image should be used as an overlay mask. Null indicates none.<para/>
        /// The overlay is assumed to already have the alpha/transparency calculated into the image.<para/>
        /// The overlay is assumed to have the same dimensions as specified by pixel width and height</param>
        /// <param name="seed">What seed should be used for generating randomness. Null indicates it doesn't matter.</param>
        public static TesseractInput GenerateTesseractInput(
            int minLength,
            int maxLength,
            bool multiLine,
            Font fontToUse,
            int pixelWidth,
            int pixelHeight,
            int margin,
            Image background = null,
            Image overlay = null,
            int? seed = null)
        {
            // Setting the seed if one wasn't provided.
            // Casting to an int is fine since this cast truncates the MSB, which changes
            // the least often.
            if (!seed.HasValue)
                seed = (int)(DateTime.Now.Ticks);
            System.Random rng = new Random(seed.Value);

            // Generate the string
            string inputText = GenerateRandomString(rng, minLength, maxLength);

            // Set base image to use
            Image bitmap = new Bitmap(pixelWidth, pixelHeight);
            Graphics drawing = Graphics.FromImage(bitmap);

            //paint the background
            drawing.Clear(Color.White);
            if (background != null)
                drawing.DrawImage(background, new Point(0, 0));

            // We only dispose the graphic and not the image itself because it 
            // might be used to create other inputs.
            drawing.Dispose();

            //Create base text image
            BoxObject[] symbolBoxes = GenerateText(bitmap, margin, inputText, fontToUse, multiLine);

            // If an overlay exists, overlay it on the image
            if (overlay != null)
            {
                Graphics overlayGraphic = Graphics.FromImage(bitmap);
                overlayGraphic.DrawImage(overlay, new Point(0, 0));
                overlayGraphic.Dispose();
            }

            // Return the produced input object
            return new TesseractInput(bitmap, symbolBoxes, seed.Value);
        }

        /// <summary>
        /// Generates the text on the image provided as well as calcuates the bounding boxes of each character.
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="inputText"></param>
        /// <param name="font"></param>
        /// <param name="multiLine">Currently not in use</param>
        /// <returns>Returns the array of calculated box objects</returns>
        /// <TODO>pt</TODO>
        private static BoxObject[] GenerateText(Image bitmap, int margin, string inputText, Font font, bool multiLine = false)
        {
            Graphics drawing = Graphics.FromImage(bitmap);

            //measure the string to see how big the image needs to be
            SizeF textSize = drawing.MeasureString(inputText, font);

            // Do we need to scale down the font size? 
            // The -50 allows for border margins
            double widthRatio = (double)textSize.Width / ((double)bitmap.Width - (margin*2));
            double heightRatio = (double)textSize.Height / ((double)bitmap.Height - (margin*2));
            double biggest = widthRatio > heightRatio ? widthRatio : heightRatio;

            Font fontToUse = new Font(font, font.Style);
            // We need to scale the font size by the inverse of the bigger of the two
            if (biggest > 1)
                fontToUse = new Font(font.FontFamily, (int)(font.Size / biggest), font.Style);
            //create a brush for the text
            Brush textBrush = new SolidBrush(Color.Black);

            textSize = drawing.MeasureString(inputText, font);
            int startX = bitmap.Width / 2 - ((int)textSize.Width / 2);
            int starty = bitmap.Height / 2 - ((int)textSize.Height / 2);

            // Generate the boxes and the image
            BoxObject[] retVal = GetBoxes(fontToUse, inputText, startX, starty);
            // String format generic typographic removes the leftmost padding which we don't account for in the boxes
            drawing.DrawString(inputText, fontToUse, textBrush, startX, starty, StringFormat.GenericTypographic);

            return retVal;
        }

        #endregion

        #region Methods

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

        private static BoxObject[] GetBoxes(Font font, string text, int startXPosition, int startYPosition)
        {
            int left = startXPosition;

            // Dummy bitmap for string measuring
            Image tempImage = new Bitmap(1, 1);
            Graphics tempGraphics = Graphics.FromImage(tempImage);

            BoxObject[] boxes = new BoxObject[text.Length];
            for (int i = 0; i < text.Length; i++)
            {
                char ch = text[i];

                //actual width is the (width of XX) - (width of X) to ignore padding
                var size = tempGraphics.MeasureString("" + ch, font);
                var size2 = tempGraphics.MeasureString("" + ch + ch, font);

                using (Bitmap b = new Bitmap((int)size.Width + 2, (int)size.Height + 2))
                {
                    using (Graphics g = Graphics.FromImage(b))
                    {
                        // Place the character on a canvas
                        g.FillRectangle(Brushes.White, 0, 0, size.Width, size.Height);
                        g.DrawString("" + ch, font, Brushes.Black, 0, 0);
                        int top = -1;
                        int bottom = -1;

                        //find the top row
                        for (int y = 0; top < 0 && y < (int)size.Height - 1; y++)
                            for (int x = 0; x < (int)size.Width; x++)
                                if (b.GetPixel(x, y).B < 2)
                                {
                                    top = y;
                                    break;
                                }

                        //find the bottom row
                        for (int y = (int)(size.Height - 1); bottom < 0 && y > 1; y--)
                            for (int x = 0; x < (int)size.Width - 1; x++)
                                if (b.GetPixel(x, y).B < 2)
                                {
                                    bottom = y;
                                    break;
                                }

                        Rectangle rect = new Rectangle(left, startYPosition + top, (int)(size2.Width - size.Width), bottom - top);
                        boxes[i] = new BoxObject(ch, rect);
                    }
                    left += (int)(size2.Width - size.Width);
                }
            }

            tempImage.Dispose();
            return boxes;
        }

        #endregion
    }
}
