/* Code created by vzrus: http://sourceforge.net/users/vzrus  
 * for Yet Another Forum.NET http://www.yetanotherforum.net/
 * and can be used and modified without any limitations as part 
 * of Yet Another Forum.NET under every open source licence.  
 * Creation date:10/3/09
 */

namespace YAF.Pages.Admin
{
  using System;
  using System.Data;
  using System.Web.Security;
  using System.Web.UI.WebControls;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Data;
  using YAF.Classes.Utils;
  using YAF.Editors;

  /// <summary>
  /// 	The control generates test data for different data layers.
  /// </summary>
  public partial class test_data : AdminPage
  {
    #region Fields

    // private string regBase = @"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$";
    /// <summary>
    /// The board create limit.
    /// </summary>
    private int boardCreateLimit = 100;

    /// <summary>
    /// The board object stats.
    /// </summary>
    private DataRow boardObjectStats = DB.board_poststats(YafContext.Current.PageBoardID);

    /// <summary>
    /// The board prefix.
    /// </summary>
    private string boardPrefix = "board-";

    /// <summary>
    /// The category create limit.
    /// </summary>
    private int categoryCreateLimit = 100;

    /// <summary>
    /// The category prefix.
    /// </summary>
    private string categoryPrefix = "cat-";

    /// <summary>
    /// The create common limit.
    /// </summary>
    private int createCommonLimit = 9999;

    /// <summary>
    /// The forum prefix.
    /// </summary>
    private string forumPrefix = "forum-";

    /// <summary>
    /// The message prefix.
    /// </summary>
    private string messagePrefix = "msg-";

    /// <summary>
    /// The pmessage prefix.
    /// </summary>
    private string pmessagePrefix = "pmsg-";

    /// <summary>
    /// The random guid.
    /// </summary>
    private string randomGuid = Guid.NewGuid().ToString();

    /// <summary>
    /// The topic prefix.
    /// </summary>
    private string topicPrefix = "topic-";

    /// <summary>
    /// The user prefix.
    /// </summary>
    private string userPrefix = "usr-";

