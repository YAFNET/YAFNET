<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TopicStatusEdit.ascx.cs" Inherits="YAF.Dialogs.TopicStatusEdit" %>

<div class="modal fade" id="TopicStatusEditDialog" tabindex="-1" role="dialog" aria-labelledby="TopicStatusEditDialog" aria-hidden="true">
    <div class="modal-dialog" role="document">

                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">
                            <YAF:LocalizedLabel ID="Title" runat="server" 
                                LocalizedTag="TITLE" LocalizedPage="ADMIN_TOPICSTATUS_EDIT" />
                        </h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <!-- Modal Content START !-->
                        <h4>
                            <YAF:HelpLabel ID="HelpLabel1" runat="server" LocalizedTag="TOPICSTATUS_NAME" LocalizedPage="ADMIN_TOPICSTATUS_EDIT" />
                        </h4>
                        <p>
                            <asp:textbox id="TopicStatusName" runat="server" CssClass="form-control" MaxLength="100"></asp:textbox></p>
                        <hr />
                        <h4>
                            <YAF:HelpLabel ID="HelpLabel2" runat="server" LocalizedTag="DEFAULT_DESCRIPTION" LocalizedPage="ADMIN_TOPICSTATUS_EDIT" />
                        </h4>
                        <p>
                            <asp:textbox id="DefaultDescription" runat="server" CssClass="form-control" MaxLength="100"></asp:textbox>
                        </p>
                        <!-- Modal Content END !-->
                    </div>
                    <div class="modal-footer">
                        <YAF:ThemeButton id="Save" runat="server" OnClick="Save_OnClick" 
                            TextLocalizedTag="ADMIN_TOPICSTATUS_EDIT" TextLocalizedPage="TITLE"
                            Type="Primary" Icon="upload">
                        </YAF:ThemeButton>
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">
                            <i class="fa fa-times fa-fw"></i>&nbsp;<YAF:LocalizedLabel runat="server" LocalizedTag="CANCEL"></YAF:LocalizedLabel>
                        </button>
                    </div>
                </div>
    </div>
</div>
