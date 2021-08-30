using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesseractInputCreator
{
    /// <summary>
    /// A wrapper for the image and symbolic representations because it was becoming annoying having to out
    /// both variables.
    /// </summary>
    public class TesseractInput
    {

        #region Fields

        /// <summary>
        /// The underlying image representation used for creating a .tif file.
        /// </summary>
        public Image image { get; private set; }

        /// <summary>
        /// The underlying symbols and their bounding boxes used for creating a .box file.
        /// </summary>
        public BoxObject[] symbolBoxes { get; private set; }

        /// <summary>
        /// Seed value used for generating this input. Recorded so as to name the file with this.
        /// </summary>
        private string fileName;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor used for setting object fields.
        /// </summary>
        /// <param name="image">The tif image.</param>
        /// <param name="symbols">The symbolic representation.</param>
        /// <param name="seed">The seed used to create.</param>
        public TesseractInput(Image image, BoxObject[] symbols, string fileName)
        {
            this.image = image;
            this.symbolBoxes = symbols;
            this.fileName = fileName;
        }

        #endregion

        #region I/O

        /// <summary>
        /// Saves this tesseract input to a .tif and a .box object in a given directory.
        /// The files names will be the seed used to generate this image so it can be recreated at a
        /// later time.
        /// </summary>
        /// <param name="directory">The directory to save this input to.</param>
        /// <param name="createVerifier">If true a secondary png file will be created with the bounding boxes also drawn onto the image.</param>
        /// <param name="dispose">After writing, do we free up the resources used by the image?</param>
        public void SaveToDirectory(DirectoryInfo directory, bool createVerifier = false, bool dispose = false)
        {
            string tifPath = Path.Combine(directory.FullName, fileName + ".tiff");
            SaveTIF(tifPath);
            string boxPath = Path.Combine(directory.FullName, fileName + ".box");
            SaveBOX(boxPath);
            // Do this last because 
            if (createVerifier)
            {
                string verifyPath = Path.Combine(directory.FullName, fileName + ".verify.png");
                SaveVerify(verifyPath);
            }
            if (dispose)
                image.Dispose();
        }

        /// <summary>
        /// Saves the image of the input to a .tif file specified by the path.
        /// </summary>
        /// <param name="filePath">The path to save this .tif to</param>
        private void SaveTIF(string filePath)
        {
            image.Save(filePath, ImageFormat.Tiff);
        }

        /// <summary>
        /// Saves the image of the input to a .png file specified by the path.
        /// This file has the bounding boxes drawn to it so that it can be used as verification
        /// for determining if the input for tesseract makes sense
        /// </summary>
        /// <param name="filePath">The path to save this .tif to</param>
        private void SaveVerify(string filePath)
        {
            Image clone = image.DeepClone();
            Graphics g = Graphics.FromImage(clone);

            foreach (BoxObject bO in symbolBoxes)
                g.DrawRectangle(new Pen(Color.Red, 3), bO.rectangle);

            g.Dispose();
            clone.Save(filePath, ImageFormat.Png);
            clone.Dispose();
        }

        /// <summary>
        /// Saves the boxObjects of the input to a .box file specified by the path.
        /// <para/>
        /// *IMPORTANT*: The Rectangle of the graphics are top-left origined while
        /// the rectangle used for tesseract is bottom-left oriented. This save method
        /// performs the conversion to have them all be bottom-left oriented.
        /// </summary>
        /// <param name="filePath">The path to save this .box to</param>
        private void SaveBOX(string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (BoxObject obj in symbolBoxes)
                {
                    string toWrite = String.Format
                    (
                        "{0} {1} {2} {3} {4} 0",
                        obj.symbol == '\n' ? '\t' : obj.symbol,
                        obj.rectangle.Left.ToString(),
                        (image.Height - obj.rectangle.Bottom).ToString(),
                        obj.rectangle.Right.ToString(),
                        (image.Height - obj.rectangle.Top).ToString()
                    );
                    writer.WriteLine(toWrite);
                }
                writer.Flush();
            }
        }

        #endregion

    }

    public static class TesseractInputExtensions
    {
        /// <summary>
        /// Transforms an input via the provided function.
        /// <para/>
        /// When performing a transformation it is YOUR job to ensure that if the graphical
        /// glyph coordinates change, the BoxObject coordinates are correspondingly changed.!
        /// </summary>
        /// <param name="func">The transforming function used to produce a new input from.</param>
        /// <returns></returns>
        public static TesseractInput Transform(this TesseractInput input, Func<TesseractInput, TesseractInput> func)
        {
            return func(input);
        }

        /// <summary>
        /// Creates a deep clone copy of a given image.
        /// </summary>
        /// <param name="image">The image to clone.</param>
        /// <returns>The clone image.</returns>
        public static Image DeepClone(this Image image)
        {
            Image clone = new Bitmap(image.Width, image.Height);
            Graphics g = Graphics.FromImage(clone);
            g.DrawImage(image, new Point(0, 0));
            g.Dispose();
            return clone;
        }
    }
}