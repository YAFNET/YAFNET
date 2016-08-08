<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.attachments" Codebehind="attachments.ascx.cs" %>

<%@ Import Namespace="YAF.Core" %>
<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">

    <div class="row">
    <div class="col-xl-12">
        <h1 class="page-header"><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_ATTACHMENTS" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
             <YAF:Pager ID="PagerTop" runat="server" OnPageChange="PagerTop_PageChange" />
            <div class="card card-primary-outline">
                <div class="card-header card-primary">
                    <i class="fa fa-paperclip fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_ATTACHMENTS" />
			</div>
                <div class="card-block">
                    <div class="alert alert-info hidden-sm-up" role="alert">
                            <YAF:LocalizedLabel ID="LocalizedLabel220" runat="server" LocalizedTag="TABLE_RESPONSIVE" LocalizedPage="ADMIN_COMMON" />
                            <span class="pull-right"><i class="fa fa-hand-o-left fa-fw"></i></span>
                        </div><div class="table-responsive">
                        <table class="table">
		<asp:Repeater runat="server" ID="List">
			<HeaderTemplate>
				<tr>
				    <thead>
					<th>
						<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="FORUM" />
					</th>
					<th>
						<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="TOPIC" />
					</th>
					<th>
						<YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="POSTED" LocalizedPage="ADMIN_ATTACHMENTS" />
					</th>
                    <th>
						<YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="USER" />
					</th>
					<th>
						<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="FILENAME" />
					</th>
					<th>
						<YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="DOWNLOADS" />
					</th>
					<th>
						<YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="CONTENT_TYPE" />
					</th>
					<th>
						<YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="SIZE" />
					</th>
					<th>
						&nbsp;
					</th>
                    </thead>
				</tr>
			</HeaderTemplate>
			<ItemTemplate>
				<tr>
					<td>
						<a target="_top" href='<%# YafBuildLink.GetLink(ForumPages.topics,"f={0}&name={1}",this.Eval("ForumID"), this.Eval("ForumName")) %>'>
							<%# this.HtmlEncode(this.Eval("ForumName")) %>
						</a>
					</td>
					<td>
						<a target="_top" href='<%# YafBuildLink.GetLink(ForumPages.posts,"t={0}",this.Eval("TopicID")) %>'>
							<%# this.HtmlEncode(this.Eval("TopicName")) %>
						</a>
					</td>
					<td>
						<%# this.Get<IDateTime>().FormatDateTimeShort(this.Eval("Posted")) %>
					</td>
                    <td>
						<YAF:UserLink ID="UserProfileLink" runat="server" UserID='<%# this.Eval("UserID").ToType<int>() %>' />
					</td>
					<td>
						<%# this.HtmlEncode(this.Eval( "FileName")) %>
					</td>
					<td>
						<%# this.Eval( "Downloads") %>
					</td>
					<td>
						<%# this.Eval( "ContentType") %>
					</td>
					<td>
						<%# this.Eval( "Bytes") %>
					</td>
					<td>
						<YAF:ThemeButton ID="ThemeButtonDelete" CssClass="btn btn-danger btn-sm"
                                    CommandName='delete' CommandArgument='<%# this.Eval( "AttachmentID") %>'
                                    TitleLocalizedTag="DELETE"
                                    Icon="trash"
                                    TextLocalizedTag="DELETE"
                                    OnLoad="Delete_Load"  runat="server">
                                </YAF:ThemeButton>
					</td>
				</tr>
			</ItemTemplate>
            <FooterTemplate>

            </FooterTemplate>
		</asp:Repeater>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>
     <YAF:Pager ID="PagerBottom" runat="server" LinkedPager="PagerTop" />
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
