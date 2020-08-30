<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.EditBoard" Codebehind="EditBoard.ascx.cs" %>


<YAF:PageLinks runat="server" ID="PageLinks" />

    <asp:UpdatePanel ID="UppdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <YAF:IconHeader runat="server"
                                    IconName="globe"
                                    LocalizedPage="ADMIN_EDITBOARD"></YAF:IconHeader>
                </div>
                <div class="card-body">
                    <div class="row">
                        <div class="mb-3 col-md-6">
                            <YAF:HelpLabel ID="HelpLabel1" runat="server" 
                                           AssociatedControlID="Name"
                                           LocalizedTag="NAME" LocalizedPage="ADMIN_EDITBOARD" />
                            <asp:TextBox ID="Name" runat="server"
                                         required="required" 
                                         CssClass="form-control" />
                            <div class="invalid-feedback">
                                <YAF:LocalizedLabel runat="server" 
                                                    LocalizedTag="MSG_NAME_BOARD" />
                            </div>
                        </div>
                        <div class="mb-3 col-md-6">
                            <YAF:HelpLabel ID="HelpLabel2" runat="server" 
                                           AssociatedControlID="Culture"
                                           LocalizedTag="CULTURE" LocalizedPage="ADMIN_EDITBOARD" />
                            <asp:DropDownList ID="Culture" runat="server" 
                                              CssClass="select2-select" />
                        </div>
                    </div>
                    <asp:PlaceHolder runat="server" ID="CreateNewAdminHolder">
                        <div class="mb-3">
                            <YAF:HelpLabel ID="HelpLabel5" runat="server" 
                                           AssociatedControlID="CreateAdminUser"
                                           LocalizedTag="ADMIN_USER" LocalizedPage="ADMIN_EDITBOARD" />
             
                            <div class="form-check form-switch">
                                <asp:CheckBox runat="server" ID="CreateAdminUser" 
                                              AutoPostBack="true" 
                                              OnCheckedChanged="CreateAdminUserCheckedChanged" 
                                              Text="&nbsp;" />
                            </div> 
                        </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder runat="server" ID="AdminInfo" Visible="false">
                        <h3>
                            <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" 
                                                LocalizedTag="HEADER2" LocalizedPage="ADMIN_EDITBOARD" />
                        </h3>
                        <div class="row">
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="HelpLabel6" runat="server" 
                                               AssociatedControlID="UserName"
                                               LocalizedTag="USER_NAME" LocalizedPage="ADMIN_EDITBOARD" />
                                <asp:TextBox runat="server" ID="UserName" 
                                             required="required"
                                             CssClass="form-control" />
                                <div class="invalid-feedback">
                                    <YAF:LocalizedLabel runat="server" 
                                                        LocalizedTag="MSG_NAME_ADMIN" />
                                </div>
                            </div>
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="HelpLabel7" runat="server" 
                                               AssociatedControlID="UserEmail"
                                               LocalizedTag="USER_MAIL" LocalizedPage="ADMIN_EDITBOARD" />
                                <asp:TextBox runat="server" ID="UserEmail" 
                                             TextMode="Email"
                                             required="required"
                                             CssClass="form-control" />
                                <div class="invalid-feedback">
                                    <YAF:LocalizedLabel runat="server" 
                                                        LocalizedTag="MSG_EMAIL_ADMIN" />
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="HelpLabel8" runat="server" 
                                               AssociatedControlID="UserPass1"
                                               LocalizedTag="USER_PASS" LocalizedPage="ADMIN_EDITBOARD" />
                                <asp:TextBox runat="server" ID="UserPass1" 
                                             TextMode="password" 
                                             required="required"
                                             CssClass="form-control" />
                                <div class="invalid-feedback">
                                    <YAF:LocalizedLabel runat="server" 
                                                        LocalizedTag="MSG_PASS_ADMIN" />
                                </div>
                            </div>
                            <div class="mb-3 col-md-6">
                                <YAF:HelpLabel ID="HelpLabel9" runat="server" 
                                               AssociatedControlID="UserPass2"
                                               LocalizedTag="VERIFY_PASS" LocalizedPage="ADMIN_EDITBOARD" />
                                <asp:TextBox runat="server" ID="UserPass2" 
                                             required="required"
                                             TextMode="password" 
                                             CssClass="form-control" />
                                <div class="invalid-feedback">
                                    <YAF:LocalizedLabel runat="server" 
                                                        LocalizedTag="MSG_PASS_ADMIN" />
                                </div>
                            </div>
                        </div>
                    </asp:PlaceHolder>
                </div>
                <div class="card-footer text-center">
                    <YAF:ThemeButton ID="Save" runat="server" 
                                     OnClick="SaveClick" 
                                     CausesValidation="True"
                                     Type="Primary"
                                     Icon="save" 
                                     TextLocalizedTag="SAVE"/>
                    <YAF:ThemeButton ID="Cancel" runat="server" 
                                     OnClick="CancelClick" 
                                     Type="Secondary"
                                     Icon="times" 
                                     TextLocalizedTag="CANCEL" />
                </div>
            </div>
        </div>
    </div>
    </ContentTemplate>
    </asp:UpdatePanel>


