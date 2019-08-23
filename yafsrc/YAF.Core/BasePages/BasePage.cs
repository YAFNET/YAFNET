namespace YAF.Core.BasePages
{
    using System;
    using System.Globalization;
    using System.Threading;
    using System.Web.UI.WebControls;

    public class BasePage : System.Web.UI.Page
    {
        protected override void InitializeCulture()
        {
            string language = "en-US";

           

            //Check if PostBack is caused by Language DropDownList.
            if (Request.Form["__EVENTTARGET"] != null && Request.Form["__EVENTTARGET"].Contains("Languages"))
            {
                //Set the Language.
                language = Request.Form[Request.Form["__EVENTTARGET"]];
                Session["language"] = language.ToString();

                SetLanguageUsingThread(language);
            }
            else
            {
                if (Session["language"] != null)
                {
                    language = Session["language"].ToString();
                }
                else
                {
                    //Detect User's Language.
                    if (Request.UserLanguages != null)
                    {
                        //Set the Language.
                        language = Request.UserLanguages[0];
                    }
                }
            }

            SetLanguageUsingThread(language);

            //base.InitializeCulture();
        }

       
        private void SetLanguageUsingThread(string selectedLanguage)
        {
            CultureInfo info = CultureInfo.CreateSpecificCulture(selectedLanguage);
            Thread.CurrentThread.CurrentUICulture = info;
            Thread.CurrentThread.CurrentCulture = info;
        }
    }
}
