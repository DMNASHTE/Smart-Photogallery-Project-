<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SearchResults.aspx.cs" Inherits="SmartPhotoGallery.SearchResultsPage" %>
<!DOCTYPE html><html><head runat="server"><title>Search results</title><link href="Content/site.css" rel="stylesheet" /></head>
<body><form id="form1" runat="server"><div class="top"><a href="Default.aspx">Home</a><h1>Event search results</h1></div><div class="wrap">
<asp:Label ID="lblQuery" runat="server" /><asp:Repeater ID="rptResults" runat="server"><ItemTemplate>
<a class="card" href='Gallery.aspx?year=<%# Server.UrlEncode(Eval("YearName").ToString()) %>&event=<%# Server.UrlEncode(Eval("EventName").ToString()) %>'>
<img src='<%# Eval("PreviewUrl") %>' alt="" /><h3><%# Eval("EventName") %></h3><p><%# Eval("YearName") %></p></a>
</ItemTemplate></asp:Repeater></div></form></body></html>
