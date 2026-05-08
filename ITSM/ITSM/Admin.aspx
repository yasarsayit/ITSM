<%@ Page Title="Admin Panel - Request Management" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Admin.aspx.cs" Inherits="ITSM.Admin" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        select[id*="ddlFilterStatus"] option[value="ONGOING"] {
            color: #198754 !important;
            font-weight: bold;
        }

        select[id*="ddlFilterStatus"] option[value="ENDED"] {
            color: #0d6efd !important;
            font-weight: bold;
        }

        .panel-table th {
            border-bottom: 2px solid #0F406B !important;
            background-color: #f8f9fa;
        }

        .panel-table td {
            vertical-align: middle;
        }

        .no-hover tbody tr:hover {
            background-color: transparent !important;
            color: inherit !important;
        }

        .badge {
            font-weight: 600;
            font-size: 11px;
            letter-spacing: 0.3px;
            padding: 4px 8px;
            opacity: 0.8;
        }

        .bg-light.text-muted.border {
            background-color: #e9ecef !important;
            color: #adb5bd !important;
            border-color: #dee2e6 !important;
            opacity: 0.5;
        }

        .table-responsive::-webkit-scrollbar {
            width: 8px;
            height: 8px;
        }

        .table-responsive::-webkit-scrollbar-track {
            background-color: #ffffff;
        }

        .table-responsive::-webkit-scrollbar-thumb {
            background-color: lightgray;
            border-radius: 4px;
        }

            .table-responsive::-webkit-scrollbar-thumb:hover {
                background-color: #b0b0b0;
            }

        .table-responsive {
            scrollbar-width: thin;
            scrollbar-color: lightgray #ffffff;
        }

        /* Arama ve Sayfalama Stilleri */
        .pagination .page-item.active .page-link {
            background-color: #0F406B;
            border-color: #0F406B;
            color: white;
        }

        .pagination .page-link {
            color: #0F406B;
            cursor: pointer;
        }

        .arama td {
            padding: 4px 5px !important;
            background-color: #f1f5f9 !important;
        }

        .frmara {
            font-size: 11px;
        }

        .btn.btn-outline-primary.btn-sm.bg-white.w-100 {
            color: #0F406B !important;
            border-color: #0F406B !important;
            background-color: #ffffff !important;
            transition: all 0.2s ease;
        }

            .btn.btn-outline-primary.btn-sm.bg-white.w-100:hover {
                background-color: #C7DFFB !important;
                color: #0F406B !important;
                border-color: #0F406B !important;
            }

            .btn.btn-outline-primary.btn-sm.bg-white.w-100:active,
            .btn.btn-outline-primary.btn-sm.bg-white.w-100:focus {
                background-color: #C7DFFB !important;
                color: #0F406B !important;
                border-color: #0F406B !important;
                box-shadow: none !important;
                transform: none !important;
            }

            .btn.btn-outline-primary.btn-sm.bg-white.w-100 i {
                color: #0F406B !important;
            }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:UpdatePanel ID="upAdminPanel" runat="server">
        <ContentTemplate>
            <div class="container-fluid" style="padding: 20px;">
                <div class="row">
                    <div class="col-lg-12">

                        <div class="card shadow-sm border mt-2">
                            <div class="card-header d-flex justify-content-between align-items-center" style="background-color: #0F406B; color: white; padding: 10px 15px;">
                                <h5 class="card-title mb-0" style="font-size: 14px; font-weight: 600;"><i class="fal fa-list-alt mr-2"></i>Pending Requests</h5>
                            </div>

                            <div class="card-body p-3">
                                <div class="d-flex justify-content-between align-items-center mb-3">
                                    <div style="font-size: 13px; font-weight: 600; color: #555;">
                                        Show 
                                        <asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged" CssClass="form-select form-select-sm d-inline-block mx-1" Style="width: auto;">
                                            <asp:ListItem Text="5" Value="5"></asp:ListItem>
                                            <asp:ListItem Text="10" Value="10" Selected="True"></asp:ListItem>
                                            <asp:ListItem Text="25" Value="25"></asp:ListItem>
                                            <asp:ListItem Text="50" Value="50"></asp:ListItem>
                                            <asp:ListItem Text="100" Value="100"></asp:ListItem>
                                        </asp:DropDownList>
                                        entries
                                    </div>
                                </div>

                                <div class="table-responsive">
                                    <table class="table panel-table table-hover table-striped mb-0" style="font-size: 13px;">
                                        <thead class="text-nowrap align-middle">
                                            <tr>
                                                <th style="width: 80px; text-align: center;">Req. ID</th>
                                                <th>Topic</th>
                                                <th>Type & Subtype</th>
                                                <th>Request Date</th>
                                                <th>Owner / Created By</th>
                                                <th>Due Date</th>
                                                <th>Priority</th>
                                                <th style="width: 100px; text-align: center;">Status</th>
                                                <th style="width: 110px; text-align: center;">Assigned Score</th>
                                                <th style="width: 120px; text-align: center;">Actions</th>
                                            </tr>
                                            <tr class="arama">
                                                <td>
                                                    <asp:TextBox ID="txtFilterReqID" runat="server" CssClass="form-control form-control-sm frmara" placeholder="Search ID..."></asp:TextBox></td>
                                                <td>
                                                    <asp:TextBox ID="txtFilterTopic" runat="server" CssClass="form-control form-control-sm frmara" placeholder="Search Topic..."></asp:TextBox></td>
                                                <td>
                                                    <asp:DropDownList ID="ddlFilterType" runat="server" CssClass="form-select form-select-sm frmara"></asp:DropDownList></td>
                                                <td>
                                                    <asp:TextBox ID="txtFilterReqDate" runat="server" CssClass="form-control form-control-sm frmara" TextMode="Date"></asp:TextBox></td>
                                                <td>
                                                    <asp:TextBox ID="txtFilterUser" runat="server" CssClass="form-control form-control-sm frmara" placeholder="Search User..."></asp:TextBox></td>
                                                <td>
                                                    <asp:TextBox ID="txtFilterDueDate" runat="server" CssClass="form-control form-control-sm frmara" TextMode="Date"></asp:TextBox></td>
                                                <td>
                                                    <asp:DropDownList ID="ddlFilterPriority" runat="server" CssClass="form-select form-select-sm frmara">
                                                        <asp:ListItem Text="All" Value=""></asp:ListItem>
                                                        <asp:ListItem Text="Urgent" Value="Urgent"></asp:ListItem>
                                                        <asp:ListItem Text="Important" Value="Important"></asp:ListItem>
                                                        <asp:ListItem Text="Standard" Value="Standard"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                                <td>

                                                    <asp:DropDownList ID="ddlFilterStatus" runat="server" CssClass="form-select form-select-sm frmara">
                                                        <asp:ListItem Text="All" Value=""></asp:ListItem>
                                                        <asp:ListItem Text="OnGoing" Value="ONGOING"></asp:ListItem>
                                                        <asp:ListItem Text="Ended" Value="ENDED"></asp:ListItem>
                                                        <asp:ListItem Text="Pending" Value="0"></asp:ListItem>
                                                        <asp:ListItem Text="Approved" Value="1"></asp:ListItem>
                                                        <asp:ListItem Text="Rejected" Value="2"></asp:ListItem>
                                                        <asp:ListItem Text="Completed" Value="3"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </td>
                                                <td></td>
                                                <td style="text-align: center;">
                                                    <div class="btn-group btn-group-sm">
                                                        <asp:LinkButton ID="lbAraTemizle" runat="server" CssClass="btn btn-warning btn-sm" OnClick="lbAraTemizle_Click" CausesValidation="false" title="Clear Filters"><i class="fas fa-broom"></i></asp:LinkButton>
                                                        <asp:LinkButton ID="lbAra" runat="server" CssClass="btn btn-primary btn-sm" OnClick="lbAra_Click" CausesValidation="false" title="Search"><i class="fas fa-search"></i></asp:LinkButton>
                                                    </div>
                                                </td>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <asp:Repeater ID="rptAdminRequests" runat="server" OnItemCommand="rptAdminRequests_ItemCommand">
                                                <ItemTemplate>
                                                    <tr class='<%# GetRowColorClass(Eval("IsConfirmed"), Eval("IsSlaExceeded")) %>'>
                                                        <td class="fw-bold text-center"><%# Eval("RequestId") %></td>
                                                        <td class='<%# GetSlaTextColor(Eval("IsConfirmed"), Eval("IsSlaExceeded")) %>' style="font-weight: 600;">
                                                            <%# Eval("Topic") %> 
                                                        </td>
                                                        <td>
                                                            <div style="font-weight: 600; color: #495057;">
                                                                <%# Eval("TypeName") != DBNull.Value ? Eval("TypeName") : "<span class='badge bg-secondary' style='font-size:10px;'>NOT SET</span>" %>
                                                            </div>
                                                            <div style="font-size: 11px; color: #6c757d; margin-top: 2px;">
                                                                <%# Eval("SubTypeName") != DBNull.Value ? Eval("SubTypeName") : "" %>
                                                            </div>
                                                        </td>
                                                        <td class='<%# GetSlaTextColor(Eval("IsConfirmed"), Eval("IsSlaExceeded")) %>'>
                                                            <%# Eval("RequestDate", "{0:dd.MM.yyyy HH:mm}") %> 
                                                        </td>
                                                        <td>
                                                            <div style="font-weight: 600; color: #495057;">
                                                                <%# Eval("RequestOwner") != DBNull.Value ? Eval("RequestOwner") : "<span class='text-muted fst-italic'>System</span>" %>
                                                            </div>
                                                            <div style="font-size: 11px; color: #6c757d; margin-top: 2px;" title="Created By">
                                                                <i class="fal fa-user-edit"></i><%# Eval("CreatedBy") %>
                                                            </div>
                                                        </td>
                                                        <td>
                                                            <%# Eval("DueDate") == DBNull.Value ? "<span class='text-muted fst-italic'>Not Set</span>" : Eval("DueDate", "{0:dd.MM.yyyy}") %> 
                                                        </td>

                                                        <td><%# GetPriorityBadges(Eval("IsImportant"), Eval("IsUrgent")) %> </td>
                                                        <td class="text-center">
                                                            <%# GetStatusBadge(Eval("IsConfirmed")) %> 
                                                        </td>
                                                        <td class="text-center">
                                                            <%# GetScoreBadge(Eval("Score"), Eval("IsConfirmed")) %> 
                                                        </td>
                                                        <td class="text-center">
                                                            <asp:LinkButton ID="btnReview" runat="server" CommandName="Review" CommandArgument='<%# Eval("RequestId") %>'
                                                                CssClass="btn btn-sm btn-outline-primary bg-white w-100" Style="font-size: 11px; font-weight: 600;">
                                                            REVIEW <i class="fal fa-arrow-right ms-1"></i> 
                                                            </asp:LinkButton>
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                            <tr id="trNoData" runat="server" visible="false">
                                                <td colspan="11" class="text-center text-muted p-4"><i class="fal fa-search fa-2x mb-2"></i>
                                                    <br />
                                                    No requests found matching your criteria!
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
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
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
