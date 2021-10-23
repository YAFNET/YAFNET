<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Email" Codebehind="Email.ascx.cs" %>


<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <h2><YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" 
                                LocalizedTag="TITLE2"/>
        </h2>
    </div>
</div>

<div class="row">
    <div class="col">
        <div class="card mb-3">
            <div class="card-header">
                <YAF:IconHeader runat="server"
                                ID="IconHeader"
                                IconName="envelope-open"
                                LocalizedTag="TITLE2"
                                LocalizedPage="IM_EMAIL"/>
            </div>
            <div class="card-body">
                <div class="mb-3">
                    <asp:Label runat="server" AssociatedControlID="">
                        <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" 
                                            LocalizedTag="SUBJECT" />
                    </asp:Label>
                    <asp:TextBox runat="server" ID="Subject" 
                                 CssClass="form-control"
                                 required="required" />
                    <div class="invalid-feedback">
                        <YAF:LocalizedLabel runat="server"
                                            LocalizedTag="NEED_SUBJECT"
                                            LocalizedPage="EMAILTOPIC"  />
                    </div>
                </div>
                <div class="mb-3">
                    <asp:Label runat="server" AssociatedControlID="">
                        <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" 
                                            LocalizedTag="BODY" />
                    </asp:Label>
                    <asp:TextBox runat="server" ID="Body" 
                                 TextMode="multiline" 
                                 Rows="10" 
                                 CssClass="form-control"
                                 required="required" />
                    <div class="invalid-feedback">
                        <YAF:LocalizedLabel runat="server"
                                            LocalizedTag="NEED_MESSAGE"
                                            LocalizedPage="EMAILTOPIC" />
                    </div>
                </div>
            </div>
            <div class="card-footer text-center">
                <YAF:ThemeButton runat="server" ID="Send"
                                 OnClick="Send_Click"
                                 CausesValidation="True"
                                 TextLocalizedTag="SEND"
                                 Type="Primary"
                                 Icon="paper-plane"/>
            </div>
        </div>
    </div>
</div>