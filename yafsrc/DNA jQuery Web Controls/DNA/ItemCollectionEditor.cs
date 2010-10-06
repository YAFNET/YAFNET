
///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Web.UI.Design;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.ComponentModel;

namespace DNA.UI.Design
{
    /// <summary>
    /// Designer editor for Item collections.
    /// </summary>
    public class ItemCollectionEditor : CollectionEditor
    {
        private object _EditValue;

        /// <summary>
        /// Initializes a new instance of the ItemCollectionEditor class.
        /// </summary>
        /// <param name="type">The type of collection this object is to edit.</param>
        public ItemCollectionEditor(Type type)
            : base(type)
        {
            _EditValue = null;
        }

        /// <summary>
        /// Creates a new form to show the current collection.
        /// </summary>
        /// <returns>The form.</returns>
        protected override CollectionForm CreateCollectionForm()
        {
            CollectionForm form = base.CreateCollectionForm();
            InitializeForm(form);

            return form;
        }

        /// <summary>
        /// Initialize the CollectionForm. Called from CreateCollectionForm().
        /// </summary>
        /// <param name="form">The CollectionForm to initialize.</param>
        protected virtual void InitializeForm(CollectionForm form)
        {
            foreach (System.Windows.Forms.Control ctrl in form.Controls)
            {
                if (ctrl is PropertyGrid)
                {
                    InitializePropertyGrid((PropertyGrid)ctrl);
                    break;
                }
            }
        }

        /// <summary>
        /// Initialize the PropertyGrid. Called from InitializeForm().
        /// </summary>
        /// <param name="propGrid">The PropertyGrid.</param>
        protected virtual void InitializePropertyGrid(PropertyGrid propGrid)
        {
            propGrid.BackColor = System.Drawing.SystemColors.Control;
            propGrid.HelpVisible = true;
            propGrid.ToolbarVisible = true;
        }
       
        /// <summary>
        /// Edits the value of the specified object using the specified service provider and context.
        /// </summary>
        /// <param name="context">A type descriptor context that can be used to gain additional context information.</param>
        /// <param name="provider">A service provider object through which editing services may be obtained.</param>
        /// <param name="value">The object to edit the value of.</param>
        /// <returns>The new value of the object. If the value of the object hasn't changed, this should return the same object it was passed.</returns>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            _EditValue = value;
            return base.EditValue(context, provider, value);
        }

        /// <summary>
        /// Destroys the specified instance of the object.
        /// </summary>
        /// <param name="instance">The object to destroy.</param>
        protected override void DestroyInstance(object instance)
        {
            // WORKAROUND: The base class tries to remove items twice, so
            // this will prevent them from throwing an error if the object
            // is not found.
            if (_EditValue is IList)
            {
                if (((IList)_EditValue).IndexOf(instance) < 0)
                {
                    return;
                }
            }

            base.DestroyInstance(instance);
        }
    }
}
