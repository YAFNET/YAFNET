<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.Email" Codebehind="Email.ascx.cs" %>


<YAF:PageLinks runat="server" ID="PageLinks" />

<div class="row">
    <div class="col-xl-12">
        <h2><YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="TITLE" /></h2>
    </div>
</div>

<div class="row">
    <div class="col">
        <div class="card mb-3">
            <div class="card-header">
                <i class="fa fa-envelope-open fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedTag="TITLE" />
            </div>
            <div class="card-body text-center">
                <form>
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="">
                            <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="SUBJECT" />
                        </asp:Label>
                        <asp:TextBox runat="server" ID="Subject" CssClass="form-control" />
                    </div>
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="">
                            <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="BODY" />
                        </asp:Label>
                        <asp:TextBox runat="server" ID="Body" TextMode="multiline" Rows="10" CssClass="form-control" />
                    </div>
                </form>
            </div>
            <div class="card-footer text-center">
                <YAF:ThemeButton runat="server" ID="Send"
                                 OnClick="Send_Click"
                                 TextLocalizedTag="SEND"
                                 Type="Primary"
                                 Icon="paper-plane"/>
            </div>
        </div>
    </div>
</div>