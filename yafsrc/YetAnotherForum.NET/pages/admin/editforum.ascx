<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.editforum"
    CodeBehind="editforum.ascx.cs" %>
<%@ Register TagPrefix="YAF" Namespace="YAF.Controls" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server" ID="Adminmenu1">
    <table class="content" cellspacing="1" cellpadding="0" width="100%">
        <tr>
            <td class="header1" colspan="2">
                <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER1" LocalizedPage="ADMIN_EDITFORUM" />
                <asp:Label ID="ForumNameTitle" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="header2" height="30" colspan="2">
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <YAF:HelpLabel ID="HelpLabel1" runat="server" LocalizedTag="CATEGORY" LocalizedPage="ADMIN_EDITFORUM" />
            </td>
            <td class="post">
                <asp:DropDownList Width="250" ID="CategoryList" runat="server" OnSelectedIndexChanged="Category_Change"
                    DataValueField="CategoryID" DataTextField="Name" CssClass="standardSelectMenu">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <YAF:HelpLabel ID="HelpLabel2" runat="server" LocalizedTag="PARENT_FORUM" LocalizedPage="ADMIN_EDITFORUM" />
                <strong></strong>
                <br />
            </td>
            <td class="post">
                <asp:DropDownList Width="250" ID="ParentList" runat="server" CssClass="standardSelectMenu">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <YAF:HelpLabel ID="HelpLabel3" runat="server" LocalizedTag="NAME" LocalizedPage="ADMIN_EDITFORUM" />
            </td>
            <td class="post">
                <asp:TextBox Width="250" ID="Name" runat="server" CssClass="edit"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <YAF:HelpLabel ID="HelpLabel4" runat="server" LocalizedTag="DESCRIPTION" LocalizedPage="ADMIN_EDITFORUM" />
            </td>
            <td class="post">
                <asp:TextBox Width="250" ID="Description" runat="server" CssClass="edit"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <YAF:HelpLabel ID="HelpLabel14" runat="server" LocalizedTag="REMOTE_URL" LocalizedPage="ADMIN_EDITFORUM" />
                <strong></strong>
                <br />
            </td>
            <td class="post">
                <asp:TextBox Width="250" ID="remoteurl" runat="server" CssClass="edit"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <YAF:HelpLabel ID="HelpLabel13" runat="server" LocalizedTag="THEME" LocalizedPage="ADMIN_EDITFORUM" />
            </td>
            <td class="post">
                <asp:DropDownList Width="250" ID="ThemeList" runat="server" CssClass="standardSelectMenu">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <YAF:HelpLabel ID="HelpLabel12" runat="server" LocalizedTag="SORT_ORDER" LocalizedPage="ADMIN_EDITFORUM" />
            </td>
            <td class="post">
                <asp:TextBox ID="SortOrder" Width="250" MaxLength="5" runat="server" Text="10" CssClass="Numeric"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <YAF:HelpLabel ID="HelpLabel11" runat="server" LocalizedTag="HIDE_NOACESS" LocalizedPage="ADMIN_EDITFORUM" />
            </td>
            <td class="post">
                <asp:CheckBox ID="HideNoAccess" runat="server"></asp:CheckBox>
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <YAF:HelpLabel ID="HelpLabel10" runat="server" LocalizedTag="LOCKED" LocalizedPage="ADMIN_EDITFORUM" />
            </td>
            <td class="post">
                <asp:CheckBox ID="Locked" runat="server"></asp:CheckBox>
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <YAF:HelpLabel ID="HelpLabel9" runat="server" LocalizedTag="NO_POSTSCOUNT" LocalizedPage="ADMIN_EDITFORUM" />
            </td>
            <td class="post">
                <asp:CheckBox ID="IsTest" runat="server"></asp:CheckBox>
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <YAF:HelpLabel ID="HelpLabel8" runat="server" LocalizedTag="PRE_MODERATED" LocalizedPage="ADMIN_EDITFORUM" />
            </td>
            <td class="post">
                <asp:CheckBox ID="Moderated" runat="server" AutoPostBack="true" OnCheckedChanged="ModeratedCheckedChanged"></asp:CheckBox>
            </td>
        </tr>
        <tr runat="server" id="ModerateNewTopicOnlyRow" Visible="false">
            <td class="postheader">
                <YAF:HelpLabel ID="HelpLabel16" runat="server" LocalizedTag="MODERATED_NEWTOPIC_ONLY" LocalizedPage="ADMIN_EDITFORUM" />
            </td>
            <td class="post">
                <asp:CheckBox ID="ModerateNewTopicOnly" runat="server" AutoPostBack="true"></asp:CheckBox>
            </td>
        </tr>
        <tr runat="server" id="ModeratedPostCountRow" Visible="false">
            <td class="postheader">
                <YAF:HelpLabel ID="HelpLabel15" runat="server" LocalizedTag="MODERATED_COUNT" LocalizedPage="ADMIN_EDITFORUM" />
            </td>
            <td class="post">
                <asp:CheckBox ID="ModerateAllPosts" runat="server" AutoPostBack="true" 
                    OnCheckedChanged="ModerateAllPostsCheckedChanged" Checked="true">
                </asp:CheckBox>
                <div>
                    <asp:TextBox Width="250" ID="ModeratedPostCount" runat="server" 
                        Visible="false" MaxLength="5" Text="5" CssClass="Numeric">
                    </asp:TextBox>
                </div>
            </td>
        </tr>
        <tr>
            <td class="postheader">
                <YAF:HelpLabel ID="HelpLabel7" runat="server" LocalizedTag="FORUM_IMAGE" LocalizedPage="ADMIN_EDITFORUM" />
            </td>
            <td class="post">
                <asp:DropDownList Width="250" ID="ForumImages" runat="server" CssClass="standardSelectMenu" />
                <img align="middle" runat="server" id="Preview" alt="" />
            </td>
        </tr>
        <tr visible="false" runat="server">
            <td class="postheader">
                <YAF:HelpLabel ID="HelpLabel6" runat="server" LocalizedTag="STYLES" LocalizedPage="ADMIN_EDITFORUM" />
                <strong></strong>
                <br />
            </td>
            <td class="post">
                <asp:TextBox Width="250" ID="Styles" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr id="NewGroupRow" runat="server">
            <td class="postheader">
                <YAF:HelpLabel ID="HelpLabel5" runat="server" LocalizedTag="INITAL_MASK" LocalizedPage="ADMIN_EDITFORUM" />
            </td>
            <td class="post">
                <asp:DropDownList Width="250" ID="AccessMaskID" OnDataBinding="BindData_AccessMaskID" CssClass="standardSelectMenu"
                    runat="server">
                </asp:DropDownList>
            </td>
        </tr>
        <asp:Repeater ID="AccessList" runat="server">
            <HeaderTemplate>
                <tr>
                    <td class="header1" colspan="2">
                        <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER2" LocalizedPage="ADMIN_EDITFORUM" />
                    </td>
                </tr>
                <tr class="header2">
                    <td>
                        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="Group" LocalizedPage="ADMIN_EDITFORUM" />
                    </td>
                    <td>
                        <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="ACCESS_MASK"
                            LocalizedPage="ADMIN_EDITFORUM" />
                    </td>
                </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td class="postheader">
                        <asp:Label ID="GroupID" Visible="false" runat="server" Text='<%# Eval( "GroupID") %>'>
                        </asp:Label>
                        <%# Eval( "GroupName") %>
                    </td>
                    <td class="post">
                        <asp:DropDownList Width="250" runat="server" ID="AccessMaskID" OnDataBinding="BindData_AccessMaskID" CssClass="standardSelectMenu"
                            OnPreRender="SetDropDownIndex" value='<%# Eval("AccessMaskID") %>' />
                        ...
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
        <tr>
            <td class="postfooter" align="center" colspan="2">
                <asp:Button ID="Save" runat="server" CssClass="pbutton"></asp:Button>&nbsp;
                <asp:Button ID="Cancel" runat="server" CssClass="pbutton"></asp:Button>
            </td>
        </tr>
    </table>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
