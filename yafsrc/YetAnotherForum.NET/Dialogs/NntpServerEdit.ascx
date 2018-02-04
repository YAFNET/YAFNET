<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NntpServerEdit.ascx.cs" Inherits="YAF.Dialogs.NntpServerEdit" %>

<div class="modal fade" id="NntpServerEditDialog" tabindex="-1" role="dialog" aria-labelledby="NntpServerEditDialog" aria-hidden="true">
    <div class="modal-dialog" role="document">

                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">
                            <YAF:LocalizedLabel ID="Title" runat="server" 
                                LocalizedTag="TITLE" LocalizedPage="ADMIN_EDITNNTPSERVER" />
                        </h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <!-- Modal Content START !-->
                        <h4>
                            <YAF:HelpLabel ID="LocalizedLabel2" runat="server" LocalizedTag="NNTP_NAME" LocalizedPage="ADMIN_EDITNNTPSERVER" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="Name" runat="server" />
                        </p><hr />
		
                        <h4>
                            <YAF:HelpLabel ID="LocalizedLabel3" runat="server" LocalizedTag="NNTP_ADRESS" LocalizedPage="ADMIN_EDITNNTPSERVER" />
                        </h4>
                        <p>
                            <asp:TextBox CssClass="form-control" ID="Address" runat="server" />
                        </p><hr />
		
                        <h4>
                            <YAF:HelpLabel ID="LocalizedLabel4" runat="server" LocalizedTag="NNTP_PORT" LocalizedPage="ADMIN_EDITNNTPSERVER" />
                        </h4>
                        <p>
                            <asp:TextBox  ID="Port" runat="server" CssClass="form-control" TextMode="Number" />
                        </p><hr />
		
                        <h4>
                            <YAF:HelpLabel ID="LocalizedLabel5" runat="server" LocalizedTag="NNTP_USERNAME" LocalizedPage="ADMIN_EDITNNTPSERVER" />
                        </h4>
                        <p>
                            <asp:TextBox  CssClass="form-control" ID="UserName" runat="server" Enabled="true" />
                        </p><hr />
		
                        <h4>
                            <YAF:HelpLabel ID="LocalizedLabel6" runat="server" LocalizedTag="NNTP_PASSWORD" LocalizedPage="ADMIN_EDITNNTPSERVER" />
                        </h4>
                        <p>
                            <asp:TextBox  CssClass="form-control" ID="UserPass" runat="server" Enabled="true" />
                        </p>
                        <!-- Modal Content END !-->
                    </div>
                    <div class="modal-footer">
                        <YAF:ThemeButton id="Save" runat="server" OnClick="Save_OnClick" 
                            TextLocalizedTag="ADMIN_EDITNNTPSERVER" TextLocalizedPage="TITLE"
                            Type="Primary" Icon="upload">
                        </YAF:ThemeButton>
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">
                            <i class="fa fa-times fa-fw"></i>&nbsp;<YAF:LocalizedLabel runat="server" LocalizedTag="CANCEL"></YAF:LocalizedLabel>
                        </button>
                    </div>
                </div>
    </div>
</div>
