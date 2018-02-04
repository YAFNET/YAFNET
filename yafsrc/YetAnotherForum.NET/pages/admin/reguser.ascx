<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.reguser" Codebehind="reguser.ascx.cs" %>

<YAF:PageLinks id="PageLinks" runat="server" />
<YAF:AdminMenu id="Adminmenu1" runat="server">
    <div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_REGUSER" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-user fa-fw"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel14" runat="server" LocalizedTag="HEADER2" LocalizedPage="ADMIN_REGUSER" />
                </div>
                <div class="card-body">

			<h4>
               <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="USERNAME" LocalizedPage="REGISTER"></YAF:LocalizedLabel>:
            </h4>
			<p>
			<asp:TextBox CssClass="form-control" id="UserName" runat="server"></asp:TextBox>
			<asp:RequiredFieldValidator id="Requiredfieldvalidator1" runat="server" EnableClientScript="False"
          ControlToValidate="UserName" ErrorMessage="User Name is required."></asp:RequiredFieldValidator>
         </p><hr />
			<h4>
               <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="EMAIL" LocalizedPage="REGISTER"></YAF:LocalizedLabel>:
            </h4>
			<p>
			<asp:TextBox CssClass="form-control" id="Email" runat="server"></asp:TextBox>
			<asp:RequiredFieldValidator id="Requiredfieldvalidator5" runat="server" EnableClientScript="False"
          ControlToValidate="Email" ErrorMessage="Email address is required."></asp:RequiredFieldValidator>
          </p><hr />
			<h4>
               <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="PASSWORD" LocalizedPage="ADMIN_REGUSER"></YAF:LocalizedLabel>
            </h4>
			<p>
			   <asp:TextBox CssClass="form-control" id="Password" runat="server" TextMode="Password"></asp:TextBox>
			   <asp:RequiredFieldValidator id="Requiredfieldvalidator2" runat="server" EnableClientScript="False"
          ControlToValidate="Password" ErrorMessage="Password is required."></asp:RequiredFieldValidator>
            </p><hr />
			<h4>
              <YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedTag="CONFIRM_PASSWORD" LocalizedPage="REGISTER"></YAF:LocalizedLabel>:
            </h4>
			<p>
			<asp:TextBox CssClass="form-control" id="Password2" runat="server" TextMode="Password"></asp:TextBox>
			<asp:CompareValidator id="Comparevalidator1" runat="server" NAME="Comparevalidator1" EnableClientScript="False"
          ControlToValidate="Password2" ErrorMessage="Passwords didnt match." ControlToCompare="Password"></asp:CompareValidator>
                </p><hr />
			<h4>
              <YAF:LocalizedLabel ID="LocalizedLabel9" runat="server" LocalizedTag="SECURITY_QUESTION" LocalizedPage="REGISTER"></YAF:LocalizedLabel>:
            </h4>
			<p>
			<asp:TextBox CssClass="form-control" id="Question" runat="server"></asp:TextBox>
			<asp:RequiredFieldValidator id="Requiredfieldvalidator3" runat="server" EnableClientScript="False"
          ControlToValidate="Question" ErrorMessage="Password Question is Required."></asp:RequiredFieldValidator>
                </p><hr />
			<h4>
              <YAF:LocalizedLabel ID="LocalizedLabel10" runat="server" LocalizedTag="SECURITY_ANSWER" LocalizedPage="REGISTER"></YAF:LocalizedLabel>:
            </h4>
			<p>
			<asp:TextBox CssClass="form-control" id="Answer" runat="server"></asp:TextBox>
			<asp:RequiredFieldValidator id="Requiredfieldvalidator4" runat="server" EnableClientScript="False"
          ControlToValidate="Answer" ErrorMessage="Password Answer is Required."></asp:RequiredFieldValidator>
                </p><hr />
			<h2>
              <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="HEADER3" LocalizedPage="ADMIN_REGUSER" />:
            </h2>
			<h4>
              <YAF:LocalizedLabel ID="LocalizedLabel11" runat="server" LocalizedTag="LOCATION" LocalizedPage="ADMIN_REGUSER"></YAF:LocalizedLabel>:
             </h4>
			<p>
			<asp:TextBox CssClass="form-control" id="Location" runat="server"></asp:TextBox>
			</p><hr />
			<h4>
                <YAF:LocalizedLabel ID="LocalizedLabel12" runat="server" LocalizedTag="HOMEPAGE" LocalizedPage="ADMIN_REGUSER"></YAF:LocalizedLabel>:
            </h4>
			<p>
			<asp:TextBox CssClass="form-control" id="HomePage" runat="server"></asp:TextBox>
                </p><hr />
			<h2>
              <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="HEADER4" LocalizedPage="ADMIN_REGUSER"></YAF:LocalizedLabel>
            </h2>
			<h4>
               <YAF:LocalizedLabel ID="LocalizedLabel13" runat="server" LocalizedTag="TIMEZONE" LocalizedPage="ADMIN_REGUSER"></YAF:LocalizedLabel>:
            </h4>
			<p>
			<asp:DropDownList CssClass="custom-select" id="TimeZones" runat="server" DataValueField="Value" DataTextField="Name"></asp:DropDownList>
                </p>
                </div>
                <div class="card-footer text-lg-center">
			<YAF:ThemeButton id="ForumRegister" runat="server" onclick="ForumRegisterClick" Type="Primary"
			                 Icon="user-plus" TextLocalizedTag="REGISTER" TextLocalizedPage="ADMIN_REGUSER"></YAF:ThemeButton>
			&nbsp;
			<YAF:Themebutton id="cancel" runat="server" onclick="CancelClick" Type="Secondary"
			                 Icon="times" TextLocalizedTag="CANCEL"></YAF:Themebutton>
                </div>
            </div>
        </div>
    </div>
</YAF:AdminMenu>
<YAF:SmartScroller id="SmartScroller1" runat="server" />