    #endregion

    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load(object sender, EventArgs e)
    {
      if (!IsPostBack)
      {
        this.PageLinks.AddLink(PageContext.BoardSettings.Name, YafBuildLink.GetLink(ForumPages.forum));
        this.PageLinks.AddLink("Administration", YafBuildLink.GetLink(ForumPages.admin_admin));
        this.PageLinks.AddLink("Users", string.Empty);

        Populate_Controls();

        string _boardOptionsCurrentBoardIn = "In Current Board";
        string _boardOptionsAllBoardsIn = "In All Boards";
        string _boardOptionsAllBoardsButCurrentIn = "In All But Current";
        string _boardOptionsAllBoardsSpecificIn = "In A Specific Board";

        TimeZones.DataSource = StaticDataHelper.TimeZones();

        DataTable categories = DB.category_list(PageContext.PageBoardID, null);

        ForumsCategory.DataSource = categories;
        TopicsCategory.DataSource = categories;
        PostsCategory.DataSource = categories;

        // Access Mask Lists               
        ForumsStartMask.DataSource = DB.accessmask_list(PageContext.PageBoardID, null);
        ForumsAdminMask.DataSource = ForumsStartMask.DataSource;
        ForumsGroups.DataSource = DB.group_list(PageContext.PageBoardID, null);

        // Board lists
        UsersBoardsList.DataSource = DB.board_list(null);
        CategoriesBoardsList.DataSource = UsersBoardsList.DataSource;
        PMessagesBoardsList.DataSource = UsersBoardsList.DataSource;


        DataBind();

        ForumsAdminMask.SelectedIndex = 0;

        if (ForumsStartMask.Items.Count > 1)
        {
          ForumsStartMask.SelectedIndex = 1;
        }

        TopicsCategory.ClearSelection();
        PostsCategory.ClearSelection();

        ForumsCategory.SelectedIndex = -1;

        TimeZones.Items.FindByValue("0").Selected = true;

        From.Text = this.PageContext.User.UserName;
        To.Text = this.PageContext.User.UserName;

        TopicsPriorityList.Items.Add(new ListItem("Normal", "0"));
        TopicsPriorityList.Items.Add(new ListItem("Sticky", "1"));
        TopicsPriorityList.Items.Add(new ListItem("Announcement", "2"));

        TopicsPriorityList.SelectedIndex = 0;

        UsersBoardsOptions.Items.Add(new ListItem(_boardOptionsCurrentBoardIn, "0"));
        UsersBoardsOptions.Items.Add(new ListItem(_boardOptionsAllBoardsIn, "1"));
        UsersBoardsOptions.Items.Add(new ListItem(_boardOptionsAllBoardsButCurrentIn, "2"));
        UsersBoardsOptions.Items.Add(new ListItem(_boardOptionsAllBoardsSpecificIn, "3"));

        UsersBoardsOptions.SelectedIndex = 0;


        CategoriesBoardsOptions.Items.Add(new ListItem(_boardOptionsCurrentBoardIn, "0"));
        CategoriesBoardsOptions.Items.Add(new ListItem(_boardOptionsAllBoardsIn, "1"));
        CategoriesBoardsOptions.Items.Add(new ListItem(_boardOptionsAllBoardsButCurrentIn, "2"));
        CategoriesBoardsOptions.Items.Add(new ListItem(_boardOptionsAllBoardsSpecificIn, "3"));

        CategoriesBoardsOptions.SelectedIndex = 0;


        PMessagesBoardsOptions.Items.Add(new ListItem(_boardOptionsCurrentBoardIn, "0"));
        PMessagesBoardsOptions.Items.Add(new ListItem(_boardOptionsAllBoardsIn, "1"));
        PMessagesBoardsOptions.Items.Add(new ListItem(_boardOptionsAllBoardsButCurrentIn, "2"));
        PMessagesBoardsOptions.Items.Add(new ListItem(_boardOptionsAllBoardsSpecificIn, "3"));

        PMessagesBoardsOptions.SelectedIndex = 0;
      }
    }

    /// <summary>
    /// The populate_ controls.
    /// </summary>
    private void Populate_Controls()
    {
    }

    #region Events

