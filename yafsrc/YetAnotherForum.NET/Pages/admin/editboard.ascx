<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.editboard" Codebehind="editboard.ascx.cs" %>


<YAF:PageLinks runat="server" ID="PageLinks" />

    <asp:UpdatePanel ID="UppdatePanel1" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
    <div class="row">
        <div class="col-xl-12">
            <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_EDITBOARD" /></h1>
        </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-globe fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_EDITBOARD" />
                </div>
                <div class="card-body">
                    <div class="form-row">
                        <div class="form-group col-md-6">
                            <YAF:HelpLabel ID="HelpLabel1" runat="server" 
                                           AssociatedControlID="Name"
                                           LocalizedTag="NAME" LocalizedPage="ADMIN_EDITBOARD" />
                            <asp:TextBox ID="Name" runat="server" 
                                         CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="form-group col-md-6">
                            <YAF:HelpLabel ID="HelpLabel2" runat="server" 
                                           AssociatedControlID="Culture"
                                           LocalizedTag="CULTURE" LocalizedPage="ADMIN_EDITBOARD" />
                            <asp:DropDownList ID="Culture" runat="server" 
                                              CssClass="custom-select" />
                        </div>
                    </div>
                    <asp:PlaceHolder runat="server" ID="BoardMembershipAppNameHolder">
                        <div class="form-group">
                            <YAF:HelpLabel ID="HelpLabel4" runat="server" 
                                           AssociatedControlID="BoardMembershipAppName"
                                           LocalizedTag="MEMBSHIP_APP_NAME" LocalizedPage="ADMIN_EDITBOARD" />
                            <asp:TextBox ID="BoardMembershipAppName" runat="server" 
                                         CssClass="form-control"></asp:TextBox>
                        </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder runat="server" ID="CreateNewAdminHolder">
                        <div class="form-group">
                            <YAF:HelpLabel ID="HelpLabel5" runat="server" 
                                           AssociatedControlID="CreateAdminUser"
                                           LocalizedTag="ADMIN_USER" LocalizedPage="ADMIN_EDITBOARD" />
             
                            <div class="custom-control custom-switch">
                                <asp:CheckBox runat="server" ID="CreateAdminUser" 
                                              AutoPostBack="true" 
                                              OnCheckedChanged="CreateAdminUserCheckedChanged" 
                                              Text="&nbsp;" />
                            </div> 
                        </div>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder runat="server" ID="AdminInfo" Visible="false">
                        <h3>
                            <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="HEADER2" LocalizedPage="ADMIN_EDITBOARD" />
                        </h3>
                        <div class="form-row">
                            <div class="form-group col-md-6">
                                <YAF:HelpLabel ID="HelpLabel6" runat="server" 
                                               AssociatedControlID="UserName"
                                               LocalizedTag="USER_NAME" LocalizedPage="ADMIN_EDITBOARD" />
                                <asp:TextBox runat="server" ID="UserName" 
                                             CssClass="form-control" />
                            </div>
                            <div class="form-group col-md-6">
                                <YAF:HelpLabel ID="HelpLabel7" runat="server" 
                                               AssociatedControlID="UserEmail"
                                               LocalizedTag="USER_MAIL" LocalizedPage="ADMIN_EDITBOARD" />
                                <asp:TextBox runat="server" ID="UserEmail" 
                                             CssClass="form-control" />
                            </div>
                        </div>
                        <div class="form-row">
                            <div class="form-group col-md-6">
                                <YAF:HelpLabel ID="HelpLabel8" runat="server" 
                                               AssociatedControlID="UserPass1"
                                               LocalizedTag="USER_PASS" LocalizedPage="ADMIN_EDITBOARD" />
                                <asp:TextBox runat="server" ID="UserPass1" TextMode="password" 
                                             CssClass="form-control" />
                            </div>
                            <div class="form-group col-md-6">
                                <YAF:HelpLabel ID="HelpLabel9" runat="server" 
                                               AssociatedControlID="UserPass2"
                                               LocalizedTag="VERIFY_PASS" LocalizedPage="ADMIN_EDITBOARD" />
                                <asp:TextBox runat="server" ID="UserPass2" 
                                             TextMode="password" 
                                             CssClass="form-control" />
                            </div>
                        </div>
                        <div class="form-row">
                            <div class="form-group col-md-6">
                                <YAF:HelpLabel ID="HelpLabel10" runat="server" 
                                               AssociatedControlID="UserPasswordQuestion"
                                               LocalizedTag="SECURITY_QUESTION" LocalizedPage="ADMIN_EDITBOARD" />
                                <asp:TextBox runat="server" ID="UserPasswordQuestion" 
                                             CssClass="form-control" />
                            </div>
                            <div class="form-group col-md-6">
                                <YAF:HelpLabel ID="HelpLabel11" runat="server" 
                                               AssociatedControlID="UserPasswordAnswer"
                                               LocalizedTag="SECURITY_ANSWER" LocalizedPage="ADMIN_EDITBOARD" />
                                <asp:TextBox runat="server" ID="UserPasswordAnswer" 
                                             CssClass="form-control" />
                            </div>
                        </div>
                    </asp:PlaceHolder>
                </div>
                <div class="card-footer text-center">
                    <YAF:ThemeButton ID="Save" runat="server" 
                                     OnClick="SaveClick" 
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


