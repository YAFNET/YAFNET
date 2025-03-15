namespace YAF.Pages;

using YAF.Core.Extensions;

/// <summary>
/// The privacy model.
/// </summary>
public class PrivacyModel : ForumPage
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "PrivacyModel" /> class.
    /// </summary>
    public PrivacyModel()
        : base("RULES", ForumPages.Privacy)
    {
    }

    /// <summary>
    ///   Gets a value indicating whether IsProtected.
    /// </summary>
    public override bool IsProtected => false;

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        // current page label (no link)
        this.PageBoardContext.PageLinks.AddLink(this.GetText("COMMON", "PRIVACY_POLICY"));
    }

    /// <summary>
    /// The on get.
    /// </summary>
    public IActionResult OnGet()
    {
        return this.Page();
    }

    /// <summary>
    /// The on post.
    /// </summary>
    public IActionResult OnPost()
    {
        // Go to the Register Page
        return this.Get<ILinkBuilder>().Redirect(ForumPages.Account_Register, true);
    }
}