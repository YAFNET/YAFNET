<%@ Page Language="c#" AutoEventWireup="True" Inherits="YAF.Error" CodeBehind="error.aspx.cs" %>

<!doctype html>
<html lang="en">
   <head runat="server">
       <meta charset="utf-8">
       <title>Forum Error</title>
       <meta name="viewport" content="width=device-width, initial-scale=1.0">
       <link href="~/Content/Themes/yaf/bootstrap-forum.min.css" rel="stylesheet" runat="server" />
       <link href="~/Content/forum.min.css" rel="stylesheet" runat="server" />
       <script src="//code.jquery.com/jquery-3.7.1.min.js"></script>
       <script src="<%= this.ResolveUrl("~/Scripts/jquery.ForumExtensions.min.js") %>"></script>
   </head>
   <body>
   <div class="container">
       <div class="bg-light p-5 rounded mt-3">
           <h1 class="display-3">Forum Error</h1>
           <p class="lead">
               <asp:Label ID="ErrorMessage" Enabled="true" runat="server" />
           </p>
           <asp:PlaceHolder runat="server" ID="ErrorDescriptionHolder" Visible="False">
               <p><button class="btn btn-outline-danger" type="button" data-bs-toggle="collapse" data-bs-target="#collapseError" aria-expanded="false" aria-controls="collapseError">
                   Show Full Error
               </button></p>
               <div class="collapse" id="collapseError">
                   <div class="card card-body">
                       <pre class="stacktrace"><code><asp:Literal runat="server" ID="ErrorDescription"></asp:Literal></code></pre>
                   </div>
               </div>
           </asp:PlaceHolder>

           <hr class="my-4"/>
           <p>
               Please contact the administrator if this message persists.
           </p>
           <p class="lead">
           <a href="Default.aspx" class="btn btn-primary btn-lg" role="button"><i class="fa fa-home"></i> Try Again</a>
       </div>
   </div>
   </body>
</html>
