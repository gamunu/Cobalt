<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="add.ascx.cs" Inherits="Cobalt.administrator.listings.add"
    ClientIDMode="Static" %>
<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <ul id="listingTabs" class="nav nav-tabs">
            <li>
                <asp:LinkButton ID="addItem" runat="server" OnClick="addItem_Click">Add Item</asp:LinkButton></li>
            <li>
                <asp:LinkButton ID="uploadImage" runat="server" OnClick="uploadImage_Click">Upload Image</asp:LinkButton></li>
        </ul>
        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Always">
            <ContentTemplate>
                <asp:Literal ID="status" runat="server"></asp:Literal>
                <asp:MultiView ID="addListing" runat="server" ActiveViewIndex="0">
                    <asp:View ID="listingData" runat="server">
                        <h3>
                            Add Listing Item</h3>
                        <div class="control-group">
                            <asp:Label ID="lbltitile" CssClass="control-label" AssociatedControlID="txttitle"
                                runat="server" Text="Title"></asp:Label>
                            <div class="controls">
                                <asp:TextBox ID="txttitle" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="control-group">
                            <asp:Label ID="lblprice" CssClass="control-label" AssociatedControlID="txtprice"
                                runat="server" Text="Price"></asp:Label>
                            <div class="controls">
                                <asp:TextBox ID="txtprice" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="control-group">
                            <asp:Label ID="lbldescription" CssClass="control-label" AssociatedControlID="txtdescription"
                                runat="server" Text="Description"></asp:Label>
                            <div class="controls">
                                <asp:TextBox ID="txtdescription" TextMode="MultiLine" runat="server"></asp:TextBox>
                            </div>
                        </div>
                        <div class="control-group">
                            <asp:Label ID="txtcategory" CssClass="control-label" AssociatedControlID="lstcategory"
                                runat="server" Text="Category"></asp:Label>
                            <div class="controls">
                                <asp:DropDownList ID="lstcategory" runat="server">
                                </asp:DropDownList>
                            </div>
                            <div class="control-group">
                                <asp:Label ID="lblapproved" CssClass="control-label" AssociatedControlID="cbxapproved"
                                    runat="server" Text="Approved"></asp:Label>
                                <div class="controls">
                                    <asp:CheckBox ID="cbxapproved" runat="server" />
                                </div>
                            </div>
                            <div class="control-group">
                                <asp:Label ID="lblfetured" CssClass="control-label" AssociatedControlID="cbxfetured"
                                    runat="server" Text="Fetured"></asp:Label>
                                <div class="controls">
                                    <asp:CheckBox ID="cbxfetured" runat="server" />
                                </div>
                            </div>
                            <div class="control-group">
                                <asp:Label ID="lblsponsor" CssClass="control-label" AssociatedControlID="cbxsponsor"
                                    runat="server" Text="Sponsor"></asp:Label>
                                <div class="controls">
                                    <asp:CheckBox ID="cbxsponsor" runat="server" />
                                </div>
                            </div>
                            <div class="control-group">
                                <asp:Label ID="lblsell" CssClass="control-label" AssociatedControlID="cbxsell" runat="server"
                                    Text="Sell"></asp:Label>
                                <div class="controls">
                                    <asp:CheckBox ID="cbxsell" runat="server" />
                                </div>
                            </div>
                            <div class="control-group">
                                <asp:Label ID="lblclosed" CssClass="control-label" AssociatedControlID="cbxclosed"
                                    runat="server" Text="Closed"></asp:Label>
                                <div class="controls">
                                    <asp:CheckBox ID="cbxclosed" runat="server" />
                                </div>
                            </div>
                            <div class="control-group">
                                <asp:Label ID="lblcity" CssClass="control-label" AssociatedControlID="txtcity" runat="server"
                                    Text="City"></asp:Label>
                                <div class="controls">
                                    <asp:TextBox ID="txtcity" MaxLength="20" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="control-group">
                                <asp:Label ID="lblcurrencyid" CssClass="control-label" AssociatedControlID="txtcurrncyid"
                                    runat="server" Text="Currency Id"></asp:Label>
                                <div class="controls">
                                    <asp:TextBox ID="txtcurrncyid" MaxLength="3" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="control-group">
                                <asp:Label ID="lblpostalcode" CssClass="control-label" AssociatedControlID="txtpostacode"
                                    runat="server" Text="PostalCode"></asp:Label>
                                <div class="controls">
                                    <asp:TextBox ID="txtpostacode" MaxLength="20" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="control-group">
                                <asp:Label ID="lblcountryiso" CssClass="control-label" AssociatedControlID="txtcountryiso"
                                    runat="server" Text="PostalCode"></asp:Label>
                                <div class="controls">
                                    <asp:TextBox ID="txtcountryiso" MaxLength="20" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="controls">
                                <asp:Button ID="btnsubmit" CssClass="btn" runat="server" Text="Submit" OnClick="btnsubmit_Click" />
                            </div>
                    </asp:View>
                    <asp:View ID="uploadImages" runat="server">
                        <ajaxToolkit:AsyncFileUpload ID="AsyncFileUpload1" runat="server" OnUploadedComplete="AsyncFileUpload1_UploadComplete" />
                    </asp:View>
                </asp:MultiView>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="addItem" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="uploadImage" EventName="Click" />
            </Triggers>
        </asp:UpdatePanel>
    </ContentTemplate>
</asp:UpdatePanel>