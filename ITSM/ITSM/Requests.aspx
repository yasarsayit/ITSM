<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Requests.aspx.cs" Inherits="ITSM.Requests" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" media="screen, print" href="css/requests-page.css">
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid" style="padding: 20px;">
        <div class="row">

            <div class="col-lg-9">

                <asp:Panel ID="pnlTable" runat="server">
                    <div id="requestsTablePanel" class="card shadow-sm border mt-4">
                        <div class="card-header d-flex justify-content-between align-items-center" style="background-color: #0F406B; color: white; padding: 12px 20px;">
                            <h5 class="card-title mb-0" style="font-size: 15px; font-weight: 600;"><i class="fal fa-list-alt mr-2"></i><%= gettext("myrequests", "My Requests") %></h5>
                            <asp:LinkButton ID="btnNewRequest" runat="server" CssClass="btn btn-light btn-sm fw-bold shadow-sm" OnClick="btnNewRequest_Click">
                                <i class="fal fa-plus me-1"></i><%= gettext("newrequest", "New Request") %>
                            </asp:LinkButton>
                        </div>
                        <div class="card-body">

                            <div class="d-flex justify-content-between align-items-center mb-3">
                                <div style="font-size: 13px; font-weight: 600; color: #555;">
                                    <%= gettext("show", "Show") %>
                                    <asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged" CssClass="form-select form-select-sm d-inline-block mx-1" Style="width: auto;">
                                        <asp:ListItem Text="5" Value="5"></asp:ListItem>
                                        <asp:ListItem Text="10" Value="10" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="25" Value="25"></asp:ListItem>
                                        <asp:ListItem Text="50" Value="50"></asp:ListItem>
                                        <asp:ListItem Text="100" Value="100"></asp:ListItem>
                                    </asp:DropDownList>
                                    <%= gettext("entries", "Entries") %>
                                </div>
                            </div>

                            <div class="table-responsive">
                                <table class="table panel-table no-hover table-bordered mb-0" style="font-size: 13px;">
                                    <thead class="table-light text-nowrap align-middle">
                                        <tr>
                                            <th style="width: 80px; text-align: center;">Req. ID</th>
                                            <th><%= gettext("topic", "Topic") %></th>
                                            <th style="width: 100px;"><%= gettext("reqdate", "Request Date") %></th>
                                            <th style="width: 100px; text-align: center;"><%= gettext("status", "Status") %></th>

                                            <th style="width: 90px; text-align: center;"><%= gettext("score", "Score") %></th>

                                            <th style="width: 130px; text-align: center;"><%= gettext("actions", "Actions") %></th>
                                        </tr>
                                        <tr class="arama">
                                            <td>
                                                <asp:TextBox ID="txtFilterReqId" runat="server" CssClass="form-control form-control-sm frmara" placeholder="ID..."></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtFilterTopic" runat="server" CssClass="form-control form-control-sm frmara" placeholder="Search Topic..."></asp:TextBox></td>
                                            <td>
                                                <asp:TextBox ID="txtFilterDate" runat="server" CssClass="form-control form-control-sm frmara" TextMode="Date"></asp:TextBox></td>
                                            <td>
                                                <asp:DropDownList ID="ddlFilterStatus" runat="server" CssClass="form-select form-select-sm frmara">
                                                    <asp:ListItem Text="All" Value=""></asp:ListItem>
                                                    <asp:ListItem Text="Pending" Value="0"></asp:ListItem>
                                                    <asp:ListItem Text="Approved" Value="1"></asp:ListItem>
                                                    <asp:ListItem Text="Rejected" Value="2"></asp:ListItem>
                                                    <asp:ListItem Text="Completed" Value="3"></asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td></td>
                                            <td style="text-align: center;">
                                                <div class="btn-group btn-group-sm">
                                                    <asp:LinkButton ID="lbAraTemizle" runat="server" CssClass="btn btn-warning btn-sm" OnClick="lbAraTemizle_Click" title="Clear Filters"><i class="fas fa-broom"></i></asp:LinkButton>
                                                    <asp:LinkButton ID="lbAra" runat="server" CssClass="btn btn-primary btn-sm" OnClick="lbAra_Click" title="Search"><i class="fas fa-search"></i></asp:LinkButton>
                                                </div>
                                            </td>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="rptRequests" runat="server" OnItemCommand="rptRequests_ItemCommand">
                                            <ItemTemplate>
                                                <tr class='text-nowrap <%# GetRowColorClass(Eval("IsConfirmed")) %>'>
                                                    <td class="fw-bold text-center"><%# Eval("RequestId") %></td>
                                                    <td style="font-weight: 600; color: #0F406B;">
                                                        <%# GetPriorityBadges(Eval("IsImportant"), Eval("IsUrgent")) %> <%# Eval("Topic") %>
                                                    </td>
                                                    <td><%# Eval("RequestDate", "{0:dd.MM.yyyy HH:mm}") %></td>
                                                    <td class="text-center"><%# GetStatusBadge(Eval("IsConfirmed")) %></td>
                                                    <td class="text-center"><%# GetScoreBadge(Eval("Score"), Eval("IsConfirmed")) %></td>

                                                    <td style="text-align: left">
                                                        <div style="display: flex; gap: 8px;">
                                                            <asp:LinkButton ID="btnViewRequest" runat="server" CommandName="ViewRequest" CommandArgument='<%# Eval("RequestId") %>' CssClass="btn btn-outline-primary btn-sm bg-white" title='<%# gettext("details_chat", "Details & Chat") %>'>
                                                                     <i class="fa fa-eye me-1" style="pointer-events: none;"></i> 
                                                            </asp:LinkButton>
                                                            <asp:PlaceHolder runat="server" Visible='<%# IsPending(Eval("IsConfirmed")) %>'>
                                                                <button type="button" class="btn btn-outline-danger btn-sm btn-delete-trigger bg-white" title='<%# gettext("delete", "Delete") %>' data-id='<%# Eval("RequestId") %>'>
                                                                    <i class="fal fa-trash-alt" style="pointer-events: none;"></i>
                                                                </button>
                                                            </asp:PlaceHolder>
                                                        </div>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>

                                        <tr id="trNoData" runat="server" visible="false">
                                            <td colspan="6" class="text-center text-muted p-4">
                                                <i class="fal fa-info-circle fa-2x mb-2"></i>
                                                <br />
                                                <%= gettext("noreqfound", "No requests found matching your criteria.") %>
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
                                            <asp:LinkButton ID="lbPrev" runat="server" CssClass="page-link" OnClick="lbPrev_Click">&laquo; <%= gettext("prev", "Prev") %></asp:LinkButton>
                                        </li>
                                        <asp:Repeater ID="rptPagination" runat="server" OnItemCommand="rptPagination_ItemCommand">
                                            <ItemTemplate>
                                                <li class='page-item <%# Convert.ToBoolean(Eval("IsActive")) ? "active" : "" %>'>
                                                    <asp:LinkButton ID="lbPage" runat="server" CssClass="page-link" CommandName="Page" CommandArgument='<%# Eval("PageNumber") %>'><%# Eval("PageNumber") %></asp:LinkButton>
                                                </li>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                        <li class="page-item" id="liNext" runat="server">
                                            <asp:LinkButton ID="lbNext" runat="server" CssClass="page-link" OnClick="lbNext_Click"> <%= gettext("next", "Next") %> &raquo;</asp:LinkButton>
                                        </li>
                                    </ul>
                                </div>
                            </div>

                        </div>
                    </div>
                </asp:Panel>

                <asp:Panel ID="pnlChat" runat="server" Visible="false">
                    <div class="d-flex justify-content-between align-items-center mb-3 mt-4">
                        <h5 class="mb-0" style="color: #0F406B; font-weight: 700; font-size: 16px;">
                            <i class="fal fa-clipboard-list-check me-2"></i><%= gettext("recrecord", "Request Record") %>(#<asp:Label ID="lblChatReqId" runat="server"></asp:Label>)  
                        </h5>
                        <asp:LinkButton ID="btnBackToTable" runat="server" CssClass="btn btn-sm btn-secondary fw-bold" OnClick="btnBackToTable_Click">
                            <i class="fal fa-arrow-left me-1"></i><%= gettext("backtolist", "Back to List") %>
                        </asp:LinkButton>
                    </div>

                    <div class="row">
                        <div class="col-md-5 mb-4">
                            <div class="card shadow-sm border h-100">
                                <div class="card-header bg-light border-bottom">
                                    <h6 class="mb-0 fw-bold" style="color: #0F406B; font-size: 13px;"><i class="fal fa-info-circle me-2"></i><%= gettext("reqde", "Request Record") %></h6>
                                </div>
                                <div class="card-body" style="font-size: 13px; background-color: #fcfcfc;">


                                    <asp:Panel ID="pnlQualityScore" runat="server" Visible="false" CssClass="mb-4 pb-3 border-bottom">
                                        <h6 style="color: #0F406B; font-weight: 700; font-size: 12px;"><i class="fal fa-star me-2 text-warning"></i><%= gettext("qualityscore", "Quality Score") %></h6>
                                        <div class="d-flex gap-2 align-items-center">
                                            <asp:DropDownList ID="ddlQualityScore" runat="server" CssClass="form-select form-select-sm shadow-sm" Style="border-color: #0F406B; height: 33px; padding-top: 4px; padding-bottom: 4px;" onchange="toggleScoreDesc()">
                                                <asp:ListItem Text="-- Select Score --" Value="-1"></asp:ListItem>
                                                <asp:ListItem Text="0 - Far below expectations" Value="0"></asp:ListItem>
                                                <asp:ListItem Text="1 - Below expectations" Value="1"></asp:ListItem>
                                                <asp:ListItem Text="2 - Met expectations" Value="2"></asp:ListItem>
                                                <asp:ListItem Text="3 - Above expectations" Value="3"></asp:ListItem>
                                            </asp:DropDownList>


                                            <asp:LinkButton ID="btnSaveScore" runat="server" CssClass="btn btn-sm btn-success fw-bold shadow-sm text-nowrap d-inline-flex align-items-center justify-content-center" Style="height: 33px; padding: 0 15px;" OnClientClick="return confirmScoreSave();">
                 <i class="fal fa-check me-1" style="margin-top: -1px;"></i> <%= gettext("save", "Save") %>
                                            </asp:LinkButton>
                                        </div>


                                        <div id="divScoreDesc" style="display: none; margin-top: 15px; width: 100%;">
                                            <label style="font-size: 12px; font-weight: 700; color: #0F406B;"><%= gettext("score_reason", "Reason for Low Score (Required)") %></label>
                                            <asp:TextBox ID="txtScoreDesc" runat="server" CssClass="form-control form-control-sm" TextMode="MultiLine" Rows="2" MaxLength="250" oninput="checkLength(this, 250, 'charCountScoreDesc');"></asp:TextBox>
                                            <span id="charCountScoreDesc" style="font-size: 0.75rem; color: #64748b; margin-top: 0.25rem; display: block;"></span>
                                        </div>


                                        <asp:Button ID="btnRealSaveScore" runat="server" OnClick="btnSaveScore_Click" Style="display: none;" ClientIDMode="Static" />

                                        <asp:Label ID="lblScoreThanks" runat="server" CssClass="text-success mt-2 d-block fw-bold" Visible="false">
           <i class="fal fa-check-circle me-1"></i> <%= gettext("feedbacktext", "Thank you for your feedback!") %>
                                        </asp:Label>

                                        <asp:Label ID="smScoreInfo" runat="server" CssClass="text-muted mt-2 d-block fst-italic">
           <%= gettext("givefeedbacktext", "This request has been completed. You can rate the service quality.") %>
                                        </asp:Label>
                                    </asp:Panel>


                                    <asp:Literal ID="litRequestDetails" runat="server"></asp:Literal>
                                </div>
                            </div>
                        </div>

                        <div class="col-md-7 mb-4">
                            <div class="card shadow-sm border h-100 d-flex flex-column">
                                <div class="card-header" style="background-color: #0F406B; color: white; padding: 12px 20px;">
                                    <h6 class="mb-0 fw-bold" style="font-size: 13px;"><i class="fal fa-comments me-2"></i><%= gettext("msj", "Mesajlar") %></h6>
                                </div>
                                <div class="card-body p-0 d-flex flex-column" style="background-color: #f8f9fa; flex: 1;">
                                    <div id="userChatContainer" class="timeline-section" style="height: 450px; overflow-y: auto; padding: 20px;">
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
                                    <div class="p-3 bg-white border-top">
                                        <div class="d-flex gap-2 align-items-start">
                                            <div class="flex-grow-1">
                                                <asp:TextBox ID="txtNewMessage" runat="server" CssClass="form-control" placeholder="Type your message here..." TextMode="MultiLine" Rows="2" MaxLength="250" oninput="checkLength(this, 250, 'charCountMessageReq');"></asp:TextBox>
                                                <span id="charCountMessageReq" style="font-size: 0.75rem; color: #64748b; margin-top: 0.25rem; display: block;"></span>
                                            </div>
                                            <asp:LinkButton ID="btnSendMessage" runat="server" CssClass="btn btn-send-custom h-100" OnClick="btnSendMessage_Click" Style="min-height: 60px;">
                                                <i class="fas fa-paper-plane me-2"></i>  <%= gettext("send", "Send") %>
                                            </asp:LinkButton>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <asp:HiddenField ID="hfChatRequestId" runat="server" />
                </asp:Panel>
            </div>

            <div class="col-lg-3 d-none d-lg-block">
                <div class="last-topics-sidebar">
                    <h6 class="mb-3" style="font-size: 12px; font-weight: 700; color: #333; border-bottom: 2px solid #0F406B; padding-bottom: 5px;"><%= gettext("rcntopic", "RECENT TOPICS")%></h6>
                    <asp:Repeater ID="rptTopics" runat="server">
                        <ItemTemplate>
                            <a href='TopicView.aspx?id=<%# Eval("KnowledgeID") %>' class="topic-preview-trigger" data-title='<%# HttpUtility.HtmlAttributeEncode(Convert.ToString(Eval("Title"))) %>' data-summary='<%# HttpUtility.HtmlAttributeEncode(Convert.ToString(Eval("Summary"))) %>' style="text-decoration: none; display: block; color: inherit;">
                                <div class="topic-item"><%# Eval("Title") %></div>
                            </a>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>

        </div>
    </div>

    <asp:HiddenField ID="hfDeleteId" runat="server" ClientIDMode="Static" />
    <asp:Button ID="btnDeleteConfirm" runat="server" OnClick="btnDeleteConfirm_Click" Style="display: none;" ClientIDMode="Static" />
    <asp:HiddenField ID="hfAlertType" runat="server" />
    <asp:HiddenField ID="hfAlertMessage" runat="server" />
    <asp:HiddenField ID="hfAlertTitle" runat="server" />
    <asp:HiddenField ID="hfRedirectUrl" runat="server" />

    <span id="topicTooltipCtaText" class="d-none" aria-hidden="true"><%= gettext("topicdetailcta", "Detaylar İçin Tıklayınız..") %></span>
    <div id="topicSummaryTooltip" class="topic-summary-tooltip" role="tooltip" aria-hidden="true"></div>

    <script type="text/javascript">
        window.onload = function () {
            var hfType = document.getElementById('<%= hfAlertType.ClientID %>');
           var hfMsg = document.getElementById('<%= hfAlertMessage.ClientID %>');
           var hfTitle = document.getElementById('<%= hfAlertTitle.ClientID %>');

           if (hfType && hfType.value !== "") {
               Swal.fire({
                   icon: hfType.value,
                   title: hfTitle.value,
                   text: hfMsg.value,

                   confirmButtonText: `<%= gettext("ok_btn", "OK") %>`,
                    confirmButtonColor: '#0F406B'
                }).then((result) => {
                    hfType.value = "";
                    hfMsg.value = "";
                    hfTitle.value = "";
                    var url = document.getElementById('<%= hfRedirectUrl.ClientID %>').value;
                    if (url) { window.location.href = url; }
                });
            }
        };

        function checkLength(textbox, maxLength, charCountId) {
            if (!textbox) return;
            var countSpan = document.getElementById(charCountId);

            if (textbox.value.length === 0) {
                countSpan.innerText = "";
                return;
            }

            if (textbox.value.length > maxLength) {
                textbox.value = textbox.value.substring(0, maxLength);

                alert(`<%= gettext("text_limit_exceeded", "Text limit exceeded! Only ") %>` + maxLength + `<%= gettext("characters_allowed", " characters allowed.") %>`);
           }


           countSpan.innerText = (maxLength - textbox.value.length) + ` <%= gettext("character", "character..") %>`;
        }


        function toggleScoreDesc() {
            var ddl = document.getElementById('<%= ddlQualityScore.ClientID %>');
            var div = document.getElementById('divScoreDesc');
            if (ddl && div) {
                if (ddl.value === '0' || ddl.value === '1') {
                    div.style.display = 'block';
                } else {
                    div.style.display = 'none';
                }
            }
        }

        function confirmScoreSave() {
            var ddl = document.getElementById('<%= ddlQualityScore.ClientID %>');
           var scoreValue = ddl ? ddl.value : "-1";

           if (scoreValue === "-1") {

               document.getElementById('btnRealSaveScore').click();
               return false;
           }

           var scoreText = ddl.options[ddl.selectedIndex].text;

           Swal.fire({
               title: `<%= gettext("are_you_sure", "Are You Sure?") %>`,
               html: `Şu puanı vermek üzeresiniz:<br><br><b>${scoreText}</b><br><br>Onaylıyor musunuz?`,
               icon: 'question',
               showCancelButton: true,
               confirmButtonColor: '#198754',
               cancelButtonColor: '#d33',
               confirmButtonText: `Evet, Onaylıyorum`,
               cancelButtonText: `<%= gettext("cancel", "Cancel") %>`
           }).then((result) => {
               if (result.isConfirmed) {
                   document.getElementById('btnRealSaveScore').click();
               }
           });
            return false;
        }

        document.addEventListener('click', function (e) {
            const deleteBtn = e.target.closest('.btn-delete-trigger');
            if (deleteBtn) {
                const id = deleteBtn.getAttribute('data-id');
                Swal.fire({

                    title: `<%= gettext("are_you_sure", "Are You Sure?") %>`,
                    text: `<%= gettext("id_will_be_deleted_1", "ID: ") %>` + id + `<%= gettext("id_will_be_deleted_2", " will be deleted!") %>`,
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonColor: '#d33',
                    cancelButtonColor: '#3085d6',
                    confirmButtonText: `<%= gettext("yes_delete", "Yes, Delete!") %>`,
                    cancelButtonText: `<%= gettext("cancel", "Cancel") %>`
                }).then((result) => {
                    if (result.isConfirmed) {
                        document.getElementById('hfDeleteId').value = id;
                        document.getElementById('btnDeleteConfirm').click();
                    }
                });
            }
        });


        function fixMyChat() {
            if (window.location.hash === '#chatSection') {
                var div = document.getElementById('userChatContainer');
                if (div) { div.scrollTop = div.scrollHeight; }
            }
        }

        function handleChatPersistence() {
            if (window.location.hash === '#chatSection') {
                var chatDiv = document.getElementById('userChatContainer');
                if (chatDiv) { chatDiv.scrollTop = chatDiv.scrollHeight; }
            }
        }

        var prm = Sys.WebForms.PageRequestManager.getInstance();
        if (prm) {
            prm.add_endRequest(function () {
                handleChatPersistence();
                bindTopicPreviewEvents();
                toggleScoreDesc();
            });
        }

        var isTopicPreviewBound = false;
        function bindTopicPreviewEvents() {
            if (isTopicPreviewBound) return;
            isTopicPreviewBound = true;

            var tooltipEl = document.getElementById('topicSummaryTooltip');
            var activeTrigger = null;

            function escapeHtml(s) {
                if (s == null || s === '') return '';
                var div = document.createElement('div');
                div.textContent = s;
                return div.innerHTML;
            }

            function hideTooltip() {
                if (!tooltipEl) return;
                tooltipEl.classList.remove('show');
                tooltipEl.setAttribute('aria-hidden', 'true');
                tooltipEl.innerHTML = '';
                activeTrigger = null;
            }

            function positionTooltip(evt) {
                if (!tooltipEl || !activeTrigger) return;
                var offsetX = 14;
                var offsetY = 18;
                var left = evt.clientX + offsetX;
                var top = evt.clientY + offsetY;

                var ttWidth = tooltipEl.offsetWidth || 320;
                var ttHeight = tooltipEl.offsetHeight || 80;
                var vw = window.innerWidth;
                var vh = window.innerHeight;

                if (left + ttWidth > vw - 8) left = vw - ttWidth - 8;
                if (top + ttHeight > vh - 8) top = evt.clientY - ttHeight - 12;
                if (top < 8) top = 8;
                if (left < 8) left = 8;

                tooltipEl.style.left = left + 'px';
                tooltipEl.style.top = top + 'px';
            }

            document.addEventListener('mouseenter', function (e) {
                var trigger = e.target.closest('.topic-preview-trigger');
                if (!trigger) return;

                var summary = trigger.getAttribute('data-summary') || '<%= gettext("norecord", "No summary available.") %>';
               if (!tooltipEl) return;
               var ctaEl = document.getElementById('topicTooltipCtaText');
               var ctaText = (ctaEl && ctaEl.textContent) ? ctaEl.textContent.trim() : 'Detaylar İçin Tıklayınız..';
               tooltipEl.innerHTML = escapeHtml(summary) + '<span class="topic-summary-tooltip-cta">' + escapeHtml(ctaText) + '</span>';
               tooltipEl.classList.add('show');
               tooltipEl.setAttribute('aria-hidden', 'false');
               activeTrigger = trigger;
               positionTooltip(e);
           }, true);

            document.addEventListener('mousemove', function (e) {
                positionTooltip(e);
            }, true);

            document.addEventListener('mouseleave', function (e) {
                if (e.target.closest('.topic-preview-trigger')) {
                    hideTooltip();
                }
            }, true);

            document.addEventListener('scroll', hideTooltip, true);
            window.addEventListener('resize', hideTooltip);
        }

        document.addEventListener('DOMContentLoaded', function () {
            fixMyChat();
            handleChatPersistence();
            bindTopicPreviewEvents();
            toggleScoreDesc();

            setTimeout(function () {
                var chatPanel = document.getElementById('<%= pnlChat.ClientID %>');
               if (!chatPanel || chatPanel.style.display === 'none') {
                   document.location.reload();
               } else {
                   console.log("Automatic refresh was cancelled because the chat was open.");
               }
           }, 30000);
       });
    </script>
</asp:Content>
