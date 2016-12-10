using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using YAF.Types;
using YAF.Types.Attributes;
using YAF.Types.Constants;
using YAF.Utils;
using YAF.Utils.Helpers;

namespace YAF.Modules
{
    using YAF.Types.Extensions;

    /// <summary>
    ///     Generates a canonical meta tag to fight the dreaded duplicate content SEO warning
    /// </summary>
    [YafModule("Canonical Meta Tag Module", "BonzoFestoon", 1)]
    public class CanonicalMetaTagModule : SimpleBaseForumModule
    {
        public override void InitAfterPage()
        {
            this.CurrentForumPage.PreRender += this.CurrentForumPage_PreRender;
        }

        /// <summary>
        ///     Handles the PreRender event of the ForumPage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void CurrentForumPage_PreRender([NotNull] object sender, [NotNull] EventArgs e)
        {
            const string topicLinkParams = "t={0}";

            var head = this.ForumControl.Page.Header
                       ?? this.CurrentForumPage.FindControlRecursiveBothAs<HtmlHead>("YafHead");

            if (head == null)
            {
                return;
            }

            // in cases where we are not going to index, but follow, we will not add a canonical tag.
            string tag;

            if (this.ForumPageType == ForumPages.posts)
            {
                var topicId = this.PageContext.PageTopicID;
                var topicUrl = YafBuildLink.GetLink(ForumPages.posts, true, topicLinkParams, topicId);

                tag = "<link rel=\"canonical\" href=\"{0}\" />".FormatWith(topicUrl);
            }
            else
            {
                // there is not much SEO value to having lists indexed
                // because they change as soon as some adds a new topic
                // or post so don't index them, but follow the links
                tag = "<meta name=\"robots\" content=\"noindex,follow\" />";
            }

            if (tag.IsSet())
            {
                head.Controls.Add(new LiteralControl(tag));
            }
        }
    }
}