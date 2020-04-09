<%@ Control Language="c#" AutoEventWireup="True" Inherits="YAF.Pages.ActiveUsers" Codebehind="ActiveUsers.ascx.cs" %>

<%@ Import Namespace="YAF.Types.Interfaces" %>
<%@ Import Namespace="YAF.Utils.Helpers" %>
<%@ Import Namespace="YAF.Types.Extensions" %>
<YAF:PageLinks runat="server" ID="PageLinks" />

<asp:Repeater ID="UserList" runat="server">
	<HeaderTemplate>
        <div class="row">
            <div class="col-xl-12">
                <div class="card mb-3">
                    <div class="card-header">
                        <i class="fa fa-users fa-fw text-secondary"></i> <YAF:LocalizedLabel ID="LocalizedLabel1" runat="server" LocalizedTag="title" />
                    </div>
                    <div class="card-body">
                        <div class="table-responsive">
	                        <table class="table tablesorter table-bordered table-striped" id="ActiveUsers">
                                <thead class="thead-light">
                                    <tr>
                                        <th>
                                            <YAF:LocalizedLabel ID="LocalizedLabel2" runat="server" 
                                                                LocalizedTag="username" />
                                        </th>
                                        <th>
                                            <YAF:LocalizedLabel ID="LocalizedLabelLatestActions" runat="server" 
                                                                LocalizedTag="latest_action" />
                                        </th>
                                        <th>
                                            <YAF:LocalizedLabel ID="LocalizedLabel3" runat="server" 
                                                                LocalizedTag="logged_in" />
                                        </th>
                                        <th>
                                            <YAF:LocalizedLabel ID="LocalizedLabel4" runat="server" 
                                                                LocalizedTag="last_active" />
                                        </th>
                                        <th>
                                            <YAF:LocalizedLabel ID="LocalizedLabel5" runat="server" 
                                                                LocalizedTag="active" />
                                        </th>
                                        <th>
                                            <YAF:LocalizedLabel ID="LocalizedLabel6" runat="server" 
                                                                LocalizedTag="browser" />
		                                </th>
                                        <th>
                                            <YAF:LocalizedLabel ID="LocalizedLabel7" runat="server" 
                                                                LocalizedTag="platform" />
		                                </th>
                                        <th id="Iptd_header1" runat="server" 
                                            visible='<%# this.PageContext.IsAdmin %>'>
                                            IP
		                                </th>
                                    </tr>
                                </thead>
                            <tbody>
	                    </HeaderTemplate>
		                <ItemTemplate>
                        <tr>
				        <td>		
					        <YAF:UserLink ID="NameLink" runat="server" 
                                          ReplaceName='<%# this.Eval(this.Get<BoardSettings>().EnableDisplayName ? "UserDisplayName" : "UserName") %>' 
                                          CrawlerName='<%# this.Eval("IsCrawler").ToType<int>() > 0 ? this.Eval("Browser").ToString() : string.Empty %>'
                                          UserID='<%# this.Eval("UserID").ToType<int>() %>' 
                                          Style='<%# this.Eval("Style").ToString() %>' />
                            <asp:PlaceHolder ID="HiddenPlaceHolder" runat="server" Visible='<%# Convert.ToBoolean(this.Eval("IsHidden"))%>' >
                                (<YAF:LocalizedLabel ID="Hidden" LocalizedTag="HIDDEN" runat="server" />)
                            </asp:PlaceHolder>				    
				        </td>
				        <td>				
					        <YAF:ActiveLocation ID="ActiveLocation2" 
                                                UserID='<%# (this.Eval("UserID") == DBNull.Value ? 0 : this.Eval("UserID")).ToType<int>() %>'
                                                UserName='<%# this.Eval("UserName") %>' 
                                                HasForumAccess='<%# Convert.ToBoolean(this.Eval("HasForumAccess")) %>' 
                                                ForumPage='<%# this.Eval("ForumPage") %>'
                                                ForumID='<%# (this.Eval("ForumID") == DBNull.Value ? 0 : this.Eval("ForumID")).ToType<int>() %>' 
                                                ForumName='<%# this.Eval("ForumName") %>'
                                                TopicID='<%# (this.Eval("TopicID") == DBNull.Value ? 0 : this.Eval("TopicID")).ToType<int>() %>' 
                                                TopicName='<%# this.Eval("TopicName") %>'
                                                LastLinkOnly="false"  runat="server"></YAF:ActiveLocation>     
				        </td>
				        <td>
					        <%# this.Get<IDateTime>().FormatTime((DateTime)((System.Data.DataRowView)Container.DataItem)["Login"]) %>
				        </td>				
				        <td>
					        <%# this.Get<IDateTime>().FormatTime((DateTime)((System.Data.DataRowView)Container.DataItem)["LastActive"]) %>
				        </td>
				        <td>
					        <%# this.Get<ILocalization>().GetTextFormatted("minutes", ((System.Data.DataRowView)Container.DataItem)["Active"])%>
				        </td>
				        <td>
					        <%# this.Eval("Browser") %>
				        </td>
				        <td>
					        <%# this.Eval("Platform") %>
				        </td>
                        <td id="Iptd1" runat="server" visible='<%# this.PageContext.IsAdmin %>'>
					         <a id="Iplink1" href='<%# string.Format(this.PageContext.BoardSettings.IPInfoPageURL,IPHelper.GetIp4Address(this.Eval("IP").ToString())) %>'
                                title='<%# this.GetText("COMMON","TT_IPDETAILS") %>' target="_blank" runat="server">
                             <%# IPHelper.GetIp4Address(this.Eval("IP").ToString())%></a>
				        </td>
                    </tr>	
                        </ItemTemplate>
                        <FooterTemplate>
                            </table>
                        </div>
                        </div>
            <div class="card-footer">
            <div id="ActiveUsersPager" class="tableSorterPager form-inline">
                <select class="pagesize custom-select custom-select-sm">
                    <option selected="selected"  value="10">10</option>
                    <option value="20">20</option>
                    <option value="30">30</option>
                    <option  value="40">40</option>
                </select>
                &nbsp;
                <div class="btn-group"  role="group">
                    <a href="#" class="first btn btn-secondary btn-sm"><span><i class="fas fa-angle-double-left"></i></span></a>
                    <a href="#" class="prev btn btn-secondary btn-sm"><span><i class="fas fa-angle-left"></i></span></a>
                    <input type="button" class="pagedisplay  btn btn-secondary btn-sm disabled"  style="width:150px" />
                    <a href="#" class="next btn btn-secondary btn-sm"><span><i class="fas fa-angle-right"></i></span></a>
                    <a href="#" class="last btn btn-secondary btn-sm"><span><i class="fas fa-angle-double-right"></i></span></a>
                </div>
            </div>
                </div>
            </div>
        </div>
                            </div>
    </FooterTemplate>
</asp:Repeater>