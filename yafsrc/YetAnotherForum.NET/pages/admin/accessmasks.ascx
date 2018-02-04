<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.accessmasks" Codebehind="accessmasks.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Flags" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
    <div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_ACCESSMASKS" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-universal-access fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="TITLE"  LocalizedPage="ADMIN_ACCESSMASKS" />
                </div>
                <div class="card-body">
		<asp:Repeater ID="List" runat="server" OnItemCommand="ListItemCommand">
		    <HeaderTemplate>
		        <div class="table-responsive">
                    <table class="table">
                        <tr>
                            <thead>
                                <th colspan="2">
                                    <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="NAME"  LocalizedPage="ADMIN_ACCESSMASKS" />
                                </th>
                            </thead>
                        </tr>
		    </HeaderTemplate>
			<ItemTemplate>
				<tr>
					<td>
					  <h5><%# this.Eval( "Name") %></h5>
					</td>
					<td>
					    <span class="float-right">
					    <YAF:ThemeButton ID="ThemeButtonEdit" Type="Info" CssClass="btn-sm"
                            CommandName="edit" CommandArgument='<%# this.Eval( "ID") %>'
                            TitleLocalizedTag="EDIT"
                            Icon="edit"
                            TextLocalizedTag="EDIT"
                            runat="server">
					    </YAF:ThemeButton>
						<YAF:ThemeButton ID="ThemeButtonDelete" Type="Danger" CssClass="btn-sm"
                                    CommandName='delete' CommandArgument='<%# this.Eval( "ID") %>'
                                    TitleLocalizedTag="DELETE"
                                    Icon="trash"
                                    TextLocalizedTag="DELETE"
                                    ReturnConfirmText='<%# this.GetText("ADMIN_ACCESSMASKS", "CONFIRM_DELETE") %>'  runat="server">
                                </YAF:ThemeButton>
					    </span>
                    </td>
				</tr>
                <tr>
                    <td colspan="2">
                        <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="READ"  LocalizedPage="ADMIN_ACCESSMASKS" />&nbsp;
					    <asp:Label ID="Label1" runat="server" CssClass='<%# this.GetItemColor(this.BitSet(this.Eval("Flags"),(int)AccessFlags.Flags.ReadAccess)) %>'><%# this.GetItemName(this.BitSet(this.Eval("Flags"),(int)AccessFlags.Flags.ReadAccess)) %></asp:Label>&nbsp;|&nbsp;
                        <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="POST"  LocalizedPage="ADMIN_ACCESSMASKS" />&nbsp;
						<asp:Label ID="Label2" runat="server" CssClass='<%# this.GetItemColor(this.BitSet(this.Eval("Flags"),(int)AccessFlags.Flags.PostAccess)) %>'><%# this.GetItemName(this.BitSet(this.Eval("Flags"),(int)AccessFlags.Flags.PostAccess)) %></asp:Label>&nbsp;|&nbsp;

						<YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="REPLY"  LocalizedPage="ADMIN_ACCESSMASKS" />&nbsp;
                        <asp:Label ID="Label3" runat="server" CssClass='<%# this.GetItemColor(this.BitSet(this.Eval("Flags"),(int)AccessFlags.Flags.ReplyAccess)) %>'><%# this.GetItemName(this.BitSet(this.Eval("Flags"),(int)AccessFlags.Flags.ReplyAccess)) %></asp:Label>&nbsp;|&nbsp;

                        <YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="PRIORITY"  LocalizedPage="ADMIN_ACCESSMASKS" />&nbsp;
                        <asp:Label ID="Label4" runat="server" CssClass='<%# this.GetItemColor(this.BitSet(this.Eval("Flags"),(int)AccessFlags.Flags.PriorityAccess)) %>'><%# this.GetItemName(this.BitSet(this.Eval("Flags"),(int)AccessFlags.Flags.PriorityAccess)) %></asp:Label>&nbsp;|&nbsp;


                    <YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="POLL"  LocalizedPage="ADMIN_ACCESSMASKS" />&nbsp;
					<asp:Label ID="Label5" runat="server" CssClass='<%# this.GetItemColor(this.BitSet(this.Eval("Flags"),(int)AccessFlags.Flags.PollAccess)) %>'><%# this.GetItemName(this.BitSet(this.Eval( "Flags"),(int)AccessFlags.Flags.PollAccess)) %></asp:Label>&nbsp;|&nbsp;

					<YAF:LocalizedLabel ID="LocalizedLabel10" runat="server" LocalizedTag="VOTE"  LocalizedPage="ADMIN_ACCESSMASKS" />&nbsp;
						<asp:Label ID="Label6" runat="server" CssClass='<%# this.GetItemColor(this.BitSet(this.Eval("Flags"),(int)AccessFlags.Flags.VoteAccess)) %>'><%# this.GetItemName(this.BitSet(this.Eval( "Flags"),(int)AccessFlags.Flags.VoteAccess)) %></asp:Label>&nbsp;|&nbsp;

				<YAF:LocalizedLabel ID="LocalizedLabel11" runat="server" LocalizedTag="MODERATOR"  LocalizedPage="ADMIN_ACCESSMASKS" />&nbsp;
						<asp:Label ID="Label7" runat="server" CssClass='<%# this.GetItemColor(this.BitSet(this.Eval("Flags"),(int)AccessFlags.Flags.ModeratorAccess)) %>'><%# this.GetItemName(this.BitSet(this.Eval( "Flags"),(int)AccessFlags.Flags.ModeratorAccess)) %></asp:Label>&nbsp;|&nbsp;

					<YAF:LocalizedLabel ID="LocalizedLabel12" runat="server" LocalizedTag="EDIT"  LocalizedPage="ADMIN_ACCESSMASKS" />&nbsp;
						<asp:Label ID="Label8" runat="server" CssClass='<%# this.GetItemColor(this.BitSet(this.Eval("Flags"),(int)AccessFlags.Flags.EditAccess)) %>'><%# this.GetItemName(this.BitSet(this.Eval( "Flags"),(int)AccessFlags.Flags.EditAccess)) %></asp:Label>&nbsp;|&nbsp;

				<YAF:LocalizedLabel ID="LocalizedLabel13" runat="server" LocalizedTag="DELETE"  LocalizedPage="ADMIN_ACCESSMASKS"/>&nbsp;
						<asp:Label ID="Label9" runat="server" CssClass='<%# this.GetItemColor(this.BitSet(this.Eval("Flags"),(int)AccessFlags.Flags.DeleteAccess)) %>'><%# this.GetItemName(this.BitSet(this.Eval( "Flags"),(int)AccessFlags.Flags.DeleteAccess)) %></asp:Label>&nbsp;|&nbsp;

				<YAF:LocalizedLabel ID="LocalizedLabel14" runat="server" LocalizedTag="UPLOAD"  LocalizedPage="ADMIN_ACCESSMASKS" />&nbsp;
						<asp:Label ID="Label10" runat="server" CssClass='<%# this.GetItemColor(this.BitSet(this.Eval("Flags"),(int)AccessFlags.Flags.UploadAccess)) %>'><%# this.GetItemName(this.BitSet(this.Eval( "Flags"),(int)AccessFlags.Flags.UploadAccess)) %></asp:Label>&nbsp;|&nbsp;

				<YAF:LocalizedLabel ID="LocalizedLabel15" runat="server" LocalizedTag="DOWNLOAD"  LocalizedPage="ADMIN_ACCESSMASKS" />&nbsp;
						<asp:Label ID="Label11" runat="server" CssClass='<%# this.GetItemColor(this.BitSet(this.Eval("Flags"),(int)AccessFlags.Flags.DownloadAccess)) %>'><%# this.GetItemName(this.BitSet(this.Eval( "Flags"),(int)AccessFlags.Flags.DownloadAccess)) %></asp:Label>
					</td>
                </tr>
			</ItemTemplate>
            <FooterTemplate>
                </table>
            </div>
            </FooterTemplate>
		</asp:Repeater>
                </div>
                <div class="card-footer text-lg-center">
				    <YAF:ThemeButton ID="New" runat="server" OnClick="NewClick" Type="Primary" 
                        Icon="plus-square" TextLocalizedTag="NEW_MASK" />
                </div>
            </div>
        </div>
    </div>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
