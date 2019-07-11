<%@ Control Language="c#" AutoEventWireup="True"
	Inherits="YAF.Pages.Admin.editaccessmask" Codebehind="editaccessmask.ascx.cs" %>


<YAF:PageLinks runat="server" ID="PageLinks" />

    <div class="row">
        <div class="col-xl-12">
            <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" 
                                    LocalizedTag="TITLE" LocalizedPage="ADMIN_EDITACCESSMASKS" /></h1>
        </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-universal-access fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" 
                                                                                                         LocalizedTag="TITLE" 
                                                                                                         LocalizedPage="ADMIN_EDITACCESSMASKS" />
                </div>
                <div class="card-body">
                    <div class="form-row">
                        <div class="form-group col-md-6">
                            <YAF:HelpLabel ID="HelpLabel1" runat="server" 
                                           AssociatedControlID="Name"
                                           LocalizedTag="MASK_NAME" LocalizedPage="ADMIN_EDITACCESSMASKS" />
                            <asp:TextBox runat="server" ID="Name" CssClass="form-control" /><asp:RequiredFieldValidator
					runat="server" Text="<br />Enter name please!" ControlToValidate="Name" Display="Dynamic" />
                        </div>
                        <div class="form-group col-md-6">
                            <YAF:HelpLabel ID="HelpLabel2" runat="server" 
                                           AssociatedControlID="SortOrder"
                                           LocalizedTag="MASK_ORDER" LocalizedPage="ADMIN_EDITACCESSMASKS" />
                            <asp:TextBox runat="server" ID="SortOrder" MaxLength="5" CssClass="form-control" TextMode="Number" /><asp:RequiredFieldValidator ID="RequiredFieldValidator1"
					runat="server" Text="<br />Enter sort order please!" ControlToValidate="SortOrder" Display="Dynamic" />
                        </div>
                    </div>
                        <div class="form-row">
                            <div class="form-group col-md-2">
                                 
                                    <YAF:HelpLabel ID="HelpLabel3" runat="server" 
                                                   AssociatedControlID="ReadAccess"
                                                   LocalizedTag="READ_ACCESS" LocalizedPage="ADMIN_EDITACCESSMASKS" />
                                 
                                <div class="custom-control custom-switch">
                                    <asp:CheckBox runat="server" ID="ReadAccess" Text="&nbsp;" />
                                </div>
                            </div>
                            <div class="form-group col-md-2">
                                 
                                    <YAF:HelpLabel ID="HelpLabel4" runat="server" 
                                                   AssociatedControlID="PostAccess"
                                                   LocalizedTag="POST_ACCESS" LocalizedPage="ADMIN_EDITACCESSMASKS" />
                                 
                                <div class="custom-control custom-switch">
                                    <asp:CheckBox runat="server" ID="PostAccess" Text="&nbsp;" />
                                </div>
                            </div>
                            <div class="form-group col-md-2">
                                 
                                    <YAF:HelpLabel ID="HelpLabel5" runat="server" 
                                                   AssociatedControlID="ReplyAccess"
                                                   LocalizedTag="REPLY_ACCESS" LocalizedPage="ADMIN_EDITACCESSMASKS" />
                                 
                                <div class="custom-control custom-switch">
                                    <asp:CheckBox runat="server" ID="ReplyAccess" Text="&nbsp;" />
                                </div>
                            </div>
                            <div class="form-group col-md-2">
                                 
                                    <YAF:HelpLabel ID="HelpLabel6" runat="server" 
                                                   AssociatedControlID="PriorityAccess"
                                                   LocalizedTag="PRIORITY_ACCESS" LocalizedPage="ADMIN_EDITACCESSMASKS" />
                                 
                                <div class="custom-control custom-switch">
                                    <asp:CheckBox runat="server" ID="PriorityAccess" Text="&nbsp;" />
                                </div>
                            </div>
                            <div class="form-group col-md-2">
                                 
                                    <YAF:HelpLabel ID="HelpLabel7" runat="server" 
                                                   AssociatedControlID="PollAccess"
                                                   LocalizedTag="POLL_ACCESS" LocalizedPage="ADMIN_EDITACCESSMASKS" />
                                 
                                <div class="custom-control custom-switch">
                                    <asp:CheckBox runat="server" ID="PollAccess" Text="&nbsp;" />
                                </div>
                            </div>
                            <div class="form-group col-md-2">
                                 
                                    <YAF:HelpLabel ID="HelpLabel8" runat="server" 
                                                   AssociatedControlID="VoteAccess"
                                                   LocalizedTag="VOTE_ACCESS" LocalizedPage="ADMIN_EDITACCESSMASKS" />
                                 
                                <div class="custom-control custom-switch">
                                    <asp:CheckBox runat="server" ID="VoteAccess" Text="&nbsp;" />
                                </div>
                            </div>
                            </div>
                        <div class="form-row">
                            <div class="form-group col-md-2">
                                 
                                    <YAF:HelpLabel ID="HelpLabel9" runat="server" 
                                                   AssociatedControlID="ModeratorAccess"
                                                   LocalizedTag="MODERATOR_ACCESS" LocalizedPage="ADMIN_EDITACCESSMASKS" />
                                 
                                <div class="custom-control custom-switch">
                                    <asp:CheckBox runat="server" ID="ModeratorAccess" Text="&nbsp;" />
                                </div>
                            </div>
                            <div class="form-group col-md-2">
                                <YAF:HelpLabel ID="HelpLabel10" runat="server" 
                                               AssociatedControlID="EditAccess"
                                               LocalizedTag="EDIT_ACCESS" LocalizedPage="ADMIN_EDITACCESSMASKS" />
                                 
                                <div class="custom-control custom-switch">
                                    <asp:CheckBox runat="server" ID="EditAccess" Text="&nbsp;" />
                                </div>
                            </div>
                            <div class="form-group col-md-2">
                             
                                <YAF:HelpLabel ID="HelpLabel11" runat="server" 
                                               AssociatedControlID="DeleteAccess"
                                               LocalizedTag="DELETE_ACCESS" LocalizedPage="ADMIN_EDITACCESSMASKS" />
                             
                            <div class="custom-control custom-switch">
                                <asp:CheckBox runat="server" ID="DeleteAccess" Text="&nbsp;" />
                            </div>
                        </div>
                        <div class="form-group col-md-2">
                             
                                <YAF:HelpLabel ID="HelpLabel12" runat="server" 
                                               AssociatedControlID="UploadAccess"
                                               LocalizedTag="UPLOAD_ACCESS" LocalizedPage="ADMIN_EDITACCESSMASKS" />
                             
                            <div class="custom-control custom-switch">
                                <asp:CheckBox runat="server" ID="UploadAccess" Text="&nbsp;" />
                            </div>
                        </div>
                        <div class="form-group col-md-2">
                             
                                <YAF:HelpLabel ID="HelpLabel13" runat="server" 
                                               AssociatedControlID="DownloadAccess"
                                               LocalizedTag="DOWNLOAD_ACCESS" LocalizedPage="ADMIN_EDITACCESSMASKS" />
                             
                            <div class="custom-control custom-switch">
                                <asp:CheckBox runat="server" ID="DownloadAccess" Text="&nbsp;" />
                            </div>
                        </div>

                    </div>
                    </div>
                <div class="card-footer text-center">
				    <YAF:ThemeButton ID="Save" runat="server" OnClick="SaveClick" 
                        Type="Primary" Icon="save" TextLocalizedTag="SAVE" />
				    <YAF:ThemeButton ID="Cancel" runat="server" OnClick="CancelClick"
                        Type="Secondary" Icon="reply" TextLocalizedTag="CANCEL" />
                </div>
            </div>
        </div>
    </div>


