<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.ranks" Codebehind="ranks.ascx.cs" %>


<%@ Import Namespace="YAF.Core" %>
<%@ Import Namespace="YAF.Types.Flags" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<YAF:PageLinks runat="server" ID="PageLinks" />

    <div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_RANKS" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-graduation-cap fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_RANKS" />
                </div>
                <div class="card-body">
		<asp:Repeater ID="RankList" OnItemCommand="RankListItemCommand" runat="server">
			<HeaderTemplate>
                <ul class="list-group">
			</HeaderTemplate>
			<ItemTemplate>
				<li class="list-group-item list-group-item-action">
                <div class="d-flex w-100 justify-content-between">
                    <h5 class="mb-1 text-break">
                        <i class="fa fa-graduation-cap fa-fw"></i>&nbsp;
						<%# this.Eval( "Name") %>
                    </h5>
                </div>
                <p>
                     <YAF:LocalizedLabel ID="HelpLabel6" Visible='<%# ((YAF.Types.Models.Rank)Container.DataItem).Description.IsSet() %>' runat="server" LocalizedTag="DESCRIPTION" LocalizedPage="ADMIN_EDITGROUP">
                         </YAF:LocalizedLabel>
                          &nbsp;<%# this.Eval("Description") %>&nbsp;
                    <br />
                    <YAF:LocalizedLabel  ID="HelpLabel12" runat="server" LocalizedTag="PRIORITY" LocalizedPage="ADMIN_EDITGROUP" />
                    <asp:Label ID="Label11" runat="server" CssClass='<%# this.GetItemColorString(this.Eval( "SortOrder" ).ToString()) %>'><%# this.Eval("SortOrder").ToString()%></asp:Label>&nbsp;|&nbsp;
                    <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="IS_START" LocalizedPage="ADMIN_RANKS" />
                     <asp:Label ID="Label4" runat="server" CssClass='<%# this.GetItemColor(this.Eval( "Flags" ).BinaryAnd(1)) %>'><%# this.GetItemName(this.Eval( "Flags" ).BinaryAnd(1)) %></asp:Label>&nbsp;|&nbsp;
                    <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="IS_LADDER" LocalizedPage="ADMIN_RANKS" />
                    <asp:Label ID="Label1" runat="server" CssClass='<%# this.GetItemColor(this.Eval( "Flags" ).BinaryAnd(RankFlags.Flags.IsLadder)) %>'><%# this.LadderInfo(Container.DataItem) %></asp:Label>&nbsp;|&nbsp;
                    <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="PM_LIMIT" LocalizedPage="ADMIN_RANKS" />
					<asp:Label ID="Label6" runat="server" CssClass='<%# this.GetItemColorString( ((YAF.Types.Models.Rank)Container.DataItem).PMLimit == int.MaxValue ? "\u221E" : ((YAF.Types.Models.Rank)Container.DataItem).PMLimit.ToString()) %>'><%#  ((YAF.Types.Models.Rank)Container.DataItem).PMLimit == int.MaxValue ? "\u221E": ((YAF.Types.Models.Rank)Container.DataItem).PMLimit.ToString()%></asp:Label>&nbsp;|&nbsp;
                    <br />
                    <YAF:LocalizedLabel  ID="HelpLabel10" runat="server" LocalizedTag="ALBUM_NUMBER" LocalizedPage="ADMIN_EDITGROUP" />
                    <asp:Label ID="Label9" runat="server" CssClass='<%# this.GetItemColorString(this.Eval( "UsrAlbums" ).ToString()) %>'><%# this.Eval("UsrAlbums").ToString()%></asp:Label>&nbsp;|&nbsp;
                    <YAF:LocalizedLabel  ID="HelpLabel11" runat="server" LocalizedTag="IMAGES_NUMBER" LocalizedPage="ADMIN_EDITGROUP" />
                    <asp:Label ID="Label10" runat="server" CssClass='<%# this.GetItemColorString(this.Eval( "UsrAlbumImages" ).ToString()) %>'><%# this.Eval("UsrAlbumImages").ToString()%></asp:Label>&nbsp;|&nbsp;
                    <br />
                    <YAF:LocalizedLabel  ID="HelpLabel13" runat="server" LocalizedTag="STYLE" LocalizedPage="ADMIN_EDITGROUP" />&nbsp;
                    <asp:Label ID="Label12" runat="server" CssClass='<%# this.GetItemColorString(((YAF.Types.Models.Rank)Container.DataItem).Style) %>'><%# ((YAF.Types.Models.Rank)Container.DataItem).Style.IsSet() && ((YAF.Types.Models.Rank)Container.DataItem).Style.Trim().Length > 0 ? "" : this.GetItemName(false)%></asp:Label>&nbsp;
                    <YAF:RoleRankStyles ID="RoleRankStylesRanks" RawStyles='<%# ((YAF.Types.Models.Rank)Container.DataItem).Style %>' runat="server" />
                    <br />
					<YAF:LocalizedLabel ID="HelpLabel7" runat="server" LocalizedTag="SIGNATURE_LENGTH" LocalizedPage="ADMIN_EDITGROUP" />
                    <asp:Label ID="Label5" runat="server" CssClass='<%# this.GetItemColorString(((YAF.Types.Models.Rank)Container.DataItem).UsrSigChars.ToString()) %>'><%# ((YAF.Types.Models.Rank)Container.DataItem).UsrSigChars.ToString().IsSet() ? this.Eval("UsrSigChars").ToString() : this.GetItemName(false) %></asp:Label>&nbsp;|&nbsp;
                    <YAF:LocalizedLabel ID="HelpLabel8" runat="server" LocalizedTag="SIG_BBCODES" LocalizedPage="ADMIN_EDITGROUP" />
                    <asp:Label ID="Label7" runat="server" CssClass='<%# this.GetItemColorString(((YAF.Types.Models.Rank)Container.DataItem).UsrSigBBCodes) %>'><%# ((YAF.Types.Models.Rank)Container.DataItem).UsrSigBBCodes.IsSet() ? this.Eval("UsrSigBBCodes").ToString() : this.GetItemName(false) %></asp:Label>&nbsp;|&nbsp;
                    <YAF:LocalizedLabel ID="HelpLabel9" runat="server"  LocalizedTag="SIG_HTML" LocalizedPage="ADMIN_EDITGROUP" />
                    <asp:Label ID="Label8" runat="server" CssClass='<%# this.GetItemColorString(((YAF.Types.Models.Rank)Container.DataItem).UsrSigHTMLTags) %>'><%#  ((YAF.Types.Models.Rank)Container.DataItem).UsrSigHTMLTags.IsSet() ? this.Eval("UsrSigHTMLTags").ToString() : this.GetItemName(false)%></asp:Label>&nbsp;|&nbsp;
                </p>
                <small>
                    <YAF:ThemeButton ID="ThemeButtonEdit" runat="server"
                                     Type="Info" 
                                     Size="Small"
                                     CommandName='edit' CommandArgument='<%# this.Eval( "ID") %>'
                                     TitleLocalizedTag="EDIT"
                                     Icon="edit"
                                     TextLocalizedTag="EDIT">
					    </YAF:ThemeButton>
						<YAF:ThemeButton ID="ThemeButtonDelete" runat="server" 
                                         Type="Danger"
                                         Size="Small"
                                    CommandName='delete' CommandArgument='<%# this.Eval( "ID") %>'
                                    TitleLocalizedTag="DELETE"
                                    Icon="trash"
                                    TextLocalizedTag="DELETE"
                                    ReturnConfirmText='<%# this.GetText("ADMIN_RANKS", "CONFIRM_DELETE") %>'>
                                </YAF:ThemeButton>
                </small>
            </li>
			</ItemTemplate>
            <FooterTemplate>
                </ul>
            </FooterTemplate>
		</asp:Repeater>
                </div>
                <div class="card-footer text-center">
				   <YAF:ThemeButton ID="NewRank" runat="server" OnClick="NewRankClick" Type="Primary"
				                    Icon="plus-square"
				                    TextLocalizedTag="NEW_RANK" />
                </div>
            </div>
        </div>
    </div>


