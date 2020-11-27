<%@ Page Title="Price" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Price.aspx.cs" Inherits="Price" %>

<asp:Content ContentPlaceHolderID="HeaderAdditionPlaceHolder" runat="server">
    <link rel="canonical" href="https://niceapi.net/Price" />

    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <style>
        * {
            box-sizing: border-box;
        }

        .columns {
            float: left;
            width: 25.0%;
            padding: 8px;
        }

        .price {
            list-style-type: none;
            border: 1px solid #eee;
            margin: 0;
            padding: 0;
            -webkit-transition: 0.3s;
            transition: 0.3s;
        }

            .price:hover {
                box-shadow: 0 8px 12px 0 rgba(0,0,0,0.2);
            }

            .price .header {
                font-size: 25px;
            }

            .price li {
                border-bottom: 1px solid #eee;
                padding: 20px;
                text-align: center;
            }

            .price .grey {
                font-size: 20px;
            }

        .button {
            border: none;
            padding: 10px 25px;
            text-align: center;
            text-decoration: none;
            font-size: 18px;
        }

        @media only screen and (max-width: 600px) {
            .columns {
                width: 100%;
            }
        }

        .promotion {
            color: black;
            background-color: darkorange;
            text-align: center;
        }
    </style>

</asp:Content>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h2 style="text-align: center">Our account types:</h2>
    <br />
    <h2 class="promotion">As we are new on the web, we give you a 50% discount.</h2>


    <asp:Literal ID="lit" runat="server" />
    <br />

    <script type="text/javascript">
        var vRegister = document.getElementById("buttonRegister");
        var vUpgradePayAsYouSend = document.getElementById("buttonUpgradePayAsYouSend");
        var vUpgradePayMonthly = document.getElementById("buttonUpgradePayMonthly");
        var vUpgradePayMonthlyDifPrice = document.getElementById("buttonUpgradePayMonthlyDifPrice");
        var vUpgradeSystemDuplication = document.getElementById("buttonUpgradeSystemDuplication");

        if (vRegister != null) {
            vRegister.onclick = function () {
                location.href = "Register";
            };
        }

        if (vUpgradePayAsYouSend != null) {
            vUpgradePayAsYouSend.onclick = function () {
                location.href = "Upgrade?Type=PayAsYouSend";
            };
        }

        if (vUpgradePayMonthly != null) {
            vUpgradePayMonthly.onclick = function () {
                location.href = "Upgrade?Type=PayMonthly";
            };
        }

        if (vUpgradePayMonthlyDifPrice != null) {
            vUpgradePayMonthlyDifPrice.onclick = function () {
                location.href = "Upgrade?Type=Pay Monthly";
            };
        }

        if (vUpgradeSystemDuplication != null) {
            vUpgradeSystemDuplication.onclick = function () {
                location.href = "Upgrade?Type=SystemDuplication";
            };
        }

    </script>

</asp:Content>


