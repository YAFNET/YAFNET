/* Code created by vzrus: http://sourceforge.net/users/vzrus  
 * for Yet Another Forum.NET http://www.yetanotherforum.net/
 * and can be used and modified without any limitations as part 
 * of Yet Another Forum.NET under every open source licence.  
 * Creation date:10/3/09
 */

namespace YAF.Pages.Admin
{
    #region Using

    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Web.Security;
    using System.Web.UI.WebControls;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Controls;
    using YAF.Core;
    using YAF.Core.Extensions;
    using YAF.Core.Helpers;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Flags;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Types.Objects;
    using YAF.Utilities;
    using YAF.Utils;
    using YAF.Utils.Helpers;

    #endregion

    /// <summary>
    /// The control generates test data for different data layers.
    /// </summary>
    public partial class test_data : AdminPage
    {
        // private string regBase = @"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$";

        #region Constants and Fields

        /// <summary>
        ///   The board create limit.
        /// </summary>
        private const int BoardCreateLimit = 100;

        /// <summary>
        ///   The category create limit.
        /// </summary>
        private const int categoryCreateLimit = 100;

        /// <summary>
        ///   The create common limit.
        /// </summary>
        private const int createCommonLimit = 9999;

        /// <summary>
        ///   The pmessage prefix.
        /// </summary>
        private const string pmessagePrefix = "pmsg-";

        /// <summary>
        ///   The random guid.
        /// </summary>
        private string randomGuid = Guid.NewGuid().ToString();

        #endregion

        #region Methods

        /// <summary>
        /// The cancel_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Cancel_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            YafBuildLink.Redirect(ForumPages.admin_test_data);
        }

