<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.TestData"
    TargetSchema="http://schemas.microsoft.com/intellisense/ie5" Codebehind="TestData.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />


<div class="card">
<div class="card-body">
<asp:Panel id="TestDataTabs" runat="server">
<ul class="nav nav-tabs" role="tablist">
    <li class="nav-item">
        <a href="#View1" class="nav-link" data-bs-toggle="tab" role="tab">Users</a>
    </li>
    <li class="nav-item">
        <a href="#View2" class="nav-link" data-bs-toggle="tab" role="tab">Boards</a>
    </li>
    <li class="nav-item">
        <a href="#View3" class="nav-link" data-bs-toggle="tab" role="tab">Categories</a>
    </li>
    <li class="nav-item">
        <a href="#View4" class="nav-link" data-bs-toggle="tab" role="tab">Forums</a>
    </li>
    <li class="nav-item">
        <a href="#View5" class="nav-link" data-bs-toggle="tab" role="tab">Topics</a>
    </li>
    <li class="nav-item">
        <a href="#View6" class="nav-link" data-bs-toggle="tab" role="tab">Messages</a>
    </li>
    <li class="nav-item">
        <a href="#View7" class="nav-link" data-bs-toggle="tab" role="tab">Private Messages</a>
    </li>
    <li class="nav-item">
        <a href="#View8" class="nav-link" data-bs-toggle="tab" role="tab">Settings/Help</a>
    </li>
</ul>
<div class="tab-content">
<div id="View1" class="tab-pane" role="tabpanel">
    <h4>
        Create Test Users
    </h4>
    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="UsersNumber">
            Number of users to create:
        </asp:Label>
        <asp:TextBox ID="UsersNumber" runat="server"
                     CssClass="form-control"
                     TextMode="Number"
                     Text="0" />
        <asp:RequiredFieldValidator ID="Requiredfieldvalidator1" runat="server"
                                    EnableClientScript="False"
                                    Text="0"
                                    ControlToValidate="UsersNumber"
                                    ErrorMessage="User's number is required."></asp:RequiredFieldValidator>
    </div>

    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="UsersNumber">
            Select board to Create users in:
        </asp:Label>
        <asp:DropDownList ID="UsersBoardsList" runat="server"
                          DataValueField="ID" DataTextField="Name"
                          CssClass="select2-select">
        </asp:DropDownList>
    </div>

    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="Password">
            Password(for all users):
        </asp:Label>
        <asp:TextBox ID="Password" runat="server"
                     CssClass="form-control" Text="TestUser1234?" />
        <asp:RequiredFieldValidator ID="PasswordRequiredfieldvalidator" runat="server"
                                    EnableClientScript="False"
                                    Text="0"
                                    ControlToValidate="Password"
                                    ErrorMessage="Users password is required."></asp:RequiredFieldValidator>
    </div>

    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="Password2">
            Confirm Password:
        </asp:Label>
        <asp:TextBox ID="Password2" runat="server"
                     CssClass="form-control"
                     Text="TestUser1234?" />
        <asp:CompareValidator ID="PasswordConfirmComparevalidator" runat="server"
                              EnableClientScript="False"
                              ControlToValidate="Password2"
                              ErrorMessage="Passwords didn't match."
                              ControlToCompare="Password"></asp:CompareValidator>
    </div>

    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="Location">
            Location(for all users):
        </asp:Label>

        <asp:TextBox ID="Location" runat="server" CssClass="form-control">Earth</asp:TextBox>
    </div>

    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="HomePage">
            Home Page(for all users):
        </asp:Label>
        <asp:TextBox ID="HomePage" runat="server"
                     CssClass="form-control"></asp:TextBox>
    </div>

    <YAF:Alert runat="server" Type="danger">
        Warning! This is a test/debug feature. Never use it in production environment.
    </YAF:Alert>
</div>