    /// <summary>
    /// The cancel_ click.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Cancel_Click(object sender, EventArgs e)
    {
      YafBuildLink.Redirect(ForumPages.admin_test_data);
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
    protected void CreateTestData_Click(object sender, EventArgs e)
    {
      if (Page.IsValid)
      {
        if (Membership.GetUser(From.Text.Trim()) == null)
        {
          PageContext.AddLoadMessage("You should enter valid 'from' user name.");
          return;
        }

        if (Membership.GetUser(To.Text.Trim()) == null)
        {
          PageContext.AddLoadMessage("You should enter valid 'to' user name.");
          return;
        }

        if (!ValidationHelper.IsValidInt(PMessagesNumber.Text.Trim()))
        {
          PageContext.AddLoadMessage("You should enter integer value for pmessage number.");
          return;
        }

        if (!ValidationHelper.IsValidInt(UsersNumber.Text.Trim()))
        {
          PageContext.AddLoadMessage("You should enter integer value for users.");
          return;
        }

        if (!ValidationHelper.IsValidInt(CategoriesNumber.Text.Trim()))
        {
          PageContext.AddLoadMessage("You should enter integer value for categories.");
          return;
        }

        if (!ValidationHelper.IsValidInt(BoardNumber.Text.Trim()))
        {
          PageContext.AddLoadMessage("You should enter integer value for boards.");
          return;
        }

        if (!ValidationHelper.IsValidInt(ForumsNumber.Text.Trim()))
        {
          PageContext.AddLoadMessage("You should enter integer value for forums.");
          return;
        }

        if (!ValidationHelper.IsValidInt(TopicsNumber.Text.Trim()))
        {
          PageContext.AddLoadMessage("You should enter  integer value for topics.");
          return;
        }

        if (!ValidationHelper.IsValidInt(PostsNumber.Text.Trim()))
        {
          PageContext.AddLoadMessage("You should enter integer value for generated posts .");
          return;
        }

        // **************************
        if (!ValidationHelper.IsValidInt(BoardsTopicsNumber.Text.Trim()))
        {
          PageContext.AddLoadMessage("You should enter integer value for topics generated messages .");
          return;
        }

        if (!ValidationHelper.IsValidInt(BoardsForumsNumber.Text.Trim()))
        {
          PageContext.AddLoadMessage("You should enter integer value for forums generated messages .");
          return;
        }

        if (!ValidationHelper.IsValidInt(BoardsCategoriesNumber.Text.Trim()))
        {
          PageContext.AddLoadMessage("You should enter integer value for categories generated messages .");
          return;
        }

        if (!ValidationHelper.IsValidInt(BoardsMessagesNumber.Text.Trim()))
        {
          PageContext.AddLoadMessage("You should enter integer value for boards generated messages .");
          return;
        }

        // ****************************
        if (!ValidationHelper.IsValidInt(CategoriesTopicsNumber.Text.Trim()))
        {
          PageContext.AddLoadMessage("You should enter integer value for forums generated messages .");
          return;
        }

        if (!ValidationHelper.IsValidInt(CategoriesForumsNumber.Text.Trim()))
        {
          PageContext.AddLoadMessage("You should enter integer value for categories generated messages .");
          return;
        }

        if (!ValidationHelper.IsValidInt(CategoriesMessagesNumber.Text.Trim()))
        {
          PageContext.AddLoadMessage("You should enter integer value for boards generated messages .");
          return;
        }

        // *************************
        if (!ValidationHelper.IsValidInt(ForumsTopicsNumber.Text.Trim()))
        {
          PageContext.AddLoadMessage("You should enter integer value for categories generated messages .");
          return;
        }

        if (!ValidationHelper.IsValidInt(ForumsMessagesNumber.Text.Trim()))
        {
          PageContext.AddLoadMessage("You should enter integer value for boards generated messages .");
          return;
        }

        // **********************************                
        if (!ValidationHelper.IsValidInt(TopicsMessagesNumber.Text.Trim()))
        {
          PageContext.AddLoadMessage("You should enter integer value for boards generated messages .");
          return;
        }

        string mesRetStr = string.Format(
          "Created: {0} Users, {1} Boards, {2} Categories, {3} Forums, {4} Topics, {5} Messages, {6} PMessages  Created.", 
          CreateUsers(), 
          CreateBoards(), 
          CreateCategories(), 
          CreateForums(), 
          CreateTopics(), 
          CreatePosts(), 
          CreatePMessages());
        DB.eventlog_create(
          PageContext.PageUserID, GetType().ToString(), "<b>Test Data Generator reports:</b><br /><p>" + mesRetStr + "</p>", EventLogTypes.Information);

        PageContext.AddLoadMessage(mesRetStr);

        // YafBuildLink.Redirect(ForumPages.admin_test_data);               
      }
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
    protected void ForumsCategory_OnSelectedIndexChanged(object sender, EventArgs e)
    {
      DataTable forums_category = DB.forum_listall_fromCat(PageContext.PageBoardID, Convert.ToInt32(ForumsCategory.SelectedValue));
      ForumsParent.DataSource = forums_category;
      ForumsParent.DataBind();
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
    protected void TopicsCategory_OnSelectedIndexChanged(object sender, EventArgs e)
    {
      DataTable topic_forums = DB.forum_listall_fromCat(PageContext.PageBoardID, Convert.ToInt32(TopicsCategory.SelectedValue));
      TopicsForum.DataSource = topic_forums;
      TopicsForum.DataBind();
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
    protected void PostsCategory_OnSelectedIndexChanged(object sender, EventArgs e)
    {
      DataTable posts_category = DB.forum_listall_fromCat(PageContext.PageBoardID, Convert.ToInt32(PostsCategory.SelectedValue));
      PostsForum.DataSource = posts_category;
      PostsForum.DataBind();
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
    protected void PostsForum_OnSelectedIndexChanged(object sender, EventArgs e)
    {
      int _forumID = 0;
      if (int.TryParse(PostsForum.SelectedValue, out _forumID))
      {
        DataTable topics = DB.topic_list(Convert.ToInt32(PostsForum.SelectedValue), PageContext.PageUserID, 0, null, 0, 100);
        PostsTopic.DataSource = topics;
        PostsTopic.DataBind();
      }
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
    protected void UsersBoardsOptions_OnSelectedIndexChanged(object sender, EventArgs e)
    {
      if (UsersBoardsOptions.SelectedIndex == 3)
      {
        UsersBoardsList.Visible = true;
      }
      else
      {
        UsersBoardsList.Visible = false;
      }
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
    protected void CategoriesBoardsOptions_OnSelectedIndexChanged(object sender, EventArgs e)
    {
      if (CategoriesBoardsOptions.SelectedIndex == 3)
      {
        CategoriesBoardsList.Visible = true;
      }
      else
      {
        CategoriesBoardsList.Visible = false;
      }
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
    protected void PMessagesBoardsOptions_OnSelectedIndexChanged(object sender, EventArgs e)
    {
      if (PMessagesBoardsOptions.SelectedIndex == 3)
      {
        PMessagesBoardsList.Visible = true;
      }
      else
      {
        PMessagesBoardsList.Visible = false;
      }
    }

    #endregion

    #region Create Methods

    /// <summary>
    /// The create users.
    /// </summary>
    /// <returns>
    /// The create users.
    /// </returns>
    private int CreateUsers()
    {
      int _users_Number = 0;

      if (!int.TryParse(UsersNumber.Text.Trim(), out _users_Number))
      {
        return 0;
      }

      if (_users_Number <= 0)
      {
        return 0;
      }

      int _outCounter = 0;
      int boardID = 0;
      int _countLimit = 1;

      bool _excludeCurrentBoard = false;
      switch (UsersBoardsOptions.SelectedIndex)
      {
        case 0:
          boardID = YafContext.Current.PageBoardID;
          break;
        case 1:
          _countLimit = UsersBoardsOptions.Items.Count;
          break;
        case 2:
          _countLimit = UsersBoardsOptions.Items.Count - 1;
          _excludeCurrentBoard = true;
          break;
        case 3:
          boardID = Convert.ToInt32(UsersBoardsList.SelectedValue);
          break;
      }

      int retBoards = 0;

      // if ( _users_Number > createCommonLimit ) _users_Number = createCommonLimit;
      for (int iboards = 0; iboards < _countLimit; iboards++)
      {
        if (boardID == 0)
        {
          boardID = Convert.ToInt32(UsersBoardsOptions.Items[iboards].Value);
        }

        int i = 0;
        for (i = 0; i < Convert.ToInt32(UsersNumber.Text.Trim()); i++)
        {
          this.randomGuid = Guid.NewGuid().ToString();
          string newEmail = this.userPrefix + this.randomGuid + "@test.info";
          string newUsername = this.userPrefix + this.randomGuid;

          if (!UserMembershipHelper.UserExists(newUsername, newEmail))
          {
            string hashinput = DateTime.Now + newEmail + Security.CreatePassword(20);
            string hash = FormsAuthentication.HashPasswordForStoringInConfigFile(hashinput, "md5");

            MembershipCreateStatus status;
            MembershipUser user = YafContext.Current.CurrentMembership.CreateUser(
              newUsername, 
              Password.Text.Trim(), 
              newEmail, 
              Question.Text.Trim(), 
              Answer.Text.Trim(), 
              !PageContext.BoardSettings.EmailVerification, 
              null, 
              out status);

            if (status == MembershipCreateStatus.Success)
            {
              // setup inital roles (if any) for this user
              RoleMembershipHelper.SetupUserRoles(boardID, newUsername);

              // create the user in the YAF DB as well as sync roles...
              int? userID = RoleMembershipHelper.CreateForumUser(user, YafContext.Current.PageBoardID);

              // create profile
              YafUserProfile userProfile = YafUserProfile.GetProfile(newUsername);

              // setup their inital profile information
              userProfile.Location = Location.Text.Trim();
              userProfile.Homepage = HomePage.Text.Trim();
              userProfile.Save();

              // save the time zone...
              if (!(Convert.ToInt32(UsersBoardsOptions.Items[i].Value) == YafContext.Current.PageBoardID && _excludeCurrentBoard))
              {
                DB.user_save(
                  UserMembershipHelper.GetUserIDFromProviderUserKey(user.ProviderUserKey), 
                  boardID, 
                  null, 
                  null, 
                  Convert.ToInt32(TimeZones.SelectedValue), 
                  null, 
                  null, 
                  null, 
                  null, 
                  null);
                _outCounter++;
              }
            }
          }

          retBoards++;
        }
      }

      return _outCounter;
    }

    /// <summary>
    /// The create boards.
    /// </summary>
    /// <returns>
    /// The create boards.
    /// </returns>
    private int CreateBoards()
    {
      int _boardNumber = 0;
      if (!int.TryParse(BoardNumber.Text.Trim(), out _boardNumber))
      {
        return 0;
      }

      if (_boardNumber <= 0)
      {
        return 0;
      }

      if (_boardNumber > this.boardCreateLimit)
      {
        _boardNumber = this.boardCreateLimit;
      }

      BoardMembershipName.Text = null;
      BoardRolesName.Text = null;
      int i = 0;
      for (i = 0; i < _boardNumber; i ++)
      {
        string boardName = this.boardPrefix + Guid.NewGuid();
        DB.board_create(PageContext.User.UserName, PageContext.User.ProviderUserKey, boardName, BoardMembershipName.Text.Trim(), BoardRolesName.Text.Trim());
        foreach (DataRow dr in DB.board_list(null).Rows)
        {
          if (dr["Name"].ToString().Trim() == boardName)
          {
            CreateCategories(Convert.ToInt32(dr["BoardID"]));
          }
        }
      }

      return i;
    }

    /// <summary>
    /// Create categories from boards
    /// </summary>
    /// <param name="boardID">
    /// </param>
    /// <returns>
    /// The create categories.
    /// </returns>
    private int CreateCategories(int boardID)
    {
      bool _excludeCurrentBoard = false;
      bool _useList = false;
      int _numCategories = 0;
      if (!int.TryParse(BoardsCategoriesNumber.Text.Trim(), out _numCategories))
      {
        return 0;
      }

      if (_numCategories < 0)
      {
        return 0;
      }

      if (_numCategories > this.categoryCreateLimit)
      {
        _numCategories = this.categoryCreateLimit;
      }

      int _numForums = 0;
      if (!int.TryParse(BoardsForumsNumber.Text.Trim(), out _numForums))
      {
        return 0;
      }

      if (_numForums < 0)
      {
        return 0;
      }

      int _numTopics = 0;
      if (!int.TryParse(BoardsTopicsNumber.Text.Trim(), out _numTopics))
      {
        return 0;
      }

      if (_numTopics < 0)
      {
        return 0;
      }

      int _numMessages = 0;
      if (!int.TryParse(BoardsMessagesNumber.Text.Trim(), out _numMessages))
      {
        return 0;
      }

      if (_numMessages < 0)
      {
        return 0;
      }


      return CreateCategoriesBase(boardID, 1, _numForums, _numTopics, _numMessages, _numCategories, _excludeCurrentBoard, _useList);
    }

    /// <summary>
    /// Create categories from Categories
    /// </summary>
    /// <returns>
    /// The create categories.
    /// </returns>
    private int CreateCategories()
    {
      int boardID = 0;

      // int categoriesLimit = 1;
      bool _excludeCurrentBoard = false;

      int _numForums = 0;
      if (!int.TryParse(CategoriesForumsNumber.Text.Trim(), out _numForums))
      {
        return 0;
      }

      if (_numForums < 0)
      {
        return 0;
      }

      int _numTopics = 0;
      if (!int.TryParse(CategoriesTopicsNumber.Text.Trim(), out _numTopics))
      {
        return 0;
      }

      if (_numTopics < 0)
      {
        return 0;
      }

      int _numMessages = 0;
      if (!int.TryParse(CategoriesMessagesNumber.Text.Trim(), out _numMessages))
      {
        return 0;
      }

      if (_numMessages < 0)
      {
        return 0;
      }

      int _numCategories = 0;
      int _boardCount = 1;
      bool _useList = false;
      switch (CategoriesBoardsOptions.SelectedIndex)
      {
        case 0:
          boardID = YafContext.Current.PageBoardID;
          break;
        case 1:
          _boardCount = CategoriesBoardsList.Items.Count;
          _useList = true;
          break;
        case 2:
          _boardCount = CategoriesBoardsList.Items.Count - 1;
          _excludeCurrentBoard = true;
          _useList = true;
          break;
        case 3:
          boardID = Convert.ToInt32(CategoriesBoardsList.SelectedValue);
          break;
      }

      if (!int.TryParse(CategoriesNumber.Text.Trim(), out _numCategories))
      {
        return 0;
      }

      if (_numCategories <= 0)
      {
        return 0;
      }

      if (_numCategories > this.categoryCreateLimit)
      {
        _numCategories = this.categoryCreateLimit;
      }

      return CreateCategoriesBase(boardID, _boardCount, _numForums, _numTopics, _numMessages, _numCategories, _excludeCurrentBoard, _useList);
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
    private int CreateCategoriesBase(
      int boardID, int boardCount, int numForums, int numTopics, int numMessages, int numCategories, bool excludeCurrentBoard, bool useList)
    {
      int ib = 0;
      int i = 0;
      for (ib = 0; ib < boardCount; ib++)
      {
        if (useList)
        {
          boardID = Convert.ToInt32(CategoriesBoardsList.Items[ib].Value);
        }

        if (!(excludeCurrentBoard && boardID == YafContext.Current.PageBoardID))
        {
          for (i = 0; i < numCategories; i++)
          {
            string catName = this.categoryPrefix + Guid.NewGuid();

            // TODO: should return number of categories created 
            DB.category_save(boardID, 0, catName, null, 100);
            DataTable dt = DB.category_simplelist(0, 10000);

            foreach (DataRow dr in dt.Rows)
            {
              if (dr["Name"].ToString() == catName)
              {
                CreateForums(Convert.ToInt32(dr["CategoryID"]), null, numForums, numTopics, numMessages);
              }
            }

            // We don't have last category index, so not implemented.... CreateForums( categoryID,numForums,numTopics,numMessages )
          }
        }

        i = 0;
      }

      return ib;
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
      int parentIDInt = 0;
      if (int.TryParse(ForumsParent.Text.Trim(), out parentIDInt))
      {
        parentID = parentIDInt;
      }

      int numTopics = 0;
      if (!int.TryParse(ForumsTopicsNumber.Text.Trim(), out numTopics))
      {
        return 0;
      }

      if (numTopics < 0)
      {
        return 0;
      }

      int numPosts = 0;
      if (!int.TryParse(ForumsMessagesNumber.Text.Trim(), out numPosts))
      {
        return 0;
      }

      if (numPosts < 0)
      {
        return 0;
      }

      int numForums = 0;
      if (!int.TryParse(ForumsNumber.Text.Trim(), out numForums))
      {
        return 0;
      }

      if (numForums <= 0)
      {
        return 0;
      }

      int categoryID = 0;
      if (!int.TryParse(ForumsCategory.SelectedValue, out categoryID))
      {
        return 0;
      }

      if (numForums > this.createCommonLimit)
      {
        numForums = this.createCommonLimit;
      }

      return CreateForums(categoryID, parentID, numForums, numTopics, numPosts);
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
    /// </param>
    /// <param name="_messagesToCreate">
    /// </param>
    /// <returns>
    /// The create forums.
    /// </returns>
    private int CreateForums(int categoryID, object parentID, int numForums, int _topicsToCreate, int _messagesToCreate)
    {
      bool countMessagesInStatistics = false;
      if (String.IsNullOrEmpty(ForumsCountMessages.Text.Trim()))
      {
        countMessagesInStatistics = true;
      }

      bool IsHiddenIfNoAccess = false;
      if (String.IsNullOrEmpty(ForumsHideNoAccess.Text.Trim()))
      {
        IsHiddenIfNoAccess = true;
      }

      IsHiddenIfNoAccess = true;

      // ForumsCategory.Items.FindByValue("0").Selected = true; 
      long uniqueForum = 0;
      int iforums = 0;
      for (iforums = 0; iforums < numForums; iforums++)
      {
        long _forumID = 0;
        this.randomGuid = Guid.NewGuid().ToString();
        DataTable _accessForumList = DB.forumaccess_list(_forumID);
        _forumID = DB.forum_save(
          _forumID, 
          categoryID, 
          parentID, 
          this.forumPrefix + this.randomGuid, 
          "Description of " + this.forumPrefix + this.randomGuid, 
          100, 
          false, 
          IsHiddenIfNoAccess, 
          countMessagesInStatistics, 
          false, 
          ForumsStartMask.SelectedValue, 
          null, 
          null, 
          false);
        if (_forumID > 0)
        {
          for (int i1 = 0; i1 < _accessForumList.Rows.Count; i1++)
          {
            DB.forumaccess_save(_forumID, _accessForumList.Rows[i1]["GroupID"], Convert.ToInt32(_accessForumList.Rows[i1]["AccessMaskID"]));
          }

          DB.forumaccess_save(_forumID, ForumsGroups.SelectedValue, ForumsAdminMask.SelectedValue);

          if (_topicsToCreate > 0)
          {
            if (uniqueForum != _forumID)
            {
              CreateTopics(Convert.ToInt32(_forumID), _topicsToCreate, _messagesToCreate);
              uniqueForum = _forumID;
            }
          }
        }
      }

      return iforums;
    }

    /// <summary>
    /// The get message flags.
    /// </summary>
    /// <returns>
    /// The get message flags.
    /// </returns>
    private int GetMessageFlags()
    {
      BaseForumEditor _forumEditor = PageContext.EditorModuleManager.GetEditorInstance(PageContext.BoardSettings.ForumEditor);
      var tFlags = new MessageFlags();

      tFlags.IsHtml = _forumEditor.UsesHTML;
      tFlags.IsBBCode = _forumEditor.UsesBBCode;
      tFlags.IsPersistent = false;

      // Bypass Approval if Admin or Moderator.
      tFlags.IsApproved = PageContext.IsAdmin;
      return tFlags.BitValue;
    }

    /// <summary>
    /// The create topics.
    /// </summary>
    /// <returns>
    /// The create topics.
    /// </returns>
    private int CreateTopics()
    {
      return CreateTopics(0, 0, 0);
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
    /// The create topics.
    /// </returns>
    private int CreateTopics(int forumID, int numTopics, int _messagesToCreate)
    {
      object _priority = 0;
      if (forumID <= 0)
      {
        _priority = TopicsPriorityList.SelectedValue;
      }

      if (numTopics <= 0)
      {
        if (!int.TryParse(TopicsNumber.Text.Trim(), out numTopics))
        {
          return 0;
        }
      }

      if (numTopics <= 0)
      {
        return 0;
      }

      int categoryID = 0;

      if (!int.TryParse(TopicsCategory.SelectedValue, out categoryID))
      {
        return 0;
      }

      if (forumID <= 0)
      {
        if (!int.TryParse(TopicsForum.SelectedValue, out forumID))
        {
          return 0;
        }
      }

      if (_messagesToCreate <= 0)
      {
        if (!int.TryParse(TopicsMessagesNumber.Text.Trim(), out _messagesToCreate))
        {
          return 0;
        }
      }

      if (_messagesToCreate < 0)
      {
        return 0;
      }

      // if ( numTopics > createCommonLimit ) numTopics = createCommonLimit;         
      int itopics = 0;
      for (itopics = 0; itopics < numTopics; itopics++)
      {
        long messageid = 0;
        this.randomGuid = Guid.NewGuid().ToString();
        object pollID = null;
        if (PollCreate.Checked)
        {
          pollID = DB.poll_save(
            "quest-" + this.randomGuid, "ans1-" + this.randomGuid, "ans2-" + this.randomGuid, null, null, null, null, null, null, null, null);
        }

        long topicID = DB.topic_save(
          forumID, 
          this.topicPrefix + this.randomGuid, 
          this.messagePrefix + this.randomGuid, 
          PageContext.PageUserID, 
          _priority, 
          pollID, 
          PageContext.User.UserName, 
          Request.UserHostAddress, 
          null, 
          string.Empty, 
          GetMessageFlags(), 
          ref messageid);
        if (_messagesToCreate > 0)
        {
          CreatePosts(forumID, Convert.ToInt32(topicID), _messagesToCreate);
        }

        // User != null ? null : From.Text
      }

      return itopics;
    }

    /// <summary>
    /// The create posts.
    /// </summary>
    /// <returns>
    /// The create posts.
    /// </returns>
    private int CreatePosts()
    {
      return CreatePosts(0, 0, 0);
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
    /// The create posts.
    /// </returns>
    private int CreatePosts(int forumID, int topicID, int numMessages)
    {
      if (numMessages <= 0)
      {
        if (!int.TryParse(PostsNumber.Text.Trim(), out numMessages))
        {
          return 0;
        }
      }

      if (numMessages <= 0)
      {
        return 0;
      }

      int categoryID = 0;
      if (!int.TryParse(PostsCategory.SelectedValue, out categoryID))
      {
        return 0;
      }

      if (forumID <= 0)
      {
        if (!int.TryParse(PostsForum.SelectedValue, out forumID))
        {
          return 0;
        }
      }

      if (topicID <= 0)
      {
        if (!int.TryParse(PostsTopic.SelectedValue, out topicID))
        {
          return 0;
        }
      }

      // if ( numMessages > createCommonLimit ) numMessages = createCommonLimit;        
      long messageid = 0;
      int iposts = 0;
      int _replyTo = -1;
      for (iposts = 0; iposts < numMessages; iposts++)
      {
        this.randomGuid = Guid.NewGuid().ToString();
        DB.message_save(
          topicID, 
          PageContext.PageUserID, 
          "msgd-" + this.randomGuid + "  " + MyMessage.Text.Trim(), 
          PageContext.User.UserName, 
          Request.UserHostAddress, 
          null, 
          _replyTo, 
          GetMessageFlags(), 
          ref messageid);

        // User != null ? null : From.Text
      }

      return iposts;
    }

    /// <summary>
    /// The create p messages.
    /// </summary>
    /// <returns>
    /// The create p messages.
    /// </returns>
    private int CreatePMessages()
    {
      int userID = PageContext.PageUserID;
      int numPMessages = 0;
      if (!int.TryParse(PMessagesNumber.Text.Trim(), out numPMessages))
      {
        return 0;
      }

      if (numPMessages <= 0)
      {
        return 0;
      }

      string _fromUser = From.Text.Trim();
      string _toUser = To.Text.Trim();
      if (numPMessages > this.createCommonLimit)
      {
        numPMessages = this.createCommonLimit;
      }

      int i = 0;
      for (i = 0; i < numPMessages; i++)
      {
        this.randomGuid = Guid.NewGuid().ToString();
        DB.pmessage_save(
          DB.user_get(YafContext.Current.PageBoardID, Membership.GetUser(_fromUser).ProviderUserKey), 
          DB.user_get(YafContext.Current.PageBoardID, Membership.GetUser(_toUser).ProviderUserKey), 
          this.topicPrefix + this.randomGuid, 
          this.pmessagePrefix + this.randomGuid + "   " + PMessageText.Text.Trim(), 
          6);

        // User != null ? null : From.Text                
      }

      if (MarkRead.Checked)
      {
        foreach (DataRow dr in DB.pmessage_list(null, DB.user_get(YafContext.Current.PageBoardID, Membership.GetUser(_toUser).ProviderUserKey), null).Rows)
        {
          DB.pmessage_markread(dr["PMessageID"]);
        }
      }

      return i;
    }

    #endregion
  }
}