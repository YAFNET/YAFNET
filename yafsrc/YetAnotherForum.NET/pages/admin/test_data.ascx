<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.test_data" Codebehind="test_data.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server" ID="Adminmenu1">
<div class="row">
    <div class="col-xl-12">
        <h1>Create Test Data</h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-cog fa-fw"></i>&nbsp;Create Test
                                    </div>
                <div class="card-body">
	<asp:Panel id="TestDataTabs" runat="server">
               <ul class="nav nav-tabs" role="tablist">
                 <li class="nav-item"><a href="#View1" class="nav-link" data-toggle="tab" role="tab">Users</a></li>
		 <li class="nav-item"><a href="#View2" class="nav-link" data-toggle="tab" role="tab">Boards</a></li>
		 <li class="nav-item"><a href="#View3" class="nav-link" data-toggle="tab" role="tab">Categories</a></li>
		 <li class="nav-item"><a href="#View4" class="nav-link" data-toggle="tab" role="tab">Forums</a></li>
		 <li class="nav-item"><a href="#View5" class="nav-link" data-toggle="tab" role="tab">Topics</a></li>		        
		 <li class="nav-item"><a href="#View6" class="nav-link" data-toggle="tab" role="tab">Messages</a></li>	
		 <li class="nav-item"><a href="#View7" class="nav-link" data-toggle="tab" role="tab">PMessages</a></li>
                 <li class="nav-item"><a href="#View8" class="nav-link" data-toggle="tab" role="tab">Settings/Help</a></li>
               </ul>
        <div class="tab-content">
                <div id="View1" class="tab-pane" role="tabpanel">
                   
					
						<h2>
							Create Test Users
						</h2>
					    <hr/>
					
						<h4>
							Number of users to create:
						</h4>
						<p>
							<asp:TextBox ID="UsersNumber" runat="server" CssClass="form-control" TextMode="Number">0</asp:TextBox>
							<asp:RequiredFieldValidator ID="Requiredfieldvalidator1" runat="server" EnableClientScript="False"
								Text="0" ControlToValidate="UsersNumber" ErrorMessage="User's number is required."></asp:RequiredFieldValidator>
						</p>
						<hr/>			
					
						<h4>
							Create Users:
						</h4>
						<p>
						<asp:RadioButtonList ID="UsersBoardsOptions"  Enabled="true"   AutoPostBack="true" runat="server"
							 	OnSelectedIndexChanged="UsersBoardsOptions_OnSelectedIndexChanged" />								
						</p><hr/>
					
					
						<p>
							<asp:DropDownList ID="UsersBoardsList" runat="server"  Visible="false" DataValueField="BoardID" DataTextField="Name" CssClass="form-control">
							</asp:DropDownList>
						</p><hr/>
					
					
						<h4>
							Password(for all users):
						</h4>
						<p>
							<asp:TextBox CssClass="form-control" ID="Password" runat="server">testuser?</asp:TextBox>
							<asp:RequiredFieldValidator ID="PasswordRequiredfieldvalidator" runat="server" EnableClientScript="False"
								Text="0" ControlToValidate="Password" ErrorMessage="Users password is required."></asp:RequiredFieldValidator>
						</p><hr/>
					
					
						<h4>
							Confirm Password:
						</h4>
						<p>
							<asp:TextBox CssClass="form-control" ID="Password2" runat="server">testuser?</asp:TextBox>
							<asp:CompareValidator ID="PasswordConfirmComparevalidator" runat="server" 
								EnableClientScript="False" ControlToValidate="Password2" ErrorMessage="Passwords didn't match."
								ControlToCompare="Password"></asp:CompareValidator>
						</p><hr/>
					
					
						<h4>
							Password Question(for all users):
						</h4>
						<p>
							<asp:TextBox CssClass="form-control" ID="Question" runat="server">testuser?</asp:TextBox>
							<asp:RequiredFieldValidator ID="Requiredfieldvalidator3" runat="server" EnableClientScript="False"
								ControlToValidate="Question" ErrorMessage="Password Question is Required."></asp:RequiredFieldValidator>
						</p><hr/>
					
					
						<h4>
							Password Answer(for all users):
						</h4>
						<p>
							<asp:TextBox CssClass="form-control" ID="Answer" runat="server">yes</asp:TextBox>
							<asp:RequiredFieldValidator ID="Requiredfieldvalidator4" runat="server" EnableClientScript="False"
								ControlToValidate="Answer" ErrorMessage="Password Answer is Required."></asp:RequiredFieldValidator>
						</p><hr/>
					
					
						<h4>
							Location(for all users):
						</h4>
						<p>
							<asp:TextBox CssClass="form-control" ID="Location" runat="server">Earth</asp:TextBox>
						</p><hr/>
					
					
						<h4>
							Home Page(for all users):
						</h4>
						<p>
							<asp:TextBox CssClass="form-control" ID="HomePage" runat="server"></asp:TextBox>
						</p><hr/>
					
					
						<h4>
							Time Zone(for all users):
						</h4>
						<p>
							<asp:DropDownList ID="TimeZones" runat="server" DataValueField="Value" DataTextField="Name" CssClass="form-control">
							</asp:DropDownList>
						</p>
													
					
						<div class="alert alert-danger" role="alert"> 
							Warning! This is a test/debug feature. Never use it in production environment.
						</div>
                </div>
                <div id="View2" class="tab-pane" role="tabpanel">
                  
					
						<h2>
							Create Test Boards
						</h2>
					
					
						<h4>
							Number of boards to create(max 10):
						</h4>
						<p>
							<asp:TextBox ID="BoardNumber" runat="server" CssClass="form-control" TextMode="Number">0</asp:TextBox>
							<asp:RequiredFieldValidator ID="Requiredfieldvalidator10" runat="server" EnableClientScript="False"
								Text="0" ControlToValidate="BoardNumber" ErrorMessage="Boards's number is required."></asp:RequiredFieldValidator>
						</p><hr/>
				
						<h4>
							Board Membership Name:
						</h4>
						<p>
							<asp:TextBox CssClass="form-control" ID="BoardMembershipName" Enabled="false" runat="server"></asp:TextBox>
						</p><hr/>
				
						<h4>
							Board Roles Name:
						</h4>
						<p>
							<asp:TextBox CssClass="form-control" ID="BoardRolesName" Enabled="false" runat="server"></asp:TextBox>
						</p><hr/>
					
					
						<h4>
							Number of Users in Each Board:
						</h4>
						<p>
							<asp:TextBox ID="BoardsUsersNumber" runat="server" Text="0" Enabled ="true" CssClass="form-control" TextMode="Number"></asp:TextBox>
							<asp:RequiredFieldValidator ID="Requiredfieldvalidator12" runat="server" EnableClientScript="False"
								Text="0" ControlToValidate="BoardsUsersNumber" ErrorMessage="User's number is required."></asp:RequiredFieldValidator>
						</p><hr/>
							<h4>
								Number of Categories in Each Board:
							</h4>
							<p>
								<asp:TextBox ID="BoardsCategoriesNumber" runat="server" Enabled="false"  datavaluefield="TopicID"
									datatextfield="Subject" CssClass="form-control" TextMode="Number">0</asp:TextBox>
							</p><hr/>
						
						
							<h4>
								Number of Forums in Each Category:
							</h4>
							<p>
								<asp:TextBox ID="BoardsForumsNumber" runat="server" Enabled="false" datavaluefield="TopicID"
									datatextfield="Subject" CssClass="form-control" TextMode="Number">0</asp:TextBox>
							</p><hr/>
							<h4>
								Number of Topics in Each Forum:
							</h4>
							<p>
								<asp:TextBox ID="BoardsTopicsNumber" runat="server" Enabled="false" datavaluefield="TopicID"
									datatextfield="Subject" CssClass="form-control" TextMode="Number">0</asp:TextBox>
							</p><hr/>
						
						
							<h4>
								Number of Messages in Each Topic:
							</h4>
							<p>
								<asp:TextBox ID="BoardsMessagesNumber" runat="server" Enabled="false" datavaluefield="TopicID"
									datatextfield="Subject" CssClass="form-control" TextMode="Number">0</asp:TextBox>
							</p><hr/>
										
					
						<div class="alert alert-danger" role="alert"> 
							Warning! This is a test/debug feature. Never use it in production environment.
						</div>
				
                </div>
                <div id="View3" class="tab-pane" role="tabpanel">
                  
					
						<h2>
							Create Test Categories
						</h2>
					
					
						<h4>
							Number of categories to create(max 100):
						</h4>
						<p>
							<asp:TextBox ID="CategoriesNumber" runat="server" CssClass="form-control" TextMode="Number">0</asp:TextBox>
							<asp:RequiredFieldValidator ID="Requiredfieldvalidator5" runat="server" EnableClientScript="False"
								Text="0" ControlToValidate="CategoriesNumber" ErrorMessage="Categories's number is required."></asp:RequiredFieldValidator>
						</p><hr/>
					
					
						<h4>
							Create Categories:
						</h4>
						<p>
						<asp:RadioButtonList CssClass="form-control" ID="CategoriesBoardsOptions"  Enabled="false"   AutoPostBack="true" runat="server"
							 	OnSelectedIndexChanged="CategoriesBoardsOptions_OnSelectedIndexChanged" />								
						</p><hr/>
					
					
						<p>
							<asp:DropDownList ID="CategoriesBoardsList" runat="server"  Visible="false" DataValueField="BoardID" DataTextField="Name" CssClass="form-control">
							</asp:DropDownList>
						</p><hr/>
										
					
						<h4>
							Number of Forums in Each Category:
						</h4>
						<p>
							<asp:TextBox ID="CategoriesForumsNumber" runat="server" Enabled="true" CssClass="form-control" TextMode="Number">0</asp:TextBox>
						</p><hr/>
					
					
						<h4>
							Number of Topics in Each Forum:
						</h4>
						<p>
							<asp:TextBox ID="CategoriesTopicsNumber" runat="server" Enabled="true" datavaluefield="TopicID"
								datatextfield="Subject" CssClass="form-control" TextMode="Number">0</asp:TextBox>
						</p><hr/>
					
					
						<h4>
							Number of Messages in Each Topic:
						</h4>
						<p>
							<asp:TextBox ID="CategoriesMessagesNumber" runat="server" Enabled="true" datavaluefield="TopicID"
								datatextfield="Subject" CssClass="form-control" TextMode="Number">0</asp:TextBox>
						</p><hr/>
								
								
						<div class="alert alert-danger" role="alert"> 
							Warning! This is a test/debug feature. Never use it in production environment.
						</div>
					
				
                </div>
                <div id="View4" class="tab-pane" role="tabpanel">
                   
					
						<h2>
							Create Test Forums
						</h2>
					
					
						<h4>
							Number of forums to create:
						</h4>
						<p>
							<asp:TextBox ID="ForumsNumber" runat="server" CssClass="form-control" TextMode="Number">0</asp:TextBox>
							<asp:RequiredFieldValidator ID="Requiredfieldvalidator6" runat="server" EnableClientScript="False"
								Text="0" ControlToValidate="ForumsNumber" ErrorMessage="Forums' number is required."></asp:RequiredFieldValidator>
						</p><hr/>
					
					
						<h4>
							In Current Board:
						</h4>
						<p>
							<asp:CheckBox CssClass="form-control" ID="CheckBox3" Enabled="false" runat="server"
								Checked="true" />
						</p><hr/>
					
					
						<h4>
							Count topics and messages in statistics:
						</h4>
						<p>
							<asp:CheckBox CssClass="form-control" ID="ForumsCountMessages" runat="server" Enabled="true" Checked="true" />
						</p><hr/>
					
					
						<h4>
							Hide forums if no access:
						</h4>
						<p>
							<asp:CheckBox CssClass="form-control" ID="ForumsHideNoAccess" runat="server" Enabled="true" Checked="true" />
						</p><hr/>
					
					
						<h4>
							Select common access mask for all groups:
						</h4>
						<p>
							<asp:DropDownList ID="ForumsStartMask" runat="server" DataValueField="AccessMaskID"
								DataTextField="Name" OnSelectedIndexChanged="ForumsCategory_OnSelectedIndexChanged" CssClass="form-control">
							</asp:DropDownList>
						</p><hr/>
					
					
						<h4>
							Select a group with special rights for the test data:
						</h4>
						<p>
							<asp:DropDownList ID="ForumsGroups" runat="server" DataValueField="GroupID" DataTextField="Name"
								OnSelectedIndexChanged="ForumsCategory_OnSelectedIndexChanged" CssClass="form-control">
							</asp:DropDownList>
						</p><hr/>
					
					
						<h4>
							Select an access mask with special rights for the test data:
						</h4>
						<p>
							<asp:DropDownList ID="ForumsAdminMask" runat="server" DataValueField="AccessMaskID"
								DataTextField="Name" OnSelectedIndexChanged="ForumsCategory_OnSelectedIndexChanged" CssClass="form-control">
							</asp:DropDownList>
						</p><hr/>
					
					
						<h4>
							Choose category:
						</h4>
						<p>
							<asp:DropDownList ID="ForumsCategory" runat="server" AutoPostBack="true" DataValueField="CategoryID"
								DataTextField="Name" OnDataBound="ForumsCategory_OnSelectedIndexChanged" OnSelectedIndexChanged="ForumsCategory_OnSelectedIndexChanged" CssClass="form-control">
							</asp:DropDownList>
						</p><hr/>
					
					
						<h4>
							Choose parent forum:
						</h4>
						<p>
							<asp:DropDownList ID="ForumsParent" runat="server" DataValueField="ForumID" DataTextField="Title" CssClass="form-control">
							</asp:DropDownList>
						</p><hr/>
					
					
						<h4>
							Number of Topics in Each Forum:
						</h4>
						<p>
							<asp:TextBox ID="ForumsTopicsNumber" runat="server" Enabled="true" datavaluefield="TopicID"
								datatextfield="Subject" CssClass="form-control" TextMode="Number">0</asp:TextBox>
						</p><hr/>
					
					
						<h4>
							Number of Messages in Each Topic:
						</h4>
						<p>
							<asp:TextBox ID="ForumsMessagesNumber" runat="server" Enabled="true" datavaluefield="TopicID"
								datatextfield="Subject" CssClass="form-control" TextMode="Number">0</asp:TextBox>
						</p><hr/>
													
					
						<div class="alert alert-danger" role="alert"> 
							Warning! This is a test/debug feature. Never use it in production environment.
						</div>
					
				
                </div>
                <div id="View5" class="tab-pane" role="tabpanel">
                  
					
						<h2>
							Create Test Topics
						</h2>
					
					
						<h4>
							Number of topics to create:
						</h4>
						<p>
							<asp:TextBox ID="TopicsNumber" runat="server" CssClass="form-control" TextMode="Number">0</asp:TextBox>
							<asp:RequiredFieldValidator ID="Requiredfieldvalidator7" runat="server" EnableClientScript="False"
								Text="0" ControlToValidate="TopicsNumber" ErrorMessage="Categories' number is required."></asp:RequiredFieldValidator>
						</p><hr/>
					
					
						<h4>
							In Current Board:
						</h4>
						<p>
							<asp:CheckBox CssClass="form-control" ID="CheckBox5" Enabled="false" runat="server"
								Checked="true" />
						</p><hr/>
					
					
						<p>
							<asp:DropDownList ID="TopicsPriorityList" runat="server" CssClass="form-control" />
						</p><hr/>
					
					
						<h4>
							Create Poll:
						</h4>
						<p>
							<asp:CheckBox CssClass="form-control" ID="PollCreate" runat="server" Enabled="true" Checked="false" />
						</p><hr/>
					
					
						<h4>
							Choose category:
						</h4>
						<p>
							<asp:DropDownList ID="TopicsCategory" runat="server" AutoPostBack="true" DataValueField="CategoryID"
								DataTextField="Name" OnDataBound="TopicsCategory_OnSelectedIndexChanged" OnSelectedIndexChanged="TopicsCategory_OnSelectedIndexChanged" CssClass="form-control">
							</asp:DropDownList>
						</p><hr/>
					
					
						<h4>
							Choose forum:
						</h4>
						<p>
							<asp:DropDownList ID="TopicsForum" runat="server" AutoPostBack="false" DataValueField="ForumID"
								DataTextField="Title" CssClass="form-control">
							</asp:DropDownList>
						</p><hr/>
					
					
						<h4>
							Number of Messages in Each Topic:
						</h4>
						<p>
							<asp:TextBox ID="TopicsMessagesNumber" runat="server" Enabled="true" datavaluefield="TopicID"
								datatextfield="Subject" CssClass="form-control" TextMode="Number">0</asp:TextBox>
						</p><hr/>
								
					
						<div class="alert alert-danger" role="alert"> 
							Warning! This is a test/debug feature. Never use it in production environment.
						</div>
					
				
                </div>
                <div id="View6" class="tab-pane" role="tabpanel">
                  
					
						<h2>
							Create Test Messages
						</h2>
					
					
						<h4>
							Number of messages to create:
						</h4>
						<p>
							<asp:TextBox ID="PostsNumber" runat="server" CssClass="form-control" TextMode="Number">0</asp:TextBox>
							<asp:RequiredFieldValidator ID="Requiredfieldvalidator8" runat="server" EnableClientScript="False"
								Text="0" ControlToValidate="PostsNumber" ErrorMessage="Categories' number is required."></asp:RequiredFieldValidator>
						</p><hr/>
					
					
						<h4>
							In Current Board:
						</h4>
						<p>
							<asp:CheckBox CssClass="form-control" ID="CheckBox7" Enabled="false" runat="server"
								Checked="true" />
						</p><hr/>
					
					
						<h4>
							Choose category:
						</h4>
						<p>
							<asp:DropDownList ID="PostsCategory" runat="server" AutoPostBack="true" DataValueField="CategoryID"
								DataTextField="Name" OnDataBound="PostsCategory_OnSelectedIndexChanged" OnSelectedIndexChanged="PostsCategory_OnSelectedIndexChanged" CssClass="form-control">
							</asp:DropDownList>
						</p><hr/>
					
					
						<h4>
							Choose forum:
						</h4>
						<p>
							<asp:DropDownList ID="PostsForum" runat="server" DataValueField="ForumID" AutoPostBack="true"
								DataTextField="Title" OnSelectedIndexChanged="PostsForum_OnSelectedIndexChanged" CssClass="form-control">
							</asp:DropDownList>
						</p><hr/>
					
					
						<h4>
							Choose topic:
						</h4>
						<p>
							<asp:DropDownList ID="PostsTopic" runat="server" DataValueField="TopicID" DataTextField="Subject" CssClass="form-control">
							</asp:DropDownList>
						</p><hr/>
					
					
						<h4>
							Enter your custom message for each test message:
						</h4>
						<p>
							<asp:TextBox CssClass="form-control" ID="MyMessage" runat="server" TextMode="MultiLine"  ></asp:TextBox>							
						</p><hr/>
							
					
						<div class="alert alert-danger" role="alert"> 
							Warning! This is a test/debug feature. Never use it in production environment.
						</div>
					
				
                </div>
                <div id="View7" class="tab-pane" role="tabpanel">
                  
					
						<h2>
							Create Test Private Messages
						</h2>
					
					
						<h4>
							Number of pmessages to create:
						</h4>
						<p>
							<asp:TextBox ID="PMessagesNumber" runat="server" CssClass="form-control" TextMode="Number">0</asp:TextBox>
							<asp:RequiredFieldValidator ID="Requiredfieldvalidator9" runat="server" EnableClientScript="False"
								Text="0" ControlToValidate="PMessagesNumber" ErrorMessage="Pmessages's number is required."></asp:RequiredFieldValidator>
						</p><hr/>
					
	                
						<h4>
							Create Messages:
						</h4>
						<p>
						<asp:RadioButtonList CssClass="form-control" ID="PMessagesBoardsOptions"  Enabled="false"   AutoPostBack="true" runat="server"
							 	OnSelectedIndexChanged="PMessagesBoardsOptions_OnSelectedIndexChanged" />								
						</p><hr/>
					
					
						<h4>						
						</h4>
						<p>
							<asp:DropDownList ID="PMessagesBoardsList" runat="server"   DataValueField="BoardID" Enabled="false" DataTextField="Name" CssClass="form-control">
							</asp:DropDownList>
						</p><hr/>
											
					
						<h4>
							From User:
						</h4>
						<p>
							<asp:TextBox CssClass="form-control" ID="From" runat="server"></asp:TextBox>
							<asp:RequiredFieldValidator ID="Requiredfieldvalidator2" runat="server" EnableClientScript="False"
								Text="0" ControlToValidate="From" ErrorMessage="User Name is Required."></asp:RequiredFieldValidator>
						</p><hr/>
					
					
						<h4>
							To User:
						</h4>
						<p>
							<asp:TextBox CssClass="form-control" ID="To" runat="server"></asp:TextBox>
							<asp:RequiredFieldValidator ID="Requiredfieldvalidator11" runat="server" EnableClientScript="False"
								Text="0" ControlToValidate="To" ErrorMessage="User Name is Required."></asp:RequiredFieldValidator>
						</p><hr/>
					
					
						<h4>
							Send To All:
						</h4>
						<p>
							<asp:CheckBox CssClass="form-control" ID="PMessagesToAll" runat="server" Enabled="false" Checked="false" />
						</p><hr/>
					
					
						<h4>
							Mark All Private Messages As Read:
						</h4>
						<p>
							<asp:CheckBox CssClass="form-control" ID="MarkRead" runat="server" Enabled="true" Checked="true" />
						</p><hr/>
					
					
						<h4>
							Enter your custom message for each test private message:
						</h4>
						<p>
							<asp:TextBox CssClass="form-control" ID="PMessageText" runat="server" TextMode="MultiLine"  ></asp:TextBox>							
						</p><hr/>
												
					
						<div class="alert alert-danger" role="alert"> 
							Warning! This is a test/debug feature. Never use it in production environment.
						</div>
                </div>
                <div id="View8" class="tab-pane" role="tabpanel">
                  
					
						<h2>
							Usage guide
						</h2>
                    <div class="alert alert-danger" role="alert"> 

						<p>Test data generator is a utility to test Yet Another Forum performance.</p>
						
							The operations take a lot of time, if you generate whales of data.
							All the time browser page will look like something hangs. On completing you will see notification window.
							If you want to generate hundreds thousands records it can take,
							hours or even days.
							<p>But <strong>never</strong> use it in production enviroment as you can't delete data using the test data generator.</p>							
					    </div>
                        <hr />			
						<h2>
							Generator Settings
						</h2>
					
					
						<h4>
							User Prefix(max 92):
						</h4>
						<p>
							<asp:TextBox CssClass="form-control" ID="UserPrefixTB" runat="server">brd-</asp:TextBox>
							<asp:RequiredFieldValidator ID="UserPrefixTBRequiredfieldvalidator" runat="server" EnableClientScript="False"
								Text="0" ControlToValidate="UserPrefixTB" ErrorMessage="A user Prefix is required."></asp:RequiredFieldValidator>
						</p><hr />
					
					
						<h4>
							Board Prefix(max 12):
						</h4>
						<p>
							<asp:TextBox CssClass="form-control" ID="BoardPrefixTB" runat="server">brd-</asp:TextBox>
							<asp:RequiredFieldValidator ID="BoardPrefixTBRequiredfieldvalidator" runat="server" EnableClientScript="False"
								Text="0" ControlToValidate="BoardPrefixTB" ErrorMessage="A board Prefix is required."></asp:RequiredFieldValidator>
						</p><hr />
					
									
						<h4>
							Category Prefix(max 12):
						</h4>
						<p>
							<asp:TextBox CssClass="form-control" ID="CategoryPrefixTB" runat="server">cat-</asp:TextBox>
							<asp:RequiredFieldValidator ID="CategoryPrefixTBRequiredfieldvalidator" runat="server" EnableClientScript="False"
								Text="cat-" ControlToValidate="CategoryPrefixTB" ErrorMessage="A Category Prefix is required."></asp:RequiredFieldValidator>
						</p><hr />
					
							
						<h4>
							Forum Prefix(max 12):
						</h4>
						<p>
							<asp:TextBox CssClass="form-control" ID="ForumPrefixTB" runat="server">frm-</asp:TextBox>
							<asp:RequiredFieldValidator ID="ForumPrefixTBRequiredfieldvalidator" runat="server" EnableClientScript="False"
								Text="frm-" ControlToValidate="ForumPrefixTB" ErrorMessage="A Forum Prefix is required."></asp:RequiredFieldValidator>
						</p><hr />
					
							
						<h4>
							Topic Name Prefix(max 92):
						</h4>
						<p>
							<asp:TextBox CssClass="form-control" ID="TopicPrefixTB" runat="server">topic-</asp:TextBox>
							<asp:RequiredFieldValidator ID="TopicPrefixTBRequiredfieldvalidator" runat="server" EnableClientScript="False"
								Text="topic-" ControlToValidate="TopicPrefixTB" ErrorMessage="A Topic Prefix is required."></asp:RequiredFieldValidator>
						</p><hr />
											
						
						<h4>
							Message Content Prefix:
						</h4>
						<p>
							<asp:TextBox CssClass="form-control" ID="MessageContentPrefixTB" runat="server">msg-</asp:TextBox>
							<asp:RequiredFieldValidator ID="MessageContentPrefixTBRequiredfieldvalidator" runat="server" EnableClientScript="False"
								Text="msg-" ControlToValidate="MessageContentPrefixTB" ErrorMessage="A Messsage Content Prefix is required."></asp:RequiredFieldValidator>
						</p><hr />
								
				
                </div>
            </div>
             </asp:Panel>
    <asp:HiddenField runat="server" ID="hidLastTab" Value="View1" />
</div>
                <div class="card-footer text-lg-center">
				<asp:Button ID="LaunchGenerator" runat="server" Text="Launch Generator" Type="Primary"
					OnClick="CreateTestData_Click"></asp:Button>&nbsp;
				<asp:Button ID="Cancel" runat="server" Text="Cancel" Type="Secondary" OnClick="Cancel_Click"></asp:Button>
                </div>
            </div>
        </div>
    </div>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
