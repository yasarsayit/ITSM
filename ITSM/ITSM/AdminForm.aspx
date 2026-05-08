<%@ Page Title="Admin Request Details" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AdminForm.aspx.cs" Inherits="ITSM.AdminForm" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .info-label {
            font-size: 11px;
            font-weight: 700;
            color: #6c757d;
            text-transform: uppercase;
            margin-bottom: 5px;
            display: block;
        }

        .edit-box {
            background-color: #f8f9fa;
            border: 1px solid #ced4da;
            padding: 15px;
            border-radius: 6px;
            margin-bottom: 20px;
        }

        .panel-table th {
            border-bottom: 2px solid #0F406B !important;
            background-color: #f8f9fa;
        }

        .panel-table td {
            vertical-align: middle;
        }

        .btn-send-custom {
            background-color: #0F406B !important;
            color: white !important;
            border: none;
            padding: 10px 25px;
            border-radius: 8px;
            font-weight: 600;
            font-size: 14px;
            transition: all 0.3s ease;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            text-transform: uppercase;
            letter-spacing: 0.5px;
            box-shadow: 0 4px 6px rgba(15, 64, 107, 0.2);
        }

            .btn-send-custom:hover:not(.disabled) {
                background-color: #083258 !important;
                transform: translateY(-2px);
                box-shadow: 0 6px 12px rgba(8, 50, 88, 0.3);
            }

        .timeline-section {
            --color: #333;
            --bgColor: #f8f9fa;
            --accent-color: #0F406B;
            font-family: "Poppins", sans-serif;
            padding: 1rem;
        }

        .timeline-ul {
            --col-gap: 2rem;
            --row-gap: 2rem;
            --line-w: 0.25rem;
            display: grid;
            grid-template-columns: var(--line-w) 1fr;
            grid-auto-columns: max-content;
            column-gap: var(--col-gap);
            list-style: none;
            width: 100%;
            margin-inline: auto;
            position: relative;
            padding-left: 0;
        }

            .timeline-ul::before {
                content: "";
                grid-column: 1;
                grid-row: 1 / span 500;
                background: #dee2e6;
                border-radius: calc(var(--line-w) / 2);
            }

            .timeline-ul li {
                grid-column: 2;
                --inlineP: 1.5rem;
                margin-inline: var(--inlineP);
                grid-row: span 2;
                display: grid;
                grid-template-rows: min-content min-content min-content;
                margin-bottom: var(--row-gap);
            }

                .timeline-ul li .sender-name-box {
                    --dateH: 2.5rem;
                    height: var(--dateH);
                    margin-inline: calc(var(--inlineP) * -1);
                    text-align: center;
                    background-color: var(--accent-color);
                    color: white;
                    font-size: 0.9rem;
                    font-weight: 700;
                    display: grid;
                    place-content: center;
                    position: relative;
                    border-radius: calc(var(--dateH) / 2) 0 0 calc(var(--dateH) / 2);
                    text-transform: uppercase;
                }

                    .timeline-ul li .sender-name-box::before {
                        content: "";
                        width: var(--inlineP);
                        aspect-ratio: 1;
                        background: var(--accent-color);
                        background-image: linear-gradient(rgba(0, 0, 0, 0.2) 100%, transparent);
                        position: absolute;
                        top: 100%;
                        clip-path: polygon(0 0, 100% 0, 0 100%);
                        right: 0;
                    }

                    .timeline-ul li .sender-name-box::after {
                        content: "";
                        position: absolute;
                        width: 1.5rem;
                        aspect-ratio: 1;
                        background: #fff;
                        border: 0.3rem solid var(--accent-color);
                        border-radius: 50%;
                        top: 50%;
                        transform: translate(50%, -50%);
                        right: calc(100% + var(--col-gap) + var(--line-w) / 2);
                    }

                .timeline-ul li .msg-content-box {
                    background: #e7f1ff;
                    position: relative;
                    padding: 1.2rem;
                    border-radius: 0 0 8px 8px;
                    box-shadow: 0 4px 6px rgba(0,0,0,0.05);
                }

                .timeline-ul li .msg-date {
                    display: block;
                    text-align: right;
                    font-size: 11px;
                    color: #6c757d;
                    margin-top: 10px;
                    font-style: italic;
                }

                .timeline-ul li:nth-child(odd) .sender-name-box {
                    background-color: #0F406B !important;
                    color: #ffffff !important;
                }

                    .timeline-ul li:nth-child(odd) .sender-name-box::before {
                        background-color: #0F406B !important;
                    }

                .timeline-ul li:nth-child(odd) .msg-content-box {
                    background-color: #e7f1ff !important;
                    border: 1px solid #dee2e6 !important;
                }

                .timeline-ul li:nth-child(even) .sender-name-box {
                    background-color: #e7f1ff !important;
                    color: #0F406B !important;
                    border: 1px solid #d1e3f8;
                }

                    .timeline-ul li:nth-child(even) .sender-name-box::before {
                        background-color: #0F406B !important;
                    }

                    .timeline-ul li:nth-child(even) .sender-name-box::after {
                        border-color: #e7f1ff !important;
                        background-color: #ffffff !important;
                    }

                .timeline-ul li:nth-child(even) .msg-content-box {
                    background-color: #ffffff !important;
                    border: 1px solid #e7f1ff !important;
                    box-shadow: 0 4px 12px rgba(0,0,0,0.05) !important;
                }

                .timeline-ul li:nth-child(even) .msg-text {
                    color: #0F406B !important;
                }

                .timeline-ul li:nth-child(even) .msg-date {
                    color: #94a3b8 !important;
                }

        @media (min-width: 45rem) {
            .timeline-ul {
                grid-template-columns: 1fr var(--line-w) 1fr;
            }

                .timeline-ul::before {
                    grid-column: 2;
                }

                .timeline-ul li:nth-child(odd) {
                    grid-column: 1;
                }

                .timeline-ul li:nth-child(even) {
                    grid-column: 3;
                }

                .timeline-ul li:nth-child(2) {
                    grid-row: 2/4;
                }

                .timeline-ul li:nth-child(odd) .sender-name-box {
                    border-radius: 0 calc(2.5rem / 2) calc(2.5rem / 2) 0;
                }

                    .timeline-ul li:nth-child(odd) .sender-name-box::before {
                        clip-path: polygon(0 0, 100% 0, 100% 100%);
                        left: 0;
                    }

                    .timeline-ul li:nth-child(odd) .sender-name-box::after {
                        transform: translate(-50%, -50%);
                        left: calc(100% + var(--col-gap) + var(--line-w) / 2);
                    }
        }
    </style>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid" style="padding: 20px;">
        <div class="row">
            <div class="col-lg-12">

                <asp:HiddenField ID="hfActiveTab" runat="server" Value="review" />
                <div class="row mb-4">
                    <div class="col-12">
                        <div class="btn-group w-100 shadow-sm" role="group">
                            <button type="button" id="btnTabReview" class="btn btn-primary" onclick="showPanel('review')"><i class="fal fa-edit me-2"></i>Review & Edit Request</button>
                            <button type="button" id="btnTabForeign" class="btn btn-outline-primary" onclick="showPanel('foreign')"><i class="fal fa-external-link-alt me-2"></i>Foreign Demands</button>
                            <button type="button" id="btnTabChat" class="btn btn-outline-primary" onclick="showPanel('chat')"><i class="fal fa-comments me-2"></i>Messages</button>
                        </div>
                    </div>
                </div>

                <div id="panel-review" style="display: block;">
                    <div class="card shadow-sm border mt-2">
                        <div class="card-header d-flex justify-content-between align-items-center" style="background-color: #0F406B; color: white; padding: 10px 15px;">
                            <h5 class="card-title mb-0" style="font-size: 14px; font-weight: 600;"><i class="fal fa-file-alt me-2"></i>Request Record (#<asp:Label ID="lblReqIdDisplay" runat="server"></asp:Label>)</h5>
                        </div>
                        <div class="card-body" style="padding: 25px;">
                            <div class="edit-box">
                                <h6 class="mb-3" style="color: #0F406B; font-weight: 700; font-size: 13px;">REQUEST DETAILS (EDITABLE)</h6>
                                <div class="row g-3">
                                    <div class="col-md-12">
                                        <label class="info-label">Topic <span class="text-danger">*</span></label>
                                        <asp:TextBox ID="txtTopic" runat="server" CssClass="form-control fw-bold" Style="color: #0F406B;"></asp:TextBox>
                                    </div>
                                    <div class="col-md-12">
                                        <label class="info-label">Description <span class="text-danger">*</span></label>
                                        <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="4" CssClass="form-control"></asp:TextBox>
                                    </div>
                                    <div class="col-md-4">
                                        <label class="info-label">Request Owner </label>
                                        <asp:DropDownList ID="ddlRequestOwner" runat="server" CssClass="form-select form-select-sm"></asp:DropDownList>
                                    </div>
                                    <div class="col-md-4">
                                        <label class="info-label">Related Person</label>
                                        <asp:DropDownList ID="ddlRelatedUser" runat="server" CssClass="form-select form-select-sm"></asp:DropDownList>
                                    </div>
                                    <div class="col-md-4">
                                        <label class="info-label">Related Device</label>
                                        <asp:DropDownList ID="ddlRelatedDevice" runat="server" CssClass="form-select form-select-sm"></asp:DropDownList>
                                    </div>
                                    <div class="col-md-12 mt-3">
                                        <label class="info-label">Attached Files</label>
                                        <div style="background-color: #fff; padding: 10px; border-radius: 4px; border: 1px dashed #ced4da; margin-bottom: 10px;">
                                            <asp:Literal ID="litExistingFiles" runat="server"></asp:Literal>
                                        </div>
                                        <label class="info-label text-primary mt-2"><i class="fal fa-upload me-1"></i>Upload New File(s)</label>
                                        <asp:FileUpload ID="fuAdminAttachment" runat="server" AllowMultiple="true" CssClass="form-control form-control-sm" accept=".jpeg,.jpg,.png,.docx,.pdf,.txt,.exe,.xlsx" />
                                    </div>
                                </div>
                            </div>

                            <hr style="border-top: 1px dashed #ccc;" class="my-4" />
                            <h6 class="mb-3" style="color: #0F406B; font-weight: 700; font-size: 13px;">ADMIN ACTIONS</h6>
                            <asp:UpdatePanel ID="upAdminTypes" runat="server">
                                <ContentTemplate>
                                    <div class="row g-3 mb-4">
                                        <div class="col-md-3">
                                            <label class="info-label">Request Type</label>
                                            <asp:DropDownList ID="ddlRequestType" runat="server" CssClass="form-select form-select-sm" AutoPostBack="true" OnSelectedIndexChanged="ddlRequestType_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                        <div class="col-md-3">
                                            <label class="info-label">Request Subtype</label>
                                            <asp:DropDownList ID="ddlRequestSubtype" runat="server" CssClass="form-select form-select-sm" Enabled="false">
                                                <asp:ListItem Text="-- Select Type First --" Value="0"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-md-3">
                                            <label class="info-label">Due Date</label>
                                            <asp:TextBox ID="txtDueDate" runat="server" CssClass="form-control form-control-sm" TextMode="DateTimeLocal"></asp:TextBox>
                                        </div>
                                        <div class="col-md-3">
                                            <label class="info-label">End Date</label>
                                            <asp:TextBox ID="txtEndDate" runat="server" CssClass="form-control form-control-sm" TextMode="DateTimeLocal" onchange="lockApprovalIfEndDate()"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row g-3 mb-4">
                                        <div class="col-md-3 d-flex align-items-center gap-3">
                                            <div class="form-check">
                                                <asp:CheckBox ID="chkIsImportant" runat="server" CssClass="form-check-input" />
                                                <label class="form-check-label text-warning fw-bold" style="font-size: 12px;">Important</label>
                                            </div>
                                            <div class="form-check">
                                                <asp:CheckBox ID="chkIsUrgent" runat="server" CssClass="form-check-input" />
                                                <label class="form-check-label text-danger fw-bold" style="font-size: 12px;">Urgent</label>
                                            </div>
                                        </div>
                                        <div class="col-md-3">
                                            <label class="info-label" style="color: #0F406B;">Approval Status</label>
                                            <asp:DropDownList ID="ddlApproval" runat="server" CssClass="form-select form-select-sm fw-bold">
                                                <asp:ListItem Text="Pending (Bekliyor)" Value="0"></asp:ListItem>
                                                <asp:ListItem Text="Approve (Onayla)" Value="1" style="color: green;"></asp:ListItem>
                                                <asp:ListItem Text="Reject (Reddet)" Value="2" style="color: red;"></asp:ListItem>
                                                <asp:ListItem Text="Completed (Tamamlandı)" Value="3" style="color: #0dcaf0;"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-md-3">
                                            <label class="info-label" style="color: #0F406B;">Score</label>
                                            <div class="p-1 border rounded bg-white shadow-sm d-flex align-items-center justify-content-between">
                                                <span class="info-label mb-0" style="color: #0F406B; font-size: 11px;"><i class="fas fa-star text-warning me-1"></i>USER QUALITY SCORE:</span>
                                                <asp:Label ID="lblQualityScore" runat="server" CssClass="badge bg-light text-muted border" Text="Not Rated Yet" Style="font-size: 11.5px; padding: 6px 10px;"></asp:Label>
                                            </div>
                                        </div>

                                        <asp:Panel ID="pnlAdminScoreDesc" runat="server" Visible="false" CssClass="col-md-12 mt-3">
                                            <label class="info-label" style="color: #0F406B;">Score Explanation</label>
                                            <asp:TextBox ID="txtAdminScoreDesc" runat="server" CssClass="form-control form-control-sm bg-light" TextMode="MultiLine" Rows="2" ReadOnly="true"></asp:TextBox><!--bura eklendi score desc-->
                                        </asp:Panel>

                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>

                            <hr style="border-top: 1px dashed #ccc;" class="my-4" />
                            <h6 class="mb-3" style="color: #0F406B; font-weight: 700; font-size: 13px;">PERSONNEL ASSIGNMENT</h6>

                            <asp:HiddenField ID="hfSelectedTaskId" runat="server" />
                            <div class="p-3 border rounded bg-light shadow-sm mb-4">
                                <div class="row g-3 align-items-end">
                                    <div class="col-md-4">
                                        <label class="info-label">Select Personnel</label>
                                        <asp:DropDownList ID="ddlEmployee" runat="server" CssClass="form-select form-select-sm shadow-sm"></asp:DropDownList>
                                    </div>
                                    <div class="col-md-3">
                                        <label class="info-label">Task Start</label>
                                        <asp:TextBox ID="txtTaskStart" runat="server" TextMode="DateTimeLocal" CssClass="form-control form-control-sm shadow-sm"></asp:TextBox>
                                    </div>
                                    <div class="col-md-3">
                                        <label class="info-label">Task End</label>
                                        <asp:TextBox ID="txtTaskEnd" runat="server" TextMode="DateTimeLocal" CssClass="form-control form-control-sm shadow-sm"></asp:TextBox>
                                    </div>
                                    <div class="col-md-2">
                                        <asp:LinkButton ID="btnAssign" runat="server" CssClass="btn btn-primary btn-sm w-100 fw-bold shadow-sm" OnClick="btnAssign_Click">
                                            <i class="fal fa-plus-circle me-1"></i> ASSIGN TASK
                                        </asp:LinkButton>
                                    </div>
                                </div>
                                <div class="mt-4">
                                    <asp:GridView ID="gvTasks" runat="server" AutoGenerateColumns="false" CssClass="table table-sm table-bordered panel-table bg-white shadow-sm" OnRowCommand="gvTasks_RowCommand">
                                        <Columns>
                                            <asp:BoundField DataField="EmployeeName" HeaderText="PERSONNEL" />
                                            <asp:BoundField DataField="StartDate" HeaderText="START DATE" DataFormatString="{0:dd.MM.yyyy HH:mm}" />
                                            <asp:BoundField DataField="EndDate" HeaderText="END DATE" DataFormatString="{0:dd.MM.yyyy HH:mm}" />
                                            <asp:TemplateField HeaderText="ACTION" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="120px">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnEditTask" runat="server" CommandName="EditTask" CommandArgument='<%# Eval("TaskID") %>' CssClass="me-2 text-decoration-none">
                                                        <i class="fal fa-edit text-primary"></i> <small>Edit</small>
                                                    </asp:LinkButton>
                                                    <asp:LinkButton ID="btnDel" runat="server" CommandName="DeleteTask" CommandArgument='<%# Eval("TaskID") %>' OnClientClick='<%# "return confirmDelete(this);" %>' CssClass="text-decoration-none">
                                                        <i class="fal fa-trash-alt text-danger"></i> <small>Del</small>
                                                    </asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>

                            <div class="d-flex justify-content-end mt-4 gap-2">
                                <asp:HiddenField ID="hfSelectedRequestId" runat="server" />
                                <asp:HiddenField ID="hfConfirmTaskClose" runat="server" ClientIDMode="Static" Value="0" />

                                <asp:LinkButton ID="btnCancel" runat="server" CssClass="btn btn-secondary rounded shadow-sm px-4 py-2" Style="font-size: 13px; font-weight: 600;" OnClick="btnCancel_Click">
                                    <i class="fal fa-arrow-left me-2"></i> BACK TO LIST
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnSaveAdmin" runat="server" CssClass="btn btn-primary rounded shadow-sm px-4 py-2" Style="font-size: 13px; font-weight: 600;" OnClick="btnSaveAdmin_Click">
                                    <i class="fal fa-check-circle me-2"></i> SAVE ALL CHANGES
                                </asp:LinkButton>
                            </div>
                        </div>
                    </div>
                </div>

                <div id="panel-foreign" style="display: none;">
                    <div class="card shadow-sm border mt-2">
                        <div class="card-header" style="background-color: #17a2b8; color: white; padding: 10px 15px;">
                            <h5 class="card-title mb-0" style="font-size: 14px; font-weight: 600;"><i class="fal fa-external-link-alt me-2"></i>Foreign Demands Management</h5>
                        </div>
                        <div class="card-body" style="padding: 25px;">
                            <asp:HiddenField ID="hfEditingForeignId" runat="server" />
                            <asp:HiddenField ID="hfDeleteForeignId" runat="server" />
                            <asp:Button ID="btnDeleteForeignConfirm" runat="server" OnClick="btnDeleteForeignConfirm_Click" Style="display: none;" />

                            <div class="row g-3 mb-4">
                                <div class="col-md-3">
                                    <label class="fw-bold small text-muted">Section</label>
                                    <asp:DropDownList ID="ddlForeignSection" runat="server" CssClass="form-select form-select-sm"></asp:DropDownList>
                                </div>
                                <div class="col-md-3">
                                    <label class="fw-bold small text-muted">Start Date</label>
                                    <asp:TextBox ID="txtForeignStartDate" runat="server" CssClass="form-control form-control-sm" TextMode="Date"></asp:TextBox>
                                </div>
                                <div class="col-md-3">
                                    <label class="fw-bold small text-muted">End Date</label>
                                    <asp:TextBox ID="txtForeignEndDate" runat="server" CssClass="form-control form-control-sm" TextMode="Date"></asp:TextBox>
                                </div>
                                <div class="col-md-12">
                                    <label class="fw-bold small text-muted">Description</label>
                                    <asp:TextBox ID="txtForeignExplanation" runat="server" CssClass="form-control form-control-sm" TextMode="MultiLine" Rows="2"></asp:TextBox>
                                </div>
                            </div>
                            <div class="mb-4">
                                <asp:LinkButton ID="btnForeignSend" runat="server" CssClass="btn btn-sm text-white px-3" Style="background-color: #0F406B;" OnClick="btnForeignSend_Click">
                                    <i class="fas fa-save me-1"></i> SAVE DEMAND
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnCancelEdit" runat="server" CssClass="btn btn-sm btn-secondary px-3" Visible="false" OnClick="btnCancelEdit_Click">
                                    CANCEL
                                </asp:LinkButton>
                            </div>

                            <div class="table-responsive" style="max-height: 300px; overflow-y: auto;">
                                <table class="table table-bordered table-sm mb-0">
                                    <thead style="background-color: #EEEEEE !important; color: #333 !important;">
                                        <tr>
                                            <th style="padding: 10px;">Section</th>
                                            <th style="padding: 10px;">Explanation</th>
                                            <th style="padding: 10px; width: 160px;">Date Range</th>
                                            <th style="padding: 10px; width: 110px; text-align: center;">Actions</th>
                                        </tr>
                                        <tr style="background-color: #f8f9fa !important;">
                                            <td style="padding: 5px;">
                                                <asp:TextBox ID="txtFilterForeignSection" runat="server" CssClass="form-control form-control-sm" placeholder="Search section..." Style="font-size: 11px;"></asp:TextBox>
                                            </td>
                                            <td style="padding: 5px;">
                                                <asp:TextBox ID="txtFilterForeignExp" runat="server" CssClass="form-control form-control-sm" placeholder="Search explanation..." Style="font-size: 11px;"></asp:TextBox>
                                            </td>
                                            <td style="padding: 5px;"></td>
                                            <td class="text-center" style="padding: 5px; vertical-align: middle;">
                                                <div class="btn-group btn-group-sm">
                                                    <asp:LinkButton ID="lbClearForeignFilter" runat="server" CssClass="btn btn-warning btn-sm" OnClick="lbClearForeignFilter_Click" title="Clear Filters">
                                                        <i class="fas fa-broom"></i>
                                                    </asp:LinkButton>
                                                    <asp:LinkButton ID="lbSearchForeign" runat="server" CssClass="btn btn-primary btn-sm" OnClick="lbSearchForeign_Click" title="Search">
                                                        <i class="fas fa-search"></i>
                                                    </asp:LinkButton>
                                                </div>
                                            </td>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="rptForeignDemands" runat="server" OnItemCommand="rptForeignDemands_ItemCommand">
                                            <ItemTemplate>
                                                <tr>
                                                    <td class="fw-bold text-primary" style="vertical-align: middle;"><%# Eval("Section") %></td>
                                                    <td style="vertical-align: middle;"><%# Eval("Explanation") %></td>
                                                    <td style="vertical-align: middle;">
                                                        <small class="text-muted"><i class="far fa-calendar-alt me-1"></i><%# Eval("StartDate", "{0:dd.MM.yyyy}") %> - <%# Eval("EndDate", "{0:dd.MM.yyyy}") %></small>
                                                    </td>
                                                    <td class="text-center" style="vertical-align: middle;">
                                                        <asp:LinkButton runat="server" CommandName="EditItem" CommandArgument='<%# Eval("ForeignDemandID") %>' CssClass="btn btn-outline-success btn-xs" ToolTip="Edit"><i class="fal fa-edit"></i></asp:LinkButton>
                                                        <button type="button" class="btn btn-outline-danger btn-xs btn-delete-foreign" data-id='<%# Eval("ForeignDemandID") %>' data-name='<%# Eval("Section") %>'><i class="fal fa-trash-alt"></i></button>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>

                <div id="panel-chat" style="display: none;">
                    <asp:UpdatePanel ID="upChat" runat="server">
                        <ContentTemplate>
                            <div class="card shadow-sm border mt-2">
                                <div class="card-header" style="background-color: #0F406B; color: white; padding: 10px 15px;">
                                    <h5 class="card-title mb-0" style="font-size: 14px; font-weight: 600;"><i class="fal fa-comments me-2"></i>Request Messages</h5>
                                </div>
                                <div class="card-body" style="padding: 25px; background-color: #f8f9fa;">
                                    <div id="userChatContainer" class="timeline-section" style="height: 450px; overflow-y: auto;">
                                        <ul class="timeline-ul">
                                            <asp:Repeater ID="rptChat" runat="server">
                                                <ItemTemplate>
                                                    <li>
                                                        <div class="sender-name-box shadow-sm">
                                                            <%# Eval("SenderName") %>
                                                        </div>
                                                        <div class="msg-content-box">
                                                            <div class="msg-text"><%# Eval("Messages") %></div>
                                                            <span class="msg-date">
                                                                <i class="fal fa-clock me-1"></i><%# Eval("RecordDate", "{0:dd.MM.yyyy HH:mm}") %>
                                                            </span>
                                                        </div>
                                                    </li>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </ul>
                                        <asp:Label ID="lblNoMessages" runat="server" Text="No messages yet." Visible="false" CssClass="text-muted fst-italic d-block text-center mt-4"></asp:Label>
                                    </div>
                                    <div class="d-flex gap-2 mt-3 align-items-start">
                                        <div class="flex-grow-1">
                                            <asp:TextBox ID="txtNewMessage" runat="server" CssClass="form-control" placeholder="Type your message..." TextMode="MultiLine" Rows="2"
                                                oninput="checkLength(this, 250, 'charCountMessage');"></asp:TextBox>
                                            <span id="charCountMessage" style="font-size: 0.75rem; color: #64748b; margin-top: 0.25rem; display: block;"></span>
                                        </div>

                                        <asp:LinkButton ID="btnSendMessage" runat="server" CssClass="btn btn-send-custom h-100" OnClick="btnSendMessage_Click" Style="min-height: 60px;">
                                            <i class="fas fa-paper-plane me-2"></i> SEND
                                        </asp:LinkButton>
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>

            </div>
        </div>
    </div>

    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script type="text/javascript">

        function lockApprovalIfEndDate() {
            var txtEnd = document.getElementById('<%= txtEndDate.ClientID %>');
            var ddlApp = document.getElementById('<%= ddlApproval.ClientID %>');

            if (txtEnd && ddlApp) {
                if (txtEnd.value.trim() !== '') {
                    ddlApp.value = '3';
                    ddlApp.style.pointerEvents = 'none';
                    ddlApp.style.backgroundColor = '#e9ecef';
                } else {
                    ddlApp.style.pointerEvents = 'auto';
                    ddlApp.style.backgroundColor = '#ffffff';
                }
            }
        }

        document.addEventListener("DOMContentLoaded", function () { restoreActiveTab(); });

        if (typeof Sys !== 'undefined' && Sys.WebForms && Sys.WebForms.PageRequestManager) {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                restoreActiveTab();
                var chatDiv = document.getElementById("chatContainer");
                if (chatDiv) chatDiv.scrollTop = chatDiv.scrollHeight;
            });
        }

        function restoreActiveTab() {
            var activeTabField = document.getElementById('<%= hfActiveTab.ClientID %>');
            if (activeTabField && activeTabField.value) showPanel(activeTabField.value, false);
        }

        function showPanel(panelName, isUserClick = true) {
            document.getElementById('panel-review').style.display = 'none';
            document.getElementById('panel-foreign').style.display = 'none';
            document.getElementById('panel-chat').style.display = 'none';

            document.getElementById('btnTabReview').className = 'btn btn-outline-primary';
            document.getElementById('btnTabForeign').className = 'btn btn-outline-primary';
            document.getElementById('btnTabChat').className = 'btn btn-outline-primary';

            if (panelName === 'review') {
                document.getElementById('panel-review').style.display = 'block';
                document.getElementById('btnTabReview').className = 'btn btn-primary';
            }
            else if (panelName === 'foreign') {
                document.getElementById('panel-foreign').style.display = 'block';
                document.getElementById('btnTabForeign').className = 'btn btn-primary';
            }
            else if (panelName === 'chat') {
                document.getElementById('panel-chat').style.display = 'block';
                document.getElementById('btnTabChat').className = 'btn btn-primary';
                var chatDiv = document.getElementById("chatContainer");
                if (chatDiv) chatDiv.scrollTop = chatDiv.scrollHeight;
            }

            if (isUserClick) {
                var activeTabField = document.getElementById('<%= hfActiveTab.ClientID %>');
                if (activeTabField) activeTabField.value = panelName;
            }
        }

        var shadowConfirm = false;
        function confirmDelete(btn) {
            if (shadowConfirm) { shadowConfirm = false; return true; }
            Swal.fire({
                title: 'Are you sure?', text: "This record will be permanently deleted!", icon: 'warning',
                showCancelButton: true, confirmButtonColor: '#0F406B', cancelButtonColor: '#d33',
                confirmButtonText: 'Yes, delete it!', cancelButtonText: 'Cancel'
            }).then((result) => {
                if (result.isConfirmed) { shadowConfirm = true; btn.click(); }
            });
            return false;
        }

        function pageLoad() {
            lockApprovalIfEndDate();

            $(document).off('click', '.btn-delete-foreign').on('click', '.btn-delete-foreign', function (e) {
                e.preventDefault();
                var id = $(this).attr('data-id');
                var name = $(this).attr('data-name');

                Swal.fire({
                    title: 'Are you sure?',
                    text: name + " demand will be deleted!",
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonColor: '#d33',
                    cancelButtonColor: '#3085d6',
                    confirmButtonText: 'Yes, Delete!',
                    cancelButtonText: 'Cancel'
                }).then((result) => {
                    if (result.isConfirmed) {
                        var hf = document.getElementById('<%= hfDeleteForeignId.ClientID %>');
                        var btn = document.getElementById('<%= btnDeleteForeignConfirm.ClientID %>');
                        if (hf && btn) {
                            hf.value = id;
                            btn.click();
                        }
                    }
                });
            });
        }

        function checkLength(textbox, maxLength, charCountId) {
            if (!textbox) return;
            var countSpan = document.getElementById(charCountId);

            if (textbox.value.length === 0) {
                countSpan.innerText = "";
                return;
            }

            if (textbox.value.length > maxLength) {
                textbox.value = textbox.value.substring(0, maxLength);
                alert("Text limit exceeded! Only " + maxLength + " characters allowed.");
            }

            countSpan.innerText = (maxLength - textbox.value.length) + " character..";
        }
    </script>
</asp:Content>
