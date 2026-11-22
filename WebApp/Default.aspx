
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="SmartPhotoGallery.DefaultPage" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Smart Photo Gallery</title>
    <link href="Content/site.css" rel="stylesheet" />

    <style>
        .home-search {
            display: flex;
            align-items: center;
            gap: 8px;
            flex-wrap: wrap;
        }

        .home-search select,
        .home-search input {
            padding: 11px;
            border-radius: 5px;
            border: 1px solid #ccc;
            font-size: 15px;
        }

        .home-search input {
            width: 220px;
        }

        .home-search button,
        .home-search input[type=submit] {
            padding: 11px 20px;
            background: #fff;
            color: #193b60;
            border: none;
            border-radius: 5px;
            cursor: pointer;
        }

        .person-results {
            margin-top: 30px;
        }

        .person-grid {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(230px, 1fr));
            gap: 20px;
            margin-top: 20px;
        }

        .person-card {
            background: white;
            padding: 12px;
            border-radius: 9px;
            box-shadow: 0 2px 9px rgba(0,0,0,.12);
        }

        .person-card img {
            width: 100%;
            height: 190px;
            object-fit: cover;
            border-radius: 6px;
        }

        .person-card a {
            text-decoration: none;
        }

        .person-info {
            margin-top: 10px;
            line-height: 1.7;
            color: #333;
            font-size: 14px;
        }

        .person-info strong {
            color: #193b60;
        }

        .search-message {
            display: block;
            margin-top: 18px;
            font-weight: 600;
        }
    </style>
</head>

<body>
<form id="form1" runat="server">

    <div class="top">

        <h1>Smart Photo Gallery</h1>

        <div class="home-search">

            <asp:DropDownList ID="ddlSearchType" runat="server">
                <asp:ListItem Text="Event" Value="Event" />
                <asp:ListItem Text="Person Name" Value="Person" />
                <asp:ListItem Text="Designation" Value="Designation" />
            </asp:DropDownList>

            <asp:TextBox
                ID="txtSearch"
                runat="server"
                placeholder="Enter search text..." />

            <asp:Button
                ID="btnSearch"
                runat="server"
                Text="Search"
                OnClick="btnSearch_Click" />

        </div>

        <a href="KeywordManagement.aspx">Keyword management</a>
        |
        <a href="PersonManagement.aspx">People</a>
        |
        <a href="FaceSearch.aspx">Face search</a>

    </div>

    <div class="wrap">

        <asp:Label
            ID="lblMessage"
            runat="server"
            CssClass="search-message">
        </asp:Label>

        <!-- Person search results -->

        <asp:Panel
            ID="pnlPersonResults"
            runat="server"
            Visible="false"
            CssClass="person-results">

            <h2>Matching Photos</h2>

            <div class="person-grid">

                <asp:Repeater
                    ID="rptPersonResults"
                    runat="server">

                    <ItemTemplate>

                        <div class="person-card">

                            <a href='<%# Eval("ImageUrl") %>' target="_blank">

                                <img
                                    src='<%# Eval("ImageUrl") %>'
                                    alt="Person photo" />

                            </a>

                            <div class="person-info">

                                <strong><%#: Eval("PersonName") %></strong>
                                <br />

                                Designation:
                                <%#: Eval("Designation") %>
                                <br />

                                Year:
                                <%#: Eval("YearName") %>
                                <br />

                                Event:
                                <%#: Eval("EventName") %>
                                <br />

                                Similarity:
                                <%#: Eval("SimilarityPercent") %>%
                                <br />

                                Review:
                                <%#: Convert.ToInt32(Eval("Reviewed")) == 1
                                    ? "Reviewed"
                                    : "Pending review" %>

                            </div>

                        </div>

                    </ItemTemplate>

                </asp:Repeater>

            </div>

        </asp:Panel>

        <!-- Existing year cards -->

        <asp:Panel ID="pnlYears" runat="server">

            <h2>Browse by year</h2>

            <asp:Repeater ID="rptYears" runat="server">

                <ItemTemplate>

                    <a class="card"
                       href='Events.aspx?year=<%# Server.UrlEncode(Eval("YearName").ToString()) %>'>

                        <img
                            src='<%# Eval("PreviewUrl") %>'
                            alt="Year preview" />

                        <h3><%#: Eval("YearName") %></h3>

                        <p><%#: Eval("EventCount") %> events</p>

                    </a>

                </ItemTemplate>

            </asp:Repeater>

        </asp:Panel>

    </div>

</form>
</body>
</html>
