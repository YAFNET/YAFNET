<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.ViewThanks" Codebehind="ViewThanks.ascx.cs" %>
<%@ Import Namespace="YAF.Core" %>
<%@ Register TagPrefix="YAF" TagName="ViewThanksList" Src="../controls/ViewThanksList.ascx" %>
<YAF:PageLinks ID="PageLinks" runat="server" />
<div class="DivTopSeparator">
</div>

        <br style="clear: both" />
       <asp:Panel id="ThanksTabs" runat="server">
               <ul>
                 <li><a href="#ThanksFromTab"><YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="ThanksFromUser" LocalizedPage="VIEWTHANKS" /></a></li>
		         <li><a href="#ThanksToTab"><YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="ThanksToUser" LocalizedPage="VIEWTHANKS" /></a></li>		        
               </ul>
                <div id="ThanksFromTab">
                   <YAF:ViewThanksList runat="server" ID="ThanksFromList" CurrentMode="FromUser" />
                </div>
                <div id="ThanksToTab">
                  <YAF:ViewThanksList runat="server" ID="ThanksToList" CurrentMode="ToUser" />
                </div>
             </asp:Panel>
        <asp:HiddenField runat="server" ID="hidLastTab" Value="0" />
<div id="Div1">
    <YAF:SmartScroller ID="SmartScroller2" runat="server" />
</div>
