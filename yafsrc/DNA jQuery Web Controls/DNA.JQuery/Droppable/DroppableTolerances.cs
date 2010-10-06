///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
namespace DNA.UI.JQuery
{
    public enum DroppableTolerances
    {
        /// <summary>
        /// draggable overlaps the droppable at least 50%
        /// </summary>
        Intersect=1,
        /// <summary>
        /// mouse pointer overlaps the droppable
        /// </summary>
        Pointer=2,
        /// <summary>
        /// draggable overlaps the droppable entirely
        /// </summary>
        Fit=3,
        /// <summary>
        /// draggable overlaps the droppable any amount
        /// </summary>
        Touch=4
    }
}
