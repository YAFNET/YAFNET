<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.ViewThanks" Codebehind="ViewThanks.ascx.cs" %>
<%@ Import Namespace="YAF.Core" %>
<%@ Register TagPrefix="YAF" TagName="ViewThanksList" Src="../controls/ViewThanksList.ascx" %>
<YAF:PageLinks ID="PageLinks" runat="server" />
<div class="DivTopSeparator">
</div>

        <br style="clear: both" />
       <asp:Panel id="ThanksTabs" runat="server">
               <ul class="nav nav-tabs" role="tablist">
                 <li class="nav-item">
                     <a href="#ThanksFromTab" class="nav-link" data-toggle="tab" role="tab">
                         <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="ThanksFromUser" LocalizedPage="VIEWTHANKS" />
                     </a>
                 </li>
		         <li class="nav-item">
		             <a href="#ThanksToTab" class="nav-link" data-toggle="tab" role="tab">
		                 <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="ThanksToUser" LocalizedPage="VIEWTHANKS" />
		             </a>
		         </li>
               </ul>
              <div class="tab-content">
              <div id="ThanksFromTab" class="tab-pane" role="tabpanel">
                   <YAF:ViewThanksList runat="server" ID="ThanksFromList" CurrentMode="FromUser" />
                </div>
                <div id="ThanksToTab" class="tab-pane" role="tabpanel">
                  <YAF:ViewThanksList runat="server" ID="ThanksToList" CurrentMode="ToUser" />
                </div>
             </div>
             </asp:Panel>
        <asp:HiddenField runat="server" ID="hidLastTab" Value="ThanksFromTab" />
<div id="Div1">
    <YAF:SmartScroller ID="SmartScroller2" runat="server" />
</div>