        /// <summary>
        /// The categories boards options_ on selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void CategoriesBoardsOptions_OnSelectedIndexChanged([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.CategoriesBoardsList.Visible = this.CategoriesBoardsOptions.SelectedIndex == 3;
        }

        /// <summary>
        /// The create test data_ click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void CreateTestData_Click([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.Page.IsValid)
            {
                return;
            }

            if (!this.ValidateControlsValues())
            {
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine("Test Data Generator reports: ");
            sb.AppendLine("Created:");
            sb.Append(this.CreateUsers());
            sb.Append(this.CreateBoards());
            sb.Append(this.CreateCategories());
            sb.Append("; ");
            sb.AppendFormat("{0} Forums, ", this.CreateForums());
            sb.AppendFormat("{0} Topics, ", this.CreateTopics(0, 0, 0));
            sb.AppendFormat("{0} Messages, ", this.CreatePosts(0, 0, 0));
            sb.AppendFormat("{0} Private Messages, ", this.CreatePMessages());

            var mesRetStr = sb.ToString();

            this.Logger.Log(this.PageContext.PageUserID, this, mesRetStr, EventLogTypes.Information);

            this.PageContext.AddLoadMessage(mesRetStr);
            YafBuildLink.Redirect(ForumPages.admin_test_data);
        }

        /// <summary>
        /// The forums category_ on selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void ForumsCategory_OnSelectedIndexChanged([NotNull] object sender, [NotNull] EventArgs e)
        {
            var forums_category = LegacyDb.forum_listall_fromCat(
                this.PageContext.PageBoardID, this.ForumsCategory.SelectedValue.ToType<int>());
            this.ForumsParent.DataSource = forums_category;
            this.ForumsParent.DataBind();
        }

        /// <summary>
        /// The On PreRender event.
        /// </summary>
        /// <param name="e">
        /// the Event Arguments
        /// </param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            // setup jQuery and Jquery Ui Tabs.
            YafContext.Current.PageElements.RegisterJsBlock(
                "TestDataTabsJs",
                JavaScriptBlocks.BootstrapTabsLoadJs(this.TestDataTabs.ClientID, this.hidLastTab.ClientID));

            base.OnPreRender(e);
        }

        /// <summary>
        /// The p messages boards options_ on selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void PMessagesBoardsOptions_OnSelectedIndexChanged([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.PMessagesBoardsList.Visible = this.PMessagesBoardsOptions.SelectedIndex == 3;
        }

        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (this.IsPostBack)
            {
                return;
            }

            this.PageLinks.AddRoot();
            this.PageLinks.AddLink(
                this.GetText("ADMIN_ADMIN", "Administration"), YafBuildLink.GetLink(ForumPages.admin_admin));

            this.PageLinks.AddLink(this.GetText("ADMIN_TEST_DATA", "TITLE"), string.Empty);

            this.Page.Header.Title = "{0} - {1}".FormatWith(
                this.GetText("ADMIN_ADMIN", "Administration"), this.GetText("ADMIN_TEST_DATA", "TITLE"));

            this.Populate_Controls();

            const string _BoardOptionsCurrentBoardIn = "In Current Board";
            const string _BoardOptionsAllBoardsIn = "In All Boards";
            const string _BoardOptionsAllBoardsButCurrentIn = "In All But Current";
            const string _BoardOptionsAllBoardsSpecificIn = "In A Specific Board";

            this.TimeZones.DataSource = StaticDataHelper.TimeZones();

            var categories = this.GetRepository<Category>().List();

            this.ForumsCategory.DataSource = categories;
            this.TopicsCategory.DataSource = categories;
            this.PostsCategory.DataSource = categories;

            // Access Mask Lists               
            this.ForumsStartMask.DataSource = this.GetRepository<AccessMask>().GetByBoardId();
            this.ForumsAdminMask.DataSource = this.ForumsStartMask.DataSource;

            this.ForumsGroups.DataSource = this.GetRepository<Group>().List(boardId: this.PageContext.PageBoardID);

            // Board lists
            this.UsersBoardsList.DataSource = this.GetRepository<Board>().List();
            this.CategoriesBoardsList.DataSource = this.UsersBoardsList.DataSource;
            this.PMessagesBoardsList.DataSource = this.UsersBoardsList.DataSource;

            this.DataBind();

            if (this.ForumsAdminMask.Items.Count > 0)
            {
                this.ForumsAdminMask.SelectedIndex = this.ForumsAdminMask.Items.Count - 1;
            }

            if (this.ForumsStartMask.Items.Count > 1)
            {
                this.ForumsStartMask.SelectedIndex = 1;
            }

            this.TopicsCategory.ClearSelection();
            this.PostsCategory.ClearSelection();

            this.ForumsCategory.SelectedIndex = -1;

            this.TimeZones.Items.FindByValue("0").Selected = true;

            this.From.Text = this.PageContext.User.UserName;
            this.To.Text = this.PageContext.User.UserName;

            this.TopicsPriorityList.Items.Add(new ListItem("Normal", "0"));
            this.TopicsPriorityList.Items.Add(new ListItem("Sticky", "1"));
            this.TopicsPriorityList.Items.Add(new ListItem("Announcement", "2"));

            this.TopicsPriorityList.SelectedIndex = 0;

            this.UsersBoardsOptions.Items.Add(new ListItem(_BoardOptionsCurrentBoardIn, "0"));
            this.UsersBoardsOptions.Items.Add(new ListItem(_BoardOptionsAllBoardsIn, "1"));
            this.UsersBoardsOptions.Items.Add(new ListItem(_BoardOptionsAllBoardsButCurrentIn, "2"));
            this.UsersBoardsOptions.Items.Add(new ListItem(_BoardOptionsAllBoardsSpecificIn, "3"));

            this.UsersBoardsOptions.SelectedIndex = 0;

            this.CategoriesBoardsOptions.Items.Add(new ListItem(_BoardOptionsCurrentBoardIn, "0"));
            this.CategoriesBoardsOptions.Items.Add(new ListItem(_BoardOptionsAllBoardsIn, "1"));
            this.CategoriesBoardsOptions.Items.Add(new ListItem(_BoardOptionsAllBoardsButCurrentIn, "2"));
            this.CategoriesBoardsOptions.Items.Add(new ListItem(_BoardOptionsAllBoardsSpecificIn, "3"));

            this.CategoriesBoardsOptions.SelectedIndex = 0;

            this.PMessagesBoardsOptions.Items.Add(new ListItem(_BoardOptionsCurrentBoardIn, "0"));
            this.PMessagesBoardsOptions.Items.Add(new ListItem(_BoardOptionsAllBoardsIn, "1"));
            this.PMessagesBoardsOptions.Items.Add(new ListItem(_BoardOptionsAllBoardsButCurrentIn, "2"));
            this.PMessagesBoardsOptions.Items.Add(new ListItem(_BoardOptionsAllBoardsSpecificIn, "3"));

            this.PMessagesBoardsOptions.SelectedIndex = 0;
        }

        /// <summary>
        /// The posts category_ on selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void PostsCategory_OnSelectedIndexChanged([NotNull] object sender, [NotNull] EventArgs e)
        {
            var posts_category = LegacyDb.forum_listall_fromCat(
                this.PageContext.PageBoardID, this.PostsCategory.SelectedValue.ToType<int>());
            this.PostsForum.DataSource = posts_category;
            this.PostsForum.DataBind();
        }

        /// <summary>
        /// The posts forum_ on selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void PostsForum_OnSelectedIndexChanged([NotNull] object sender, [NotNull] EventArgs e)
        {
            int _forumID;

            if (!int.TryParse(this.PostsForum.SelectedValue, out _forumID))
            {
                return;
            }

            var topics = LegacyDb.topic_list(
                this.PostsForum.SelectedValue.ToType<int>(),
                this.PageContext.PageUserID,
                DateTimeHelper.SqlDbMinTime(),
                DateTime.UtcNow,
                0,
                100,
                false,
                false,
                false);

            this.PostsTopic.DataSource = topics;
            this.PostsTopic.DataBind();
        }

        /// <summary>
        /// The topics category_ on selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void TopicsCategory_OnSelectedIndexChanged([NotNull] object sender, [NotNull] EventArgs e)
        {
            var topic_forums = LegacyDb.forum_listall_fromCat(
                this.PageContext.PageBoardID, this.TopicsCategory.SelectedValue.ToType<int>());
            this.TopicsForum.DataSource = topic_forums;
            this.TopicsForum.DataBind();
        }

        /// <summary>
        /// The users boards options_ on selected index changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void UsersBoardsOptions_OnSelectedIndexChanged([NotNull] object sender, [NotNull] EventArgs e)
        {
            this.UsersBoardsList.Visible = this.UsersBoardsOptions.SelectedIndex == 3;
        }

        /// <summary>
        /// The create boards.
        /// </summary>
        /// <returns>
        /// The number of created boards.
        /// </returns>
        private string CreateBoards()
        {
            int _boardNumber;
            int _usersNumber;
            if (!int.TryParse(this.BoardNumber.Text.Trim(), out _boardNumber))
            {
                return null;
            }

            if (_boardNumber <= 0)
            {
                return null;
            }

            if (!int.TryParse(this.BoardsUsersNumber.Text.Trim(), out _usersNumber))
            {
                return null;
            }

            if (_usersNumber < 0)
            {
                return null;
            }

            if (_boardNumber > BoardCreateLimit)
            {
                _boardNumber = BoardCreateLimit;
            }

            this.BoardMembershipName.Text = null;
            this.BoardRolesName.Text = null;
            int i;
            for (i = 0; i < _boardNumber; i++)
            {
                var boardName = this.BoardPrefixTB.Text.Trim() + Guid.NewGuid();
                var curboard = this.GetRepository<Board>().Create(
                    boardName,
                    "en-US",
                    "english.xml",
                    this.BoardMembershipName.Text.Trim(),
                    this.BoardRolesName.Text.Trim(),
                    this.PageContext.User.UserName,
                    this.PageContext.User.Email,
                    this.PageContext.User.ProviderUserKey.ToString(),
                    this.PageContext.IsHostAdmin,
                    Config.CreateDistinctRoles && Config.IsAnyPortal ? "YAF " : string.Empty);

                this.CreateUsers(curboard, _usersNumber);
            }

            return string.Format("{0} Boards, {1} Users in each Board; ", i, _usersNumber);
        }

        /// <summary>
        /// Create categories from boards
        /// </summary>
        /// <param name="boardID">
        /// The boardID 
        /// </param>
        /// <returns>
        /// The create categories.
        /// </returns>
        private string CreateCategories(int boardID)
        {
            var noCategories = "0 categories";
            var excludeCurrentBoardB = false;
            var useListB = false;
            var numCategoriesInt = 0;
            if (!int.TryParse(this.BoardsCategoriesNumber.Text.Trim(), out numCategoriesInt))
            {
                return noCategories;
            }

            if (numCategoriesInt < 0)
            {
                return noCategories;
            }

            if (numCategoriesInt > categoryCreateLimit)
            {
                numCategoriesInt = categoryCreateLimit;
            }

            int _numForums;
            if (!int.TryParse(this.BoardsForumsNumber.Text.Trim(), out _numForums))
            {
                return noCategories;
            }

            if (_numForums < 0)
            {
                return noCategories;
            }

            int _numTopics;
            if (!int.TryParse(this.BoardsTopicsNumber.Text.Trim(), out _numTopics))
            {
                return noCategories;
            }

            if (_numTopics < 0)
            {
                return noCategories;
            }

            int _numMessages;
            if (!int.TryParse(this.BoardsMessagesNumber.Text.Trim(), out _numMessages))
            {
                return noCategories;
            }

            if (_numMessages < 0)
            {
                return noCategories;
            }

            return this.CreateCategoriesBase(
                boardID, 1, _numForums, _numTopics, _numMessages, numCategoriesInt, excludeCurrentBoardB, useListB);
        }

        /// <summary>
        /// Create categories from Categories
        /// </summary>
        /// <returns>
        /// The create categories.
        /// </returns>
        private string CreateCategories()
        {
            const string noCategories = "0 categories";
            var boardID = 0;

            // int categoriesLimit = 1;
            var _excludeCurrentBoard = false;

            int _numForums;
            if (!int.TryParse(this.CategoriesForumsNumber.Text.Trim(), out _numForums))
            {
                return noCategories;
            }

            if (_numForums < 0)
            {
                return noCategories;
            }

            int _numTopics;
            if (!int.TryParse(this.CategoriesTopicsNumber.Text.Trim(), out _numTopics))
            {
                return "0 Categories";
            }

            if (_numTopics < 0)
            {
                return noCategories;
            }

            int _numMessages;
            if (!int.TryParse(this.CategoriesMessagesNumber.Text.Trim(), out _numMessages))
            {
                return noCategories;
            }

            if (_numMessages < 0)
            {
                return noCategories;
            }

            int _numCategories;
            var _boardCount = 1;
            var _useList = false;
            switch (this.CategoriesBoardsOptions.SelectedIndex)
            {
                case 0:
                    boardID = YafContext.Current.PageBoardID;
                    break;
                case 1:
                    _boardCount = this.CategoriesBoardsList.Items.Count;
                    _useList = true;
                    break;
                case 2:
                    _boardCount = this.CategoriesBoardsList.Items.Count - 1;
                    _excludeCurrentBoard = true;
                    _useList = true;
                    break;
                case 3:
                    boardID = this.CategoriesBoardsList.SelectedValue.ToType<int>();
                    break;
            }

            if (!int.TryParse(this.CategoriesNumber.Text.Trim(), out _numCategories))
            {
                return noCategories;
            }

            if (_numCategories <= 0)
            {
                return noCategories;
            }

            if (_numCategories > categoryCreateLimit)
            {
                _numCategories = categoryCreateLimit;
            }

            return this.CreateCategoriesBase(
                boardID,
                _boardCount,
                _numForums,
                _numTopics,
                _numMessages,
                _numCategories,
                _excludeCurrentBoard,
                _useList);
        }

        /// <summary>
        /// The create categories base.
        /// </summary>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <param name="boardCount">
        /// The board count.
        /// </param>
        /// <param name="numForums">
        /// The num forums.
        /// </param>
        /// <param name="numTopics">
        /// The num topics.
        /// </param>
        /// <param name="numMessages">
        /// The num messages.
        /// </param>
        /// <param name="numCategories">
        /// The num categories.
        /// </param>
        /// <param name="excludeCurrentBoard">
        /// The exclude current board.
        /// </param>
        /// <param name="useList">
        /// The use list.
        /// </param>
        /// <returns>
        /// The create categories base.
        /// </returns>
        private string CreateCategoriesBase(
            int boardID,
            int boardCount,
            int numForums,
            int numTopics,
            int numMessages,
            int numCategories,
            bool excludeCurrentBoard,
            bool useList)
        {
            int ib;
            for (ib = 0; ib < boardCount; ib++)
            {
                if (useList)
                {
                    boardID = this.CategoriesBoardsList.Items[ib].Value.ToType<int>();
                }

                int i;
                if (!(excludeCurrentBoard && boardID == YafContext.Current.PageBoardID))
                {
                    for (i = 0; i < numCategories; i++)
                    {
                        var catName = this.CategoryPrefixTB.Text.Trim() + Guid.NewGuid();

                        // TODO: should return number of categories created 
                        this.GetRepository<Category>().Save(null, catName, null, 100, boardID);
                        var dt = this.GetRepository<Category>().Simplelist(0, 10000);

                        foreach (var dr in dt.Rows.Cast<DataRow>().Where(dr => dr["Name"].ToString() == catName))
                        {
                            this.CreateForums(dr["CategoryID"].ToType<int>(), null, numForums, numTopics, numMessages);
                        }

                        // We don't have last category index, so not implemented.... CreateForums( categoryID,numForums,numTopics,numMessages )
                    }
                }

                i = 0;
            }

            return "{0} Categories, ".FormatWith(ib);
        }

        /// <summary>
        /// Create forums from Forums page
        /// </summary>
        /// <returns>
        /// The create forums.
        /// </returns>
        private int CreateForums()
        {
            object parentID = null;
            int parentIDInt;
            if (int.TryParse(this.ForumsParent.Text.Trim(), out parentIDInt))
            {
                parentID = parentIDInt;
            }

            int numTopics;
            if (!int.TryParse(this.ForumsTopicsNumber.Text.Trim(), out numTopics))
            {
                return 0;
            }

            if (numTopics < 0)
            {
                return 0;
            }

            int numPosts;
            if (!int.TryParse(this.ForumsMessagesNumber.Text.Trim(), out numPosts))
            {
                return 0;
            }

            if (numPosts < 0)
            {
                return 0;
            }

            int numForums;
            if (!int.TryParse(this.ForumsNumber.Text.Trim(), out numForums))
            {
                return 0;
            }

            if (numForums <= 0)
            {
                return 0;
            }

            int categoryID;
            if (!int.TryParse(this.ForumsCategory.SelectedValue, out categoryID))
            {
                return 0;
            }

            if (numForums > createCommonLimit)
            {
                numForums = createCommonLimit;
            }

            return this.CreateForums(categoryID, parentID, numForums, numTopics, numPosts);
        }

        /// <summary>
        /// Create forums from Categories
        /// </summary>
        /// <param name="categoryID">
        /// </param>
        /// <param name="parentID">
        /// The parent ID.
        /// </param>
        /// <param name="numForums">
        /// The num Forums.
        /// </param>
        /// <param name="_topicsToCreate">
        /// Number of topics to create.
        /// </param>
        /// <param name="_messagesToCreate">
        /// Number of messages to create.
        /// </param>
        /// <returns>
        /// The create forums.
        /// </returns>
        private int CreateForums(
            int categoryID, [NotNull] object parentID, int numForums, int _topicsToCreate, int _messagesToCreate)
        {
            var countMessagesInStatistics = this.ForumsCountMessages.Text.Trim().IsNotSet();

            var isHiddenIfNoAccess = this.ForumsHideNoAccess.Text.Trim().IsNotSet();

            isHiddenIfNoAccess = true;

            // ForumsCategory.Items.FindByValue("0").Selected = true; 
            long uniqueForum = 0;
            int iforums;
            for (iforums = 0; iforums < numForums; iforums++)
            {
                long _forumID = 0;
                this.randomGuid = Guid.NewGuid().ToString();
                var accessForumList = this.GetRepository<ForumAccess>().GetForumAccessList(_forumID.ToType<int>());
                _forumID = LegacyDb.forum_save(
                    _forumID,
                    categoryID,
                    parentID,
                    this.ForumPrefixTB.Text.Trim() + this.randomGuid,
                    "Description of " + this.ForumPrefixTB.Text.Trim() + this.randomGuid,
                    100,
                    false,
                    isHiddenIfNoAccess,
                    countMessagesInStatistics,
                    false,
                    null,
                    false,
                    this.ForumsStartMask.SelectedValue,
                    null,
                    null,
                    null,
                    null,
                    false);

                if (_forumID <= 0)
                {
                    continue;
                }

                foreach (var access in accessForumList)
                {
                    this.GetRepository<ForumAccess>().Save(
                        _forumID.ToType<int>(),
                        access.GroupID,
                        access.AccessMaskID);
                }

                this.GetRepository<ForumAccess>().Save(
                    _forumID.ToType<int>(),
                    this.ForumsGroups.SelectedValue.ToType<int>(),
                    this.ForumsAdminMask.SelectedValue.ToType<int>());

                if (_topicsToCreate <= 0)
                {
                    continue;
                }

                if (uniqueForum == _forumID)
                {
                    continue;
                }

                this.CreateTopics(_forumID.ToType<int>(), _topicsToCreate, _messagesToCreate);
                uniqueForum = _forumID;
            }

            return iforums;
        }

        /// <summary>
        /// The create p messages.
        /// </summary>
        /// <returns>
        /// The number of created p messages.
        /// </returns>
        private int CreatePMessages()
        {
            var userID = this.PageContext.PageUserID;
            int numPMessages;
            if (!int.TryParse(this.PMessagesNumber.Text.Trim(), out numPMessages))
            {
                return 0;
            }

            if (numPMessages <= 0)
            {
                return 0;
            }

            var _fromUser = this.From.Text.Trim();
            var _toUser = this.To.Text.Trim();
            if (numPMessages > createCommonLimit)
            {
                numPMessages = createCommonLimit;
            }

            int i;
            for (i = 0; i < numPMessages; i++)
            {
                this.randomGuid = Guid.NewGuid().ToString();
                LegacyDb.pmessage_save(
                    LegacyDb.user_get(YafContext.Current.PageBoardID, Membership.GetUser(_fromUser).ProviderUserKey),
                    LegacyDb.user_get(YafContext.Current.PageBoardID, Membership.GetUser(_toUser).ProviderUserKey),
                    this.TopicPrefixTB.Text.Trim() + this.randomGuid,
                    "{0}{1}   {2}".FormatWith(pmessagePrefix, this.randomGuid, this.PMessageText.Text.Trim()),
                    6, 
                    -1);
            }

            if (this.MarkRead.Checked)
            {
                var userAID = LegacyDb.user_get(
                    YafContext.Current.PageBoardID, Membership.GetUser(_toUser).ProviderUserKey);
                foreach (DataRow dr in LegacyDb.pmessage_list(null, userAID, null).Rows)
                {
                    LegacyDb.pmessage_markread(dr["PMessageID"]);

                    // Clearing cache with old permissions data...
                    this.Get<IDataCache>().Remove(Constants.Cache.ActiveUserLazyData.FormatWith(userAID));
                }
            }

            return i;
        }

        /// <summary>
        /// The create posts.
        /// </summary>
        /// <param name="forumID">
        /// The forum id.
        /// </param>
        /// <param name="topicID">
        /// The topic id.
        /// </param>
        /// <param name="numMessages">
        /// The num messages.
        /// </param>
        /// <returns>
        /// The number of created posts.
        /// </returns>
        private int CreatePosts(int forumID, int topicID, int numMessages)
        {
            if (numMessages <= 0)
            {
                if (!int.TryParse(this.PostsNumber.Text.Trim(), out numMessages))
                {
                    return 0;
                }
            }

            if (numMessages <= 0)
            {
                return 0;
            }

            int categoryID;
            if (!int.TryParse(this.PostsCategory.SelectedValue, out categoryID))
            {
                return 0;
            }

            if (forumID <= 0)
            {
                if (!int.TryParse(this.PostsForum.SelectedValue, out forumID))
                {
                    return 0;
                }
            }

            if (topicID <= 0)
            {
                if (!int.TryParse(this.PostsTopic.SelectedValue, out topicID))
                {
                    return 0;
                }
            }

            // if ( numMessages > createCommonLimit ) numMessages = createCommonLimit;        
            long messageid = 0;
            int iposts;
            const int _replyTo = -1;
            for (iposts = 0; iposts < numMessages; iposts++)
            {
                this.randomGuid = Guid.NewGuid().ToString();
                LegacyDb.message_save(
                    topicID,
                    this.PageContext.PageUserID,
                    "msgd-" + this.randomGuid + "  " + this.MyMessage.Text.Trim(),
                    this.PageContext.User.UserName,
                    this.Request.GetUserRealIPAddress(),
                    null,
                    _replyTo,
                    this.GetMessageFlags(),
                    ref messageid);

                // User != null ? null : From.Text
            }

            return iposts;
        }

        /// <summary>
        /// The create topics.
        /// </summary>
        /// <param name="forumID">
        /// The forum id.
        /// </param>
        /// <param name="numTopics">
        /// The num topics.
        /// </param>
        /// <param name="_messagesToCreate">
        /// The _messages to create.
        /// </param>
        /// <returns>
        /// Number of created topics.
        /// </returns>
        private int CreateTopics(int forumID, int numTopics, int _messagesToCreate)
        {
            object _priority = 0;
            if (forumID <= 0)
            {
                _priority = this.TopicsPriorityList.SelectedValue;
            }

            if (numTopics <= 0)
            {
                if (!int.TryParse(this.TopicsNumber.Text.Trim(), out numTopics))
                {
                    return 0;
                }
            }

            if (numTopics <= 0)
            {
                return 0;
            }

            int categoryID;

            if (!int.TryParse(this.TopicsCategory.SelectedValue, out categoryID))
            {
                return 0;
            }

            if (forumID <= 0)
            {
                if (!int.TryParse(this.TopicsForum.SelectedValue, out forumID))
                {
                    return 0;
                }
            }

            if (_messagesToCreate <= 0)
            {
                if (!int.TryParse(this.TopicsMessagesNumber.Text.Trim(), out _messagesToCreate))
                {
                    return 0;
                }
            }

            if (_messagesToCreate < 0)
            {
                return 0;
            }

            // if ( numTopics > createCommonLimit ) numTopics = createCommonLimit;         
            int itopics;
            for (itopics = 0; itopics < numTopics; itopics++)
            {
                long messageid = 0;
                this.randomGuid = Guid.NewGuid().ToString();
                object pollID = null;

                var topicID = LegacyDb.topic_save(
                    forumID,
                    this.TopicPrefixTB.Text.Trim() + this.randomGuid,
                    string.Empty,
                    string.Empty,
                    "{0}{1}descr".FormatWith(this.TopicPrefixTB.Text.Trim(), this.randomGuid),
                    this.MessageContentPrefixTB.Text.Trim() + this.randomGuid,
                    this.PageContext.PageUserID,
                    _priority,
                    this.PageContext.User.UserName,
                    this.Request.GetUserRealIPAddress(),
                    DateTime.UtcNow,
                    string.Empty,
                    this.GetMessageFlags(),
                    ref messageid);

                if (this.PollCreate.Checked)
                {
                    // vzrus: always one in current code - a number of  questions
                    const int questionsTotal = 1;

                    var pollList = new List<PollSaveList>(questionsTotal);

                    var rawChoices = new string[3,2];

                    rawChoices[0, 0] = "ans1-" + this.randomGuid;
                    rawChoices[0, 1] = "ans2-" + this.randomGuid;
                    rawChoices[1, 0] = null;
                    rawChoices[1, 1] = null;
                    rawChoices[2, 0] = null;
                    rawChoices[2, 1] = null;

                    object datePollExpire = null;
                    pollList.Add(
                        new PollSaveList(
                            "quest-" + this.randomGuid,
                            rawChoices,
                            (DateTime?)datePollExpire,
                            this.PageContext.PageUserID,
                            (int?)topicID,
                            null,
                            null,
                            null,
                            null,
                            null,
                            false,
                            false,
                            false,
                            false,
                            false));
                    pollID = LegacyDb.poll_save(pollList);
                }

                if (_messagesToCreate > 0)
                {
                    this.CreatePosts(forumID, topicID.ToType<int>(), _messagesToCreate);
                }

                // User != null ? null : From.Text
            }

            return itopics;
        }

        /// <summary>
        /// From Users Tab
        /// </summary>
        /// <returns>
        /// The create users.
        /// </returns>
        private string CreateUsers()
        {
            var boardID = 0;
            int _users_Number;
            const int _outCounter = 0;
            var _countLimit = 1;
            var _excludeCurrentBoard = false;

            if (!int.TryParse(this.UsersNumber.Text.Trim(), out _users_Number))
            {
                return null;
            }

            if (_users_Number <= 0)
            {
                return null;
            }

            switch (this.UsersBoardsOptions.SelectedIndex)
            {
                case 0:
                    boardID = YafContext.Current.PageBoardID;
                    break;
                case 1:
                    _countLimit = this.UsersBoardsList.Items.Count;
                    break;
                case 2:
                    _countLimit = this.UsersBoardsList.Items.Count - 1;
                    _excludeCurrentBoard = true;
                    break;
                case 3:
                    boardID = this.UsersBoardsList.SelectedValue.ToType<int>();
                    break;
            }

            return this.CreateUsers(0, _users_Number, _outCounter, _countLimit, _excludeCurrentBoard);
        }

        /// <summary>
        /// Create Users From Board
        /// </summary>
        /// <param name="boardID">
        /// </param>
        /// <param name="_users_Number">
        /// </param>
        private void CreateUsers(int boardID, int _users_Number)
        {
            var _outCounter = 0;
            var _countLimit = 1;
            var _excludeCurrentBoard = false;

            this.CreateUsers(boardID, _users_Number, _outCounter, _countLimit, _excludeCurrentBoard);
            return;
        }

        /// <summary>
        /// The create users.
        /// </summary>
        /// <param name="boardID">
        /// The board id.
        /// </param>
        /// <param name="_users_Number">
        /// The _users_ number.
        /// </param>
        /// <param name="_outCounter">
        /// The _out counter.
        /// </param>
        /// <param name="_countLimit">
        /// The _count limit.
        /// </param>
        /// <param name="_excludeCurrentBoard">
        /// The _exclude current board.
        /// </param>
        /// <returns>
        /// The string with number of created users.
        /// </returns>
        private string CreateUsers(
            int boardID, int _users_Number, int _outCounter, int _countLimit, bool _excludeCurrentBoard)
        {
            int iboards;

            // if ( _users_Number > createCommonLimit ) _users_Number = createCommonLimit;
            for (iboards = 0; iboards < _countLimit; iboards++)
            {
                boardID = this.UsersBoardsList.Items[iboards].Value.ToType<int>();
                int i;
                for (i = 0; i < this.UsersNumber.Text.Trim().ToType<int>(); i++)
                {
                    this.randomGuid = Guid.NewGuid().ToString();
                    var newEmail = this.UserPrefixTB.Text.Trim() + this.randomGuid + "@test.info";
                    var newUsername = this.UserPrefixTB.Text.Trim() + this.randomGuid;

                    if (UserMembershipHelper.UserExists(newUsername, newEmail))
                    {
                        continue;
                    }

                    var hashinput = DateTime.UtcNow + newEmail + Security.CreatePassword(20);
                    var hash = FormsAuthentication.HashPasswordForStoringInConfigFile(hashinput, "md5");

                    MembershipCreateStatus status;
                    var user = this.Get<MembershipProvider>().CreateUser(
                        newUsername,
                        this.Password.Text.Trim(),
                        newEmail,
                        this.Question.Text.Trim(),
                        this.Answer.Text.Trim(),
                        !this.Get<YafBoardSettings>().EmailVerification,
                        null,
                        out status);

                    if (status != MembershipCreateStatus.Success)
                    {
                        continue;
                    }

                    // setup inital roles (if any) for this user
                    RoleMembershipHelper.SetupUserRoles(boardID, newUsername);

                    // create the user in the YAF DB as well as sync roles...
                    var userID = RoleMembershipHelper.CreateForumUser(user, boardID);

                    // create profile
                    var userProfile = YafUserProfile.GetProfile(newUsername);

                    // setup their inital profile information
                    userProfile.Location = this.Location.Text.Trim();
                    userProfile.Homepage = this.HomePage.Text.Trim();
                    userProfile.Save();

                    // save the time zone...
                    if (
                        !(this.UsersBoardsList.Items[iboards].Value.ToType<int>() == YafContext.Current.PageBoardID &&
                          _excludeCurrentBoard))
                    {
                        LegacyDb.user_save(
                            LegacyDb.user_get(boardID, user.ProviderUserKey),
                            boardID,
                            null,
                            null,
                            null,
                            this.TimeZones.SelectedValue.ToType<int>(),
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null);
                        _outCounter++;
                    }
                }
            }

            return _outCounter + " Users in " + iboards + " Board(s); ";
        }

        /// <summary>
        /// The get message flags.
        /// </summary>
        /// <returns>
        /// The method returns message flags.
        /// </returns>
        private int GetMessageFlags()
        {
            var editorModule = this.PageContext.Get<IModuleManager<ForumEditor>>().GetBy(this.Get<YafBoardSettings>().ForumEditor);

            var topicFlags = new MessageFlags
                {
                    IsHtml = editorModule.UsesHTML,
                    IsBBCode = editorModule.UsesBBCode,
                    IsPersistent = false,
                    IsApproved = this.PageContext.IsAdmin
                };

            // Bypass Approval if Admin or Moderator.
            return topicFlags.BitValue;
        }

        /// <summary>
        /// The populate_ controls.
        /// </summary>
        private void Populate_Controls()
        {
        }

        /// <summary>
        /// The validate controls values.
        /// </summary>
        /// <returns>
        /// The method returns true if all controls values are valid.
        /// </returns>
        private bool ValidateControlsValues()
        {
            if (Membership.GetUser(this.From.Text.Trim()) == null)
            {
                this.PageContext.AddLoadMessage("You should enter valid 'from' user name.");
                return false;
            }

            if (Membership.GetUser(this.To.Text.Trim()) == null)
            {
                this.PageContext.AddLoadMessage("You should enter valid 'to' user name.");
                return false;
            }

            if (!ValidationHelper.IsValidInt(this.PMessagesNumber.Text.Trim()))
            {
                this.PageContext.AddLoadMessage("You should enter integer value for pmessage number.");
                return false;
            }

            if (!ValidationHelper.IsValidInt(this.UsersNumber.Text.Trim()))
            {
                this.PageContext.AddLoadMessage("You should enter integer value for users.");
                return false;
            }

            if (!ValidationHelper.IsValidInt(this.CategoriesNumber.Text.Trim()))
            {
                this.PageContext.AddLoadMessage("You should enter integer value for categories.");
                return false;
            }

            if (!ValidationHelper.IsValidInt(this.BoardNumber.Text.Trim()))
            {
                this.PageContext.AddLoadMessage("You should enter integer value for boards.");
                return false;
            }

            if (!ValidationHelper.IsValidInt(this.ForumsNumber.Text.Trim()))
            {
                this.PageContext.AddLoadMessage("You should enter integer value for forums.");
                return false;
            }

            if (!ValidationHelper.IsValidInt(this.TopicsNumber.Text.Trim()))
            {
                this.PageContext.AddLoadMessage("You should enter  integer value for topics.");
                return false;
            }

            if (!ValidationHelper.IsValidInt(this.PostsNumber.Text.Trim()))
            {
                this.PageContext.AddLoadMessage("You should enter integer value for generated posts .");
                return false;
            }

            // **************************
            if (!ValidationHelper.IsValidInt(this.BoardsTopicsNumber.Text.Trim()))
            {
                this.PageContext.AddLoadMessage("You should enter integer value for topics generated messages .");
                return false;
            }

            if (!ValidationHelper.IsValidInt(this.BoardsForumsNumber.Text.Trim()))
            {
                this.PageContext.AddLoadMessage("You should enter integer value for forums generated messages .");
                return false;
            }

            if (!ValidationHelper.IsValidInt(this.BoardsCategoriesNumber.Text.Trim()))
            {
                this.PageContext.AddLoadMessage("You should enter integer value for categories generated messages .");
                return false;
            }

            if (!ValidationHelper.IsValidInt(this.BoardsMessagesNumber.Text.Trim()))
            {
                this.PageContext.AddLoadMessage("You should enter integer value for boards generated messages .");
                return false;
            }

            // ****************************
            if (!ValidationHelper.IsValidInt(this.CategoriesTopicsNumber.Text.Trim()))
            {
                this.PageContext.AddLoadMessage("You should enter integer value for forums generated messages .");
                return false;
            }

            if (!ValidationHelper.IsValidInt(this.CategoriesForumsNumber.Text.Trim()))
            {
                this.PageContext.AddLoadMessage("You should enter integer value for categories generated messages .");
                return false;
            }

            if (!ValidationHelper.IsValidInt(this.CategoriesMessagesNumber.Text.Trim()))
            {
                this.PageContext.AddLoadMessage("You should enter integer value for boards generated messages .");
                return false;
            }

            // *************************
            if (!ValidationHelper.IsValidInt(this.ForumsTopicsNumber.Text.Trim()))
            {
                this.PageContext.AddLoadMessage("You should enter integer value for categories generated messages .");
                return false;
            }

            if (!ValidationHelper.IsValidInt(this.ForumsMessagesNumber.Text.Trim()))
            {
                this.PageContext.AddLoadMessage("You should enter integer value for boards generated messages .");
                return false;
            }

            // **********************************                
            if (!ValidationHelper.IsValidInt(this.TopicsMessagesNumber.Text.Trim()))
            {
                this.PageContext.AddLoadMessage("You should enter integer value for boards generated messages .");
                return false;
            }

            if (!ValidationHelper.IsValidInt(this.BoardsUsersNumber.Text.Trim()))
            {
                this.PageContext.AddLoadMessage("You should enter integer value for users generated with boards.");
                return false;
            }

            return true;
        }

        #endregion
    }
}