<%@ Control Language="c#" AutoEventWireup="True" CodeFile="test_data.ascx.cs" Inherits="YAF.Pages.Admin.test_data" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<YAF:PageLinks runat="server" id="PageLinks" />
<YAF:adminmenu runat="server" id="Adminmenu1">

<DotNetAge:Tabs id="HostSettingsTabs" runat="server" activetabevent="Click" asyncload="false"
		autopostback="false" collapsible="false" contentcssclass="" contentstyle="" deselectable="false"
		enabledcontentcache="false" headercssclass="" headerstyle="" onclienttabadd=""
		onclienttabdisabled="" onclienttabenabled="" onclienttabLoad="" onclienttabremove=""
		onclienttabselected="" onclienttabshow="" selectedindex="0" sortable="false" spinner="">
		<Animations>
		</Animations> 
		
    
<Views>		
<DotNetAge:View runat="server" id="View1" text="Users" navigateurl="" headercssclass=""
				headerstyle="" target="_blank">	
  <table class="content" cellspacing="1" cellpadding="0" width="100%">     
     <tr>
      <td class="header1" colspan="2">Create Test Users</td>
    </tr>
    <tr>
      <td class="postheader" width="50%">Number of users to create:</td>
      <td class="post">
        <asp:TextBox id="UsersNumber" runat="server">0</asp:TextBox>
        <asp:RequiredFieldValidator id="Requiredfieldvalidator1" runat="server" enableclientscript="False" text="0"
          controltovalidate="UsersNumber" errormessage="User's number is required."></asp:RequiredFieldValidator></td>
    </tr>
     <tr>
      <td class="postheader" width="50%">Create in CurrentBoard:</td>
      <td class="post"><asp:CheckBox id="CreateUsersInCurrentBoardCheckBox" Enabled="false" runat="server" checked="true"/></td>
      </tr>    
       <tr>
      <td class="postheader">Password(for all users):</td>
      <td class="post">
        <asp:TextBox id="Password" runat="server" >testuser?</asp:TextBox>
        <asp:RequiredFieldValidator id="PasswordRequiredfieldvalidator" runat="server" enableclientscript="False" text="0"
          controltovalidate="Password" errormessage="Users password is required."></asp:RequiredFieldValidator></td>
    </tr>
    <tr>
      <td class="postheader">Confirm Password:</td>
      <td class="post">
        <asp:TextBox id="Password2" runat="server" >testuser?</asp:TextBox>
        <asp:CompareValidator id="PasswordConfirmComparevalidator" runat="server" NAME="Comparevalidator1" enableclientscript="False"
          controltovalidate="Password2" errormessage="Passwords didn't match." ControlToCompare="Password"></asp:CompareValidator></td>
    </tr>
    <tr>
      <td class="postheader">Password Question(for all users):</td>
      <td class="post">
        <asp:TextBox id="Question" runat="server">testuser?</asp:TextBox>
        <asp:RequiredFieldValidator id="Requiredfieldvalidator3" runat="server" enableclientscript="False"
          controltovalidate="Question" errormessage="Password Question is Required."></asp:RequiredFieldValidator></td>
    </tr>    
    <tr>
      <td class="postheader">Password Answer(for all users):</td>
      <td class="post">
        <asp:TextBox id="Answer" runat="server">yes</asp:TextBox>
        <asp:RequiredFieldValidator id="Requiredfieldvalidator4" runat="server" enableclientscript="False"
          controltovalidate="Answer" errormessage="Password Answer is Required."></asp:RequiredFieldValidator></td>
    </tr>  
       <tr>
      <td class="postheader">Location(for all users):</td>
      <td class="post">
        <asp:TextBox id="Location" runat="server">Earth</asp:TextBox></td>
    </tr> 
     <tr>
      <td class="postheader">Home Page(for all users):</td>
      <td class="post">
        <asp:TextBox id="HomePage" runat="server"></asp:TextBox></td>
    </tr>
    <tr>
      <td class="postheader">Time Zone(for all users):</td>
      <td class="post">
        <asp:DropDownList id="TimeZones" runat="server" datavaluefield="Value" datatextfield="Name"></asp:DropDownList></td>
    </tr>
    <tr>
      <td class="header1" colspan="2">Delete Test Users</td>
    </tr>   
     <tr>
      <td class="postheader" width="50%">Delete All Test Users:</td> 
      <td class="post"><asp:CheckBox id="DeleteUsersCheckBox" runat="server" enabled="true" checked="false"  /></td>
      </tr>
         <tr>
      <td class="postheader" width="50%">From Current Board:</td>
      <td class="post"><asp:CheckBox id="DeleteUsersCurrentBoardCheckBox" enabled="false" runat="server" checked="true"/></td>
      </tr>
        <tr>
      <td class="postheader" colspan="2">Warning! This is a test/debug feature. Never use it in production environment.</td>
    </tr>
      </table>
    	</DotNetAge:View>      	
   	<DotNetAge:View runat="server" id="View2" text="Boards" navigateurl="" headercssclass=""
				headerstyle="" target="_blank" >	
	<table class="content" cellspacing="1" cellpadding="0" width="100%">	
	<tr>
      <td class="header1" colspan="2">Create Boards</td>
    </tr> 
     <tr>
       <td class="postheader" width="50%">Number of boards to create(max 10):</td>
       <td class="post">
        <asp:TextBox id="BoardNumber" runat="server">0</asp:TextBox>
        <asp:RequiredFieldValidator  id="Requiredfieldvalidator10" runat="server" enableclientscript="False" text="0"
          controltovalidate="BoardNumber" errormessage="Boards's number is required."></asp:RequiredFieldValidator> </td> 
      </tr>
        <tr>
        <td class="postheader" width="50%">Board Membership Name:</td> 
         <td class="post"> 
        <asp:TextBox id="BoardMembershipName" enabled="false" runat="server"></asp:TextBox></td>
          </tr>
       <tr>
        <td class="postheader" width="50%">Board Roles Name:</td> 
          <td class="post">
        <asp:TextBox id="BoardRolesName" enabled="false" runat="server"></asp:TextBox></td>
       </tr> 
      <tr>
       <tr>
        <td class="postheader">Number of Categories in Each Board:</td>     
      <td class="post">
       <asp:TextBox id="BoardsCategoriesNumber" runat="server" enabled="false" datavaluefield="TopicID" datatextfield="Subject">0</asp:TextBox></td>   
     </tr>
       <tr>
        <td class="postheader">Number of Forums in Each Category:</td>     
      <td class="post">
       <asp:TextBox id="BoardsForumsNumber" runat="server" enabled="false" datavaluefield="TopicID" datatextfield="Subject">0</asp:TextBox></td>   
     </tr>
           <tr>
        <td class="postheader">Number of Topics in Each Forum:</td>     
      <td class="post">
       <asp:TextBox id="BoardsTopicsNumber" runat="server" enabled="false" datavaluefield="TopicID" datatextfield="Subject">0</asp:TextBox></td>   
     </tr>
     <tr>
      <td class="postheader">Number of Messages in Each Topic:</td>  
      <td class="post">
       <asp:TextBox id="BoardsMessagesNumber" runat="server" enabled="false" datavaluefield="TopicID" datatextfield="Subject">0</asp:TextBox></td>   
     </tr>
      <td class="header1" colspan="2">Delete Boards</td>
    </tr>   
    <tr>
      <td class="postheader" width="50%">Delete All Test Boards:</td>        
      <td class="post"><asp:CheckBox id="DeleteBoardsCheckBox" runat="server" enabled="false" checked="false" /></td>
     </tr> 
     <tr>
      <td class="postheader" colspan="2">Warning! This is a test/debug feature. Never use it in production environment.</td>
    </tr>     
   </table>
    </DotNetAge:View>  
    	<DotNetAge:View runat="server" id="View3" text="Categories" navigateurl="" headercssclass=""
				headerstyle="" target="_blank">	
	 <table class="content" cellspacing="1" cellpadding="0" width="100%">
	 <tr>			
      <td class="header1" colspan="2">Create Test Categories</td>
     </tr> 
     <tr>
       <td class="postheader" width="50%">Number of categories to create(max 100):</td>
      <td class="post">
        <asp:TextBox id="CategoriesNumber" runat="server">0</asp:TextBox>
        <asp:RequiredFieldValidator  id="Requiredfieldvalidator5" runat="server" enableclientscript="False" text="0"
          controltovalidate="CategoriesNumber" errormessage="Categories's number is required."></asp:RequiredFieldValidator></td>   
      </tr>
       <tr>
        <td class="postheader">Number of Forums in Each Category:</td>     
      <td class="post">
       <asp:TextBox id="CategoriesForumsNumber" runat="server" enabled="false" datavaluefield="TopicID" datatextfield="Subject">0</asp:TextBox></td>   
     </tr>
                <tr>
        <td class="postheader">Number of Topics in Each Forum:</td>     
      <td class="post">
       <asp:TextBox id="CategoriesTopicsNumber" runat="server" enabled="false" datavaluefield="TopicID" datatextfield="Subject">0</asp:TextBox></td>   
     </tr>
     <tr>
      <td class="postheader">Number of Messages in Each Topic:</td>  
      <td class="post">
       <asp:TextBox id="CategoriesMessagesNumber" runat="server" enabled="false" datavaluefield="TopicID" datatextfield="Subject">0</asp:TextBox></td>   
     </tr>
    	 <tr>			
      <td class="header1" colspan="2">Delete Test Categories</td>
    </tr>
          <tr>
      <td class="postheader" width="50%">Delete All Test Categories:</td>
      <td class="post"><asp:CheckBox id="DeleteCategoriesCheckBox" runat="server" enabled="false" checked="false" /></td>
      </tr>
       <tr>
      <td class="postheader" colspan="2">Warning! This is a test/debug feature. Never use it in production environment.</td>
    </tr>
    </table>
    </DotNetAge:View>  
    	<DotNetAge:View runat="server" id="View4" text="Forums" navigateurl="" headercssclass=""
				headerstyle="" target="_blank">	
		 <table class="content" cellspacing="1" cellpadding="0" width="100%">
	 <tr>
      <td class="header1" colspan="2">Create Test Forums</td>
     </tr>
     <tr>
      <td class="postheader" width="50%">Number of forums to create:</td>
            <td class="post">            
        <asp:TextBox id="ForumsNumber" runat="server">0</asp:TextBox>
         <asp:RequiredFieldValidator id="Requiredfieldvalidator6" runat="server" enableclientscript="False" text="0"
          controltovalidate="ForumsNumber" errormessage="Forums' number is required."></asp:RequiredFieldValidator></td>
      </tr> 
    <tr>
      <td class="postheader" width="50%">Count topics and messages in statistics:</td>
      <td class="post"><asp:CheckBox id="ForumsCountMessages" runat="server" enabled="true" checked="true" /></td>
    </tr>
       <tr>
      <td class="postheader" width="50%">Hide forums if no access:</td>
      <td class="post"><asp:CheckBox id="ForumsHideNoAccess" runat="server" enabled="true" checked="true" /></td>
    </tr>
          <tr>
      <td class="postheader">Select common access mask for all groups:</td>
      <td class="post">
        <asp:DropDownList id="ForumsStartMask" runat="server"  datavaluefield="AccessMaskID" datatextfield="Name" onselectedindexchanged="ForumsCategory_OnSelectedIndexChanged"></asp:DropDownList></td>
       </tr>
         <tr>
      <td class="postheader">Select a group with special rights for the test data:</td>
      <td class="post">
        <asp:DropDownList id="ForumsGroups" runat="server"  datavaluefield="GroupID" datatextfield="Name" onselectedindexchanged="ForumsCategory_OnSelectedIndexChanged"></asp:DropDownList></td>
       </tr> 
         <tr>
      <td class="postheader">Select an access mask with special rights for the test data:</td>
      <td class="post">
        <asp:DropDownList id="ForumsAdminMask" runat="server"  datavaluefield="AccessMaskID" datatextfield="Name" onselectedindexchanged="ForumsCategory_OnSelectedIndexChanged"></asp:DropDownList></td>
       </tr> 
        <tr>
      <td class="postheader">Choose category:</td>
      <td class="post">
        <asp:DropDownList id="ForumsCategory" runat="server" autopostback="true" datavaluefield="CategoryID" datatextfield="Name" OnDataBound="ForumsCategory_OnSelectedIndexChanged" onselectedindexchanged="ForumsCategory_OnSelectedIndexChanged"></asp:DropDownList></td>
       </tr> 
       <tr> 
    <td class="postheader">Choose parent forum:</td>
      <td class="post">
        <asp:DropDownList id="ForumsParent" runat="server" datavaluefield="ForumID" datatextfield="Title" ></asp:DropDownList></td>
    </tr> 
     <tr>
        <td class="postheader">Number of Topics in Each Forum:</td>     
      <td class="post">
       <asp:TextBox id="ForumsTopicsNumber" runat="server" enabled="true" datavaluefield="TopicID" datatextfield="Subject">0</asp:TextBox></td>   
     </tr>
     <tr>
      <td class="postheader">Number of Messages in Each Topic:</td>  
      <td class="post">
       <asp:TextBox id="ForumsMessagesNumber" runat="server" enabled="true" datavaluefield="TopicID" datatextfield="Subject">0</asp:TextBox></td>   
     </tr>
      <tr>
      <td class="header1" colspan="2">Delete Test Forums</td>
     </tr>
    <tr>
      <td class="postheader" width="50%">Delete All Test Forums:</td>      
      <td class="post"><asp:CheckBox id="DeleteForumsCheckBox" runat="server"  enabled="true"  checked="false" /> </td>
      </tr>
      <tr>
      <td class="postheader" colspan="2">Warning! This is a test/debug feature. Never use it in production environment.</td>
    </tr>
    	</table>
    </DotNetAge:View>  
    <DotNetAge:View runat="server" id="View5" text="Topics" navigateurl="" headercssclass=""
				headerstyle="" target="_blank">	
	 <table class="content" cellspacing="1" cellpadding="0" width="100%">			
    <tr>
      <td class="header1" colspan="2">Create Test Topics</td>
    </tr>
    <tr>
          <td class="postheader" width="50%">Number of topics to create:</td>
            <td class="post">
        <asp:TextBox id="TopicsNumber" runat="server">0</asp:TextBox>
        <asp:RequiredFieldValidator id="Requiredfieldvalidator7" runat="server" enableclientscript="False" text="0"
          controltovalidate="TopicsNumber" errormessage="Categories' number is required."></asp:RequiredFieldValidator></td>
           </tr>
    	 <tr>
    	<td class="post" width="80%">
			<asp:DropDownList ID="TopicsPriorityList" runat="server" />			
		</td>
		</tr>
			 <tr>
      <td class="postheader" width="50%">Create Poll:</td>
      <td class="post"><asp:CheckBox id="PollCreate" runat="server" enabled="true" checked="false" /></td>
    </tr>
    <tr>  
      <td class="postheader">Choose category:</td>
      <td class="post">
        <asp:DropDownList id="TopicsCategory" runat="server" autopostback="true" datavaluefield="CategoryID" datatextfield="Name" OnDataBound="TopicsCategory_OnSelectedIndexChanged" onselectedindexchanged="TopicsCategory_OnSelectedIndexChanged"></asp:DropDownList></td>
     </tr>
     <tr> 
    <td class="postheader">Choose forum:</td>
      <td class="post">
        <asp:DropDownList id="TopicsForum" runat="server"  autopostback="false" datavaluefield="ForumID" datatextfield="Title"></asp:DropDownList></td>
    </tr> 
     <tr>
        <td class="postheader">Number of Messages in Each Topic:</td>        
      <td class="post">
       <asp:TextBox id="TopicsMessagesNumber" runat="server" enabled="true" datavaluefield="TopicID" datatextfield="Subject">0</asp:TextBox></td>   
     </tr>
     <tr>
     <td class="header1" colspan="2">Delete Test Topics</td>
    </tr>
    <tr>
      <td class="postheader" width="50%">Delete All Test Topics:</td>
      <td class="post"><asp:CheckBox id="DeleteTopicsCheckBox" runat="server" enabled="false" checked="false" /></td>
    </tr>
     <tr>
      <td class="postheader" colspan="2">Warning! This is a test/debug feature. Never use it in production environment.</td>
    </tr>
    </table>
     </DotNetAge:View>  
    	<DotNetAge:View runat="server" id="View6" text="Messages" navigateurl="" headercssclass=""
				headerstyle="" target="_blank">	
	 <table class="content" cellspacing="1" cellpadding="0" width="100%">			
    <tr>
      <td class="header1" colspan="2">Create Test Messages</td>
    </tr> 
     <tr> 
     <td class="postheader" width="50%">Number of messages to create:</td>
            <td class="post">
        <asp:TextBox id="PostsNumber" runat="server">0</asp:TextBox>
        <asp:RequiredFieldValidator id="Requiredfieldvalidator8" runat="server" enableclientscript="False" text="0"
          controltovalidate="PostsNumber" errormessage="Categories' number is required."></asp:RequiredFieldValidator></td>
    </tr>
        <tr>
      <td class="postheader">Choose category:</td>
      <td class="post">
        <asp:DropDownList id="PostsCategory" runat="server" autopostback="true" datavaluefield="CategoryID" datatextfield="Name" OnDataBound="PostsCategory_OnSelectedIndexChanged" onselectedindexchanged="PostsCategory_OnSelectedIndexChanged"></asp:DropDownList></td>
    </tr>
     <tr>
    <td class="postheader">Choose forum:</td>
      <td class="post">
        <asp:DropDownList id="PostsForum" runat="server" datavaluefield="ForumID" autopostback="true" datatextfield="Title" onselectedindexchanged="PostsForum_OnSelectedIndexChanged" ></asp:DropDownList></td>
    </tr>
     <tr>
        <td class="postheader">Choose topic:</td>        
      <td class="post">
       <asp:DropDownList id="PostsTopic" runat="server" datavaluefield="TopicID" datatextfield="Subject"></asp:DropDownList></td>   
     </tr>
     <tr>
      <td class="header1" colspan="2">Delete Test Messages</td>
    </tr>      
     <tr>
      <td class="postheader" width="50%">Delete All Test Messages:</td>
      <td class="post"><asp:CheckBox id="DeletePostsCheckBox" runat="server" enabled="false" checked="false"/></td>
      </tr>
       <tr>
      <td class="postheader" colspan="2">Warning! This is a test/debug feature. Never use it in production environment.</td>
    </tr>
     </table>
     </DotNetAge:View>  
    	<DotNetAge:View runat="server" id="View7" text="PMessages" navigateurl="" headercssclass=""
				headerstyle="" target="_blank" enabled="true" >	
	 <table class="content" cellspacing="1" cellpadding="0" width="100%">			
      <tr>
     <td class="header1" colspan="2">Create Test Private Messages</td>
    </tr>
     <tr>
       <td class="postheader" width="50%">Number of pmessages to create:</td>
      <td class="post">
        <asp:TextBox id="PMessagesNumber" runat="server">0</asp:TextBox>
        <asp:RequiredFieldValidator  id="Requiredfieldvalidator9" runat="server" enableclientscript="False" text="0"
        controltovalidate="PMessagesNumber" errormessage="Pmessages's number is required."></asp:RequiredFieldValidator></td>   
    </tr> 
    <tr>
      <td class="postheader" width="50%">From User:</td>
      <td class="post">
        <asp:TextBox id="From" runat="server"></asp:TextBox>        
        <asp:RequiredFieldValidator  id="Requiredfieldvalidator2" runat="server" enableclientscript="False" text="0"
          controltovalidate="From" errormessage="User Name is Required."></asp:RequiredFieldValidator></td>   
   
    </tr> 
    <tr>
       <td class="postheader" width="50%">To User:</td>
      <td class="post">
        <asp:TextBox id="To" runat="server"></asp:TextBox>
        <asp:RequiredFieldValidator  id="Requiredfieldvalidator11" runat="server" enableclientscript="False" text="0"
          controltovalidate="To" errormessage="User Name is Required."></asp:RequiredFieldValidator></td>   
    </tr> 
      <tr>
      <td class="postheader" width="50%">Send To All:</td>
      <td class="post"><asp:CheckBox id="PMessagesToAll" runat="server" enabled="false" checked="false"/></td>
      </tr>
      <tr>
      <td class="header1" colspan="2">Delete Test Private Messages(Disabled)</td>
    </tr> 
          <tr>
      <td class="postheader" width="50%">Delete All Test Private Messages:</td>
      <td class="post"><asp:CheckBox id="DeletePMessagesCheckBox" runat="server" enabled="false" checked="false" /></td>
    </tr>
    <tr>
      <td class="postheader" colspan="2">Warning! This is a test/debug feature. Never use it in production environment.</td>
    </tr>   
     </table>
      </DotNetAge:View>  
    	<DotNetAge:View runat="server" id="View8" text="Combinations" navigateurl="" headercssclass=""
				headerstyle="" target="_blank"  enabled="false">	
		 <table class="content" cellspacing="1" cellpadding="0" width="100%">		
      <tr>
            <td class="header2" align="center" colspan="2">Delete Test Data:</td>
      </tr>
      </table>
      </DotNetAge:View>  
      </Views>
      </DotNetAge:Tabs>
     <tr>
      <td class="footer1" align="center" colspan="2">
        <asp:Button id="LaunchGenerator" runat="server" text="Launch Generator" onclick="CreateTestData_Click"></asp:Button>
         <asp:button id="cancel" runat="server" text="Cancel" onclick="cancel_Click"></asp:button></td>
    </tr> 
</YAF:adminmenu>
<YAF:SmartScroller id="SmartScroller1" runat = "server" />