<div id="View2" class="tab-pane" role="tabpanel">
    <h4>
        Create Test Boards
    </h4>

    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="BoardNumber">
            Number of boards to create(max 10):
        </asp:Label>
        <asp:TextBox ID="BoardNumber" runat="server"
                     CssClass="form-control"
                     TextMode="Number"
                     Text="0" />
        <asp:RequiredFieldValidator ID="Requiredfieldvalidator10" runat="server"
                                    EnableClientScript="False"
                                    Text="0"
                                    ControlToValidate="BoardNumber"
                                    ErrorMessage="Boards's number is required." />
    </div>

    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="BoardsUsersNumber">
            Number of Users in Each Board:
        </asp:Label>
        <asp:TextBox ID="BoardsUsersNumber" runat="server"
                     Text="0"
                     CssClass="form-control"
                     TextMode="Number" />
        <asp:RequiredFieldValidator ID="Requiredfieldvalidator12" runat="server"
                                    EnableClientScript="False"
                                    Text="0"
                                    ControlToValidate="BoardsUsersNumber"
                                    ErrorMessage="User's number is required." />
    </div>

    <YAF:Alert runat="server" Type="danger">
        Warning! This is a test/debug feature. Never use it in production environment.
    </YAF:Alert>
</div>
<div id="View3" class="tab-pane" role="tabpanel">
    <h4>
        Create Test Categories
    </h4>

    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="CategoriesNumber">
            Number of categories to create(max 100):
        </asp:Label>
        <asp:TextBox ID="CategoriesNumber" runat="server"
                     CssClass="form-control"
                     TextMode="Number"
                     Text="0" />
        <asp:RequiredFieldValidator ID="Requiredfieldvalidator5" runat="server"
                                    EnableClientScript="False"
                                    Text="0"
                                    ControlToValidate="CategoriesNumber"
                                    ErrorMessage="Categories's number is required." />
    </div>

    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="CategoriesBoardsList">
            Create Categories in Board:
        </asp:Label>
        <asp:DropDownList ID="CategoriesBoardsList" runat="server"
                          DataValueField="ID" DataTextField="Name"
                          CssClass="select2-select">
        </asp:DropDownList>
    </div>

    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="CategoriesForumsNumber">
            Number of Forums in Each Category:
        </asp:Label>
        <asp:TextBox ID="CategoriesForumsNumber" runat="server"  CssClass="form-control" TextMode="Number"
                     Text="0" />
    </div>

    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="CategoriesTopicsNumber">
            Number of Topics in Each Forum:
        </asp:Label>
        <asp:TextBox ID="CategoriesTopicsNumber" runat="server"
                     CssClass="form-control" TextMode="Number"
                     Text="0" />
    </div>

    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="CategoriesMessagesNumber">
            Number of Messages in Each Topic:
        </asp:Label>
        <asp:TextBox ID="CategoriesMessagesNumber" runat="server"
                     CssClass="form-control" TextMode="Number"
                     Text="0" />
    </div>

    <YAF:Alert runat="server" Type="danger">
        Warning! This is a test/debug feature. Never use it in production environment.
    </YAF:Alert>

