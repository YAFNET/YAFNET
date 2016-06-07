<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="YafHeader.ascx.cs" Inherits="YAF.Controls.YafHeader" %>
<div id="yafheader">
    <asp:Panel id="GuestUserMessage" CssClass="guestUser" runat="server" Visible="false">
       <asp:Label id="GuestMessage" runat="server"></asp:Label>
    </asp:Panel>
   
    <div class="outerMenuContainer">
        <asp:Panel id="UserContainer" CssClass="menuMyContainer" runat="server" Visible="false">
            <ul class="menuMyList">
                <li class="menuMy myProfile">
                  <asp:HyperLink id="MyProfile" runat="server" Target="_top"></asp:HyperLink>
                </li>
                <asp:PlaceHolder ID="MyInboxItem" runat="server">
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="MyBuddiesItem" runat="server">
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="MyAlbumsItem" runat="server">
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="MyTopicItem" runat="server">
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="LogutItem" runat="server" Visible="false">
                 <li class="menuAccount">
                   <asp:LinkButton ID="LogOutButton" runat="server" OnClick="LogOutClick" OnClientClick="createCookie('ScrollPosition',document.all ? document.scrollTop : window.pageYOffset);"></asp:LinkButton>
                 </li>
                </asp:PlaceHolder>
            </ul>
        </asp:Panel>
        <asp:Panel id="LoggedInUserPanel" CssClass="loggedInUser" runat="server" Visible="false">
        </asp:Panel>
        <div class="menuContainer">
            <ul class="menuList">
                <asp:PlaceHolder ID="menuListItems" runat="server">
                </asp:PlaceHolder>
            </ul>
            <asp:Panel ID="quickSearch" runat="server" CssClass="QuickSearch" Visible="false">
               <asp:TextBox ID="searchInput" runat="server"></asp:TextBox>&nbsp;
               <asp:LinkButton ID="doQuickSearch" onkeydown="" runat="server" CssClass="QuickSearchButton"
                    OnClick="QuickSearchClick">
               </asp:LinkButton>
            </asp:Panel>
            <asp:PlaceHolder ID="AdminModHolder" runat="server" Visible="false">
              <ul class="menuAdminList">
                <asp:PlaceHolder ID="menuAdminItems" runat="server"></asp:PlaceHolder>
              </ul>
            </asp:PlaceHolder>
        </div>
    </div>
    <div id="yafheaderEnd">
    </div>
</div>
