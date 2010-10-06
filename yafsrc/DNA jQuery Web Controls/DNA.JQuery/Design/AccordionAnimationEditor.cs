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
    public class AccordionAnimationEditor:UITypeEditor
    {
        private class AnimationListBox : ListBox
        {
            public AnimationListBox()
            {
                this.BorderStyle = BorderStyle.None;
                Items.AddRange(new object[]{
                    "easeslide","bounceslide"
                });
            }
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }


        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                // This service is in charge of popping our dropdown.
                IWindowsFormsEditorService service1 = ((IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService)));

                if (service1 != null)
                {
                    AnimationListBox animateDropDown = new AnimationListBox();
                    service1.DropDownControl(animateDropDown);

                    if (animateDropDown.SelectedItem != null)
                        value = animateDropDown.SelectedItem.ToString();

                    // Close the list control after selection.
                    service1.CloseDropDown();
                }
            }


            return value;
        }
    }
}
