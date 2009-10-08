/* Code created by vzrus: http://sourceforge.net/users/vzrus  
 * for Yet Another Forum.NET http://www.yetanotherforum.net/
 * and can be used and modified without any limitations as part 
 * of Yet Another Forum.NET under every open source licence.  
 * Creation date:10/3/09
 */

namespace YAF.Pages.Admin
{
	using System;
	using System.Web.Security;
	using YAF.Classes;
	using YAF.Classes.Core;
	using YAF.Classes.Utils;
    using System.Data;

	/// <summary>
	///	The control generates test data for different data layers.
	/// </summary>
	public partial class test_data : YAF.Classes.Core.AdminPage
	{

        #region Fields
        private string regBase = @"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$";
        private String randomGuid = System.Guid.NewGuid( ).ToString( );        
        private string userPrefix = "usr-";
        private string boardPrefix = "board-";
        private string forumPrefix = "forum-";
        private string categoryPrefix = "cat-";
        private string messagePrefix = "msg-";
        private string topicPrefix = "topic-";
        private string pmessagePrefix = "pmsg-";
        private int categoryCreateLimit = 100;
        private int boardCreateLimit = 100;

        //This limits number of records returned in selects for delete operations
        private int delCommonLimit = 999;
        //This limits number of created records, currently is fasing out and will be outdated.
        private int createCommonLimit = 9999;
        private DataRow boardObjectStats = YAF.Classes.Data.DB.board_poststats(YafContext.Current.PageBoardID);
        #endregion

		protected void Page_Load( object sender, System.EventArgs e )
		{
            
			if ( !IsPostBack )
			{

				PageLinks.AddLink( PageContext.BoardSettings.Name, YafBuildLink.GetLink( ForumPages.forum ) );
				PageLinks.AddLink( "Administration", YafBuildLink.GetLink( ForumPages.admin_admin ) );
				PageLinks.AddLink( "Users", "" );                
                
                System.Data.DataTable categories = YAF.Classes.Data.DB.category_list(PageContext.PageBoardID, null);
				TimeZones.DataSource = StaticDataHelper.TimeZones( );
                ForumsCategory.DataSource = categories;                
                TopicsCategory.DataSource = categories;
                PostsCategory.DataSource = categories;                
                ForumsStartMask.DataSource = YAF.Classes.Data.DB.accessmask_list(PageContext.PageBoardID, null);
                if ( ForumsStartMask.Items.Count > 1 )
                ForumsStartMask.SelectedIndex = 1;
                ForumsAdminMask.DataSource = ForumsStartMask.DataSource;
                ForumsAdminMask.SelectedIndex = 0;
                ForumsGroups.DataSource = YAF.Classes.Data.DB.group_list(PageContext.PageBoardID, null);
               
                DataBind();
               
                ForumsCategory.SelectedIndex = -1;
                TopicsCategory.ClearSelection();
                PostsCategory.ClearSelection();              
                
                ForumsCategory.SelectedIndex = -1;

                
				TimeZones.Items.FindByValue( "0" ).Selected = true;
              
                From.Text = PageContext.User.UserName.ToString( );
                To.Text = PageContext.User.UserName.ToString();
                TopicsPriorityList.Items.Add(new System.Web.UI.WebControls.ListItem("Normal", "0"));
                TopicsPriorityList.Items.Add(new System.Web.UI.WebControls.ListItem("Sticky", "1"));
                TopicsPriorityList.Items.Add(new System.Web.UI.WebControls.ListItem("Announcement", "2"));
                TopicsPriorityList.SelectedIndex = 0;
                
                //AllUsers.Visible = PageContext.IsAdmin;
               // AllUsersFrom.Visible = PageContext.IsAdmin;
			}
            
            
		}
		
        #region Events

        protected void cancel_Click(object sender, EventArgs e)
        {           
            YafBuildLink.Redirect(ForumPages.admin_test_data);
        }

        protected void CreateTestData_Click(object sender, System.EventArgs e)
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

