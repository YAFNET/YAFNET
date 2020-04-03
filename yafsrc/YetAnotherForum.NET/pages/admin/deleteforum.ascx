<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Admin.deleteforum"
    CodeBehind="deleteforum.ascx.cs" %>


<YAF:PageLinks runat="server" ID="PageLinks" />

    <div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" 
                                LocalizedTag="TITLE" 
                                LocalizedPage="ADMIN_DELETEFORUM" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-comments fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" 
                                                                                                 LocalizedTag="HEADER1" 
                                                                                                 LocalizedPage="ADMIN_DELETEFORUM" />
                    <asp:Label ID="ForumNameTitle" runat="server" CssClass="font-weight-bold"></asp:Label>
                </div>
                <div class="card-body">
                    <div class="form-group">
                        <YAF:HelpLabel ID="HelpLabel11" runat="server" 
                                       AssociatedControlID="MoveTopics"
                                       LocalizedTag="MOVE_TOPICS" LocalizedPage="ADMIN_DELETEFORUM" />
                        <div class="custom-control custom-switch">
                            <asp:CheckBox ID="MoveTopics" runat="server" 
                                          AutoPostBack="True" 
                                          Text="&nbsp;"></asp:CheckBox>
                        </div>
                    </div>
                    <div class="form-group">
                        <YAF:HelpLabel ID="HelpLabel2" runat="server" 
                                       AssociatedControlID="ForumList"
                                       LocalizedTag="NEW_FORUM" LocalizedPage="ADMIN_DELETEFORUM" />
                        <asp:DropDownList ID="ForumList" runat="server" 
                                          Enabled="false" 
                                          CssClass="select2-image-select">
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="card-footer text-center">
                    <YAF:ThemeButton ID="Delete" runat="server" 
                                     CssClass="btn btn-danger"
                                     Icon="trash" 
                                     TextLocalizedTag="DELETE_FORUM" TextLocalizedPage="ADMIN_DELETEFORUM">
                    </YAF:ThemeButton>
                    <YAF:ThemeButton ID="Cancel" runat="server" 
                                     Type="Secondary"
                                     Icon="times" 
                                     TextLocalizedTag="CANCEL"></YAF:ThemeButton>
                </div>
            </div>
        </div>
    </div>


<asp:UpdatePanel ID="UpdatePanel1" runat="server">
	<ContentTemplate>
		<asp:Timer ID="UpdateStatusTimer" runat="server" Enabled="false" Interval="4000" OnTick="UpdateStatusTimerTick" />
	</ContentTemplate>
</asp:UpdatePanel>

<div>
	<div id="DeleteForumMessage" style="display:none">
		<div class="card text-white text-center bg-danger mb-3">
		    <div class="card-body">
		        <blockquote class="blockquote">
                    <p>
                        <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="DELETE_TITLE" LocalizedPage="ADMIN_DELETEFORUM" />
                    </p>
                    <p>
                        <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedTag="DELETE_MSG" LocalizedPage="ADMIN_DELETEFORUM" />
                    </p>
                    <footer>
                        <div class="fa-3x"><i class="fas fa-spinner fa-pulse"></i></div>
                    </footer>
                </blockquote>
            </div>
        </div>
    </div>
</div>


