<%@ Page Title="Add/Edit Additional Device" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AddDeviceForm.aspx.cs" Inherits="ITSM.AddDeviceForm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .form-label {
            font-weight: 600;
            color: #333;
            text-transform: uppercase;
            font-size: 0.85rem;
            margin-bottom: 0.5rem;
            display: block;
        }

        .section-title {
            border-bottom: 2px solid #0F406B;
            margin: 25px 0 15px 0;
            padding-bottom: 5px;
            color: #0F406B;
            font-size: 0.95rem;
            font-weight: bold;
        }

        .main-container {
            padding: 30px;
            background: #fff;
            border-radius: 8px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.1);
            margin-bottom: 50px;
        }

        .footer-buttons {
            border-top: 1px solid #eee;
            padding-top: 20px;
            margin-top: 20px;
            display: flex;
            justify-content: flex-end;
            gap: 10px;
        }
    </style>
</asp:Content>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:HiddenField ID="hfID" runat="server" />
    <asp:HiddenField ID="hfAlertType" runat="server" />
    <asp:HiddenField ID="hfAlertMessage" runat="server" />
    <asp:HiddenField ID="hfAlertTitle" runat="server" />
    <asp:HiddenField ID="hfRedirectUrl" runat="server" />

    <div class="main-container">
        <h2 class="subheader-title">
            <asp:Literal ID="lblPageTitle" runat="server" Text="Add New Additional Device"></asp:Literal></h2>


        <div class="row">
            <div class="col-md-6 mb-3">
                <label class="form-label">Device Type (Monitor, Mouse etc.)</label>
                <asp:TextBox ID="txtDeviceType" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
            <div class="row">
                <div class="col-md-12 mb-3">
                    <label class="form-label">Description / Serial Number</label>
                    <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2"></asp:TextBox>
                </div>
            </div>
            <%--<div class="col-md-6 mb-3">
                <label class="form-label">Assignment (Assign to Main Device/s)</label>
                <div class="input-group">
                    <asp:DropDownList ID="ddlMainDevices" runat="server" CssClass="form-select"></asp:DropDownList>
                    <asp:LinkButton ID="btnAddDevice" runat="server" CssClass="btn btn-outline-primary" OnClick="btnAddDevice_Click">
            <i class="fal fa-plus"></i> Add
                    </asp:LinkButton>
                </div>
                <div class="mt-2 d-flex flex-wrap gap-2">
                    <asp:Repeater ID="rptSelectedDevices" runat="server" OnItemCommand="rptSelectedDevices_ItemCommand">
                        <ItemTemplate>
                            <span class="badge bg-light text-dark border d-flex align-items-center p-2">
                                <%# Eval("DeviceInfo") %>
                                <asp:LinkButton ID="btnRemove" runat="server" CommandName="Remove" CommandArgument='<%# Eval("DeviceID") %>'
                                    CssClass="ms-2 text-danger" Style="text-decoration: none;">
                        <i class="fal fa-times"></i>
                                </asp:LinkButton>
                            </span>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>--%>
        </div>



        <div class="section-title">FINANCIAL & STATUS</div>
        <div class="row">
            <div class="col-md-3 mb-3">
                <label class="form-label">Purchase Date</label>
                <asp:TextBox ID="txtPurchaseDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
            </div>
            <div class="col-md-3 mb-3">
                <label class="form-label">Warranty End Date</label>
                <asp:TextBox ID="txtWarrantyEnd" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
            </div>
            <div class="col-md-3 mb-3">
                <label class="form-label">Scrap Date</label>
                <asp:TextBox ID="txtScrapDate" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
            </div>
            <div class="col-md-3 mb-3">
                <label class="form-label">Scrap Description (Reason)</label>
                <asp:TextBox ID="txtScrapDescription" runat="server" CssClass="form-control" placeholder="Scrap, Gift, Donation..."></asp:TextBox>
            </div>
        </div>

        <div class="row">
            <div class="col-md-4 mb-3">
                <label class="form-label">Purchase Number</label>
                <asp:TextBox ID="txtPurchaseNumber" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
            <div class="col-md-4 mb-3">
                <label class="form-label">Accounting Code</label>
                <asp:TextBox ID="txtAccountingCode" runat="server" CssClass="form-control"></asp:TextBox>
            </div>
            <div class="col-md-4 mb-3">
                <label class="form-label">Status</label>
                <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-select">
                    <asp:ListItem Value="1">Active</asp:ListItem>
                    <asp:ListItem Value="0">Passive</asp:ListItem>
                </asp:DropDownList>
            </div>
        </div>



        <div class="section-title">ASSIGN TO MAIN DEVİCE</div>
        <div class="col-md-6 mb-3">
            <label class="form-label">Assignment (Assign to Main Device/s)</label>
            <div class="input-group">
                <asp:DropDownList ID="ddlMainDevices" runat="server" CssClass="form-select"></asp:DropDownList>
                <asp:LinkButton ID="btnAddDevice" runat="server" CssClass="btn btn-outline-primary" OnClick="btnAddDevice_Click">
<i class="fal fa-plus"></i> Add
                </asp:LinkButton>
            </div>
            <div class="mt-2 d-flex flex-wrap gap-2">
                <asp:Repeater ID="rptSelectedDevices" runat="server" OnItemCommand="rptSelectedDevices_ItemCommand">
                    <ItemTemplate>
                        <span class="badge bg-light text-dark border d-flex align-items-center p-2">
                            <%# Eval("DeviceInfo") %>
                            <asp:LinkButton ID="btnRemove" runat="server" CommandName="Remove" CommandArgument='<%# Eval("DeviceID") %>'
                                CssClass="ms-2 text-danger" Style="text-decoration: none;">
            <i class="fal fa-times"></i>
                            </asp:LinkButton>
                        </span>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>

        <div class="footer-buttons">
            <asp:LinkButton ID="btnCancel" runat="server" CssClass="btn btn-secondary px-5" OnClick="btnCancel_Click">CANCEL</asp:LinkButton>
            <asp:LinkButton ID="btnSave" runat="server" CssClass="btn btn-primary px-5" OnClick="btnSave_Click" Style="background-color: #0F406B; border-color: #0F406B;">
                <i class="fal fa-save mr-1"></i> SAVE RECORD
            </asp:LinkButton>
        </div>
    </div>

    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <script type="text/javascript">
        window.onload = function () {
            var type = document.getElementById('<%= hfAlertType.ClientID %>').value;
            var msg = document.getElementById('<%= hfAlertMessage.ClientID %>').value;
            var title = document.getElementById('<%= hfAlertTitle.ClientID %>').value;
            var url = document.getElementById('<%= hfRedirectUrl.ClientID %>').value;

            if (type && msg) {
                Swal.fire({ icon: type, title: title, text: msg }).then(() => {
                    if (url && type === 'success') window.location.href = url;
                });
            }
        };
    </script>
</asp:Content>
