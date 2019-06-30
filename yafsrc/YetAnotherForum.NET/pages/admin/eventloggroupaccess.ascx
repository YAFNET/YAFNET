<%@ Control Language="c#" AutoEventWireup="True"
	Inherits="YAF.Pages.Admin.eventloggroupaccess" Codebehind="eventloggroupaccess.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

    <div class="row">
    <div class="col-xl-12">
        <h1><YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="TITLE"  LocalizedPage="ADMIN_EVENTLOGROUPACCESS" /></h1>
    </div>
    </div>
    <div class="row">
        <div class="col-xl-12">
            <div class="card mb-3">
                <div class="card-header">
                    <i class="fa fa-users fa-fw"></i>&nbsp; <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="HEADER" LocalizedPage="ADMIN_EVENTLOGROUPACCESS" />
                    <YAF:LocalizedLabel ID="GroupNameLabel" runat="server" LocalizedTag="GROUPNAME" LocalizedPage="ADMIN_EVENTLOGROUPACCESS" />&nbsp;
                    <span class="font-weight-bold">
                        <asp:Label ID="GroupName" runat="server"  />
                    </span>
                </div>
                <div class="card-body">
                    <asp:Repeater ID="AccessList" OnItemDataBound="AccessList_OnItemDataBound" runat="server">
              <HeaderTemplate>
                  <ul class="list-group">
              </HeaderTemplate>
            <ItemTemplate>
                 <li class="list-group-item list-group-item-action">
                <div class="d-flex w-100 justify-content-between">
                    <h5 class="mb-1 text-break">
                        <asp:Label ID="EventText" runat="server" />
                    </h5>
                </div>
                     <h6>
                         <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedTag="VIEWACCESS"  LocalizedPage="ADMIN_EVENTLOGROUPACCESS" />
                     </h6>
                <p class="custom-control custom-switch">
                    <asp:CheckBox  ID="ViewAccess" runat="server" Text="&nbsp;"/>
                </p>
                     <h6>
                         <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="DELETEACCESS"  LocalizedPage="ADMIN_EVENTLOGROUPACCESS" />
                     </h6>
                     <p class="custom-control custom-switch">
                         <asp:CheckBox  ID="DeleteAccess" runat="server" Text="&nbsp;"/>
                     </p>
            </li>
            </ItemTemplate>
              <FooterTemplate>
                  </ul>
              </FooterTemplate>
        </asp:Repeater>
                </div>
                    <div class="card-footer text-center">
				    <YAF:ThemeButton ID="Save" runat="server" 
                                     OnClick="Save_Click" 
                                     CssClass="mt-1" 
                                     Type="Primary"            
				                     Icon="save" 
                                     TextLocalizedTag="SAVE" 
                                     TextLocalizedPage="ADMIN_EVENTLOGROUPACCESS" />
                    <YAF:ThemeButton ID="GrantAll" runat="server" 
                                     OnClick="GrantAll_Click" 
                                     CssClass="mt-1" 
                                     Type="Info"
                                     Icon="check" 
                                     TextLocalizedTag="GRANTALL" 
                                     TextLocalizedPage="ADMIN_EVENTLOGROUPACCESS" />
                    <YAF:ThemeButton ID="RevokeAll" runat="server" OnClick="RevokeAll_Click" 
                                     CssClass="mt-1" 
                                     Type="Danger"
                                     Icon="trash" 
                                     TextLocalizedTag="REVOKEALL" 
                                     TextLocalizedPage="ADMIN_EVENTLOGROUPACCESS" />
                    <YAF:ThemeButton ID="GrantAllDelete" runat="server" 
                                     OnClick="GrantAllDelete_Click" 
                                     CssClass="mt-1" 
                                     Type="Info"
                                     Icon="check" 
                                     TextLocalizedTag="GRANTALLDELETE" 
                                     TextLocalizedPage="ADMIN_EVENTLOGROUPACCESS" />
                    <YAF:ThemeButton ID="RevokeAllDelete" runat="server" 
                                     OnClick="RevokeAllDelete_Click" 
                                     CssClass="mt-1" 
                                     Type="Danger"
                                     Icon="trash" 
                                     TextLocalizedTag="REVOKEALLDELETE" 
                                     TextLocalizedPage="ADMIN_EVENTLOGROUPACCESS" />
				    <YAF:ThemeButton ID="Cancel" runat="server" 
                                     OnClick="Cancel_Click" 
                                     CssClass="mt-1" 
                                     Type="Secondary"
				                     Icon="times" 
                                     TextLocalizedTag="CANCEL" 
                                     TextLocalizedPage="ADMIN_EVENTLOGROUPACCESS" />
                </div>
            </div>
        </div>
    </div>


