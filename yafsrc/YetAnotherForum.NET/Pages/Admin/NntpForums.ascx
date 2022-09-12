<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.NntpForums"
    CodeBehind="NntpForums.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<%@ Register TagPrefix="modal" TagName="Edit" Src="../../Dialogs/NntpForumEdit.ascx" %>


<YAF:PageLinks runat="server" ID="PageLinks" />

    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <YAF:IconHeader runat="server"
                                    IconName="newspaper"
                                    LocalizedPage="ADMIN_NNTPFORUMS"></YAF:IconHeader>
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
                            <%# this.Eval("Item2.Name") %>
                        </h5>
                        <small>
                            <span class="fw-bold">
                                <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="Active" LocalizedPage="ADMIN_NNTPFORUMS" />
                            </span>
                            <div class="badge bg-<%# this.Eval("Item1.Active").ToType<bool>() ? "success" : "secondary" %>">
                                <%# this.Eval("Item1.Active") %>
                            </div>
                        </small>
                    </div>
                    <p class="mb-1">
                        <span class="fw-bold">
                            <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="Group" LocalizedPage="ADMIN_NNTPFORUMS" />
                        </span>
                        <%# this.Eval("Item1.GroupName") %>
                        
                        <span class="fw-bold">
                            <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="Forum" LocalizedPage="ADMIN_NNTPFORUMS" />
                        </span>
                        <%# this.Eval("Item3.Name") %>
                    </p>
                    <small>
                        <div class="btn-group btn-group-sm">
                            <YAF:ThemeButton ID="ThemeButtonEdit" 
                                             Type="Info" 
                                             Size="Small" runat="server"
                                             CommandName="edit" 
                                             CommandArgument='<%# this.Eval("Item1.ID") %>'
                                             Icon="edit" 
                                             TextLocalizedTag="EDIT">
                            </YAF:ThemeButton>
                            <YAF:ThemeButton ID="ThemeButtonDelete" 
                                             Type="Danger" 
                                             Size="Small" runat="server"
                                             CommandName="delete" 
                                             CommandArgument='<%# this.Eval("Item1.ID") %>'
                                             Icon="trash" 
                                             TextLocalizedTag="DELETE"
                                             ReturnConfirmTag="DELETE_FORUM">
                            </YAF:ThemeButton>
                        </div>
                    </small>
                    <div class="dropdown-menu context-menu" aria-labelledby="context menu">
                        <YAF:ThemeButton ID="ThemeButton1" 
                                         Type="None" 
                                         CssClass="dropdown-item" runat="server"
                                         CommandName="edit" 
                                         CommandArgument='<%# this.Eval("Item1.ID") %>'
                                         Icon="edit" 
                                         TextLocalizedTag="EDIT">
                        </YAF:ThemeButton>
                        <YAF:ThemeButton ID="ThemeButton2" 
                                         Type="None" 
                                         CssClass="dropdown-item" runat="server"
                                         CommandName="delete" 
                                         CommandArgument='<%# this.Eval("Item1.ID") %>'
                                         Icon="trash" 
                                         TextLocalizedTag="DELETE"
                                         ReturnConfirmTag="DELETE_FORUM">
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