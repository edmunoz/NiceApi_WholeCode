<%@ Page Title="Your Details" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Details.aspx.cs" Inherits="Data" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h2><%: Title %>.</h2>

    <div class="row">
        <div class="col-md-8">
            <section id="loginForm">
                <div class="form-horizontal">
                    <asp:PlaceHolder runat="server" ID="ErrorMessage" Visible="false">
                        <p class="text-danger">
                            <asp:Literal runat="server" ID="FailureText" />
                        </p>
                    </asp:PlaceHolder>
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="UserName" CssClass="col-md-2 control-label">User name</asp:Label>
                        <div class="col-md-10">
                            <asp:Label runat="server" ID="UserName" CssClass="form-control" Text="cool" />
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="UserEmail" CssClass="col-md-2 control-label">User email</asp:Label>
                        <div class="col-md-10">
                            <asp:Label runat="server" ID="UserEmail" CssClass="form-control" Text="cool" />
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="ApiGuId" CssClass="col-md-2 control-label">X-APIId</asp:Label>
                        <div class="col-md-10">
                            <asp:Label runat="server" ID="ApiGuId" CssClass="form-control" Text="cool" />
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="Status" CssClass="col-md-2 control-label">Status</asp:Label>
                        <div class="col-md-10">
                            <asp:Label runat="server" ID="Status" CssClass="form-control" Text="cool" />
                        </div>
                    </div>
                    <div runat="server" id="SectionTelNumbers" visible="false">
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="Tel1" CssClass="col-md-2 control-label">Mobile # 1</asp:Label>
                            <div class="col-md-10">
                                <asp:Label runat="server" ID="Tel1" CssClass="form-control" Text="cool" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="Tel2" CssClass="col-md-2 control-label">Mobile # 2</asp:Label>
                            <div class="col-md-10">
                                <asp:Label runat="server" ID="Tel2" CssClass="form-control" Text="cool" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="Tel3" CssClass="col-md-2 control-label">Mobile # 3</asp:Label>
                            <div class="col-md-10">
                                <asp:Label runat="server" ID="Tel3" CssClass="form-control" Text="cool" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="Tel4" CssClass="col-md-2 control-label">Mobile # 4</asp:Label>
                            <div class="col-md-10">
                                <asp:Label runat="server" ID="Tel4" CssClass="form-control" Text="cool" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="Tel5" CssClass="col-md-2 control-label">Mobile # 5</asp:Label>
                            <div class="col-md-10">
                                <asp:Label runat="server" ID="Tel5" CssClass="form-control" Text="cool" />
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </div>
    </div>
</asp:Content>