</div>
<div id="View4" class="tab-pane" role="tabpanel">
    <h4>
        Create Test Forums
    </h4>

    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="ForumsNumber">
            Number of forums to create:
        </asp:Label>
        <asp:TextBox ID="ForumsNumber" runat="server" CssClass="form-control" TextMode="Number"
                     Text="0" />
        <asp:RequiredFieldValidator ID="Requiredfieldvalidator6" runat="server" EnableClientScript="False"
                                    Text="0" ControlToValidate="ForumsNumber" ErrorMessage="Forums' number is required."></asp:RequiredFieldValidator>
    </div>

    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="ForumsCountMessages">
            Count topics and messages in statistics:
        </asp:Label>
        <div class="form-check form-switch">
            <asp:CheckBox ID="ForumsCountMessages" runat="server" Checked="true" />
        </div>
    </div>

    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="ForumsHideNoAccess">
            Hide forums if no access:
        </asp:Label>
        <div class="form-check form-switch"><asp:CheckBox ID="ForumsHideNoAccess" runat="server" Checked="true" /></div>
    </div>

    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="ForumsStartMask">
            Select common access mask for all groups:
        </asp:Label>
        <asp:DropDownList ID="ForumsStartMask" runat="server"
                          DataValueField="ID"
                          DataTextField="Name"
                          CssClass="select2-select">
        </asp:DropDownList>
    </div>

    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="ForumsCategory">
            Choose category:
        </asp:Label>
        <asp:DropDownList ID="ForumsCategory" runat="server" AutoPostBack="true" DataValueField="ID"
                          DataTextField="Name" OnDataBound="ForumsCategory_OnSelectedIndexChanged" OnSelectedIndexChanged="ForumsCategory_OnSelectedIndexChanged" CssClass="select2-select">
        </asp:DropDownList>
    </div>

    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="ForumsParent">
            Choose parent forum:
        </asp:Label>
        <asp:DropDownList ID="ForumsParent" runat="server" DataValueField="ForumID" DataTextField="Forum" CssClass="select2-select">
        </asp:DropDownList>
    </div>

    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="ForumsTopicsNumber">
            Number of Topics in Each Forum:
        </asp:Label>
        <asp:TextBox ID="ForumsTopicsNumber" runat="server"
                     CssClass="form-control" TextMode="Number"
                     Text="0" />
    </div>

    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="ForumsMessagesNumber">
            Number of Messages in Each Topic:
        </asp:Label>
        <asp:TextBox ID="ForumsMessagesNumber" runat="server"
                     CssClass="form-control" TextMode="Number"
                     Text="0" />
    </div>

    <YAF:Alert runat="server" Type="danger">
        Warning! This is a test/debug feature. Never use it in production environment.
    </YAF:Alert>
</div>
<div id="View5" class="tab-pane" role="tabpanel">
    <h4>
        Create Test Topics
    </h4>

    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="TopicsNumber">
            Number of topics to create:
        </asp:Label>
        <asp:TextBox ID="TopicsNumber" runat="server" CssClass="form-control" TextMode="Number"
                     Text="0" />
        <asp:RequiredFieldValidator ID="Requiredfieldvalidator7" runat="server" EnableClientScript="False"
                                    Text="0" ControlToValidate="TopicsNumber" ErrorMessage="Categories' number is required."></asp:RequiredFieldValidator>
    </div>

    <div class="mb-3">
        <asp:DropDownList ID="TopicsPriorityList" runat="server" CssClass="select2-select" />
    </div>

    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="PollCreate">
            Create Poll:
        </asp:Label>
        <div class="form-check form-switch"><asp:CheckBox ID="PollCreate" runat="server" Checked="false" /></div>
    </div>

    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="TopicsCategory">
            Choose category:
        </asp:Label>
        <asp:DropDownList ID="TopicsCategory" runat="server" AutoPostBack="true" DataValueField="ID"
                          DataTextField="Name" OnDataBound="TopicsCategory_OnSelectedIndexChanged"
                          OnSelectedIndexChanged="TopicsCategory_OnSelectedIndexChanged" CssClass="select2-select">
        </asp:DropDownList>
    </div>

    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="TopicsForum">
            Choose forum:
        </asp:Label>
        <asp:DropDownList ID="TopicsForum" runat="server" AutoPostBack="false" DataValueField="ForumID"
                          DataTextField="Forum" CssClass="select2-select">
        </asp:DropDownList>
    </div>

    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="TopicsMessagesNumber">
            Number of Messages in Each Topic:
        </asp:Label>
        <asp:TextBox ID="TopicsMessagesNumber" runat="server" CssClass="form-control" TextMode="Number"
                     Text="0" />
    </div>

    <YAF:Alert runat="server" Type="danger">
        Warning! This is a test/debug feature. Never use it in production environment.
    </YAF:Alert>

