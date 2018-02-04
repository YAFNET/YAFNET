<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NntpForumEdit.ascx.cs" Inherits="YAF.Dialogs.NntpForumEdit" %>

<div class="modal fade" id="NntpForumEditDialog" tabindex="-1" role="dialog" aria-labelledby="NntpForumEditDialog" aria-hidden="true">
    <div class="modal-dialog" role="document">

                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">
                            <YAF:LocalizedLabel ID="Title" runat="server" 
                                LocalizedTag="TITLE" LocalizedPage="ADMIN_EDITNNTPFORUM" />
                        </h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <!-- Modal Content START !-->
                        <h4>
                            <YAF:HelpLabel ID="LocalizedLabel2" runat="server" LocalizedTag="SERVER" LocalizedPage="ADMIN_EDITNNTPFORUM" />
                        </h4>
                        <p>
                            <asp:DropDownList ID="NntpServerID" runat="server" CssClass="custom-select" />
                        </p>
                        <hr />
                        <h4>
                            <YAF:HelpLabel ID="LocalizedLabel3" runat="server" LocalizedTag="GROUP" LocalizedPage="ADMIN_EDITNNTPFORUM" />
                        </h4>
                        <p>
                            <asp:TextBox ID="GroupName" runat="server"  CssClass="form-control" />
                        </p>
                        <hr />
                        <h4>
                            <YAF:HelpLabel ID="LocalizedLabel4" runat="server" LocalizedTag="FORUM" LocalizedPage="ADMIN_EDITNNTPFORUM" />
                        </h4>
                        <p>
                            <asp:DropDownList ID="ForumID" runat="server"  CssClass="custom-select" />
                        </p>
                        <hr />
                        <h4>
                            <YAF:HelpLabel ID="HelpLabel10" runat="server" LocalizedTag="DATECUTOFF" LocalizedPage="ADMIN_EDITNNTPFORUM" />
                        </h4>
                        <p>
                            <asp:TextBox  CssClass="form-control" ID="DateCutOff" runat="server" Enabled="true" TextMode="DateTime" />
                        </p>
                        <hr />
                        <h4>
                            <YAF:HelpLabel ID="LocalizedLabel5" runat="server" LocalizedTag="ACTIVE" LocalizedPage="ADMIN_EDITNNTPFORUM" />
                        </h4>
                        <p>
                            <asp:CheckBox ID="Active" runat="server" Checked="true" CssClass="form-control" />
                        </p>
                        <!-- Modal Content END !-->
                    </div>
                    <div class="modal-footer">
                        <YAF:ThemeButton id="Save" runat="server" OnClick="Save_OnClick" 
                            TextLocalizedTag="ADMIN_EDITNNTPFORUM" TextLocalizedPage="TITLE"
                            Type="Primary" Icon="upload">
                        </YAF:ThemeButton>
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">
                            <i class="fa fa-times fa-fw"></i>&nbsp;<YAF:LocalizedLabel runat="server" LocalizedTag="CANCEL"></YAF:LocalizedLabel>
                        </button>
                    </div>
                </div>
    </div>
</div>
