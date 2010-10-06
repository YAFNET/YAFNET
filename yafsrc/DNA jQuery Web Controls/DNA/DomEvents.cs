
/// Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNA.UI
{
    public enum DomEvents
    {
        /// <summary>
        /// Fires when the user clicks the left mouse button on the object.
        /// </summary>
        Click,
        /// <summary>
        /// Fires when the user aborts the download of an image.
        /// </summary>
        Abort,
        /// <summary>
        /// Fires when the object is set as the active element.
        /// </summary>
        Active,
        /// <summary>
        /// Fires on the object immediately after its associated document prints or previews for printing.
        /// </summary>
        AfterPrint,
        /// <summary>
        /// Fires on a databound object after successfully updating the associated data in the data source object.
        /// </summary>
        AfterUpdate,
        /// <summary>
        /// Fires immediately before the object is set as the active element.
        /// </summary>
        BeforeActive,
        /// <summary>
        /// Fires on the source object before the selection is copied to the system clipboard.
        /// </summary>
        BeforeCopy,
        /// <summary>
        /// Fires on the source object before the selection is deleted from the document.
        /// </summary>
        BeforeCut,
        /// <summary>
        /// Fires immediately before the activeElement is changed from the current object to another object in the parent document.
        /// </summary>
        BeforeDeactivate,
        /// <summary>
        /// Fires before an object contained in an editable element enters a UI-activated state or when an editable container object is control selected.
        /// </summary>
        BeforeEditFocus,
        /// <summary>
        /// Fires on the target object before the selection is pasted from the system clipboard to the document.
        /// </summary>
        BeforePaste,
        /// <summary>
        /// Fires on the object before its associated document prints or previews for printing.
        /// </summary>
        BeforePrint,
        /// <summary>
        /// Fires prior to a page being unloaded.
        /// </summary>
        BeforeUnload,
        /// <summary>
        /// Fires on a databound object before updating the associated data in the data source object.
        /// </summary>
        BeforeUpdate,
        /// <summary>
        /// Fires when the object loses the input focus.
        /// </summary>
        Blur,
        /// <summary>
        /// Fires when the behavior property of the marquee object is set to "alternate" and the contents of the marquee reach one side of the window.
        /// </summary>
        Bounce,
        /// <summary>
        /// Fires when data changes in the data provider.
        /// </summary>
        CellChange,
        /// <summary>
        /// Fires when the user clicks the right mouse button in the client area, opening the context menu.
        /// </summary>
        ContextMenu,
        /// <summary>
        /// Fires when the user is about to make a control selection of the object.
        /// </summary>
        ContorlSelect,
        /// <summary>
        /// Fires on the source element when the user copies the object or selection, adding it to the system clipboard.
        /// </summary>
        Copy,
        /// <summary>
        /// Fires on the source element when the object or selection is removed from the document and added to the system clipboard.
        /// </summary>
        Cut,
        /// <summary>
        /// Fires when the contents of the object or selection have changed.
        /// </summary>
        Change,
        /// <summary>
        /// Fires when the user double-clicks the object.
        /// </summary>
        DblClick,
        /// <summary>
        /// Fires periodically as data arrives from data source objects that asynchronously transmit their data.
        /// </summary>
        DataAvailable,
        /// <summary>
        /// Fires when the data set exposed by a data source object changes.
        /// </summary>
        DateSetChanged,
        /// <summary>
        /// Fires when the activeElement is changed from the current object to another object in the parent document.
        /// </summary>
        Deactivate,
        /// <summary>
        /// Fires on the source object continuously during a drag operation.
        /// </summary>
        Drag,
        /// <summary>
        /// Fires on the source object when the user releases the mouse at the close of a drag operation.
        /// </summary>
        DragEnd,
        /// <summary>
        /// Fires on the target element when the user drags the object to a valid drop target.
        /// </summary>
        DragEnter,
        /// <summary>
        /// Fires on the target object when the user moves the mouse out of a valid drop target during a drag operation.
        /// </summary>
        DragLeave,
        /// <summary>
        /// Fires on the target element continuously while the user drags the object over a valid drop target.
        /// </summary>
        DragOver,
        /// <summary>
        /// Fires on the source object when the user starts to drag a text selection or selected object.
        /// </summary>
        DragStart,
        /// <summary>
        /// Fires on the target object when the mouse button is released during a drag-and-drop operation.
        /// </summary>
        Drop,
        /// <summary>
        /// Fires on a databound object when an error occurs while updating the associated data in the data source object.
        /// </summary>
        ErrorUpdate,
        /// <summary>
        /// Raised when there is an error that prevents the completion of the cross-domain request.
        /// </summary>
        Error,
        /// <summary>
        /// Fires when a visual filter changes state or completes a transition.
        /// </summary>
        FilterChange,
        /// <summary>
        /// Fires when marquee looping is complete.
        /// </summary>
        Finish,
        /// <summary>
        /// Fires when the object receives focus.
        /// </summary>
        Focus,
        /// <summary>
        /// Fires for an element just prior to setting focus on that element.
        /// </summary>
        FocusIn,
        /// <summary>
        /// Fires for the current element with focus immediately after moving focus to another element.
        /// </summary>
        FocusOut,
        /// <summary>
        /// Raised when there are changes to the portion of a URL that follows the number sign (#).
        /// </summary>
        HashChange,
        /// <summary>
        /// Fires when the user presses the F1 key while the browser is the active window.
        /// </summary>
        Help,
        /// <summary>
        /// Fires when the user presses a key.
        /// </summary>
        KeyDown,
        /// <summary>
        /// Fires when the user presses an alphanumeric key.
        /// </summary>
        KeyPress,
        /// <summary>
        /// Fires when the user releases a key.
        /// </summary>
        KeyUp,
        /// <summary>
        /// Raised when the object has been completely received from the server.
        /// </summary>
        Load,
        /// <summary>
        /// Fires when the print or print preview layout process finishes filling the current LayoutRect object with content from the source document.
        /// </summary>
        LayoutComplete,
        /// <summary>
        /// Fires when the object loses the mouse capture.
        /// </summary>
        LoseCapture,
        /// <summary>
        /// Fires when the user sends a cross-document message with postMessage.
        /// </summary>
        Message,
        /// <summary>
        /// Fires when the user clicks the object with either mouse button.
        /// </summary>
        MouseDown,
        /// <summary>
        /// Fires when the user releases a mouse button while the mouse is over the object.
        /// </summary>
        MouseUp,
        /// <summary>
        /// Fires when the user moves the mouse pointer into the object.
        /// </summary>
        MouseOver,
        /// <summary>
        /// Fires when the user moves the mouse over the object.
        /// </summary>
        MouseMove,
        /// <summary>
        /// Fires when the user moves the mouse pointer outside the boundaries of the object.
        /// </summary>
        MouseOut,
        /// <summary>
        /// Fires when the user moves the mouse pointer outside the boundaries of the object.
        /// </summary>
        MouseLeave,
        /// <summary>
        /// Fires when the wheel button is rotated.
        /// </summary>
        MouseWheel,
        /// <summary>
        /// Fires when the object moves.
        /// </summary>
        Move,
        /// <summary>
        /// Fires when the object stops moving.
        /// </summary>
        MoveEnd,
        /// <summary>
        /// Fires when the object starts to move.
        /// </summary>
        MoveStart,
        /// <summary>
        /// Raised when Windows Internet Explorer is working offline.(IE Only)
        /// </summary>
        Offline,
        /// <summary>
        /// Raised when Internet Explorer is working online.(IE Only)
        /// </summary>
        Online,
        Page,
        /// <summary>
        /// Fires on the target object when the user pastes data, transferring the data from the system clipboard to the document.
        /// </summary>
        Paste,
        /// <summary>
        /// Raised when the browser starts receiving data from the server.
        /// </summary>
        Progress,
        /// <summary>
        /// Fires when a property changes on the object.
        /// </summary>
        ProgressChange,
        /// <summary>
        /// Sets or retrieves the event handler for asynchronous requests.
        /// </summary>
        ReadStateChange,
        /// <summary>
        /// Fires when the user resets a form.
        /// </summary>
        Reset,
        /// <summary>
        /// Fires when the size of the object is about to change.
        /// </summary>
        Resize,
        /// <summary>
        /// Fires when the user finishes changing the dimensions of the object in a control selection.
        /// </summary>
        ResizeEnd,
        /// <summary>
        /// Fires when the user begins to change the dimensions of the object in a control selection.
        /// </summary>
        ResizeStart,
        /// <summary>
        /// Fires to indicate that the current row has changed in the data source and new data values are available on the object.
        /// </summary>
        RowEnter,
        /// <summary>
        /// Fires just before the data source control changes the current row in the object.
        /// </summary>
        RowExit,
        /// <summary>
        /// Fires when rows are about to be deleted from the recordset.
        /// </summary>
        RowsDelete,
        /// <summary>
        /// Fires just after new rows are inserted in the current recordset.
        /// </summary>
        RowsInstered,
        /// <summary>
        /// Fires when the user repositions the scroll box in the scroll bar on the object.
        /// </summary>
        Scroll,
        /// <summary>
        /// Fires when the current selection changes.
        /// </summary>
        Select,
        /// <summary>
        /// Fires when the selection state of a document changes.
        /// </summary>
        SelectionChange,
        /// <summary>
        /// Fires when the object is being selected.
        /// </summary>
        SelectStart,
        /// <summary>
        /// Fires at the beginning of every loop of the marquee object.
        /// </summary>
        Start,
        /// <summary>
        /// Fires when the user clicks the Stop button or leaves the Web page.
        /// </summary>
        Stop,
        /// <summary>
        /// Fires when a DOM Storage area is updated.
        /// </summary>
        Storage,
        /// <summary>
        /// Fires when a local DOM Storage area is written to disk.
        /// </summary>
        StorageCommit,
        /// <summary>
        /// Fires when a FORM is about to be submitted.
        /// </summary>
        Submit,
        /// <summary>
        /// Fires immediately before the object is unloaded.
        /// </summary>
        Unload,
        /// <summary>
        /// Raised when there is an error that prevents the completion of the request.
        /// </summary>
        TimeOut,
    }
}