</div>
<div id="View6" class="tab-pane" role="tabpanel">
    <h4>
        Create Test Messages
    </h4>

    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="PostsNumber">
            Number of messages to create:
        </asp:Label>
        <asp:TextBox ID="PostsNumber" runat="server" CssClass="form-control" TextMode="Number"
                     Text="0" />
        <asp:RequiredFieldValidator ID="Requiredfieldvalidator8" runat="server" EnableClientScript="False"
                                    Text="0" ControlToValidate="PostsNumber" ErrorMessage="Categories' number is required."></asp:RequiredFieldValidator>
    </div>

    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="PostsCategory">
            Choose category:
        </asp:Label>
        <asp:DropDownList ID="PostsCategory" runat="server" AutoPostBack="true" DataValueField="ID"
                          DataTextField="Name" OnDataBound="PostsCategory_OnSelectedIndexChanged" OnSelectedIndexChanged="PostsCategory_OnSelectedIndexChanged" CssClass="select2-select">
        </asp:DropDownList>
    </div>
    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="PostsForum">
            Choose forum:
        </asp:Label>
        <asp:DropDownList ID="PostsForum" runat="server" DataValueField="ForumID" AutoPostBack="true"
                          DataTextField="Forum" OnSelectedIndexChanged="PostsForum_OnSelectedIndexChanged" CssClass="select2-select">
        </asp:DropDownList>
    </div>

    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="PostsTopic">
            Choose topic:
        </asp:Label>
        <asp:DropDownList ID="PostsTopic" runat="server" DataValueField="TopicID" DataTextField="Subject" CssClass="select2-select">
        </asp:DropDownList>
    </div>

    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="MyMessage">
            Enter your custom message for each test message:
        </asp:Label>
        <asp:TextBox ID="MyMessage" runat="server" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
    </div>

    <YAF:Alert runat="server" Type="danger">
        Warning! This is a test/debug feature. Never use it in production environment.
    </YAF:Alert>
</div>
<div id="View7" class="tab-pane" role="tabpanel">
    <h4>
        Create Test Private Messages
    </h4>

    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="PMessagesNumber">
            Number of Private Messages to create:
        </asp:Label>
        <asp:TextBox ID="PMessagesNumber" runat="server" CssClass="form-control" TextMode="Number"
                     Text="0" />
        <asp:RequiredFieldValidator ID="Requiredfieldvalidator9" runat="server" EnableClientScript="False"
                                    Text="0" ControlToValidate="PMessagesNumber" ErrorMessage="Pmessages's number is required."></asp:RequiredFieldValidator>
    </div>

    <div class="mb-3">
        <asp:DropDownList ID="PMessagesBoardsList" runat="server"
                          DataValueField="ID"
                          DataTextField="Name"
                          CssClass="select2-select">
        </asp:DropDownList>
    </div>

    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="From">
            From User:
        </asp:Label>
        <asp:TextBox ID="From" runat="server" CssClass="form-control"></asp:TextBox>
        <asp:RequiredFieldValidator ID="Requiredfieldvalidator2" runat="server" EnableClientScript="False"
                                    Text="0" ControlToValidate="From" ErrorMessage="User Name is Required."></asp:RequiredFieldValidator>
    </div>

    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="To">
            To User:
        </asp:Label>
        <asp:TextBox ID="To" runat="server" CssClass="form-control"></asp:TextBox>
        <asp:RequiredFieldValidator ID="Requiredfieldvalidator11" runat="server" EnableClientScript="False"
                                    Text="0" ControlToValidate="To" ErrorMessage="User Name is Required."></asp:RequiredFieldValidator>
    </div>

    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="PMessagesToAll">
            Send To All:
        </asp:Label>
        <div class="form-check form-switch"><asp:CheckBox ID="PMessagesToAll" runat="server" Enabled="false" Checked="false" />
        </div>
    </div>

    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="MarkRead">
            Mark All Private Messages As Read:
        </asp:Label>
        <div class="form-check form-switch">
            <asp:CheckBox ID="MarkRead" runat="server" Checked="true" />
        </div>
    </div>

    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="PMessageText">
            Enter your custom message for each test private message:
        </asp:Label>

        <asp:TextBox ID="PMessageText" runat="server" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
    </div>

    <YAF:Alert runat="server" Type="danger">
        Warning! This is a test/debug feature. Never use it in production environment.
    </YAF:Alert>
