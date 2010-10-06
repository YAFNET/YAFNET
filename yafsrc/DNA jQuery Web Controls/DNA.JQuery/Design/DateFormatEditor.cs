///  Copyright (c) 2009 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html
using System.Web;
using System.Security.Permissions;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Web.UI.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace DNA.UI.JQuery.Design
{
    public class DateFormatEditor : UITypeEditor
    {
        private class DateFormatListBox : ListBox
        {
            public DateFormatListBox()
            {
                this.BorderStyle = BorderStyle.None;
                Items.AddRange(new object[]{
                 "Default - MM/dd/yyyy | MM/dd/yyyy",
                 "ISO 8601 - yy-mm-dd | yyyy-MM-dd",
                 "Short - d MMM yy | d MMM, yy",
                 "Medium - d MMMM, yy | d MMMM, yy",
                 "Full - dddd, d MMMM, yyyy | dddd, d MMMM, yyyy"
                });
            }
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override bool IsDropDownResizable
        {
            get
            {
                return true;
            }
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                // This service is in charge of popping our dropdown.
                IWindowsFormsEditorService service1 = ((IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService)));

                if (service1 != null)
                {
                    DateFormatListBox formatDropDown = new DateFormatListBox();
                    // Drop the list control.

                    service1.DropDownControl(formatDropDown);
                    
                    if (formatDropDown.SelectedItem != null)
                    {
                        string[] values = formatDropDown.SelectedItem.ToString().Split(new char[] { '|' });
                        value = values[1].Trim();
                    }

                    // Close the list control after selection.
                    service1.CloseDropDown();
                }
            }


            return value;
        }

    }
}
