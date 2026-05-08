<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ITSM._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .dashboard-card {
            background: #fff;
            border-radius: 10px;
            box-shadow: 0 4px 15px rgba(0,0,0,0.05);
            border: none;
            margin-bottom: 25px;
        }

        .card-header-custom {
            background-color: #0F406B;
            color: white;
            border-radius: 10px 10px 0 0 !important;
            padding: 15px 20px;
            font-weight: 600;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .table-custom th {
            background-color: #f8f9fa;
            color: #0F406B;
            font-size: 12px;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }

        .badge-status {
            font-size: 10px;
            padding: 5px 10px;
        }


        .dashboard-card {
            background: #fff;
            border-radius: 10px;
            box-shadow: 0 4px 15px rgba(0,0,0,0.05);
            border: none;
            margin-bottom: 25px;
        }

        .card-header-custom {
            background-color: #0F406B;
            color: white;
            border-radius: 10px 10px 0 0 !important;
            padding: 15px 20px;
            font-weight: 600;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .table-custom th {
            background-color: #f8f9fa;
            color: #0F406B;
            font-size: 12px;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }

        .badge-status {
            font-size: 10px;
            padding: 5px 10px;
        }

        .sla-danger {
            color: #dc3545 !important;
            font-weight: bold;
        }

        .sla-blink {
            animation: blinker 1.5s linear infinite;
            background-color: #ff0000;
            color: white;
            padding: 10px;
            border-radius: 5px;
            margin-bottom: 15px;
            text-align: center;
            font-weight: bold;
        }

        @keyframes blinker {
            50% {
                opacity: 0.5;
            }
        }

        .status-processing {
            background-color: #fff3cd !important;
            color: #856404 !important;
        }

        .status-rejected {
            background-color: #f8d7da !important;
            color: #721c24 !important;
        }

        .status-completed {
            background-color: #d4edda !important;
            color: #155724 !important;
        }

        .status-waiting {
            background-color: #ffffff !important;
        }


        .table-custom tbody tr td {
            background-color: transparent !important;
        }



        .avg-time-card {
            background: linear-gradient(135deg, #0F406B 0%, #1a5f9a 100%);
            color: white;
            padding: 20px;
            border-radius: 12px;
            box-shadow: 0 4px 15px rgba(15, 64, 107, 0.2);
            border: none;
        }

        .avg-icon {
            width: 50px;
            height: 50px;
            background: rgba(255, 255, 255, 0.2);
            border-radius: 10px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 24px;
            color: #ffc107;
        }

        .avg-label {
            font-size: 12px;
            text-transform: uppercase;
            letter-spacing: 1px;
            opacity: 0.8;
            display: block;
        }

        .avg-value {
            font-weight: 700;
            font-size: 24px;
            margin-top: 5px;
        }


        .custom-header-btn {
            position: absolute;
            top: 25px;
            right: 180px;
            z-index: 9999;
            font-weight: 600;
            font-size: 12px;
            padding: 6px 12px;
            border-radius: 4px;
            background-color: #0F406B !important;
            border-color: #0F406B !important;
            color: white !important;
        }
    </style>
    <asp:PlaceHolder ID="phAdminDashboard" runat="server" Visible="false">
        <a href="RequestForm.aspx" class="btn btn-primary shadow-sm custom-header-btn">
            <i class="fas fa-plus" style="margin-right: 4px;"></i>Create New Request
        </a>
        <div class="container-fluid mt-4">

            <asp:Panel ID="pnlSlaAlert" runat="server" Visible="false" CssClass="sla-blink">
                <i class="fas fa-exclamation-triangle me-2"></i>
                ATTENTION: THERE ARE APPLICATIONS WHOSE SLA PERIOD HAS EXPIRED AND HAVE NOT BEEN ASSIGNED!
            </asp:Panel>

            <div class="avg-time-card mb-4">
                <div class="row align-items-center">
                    <div class="col-auto">
                        <div class="avg-icon">
                            <i class="fas fa-bolt"></i>
                        </div>
                    </div>
                    <div class="col">
                        <span class="avg-label">AVERAGE RESOLUTION TIME</span>
                        <h4 class="avg-value mb-0">
                            <asp:Literal ID="litAvgTime" runat="server">Calculating...</asp:Literal></h4>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-lg-6">
                    <div class="card dashboard-card">
                        <div class="card-header-custom">
                            <span><i class="fas fa-list-ul me-2"></i>REQUESTS</span>
                            <small class="badge bg-light text-dark">
                                <asp:Literal ID="litOpenCount" runat="server">0</asp:Literal></small>
                        </div>
                        <div class="card-body p-0">
                            <div class="table-responsive">
                                <table class="table table-hover table-custom mb-0">
                                    <thead>
                                        <tr>
                                            <th>ID</th>
                                            <th>Topic</th>
                                            <th>User</th>
                                            <th>Date</th>
                                            <th>Status</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="rptOpenRequests" runat="server">
                                            <ItemTemplate>
                                                <tr class='<%# GetRowClass(Eval("IsConfirmed"), 0) %>'
                                                    onclick="window.location='AdminForm.aspx?id=<%# Eval("RequestId") %>';"
                                                    style="cursor: pointer;">
                                                    <td>#<%# Eval("RequestId") %></td>
                                                    <td><strong><%# Eval("Topic") %></strong></td>
                                                    <td><%# Server.HtmlEncode(Convert.ToString(Eval("RequesterName"))) %></td>
                                                    <td><%# Eval("RequestDate", "{0:dd.MM.yyyy HH:mm}") %></td>
                                                    <td><%# GetStatusBadge(Eval("IsConfirmed")) %></td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="col-lg-6">
                    <div class="card dashboard-card">
                        <div class="card-header-custom" style="background-color: #2c3e50;">
                            <span><i class="fas fa-user-check me-2"></i>REQUESTS ASSIGNED TO ME</span>
                            <small class="badge bg-warning text-dark">
                                <asp:Literal ID="litMyCount" runat="server">0</asp:Literal></small>
                        </div>
                        <div class="card-body p-0">
                            <div class="table-responsive">
                                <table class="table table-hover table-custom mb-0">
                                    <thead>
                                        <tr>
                                            <th>ID</th>
                                            <th>Topic</th>
                                            <th>Date</th>
                                            <th>Priorty</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="rptMyRequests" runat="server">
                                            <ItemTemplate>
                                                <tr onclick="window.location='AdminForm.aspx?id=<%# Eval("RequestId") %>';" style="cursor: pointer;" onmouseover="this.style.backgroundColor='#f8f9fa';" onmouseout="this.style.backgroundColor='transparent';">
                                                    <td>#<%# Eval("RequestId") %></td>
                                                    <td><strong><%# Eval("Topic") %></strong></td>
                                                    <td><%# Eval("RequestDate", "{0:dd.MM.yyyy}") %></td>
                                                    <td><%# GetPriorityBadges(Eval("IsImportant"), Eval("IsUrgent")) %></td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <script>
            setInterval(function () { location.reload(); }, 30000);


        </script>

    </asp:PlaceHolder>
</asp:Content>
