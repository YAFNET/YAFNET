<%@ Control Language="C#" AutoEventWireup="true"
	Inherits="YAF.Controls.EditUsersSuspend" Codebehind="EditUsersSuspend.ascx.cs" %>


<h2 runat="server" id="trHeader">
			<YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedPage="PROFILE" LocalizedTag="SUSPEND_USER" />
		</h2>

    <asp:PlaceHolder runat="server" ID="SuspendedHolder">
        <div class="alert alert-warning" role="alert">
            <h4 class="alert-heading">
                <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" LocalizedPage="PROFILE" LocalizedTag="SUSPEND_CURRENT" />
            </h4>
            <p><strong><YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" LocalizedPage="PROFILE" LocalizedTag="SUSPEND_REASON" /></strong> 
                <asp:Label runat="server" ID="CurrentSuspendedReason" />
                <strong><YAF:LocalizedLabel ID="LocalizedLabel8" runat="server" LocalizedPage="PROFILE" LocalizedTag="SUSPEND_BY" /></strong>
                <YAF:UserLink runat="server" ID="SuspendedBy" />
                <strong><YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedPage="PROFILE" LocalizedTag="ENDS" /></strong>
                <%= this.GetSuspendedTo() %>
            </p>
            <hr/>
            <p class="mb-0">
                <YAF:ThemeButton runat="server" ID="RemoveSuspension" 
                                 Type="Danger" 
                                 Size="Small" 
                                 OnClick="RemoveSuspension_Click"
                                 TextLocalizedTag="REMOVESUSPENSION"
                                 Icon="flag"/>
            </p>
        </div>
        <hr/>
    </asp:PlaceHolder>

        <h2>
            <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedPage="PROFILE" LocalizedTag="SUSPEND_NEW" />
        </h2>
<div class="mb-3">
    <asp:Label runat="server" AssociatedControlID="SuspendedReason">
        <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" LocalizedPage="PROFILE" LocalizedTag="SUSPEND_REASON" />
    </asp:Label>
    <asp:TextBox Style="height:80px;" ID="SuspendedReason" runat="server" TextMode="MultiLine" CssClass="form-control"></asp:TextBox>
</div>
<div class="mb-3">
    <asp:Label runat="server" AssociatedControlID="SuspendCount">
        <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedPage="PROFILE" LocalizedTag="SUSPEND_USER" />
    </asp:Label>
    <asp:TextBox runat="server" ID="SuspendCount" CssClass="form-control" TextMode="Number" />
    <div class="form-check form-check-inline mt-1">
        <asp:RadioButtonList
            runat="server" ID="SuspendUnit" 
            RepeatLayout="UnorderedList"
            CssClass="list-unstyled d-inline-flex mt-2" />
    </div>
</div>
<YAF:Alert runat="server" Type="info">
    <YAF:Icon runat="server" IconName="info-circle" /><asp:Label runat="server" ID="SuspendInfo"></asp:Label>
</YAF:Alert>

<div class="text-center">
    <YAF:ThemeButton runat="server" ID="Suspend" OnClick="Suspend_Click" 
                     Type="Primary"
                     Icon="flag"
                     TextLocalizedTag="SUSPEND"/>
</div>
