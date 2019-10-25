<%@ Page Language="c#" AutoEventWireup="True" Inherits="YAF.Error" CodeBehind="error.aspx.cs" %>

<!doctype html>
<html lang="en">
   <head runat="server">
       <title>Forum Error</title>
       <meta name="viewport" content="width=device-width, initial-scale=1.0">
       <link href="~/Content/Themes/yaf/bootstrap-forum.min.css" rel="stylesheet" runat="server" />
   </head>
   <body>
       <div class="jumbotron">
           <h1 class="display-3">Forum Error</h1>
           <p class="lead">
               <asp:Label ID="ErrorMessage" Enabled="true" runat="server" />
           </p>
           <hr class="my-4"/>
           <p>
               Please contact the administrator if this message persists.
           </p>
           <div class="alert alert-info">
               <strong>Note:</strong>&nbsp;If you are the administrator, and need help with this problem, then Turn off <strong>CustomErrors</strong> in your <strong>web.config</strong>.
           </div>
           <p class="lead">
               <a href="Default.aspx" class="btn btn-primary btn-lg" role="button"><i class="fa fa-home"></i> Try Again</a>
       </div>
   </body>
</html>
