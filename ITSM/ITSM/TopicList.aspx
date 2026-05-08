<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TopicList.aspx.cs" Inherits="ITSM.TopicList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .custom-container {
            max-width: 1200px;
            margin: 0 auto;
            padding: 20px;
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

        .badge-visible {
            background-color: #d4edda;
            color: #155724;
            padding: 4px 8px;
            border-radius: 4px;
            font-size: 11px;
            font-weight: 600;
        }

        .badge-hidden {
            background-color: #f8d7da;
            color: #721c24;
            padding: 4px 8px;
            border-radius: 4px;
            font-size: 11px;
            font-weight: 600;
        }

        .arama td {
            padding: 4px 5px !important;
            background-color: #f1f5f9 !important;
        }

        .frmara {
            font-size: 11px;
        }

        .pagination .page-item.active .page-link {
            background-color: #0F406B;
            border-color: #0F406B;
            color: white;
        }

        .pagination .page-link {
            color: #0F406B;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid" style="padding: 20px;">
        <div class="card shadow-sm border">
            <div class="card-header" style="background-color: #0F406B; color: white; padding: 12px 15px;">
                <h5 class="card-title mb-0" style="font-size: 14px; font-weight: 600;">
                    <i class="fal fa-list mr-2"></i><%= gettext("savedtopics"," Saved Topics")%>
                </h5>
            </div>

            <div class="card-body p-0">
                <div class="custom-container">

                    <div class="d-flex justify-content-between align-items-center mb-3">
                        <div style="font-size: 13px; font-weight: 600; color: #555;">
                            <%= gettext("topicshow","Show")%>
                            <asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged" CssClass="form-select form-select-sm d-inline-block mx-1" Style="width: auto;">
                                <asp:ListItem Text="10" Value="10" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="25" Value="25"></asp:ListItem>
                                <asp:ListItem Text="50" Value="50"></asp:ListItem>
                            </asp:DropDownList>
                            <%= gettext("Entries","entries")%>
                        </div>
                    </div>

                    <asp:Panel ID="pnlNoData" runat="server" Visible="false" CssClass="alert alert-warning text-center">
                        <i class="fa fa-exclamation-circle fa-lg"></i>
                        <strong><%= gettext("note"," Note: ")%></strong> <%= gettext("no_data_found","There are no saved topics to display.")%>
                    </asp:Panel>

                    <asp:Panel ID="pnlTable" runat="server">
                        <asp:Repeater ID="rptTopicList" runat="server">
                            <HeaderTemplate>
                                <table class="table panel-table no-hover table-striped table-bordered mb-0" style="font-size: 13px;">
                                    <thead class="text-nowrap">
                                        <tr>
                                            <th style="width: 80px; text-align: center;"><%= gettext("topicId","ID")%></th>
                                            <th><%= gettext("topictitle","Topic Title")%></th>

                                            <th style="width: 100px; text-align: center;"><%= gettext("tstatus","Status")%></th>
                                            <th style="width: 150px;"><%= gettext("trecdate","Record Date")%></th>
                                            <th style="width: 80px; text-align: center;"><%= gettext("topicedit","Edit")%></th>
                                        </tr>
                                        <tr class="arama">
                                            <td>
                                                <asp:TextBox ID="txtFID" runat="server" CssClass="form-control form-control-sm frmara" placeholder='<%# gettext("plcholderId", "ID...") %>'></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtFTitle" runat="server" CssClass="form-control form-control-sm frmara" placeholder='<%# gettext("searchtitle", "Search Title...") %>'></asp:TextBox></td>
                                            <td>
                                                <asp:DropDownList ID="ddlFStatus" runat="server" CssClass="form-control form-control-sm frmara">
                                                    <asp:ListItem Text="All" Value=""></asp:ListItem>
                                                    <asp:ListItem Text="Visible" Value="1"></asp:ListItem>
                                                    <asp:ListItem Text="Hidden" Value="0"></asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtFDate" runat="server" CssClass="form-control form-control-sm frmara" placeholder="01.01.2026" TextMode="Date"></asp:TextBox></td>
                                            <td style="text-align: center;">
                                                <div class="btn-group btn-group-sm">
                                                    <asp:LinkButton ID="lbClear" runat="server" CssClass="btn btn-warning" OnClick="lbClear_Click"><i class="fas fa-broom"></i></asp:LinkButton>
                                                    <asp:LinkButton ID="lbSearch" runat="server" CssClass="btn btn-primary" OnClick="lbSearch_Click"><i class="fas fa-search"></i></asp:LinkButton>
                                                </div>
                                            </td>
                                        </tr>
                                    </thead>
                                    <tbody>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td class="fw-bold text-center"><%# Eval("KnowledgeID") %></td>
                                    <td style="font-weight: 600; color: #0F406B;"><%# Eval("Title") %></td>

                                    <td class="text-center">
                                        <%# (Eval("IsVisible") != DBNull.Value && Convert.ToBoolean(Eval("IsVisible"))) 
                                        ? string.Format("<span class='badge-visible'>{0}</span>", gettext("status_visible", "Visible")) 
                                        : string.Format("<span class='badge-hidden'>{0}</span>", gettext("status_hidden", "Hidden")) %>
                                    </td>
                                    <td><%# Eval("RecordDate", "{0:dd.MM.yyyy HH:mm}") %></td>
                                    <td class="text-center">
                                        <div class="d-flex justify-content-center align-items-center" style="gap: 8px;">
                                            <a href='TopicView.aspx?id=<%# Eval("KnowledgeID") %>' class="btn btn-outline-success btn-sm d-inline-flex align-items-center justify-content-center" style="width: 32px; height: 32px;" title='<%# gettext("topicview_tip", "View Topic") %>'>
                                                <i class="fa fa-eye"></i>
                                            </a>
                                            <a href='Topic.aspx?id=<%# Eval("KnowledgeID") %>' class="btn btn-outline-primary btn-sm d-inline-flex align-items-center justify-content-center" style="width: 32px; height: 32px;" title='<%# gettext("topicedit_tip", "Edit Topic") %>'>
                                                <i class="fa fa-edit"></i>
                                            </a>
                                        </div>
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                </tbody>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>

                        <div class="d-flex justify-content-between align-items-center mt-3">
                            <div class="text-muted" style="font-size: 13px;">
                                <asp:Label ID="lblPageInfo" runat="server"></asp:Label>
                            </div>
                            <ul class="pagination pagination-sm mb-0">
                                <li class="page-item" id="liPrev" runat="server">
                                    <asp:LinkButton ID="lbPrev" runat="server" CssClass="page-link" OnClick="lbPrev_Click">&laquo;<%# gettext("topicprev","Prev")%> </asp:LinkButton>
                                </li>
                                <asp:Repeater ID="rptPagination" runat="server" OnItemCommand="rptPagination_ItemCommand">
                                    <ItemTemplate>
                                        <li class='page-item <%# Convert.ToBoolean(Eval("IsActive")) ? "active" : "" %>'>
                                            <asp:LinkButton ID="lbPage" runat="server" CssClass="page-link" CommandName="Page" CommandArgument='<%# Eval("PageNumber") %>'><%# Eval("PageNumber") %></asp:LinkButton>
                                        </li>
                                    </ItemTemplate>
                                </asp:Repeater>
                                <li class="page-item" id="liNext" runat="server">
                                    <asp:LinkButton ID="lbNext" runat="server" CssClass="page-link" OnClick="lbNext_Click"><%# gettext("topiclistnext","Next")%> &raquo;</asp:LinkButton>
                                </li>
                            </ul>
                        </div>
                    </asp:Panel>

                </div>
            </div>
        </div>
    </div>
</asp:Content>
