<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.nntpforums"
    CodeBehind="nntpforums.ascx.cs" %>
<%@ Register TagPrefix="modal" TagName="Edit" Src="../../Dialogs/NntpForumEdit.ascx" %>


<YAF:PageLinks runat="server" ID="PageLinks" />

    <div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_NNTPFORUMS" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-newspaper fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_NNTPFORUMS" />
                    </div>
                <div class="card-body">
                    <asp:Repeater ID="RankList" OnItemCommand="RankListItemCommand" runat="server">
            <HeaderTemplate>
                <ul class="list-group">
            </HeaderTemplate>
            <ItemTemplate>
                <li class="list-group-item list-group-item-action list-group-item-menu">
                    <div class="d-flex w-100 justify-content-between">
                        <h5 class="mb-1">
                            <%# this.Eval( "Name") %>
                        </h5>
                        <small>
                            <span class="font-weight-bold">
                                <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="Active" LocalizedPage="ADMIN_NNTPFORUMS" />
                            </span>
                            <%# this.Eval( "Active") %>
                        </small>
                    </div>
                    <p class="mb-1">
                        <span class="font-weight-bold">
                            <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="Group" LocalizedPage="ADMIN_NNTPFORUMS" />
                        </span>
                        <%# this.Eval( "GroupName") %>
                        
                        <span class="font-weight-bold">
                            <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="Forum" LocalizedPage="ADMIN_NNTPFORUMS" />
                        </span>
                        <%# this.Eval( "ForumName") %>
                    </p>
                    <small>
                        <YAF:ThemeButton ID="ThemeButtonEdit" 
                                         Type="Info" 
                                         Size="Small" runat="server"
                                         CommandName="edit" 
                                         CommandArgument='<%# this.Eval("NntpForumID") %>'
                                         Icon="edit" 
                                         TextLocalizedTag="EDIT">
                        </YAF:ThemeButton>
                        <YAF:ThemeButton ID="ThemeButtonDelete" 
                                         Type="Danger" 
                                         Size="Small" runat="server"
                                         CommandName="delete" 
                                         CommandArgument='<%# this.Eval("NntpForumID") %>'
                                         Icon="trash" 
                                         TextLocalizedTag="DELETE"
                                         ReturnConfirmText='<%# this.GetText("ADMIN_NNTPFORUMS", "DELETE_FORUM") %>'>
                        </YAF:ThemeButton>
                    </small>
                    <div class="dropdown-menu context-menu" aria-labelledby="context menu">
                        <YAF:ThemeButton ID="ThemeButton1" 
                                         Type="None" 
                                         CssClass="dropdown-item" runat="server"
                                         CommandName="edit" 
                                         CommandArgument='<%# this.Eval("NntpForumID") %>'
                                         Icon="edit" 
                                         TextLocalizedTag="EDIT">
                        </YAF:ThemeButton>
                        <YAF:ThemeButton ID="ThemeButton2" 
                                         Type="None" 
                                         CssClass="dropdown-item" runat="server"
                                         CommandName="delete" 
                                         CommandArgument='<%# this.Eval("NntpForumID") %>'
                                         Icon="trash" 
                                         TextLocalizedTag="DELETE"
                                         ReturnConfirmText='<%# this.GetText("ADMIN_NNTPFORUMS", "DELETE_FORUM") %>'>
                        </YAF:ThemeButton>
                        <div class="dropdown-divider"></div>
                        <YAF:ThemeButton ID="NewForum" runat="server" 
                                         Type="None" 
                                         CssClass="dropdown-item" 
                                         OnClick="NewForumClick"
                                         Icon="plus-square" 
                                         TextLocalizedTag="NEW_FORUM" 
                                         TextLocalizedPage="ADMIN_NNTPFORUMS" />
                    </div>
                </li>
            </ItemTemplate>
            <FooterTemplate>
                </ul>
            </FooterTemplate>
        </asp:Repeater>
                </div>
                <div class="card-footer text-center">
                    <YAF:ThemeButton ID="NewForum" runat="server" 
                                     Type="Primary" 
                                     OnClick="NewForumClick"
                                     Icon="plus-square" 
                                     TextLocalizedTag="NEW_FORUM" 
                                     TextLocalizedPage="ADMIN_NNTPFORUMS" />
                </div>
            </div>
        </div>
    </div>



<modal:Edit ID="EditDialog" runat="server" />