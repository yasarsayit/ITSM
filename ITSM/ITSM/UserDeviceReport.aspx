<%@ Page Title="User-Device Assignments Report" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="UserDeviceReport.aspx.cs" Inherits="ITSM.UserDeviceReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .pagination .page-item.active .page-link {
            background-color: #0F406B;
            border-color: #0F406B;
            color: white;
        }

        .pagination .page-link {
            color: #0F406B;
        }

        .arama td {
            padding: 4px 5px !important;
            background-color: #f1f5f9 !important;
        }

        .frmara {
            font-size: 11px;
        }
    </style>

    <div class="container-fluid" style="padding: 20px;">
        <div class="card shadow mb-4">
            <div class="card-header d-flex justify-content-between align-items-center" style="background-color: #0F406B; color: white;">
                <h3 class="card-title mb-0" style="font-size: 16px;"><i class="fal fa-user-tag me-2"></i>Active Assigned Device List</h3>
            </div>

            <div class="card-body">
                <div class="d-flex justify-content-between align-items-center mb-3">
                    <div style="font-size: 13px; font-weight: 600; color: #555;">
                        Show 
                        <asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged" CssClass="form-select form-select-sm d-inline-block mx-1" Style="width: auto;">
                            <asp:ListItem Text="5" Value="5"></asp:ListItem>
                            <asp:ListItem Text="10" Value="10" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="25" Value="25"></asp:ListItem>
                            <asp:ListItem Text="50" Value="50"></asp:ListItem>
                        </asp:DropDownList>
                        entries
                    </div>
                </div>

                <div class="table-responsive">
                    <asp:Repeater ID="rptAssignedDevices" runat="server">
                        <HeaderTemplate>
                            <table class="table table-sm table-bordered table-hover align-middle">
                                <thead class="table-light text-nowrap">
                                    <tr>
                                        <th>Personnel</th>
                                        <th>Device Information</th>
                                        <th>Start Date</th>
                                        <th style="width: 100px; text-align: center;">Actions</th>
                                    </tr>
                                    <tr class="arama">
                                        <td>
                                            <asp:TextBox ID="txtFUser" runat="server" CssClass="form-control form-control-sm frmara" placeholder="Search Personnel..."></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtFDevice" runat="server" CssClass="form-control form-control-sm frmara" placeholder="Search Device..."></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txtFDate" runat="server" CssClass="form-control form-control-sm frmara" placeholder="Search Date..."></asp:TextBox>
                                        </td>
                                        <td style="text-align: center;">
                                            <div class="btn-group btn-group-sm gridsearchgrp">
                                                <asp:LinkButton ID="lbAraTemizle" runat="server" CssClass="btn btn-warning btn-sm gridsearchbtn" OnClick="lbAraTemizle_Click" title="Clear Filters"><i class="fas fa-broom"></i></asp:LinkButton>
                                                <asp:LinkButton ID="lbAra" runat="server" CssClass="btn btn-primary btn-sm gridsearchbtn" OnClick="lbAra_Click" title="Search"><i class="fas fa-search"></i></asp:LinkButton>
                                            </div>
                                        </td>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("NameSurname") %></td>
                                <td><%# Eval("Brand") %> <%# Eval("Model") %> (<%# Eval("SerialNumber") %>)</td>
                                <td><%# Eval("StartDate", "{0:dd.MM.yyyy}") %></td>
                                <td style="text-align: center;"></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>

                <div class="d-flex justify-content-between align-items-center mt-3">
                    <div class="text-muted" style="font-size: 13px;">
                        <asp:Label ID="lblPageInfo" runat="server"></asp:Label>
                    </div>
                    <div>
                        <ul class="pagination pagination-sm mb-0">
                            <li class="page-item" id="liPrev" runat="server">
                                <asp:LinkButton ID="lbPrev" runat="server" CssClass="page-link" OnClick="lbPrev_Click">&laquo; Prev</asp:LinkButton>
                            </li>
                            <asp:Repeater ID="rptPagination" runat="server" OnItemCommand="rptPagination_ItemCommand">
                                <ItemTemplate>
                                    <li class='page-item <%# Convert.ToBoolean(Eval("IsActive")) ? "active" : "" %>'>
                                        <asp:LinkButton ID="lbPage" runat="server" CssClass="page-link" CommandName="Page" CommandArgument='<%# Eval("PageNumber") %>'><%# Eval("PageNumber") %></asp:LinkButton>
                                    </li>
                                </ItemTemplate>
                            </asp:Repeater>
                            <li class="page-item" id="liNext" runat="server">
                                <asp:LinkButton ID="lbNext" runat="server" CssClass="page-link" OnClick="lbNext_Click">Next &raquo;</asp:LinkButton>
                            </li>
                        </ul>
                    </div>
                </div>

            </div>
        </div>
    </div>
</asp:Content>
