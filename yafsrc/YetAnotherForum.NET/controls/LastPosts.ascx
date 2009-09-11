<%@ Control Language="C#" AutoEventWireup="true" CodeFile="LastPosts.ascx.cs" Inherits="YAF.Controls.LastPosts" %>
<asp:Timer ID="LastPostUpdateTimer" runat="server" Interval="15000" OnTick="LastPostUpdateTimer_Tick">
</asp:Timer>
<div style="overflow: scroll; height: 400px;">
	<asp:UpdatePanel ID="LastPostUpdatePanel" runat="server" UpdateMode="Conditional">
		<ContentTemplate>
			<asp:UpdatePanel ID="InnerUpdatePanel" runat="server" UpdateMode="Conditional">
				<Triggers>
					<asp:AsyncPostBackTrigger ControlID="LastPostUpdateTimer" />
				</Triggers>
				<ContentTemplate>
					<asp:Repeater ID="repLastPosts" runat="server">
						<HeaderTemplate>
							<table class="content" width="100%" align="center">
								<tr>
									<td class="header2" align="center" colspan="2">
										<YAF:LocalizedLabel ID="Last10" LocalizedTag="LAST10" runat="server" />
									</td>
								</tr>
						</HeaderTemplate>
						<FooterTemplate>
							</table>
						</FooterTemplate>
						<ItemTemplate>
							<tr class="postheader">
								<td width="20%">
									<b>
										<YAF:UserLink ID="ProfileLink" runat="server" UserID='<%# Convert.ToInt32(Eval( "UserID" )) %>'
											UserName='<%# Eval( "UserName" ).ToString() %>' BlankTarget="true" />
									</b>
								</td>
								<td width="80%" class="small" align="left">
									<b>
										<YAF:LocalizedLabel ID="Posted" LocalizedTag="POSTED" runat="server" />
									</b>
									<%# YafServices.DateTime.FormatDateTime((System.DateTime)((System.Data.DataRowView)Container.DataItem)["Posted"]) %>
								</td>
							</tr>
							<tr class="post">
								<td>
									&nbsp;
								</td>
								<td valign="top" class="message">
									<YAF:MessagePostData ID="MessagePostPrimary" runat="server" DataRow="<%# Container.DataItem %>"
										ShowAttachments="false">
									</YAF:MessagePostData>
								</td>
							</tr>
						</ItemTemplate>
						<AlternatingItemTemplate>
							<tr class="postheader">
								<td width="20%">
									<b>
										<YAF:UserLink ID="ProfileLinkAlt" runat="server" UserID='<%# Convert.ToInt32(Eval( "UserID" )) %>'
											UserName='<%# Eval( "UserName" ).ToString() %>' BlankTarget="true" />
									</b>
								</td>
								<td width="80%" class="small" align="left">
									<b>
										<YAF:LocalizedLabel ID="PostedAlt" LocalizedTag="POSTED" runat="server" />
									</b>
									<%# YafServices.DateTime.FormatDateTime( ( System.DateTime ) ( ( System.Data.DataRowView ) Container.DataItem ) ["Posted"] )%>
								</td>
							</tr>
							<tr class="post_alt">
								<td>
									&nbsp;
								</td>
								<td valign="top" class="message">
									<YAF:MessagePostData ID="MessagePostAlt" runat="server" DataRow="<%# Container.DataItem %>"
										ShowAttachments="false">
									</YAF:MessagePostData>
								</td>
							</tr>
						</AlternatingItemTemplate>
					</asp:Repeater>
				</ContentTemplate>
			</asp:UpdatePanel>
		</ContentTemplate>
	</asp:UpdatePanel>
</div>
