<%@ Page Title="Primary Device - Additional Device Connection Report" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AddDeviceDeviceReport.aspx.cs" Inherits="ITSM.AddDeviceDeviceReport" %>

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
                <h3 class="card-title mb-0" style="font-size: 16px;"><i class="fal fa-link me-2"></i>Active Primary Device - Additional Device Connection List</h3>
            </div>

            <div class="card-body">

                <div class="d-flex justify-content-between align-items-center mb-3">
                    <div style="font-size: 13px; font-weight: 600; color: #555;">
                        Show 
                        <asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged" CssClass="form-select form-select-sm d-inline-block mx-1" Style="width: auto;">
                            <asp:ListItem Text="5" Value="5"></asp:ListItem>
                            <asp:ListItem Text="10" Value="10"></asp:ListItem>
                            <asp:ListItem Text="25" Value="25" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="50" Value="50"></asp:ListItem>
                            <asp:ListItem Text="100" Value="100"></asp:ListItem>
                        </asp:DropDownList>
                        entries
                    </div>
                </div>

                <div class="table-responsive">
                    <table class="table table-sm table-bordered table-hover align-middle" width="100%" cellspacing="0">
                        <thead class="table-light text-nowrap">
                            <tr>
                                <th>Primary Device (Brand / Model - Serial No)</th>
                                <th>Additional Device (Type - Description)</th>
                                <th>Connection Date</th>
                            </tr>
                            <tr class="arama">
                                <td>
                                    <asp:TextBox ID="txtSearchPrimary" runat="server" CssClass="form-control form-control-sm frmara" placeholder="Search Primary Device..."></asp:TextBox>
                                </td>
                                <td>
                                    <asp:TextBox ID="txtSearchSecondary" runat="server" CssClass="form-control form-control-sm frmara" placeholder="Search Additional Device..."></asp:TextBox>
                                </td>
                                <td>
                                    <div class="input-group input-group-sm">
                                        <asp:TextBox ID="txtSearchDate" runat="server" CssClass="form-control form-control-sm frmara" placeholder="Search Date..."></asp:TextBox>
                                        <asp:LinkButton ID="btnSearch" runat="server" CssClass="btn btn-primary btn-sm gridsearchbtn" OnClick="btnSearch_Click"><i class="fas fa-search"></i></asp:LinkButton>
                                        <asp:LinkButton ID="btnClear" runat="server" CssClass="btn btn-warning btn-sm gridsearchbtn" OnClick="btnClear_Click"><i class="fas fa-times"></i></asp:LinkButton>
                                    </div>
                                </td>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptRelations" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td><%# Eval("PrimaryDeviceName") %></td>
                                        <td><%# Eval("SecondaryDeviceName") %></td>
                                        <td><%# Eval("RecordDateText") %></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <tr id="trNoData" runat="server" visible="false">
                                <td colspan="3" class="text-center text-danger fw-bold py-4">
                                    <i class="fal fa-exclamation-triangle fa-2x mb-2 d-block"></i>
                                    No active connections found.
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>

                <div class="d-flex justify-content-between align-items-center mt-3" id="divPagination" runat="server">
                    <div class="text-muted" style="font-size: 13px;">
                        <asp:Label ID="lblPageInfo" runat="server"></asp:Label>
                    </div>
                    <div>
                        <ul class="pagination pagination-sm mb-0">
                            <li class="page-item" id="liPrev" runat="server">
                                <asp:LinkButton ID="lbPrev" runat="server" CssClass="page-link" OnClick="lbPrev_Click">&laquo; Previous</asp:LinkButton>
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
