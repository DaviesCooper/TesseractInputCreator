using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesseractInputCreator
{

    /// <summary>
    /// A class used to store a glyph alongside its bounding box for an arbitrary graphic.
    /// To be translated for a .box object for use with tesseract training.
    /// <para/>
    /// Note: This box object must externally be associated with a specific .TIF image
    /// due to the canvas used supervening on the rectangle location.
    /// </summary>
    public class BoxObject
    {

        #region Fields

        /// <summary>
        /// The Bounding rectangle of the glyph.
        /// </summary>
        public Rectangle rectangle { get; private set; }

        /// <summary>
        /// The symbol the glyph represents
        /// </summary>
        public char symbol { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Argumented constructor.
        /// </summary>
        /// <param name="sym">The symbol represented by the glyph.</param>
        /// <param name="rect">The bounding rectangle of the glyph.</param>
        public BoxObject(char sym, Rectangle rect)
        {
            symbol = sym;
            rectangle = rect;
        }

        #endregion

        #region Methods

        /// <summary>
        /// *symbol* *left* *bottom* *right* *top* *page*
        /// <para/>
        /// Note: Page will always be 0 for our purposes.
        /// </summary>
        /// <returns>The Box object formatted in .box format as <a href="https://github.com/tesseract-ocr/tessdoc/blob/master/tess4/TrainingTesseract-4.00.md#making-box-files">specified by Tesseract.</a></returns>
        public override string ToString()
        {
            return String.Format
            (
                "{0} {1} {2} {3} {4} 0",
                symbol,
                rectangle.Left.ToString(),
                rectangle.Bottom.ToString(),
                rectangle.Right.ToString(),
                rectangle.Top.ToString()
            );
        }

        #endregion
    }

    
}
