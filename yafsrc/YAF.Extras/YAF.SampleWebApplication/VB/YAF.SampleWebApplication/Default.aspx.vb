' Yet Another Forum.NET
' Copyright (C) Jaben Cargman
' http://www.yetanotherforum.net/
' 
' This program is free software; you can redistribute it and/or
' modify it under the terms of the GNU General Public License
' as published by the Free Software Foundation; either version 2
' of the License, or (at your option) any later version.
' 
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
' 
' You should have received a copy of the GNU General Public License
' along with this program; if not, write to the Free Software
' Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

Imports YAF.Classes
Imports YAF.Controls
Imports YAF.Core
Imports YAF.Types.Interfaces
Imports YAF.Utilities
Imports YAF.Utils

Public Class _Default
    Inherits Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Try
            Dim csType As Type = GetType(Page)

            If Not YafContext.Current.[Get](Of YafBoardSettings)().ShowRelativeTime Then
                Return
            End If

            Dim uRLToResource = Config.JQueryFile

            If Not uRLToResource.StartsWith("http") Then
                uRLToResource = YafForumInfo.GetURLToResource(Config.JQueryFile)
            End If

            ScriptManager.RegisterClientScriptInclude(Me, csType, "JQuery", uRLToResource)

            ScriptManager.RegisterClientScriptInclude(Me, csType, "jqueryTimeagoscript", YafForumInfo.GetURLToResource("js/jquery.timeago.js"))

            ScriptManager.RegisterStartupScript(Me, csType, "timeagoloadjs", JavaScriptBlocks.TimeagoLoadJs, True)
        Catch generatedExceptionName As Exception
            Me.Response.Redirect("~/forum/install/default.aspx")
        End Try
    End Sub
End Class