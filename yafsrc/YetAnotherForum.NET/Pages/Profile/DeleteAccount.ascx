<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Profile.DeleteAccount" CodeBehind="DeleteAccount.ascx.cs" %>

<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-sm-auto">
        <YAF:ProfileMenu runat="server"></YAF:ProfileMenu>
    </div>
    <div class="col">
        <div class="row">
            <div class="col">
                <div class="card mb-3">
                    <div class="card-header">
                        <YAF:IconHeader runat="server"
                                        IconName="user-alt-slash"
                                        LocalizedPage="DELETE_ACCOUNT" />
                    </div>
                    <div class="card-body">
                        <h6 class="card-title">
                            <YAF:LocalizedLabel ID="LocalizedLabel200" runat="server" 
                                                LocalizedTag="OPTIONS_TITLE" />
                        </h6>
                        <div class="mb-3">
                            <div class="form-check">
                                <asp:RadioButtonList ID="Options" runat="server" 
                                                     RepeatLayout="UnorderedList"
                                                     CssClass="list-unstyled" />
                            </div>
                        </div>
                        <div class="text-lg-center">
                            <YAF:ThemeButton ID="DeleteUser" runat="server"
                                             OnClick="DeleteUserClick"
                                             CssClass="me-2"
                                             TextLocalizedTag="CONTINUE"
                                             ReturnConfirmTag="CONFIRM"
                                             Type="Danger"
                                             Icon="user-alt-slash"/>
                            <YAF:ThemeButton ID="Cancel" runat="server"
                                             TextLocalizedTag="CANCEL"
                                             Type="Secondary"
                                             Icon="times"/>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>