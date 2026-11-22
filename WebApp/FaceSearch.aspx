
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="FaceSearch.aspx.cs" Inherits="SmartPhotoGallery.FaceSearchPage" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Person Photo Search</title>
    <link href="Content/site.css" rel="stylesheet" />

    <style>
        .search-box {
            display: flex;
            gap: 10px;
            margin: 25px 0;
            max-width: 650px;
        }

        .search-input {
            flex: 1;
            padding: 12px;
            border: 1px solid #ccc;
            border-radius: 5px;
            font-size: 16px;
        }

        .search-button {
            padding: 12px 25px;
            background: #193b60;
            color: white;
            border: none;
            border-radius: 5px;
            cursor: pointer;
        }

        .status {
            display: block;
            margin: 15px 0;
            color: #333;
        }

        .photo-grid {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(220px, 1fr));
            gap: 20px;
            margin-top: 20px;
        }

        .photo-card {
            background: white;
            border-radius: 8px;
            padding: 12px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.12);
        }

        .photo-card img {
            width: 100%;
            height: 190px;
            object-fit: cover;
            border-radius: 5px;
        }

        .photo-info {
            margin-top: 10px;
            font-size: 14px;
            line-height: 1.7;
        }

        .photo-info strong {
            color: #193b60;
        }
    </style>
</head>

<body>
<form id="form1" runat="server">

    <div class="top">
        <a href="Default.aspx">Home</a>
        <h1>Person Photo Search</h1>
    </div>

    <div class="wrap">

        <h2>Search Photos by Person</h2>

        <p>Enter a person's name or designation to find matching photos.</p>

        <div class="search-box">

            <asp:TextBox
                ID="txtSearch"
                runat="server"
                CssClass="search-input"
                placeholder="Enter person name or designation">
            </asp:TextBox>

            <asp:Button
                ID="btnSearch"
                runat="server"
                Text="Search"
                CssClass="search-button"
                OnClick="btnSearch_Click" />

        </div>

        <asp:Label
            ID="lblStatus"
            runat="server"
            CssClass="status">
        </asp:Label>

        <asp:Panel ID="pnlResults" runat="server" Visible="false">

            <div class="photo-grid">

                <asp:Repeater ID="rptResults" runat="server">

                    <ItemTemplate>

                        <div class="photo-card">

                            <a href='<%# Eval("ImageUrl") %>' target="_blank">
                                <img
                                    src='<%# Eval("ImageUrl") %>'
                                    alt="Matching photo" />
                            </a>

                            <div class="photo-info">

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

                                <span>
                                    <%# Convert.ToInt32(Eval("Reviewed")) == 1
                                        ? "Reviewed"
                                        : "Pending review" %>
                                </span>

                            </div>

                        </div>

                    </ItemTemplate>

                </asp:Repeater>

            </div>

        </asp:Panel>

    </div>

</form>
</body>
</html>
