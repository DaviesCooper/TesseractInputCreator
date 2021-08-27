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
    }

    
}
