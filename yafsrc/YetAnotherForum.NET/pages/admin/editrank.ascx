<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.editrank" Codebehind="editrank.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
    <div class="row">
        <div class="col-xl-12">
            <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_EDITRANK" /></h1>
        </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-graduation-cap fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_EDITRANK" />
                </div>
                <div class="card-body">
			<h4>
				<YAF:HelpLabel ID="HelpLabel1" runat="server" LocalizedTag="RANK_NAME" LocalizedPage="ADMIN_EDITRANK" />
            </h4>
			<p>
				<asp:TextBox  ID="Name" runat="server" CssClass="form-control" />
		    </p><hr />

			<h4>
				<YAF:HelpLabel ID="HelpLabel2" runat="server" LocalizedTag="IS_START" LocalizedPage="ADMIN_EDITRANK" />
            </h4>
			<p>
				<asp:CheckBox ID="IsStart" runat="server" CssClass="form-control"></asp:CheckBox>
		    </p><hr />

			<h4>
				<YAF:HelpLabel ID="HelpLabel3" runat="server" LocalizedTag="LADDER_GROUP" LocalizedPage="ADMIN_EDITRANK" />
            </h4>
			<p>
				<asp:CheckBox ID="IsLadder" runat="server" CssClass="form-control"></asp:CheckBox>
		    </p><hr />

			<h4>
				<YAF:HelpLabel ID="HelpLabel4" runat="server" LocalizedTag="MIN_POSTS" LocalizedPage="ADMIN_EDITRANK" />
            </h4>
			<p>
				<asp:TextBox ID="MinPosts"  runat="server" CssClass="form-control" TextMode="Number" />
		    </p><hr />

			<h4>
				<YAF:HelpLabel ID="HelpLabel5" runat="server" LocalizedTag="PRIVATE_MESSAGES" LocalizedPage="ADMIN_EDITRANK" />
            </h4>
			<p>
				<asp:TextBox ID="PMLimit"  Text="0" runat="server" CssClass="form-control" TextMode="Number" />
		    </p><hr />

			<h4>
				<YAF:HelpLabel ID="HelpLabel6" runat="server" LocalizedTag="RANK_DESC" LocalizedPage="ADMIN_EDITRANK" />
            </h4>
			<p>
				<asp:TextBox ID="Description" runat="server" CssClass="form-control" />
		     </p><hr />

			<h4>
				<YAF:HelpLabel ID="HelpLabel7" runat="server" LocalizedTag="SIG_LENGTH" LocalizedPage="ADMIN_EDITRANK" />
            </h4>
			<p>
				<asp:TextBox ID="UsrSigChars" runat="server" CssClass="form-control" TextMode="Number" />
		    </p><hr />

			<h4>
				<YAF:HelpLabel ID="HelpLabel8" runat="server" LocalizedTag="SIG_BBCODE" LocalizedPage="ADMIN_EDITRANK" />
            </h4>
			<p>
				<asp:TextBox ID="UsrSigBBCodes" runat="server" CssClass="form-control" />

	        </p><hr />

			<h4>
				<YAF:HelpLabel ID="HelpLabel9" runat="server" LocalizedTag="SIG_HTML" LocalizedPage="ADMIN_EDITRANK" />
            </h4>
			<p>
				<asp:TextBox ID="UsrSigHTMLTags" runat="server" CssClass="form-control" />

			</p><hr />
			<h4>
				<YAF:HelpLabel ID="HelpLabel10" runat="server" LocalizedTag="ALBUMS_NUMBER" LocalizedPage="ADMIN_EDITRANK" />
            </h4>
			<p>
				<asp:TextBox ID="UsrAlbums" Text="0" runat="server" CssClass="form-control" TextMode="Number" />

		    </p><hr />
			<h4>
				<YAF:HelpLabel ID="HelpLabel11" runat="server" LocalizedTag="IMAGES_NUMBER" LocalizedPage="ADMIN_EDITRANK" />
            </h4>
			<p>
				<asp:TextBox ID="UsrAlbumImages" Text="0" runat="server" CssClass="form-control" TextMode="Number" />

		    </p><hr />
			<h4>
				<YAF:HelpLabel ID="HelpLabel12" runat="server" LocalizedTag="RANK_PRIO" LocalizedPage="ADMIN_EDITRANK" />
            </h4>
			<p>
				<asp:TextBox ID="RankPriority" Text="0" runat="server" CssClass="form-control" TextMode="Number" />
		   </p><hr />

			<h4>
				<YAF:HelpLabel ID="HelpLabel13" runat="server" LocalizedTag="RANK_STYLE" LocalizedPage="ADMIN_EDITRANK" />
            </h4>
			<p>
				<asp:TextBox ID="Style" Text="" TextMode="MultiLine" runat="server" CssClass="form-control" />
		    </p><hr />

			<h4>
				<YAF:HelpLabel ID="HelpLabel14" runat="server" LocalizedTag="RANK_IMAGE" LocalizedPage="ADMIN_EDITRANK" />
			</h4>
			<p>
				<asp:DropDownList ID="RankImage" runat="server" CssClass="custom-select" />
				<img style="vertical-align:middle" src="" alt="Rank Image" runat="server" id="Preview" />
			</p>

                </div>
                <div class="card-footer text-lg-center">
				    <YAF:ThemeButton ID="Save" runat="server" OnClick="Save_Click" Type="Primary"            
				                     Icon="save" TextLocalizedTag="SAVE"></YAF:ThemeButton>&nbsp;
				    <YAF:ThemeButton ID="Cancel" runat="server" OnClick="Cancel_Click" Type="Secondary"
				                     Icon="times" TextLocalizedTag="CANCEL"></YAF:ThemeButton>
                </div>
            </div>
        </div>
    </div>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