</div>
<div id="View8" class="tab-pane" role="tabpanel">
    <h4>
        Usage guide
    </h4>

    <YAF:Alert runat="server" Type="warning">
        <p>Test data generator is a utility to test Yet Another Forum performance.</p>
        <br />
        The operations take a lot of time, if you generate whales of data.
        All the time browser page will look like something hangs. On completing you will see notification window.
        If you want to generate hundreds thousands records it can take,
        hours or even days.
        <p><strong>never</strong> use it in production environment as you can't delete data using the test data generator.</p>
    </YAF:Alert>

    <h4>
        Generator Settings
    </h4>

    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="UserPrefixTB">
            User Prefix(max 92):
        </asp:Label>
        <asp:TextBox ID="UserPrefixTB" runat="server" CssClass="form-control">brd-</asp:TextBox>
        <asp:RequiredFieldValidator ID="UserPrefixTBRequiredfieldvalidator" runat="server" EnableClientScript="False"
                                    Text="0" ControlToValidate="UserPrefixTB" ErrorMessage="A user Prefix is required."></asp:RequiredFieldValidator>
    </div>

    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="BoardPrefixTB">
            Board Prefix(max 12):
        </asp:Label>
        <asp:TextBox ID="BoardPrefixTB" runat="server" CssClass="form-control">brd-</asp:TextBox>
        <asp:RequiredFieldValidator ID="BoardPrefixTBRequiredfieldvalidator" runat="server" EnableClientScript="False"
                                    Text="0" ControlToValidate="BoardPrefixTB" ErrorMessage="A board Prefix is required."></asp:RequiredFieldValidator>
    </div>

    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="CategoryPrefixTB">
            Category Prefix(max 12):
        </asp:Label>
        <asp:TextBox ID="CategoryPrefixTB" runat="server" CssClass="form-control">cat-</asp:TextBox>
        <asp:RequiredFieldValidator ID="CategoryPrefixTBRequiredfieldvalidator" runat="server" EnableClientScript="False"
                                    Text="cat-" ControlToValidate="CategoryPrefixTB" ErrorMessage="A Category Prefix is required."></asp:RequiredFieldValidator>
    </div>

    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="ForumPrefixTB">
            Forum Prefix(max 12):
        </asp:Label>
        <asp:TextBox ID="ForumPrefixTB" runat="server" CssClass="form-control">frm-</asp:TextBox>
        <asp:RequiredFieldValidator ID="ForumPrefixTBRequiredfieldvalidator" runat="server" EnableClientScript="False"
                                    Text="frm-" ControlToValidate="ForumPrefixTB" ErrorMessage="A Forum Prefix is required."></asp:RequiredFieldValidator>
    </div>

    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="TopicPrefixTB">
            Topic Name Prefix(max 92):
        </asp:Label>
        <asp:TextBox ID="TopicPrefixTB" runat="server" CssClass="form-control">topic-</asp:TextBox>
        <asp:RequiredFieldValidator ID="TopicPrefixTBRequiredfieldvalidator" runat="server" EnableClientScript="False"
                                    Text="topic-" ControlToValidate="TopicPrefixTB" ErrorMessage="A Topic Prefix is required."></asp:RequiredFieldValidator>
    </div>

    <div class="mb-3">
        <asp:Label runat="server" AssociatedControlID="MessageContentPrefixTB">
            Message Content Prefix:
        </asp:Label>
        <asp:TextBox ID="MessageContentPrefixTB" runat="server" CssClass="form-control">msg-</asp:TextBox>
        <asp:RequiredFieldValidator ID="MessageContentPrefixTBRequiredfieldvalidator" runat="server" EnableClientScript="False"
                                    Text="msg-" ControlToValidate="MessageContentPrefixTB" ErrorMessage="A Messsage Content Prefix is required."></asp:RequiredFieldValidator>
    </div>
</div>
</div>
</asp:Panel>
<asp:HiddenField runat="server" ID="hidLastTab" Value="View1" />
<YAF:ThemeButton ID="LaunchGenerator" runat="server"
                 Text="Launch Generator"
                 Type="Primary"
                 CssClass="card-link"
                 CausesValidation="True"
                 OnClick="CreateTestData_Click" />
<YAF:ThemeButton ID="Cancel" runat="server"
                 TextLocalizedTag="CANCEL"
                 Size="Normal"
                 Type="Danger"
                 CssClass="card-link"
                 CausesValidation="False"
                 OnClick="Cancel_Click" />
</div>
</div>