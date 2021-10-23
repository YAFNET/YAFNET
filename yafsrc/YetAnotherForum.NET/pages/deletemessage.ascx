<%@ Control Language="c#" AutoEventWireup="True"
    Inherits="YAF.Pages.DeleteMessage" Codebehind="DeleteMessage.ascx.cs" %>
<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Types.Models" %>
<%@ Import Namespace="YAF.Core.Extensions" %>
<%@ Import Namespace="YAF.Types.Interfaces.Services" %>
<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <h2><asp:Label ID="Title" runat="server" /></h2>
    </div>
</div>

<div class="row">
    <div class="col">
        <div class="card mb-3">
            <div class="card-header">
                <YAF:Icon runat="server" 
                          IconStackName="trash"
                          IconStackType="fa-inverse"
                          IconName="comment"
                          IconStackSize="fa-1x"
                          IconType="text-secondary"></YAF:Icon>
                <YAF:LocalizedLabel runat="server" LocalizedTag="subject" />&nbsp;<asp:Label runat="server" ID="Subject" />
            </div>
            <div class="card-body">
                <form>
                    <asp:PlaceHolder runat="server" id="PreviewRow" visible="false">
                    <div class="mb-3">
                        <asp:Label runat="server">
                            <YAF:LocalizedLabel runat="server" LocalizedTag="previewtitle" />
                        </asp:Label>
                        <div class="card bg-light">
                            <div class="card-body">
                                <YAF:MessagePost ID="MessagePreview" runat="server">
                                </YAF:MessagePost>
                            </div>
                        </div>
                    </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder runat="server" id="DeleteReasonRow">
                    <div class="mb-3">
                        <asp:Label runat="server" AssociatedControlID="ReasonEditor">
                            <% = this.GetReasonText() %>
                        </asp:Label>
                        <asp:TextBox ID="ReasonEditor" runat="server" 
                                     CssClass="form-control" 
                                     MaxLength="100"
                                     required="required" />
                        <div class="invalid-feedback">
                            <YAF:LocalizedLabel runat="server"
                                                LocalizedTag="NEED_REASON" />
                        </div>
                    </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder id="EraseRow" runat="server" visible="false">
                        <div class="mb-3">
                            <asp:CheckBox ID="EraseMessage" runat="server" 
                                          Checked="false"
                                          CssClass="form-check"/>
                        </div>
                    </asp:PlaceHolder>
                </form>
            </div>
            <div class="card-footer text-center">
                <YAF:ThemeButton ID="DeleteUndelete" runat="server" 
                                 OnClick="ToggleDelete_Click"
                                 CausesValidation="True"
                                 DataToggle="tooltip"
                                 Type="Danger"
                                 Visible="False"
                                 Icon="trash"/>
                <YAF:ThemeButton ID="Delete" runat="server" 
                                 OnClick="ToggleDeleteStatus_Click"
                                 CausesValidation="True"
                                 Type="Danger"
                                 Icon="trash"/>
                <YAF:ThemeButton ID="Cancel" runat="server" 
                                 OnClick="Cancel_Click"
                                 TextLocalizedTag="CANCEL"
                                 Type="Secondary"
                                 Icon="reply"/>
            </div>
        </div>
    </div>
</div>

<asp:Repeater ID="LinkedPosts" runat="server" Visible="false">
    <HeaderTemplate>
        <div class="row">
        <div class="col">
        <div class="card mb-3">
        <div class="card-header">
            <YAF:Icon runat="server" 
                      IconStackName="trash"
                      IconStackType="fa-inverse"
                      IconName="comment"
                      IconStackSize="fa-1x"
                      IconType="text-secondary"></YAF:Icon>
            <asp:CheckBox ID="DeleteAllPosts" runat="server" 
                          CssClass="form-check d-inline-block" Text='<%# this.GetText("DELETE_ALL") %>' />
            
        </div>
        <div class="card-body">
    </HeaderTemplate>
    <FooterTemplate>
        </div>
        </div>
        </div>
        </div>
    </FooterTemplate>
    <ItemTemplate>
       <div class="card my-3">
        <div class="card-body">
            <div class="card-title h5">
                <footer class="blockquote-footer">
                    <YAF:UserLink ID="ProfileLink" runat="server" 
                                  Suspended="<%# ((Tuple<Message, User>)Container.DataItem).Item2.Suspended %>"
                                  Style="<%# ((Tuple<Message, User>)Container.DataItem).Item2.UserStyle %>"
                                  UserID="<%# ((Tuple<Message, User>)Container.DataItem).Item1.UserID %>"
                                  ReplaceName="<%# ((Tuple<Message, User>)Container.DataItem).Item2.DisplayOrUserName() %>"
                                  BlankTarget="true" />
                    <small class="text-muted">
                        <YAF:Icon runat="server" 
                                  IconName="calendar-day"
                                  IconNameBadge="clock"></YAF:Icon>
                        <%# this.Get<IDateTimeService>().FormatDateTime(((Tuple<Message, User>)Container.DataItem).Item1.Posted)%>
                    </small>
                </footer>
            </div>
            <div class="card-text">
                <YAF:MessagePostData ID="MessagePost1" runat="server" 
                                     CurrentMessage="<%# ((Tuple<Message, User>)Container.DataItem).Item1 %>"
                                     ShowAttachments="false" 
                                     ShowSignature="false">
                </YAF:MessagePostData>
            </div>
        </div>
        </div>
    </ItemTemplate>
</asp:Repeater>