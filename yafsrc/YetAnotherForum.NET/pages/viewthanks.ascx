<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.ViewThanks" Codebehind="ViewThanks.ascx.cs" %>
<%@ Import Namespace="System.Web.DynamicData" %>
<%@ Import Namespace="System.Web.UI" %>
<%@ Import Namespace="System.Web.UI.HtmlControls" %>
<%@ Import Namespace="System.Web.UI.WebControls" %>
<%@ Import Namespace="System.Web.UI.WebControls" %>
<%@ Import Namespace="System.Web.UI.WebControls.Expressions" %>
<%@ Import Namespace="System.Web.UI.WebControls.WebParts" %>
<%@ Import Namespace="YAF" %>
<%@ Import Namespace="YAF.Classes" %>
<%@ Import Namespace="YAF.Configuration" %>
<%@ Import Namespace="YAF.Web.Controls" %>
<%@ Register TagPrefix="YAF" TagName="ViewThanksList" Src="../controls/ViewThanksList.ascx" %>


<YAF:PageLinks ID="PageLinks" runat="server" />

<div class="row">
    <div class="col-xl-12">
        <h2><YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" LocalizedTag="TITLE" /></h2>
    </div>
</div>

<asp:PlaceHolder runat="server" ID="MenuHolder">
<div class="row">
<div class="col-sm-auto">
    <YAF:ProfileMenu ID="ProfileMenu1" runat="server" />
</div>
</asp:PlaceHolder>
<div class="col">

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
                  <div class="row">
                      <div class="col">
                          <div class="card mb-3">
                              <div class="card-header">
                                  <i class="fa fa-hand-holding-heart fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" 
                                                                                                    LocalizedTag="ThanksFromUser" 
                                                                                                    LocalizedPage="VIEWTHANKS" />
                              </div>
                              <div class="card-body">

                                  <YAF:ViewThanksList runat="server" ID="ThanksFromList" CurrentMode="FromUser" />
                              </div>
                          </div>
                      </div>
                  </div>
                   
                </div>
                <div id="ThanksToTab" class="tab-pane" role="tabpanel">
                    <div class="row">
                        <div class="col">
                            <div class="card mb-3">
                                <div class="card-header">
                                    <i class="fa fa-heart fa-fw text-secondary"></i>&nbsp;<YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" 
                                                                                                      LocalizedTag="ThanksToUser" 
                                                                                                      LocalizedPage="VIEWTHANKS" />
                                </div>
                                <div class="card-body">
                                    <YAF:ViewThanksList runat="server" ID="ThanksToList" CurrentMode="ToUser" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
             </div>
             </asp:Panel>
        <asp:HiddenField runat="server" ID="hidLastTab" Value="ThanksFromTab" />
</div>
    </div>