<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="YAF.Pages.cp_editbuddies" Codebehind="cp_editbuddies.ascx.cs" %>
<%@ Import Namespace="YAF.Core" %>
<%@ Register TagPrefix="YAF" TagName="BuddyList" Src="../controls/BuddyList.ascx" %>
<YAF:PageLinks runat="server" ID="PageLinks" />
<table width="100%" cellspacing="1" cellpadding="0" class="content">
    <tr>
        <td colspan="2" class="header1">
            <YAF:LocalizedLabel ID="BuddyList" runat="server" LocalizedTag="YOUR_BUDDYLIST" />
        </td>
    </tr>
    <tr>
        <td valign="top" rowspan="2">
             <asp:Panel id="BuddiesTabs" runat="server">
               <ul class="nav nav-tabs" role="tablist">
                 <li class="nav-item">
                     <a href="#BuddyListTab" class="nav-link" data-toggle="tab" role="tab">
                         <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" LocalizedTag="BUDDYLIST" LocalizedPage="CP_EDITBUDDIES" />
                     </a>
                 </li>
		         <li class="nav-item">
		             <a href="#PendingRequestsTab" class="nav-link" data-toggle="tab" role="tab">
		                 <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="PENDING_REQUESTS" LocalizedPage="CP_EDITBUDDIES" />
		             </a>
		         </li>
		         <li class="nav-item">
		             <a href="#YourRequestsTab" class="nav-link" data-toggle="tab" role="tab">
		                 <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" LocalizedTag="YOUR_REQUESTS" LocalizedPage="CP_EDITBUDDIES" />
		             </a>
		         </li>
               </ul>
              <div class="tab-content">
                <div id="BuddyListTab" class="tab-pane" role="tabpanel">
                  <YAF:BuddyList runat="server" ID="BuddyList1" />
                </div>
                <div id="PendingRequestsTab" class="tab-pane" role="tabpanel">
                  <YAF:BuddyList runat="server" ID="PendingBuddyList" />
                </div>
                <div id="YourRequestsTab" class="tab-pane" role="tabpanel">
                  <YAF:BuddyList runat="server" ID="BuddyRequested" />
                </div>
             </div>
             </asp:Panel>
        </td>
    </tr>
</table>
<asp:HiddenField runat="server" ID="hidLastTab" Value="BuddyListTab" />
<div id="DivSmartScroller">
    <YAF:SmartScroller ID="SmartScroller1" runat="server" />
</div>