                string mesRetStr = string.Format("Created: {0} Users, {1} Boards, {2} Categories, {3} Forums, {4} Topics, {5} Messages, {6} PMessages  Created. {7} Users, {8} Boards, {9} Categories, {10} Forums, {11} Topics, {12} Messages, {13} PMessages Deleted.", CreateUsers(), CreateBoards(), CreateCategories(), CreateForums(), CreateTopics(), CreatePosts(), CreatePMessages(), DeleteTestUsers(), DeleteTestBoards(), DeleteTestCategories(), DeleteTestForums(), DeleteTestTopics(), DeleteTestPosts(), DeleteTestPMessages());
                YAF.Classes.Data.DB.eventlog_create(PageContext.PageUserID, this.GetType().ToString(), "<b>Test Data Generator reports:</b><br /><p>" + mesRetStr + "</p>", EventLogTypes.Information);
               
                PageContext.AddLoadMessage( mesRetStr );
                // YafBuildLink.Redirect(ForumPages.admin_test_data);               
            }
        }

        protected void ForumsCategory_OnSelectedIndexChanged(object sender, EventArgs e)
         {
             System.Data.DataTable forums_category = YAF.Classes.Data.DB.forum_listall_fromCat(PageContext.PageBoardID, Convert.ToInt32(ForumsCategory.SelectedValue));
             ForumsParent.DataSource = forums_category;
             ForumsParent.DataBind();
             
         }
        protected void TopicsCategory_OnSelectedIndexChanged(object sender, EventArgs e)
         {
             System.Data.DataTable topic_forums = YAF.Classes.Data.DB.forum_listall_fromCat(PageContext.PageBoardID, Convert.ToInt32(TopicsCategory.SelectedValue));
             TopicsForum.DataSource = topic_forums;
             TopicsForum.DataBind();

         }
        protected void PostsCategory_OnSelectedIndexChanged(object sender, EventArgs e)
         {
             System.Data.DataTable posts_category = YAF.Classes.Data.DB.forum_listall_fromCat(PageContext.PageBoardID, Convert.ToInt32(PostsCategory.SelectedValue));
             PostsForum.DataSource = posts_category;
             PostsForum.DataBind();

         }
        protected void PostsForum_OnSelectedIndexChanged(object sender, EventArgs e)
         {
             int _forumID = 0;
             if (int.TryParse(PostsForum.SelectedValue, out _forumID))
             {
                 System.Data.DataTable topics = YAF.Classes.Data.DB.topic_list(Convert.ToInt32(PostsForum.SelectedValue), PageContext.PageUserID, 0, null, 0, 100);
                 PostsTopic.DataSource = topics;
                 PostsTopic.DataBind();
             }

         }

             
        #endregion

        #region Create Methods
 
        protected int CreateUsers()
        {
            int _users_Number = 0;            
            if ( !int.TryParse(UsersNumber.Text.Trim(), out  _users_Number ) ) return 0;
            if ( _users_Number <= 0 ) return 0;
            int boardID = 0;            
            if ( CreateUsersInCurrentBoardCheckBox.Checked )
                boardID = YafContext.Current.PageBoardID;
            if ( _users_Number > createCommonLimit ) _users_Number = createCommonLimit;
            int i = 0;
            for ( i = 0; i < Convert.ToInt32( UsersNumber.Text.Trim( ) ); i++ )
            {
                randomGuid = System.Guid.NewGuid( ).ToString( );
                string newEmail = userPrefix + randomGuid + "@test.info";
                string newUsername = userPrefix + randomGuid;

                if ( !UserMembershipHelper.UserExists( newUsername, newEmail ) )
                {

                    string hashinput = DateTime.Now.ToString( ) + newEmail + Security.CreatePassword( 20 );
                    string hash = FormsAuthentication.HashPasswordForStoringInConfigFile( hashinput, "md5" );

                    MembershipCreateStatus status;
                    MembershipUser user = PageContext.CurrentMembership.CreateUser( newUsername, Password.Text.Trim( ), newEmail, Question.Text.Trim( ), Answer.Text.Trim( ), !PageContext.BoardSettings.EmailVerification, null, out status );

                    if ( status == MembershipCreateStatus.Success )
                    {

                        // setup inital roles (if any) for this user
                        RoleMembershipHelper.SetupUserRoles( boardID, newUsername );

                        // create the user in the YAF DB as well as sync roles...
                        int? userID = RoleMembershipHelper.CreateForumUser( user, YafContext.Current.PageBoardID );

                        // create profile
                        YafUserProfile userProfile = YafUserProfile.GetProfile( newUsername );
                        // setup their inital profile information
                        userProfile.Location = Location.Text.Trim( );
                        userProfile.Homepage = HomePage.Text.Trim( );
                        userProfile.Save( );

                        // save the time zone...
                        YAF.Classes.Data.DB.user_save( UserMembershipHelper.GetUserIDFromProviderUserKey(user.ProviderUserKey ), PageContext.PageBoardID, null, null, Convert.ToInt32(TimeZones.SelectedValue ), null, null, null, null, null );

                    }
                }
            }
            return i;
        }
        protected int CreateBoards( )
        {
            int _boardNumber = 0;
            if ( !int.TryParse( BoardNumber.Text.Trim(),out _boardNumber ) )
                return 0;
            if ( _boardNumber <= 0 ) return 0;
            if (_boardNumber > boardCreateLimit) _boardNumber = boardCreateLimit;
            BoardMembershipName.Text = null;
            BoardRolesName.Text = null;
            int i = 0;
            for ( i = 0; i < _boardNumber; i ++ )
            {
                YAF.Classes.Data.DB.board_create( PageContext.User.UserName, PageContext.User.ProviderUserKey, boardPrefix + System.Guid.NewGuid( ).ToString( ), BoardMembershipName.Text.Trim( ), BoardRolesName.Text.Trim( ) );
            }
            return i;
        }
        protected int CreateCategories()
        {
            return CreateCategories(0,0,0,0); 
        }
        protected int CreateCategories( int boardID, int numForums, int numTopics, int numMessages )
        {
            int _numCategories = 0;
            if ( !int.TryParse( CategoriesNumber.Text.Trim( ), out _numCategories ) )
                return 0;
            if (boardID <= 0)
                boardID = YafContext.Current.PageBoardID;
            if ( _numCategories <= 0 ) return 0;
            if (_numCategories > categoryCreateLimit) _numCategories = categoryCreateLimit;
           
            int i = 0;
            for ( i = 0; i < _numCategories; i++ )
            {
                randomGuid = System.Guid.NewGuid( ).ToString( );
                // TODO: should return number of categories created 
                YAF.Classes.Data.DB.category_save( boardID, 0, categoryPrefix + randomGuid, null, 100);
               
              // if (numForums > 0 && numTopics > 0 && numMessages > 0) CreateForums( categoryID,numForums,numTopics,numMessages )
              
            }

            return i;
        }
        protected int CreateForums()
        {
            return CreateForums(0);
        }
        protected int CreateForums( int categoryID )
        {
            int numForums = 0;
            if ( !int.TryParse( ForumsNumber.Text.Trim( ), out numForums ) )
                return 0;
            if ( numForums <= 0 ) return 0;
            
            if ( !int.TryParse( ForumsCategory.SelectedValue, out categoryID ) )
                return 0;

            object parentID = null;
            //if (!int.TryParse(ForumsParent.Text.Trim(), out parentID))
            //   return 0;

            int _topicsToCreate = 0;
            if (!int.TryParse(ForumsMessagesNumber.Text.Trim(), out _topicsToCreate))
                return 0;
            if (_topicsToCreate < 0) return 0;

            int _messagesToCreate = 0;
            if (!int.TryParse(ForumsMessagesNumber.Text.Trim(), out _messagesToCreate))
                return 0;
            if (_messagesToCreate < 0) return 0;
           
           
           if ( numForums > createCommonLimit ) numForums = createCommonLimit;
           bool countMessagesInStatistics = false;
           if ( String.IsNullOrEmpty( ForumsCountMessages.Text.Trim( ) ) )
               countMessagesInStatistics = true;
           bool IsHiddenIfNoAccess = false;
           if ( String.IsNullOrEmpty( ForumsHideNoAccess.Text.Trim( ) ) )
               IsHiddenIfNoAccess = true;
           IsHiddenIfNoAccess = true;
            // ForumsCategory.Items.FindByValue("0").Selected = true;             
            long uniqueForum = 0;
            int iforums = 0;
            for (iforums = 0; iforums < numForums; iforums++)
            {
                long _forumID = 0;
                randomGuid = System.Guid.NewGuid( ).ToString( );
                DataTable _accessForumList = YAF.Classes.Data.DB.forumaccess_list( _forumID );
                _forumID = YAF.Classes.Data.DB.forum_save(_forumID, categoryID, parentID, forumPrefix + randomGuid, "Description of " + forumPrefix + randomGuid, 100, false, IsHiddenIfNoAccess, countMessagesInStatistics, false, ForumsStartMask.SelectedValue, null, null, false);
                if ( _forumID > 0 )
                {                   
             
                    for ( int i1 = 0; i1 < _accessForumList.Rows.Count; i1++ )
                    {                   
                        YAF.Classes.Data.DB.forumaccess_save( _forumID, _accessForumList.Rows[ i1 ][ "GroupID" ], Convert.ToInt32( _accessForumList.Rows[ i1 ][ "AccessMaskID" ] ) );
                    }

                    YAF.Classes.Data.DB.forumaccess_save(_forumID, ForumsGroups.SelectedValue, ForumsAdminMask.SelectedValue);
                   
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
      
        protected int GetMessageFlags( )
        {
            YAF.Editors.BaseForumEditor _forumEditor = PageContext.EditorModuleManager.GetEditorInstance(PageContext.BoardSettings.ForumEditor);
            YAF.Classes.Data.MessageFlags tFlags = new YAF.Classes.Data.MessageFlags();

            tFlags.IsHtml = _forumEditor.UsesHTML;
            tFlags.IsBBCode = _forumEditor.UsesBBCode;
            tFlags.IsPersistent = false;

            // Bypass Approval if Admin or Moderator.
            tFlags.IsApproved = PageContext.IsAdmin;
            return tFlags.BitValue;
        }
        protected int CreateTopics( )
        {
            return CreateTopics( 0,0,0 );
        }
        protected int CreateTopics(int forumID, int numTopics, int _messagesToCreate)
        {
            
            object _priority = 0;
            if (forumID <=0)
            _priority = TopicsPriorityList.SelectedValue;

            if (numTopics <= 0)
            {
                if (!int.TryParse(TopicsNumber.Text.Trim(), out numTopics))
                    return 0;
            }
         
            if (numTopics <= 0) return 0;
            
            int categoryID = 0;

            if ( !int.TryParse( TopicsCategory.SelectedValue, out categoryID ) )
                return 0;
            if (forumID <= 0)
            {
                if (!int.TryParse(TopicsForum.SelectedValue, out forumID))
                    return 0;
            }
            if (_messagesToCreate <= 0)
            {
                if (!int.TryParse(TopicsMessagesNumber.Text.Trim(), out _messagesToCreate))
                    return 0;
            }
            if (_messagesToCreate < 0) return 0;
          // if ( numTopics > createCommonLimit ) numTopics = createCommonLimit;         
            
            int itopics = 0;
            for (itopics = 0; itopics < numTopics; itopics++)
            {
                long messageid = 0;                
                randomGuid = System.Guid.NewGuid( ).ToString( );
                object pollID = null;
                if ( PollCreate.Checked ) pollID = YAF.Classes.Data.DB.poll_save( "quest-" + randomGuid, "ans1-" + randomGuid, "ans2-" + randomGuid, null, null, null, null, null, null, null, null );
                long topicID = YAF.Classes.Data.DB.topic_save( forumID, topicPrefix + randomGuid, messagePrefix + randomGuid, PageContext.PageUserID, _priority , pollID, PageContext.User.UserName,
                Request.UserHostAddress, null, "", GetMessageFlags( ), ref messageid );
                if (_messagesToCreate > 0)
                {
                    CreatePosts(forumID, Convert.ToInt32(topicID), _messagesToCreate);
                }
                //   User != null ? null : From.Text
            }
            return itopics;
        }

        private int CreatePosts()
        {
            return CreatePosts(0,0,0);
        }       
        private int CreatePosts(int forumID, int topicID, int numMessages )
        {

            if (numMessages <= 0)
            {
                if (!int.TryParse(PostsNumber.Text.Trim(), out numMessages))
                    return 0;
            }

            if (numMessages <= 0) return 0;

            int categoryID = 0;
            if (!int.TryParse(PostsCategory.SelectedValue, out categoryID))
                return 0;
            if (forumID <= 0)
            {
                if (!int.TryParse(PostsForum.SelectedValue, out forumID))
                    return 0;
            }
            if (topicID <= 0)
            {
                if (!int.TryParse(PostsTopic.SelectedValue, out topicID))
                return 0;
            }
          
           // if ( numMessages > createCommonLimit ) numMessages = createCommonLimit;        

        
            long messageid = 0;
            int iposts = 0;
            int _replyTo = -1;
            for (iposts = 0; iposts < numMessages; iposts++)
            {

                randomGuid = System.Guid.NewGuid( ).ToString( );
                YAF.Classes.Data.DB.message_save(topicID, PageContext.PageUserID, "msgd-" + randomGuid + "  " + MyMessage.Text.Trim(), PageContext.User.UserName, Request.UserHostAddress, null, _replyTo, GetMessageFlags(), ref messageid);
                //   User != null ? null : From.Text
            }
            return iposts;
        }
        protected int CreatePMessages( )
        {
            int numPMessages = 0;
            if (!int.TryParse(PMessagesNumber.Text.Trim(), out numPMessages))
                return 0;
            if (numPMessages <= 0) return 0;

            string _fromUser = From.Text.Trim( );
            string _toUser = To.Text.Trim( );           
           // if ( numPMessages > createCommonLimit ) numPMessages = createCommonLimit;
            
            int i = 0;
            for ( i = 0; i < numPMessages; i++ )
            {

                randomGuid = System.Guid.NewGuid( ).ToString( );
                YAF.Classes.Data.DB.pmessage_save( PageContext.PageUserID, PageContext.PageUserID, topicPrefix + randomGuid, pmessagePrefix + randomGuid, GetMessageFlags( ) );
                //   User != null ? null : From.Text
            }
            return i;
        }
      
        #endregion

        #region Delete Methods
      
        protected int DeleteTestUsers( )
         {
             int delCount = 0;
             //The field is already validated as integer!
             if ( !DeleteUsersCheckBox.Checked )
                 return 0;
             int boardID = 0;
             if ( DeleteUsersCurrentBoardCheckBox.Checked )
                 boardID = YafContext.Current.PageBoardID;
             int _total = 0;            
             
             for ( int ii = 0; ii < 10000; ii++ )            
             {

                 MembershipUserCollection muc = Membership.GetAllUsers(ii, delCommonLimit, out _total);
                 if ( _total == 0 ) break;
                 foreach ( MembershipUser musr in muc )
                 {
                     
                     if (System.Text.RegularExpressions.Regex.IsMatch(musr.UserName.Replace(userPrefix, ""), regBase))
                     {
                         YAF.Classes.Data.DB.user_delete(YAF.Classes.Data.DB.user_get(YafContext.Current.PageBoardID, musr.ProviderUserKey));
                         delCount++;
                     }
                    
                 }            
             }
            
             return delCount;
         }
        protected int DeleteTestBoards( )
         {           
             if ( !DeleteBoardsCheckBox.Checked )
                 return 0;
             DataTable _boardList = YAF.Classes.Data.DB.board_list( null );
             int delCount = 0;
             foreach ( DataRow dr in _boardList.Rows )
             {
                if ( System.Text.RegularExpressions.Regex.IsMatch( dr["Name"].ToString( ).Replace( boardPrefix, "" ), regBase ) )
                {
                     YAF.Classes.Data.DB.board_delete( dr["BoardID"] );
                     delCount++;
                 }

             }
             return delCount;
         }
        protected int DeleteTestCategories()
         {
            
            if ( !DeleteCategoriesCheckBox.Checked )
                 return 0;
             DataTable _categoryList = YAF.Classes.Data.DB.category_list( YafContext.Current.PageBoardID, null );
             int delCount = 0;
             foreach ( DataRow dr in _categoryList.Rows )
             {
                 if ( System.Text.RegularExpressions.Regex.IsMatch(dr["Name"].ToString().Replace(categoryPrefix, ""), regBase ) )
                 {
                     YAF.Classes.Data.DB.category_delete( dr[ "CategoryID" ] );
                     delCount++;
                 }

             }
             return delCount;
         }
        protected int DeleteTestForums( )
         {          
              
              if ( !DeleteForumsCheckBox.Checked )
                return 0;
              int _boardForums = 0;
              if (!int.TryParse(boardObjectStats["Forums"].ToString(), out _boardForums))
                  return 0;

              int delCount = 0;
              int startForumList = 0;

              for (int i = 0; i < _boardForums / delCommonLimit + 1; i++)
              {
                  
                  DataTable _forumList = YAF.Classes.Data.DB.forum_listall(YafContext.Current.PageBoardID, PageContext.PageUserID, startForumList);

                  foreach (DataRow dr in _forumList.Rows)
                  {
                      if (System.Text.RegularExpressions.Regex.IsMatch(dr["Forum"].ToString().Replace(forumPrefix, ""), regBase))
                      {
                          YAF.Classes.Data.DB.forum_delete(dr["ForumID"]);
                          delCount++;
                      }

                  }
                  startForumList = startForumList + delCommonLimit;
                  i++;
              }
             return delCount;
         }
        protected int DeleteTestTopics( )
         {
             if ( !DeleteTopicsCheckBox.Checked )
                  return 0;
             int _boardTopics = 0;
             if (!int.TryParse(boardObjectStats["Topics"].ToString(), out _boardTopics))
                  return 0;

              int delCount = 0;
              int startTopicList = 0;

              for (int i = 0; i < _boardTopics / delCommonLimit + 1; i++)
              {
                  DataTable _topicList = YAF.Classes.Data.DB.topic_simplelist(startTopicList, delCommonLimit);
                 
                  foreach (DataRow dr in _topicList.Rows)
                  {
                      if (System.Text.RegularExpressions.Regex.IsMatch(dr["Topic"].ToString().Replace(topicPrefix, ""), regBase))
                      {
                          YAF.Classes.Data.DB.topic_delete(dr["TopicID"], true);
                          delCount++;
                      }

                  }
                  startTopicList = startTopicList + delCommonLimit;
                  i++;
              }
              return delCount;
         }
        protected int DeleteTestPosts()
        {   
                int ii = 0;
                if (int.TryParse(DeletedMessagesNumber.Text.Trim(),out ii))
                    return DeleteTestPosts(ii); 
                else              
                    return DeleteTestPosts(null);
        }
        protected int DeleteTestPosts( int? numDel )
         {

             if ( !DeletePostsCheckBox.Checked )
                 return 0;
             int _boardMessages = 0;
             if (!int.TryParse(boardObjectStats["Posts"].ToString(), out _boardMessages))
                  return 0;
   


              int delCount = 0;
              int startMessageList = 0;

              for (int i = 0; i < _boardMessages / delCommonLimit + 1; i++)
              {
                  DataTable _postList = YAF.Classes.Data.DB.message_simplelist(startMessageList, delCommonLimit);
                 
             foreach ( DataRow dr in _postList.Rows )
             {
                 DataTable _messageList = YAF.Classes.Data.DB.message_list(dr[ "MessageID" ]);

                 System.Text.RegularExpressions.Regex regx = new System.Text.RegularExpressions.Regex(regBase);
                 System.Text.RegularExpressions.MatchCollection matches = regx.Matches(_messageList.Rows[0]["Message"].ToString().Replace(messagePrefix, ""));

                 foreach (System.Text.RegularExpressions.Match match in matches)
                 {
                     YAF.Classes.Data.DB.message_delete(dr["MessageID"], false, "", 1, true);
                     delCount++;
                     if (numDel != null)
                     {
                         numDel--;
                         if (numDel == 0) return delCount;
                     }
                     break;
                 }
                

                 /* if (System.Text.RegularExpressions.Regex.IsMatch(_messageList.Rows[0]["Message"].ToString().Replace(messagePrefix, ""), regBase))
                 {
                     YAF.Classes.Data.DB.message_delete( dr["MessageID"], false, "", 1, true );
                     delCount++;
                 }*/

             }
             startMessageList = startMessageList + delCommonLimit;
             i++;
              }
             return delCount;
         }
        protected int DeleteTestPMessages( )
         {
             int delCount = 0;
            /* DataTable _pmessageList = YAF.Classes.Data.DB.pmessage_list(null, null,dr);
             
             foreach ( DataRow dr in _pmessageList.Rows )
             {
                 if (dr["Name"].ToString() == pmessagePattern)
                 {
                     YAF.Classes.Data.DB.category_delete(dr["PMessageID"]);
                     delCount++;
                 }

             }*/
             return delCount;
         }
        protected int DeleteAllTestData( string userPattern )
         {
             return 0;
         }

        #endregion        

    }
    
}
