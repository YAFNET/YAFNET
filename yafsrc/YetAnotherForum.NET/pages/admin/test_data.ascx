<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.test_data"
	TargetSchema="http://schemas.microsoft.com/intellisense/ie5" Codebehind="test_data.ascx.cs" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server" ID="Adminmenu1">
	<asp:Panel id="TestDataTabs" runat="server">
               <ul>
                 <li><a href="#View1">Users</a></li>
		 <li><a href="#View2">Boards</a></li>
		 <li><a href="#View3">Categories</a></li>
		 <li><a href="#View4">Forums</a></li>
		 <li><a href="#View5">Topics</a></li>		        
		 <li><a href="#View6">Messages</a></li>	
		 <li><a href="#View7">PMessages</a></li>
                 <li><a href="#View8">Settings/Help</a></li>
               </ul>
                <div id="View1">
                   <table class="content" cellspacing="1" cellpadding="0" width="100%">
					<tr>
						<td class="header1" colspan="2">
							Create Test Users
						</td>
					</tr>
					<tr>
						<td class="postheader" width="50%">
							Number of users to create:
						</td>
						<td class="post">
							<asp:TextBox ID="UsersNumber" runat="server" CssClass="Numeric">0</asp:TextBox>
							<asp:RequiredFieldValidator ID="Requiredfieldvalidator1" runat="server" EnableClientScript="False"
								Text="0" ControlToValidate="UsersNumber" ErrorMessage="User's number is required."></asp:RequiredFieldValidator>
						</td>
					</tr>					
					<tr>
						<td class="postheader" width="50%">
							Create Users:
						</td>
						<td class="post">
						<asp:RadioButtonList ID="UsersBoardsOptions"  Enabled="true"   AutoPostBack="true" runat="server"
							 	OnSelectedIndexChanged="UsersBoardsOptions_OnSelectedIndexChanged" />								
						</td>
					</tr>
					<tr>
						<td class="postheader">						
						</td>
						<td class="post">
							<asp:DropDownList ID="UsersBoardsList" runat="server"  Visible="false" DataValueField="BoardID" DataTextField="Name" CssClass="standardSelectMenu">
							</asp:DropDownList>
						</td>
					</tr>
					<tr>
						<td class="postheader">
							Password(for all users):
						</td>
						<td class="post">
							<asp:TextBox ID="Password" runat="server">testuser?</asp:TextBox>
							<asp:RequiredFieldValidator ID="PasswordRequiredfieldvalidator" runat="server" EnableClientScript="False"
								Text="0" ControlToValidate="Password" ErrorMessage="Users password is required."></asp:RequiredFieldValidator>
						</td>
					</tr>
					<tr>
						<td class="postheader">
							Confirm Password:
						</td>
						<td class="post">
							<asp:TextBox ID="Password2" runat="server">testuser?</asp:TextBox>
							<asp:CompareValidator ID="PasswordConfirmComparevalidator" runat="server" 
								EnableClientScript="False" ControlToValidate="Password2" ErrorMessage="Passwords didn't match."
								ControlToCompare="Password"></asp:CompareValidator>
						</td>
					</tr>
					<tr>
						<td class="postheader">
							Password Question(for all users):
						</td>
						<td class="post">
							<asp:TextBox ID="Question" runat="server">testuser?</asp:TextBox>
							<asp:RequiredFieldValidator ID="Requiredfieldvalidator3" runat="server" EnableClientScript="False"
								ControlToValidate="Question" ErrorMessage="Password Question is Required."></asp:RequiredFieldValidator>
						</td>
					</tr>
					<tr>
						<td class="postheader">
							Password Answer(for all users):
						</td>
						<td class="post">
							<asp:TextBox ID="Answer" runat="server">yes</asp:TextBox>
							<asp:RequiredFieldValidator ID="Requiredfieldvalidator4" runat="server" EnableClientScript="False"
								ControlToValidate="Answer" ErrorMessage="Password Answer is Required."></asp:RequiredFieldValidator>
						</td>
					</tr>
					<tr>
						<td class="postheader">
							Location(for all users):
						</td>
						<td class="post">
							<asp:TextBox ID="Location" runat="server">Earth</asp:TextBox>
						</td>
					</tr>
					<tr>
						<td class="postheader">
							Home Page(for all users):
						</td>
						<td class="post">
							<asp:TextBox ID="HomePage" runat="server"></asp:TextBox>
						</td>
					</tr>
					<tr>
						<td class="postheader">
							Time Zone(for all users):
						</td>
						<td class="post">
							<asp:DropDownList ID="TimeZones" runat="server" DataValueField="Value" DataTextField="Name" CssClass="standardSelectMenu">
							</asp:DropDownList>
						</td>
					</tr>								
					<tr>
						<td class="postheader" colspan="2">
							Warning! This is a test/debug feature. Never use it in production environment.
						</td>
					</tr>
				</table>
                </div>
                <div id="View2">
                  <table class="content" cellspacing="1" cellpadding="0" width="100%">
					<tr>
						<td class="header1" colspan="2">
							Create Test Boards
						</td>
					</tr>
					<tr>
						<td class="postheader" width="50%">
							Number of boards to create(max 10):
						</td>
						<td class="post">
							<asp:TextBox ID="BoardNumber" runat="server" CssClass="Numeric">0</asp:TextBox>
							<asp:RequiredFieldValidator ID="Requiredfieldvalidator10" runat="server" EnableClientScript="False"
								Text="0" ControlToValidate="BoardNumber" ErrorMessage="Boards's number is required."></asp:RequiredFieldValidator>
						</td>
					</tr>
					<tr>
						<td class="postheader" width="50%">
							Board Membership Name:
						</td>
						<td class="post">
							<asp:TextBox ID="BoardMembershipName" Enabled="false" runat="server"></asp:TextBox>
						</td>
					</tr>
					<tr>
						<td class="postheader" width="50%">
							Board Roles Name:
						</td>
						<td class="post">
							<asp:TextBox ID="BoardRolesName" Enabled="false" runat="server"></asp:TextBox>
						</td>
					</tr>
					<tr>
						<td class="postheader" width="50%">
							Number of Users in Each Board:
						</td>
						<td class="post">
							<asp:TextBox ID="BoardsUsersNumber" runat="server" Text="0" Enabled ="true" CssClass="Numeric"></asp:TextBox>
							<asp:RequiredFieldValidator ID="Requiredfieldvalidator12" runat="server" EnableClientScript="False"
								Text="0" ControlToValidate="BoardsUsersNumber" ErrorMessage="User's number is required."></asp:RequiredFieldValidator>
						</td>
					    </tr>
					
						<tr>
							<td class="postheader">
								Number of Categories in Each Board:
							</td>
							<td class="post">
								<asp:TextBox ID="BoardsCategoriesNumber" runat="server" Enabled="false"  datavaluefield="TopicID"
									datatextfield="Subject" CssClass="Numeric">0</asp:TextBox>
							</td>
						</tr>
						<tr>
							<td class="postheader">
								Number of Forums in Each Category:
							</td>
							<td class="post">
								<asp:TextBox ID="BoardsForumsNumber" runat="server" Enabled="false" datavaluefield="TopicID"
									datatextfield="Subject" CssClass="Numeric">0</asp:TextBox>
							</td>
						</tr>
						<tr>
							<td class="postheader">
								Number of Topics in Each Forum:
							</td>
							<td class="post">
								<asp:TextBox ID="BoardsTopicsNumber" runat="server" Enabled="false" datavaluefield="TopicID"
									datatextfield="Subject" CssClass="Numeric">0</asp:TextBox>
							</td>
						</tr>
						<tr>
							<td class="postheader">
								Number of Messages in Each Topic:
							</td>
							<td class="post">
								<asp:TextBox ID="BoardsMessagesNumber" runat="server" Enabled="false" datavaluefield="TopicID"
									datatextfield="Subject" CssClass="Numeric">0</asp:TextBox>
							</td>
						</tr>				
					<tr>
						<td class="postheader" colspan="2">
							Warning! This is a test/debug feature. Never use it in production environment.
						</td>
					</tr>
				</table>
                </div>
                <div id="View3">
                  <table class="content" cellspacing="1" cellpadding="0" width="100%">
					<tr>
						<td class="header1" colspan="2">
							Create Test Categories
						</td>
					</tr>
					<tr>
						<td class="postheader" width="50%">
							Number of categories to create(max 100):
						</td>
						<td class="post">
							<asp:TextBox ID="CategoriesNumber" runat="server" CssClass="Numeric">0</asp:TextBox>
							<asp:RequiredFieldValidator ID="Requiredfieldvalidator5" runat="server" EnableClientScript="False"
								Text="0" ControlToValidate="CategoriesNumber" ErrorMessage="Categories's number is required."></asp:RequiredFieldValidator>
						</td>
					</tr>
					<tr>
						<td class="postheader" width="50%">
							Create Categories:
						</td>
						<td class="post">
						<asp:RadioButtonList ID="CategoriesBoardsOptions"  Enabled="false"   AutoPostBack="true" runat="server"
							 	OnSelectedIndexChanged="CategoriesBoardsOptions_OnSelectedIndexChanged" />								
						</td>
					</tr>
					<tr>
						<td class="postheader">						
						</td>
						<td class="post">
							<asp:DropDownList ID="CategoriesBoardsList" runat="server"  Visible="false" DataValueField="BoardID" DataTextField="Name" CssClass="standardSelectMenu">
							</asp:DropDownList>
						</td>
					</tr>					
					<tr>
						<td class="postheader">
							Number of Forums in Each Category:
						</td>
						<td class="post">
							<asp:TextBox ID="CategoriesForumsNumber" runat="server" Enabled="true" CssClass="Numeric">0</asp:TextBox>
						</td>
					</tr>
					<tr>
						<td class="postheader">
							Number of Topics in Each Forum:
						</td>
						<td class="post">
							<asp:TextBox ID="CategoriesTopicsNumber" runat="server" Enabled="true" datavaluefield="TopicID"
								datatextfield="Subject" CssClass="Numeric">0</asp:TextBox>
						</td>
					</tr>
					<tr>
						<td class="postheader">
							Number of Messages in Each Topic:
						</td>
						<td class="post">
							<asp:TextBox ID="CategoriesMessagesNumber" runat="server" Enabled="true" datavaluefield="TopicID"
								datatextfield="Subject" CssClass="Numeric">0</asp:TextBox>
						</td>
					</tr>			
					<tr>			
						<td class="postheader" colspan="2">
							Warning! This is a test/debug feature. Never use it in production environment.
						</td>
					</tr>
				</table>
                </div>
                <div id="View4">
                   <table class="content" cellspacing="1" cellpadding="0" width="100%">
					<tr>
						<td class="header1" colspan="2">
							Create Test Forums
						</td>
					</tr>
					<tr>
						<td class="postheader" width="50%">
							Number of forums to create:
						</td>
						<td class="post">
							<asp:TextBox ID="ForumsNumber" runat="server" CssClass="Numeric">0</asp:TextBox>
							<asp:RequiredFieldValidator ID="Requiredfieldvalidator6" runat="server" EnableClientScript="False"
								Text="0" ControlToValidate="ForumsNumber" ErrorMessage="Forums' number is required."></asp:RequiredFieldValidator>
						</td>
					</tr>
					<tr>
						<td class="postheader" width="50%">
							In Current Board:
						</td>
						<td class="post">
							<asp:CheckBox ID="CheckBox3" Enabled="false" runat="server"
								Checked="true" />
						</td>
					</tr>
					<tr>
						<td class="postheader" width="50%">
							Count topics and messages in statistics:
						</td>
						<td class="post">
							<asp:CheckBox ID="ForumsCountMessages" runat="server" Enabled="true" Checked="true" />
						</td>
					</tr>
					<tr>
						<td class="postheader" width="50%">
							Hide forums if no access:
						</td>
						<td class="post">
							<asp:CheckBox ID="ForumsHideNoAccess" runat="server" Enabled="true" Checked="true" />
						</td>
					</tr>
					<tr>
						<td class="postheader">
							Select common access mask for all groups:
						</td>
						<td class="post">
							<asp:DropDownList ID="ForumsStartMask" runat="server" DataValueField="AccessMaskID"
								DataTextField="Name" OnSelectedIndexChanged="ForumsCategory_OnSelectedIndexChanged" CssClass="standardSelectMenu">
							</asp:DropDownList>
						</td>
					</tr>
					<tr>
						<td class="postheader">
							Select a group with special rights for the test data:
						</td>
						<td class="post">
							<asp:DropDownList ID="ForumsGroups" runat="server" DataValueField="GroupID" DataTextField="Name"
								OnSelectedIndexChanged="ForumsCategory_OnSelectedIndexChanged" CssClass="standardSelectMenu">
							</asp:DropDownList>
						</td>
					</tr>
					<tr>
						<td class="postheader">
							Select an access mask with special rights for the test data:
						</td>
						<td class="post">
							<asp:DropDownList ID="ForumsAdminMask" runat="server" DataValueField="AccessMaskID"
								DataTextField="Name" OnSelectedIndexChanged="ForumsCategory_OnSelectedIndexChanged" CssClass="standardSelectMenu">
							</asp:DropDownList>
						</td>
					</tr>
					<tr>
						<td class="postheader">
							Choose category:
						</td>
						<td class="post">
							<asp:DropDownList ID="ForumsCategory" runat="server" AutoPostBack="true" DataValueField="CategoryID"
								DataTextField="Name" OnDataBound="ForumsCategory_OnSelectedIndexChanged" OnSelectedIndexChanged="ForumsCategory_OnSelectedIndexChanged" CssClass="standardSelectMenu">
							</asp:DropDownList>
						</td>
					</tr>
					<tr>
						<td class="postheader">
							Choose parent forum:
						</td>
						<td class="post">
							<asp:DropDownList ID="ForumsParent" runat="server" DataValueField="ForumID" DataTextField="Title" CssClass="standardSelectMenu">
							</asp:DropDownList>
						</td>
					</tr>
					<tr>
						<td class="postheader">
							Number of Topics in Each Forum:
						</td>
						<td class="post">
							<asp:TextBox ID="ForumsTopicsNumber" runat="server" Enabled="true" datavaluefield="TopicID"
								datatextfield="Subject" CssClass="Numeric">0</asp:TextBox>
						</td>
					</tr>
					<tr>
						<td class="postheader">
							Number of Messages in Each Topic:
						</td>
						<td class="post">
							<asp:TextBox ID="ForumsMessagesNumber" runat="server" Enabled="true" datavaluefield="TopicID"
								datatextfield="Subject" CssClass="Numeric">0</asp:TextBox>
						</td>
					</tr>								
					<tr>
						<td class="postheader" colspan="2">
							Warning! This is a test/debug feature. Never use it in production environment.
						</td>
					</tr>
				</table>
                </div>
                <div id="View5">
                  <table class="content" cellspacing="1" cellpadding="0" width="100%">
					<tr>
						<td class="header1" colspan="2">
							Create Test Topics
						</td>
					</tr>
					<tr>
						<td class="postheader" width="50%">
							Number of topics to create:
						</td>
						<td class="post">
							<asp:TextBox ID="TopicsNumber" runat="server" CssClass="Numeric">0</asp:TextBox>
							<asp:RequiredFieldValidator ID="Requiredfieldvalidator7" runat="server" EnableClientScript="False"
								Text="0" ControlToValidate="TopicsNumber" ErrorMessage="Categories' number is required."></asp:RequiredFieldValidator>
						</td>
					</tr>
					<tr>
						<td class="postheader" width="50%">
							In Current Board:
						</td>
						<td class="post">
							<asp:CheckBox ID="CheckBox5" Enabled="false" runat="server"
								Checked="true" />
						</td>
					</tr>
					<tr>
						<td class="post" width="80%">
							<asp:DropDownList ID="TopicsPriorityList" runat="server" CssClass="standardSelectMenu" />
						</td>
					</tr>
					<tr>
						<td class="postheader" width="50%">
							Create Poll:
						</td>
						<td class="post">
							<asp:CheckBox ID="PollCreate" runat="server" Enabled="true" Checked="false" />
						</td>
					</tr>
					<tr>
						<td class="postheader">
							Choose category:
						</td>
						<td class="post">
							<asp:DropDownList ID="TopicsCategory" runat="server" AutoPostBack="true" DataValueField="CategoryID"
								DataTextField="Name" OnDataBound="TopicsCategory_OnSelectedIndexChanged" OnSelectedIndexChanged="TopicsCategory_OnSelectedIndexChanged" CssClass="standardSelectMenu">
							</asp:DropDownList>
						</td>
					</tr>
					<tr>
						<td class="postheader">
							Choose forum:
						</td>
						<td class="post">
							<asp:DropDownList ID="TopicsForum" runat="server" AutoPostBack="false" DataValueField="ForumID"
								DataTextField="Title" CssClass="standardSelectMenu">
							</asp:DropDownList>
						</td>
					</tr>
					<tr>
						<td class="postheader">
							Number of Messages in Each Topic:
						</td>
						<td class="post">
							<asp:TextBox ID="TopicsMessagesNumber" runat="server" Enabled="true" datavaluefield="TopicID"
								datatextfield="Subject" CssClass="Numeric">0</asp:TextBox>
						</td>
					</tr>			
					<tr>
						<td class="postheader" colspan="2">
							Warning! This is a test/debug feature. Never use it in production environment.
						</td>
					</tr>
				</table>
                </div>
                <div id="View6">
                  <table class="content" cellspacing="1" cellpadding="0" width="100%">
					<tr>
						<td class="header1" colspan="2">
							Create Test Messages
						</td>
					</tr>
					<tr>
						<td class="postheader" width="50%">
							Number of messages to create:
						</td>
						<td class="post">
							<asp:TextBox ID="PostsNumber" runat="server" CssClass="Numeric">0</asp:TextBox>
							<asp:RequiredFieldValidator ID="Requiredfieldvalidator8" runat="server" EnableClientScript="False"
								Text="0" ControlToValidate="PostsNumber" ErrorMessage="Categories' number is required."></asp:RequiredFieldValidator>
						</td>
					</tr>
					<tr>
						<td class="postheader" width="50%">
							In Current Board:
						</td>
						<td class="post">
							<asp:CheckBox ID="CheckBox7" Enabled="false" runat="server"
								Checked="true" />
						</td>
					</tr>
					<tr>
						<td class="postheader">
							Choose category:
						</td>
						<td class="post">
							<asp:DropDownList ID="PostsCategory" runat="server" AutoPostBack="true" DataValueField="CategoryID"
								DataTextField="Name" OnDataBound="PostsCategory_OnSelectedIndexChanged" OnSelectedIndexChanged="PostsCategory_OnSelectedIndexChanged" CssClass="standardSelectMenu">
							</asp:DropDownList>
						</td>
					</tr>
					<tr>
						<td class="postheader">
							Choose forum:
						</td>
						<td class="post">
							<asp:DropDownList ID="PostsForum" runat="server" DataValueField="ForumID" AutoPostBack="true"
								DataTextField="Title" OnSelectedIndexChanged="PostsForum_OnSelectedIndexChanged" CssClass="standardSelectMenu">
							</asp:DropDownList>
						</td>
					</tr>
					<tr>
						<td class="postheader">
							Choose topic:
						</td>
						<td class="post">
							<asp:DropDownList ID="PostsTopic" runat="server" DataValueField="TopicID" DataTextField="Subject" CssClass="standardSelectMenu">
							</asp:DropDownList>
						</td>
					</tr>
					<tr>
						<td class="postheader" width="50%">
							Enter your custom message for each test message:
						</td>
						<td class="post">
							<asp:TextBox ID="MyMessage" runat="server" TextMode="MultiLine" style="width:240px;height:auto"  ></asp:TextBox>							
						</td>
					</tr>		
					<tr>
						<td class="postheader" colspan="2">
							Warning! This is a test/debug feature. Never use it in production environment.
						</td>
					</tr>
				</table>
                </div>
                <div id="View7">
                  <table class="content" cellspacing="1" cellpadding="0" width="100%">
					<tr>
						<td class="header1" colspan="2">
							Create Test Private Messages
						</td>
					</tr>
					<tr>
						<td class="postheader" width="50%">
							Number of pmessages to create:
						</td>
						<td class="post">
							<asp:TextBox ID="PMessagesNumber" runat="server" CssClass="Numeric">0</asp:TextBox>
							<asp:RequiredFieldValidator ID="Requiredfieldvalidator9" runat="server" EnableClientScript="False"
								Text="0" ControlToValidate="PMessagesNumber" ErrorMessage="Pmessages's number is required."></asp:RequiredFieldValidator>
						</td>
					</tr>
	                <tr>
						<td class="postheader" width="50%">
							Create Messages:
						</td>
						<td class="post">
						<asp:RadioButtonList ID="PMessagesBoardsOptions"  Enabled="false"   AutoPostBack="true" runat="server"
							 	OnSelectedIndexChanged="PMessagesBoardsOptions_OnSelectedIndexChanged" />								
						</td>
					</tr>
					<tr>
						<td class="postheader">						
						</td>
						<td class="post">
							<asp:DropDownList ID="PMessagesBoardsList" runat="server"   DataValueField="BoardID" Enabled="false" DataTextField="Name" CssClass="standardSelectMenu">
							</asp:DropDownList>
						</td>
					</tr>						
					<tr>
						<td class="postheader" width="50%">
							From User:
						</td>
						<td class="post">
							<asp:TextBox ID="From" runat="server"></asp:TextBox>
							<asp:RequiredFieldValidator ID="Requiredfieldvalidator2" runat="server" EnableClientScript="False"
								Text="0" ControlToValidate="From" ErrorMessage="User Name is Required."></asp:RequiredFieldValidator>
						</td>
					</tr>
					<tr>
						<td class="postheader" width="50%">
							To User:
						</td>
						<td class="post">
							<asp:TextBox ID="To" runat="server"></asp:TextBox>
							<asp:RequiredFieldValidator ID="Requiredfieldvalidator11" runat="server" EnableClientScript="False"
								Text="0" ControlToValidate="To" ErrorMessage="User Name is Required."></asp:RequiredFieldValidator>
						</td>
					</tr>
					<tr>
						<td class="postheader" width="50%">
							Send To All:
						</td>
						<td class="post">
							<asp:CheckBox ID="PMessagesToAll" runat="server" Enabled="false" Checked="false" />
						</td>
					</tr>
					<tr>
						<td class="postheader" width="50%">
							Mark All Private Messages As Read:
						</td>
						<td class="post">
							<asp:CheckBox ID="MarkRead" runat="server" Enabled="true" Checked="true" />
						</td>
					</tr>
					<tr>
						<td class="postheader" width="50%">
							Enter your custom message for each test private message:
						</td>
						<td class="post">
							<asp:TextBox ID="PMessageText" runat="server" TextMode="MultiLine" style="width:280px;height:auto"  ></asp:TextBox>							
						</td>
					</tr>							
					<tr>
						<td class="postheader" colspan="2">
							Warning! This is a test/debug feature. Never use it in production environment.
						</td>
					</tr>
				</table>
                </div>
                <div id="View8">
                  <table class="content" cellspacing="1" cellpadding="0" width="100%">
					<tr>
						<td class="header1" colspan="2">
							Usage guide
						</td>
					</tr>
					<tr>
						<td class="post" align="center" colspan="2">						 
							<p>Test data generator is a utility to test Yet Another Forum performance.</p>
							<br />
							The operations take a lot of time, if you generate whales of data.
							All the time browser page will look like something hangs. On completiing you will see notification window.
							If you want to generate hundreds thousands records it can take,
							hours or even days.
							<p><strong>never</strong> use it in production enviroment as you can't delete data using the test data generator.</p>							
							<br />
							
						</td>
					</tr>
					<tr>
						<td class="header1" colspan="2">
							Generator Settings
						</td>
					</tr>
					<tr>
						<td class="postheader" width="50%">
							User Prefix(max 92):
						</td>
						<td class="post">
							<asp:TextBox ID="UserPrefixTB" runat="server">brd-</asp:TextBox>
							<asp:RequiredFieldValidator ID="UserPrefixTBRequiredfieldvalidator" runat="server" EnableClientScript="False"
								Text="0" ControlToValidate="UserPrefixTB" ErrorMessage="A user Prefix is required."></asp:RequiredFieldValidator>
						</td>
					</tr>
					<tr>
						<td class="postheader" width="50%">
							Board Prefix(max 12):
						</td>
						<td class="post">
							<asp:TextBox ID="BoardPrefixTB" runat="server">brd-</asp:TextBox>
							<asp:RequiredFieldValidator ID="BoardPrefixTBRequiredfieldvalidator" runat="server" EnableClientScript="False"
								Text="0" ControlToValidate="BoardPrefixTB" ErrorMessage="A board Prefix is required."></asp:RequiredFieldValidator>
						</td>
					</tr>
									<tr>
						<td class="postheader" width="50%">
							Category Prefix(max 12):
						</td>
						<td class="post">
							<asp:TextBox ID="CategoryPrefixTB" runat="server">cat-</asp:TextBox>
							<asp:RequiredFieldValidator ID="CategoryPrefixTBRequiredfieldvalidator" runat="server" EnableClientScript="False"
								Text="cat-" ControlToValidate="CategoryPrefixTB" ErrorMessage="A Category Prefix is required."></asp:RequiredFieldValidator>
						</td>
					</tr>
							<tr>
						<td class="postheader" width="50%">
							Forum Prefix(max 12):
						</td>
						<td class="post">
							<asp:TextBox ID="ForumPrefixTB" runat="server">frm-</asp:TextBox>
							<asp:RequiredFieldValidator ID="ForumPrefixTBRequiredfieldvalidator" runat="server" EnableClientScript="False"
								Text="frm-" ControlToValidate="ForumPrefixTB" ErrorMessage="A Forum Prefix is required."></asp:RequiredFieldValidator>
						</td>
					</tr>
							<tr>
						<td class="postheader" width="50%">
							Topic Name Prefix(max 92):
						</td>
						<td class="post">
							<asp:TextBox ID="TopicPrefixTB" runat="server">topic-</asp:TextBox>
							<asp:RequiredFieldValidator ID="TopicPrefixTBRequiredfieldvalidator" runat="server" EnableClientScript="False"
								Text="topic-" ControlToValidate="TopicPrefixTB" ErrorMessage="A Topic Prefix is required."></asp:RequiredFieldValidator>
						</td>
					</tr>						
						<tr>
						<td class="postheader" width="50%">
							Message Content Prefix:
						</td>
						<td class="post">
							<asp:TextBox ID="MessageContentPrefixTB" runat="server">msg-</asp:TextBox>
							<asp:RequiredFieldValidator ID="MessageContentPrefixTBRequiredfieldvalidator" runat="server" EnableClientScript="False"
								Text="msg-" ControlToValidate="MessageContentPrefixTB" ErrorMessage="A Messsage Content Prefix is required."></asp:RequiredFieldValidator>
						</td>
					</tr>			
				</table>
                </div>
             </asp:Panel>
    <asp:HiddenField runat="server" ID="hidLastTab" Value="0" />
    <asp:HiddenField runat="server" ID="hidLastTabId" Value="0" />
	<table class="content" cellspacing="1" cellpadding="0" width="100%">
		<tr>
			<td class="postfooter" align="center">
				<asp:Button ID="LaunchGenerator" runat="server" Text="Launch Generator" CssClass="pbutton"
					OnClick="CreateTestData_Click"></asp:Button>
				<asp:Button ID="Cancel" runat="server" Text="Cancel" CssClass="pbutton" OnClick="Cancel_Click"></asp:Button>
			</td>
		</tr>
	</table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
