<%@ Page Title="DEVICE INFORMATION ENTRY FORM" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CategoryManagement.aspx.cs" Inherits="ITSM.CategoryManagement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .card-custom {
            border-radius: 8px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
            margin-top: 20px;
            border: none;
        }

        .header-blue {
            background-color: #0F406B;
            color: white;
            border-radius: 8px 8px 0 0;
            padding: 15px;
            font-weight: bold;
        }

        .btn-it {
            background-color: #0F406B !important;
            color: white !important;
            border: none;
        }

            .btn-it:hover {
                opacity: 0.8;
                color: white !important;
            }


        .container, .container.body-content {
            max-width: 96% !important;
        }


        .custom-input-group label {
            margin-bottom: 4px;
            color: #1a1a1a;
            white-space: nowrap;
            font-size: 0.85rem !important;
        }

        .custom-input-group {
            display: flex;
            width: 100%;
            gap: 10px;
            align-items: flex-end;
        }


            .custom-input-group label {
                margin-bottom: 4px;
                color: #1a1a1a;
            }

            .custom-input-group .form-control {
                flex: 1 1 auto;
                border-radius: 0.25rem !important;
                height: 38px;
            }

            .custom-input-group .ddl-main-wrapper {
                flex: 1 1 auto;
                min-width: 170px;
                max-width: 40%;
            }

            .custom-input-group .btn {
                border-radius: 0.25rem !important;
                flex: 0 0 auto;
                height: 38px;
                padding: 0 20px;
            }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="mainContent" runat="server">
    <div class="container-fluid px-2 py-4">
        <div class="d-flex justify-content-end mb-2">
            <a href="Admin.aspx" class="btn btn-it btn-sm"><i class="fas fa-undo"></i><%= gettext("ctgpanel","Back to Panel")%></a>
        </div>
        <div class="row">
            <div class="col-md-5">
                <div class="card card-custom">
                    <div class="header-blue"><%= gettext("ctgreq","Main Request Types")%></div>
                    <div class="card-body">
                        <div class="custom-input-group mb-3">
                            <div class="d-flex flex-column flex-grow-1">
                                <label class="font-weight-bold" style="font-size: 0.9rem;"><%= gettext("trktype","Turkish Type Name")%></label>
                                <asp:TextBox ID="txtNewType" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="d-flex flex-column flex-grow-1">
                                <label class="font-weight-bold" style="font-size: 0.9rem;"><%= gettext("engtype","English Type Name")%></label>
                                <asp:TextBox ID="txtNewTypeEN" runat="server" CssClass="form-control"></asp:TextBox>
                            </div>
                            <asp:Button ID="btnAddType" runat="server" Text='<%# gettext("btn_add", "Add") %>' CssClass="btn btn-it" OnClick="btnAddType_Click" />
                        </div>
                        <asp:GridView ID="gvTypes" runat="server" CssClass="table table-sm table-hover" AutoGenerateColumns="False" DataKeyNames="Id" OnSelectedIndexChanged="gvTypes_SelectedIndexChanged">
                            <Columns>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <%= gettext("header_type_desc", "Type List") %>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <strong><%# Eval("TypeName") %></strong> / <span><%# Eval("TypeNameEN") %></span>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnEditMain" runat="server" CommandName="Select" ForeColor="#0F406B">
                                            <i class="fas fa-edit"></i>
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>

            <div class="col-md-7">
                <div class="card card-custom">
                    <div class="header-blue"><%= gettext("subreqtypes","Sub Request Types")%> </div>
                    <div class="card-body">
                        <div class="custom-input-group mb-3">
                            <div class="d-flex flex-column ddl-main-wrapper">
                                <label class="font-weight-bold" style="font-size: 0.9rem;"><%= gettext("select_main_type_lbl", "Main Type")%></label>
                                <asp:DropDownList ID="ddlMainTypes" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlMainTypes_SelectedIndexChanged"></asp:DropDownList>
                            </div>

                            <div class="d-flex flex-column flex-grow-1">
                                <label class="font-weight-bold" style="font-size: 0.9rem;"><%= gettext("trksubtype", "Turkish Subtype Name")%></label>
                                <asp:TextBox ID="txtNewSubType" runat="server" CssClass="form-control txt-sub"></asp:TextBox>
                            </div>

                            <div class="d-flex flex-column flex-grow-1">
                                <label class="font-weight-bold" style="font-size: 0.9rem;"><%= gettext("engsubtype", "English Subtype Name")%></label>
                                <asp:TextBox ID="txtNewSubTypeEN" runat="server" CssClass="form-control txt-sub"></asp:TextBox>
                            </div>
                            <asp:Button ID="btnAddSubType" runat="server" Text='<%#gettext("btn_add", "Add")%>' CssClass="btn btn-it btn-action" OnClick="btnAddSubType_Click" />
                        </div>
                        <asp:GridView ID="gvSubTypes" runat="server" CssClass="table table-sm table-hover" AutoGenerateColumns="False" DataKeyNames="Id" OnSelectedIndexChanged="gvSubTypes_SelectedIndexChanged">
                            <Columns>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <%= gettext("header_subtype_desc", "Sub-Type List") %>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <strong><%# Eval("SubTypeName") %></strong> / <span><%# Eval("SubTypeNameEN") %></span>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-Width="40px" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnEditSub" runat="server" CommandName="Select" ForeColor="#0F406B">
                                            <i class="fas fa-edit"></i>
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        function showAlert(message, type) {
            Swal.fire({
                icon: type,
                title: message,
                confirmButtonText: 'OK'
            });
        }
    </script>
</asp:Content>
