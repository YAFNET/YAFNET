<%@ Control Language="C#" AutoEventWireup="true" Inherits="YAF.Controls.ViewThanksList"
    CodeBehind="ViewThanksList.ascx.cs" %>

<%@ Import Namespace="YAF.Core" %>
<%@ Import Namespace="YAF.Types.Constants" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<%@ Import Namespace="System.Data" %>

<div class="row">
    <div class="col">
        <YAF:Pager runat="server" ID="PagerTop" OnPageChange="Pager_PageChange" />
    </div>
</div>

<asp:Repeater ID="ThanksRes" runat="server" OnItemCreated="ThanksRes_ItemCreated">
    <HeaderTemplate>
        <div class="row">
            <div class="col">
    </HeaderTemplate>
    <ItemTemplate>
        <div class="card w-100 mb-3">
            <div class="card-header">
                <h6>
                    <YAF:ThemeButton runat="server"
                                     TitleLocalizedPage="COMMON" TitleLocalizedTag="VIEW_TOPIC"
                                     NavigateUrl='<%# YafBuildLink.GetLink(ForumPages.posts,"m={0}#post{0}", Container.DataItemToField<int>("MessageID")) %>'
                                     Type="Link"
                                     Text='<%# this.HtmlEncode(Container.DataItemToField<string>("Topic")) %>'>
                    </YAF:ThemeButton>
                    <small class="text-muted">
                        <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="POSTED" />
                        <YAF:DisplayDateTime ID="PostedDateTime" runat="server" 
                                             DateTime='<%# Container.DataItemToField<DateTime>("Posted") %>'>
                        </YAF:DisplayDateTime>
                    </small>
                </h6>
            </div>
            <div class="card-body">
                <p class="card-text">
                    <YAF:MessagePostData ID="MessagePostPrimary" runat="server" ShowAttachments="false"
                                         ShowSignature="false" DataRow="<%# (DataRow)Container.DataItem %>">
                    </YAF:MessagePostData>
                </p>
            </div><div class="card-footer">
                <small class="text-muted">
                    <asp:PlaceHolder id="ThanksNumberCell" runat="server">
                        <i class="fas fa-heart text-danger"></i>
                        <%# string.Format(this.GetText("THANKSNUMBER"),  Container.DataItemToField<int?>("MessageThanksNumber")) %>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder id="NameCell" runat="server">
                        <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="POSTEDBY" />
                        <YAF:UserLink ID="UserLink1" runat="server" UserID='<%# Container.DataItemToField<int>("UserID") %>' />
                    </asp:PlaceHolder>
                </small>
                
            </div>
        </div>
    </ItemTemplate>
    <FooterTemplate>
            </div>
        </div>
    </FooterTemplate>
</asp:Repeater>
<asp:PlaceHolder ID="NoResults" runat="Server" Visible="false">
    <YAF:Alert runat="server" Type="info">
        <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="NO_THANKS" />
    </YAF:Alert>
</asp:PlaceHolder>

<div class="row">
    <div class="col">
        <YAF:Pager runat="server" ID="PagerBottom" LinkedPager="PagerTop" OnPageChange="Pager_PageChange" />
    </div>
</div>
