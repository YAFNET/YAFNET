<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="true" Inherits="YAF.Controls.ProfileYourAccount"
    CodeBehind="ProfileYourAccount.ascx.cs" %>


<div class="row">
    <div class="col-sm-2 mb-3">
        <span class="pull-right">
            <asp:Image runat="server" ID="AvatarImage" 
                       CssClass="img-fluid rounded" 
                       AlternateText="avatar" />
        </span>
    </div>
<div class="col">
<ul class="list-group">
    <li class="list-group-item text-muted" contenteditable="false">
        <YAF:LocalizedLabel ID="YourAccountLocalized" runat="server" LocalizedTag="YOUR_ACCOUNT" />
    </li>
    <li class="list-group-item text-right">
        <span class="float-left">
            <span class="font-weight-bold">
            <YAF:LocalizedLabel ID="YourUsernameLocalized" runat="server" LocalizedTag="YOUR_USERNAME" />
        </span>
        </span> <asp:Label ID="Name" runat="server" />
    </li>
    <asp:PlaceHolder ID="DisplayNameHolder" runat="server">
    <li class="list-group-item text-right">
        <span class="float-left">
            <span class="font-weight-bold">
            <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="YOUR_USERDISPLAYNAME" />
        </span>
        </span> <asp:Label ID="DisplayName" runat="server" />
    </li>
    </asp:PlaceHolder>
    <li class="list-group-item text-right">
        <span class="float-left">
            <span class="font-weight-bold"><YAF:LocalizedLabel ID="YourEmailLocalized" runat="server" LocalizedTag="YOUR_EMAIL" /></span>
        </span> <asp:Label ID="AccountEmail" runat="server" />
    </li>
    <li class="list-group-item text-right">
        <span class="float-left">
            <span class="font-weight-bold"><YAF:LocalizedLabel ID="NumPostsLocalized" runat="server" LocalizedTag="NUMPOSTS" /></span>
        </span> <asp:Label ID="NumPosts" runat="server" />
               
    </li>
    <li class="list-group-item text-right">
        <span class="float-left">
            <span class="font-weight-bold"><YAF:LocalizedLabel ID="GroupsLocalized" runat="server" LocalizedTag="GROUPS" /></span>
        </span> <asp:Repeater ID="Groups" runat="server">
            <ItemTemplate>
                <span runat="server" style='<%# DataBinder.Eval(Container.DataItem,"Style") %>'>
                    <%# DataBinder.Eval(Container.DataItem,"Name") %></span>
            </ItemTemplate>
            <SeparatorTemplate>
                ,
            </SeparatorTemplate>
        </asp:Repeater>
    </li>
    <li class="list-group-item text-right">
        <span class="float-left">
            <span class="font-weight-bold"> <YAF:LocalizedLabel ID="JoinedLocalized" runat="server" LocalizedTag="JOINED" /></span>
        </span> <asp:Label ID="Joined" runat="server" />
               
    </li>
        </ul>
    </div>
</div>