<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.reguser" Codebehind="reguser.ascx.cs" %>


<YAF:PageLinks id="PageLinks" runat="server" />
    <div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" 
                                LocalizedTag="HEADER" 
                                LocalizedPage="ADMIN_REGUSER" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-user-plus fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel14" runat="server" 
                                                                                             LocalizedTag="HEADER2" 
                                                                                             LocalizedPage="ADMIN_REGUSER" />
                </div>
                <div class="card-body">
                    <div class="form-row">
                        <div class="form-group col-md-6">
                            <asp:Label runat="server" AssociatedControlID="UserName">
                                <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" 
                                                    LocalizedTag="USERNAME" LocalizedPage="REGISTER"></YAF:LocalizedLabel>:
                            </asp:Label>
                            <asp:TextBox CssClass="form-control" id="UserName" runat="server"></asp:TextBox>
                            <asp:RequiredFieldValidator id="Requiredfieldvalidator1" runat="server" 
                                                        ControlToValidate="UserName" 
                                                        ErrorMessage="User Name is required." 
                                                        CssClass="form-text text-danger"></asp:RequiredFieldValidator>
                        </div>
                        <div class="form-group col-md-6">
                            <asp:Label runat="server" AssociatedControlID="Email">
                                <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" 
                                                    LocalizedTag="EMAIL" LocalizedPage="REGISTER"></YAF:LocalizedLabel>:
                            </asp:Label>
                            <asp:TextBox CssClass="form-control" id="Email" runat="server" 
                                         TextMode="Email"></asp:TextBox>
                            <asp:RequiredFieldValidator id="Requiredfieldvalidator5" runat="server" 
                                                        ControlToValidate="Email" 
                                                        ErrorMessage="Email address is required." 
                                                        CssClass="form-text text-danger"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <div class="form-row">
                        <div class="form-group col-md-6">
                            <asp:Label runat="server" AssociatedControlID="Password">
                                <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" 
                                                    LocalizedTag="PASSWORD" LocalizedPage="ADMIN_REGUSER"></YAF:LocalizedLabel>
                            </asp:Label>
                            <asp:TextBox id="Password" runat="server" 
                                         TextMode="Password"
                                         CssClass="form-control"></asp:TextBox>
                            <asp:RequiredFieldValidator id="Requiredfieldvalidator2" runat="server" 
                                                        ControlToValidate="Password" 
                                                        ErrorMessage="Password is required." 
                                                        CssClass="form-text text-danger"></asp:RequiredFieldValidator>
                        </div>
                        <div class="form-group col-md-6">
                            <asp:Label runat="server" AssociatedControlID="Password2">
                                <YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" 
                                                    LocalizedTag="CONFIRM_PASSWORD" LocalizedPage="REGISTER"></YAF:LocalizedLabel>:
                            </asp:Label>
                            <asp:TextBox id="Password2" runat="server" 
                                         TextMode="Password"
                                         CssClass="form-control" ></asp:TextBox>
                            <asp:CompareValidator id="Comparevalidator1" runat="server" 
                                                  Name="Comparevalidator1" 
                                                  ControlToValidate="Password2" 
                                                  ErrorMessage="Passwords didnt match." 
                                                  ControlToCompare="Password"
                                                  CssClass="form-text text-danger"></asp:CompareValidator>
                        </div>
                    </div>
                    <div class="form-row">
                        <div class="form-group col-md-6">
                            <asp:Label runat="server" AssociatedControlID="Question">
                                <YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" 
                                                    LocalizedTag="SECURITY_QUESTION" 
                                                    LocalizedPage="REGISTER"></YAF:LocalizedLabel>:
                            </asp:Label>
                            <asp:TextBox CssClass="form-control" id="Question" runat="server"></asp:TextBox>
                            <asp:RequiredFieldValidator id="Requiredfieldvalidator3" runat="server" 
                                                        ControlToValidate="Question" 
                                                        ErrorMessage="Password Question is Required." 
                                                        CssClass="form-text text-danger"></asp:RequiredFieldValidator>
                        </div>
                        <div class="form-group col-md-6">
                            <asp:Label runat="server" AssociatedControlID="Answer">
                                <YAF:LocalizedLabel ID="LocalizedLabel10" runat="server" 
                                                    LocalizedTag="SECURITY_ANSWER" 
                                                    LocalizedPage="REGISTER"></YAF:LocalizedLabel>:
                            </asp:Label>
                            <asp:TextBox CssClass="form-control" id="Answer" runat="server"></asp:TextBox>
                            <asp:RequiredFieldValidator id="Requiredfieldvalidator4" runat="server" 
                                                        ControlToValidate="Answer" 
                                                        ErrorMessage="Password Answer is Required." 
                                                        CssClass="form-text text-danger"></asp:RequiredFieldValidator>
                        </div>
                    </div>
                    <h3>
                        <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" 
                                            LocalizedTag="HEADER3" 
                                            LocalizedPage="ADMIN_REGUSER" />:
                    </h3>
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="Location">
                            <YAF:LocalizedLabel ID="LocalizedLabel11" runat="server" 
                                                LocalizedTag="LOCATION" 
                                                LocalizedPage="ADMIN_REGUSER"></YAF:LocalizedLabel>:
                        </asp:Label>
                        <asp:TextBox id="Location" runat="server"
                                     CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="HomePage">
                            <YAF:LocalizedLabel ID="LocalizedLabel12" runat="server" 
                                                LocalizedTag="HOMEPAGE" 
                                                LocalizedPage="ADMIN_REGUSER"></YAF:LocalizedLabel>:
                        </asp:Label>
                        <asp:TextBox id="HomePage" runat="server"
                                     CssClass="form-control"></asp:TextBox>
                    </div>
                    <h3>
                        <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" 
                                            LocalizedTag="HEADER4" 
                                            LocalizedPage="ADMIN_REGUSER"></YAF:LocalizedLabel>
                    </h3>
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="TimeZones">
                            <YAF:LocalizedLabel ID="LocalizedLabel13" runat="server" 
                                                LocalizedTag="TIMEZONE" 
                                                LocalizedPage="ADMIN_REGUSER"></YAF:LocalizedLabel>:
                        </asp:Label>
                        <asp:DropDownList id="TimeZones" runat="server" 
                                          DataValueField="Value" 
                                          DataTextField="Name"
                                          CssClass="custom-select"></asp:DropDownList>
                    </div>
                </div>
                <div class="card-footer text-center">
                    <YAF:ThemeButton id="ForumRegister" runat="server" 
                                     Onclick="ForumRegisterClick" 
                                     Type="Primary"
                                     Icon="user-plus" 
                                     TextLocalizedTag="REGISTER" TextLocalizedPage="ADMIN_REGUSER">
                    </YAF:ThemeButton>
                    &nbsp;
                    <YAF:ThemeButton id="cancel" runat="server" 
                                     Onclick="CancelClick" 
                                     Type="Secondary"
                                     Icon="times" 
                                     TextLocalizedTag="CANCEL">
                    </YAF:ThemeButton>
                </div>
            </div>
        </div>
    </div>


