<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.accessmasks" Codebehind="accessmasks.ascx.cs" %>


<%@ Import Namespace="YAF.Types.Flags" %>
<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_ACCESSMASKS" /></h1>
    </div>
</div>

<div class="row">
    <div class="col-xl-12">
        <div class="card mb-3">
            <div class="card-header">
                <i class="fa fa-universal-access fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" 
                                                                                                         LocalizedTag="TITLE"  
                                                                                                         LocalizedPage="ADMIN_ACCESSMASKS" />
            </div>
            <div class="card-body">
                <asp:Repeater ID="List" runat="server" OnItemCommand="ListItemCommand">
                    <HeaderTemplate>
                        <ul class="list-group">
                        </HeaderTemplate>
                    <ItemTemplate>
				<li class="list-group-item list-group-item-action list-group-item-menu">
                    <div class="d-flex w-100 justify-content-between">
					<h5 class="mb-1"><%# this.Eval( "Name") %></h5>
                        <small class="d-none d-md-block">
                            <span class="font-weight-bold">
                                <YAF:LocalizedLabel runat="server" LocalizedTag="SORT_ORDER"></YAF:LocalizedLabel>
                            </span>
                            <%# this.Eval( "SortOrder") %>
                        </small>
                    </div>
                    <p>
                        <ul class="list-inline">
                            <li class="list-inline-item">
                                <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" 
                                                    LocalizedTag="READ" LocalizedPage="ADMIN_ACCESSMASKS" />:&nbsp;
                                <asp:Label ID="Label1" runat="server" 
                                           CssClass='<%# this.GetItemColor(this.BitSet(this.Eval("Flags"),(int)AccessFlags.Flags.ReadAccess)) %>'>
                                    <%# this.GetItemName(this.BitSet(this.Eval("Flags"),(int)AccessFlags.Flags.ReadAccess)) %>
                                </asp:Label>
                            </li>
                            <li class="list-inline-item">
                                <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" 
                                                    LocalizedTag="POST" LocalizedPage="ADMIN_ACCESSMASKS" />:&nbsp;
                                <asp:Label ID="Label2" runat="server" 
                                           CssClass='<%# this.GetItemColor(this.BitSet(this.Eval("Flags"),(int)AccessFlags.Flags.PostAccess)) %>'>
                                    <%# this.GetItemName(this.BitSet(this.Eval("Flags"),(int)AccessFlags.Flags.PostAccess)) %>
                                </asp:Label>
                            </li>
                            <li class="list-inline-item">
                                <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" 
                                                    LocalizedTag="REPLY" LocalizedPage="ADMIN_ACCESSMASKS" />:&nbsp;
                                <asp:Label ID="Label3" runat="server" 
                                           CssClass='<%# this.GetItemColor(this.BitSet(this.Eval("Flags"),(int)AccessFlags.Flags.ReplyAccess)) %>'>
                                    <%# this.GetItemName(this.BitSet(this.Eval("Flags"),(int)AccessFlags.Flags.ReplyAccess)) %>
                                </asp:Label>
                            </li>

                            <li class="list-inline-item">
                                <YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" 
                                                    LocalizedTag="PRIORITY" LocalizedPage="ADMIN_ACCESSMASKS" />:&nbsp;
                                <asp:Label ID="Label4" runat="server" 
                                           CssClass='<%# this.GetItemColor(this.BitSet(this.Eval("Flags"),(int)AccessFlags.Flags.PriorityAccess)) %>'>
                                    <%# this.GetItemName(this.BitSet(this.Eval("Flags"),(int)AccessFlags.Flags.PriorityAccess)) %>
                                </asp:Label>

                            </li>
                            <li class="list-inline-item">
                                <YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" 
                                                    LocalizedTag="POLL" LocalizedPage="ADMIN_ACCESSMASKS" />:&nbsp;
                                <asp:Label ID="Label5" runat="server" 
                                           CssClass='<%# this.GetItemColor(this.BitSet(this.Eval("Flags"),(int)AccessFlags.Flags.PollAccess)) %>'>
                                    <%# this.GetItemName(this.BitSet(this.Eval( "Flags"),(int)AccessFlags.Flags.PollAccess)) %>
                                </asp:Label>
                            </li>

                            <li class="list-inline-item">
                                <YAF:LocalizedLabel ID="LocalizedLabel10" runat="server" 
                                                    LocalizedTag="VOTE" LocalizedPage="ADMIN_ACCESSMASKS" />:&nbsp;
                                <asp:Label ID="Label6" runat="server" 
                                           CssClass='<%# this.GetItemColor(this.BitSet(this.Eval("Flags"),(int)AccessFlags.Flags.VoteAccess)) %>'>
                                    <%# this.GetItemName(this.BitSet(this.Eval( "Flags"),(int)AccessFlags.Flags.VoteAccess)) %>
                                </asp:Label>
                            </li>

                            <li class="list-inline-item">
                                <YAF:LocalizedLabel ID="LocalizedLabel11" runat="server" 
                                                    LocalizedTag="MODERATOR" LocalizedPage="ADMIN_ACCESSMASKS" />:&nbsp;
                                <asp:Label ID="Label7" runat="server" 
                                           CssClass='<%# this.GetItemColor(this.BitSet(this.Eval("Flags"),(int)AccessFlags.Flags.ModeratorAccess)) %>'>
                                    <%# this.GetItemName(this.BitSet(this.Eval( "Flags"),(int)AccessFlags.Flags.ModeratorAccess)) %>
                                </asp:Label>
                            </li>

                            <li class="list-inline-item">
                                <YAF:LocalizedLabel ID="LocalizedLabel12" runat="server" 
                                                    LocalizedTag="EDIT" LocalizedPage="ADMIN_ACCESSMASKS" />:&nbsp;
                                <asp:Label ID="Label8" runat="server" 
                                           CssClass='<%# this.GetItemColor(this.BitSet(this.Eval("Flags"),(int)AccessFlags.Flags.EditAccess)) %>'>
                                    <%# this.GetItemName(this.BitSet(this.Eval( "Flags"),(int)AccessFlags.Flags.EditAccess)) %>
                                </asp:Label>
                            </li>

                            <li class="list-inline-item">
                                <YAF:LocalizedLabel ID="LocalizedLabel13" runat="server" 
                                                    LocalizedTag="DELETE" LocalizedPage="ADMIN_ACCESSMASKS"/>:&nbsp;
                                <asp:Label ID="Label9" runat="server" 
                                           CssClass='<%# this.GetItemColor(this.BitSet(this.Eval("Flags"),(int)AccessFlags.Flags.DeleteAccess)) %>'>
                                    <%# this.GetItemName(this.BitSet(this.Eval( "Flags"),(int)AccessFlags.Flags.DeleteAccess)) %>
                                </asp:Label>
                            </li>

                            <li class="list-inline-item">
                                <YAF:LocalizedLabel ID="LocalizedLabel14" runat="server" 
                                                    LocalizedTag="UPLOAD" LocalizedPage="ADMIN_ACCESSMASKS" />:&nbsp;
                                <asp:Label ID="Label10" runat="server" 
                                           CssClass='<%# this.GetItemColor(this.BitSet(this.Eval("Flags"),(int)AccessFlags.Flags.UploadAccess)) %>'>
                                    <%# this.GetItemName(this.BitSet(this.Eval( "Flags"),(int)AccessFlags.Flags.UploadAccess)) %>
                                </asp:Label>
                            </li>

                            <li class="list-inline-item">
                                <YAF:LocalizedLabel ID="LocalizedLabel15" runat="server" 
                                                    LocalizedTag="DOWNLOAD" LocalizedPage="ADMIN_ACCESSMASKS" />:&nbsp;
                                <asp:Label ID="Label11" runat="server" 
                                           CssClass='<%# this.GetItemColor(this.BitSet(this.Eval("Flags"),(int)AccessFlags.Flags.DownloadAccess)) %>'>
                                    <%# this.GetItemName(this.BitSet(this.Eval( "Flags"),(int)AccessFlags.Flags.DownloadAccess)) %>
                                </asp:Label>
                            </li>
					</ul>
                        </p>
                    <small>
                        <YAF:ThemeButton ID="ThemeButtonEdit" runat="server"
                                         Type="Info" 
                                         Size="Small"
                                         CommandName="edit" 
                                         CommandArgument='<%# this.Eval( "ID") %>'
                                         TitleLocalizedTag="EDIT"
                                         Icon="edit"
                                         TextLocalizedTag="EDIT">
                        </YAF:ThemeButton>
                        <YAF:ThemeButton ID="ThemeButtonDelete" runat="server"
                                         Type="Danger" 
                                         Size="Small"
                                         CommandName="delete" 
                                         CommandArgument='<%# this.Eval( "ID") %>'
                                         TitleLocalizedTag="DELETE"
                                         Icon="trash"
                                         TextLocalizedTag="DELETE"
                                         ReturnConfirmText='<%# this.GetText("ADMIN_ACCESSMASKS", "CONFIRM_DELETE") %>'>
                        </YAF:ThemeButton>
                        <div class="dropdown-menu context-menu" aria-labelledby="context menu">
                            <YAF:ThemeButton ID="ThemeButton1" runat="server"
                                             Type="None" 
                                             CssClass="dropdown-item"
                                             CommandName="edit" 
                                             CommandArgument='<%# this.Eval( "ID") %>'
                                             TitleLocalizedTag="EDIT"
                                             Icon="edit"
                                             TextLocalizedTag="EDIT">
                            </YAF:ThemeButton>
                            <YAF:ThemeButton ID="ThemeButton2" runat="server"
                                             Type="None"
                                             CssClass="dropdown-item"
                                             CommandName="delete" 
                                             CommandArgument='<%# this.Eval( "ID") %>'
                                             TitleLocalizedTag="DELETE"
                                             Icon="trash"
                                             TextLocalizedTag="DELETE"
                                             ReturnConfirmText='<%# this.GetText("ADMIN_ACCESSMASKS", "CONFIRM_DELETE") %>'>
                            </YAF:ThemeButton>
                            <div class="dropdown-divider"></div>
                            <YAF:ThemeButton ID="New" runat="server" 
                                             OnClick="NewClick" 
                                             Type="None" 
                                             CssClass="dropdown-item"
                                             Icon="plus-square" 
                                             TextLocalizedTag="NEW_MASK" />
                        </div>
                    </small>
                </li>
			</ItemTemplate>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
            <div class="card-footer text-center">
                <YAF:ThemeButton ID="New" runat="server" 
                                 OnClick="NewClick" 
                                 Type="Primary" 
                                 Icon="plus-square" 
                                 TextLocalizedTag="NEW_MASK" />
            </div>
        </div>
    </div>
</div>