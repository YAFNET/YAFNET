<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.DeleteForum"
    CodeBehind="DeleteForum.ascx.cs" %>


<YAF:PageLinks runat="server" ID="PageLinks" />

    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <YAF:IconHeader runat="server" ID="IconHeader"
                                    IconName="comments" LocalizedTag="TITLE" LocalizedPage="ADMIN_DELETEFORUM"></YAF:IconHeader>
                </div>
                <div class="card-body">
                    <div class="mb-3">
                        <YAF:HelpLabel ID="HelpLabel11" runat="server"
                                       AssociatedControlID="MoveTopics"
                                       LocalizedTag="MOVE_TOPICS" LocalizedPage="ADMIN_DELETEFORUM" />
                        <div class="form-check form-switch">
                            <asp:CheckBox ID="MoveTopics" runat="server"
                                          Text="&nbsp;"></asp:CheckBox>
                        </div>
                    </div>
                    <div class="mb-3">
                        <YAF:HelpLabel ID="HelpLabel2" runat="server"
                                       LocalizedTag="NEW_FORUM" LocalizedPage="ADMIN_DELETEFORUM" />
                        <select id="ForumList" name="forumList"></select>
                        <asp:HiddenField runat="server" ID="ForumListSelected" Value="-1" />
                    </div>
                </div>
                <div class="card-footer text-center">
                    <YAF:ThemeButton ID="Delete" runat="server"
                                     CssClass="btn btn-danger"
                                     Icon="trash"
                                     TextLocalizedTag="DELETE_FORUM" TextLocalizedPage="ADMIN_DELETEFORUM">
                    </YAF:ThemeButton>
                    <YAF:ThemeButton ID="Cancel" runat="server"
                                     Type="Secondary"
                                     Icon="times"
                                     TextLocalizedTag="CANCEL"></YAF:ThemeButton>
                </div>
            </div>
        </div>
    </div>


<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <asp:Timer ID="UpdateStatusTimer" runat="server" Enabled="false" Interval="4000" OnTick="UpdateStatusTimerTick" />
    </ContentTemplate>
</asp:UpdatePanel>

<div class="modal fade" id="DeleteForumMessage" aria-hidden="true" aria-labelledby="MessageToggleLabel" tabindex="-1">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header">
                <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="DELETE_TITLE" LocalizedPage="ADMIN_DELETEFORUM" />
            </div>
            <div class="modal-body text-center">
                <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="DELETE_MSG" LocalizedPage="ADMIN_DELETEFORUM" />
                <div class="fa-3x"><i class="fas fa-spinner fa-pulse"></i></div>
            </div>
        </div>
    </div>
</div>