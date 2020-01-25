<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.cp_deleteaccount"CodeBehind="cp_deleteaccount.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <h2><YAF:LocalizedLabel ID="LocalizedLabel11" runat="server" 
                                LocalizedTag="TITLE" LocalizedPage="CP_DELETEACCOUNT" /></h2>
    </div>
</div>

<div class="row">
    <div class="col">
        <div class="row">
            <div class="col">
                <div class="card mb-3">
                    <div class="card-header">
                        <YAF:Icon runat="server" 
                                  IconName="user-alt-slash"
                                  IconType="text-secondary"></YAF:Icon>
                        <YAF:LocalizedLabel ID="LocalizedLabel12" runat="server" 
                                            LocalizedTag="TITLE" LocalizedPage="CP_DELETEACCOUNT" />
                    </div>
                    <div class="card-body">
                        <h6 class="card-title">
                            <YAF:LocalizedLabel ID="LocalizedLabel200" runat="server" 
                                                LocalizedTag="OPTIONS_TITLE" />
                        </h6>
                        <div class="form-group">
                            <div class="custom-control custom-radio">
                                <asp:RadioButtonList ID="Options" runat="server" 
                                                     RepeatLayout="UnorderedList"
                                                     CssClass="list-unstyled" />
                            </div>
                        </div>
                    </div>
                    <div class="card-footer text-center">
                        <YAF:ThemeButton ID="DeleteUser" runat="server"
                                         OnClick="DeleteUserClick"
                                         TextLocalizedTag="CONTINUE"
                                         Type="Danger"
                                         Icon="user-alt-slash"/>
                        <YAF:ThemeButton ID="Cancel" runat="server"
                                         TextLocalizedTag="CANCEL"
                                         Type="Secondary"
                                         Icon="reply"/>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>