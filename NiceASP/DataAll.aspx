<%@ Page Title="Edit" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="DataAll.aspx.cs" Inherits="Data" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h2><%: Title %>.</h2>

    <div class="row">
        <div class="col-md-8">
            <section id="loginForm">
                <div class="form-horizontal">
                    <div runat="server" id="sectionEmailInput" visible="false">
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="I_UserEmail" CssClass="col-md-2 control-label">User Email</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="I_UserEmail" CssClass="form-control" />
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="col-md-offset-2 col-md-10">
                                <asp:Button runat="server" OnClick="GetData_Click" Text="Get Data" CssClass="btn btn-default" />
                            </div>
                        </div>
                    </div>

                    <div runat="server" id="sectinDetails" visible="false">
                        <div class="form-group">
                            <div class="col-md-offset-2 col-md-10">
                                <asp:Button runat="server" OnClick="UpdateData_Click" Text="Update Data" CssClass="btn btn-default" />
                                <asp:Button runat="server" OnClick="ShowDetails_Click" Text="Show Details" CssClass="btn btn-default" />
                                <asp:Label runat="server" ID="L_FileVersion" CssClass="col-md-2 control-label" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox runat="server" ID="L_UserName_Store" />
                            <asp:Label runat="server" AssociatedControlID="L_UserName" CssClass="col-md-2 control-label">User Name</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="L_UserName" CssClass="form-control" OnTextChanged="L_UserName_TextChanged" AutoPostBack="true" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox runat="server" ID="L_UserEmail_Store" />
                            <asp:Label runat="server" AssociatedControlID="L_UserEmail" CssClass="col-md-2 control-label">User Email</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="L_UserEmail" CssClass="form-control" OnTextChanged="L_UserEmail_TextChanged" AutoPostBack="true" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="L_ApiGuId" CssClass="col-md-2 control-label">X-APIId</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="L_ApiGuId" CssClass="form-control" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox runat="server" ID="L_UserPassword_Store" />
                            <asp:Label runat="server" AssociatedControlID="L_UserPassword" CssClass="col-md-2 control-label">User Password</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="L_UserPassword" CssClass="form-control" OnTextChanged="L_UserPassword_TextChanged" AutoPostBack="true" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox runat="server" ID="L_AllTelNumbers_Store" />
                            <asp:Label runat="server" AssociatedControlID="L_AllTelNumbers" CssClass="col-md-2 control-label">All Numbers</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="L_AllTelNumbers" CssClass="form-control" OnTextChanged="L_AllTelNumbers_TextChanged" AutoPostBack="true" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox runat="server" ID="L_AllUnconfTelNumbers_Store" />
                            <asp:Label runat="server" AssociatedControlID="L_AllUnconfTelNumbers" CssClass="col-md-2 control-label">All Unconfirmed Numbers</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="L_AllUnconfTelNumbers" CssClass="form-control" OnTextChanged="L_AllUnconfTelNumbers_TextChanged" AutoPostBack="true" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox runat="server" ID="L_CreationIp_Store" />
                            <asp:Label runat="server" AssociatedControlID="L_CreationIp" CssClass="col-md-2 control-label">Creation Ip</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="L_CreationIp" CssClass="form-control" OnTextChanged="L_CreationIp_TextChanged" AutoPostBack="true" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="L_CountryName" CssClass="col-md-2 control-label">Country Name</asp:Label>
                            <div class="col-md-10">
                                <asp:Label runat="server" ID="L_CountryName" CssClass="form-control" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="L_CreatedDate" CssClass="col-md-2 control-label">CreatedDate</asp:Label>
                            <div class="col-md-10">
                                <asp:Label runat="server" ID="L_CreatedDate" CssClass="form-control" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox runat="server" ID="L_Status_Store" />
                            <asp:Label runat="server" AssociatedControlID="L_Status" CssClass="col-md-2 control-label">Status</asp:Label>
                            <div class="col-md-10">
                                <asp:Button runat="server" OnClick="ResetStatus_Click" Text="ResetStatus" CssClass="btn btn-default" />
                                <asp:TextBox runat="server" ID="L_Status" CssClass="form-control" OnTextChanged="L_Status_TextChanged" AutoPostBack="true" />
                                <div runat="server" id="L_StatusExplained" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox runat="server" ID="L_Comment_Store" />
                            <asp:Label runat="server" AssociatedControlID="L_Comment" CssClass="col-md-2 control-label">Comment</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="L_Comment" CssClass="form-control" OnTextChanged="L_Comment_TextChanged" AutoPostBack="true" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox runat="server" ID="L_DeleteOnFailed_Store" />
                            <asp:Label runat="server" AssociatedControlID="L_DeleteOnFailed" CssClass="col-md-2 control-label">Delete on Fail</asp:Label>
                            <div class="col-md-10">
                                <asp:CheckBox runat="server" ID="L_DeleteOnFailed" CssClass="form-control" OnCheckedChanged="L_DeleteOnFailed_CheckedChanged" AutoPostBack="true" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox runat="server" ID="L_AddNumberAllowedWithAPI_Store" />
                            <asp:Label runat="server" AssociatedControlID="L_AddNumberAllowedWithAPI" CssClass="col-md-2 control-label">Add number- allowed with API</asp:Label>
                            <div class="col-md-10">
                                <asp:CheckBox runat="server" ID="L_AddNumberAllowedWithAPI" CssClass="form-control" OnCheckedChanged="L_AddNumberAllowedWithAPI_CheckedChanged" AutoPostBack="true" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox runat="server" ID="L_AddNumberActivateOnSyncRequest_Store" />
                            <asp:Label runat="server" AssociatedControlID="L_AddNumberActivateOnSyncRequest" CssClass="col-md-2 control-label">Add number- Confirm on Tel sync</asp:Label>
                            <div class="col-md-10">
                                <asp:CheckBox runat="server" ID="L_AddNumberActivateOnSyncRequest" CssClass="form-control" OnCheckedChanged="L_AddNumberActivateOnSyncRequest_CheckedChanged" AutoPostBack="true" />
                            </div>
                        </div>
                    </div>

                    <div runat="server" id="sectionFree" visible="false">
                        <hr />
                        <h3>sectionFree</h3>
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="Lf_LastMsgQueued" CssClass="col-md-2 control-label">Last queued message</asp:Label>
                            <div class="col-md-10">
                                <asp:Label runat="server" ID="Lf_LastMsgQueued" CssClass="form-control" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox runat="server" ID="Lf_MsgSent_Store" />
                            <asp:Label runat="server" AssociatedControlID="Lf_MsgSent" CssClass="col-md-2 control-label">Messages Sent</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="Lf_MsgSent" CssClass="form-control" OnTextChanged="Lf_MsgSent_TextChanged" AutoPostBack="true" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox runat="server" ID="Lf_MsgLeft_Store" />
                            <asp:Label runat="server" AssociatedControlID="Lf_MsgLeft" CssClass="col-md-2 control-label">Messages Left</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="Lf_MsgLeft" CssClass="form-control" OnTextChanged="Lf_MsgLeft_TextChanged" AutoPostBack="true" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox runat="server" ID="Lf_MinDelayInSeconds_Store" />
                            <asp:Label runat="server" AssociatedControlID="Lf_MinDelayInSeconds" CssClass="col-md-2 control-label">Minimum delay</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="Lf_MinDelayInSeconds" CssClass="form-control" OnTextChanged="Lf_MinDelayInSeconds_TextChanged" AutoPostBack="true" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox runat="server" ID="Lf_SendFooter_Store" />
                            <asp:Label runat="server" AssociatedControlID="Lf_SendFooter" CssClass="col-md-2 control-label">Send Footer</asp:Label>
                            <div class="col-md-10">
                                <asp:CheckBox runat="server" ID="Lf_SendFooter" CssClass="form-control" OnCheckedChanged="Lf_SendFooter_CheckedChanged" AutoPostBack="true" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox runat="server" ID="Lf_WelcomeCounter_Store" />
                            <asp:Label runat="server" AssociatedControlID="Lf_WelcomeCounter" CssClass="col-md-2 control-label">Welcome Counter</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="Lf_WelcomeCounter" CssClass="form-control" OnTextChanged="Lf_WelcomeCounter_TextChanged" AutoPostBack="true" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox runat="server" ID="Lf_MsgQueued_Store" />
                            <asp:Label runat="server" AssociatedControlID="Lf_MsgQueued" CssClass="col-md-2 control-label">Msg Queued</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="Lf_MsgQueued" CssClass="form-control" OnTextChanged="Lf_MsgQueued_TextChanged" AutoPostBack="true" />
                            </div>
                        </div>
                    </div>

                    <div runat="server" id="sectionMonthly" visible="false">
                        <hr />
                        <h3>sectionMonthly</h3>
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="Lm_LastMsgQueued" CssClass="col-md-2 control-label">Last queued message</asp:Label>
                            <div class="col-md-10">
                                <asp:Label runat="server" ID="Lm_LastMsgQueued" CssClass="form-control" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox runat="server" ID="Lm_MsgSent_Store" />
                            <asp:Label runat="server" AssociatedControlID="Lm_MsgSent" CssClass="col-md-2 control-label">Messages Sent</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="Lm_MsgSent" CssClass="form-control" OnTextChanged="Lm_MsgSent_TextChanged" AutoPostBack="true" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox runat="server" ID="Lm_PaidUntil_Store" />
                            <asp:Label runat="server" AssociatedControlID="Lm_PaidUntil" CssClass="col-md-2 control-label">Paid Until</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="Lm_PaidUntil" CssClass="form-control" OnTextChanged="Lm_PaidUntil_TextChanged" AutoPostBack="true" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox runat="server" ID="Lm_MinDelayInSeconds_Store" />
                            <asp:Label runat="server" AssociatedControlID="Lm_MinDelayInSeconds" CssClass="col-md-2 control-label">Minimum delay</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="Lm_MinDelayInSeconds" CssClass="form-control" OnTextChanged="Lm_MinDelayInSeconds_TextChanged" AutoPostBack="true" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox runat="server" ID="Lm_CostPerNumber_Store" />
                            <asp:Label runat="server" AssociatedControlID="Lm_CostPerNumber" CssClass="col-md-2 control-label">Cost Per Number</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="Lm_CostPerNumber" CssClass="form-control" OnTextChanged="Lm_CostPerNumber_TextChanged" AutoPostBack="true" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox runat="server" ID="Lm_CurrentCredit_Store" />
                            <asp:Label runat="server" AssociatedControlID="Lm_CurrentCredit" CssClass="col-md-2 control-label">Current Credit</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="Lm_CurrentCredit" CssClass="form-control" OnTextChanged="Lm_CurrentCredit_TextChanged" AutoPostBack="true" />
                            </div>
                        </div>
                    </div>

                    <div runat="server" id="sectionMonthlyDifPrice" visible="false">
                        <hr />
                        <h3>sectionMonthlyDifPrice</h3>
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="Lm2_LastMsgQueued" CssClass="col-md-2 control-label">Last queued message</asp:Label>
                            <div class="col-md-10">
                                <asp:Label runat="server" ID="Lm2_LastMsgQueued" CssClass="form-control" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox runat="server" ID="Lm2_TotalMsgSent_Store" />
                            <asp:Label runat="server" AssociatedControlID="Lm2_TotalMsgSent" CssClass="col-md-2 control-label">Total Messages Sent</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="Lm2_TotalMsgSent" CssClass="form-control" OnTextChanged="Lm2_TotalMsgSent_TextChanged" AutoPostBack="true" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox runat="server" ID="Lm2_ThisMonthMsgSent_Store" />
                            <asp:Label runat="server" AssociatedControlID="Lm2_ThisMonthMsgSent" CssClass="col-md-2 control-label">This Month Messages Sent</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="Lm2_ThisMonthMsgSent" CssClass="form-control" OnTextChanged="Lm2_ThisMonthMsgSent_TextChanged" AutoPostBack="true" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox runat="server" ID="Lm2_PeriodeStart_Store" />
                            <asp:Label runat="server" AssociatedControlID="Lm2_PeriodeStart" CssClass="col-md-2 control-label">Periode Start</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="Lm2_PeriodeStart" CssClass="form-control" OnTextChanged="Lm2_PeriodeStart_TextChanged" AutoPostBack="true" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox runat="server" ID="Lm2_PeriodeDurationInDays_Store" />
                            <asp:Label runat="server" AssociatedControlID="Lm2_PeriodeDurationInDays" CssClass="col-md-2 control-label">Periode Duration InDays</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="Lm2_PeriodeDurationInDays" CssClass="form-control" OnTextChanged="Lm2_PeriodeDurationInDays_TextChanged" AutoPostBack="true" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox runat="server" ID="Lm2_MinDelayInSeconds_Store" />
                            <asp:Label runat="server" AssociatedControlID="Lm2_MinDelayInSeconds" CssClass="col-md-2 control-label">Minimum delay</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="Lm2_MinDelayInSeconds" CssClass="form-control" OnTextChanged="Lm2_MinDelayInSeconds_TextChanged" AutoPostBack="true" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox runat="server" ID="Lm2_CostPerNumber_Store" />
                            <asp:Label runat="server" AssociatedControlID="Lm2_CostPerNumber" CssClass="col-md-2 control-label">Cost Per Number</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="Lm2_CostPerNumber" CssClass="form-control" OnTextChanged="Lm2_CostPerNumber_TextChanged" AutoPostBack="true" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox runat="server" ID="Lm2_CurrentCredit_Store" />
                            <asp:Label runat="server" AssociatedControlID="Lm2_CurrentCredit" CssClass="col-md-2 control-label">Current Credit</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="Lm2_CurrentCredit" CssClass="form-control" OnTextChanged="Lm2_CurrentCredit_TextChanged" AutoPostBack="true" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox runat="server" ID="Lm2_LevelDefinitions_Store" />
                            <asp:Label runat="server" AssociatedControlID="Lm2_LevelDefinitions" CssClass="col-md-2 control-label">Level Definitions</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="Lm2_LevelDefinitions" CssClass="form-control" OnTextChanged="Lm2_LevelDefinitions_TextChanged" AutoPostBack="true" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox runat="server" ID="Lm2_Level_Store" />
                            <asp:Label runat="server" AssociatedControlID="Lm2_Level" CssClass="col-md-2 control-label">Level</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="Lm2_Level" CssClass="form-control" OnTextChanged="Lm2_Level_TextChanged" AutoPostBack="true" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox runat="server" ID="Lm2_AutoInceremntLevel_Store" />
                            <asp:Label runat="server" AssociatedControlID="Lm2_AutoInceremntLevel" CssClass="col-md-2 control-label">AutoInceremnt Level</asp:Label>
                            <div class="col-md-10">
                                <asp:CheckBox runat="server" ID="Lm2_AutoInceremntLevel" CssClass="form-control" OnCheckedChanged="Lm2_AutoInceremntLevel_CheckedChanged" AutoPostBack="true" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox runat="server" ID="Lm2_AutoRenewMonthPayment_Store" />
                            <asp:Label runat="server" AssociatedControlID="Lm2_AutoRenewMonthPayment" CssClass="col-md-2 control-label">AutoRenew MonthPayment</asp:Label>
                            <div class="col-md-10">
                                <asp:CheckBox runat="server" ID="Lm2_AutoRenewMonthPayment" CssClass="form-control" OnCheckedChanged="Lm2_AutoRenewMonthPayment_CheckedChanged" AutoPostBack="true" />
                            </div>
                        </div>
                    </div>

                    <div runat="server" id="sectionPayAsSent" visible="false">
                        <hr />
                        <h3>sectionPayAsSent</h3>
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="Lp_LastMsgQueued" CssClass="col-md-2 control-label">Last queued message</asp:Label>
                            <div class="col-md-10">
                                <asp:Label runat="server" ID="Lp_LastMsgQueued" CssClass="form-control" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox runat="server" ID="Lp_MsgSent_Store" />
                            <asp:Label runat="server" AssociatedControlID="Lp_MsgSent" CssClass="col-md-2 control-label">Messages Sent</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="Lp_MsgSent" CssClass="form-control" OnTextChanged="Lp_MsgSent_TextChanged" AutoPostBack="true" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox runat="server" ID="Lp_MinDelayInSeconds_Store" />
                            <asp:Label runat="server" AssociatedControlID="Lp_MinDelayInSeconds" CssClass="col-md-2 control-label">Minimum delay</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="Lp_MinDelayInSeconds" CssClass="form-control" OnTextChanged="Lp_MinDelayInSeconds_TextChanged" AutoPostBack="true" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox runat="server" ID="Lp_CostPerNumber_Store" />
                            <asp:Label runat="server" AssociatedControlID="Lp_CostPerNumber" CssClass="col-md-2 control-label">Cost Per Number</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="Lp_CostPerNumber" CssClass="form-control" OnTextChanged="Lp_CostPerNumber_TextChanged" AutoPostBack="true" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox runat="server" ID="Lp_CostPerMessage_Store" />
                            <asp:Label runat="server" AssociatedControlID="Lp_CostPerMessage" CssClass="col-md-2 control-label">Cost Per Message</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="Lp_CostPerMessage" CssClass="form-control" OnTextChanged="Lp_CostPerMessage_TextChanged" AutoPostBack="true" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox runat="server" ID="Lp_CurrentCredit_Store" />
                            <asp:Label runat="server" AssociatedControlID="Lp_CurrentCredit" CssClass="col-md-2 control-label">Current Credit</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="Lp_CurrentCredit" CssClass="form-control" OnTextChanged="Lp_CurrentCredit_TextChanged" AutoPostBack="true" />
                            </div>
                        </div>
                    </div>

                    <div runat="server" id="sectionSystemDuplication" visible="false">
                        <hr />
                        <h3>sectionSystemDuplication</h3>
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="Ld_LastMsgQueued" CssClass="col-md-2 control-label">Last queued message</asp:Label>
                            <div class="col-md-10">
                                <asp:Label runat="server" ID="Ld_LastMsgQueued" CssClass="form-control" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox runat="server" ID="Ld_MsgSent_Store" />
                            <asp:Label runat="server" AssociatedControlID="Ld_MsgSent" CssClass="col-md-2 control-label">Messages Sent</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="Ld_MsgSent" CssClass="form-control" OnTextChanged="Ld_MsgSent_TextChanged" AutoPostBack="true" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:CheckBox runat="server" ID="Ld_PaidUntil_Store" />
                            <asp:Label runat="server" AssociatedControlID="Ld_PaidUntil" CssClass="col-md-2 control-label">Paid Until</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="Ld_PaidUntil" CssClass="form-control" OnTextChanged="Ld_PaidUntil_TextChanged" AutoPostBack="true" />
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </div>
    </div>
</asp:Content>

