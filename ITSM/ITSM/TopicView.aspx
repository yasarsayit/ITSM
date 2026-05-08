<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TopicView.aspx.cs" Inherits="ITSM.TopicView" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .view-wrapper {
            max-width: 1000px;
            margin: 30px auto;
            padding: 20px;
        }

        .view-header {
            background: #0F406B;
            color: white;
            padding: 20px 30px;
            border-radius: 12px 12px 0 0;
        }

        .view-title {
            font-size: 24px;
            font-weight: 700;
            margin: 0;
        }

        .view-summary {
            font-size: 14px;
            font-style: italic;
            color: #d1d9e1;
            margin-top: 10px;
            margin-bottom: 0;
        }

        .view-content-box {
            background: white;
            padding: 40px;
            border: 1px solid #e0e6ed;
            border-top: none;
            border-radius: 0 0 12px 12px;
            box-shadow: 0 10px 25px rgba(15, 64, 107, 0.05);
            min-height: 400px;
        }

        .html-content {
            font-family: Helvetica, Arial, sans-serif;
            font-size: 16px;
            line-height: 1.6;
            color: #333;
        }

            .html-content img {
                max-width: 100%;
                height: auto;
                border-radius: 8px;
                margin: 15px 0;
            }

            .html-content a {
                color: #0F406B;
                text-decoration: underline;
            }

        .back-btn {
            margin-bottom: 20px;
            display: inline-block;
            color: #0F406B;
            font-weight: bold;
            text-decoration: none;
        }

            .back-btn:hover {
                color: #0b2e4d;
                text-decoration: underline;
            }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="view-wrapper">
        <a href="Requests.aspx" class="back-btn"><i class="fa fa-arrow-left"></i><%= gettext("backtorqst"," Back to Requests")%></a>

        <div class="view-header">
            <h1 class="view-title">
                <asp:Literal ID="litTitle" runat="server"></asp:Literal></h1>
            <p class="view-summary">
                <asp:Literal ID="litSummary" runat="server"></asp:Literal>
            </p>
        </div>

        <div class="view-content-box">
            <div class="html-content">
                <asp:Literal ID="litContent" runat="server"></asp:Literal>
            </div>
        </div>
    </div>
</asp:Content>
