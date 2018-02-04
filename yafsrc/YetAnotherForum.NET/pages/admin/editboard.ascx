<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.editboard" Codebehind="editboard.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />
<YAF:AdminMenu runat="server">
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
                    <i class="fa fa-globe fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="TITLE" LocalizedPage="ADMIN_EDITBOARD" />
                </div>
                <div class="card-body">
            <h4>
                <YAF:HelpLabel ID="HelpLabel1" runat="server" LocalizedTag="NAME" LocalizedPage="ADMIN_EDITBOARD" />
            </h4>
            <p>
                <asp:TextBox ID="Name" runat="server" CssClass="form-control"></asp:TextBox>
            </p><hr />
              <p>
            <h4>
                <YAF:HelpLabel ID="HelpLabel2" runat="server" LocalizedTag="CULTURE" LocalizedPage="ADMIN_EDITBOARD" />
            </h4>
            <p>
                <asp:DropDownList ID="Culture" runat="server" CssClass="custom-select" />
            </p><hr />
            <h4>
                <YAF:HelpLabel ID="HelpLabel3" runat="server" LocalizedTag="THREADED" LocalizedPage="ADMIN_EDITBOARD" />
            </h4>
            <p>
                <asp:CheckBox runat="server" ID="AllowThreaded" CssClass="form-control" />
            </p><hr />
        <asp:PlaceHolder runat="server" ID="BoardMembershipAppNameHolder">
            <h4>
                <YAF:HelpLabel ID="HelpLabel4" runat="server" LocalizedTag="MEMBSHIP_APP_NAME" LocalizedPage="ADMIN_EDITBOARD" />
            </h4>
            <p>
                <asp:TextBox ID="BoardMembershipAppName" runat="server" CssClass="form-control"></asp:TextBox>
            </p><hr />
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="CreateNewAdminHolder">
            <h4>
                <YAF:HelpLabel ID="HelpLabel5" runat="server" LocalizedTag="ADMIN_USER" LocalizedPage="ADMIN_EDITBOARD" />
            </h4>
            <p>
                <asp:CheckBox runat="server" ID="CreateAdminUser" AutoPostBack="true" OnCheckedChanged="CreateAdminUserCheckedChanged" CssClass="form-control" />
            </p><hr />
        </asp:PlaceHolder>
        <asp:PlaceHolder runat="server" ID="AdminInfo" Visible="false">
                <h3>
                  <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="HEADER2" LocalizedPage="ADMIN_EDITBOARD" />
                </h3>
                <h4>
                    <YAF:HelpLabel ID="HelpLabel6" runat="server" LocalizedTag="USER_NAME" LocalizedPage="ADMIN_EDITBOARD" />
                </h4>
                <p>
                    <asp:TextBox runat="server" ID="UserName" CssClass="form-control" />
                </p><hr />
                <h4>
                    <YAF:HelpLabel ID="HelpLabel7" runat="server" LocalizedTag="USER_MAIL" LocalizedPage="ADMIN_EDITBOARD" />
                </h4>
                <p>
                    <asp:TextBox runat="server" ID="UserEmail" CssClass="form-control" />
                </p><hr />
                <h4>
                    <YAF:HelpLabel ID="HelpLabel8" runat="server" LocalizedTag="USER_PASS" LocalizedPage="ADMIN_EDITBOARD" />
                </h4>
                <p>
                    <asp:TextBox runat="server" ID="UserPass1" TextMode="password" CssClass="form-control" />
                </p><hr />
                <h4>
                    <YAF:HelpLabel ID="HelpLabel9" runat="server" LocalizedTag="VERIFY_PASS" LocalizedPage="ADMIN_EDITBOARD" />
                </h4>
                <p>
                    <asp:TextBox runat="server" ID="UserPass2" TextMode="password" CssClass="form-control" />
                </p><hr />
                <h4>
                    <YAF:HelpLabel ID="HelpLabel10" runat="server" LocalizedTag="SECURITY_QUESTION" LocalizedPage="ADMIN_EDITBOARD" />
                </h4>
                <p>
                    <asp:TextBox runat="server" ID="UserPasswordQuestion" CssClass="form-control" />
                </p><hr />
                <h4>
                    <YAF:HelpLabel ID="HelpLabel11" runat="server" LocalizedTag="SECURITY_ANSWER" LocalizedPage="ADMIN_EDITBOARD" />
                </h4>
                <p>
                    <asp:TextBox runat="server" ID="UserPasswordAnswer" CssClass="form-control" />
                </p>
        </asp:PlaceHolder>
                </div>
                <div class="card-footer text-lg-center">
                    <YAF:ThemeButton ID="Save" runat="server" OnClick="SaveClick" Type="Primary"
                                     Icon="save" TextLocalizedTag="SAVE"/>
                    <YAF:ThemeButton ID="Cancel" runat="server" OnClick="CancelClick" Type="Secondary"
                                     Icon="times" TextLocalizedTag="CANCEL" />
                </div>
            </div>
        </div>
    </div>
    </ContentTemplate>
    </asp:UpdatePanel>
</YAF:AdminMenu>
<YAF:SmartScroller ID="SmartScroller1" runat="server" />
