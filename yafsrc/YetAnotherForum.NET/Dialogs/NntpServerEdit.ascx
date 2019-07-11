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
                        <div class="form-row">
                            <div class="form-group col-md-6">
                                <YAF:HelpLabel ID="LocalizedLabel2" runat="server"
                                               AssociatedControlID="Name"
                                               LocalizedTag="NNTP_NAME" LocalizedPage="ADMIN_EDITNNTPSERVER" />
                                <asp:TextBox CssClass="form-control" ID="Name" runat="server" />
                            </div>
                            <div class="form-group col-md-6">
                                <YAF:HelpLabel ID="LocalizedLabel3" runat="server"
                                               AssociatedControlID="Address"
                                               LocalizedTag="NNTP_ADRESS" LocalizedPage="ADMIN_EDITNNTPSERVER" />
                                <asp:TextBox CssClass="form-control" ID="Address" runat="server" />
                            </div>
                        </div>
                        <div class="form-group">
                            <YAF:HelpLabel ID="LocalizedLabel4" runat="server"
                                           AssociatedControlID="Port"
                                           LocalizedTag="NNTP_PORT" LocalizedPage="ADMIN_EDITNNTPSERVER" />
                            <asp:TextBox  ID="Port" runat="server" 
                                          CssClass="form-control" 
                                          TextMode="Number" />
                        </div>
                        <div class="form-row">
                            <div class="form-group col-md-6">
                                <YAF:HelpLabel ID="LocalizedLabel5" runat="server"
                                               AssociatedControlID="UserName"
                                               LocalizedTag="NNTP_USERNAME" LocalizedPage="ADMIN_EDITNNTPSERVER" />
                                <asp:TextBox ID="UserName" runat="server"
                                             Enabled="true"
                                             CssClass="form-control" />
                            </div>
                            <div class="form-group col-md-6">
                                <YAF:HelpLabel ID="LocalizedLabel6" runat="server"
                                               AssociatedControlID="UserPass"
                                               LocalizedTag="NNTP_PASSWORD" LocalizedPage="ADMIN_EDITNNTPSERVER" />
                                <asp:TextBox ID="UserPass" runat="server"
                                             CssClass="form-control"
                                             Enabled="true" />
                            </div>
                        </div>
                        <!-- Modal Content END !-->
                    </div>
                    <div class="modal-footer">
                        <YAF:ThemeButton id="Save" runat="server"
                                         OnClick="Save_OnClick"
                                         TextLocalizedTag="ADMIN_EDITNNTPSERVER" TextLocalizedPage="TITLE"
                                         Type="Primary" Icon="save">
                        </YAF:ThemeButton>
                        <YAF:ThemeButton runat="server" ID="Cancel"
                                         DataDismiss="modal"
                                         TextLocalizedTag="CANCEL"
                                         Type="Secondary"
                                         Icon="times">
                        </YAF:ThemeButton>
                    </div>
                </div>
    </div>
</div>
