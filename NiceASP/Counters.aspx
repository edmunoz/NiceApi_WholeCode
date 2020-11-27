<%@ Page Title="Counters" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Counters.aspx.cs" Inherits="Counters" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h2><%: Title %>.</h2>


    <div class="row">
        <div class="col-md-8">
            <section id="loginForm">
                <div class="form-horizontal">
                    <h4>Your account counters:</h4>
                    <hr />
                    <asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="false">
                        <p class="text-danger">
                            <asp:Literal runat="server" ID="FailureText" />
                        </p>
                    </asp:PlaceHolder>
                    <div class="form-horizontal">
                        <div runat="server" id="sectionWrong" visible="false">
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="X_Status" CssClass="col-md-2 control-label">Status</asp:Label>
                                <div class="col-md-10">
                                    <asp:Label runat="server" ID="X_Status" CssClass="form-control" />
                                </div>
                            </div>
                        </div>

                        <div runat="server" id="sectionFree" visible="false">
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="F_Tel1" CssClass="col-md-2 control-label">Mobile # 1</asp:Label>
                                <div class="col-md-10">
                                    <asp:Label runat="server" ID="F_Tel1" CssClass="form-control" />
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="F_Tel2" CssClass="col-md-2 control-label">Mobile # 2</asp:Label>
                                <div class="col-md-10">
                                    <asp:Label runat="server" ID="F_Tel2" CssClass="form-control" />
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="F_Tel3" CssClass="col-md-2 control-label">Mobile # 3</asp:Label>
                                <div class="col-md-10">
                                    <asp:Label runat="server" ID="F_Tel3" CssClass="form-control" />
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="F_Tel4" CssClass="col-md-2 control-label">Mobile # 4</asp:Label>
                                <div class="col-md-10">
                                    <asp:Label runat="server" ID="F_Tel4" CssClass="form-control" />
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="F_Tel5" CssClass="col-md-2 control-label">Mobile # 5</asp:Label>
                                <div class="col-md-10">
                                    <asp:Label runat="server" ID="F_Tel5" CssClass="form-control" />
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="F_LastMsgQueued" CssClass="col-md-2 control-label">Last queued message</asp:Label>
                                <div class="col-md-10">
                                    <asp:Label runat="server" ID="F_LastMsgQueued" CssClass="form-control" />
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="F_MsgSent" CssClass="col-md-2 control-label">Messages Sent</asp:Label>
                                <div class="col-md-10">
                                    <asp:Label runat="server" ID="F_MsgSent" CssClass="form-control" />
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="F_MsgLeft" CssClass="col-md-2 control-label">Messages Left</asp:Label>
                                <div class="col-md-10">
                                    <asp:Label runat="server" ID="F_MsgLeft" CssClass="form-control" />
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="F_MinDelayInSeconds" CssClass="col-md-2 control-label">Minimum delay</asp:Label>
                                <div class="col-md-10">
                                    <asp:Label runat="server" ID="F_MinDelayInSeconds" CssClass="form-control" />
                                </div>
                            </div>
                        </div>

                        <div runat="server" id="sectionMonthly" visible="false">
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="M_Tels" CssClass="col-md-2 control-label">Active Numbers</asp:Label>
                                <div class="col-md-10">
                                    <asp:Label runat="server" ID="M_Tels" CssClass="form-control" />
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="M_ActiveUntil" CssClass="col-md-2 control-label">Send allowed until</asp:Label>
                                <div class="col-md-10">
                                    <asp:Label runat="server" ID="M_ActiveUntil" CssClass="form-control" />
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="M_LastQueued" CssClass="col-md-2 control-label">Last message sent</asp:Label>
                                <div class="col-md-10">
                                    <asp:Label runat="server" ID="M_LastQueued" CssClass="form-control" />
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="M_CurrentCredit" CssClass="col-md-2 control-label">Current credit</asp:Label>
                                <div class="col-md-10">
                                    <asp:Label runat="server" ID="M_CurrentCredit" CssClass="form-control" />
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="M_CostPerNumber" CssClass="col-md-2 control-label">Cost per added number</asp:Label>
                                <div class="col-md-10">
                                    <asp:Label runat="server" ID="M_CostPerNumber" CssClass="form-control" />
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="M_MsgSent" CssClass="col-md-2 control-label">Messages sent</asp:Label>
                                <div class="col-md-10">
                                    <asp:Label runat="server" ID="M_MsgSent" CssClass="form-control" />
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="M_MinDelayInSeconds" CssClass="col-md-2 control-label">Minimum delay</asp:Label>
                                <div class="col-md-10">
                                    <asp:Label runat="server" ID="M_MinDelayInSeconds" CssClass="form-control" />
                                </div>
                            </div>
                        </div>

                        <div runat="server" id="sectionMonthlyDifPrice" visible="false">
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="M2_Tels" CssClass="col-md-2 control-label">Active Numbers</asp:Label>
                                <div class="col-md-10">
                                    <asp:Label runat="server" ID="M2_Tels" CssClass="form-control" />
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="M2_ActiveUntil" CssClass="col-md-2 control-label">Send allowed until</asp:Label>
                                <div class="col-md-10">
                                    <asp:Label runat="server" ID="M2_ActiveUntil" CssClass="form-control" />
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="M2_LastQueued" CssClass="col-md-2 control-label">Last message sent</asp:Label>
                                <div class="col-md-10">
                                    <asp:Label runat="server" ID="M2_LastQueued" CssClass="form-control" />
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="M2_CurrentCredit" CssClass="col-md-2 control-label">Current credit</asp:Label>
                                <div class="col-md-10">
                                    <asp:Label runat="server" ID="M2_CurrentCredit" CssClass="form-control" />
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="M2_CostPerNumber" CssClass="col-md-2 control-label">Cost per added number</asp:Label>
                                <div class="col-md-10">
                                    <asp:Label runat="server" ID="M2_CostPerNumber" CssClass="form-control" />
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="M2_TotalMsgSent" CssClass="col-md-2 control-label">Total messages sent</asp:Label>
                                <div class="col-md-10">
                                    <asp:Label runat="server" ID="M2_TotalMsgSent" CssClass="form-control" />
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="M2_MsgSentThisPeriod" CssClass="col-md-2 control-label">Messages sent this period</asp:Label>
                                <div class="col-md-10">
                                    <asp:Label runat="server" ID="M2_MsgSentThisPeriod" CssClass="form-control" />
                                </div>
                            </div>
                        </div>


                        <div runat="server" id="sectionPayAsSent" visible="false">
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="P_Tels" CssClass="col-md-2 control-label">Active Numbers</asp:Label>
                                <div class="col-md-10">
                                    <asp:Label runat="server" ID="P_Tels" CssClass="form-control" />
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="P_CurrentCredit" CssClass="col-md-2 control-label">Current credit</asp:Label>
                                <div class="col-md-10">
                                    <asp:Label runat="server" ID="P_CurrentCredit" CssClass="form-control" />
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="P_CostPerNumber" CssClass="col-md-2 control-label">Cost per added number</asp:Label>
                                <div class="col-md-10">
                                    <asp:Label runat="server" ID="P_CostPerNumber" CssClass="form-control" />
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="P_CostPerMessage" CssClass="col-md-2 control-label">Cost per message</asp:Label>
                                <div class="col-md-10">
                                    <asp:Label runat="server" ID="P_CostPerMessage" CssClass="form-control" />
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="P_LastQueued" CssClass="col-md-2 control-label">Last message sent</asp:Label>
                                <div class="col-md-10">
                                    <asp:Label runat="server" ID="P_LastQueued" CssClass="form-control" />
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="P_MsgSent" CssClass="col-md-2 control-label">Messages sent</asp:Label>
                                <div class="col-md-10">
                                    <asp:Label runat="server" ID="P_MsgSent" CssClass="form-control" />
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="P_MinDelayInSeconds" CssClass="col-md-2 control-label">Minimum delay</asp:Label>
                                <div class="col-md-10">
                                    <asp:Label runat="server" ID="P_MinDelayInSeconds" CssClass="form-control" />
                                </div>
                            </div>
                        </div>

                        <div runat="server" id="sectionSystemDuplication" visible="false">
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="D_ActiveUntil" CssClass="col-md-2 control-label">Send allowed until</asp:Label>
                                <div class="col-md-10">
                                    <asp:Label runat="server" ID="D_ActiveUntil" CssClass="form-control" />
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="D_LastQueued" CssClass="col-md-2 control-label">Last message sent</asp:Label>
                                <div class="col-md-10">
                                    <asp:Label runat="server" ID="D_LastQueued" CssClass="form-control" />
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="D_MsgSent" CssClass="col-md-2 control-label">Messages sent</asp:Label>
                                <div class="col-md-10">
                                    <asp:Label runat="server" ID="D_MsgSent" CssClass="form-control" />
                                </div>
                            </div>
                        </div>

                    </div>
                </div>
            </section>
        </div>
    </div>
</asp:Content>
