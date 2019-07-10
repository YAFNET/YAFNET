<%@ Control Language="c#" AutoEventWireup="True"
    Inherits="YAF.Pages.Admin.editcategory" Codebehind="editcategory.ascx.cs" %>


<YAF:PageLinks ID="PageLinks" runat="server" />

    <div class="row">
        <div class="col-xl-12">
            <h1><YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_EDITCATEGORY" />&nbsp;<asp:Label ID="Label1" runat="server"></asp:Label></h1>
        </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-comments fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_EDITCATEGORY" />&nbsp;<asp:Label ID="CategoryNameTitle" runat="server"></asp:Label>
                </div>
                <div class="card-body">
			 
			  <YAF:HelpLabel ID="HelpLabel1" runat="server" LocalizedTag="CATEGORY_NAME" LocalizedPage="ADMIN_EDITCATEGORY" />
			 
			<p>
			<asp:TextBox ID="Name" runat="server" MaxLength="50" CssClass="form-control"></asp:TextBox></p><hr />
		   
			  <YAF:HelpLabel ID="HelpLabel2" runat="server" LocalizedTag="CATEGORY_IMAGE" LocalizedPage="ADMIN_EDITCATEGORY" />
			 
			<p>
				<img align="middle" alt="Preview" runat="server" id="Preview" class="img-thumbnail" />
                </p>
                    <p>
                <asp:DropDownList ID="CategoryImages" runat="server" CssClass="custom-select" />
		</p><hr />
		    
			  <YAF:HelpLabel ID="HelpLabel3" runat="server" LocalizedTag="SORT_ORDER" LocalizedPage="ADMIN_EDITCATEGORY" />
			 
			<p>
			<asp:TextBox ID="SortOrder" runat="server" MaxLength="5" CssClass="form-control" TextMode="Number"></asp:TextBox></p>
                </div>
                <div class="card-footer text-center">
                    <YAF:ThemeButton ID="Save" runat="server" OnClick="SaveClick" Type="Primary"
                                     Icon="save" TextLocalizedTag="Save"></YAF:ThemeButton>
                    <YAF:ThemeButton ID="Cancel" runat="server" OnClick="CancelClick" Type="Secondary"
                                     Icon="times" TextLocalizedTag="CANCEL"></YAF:ThemeButton>
                </div>
            </div>
        </div>
    </div>


