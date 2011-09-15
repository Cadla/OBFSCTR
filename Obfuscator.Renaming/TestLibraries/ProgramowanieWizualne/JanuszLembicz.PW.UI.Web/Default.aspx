<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="JanuszLembicz.PW.UI.Web._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        #form1
        {
            height: 235px;
            width: 1062px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table>
            <tr>
                <td>
                    <table>
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <td>
                                            Product name:
                                        </td>
                                        <td>
                                            <asp:TextBox ID="ProductNameTextBox" runat="server" Style="margin-left: 0px"></asp:TextBox>
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="ProductNameTextBox"
                                                ErrorMessage="Invalid value" ValidationExpression="[a-zA-Z]*"></asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <td>
                                            Producer name:
                                        </td>
                                        <td>
                                            <asp:TextBox ID="ProducerNameTextBox" runat="server"></asp:TextBox>
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="ProducerNameTextBox"
                                                ErrorMessage="Invalid value" ValidationExpression="[a-zA-Z]*"></asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <td>
                                            Min memory capacity:
                                        </td>
                                        <td>
                                            <asp:CheckBox ID="MinMemoryCapacityCheckBox" runat="server" />
                                        </td>
                                        <td>
                                            <asp:TextBox ID="MinMemoryCapacityTextBox" runat="server"></asp:TextBox>
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="MinMemoryCapacityTextBox"
                                                ErrorMessage="Invalid value" ValidationExpression="0|[1-9]+[0-9]*"></asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <td>
                                          Max memory capacity:
                                        </td>
                                        <td>
                                            <asp:CheckBox ID="MaxMemoryCapacityCheckBox" runat="server" />
                                        </td>
                                        <td>
                                            <asp:TextBox ID="MaxMemoryCapacityTextBox" runat="server"></asp:TextBox>
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator5" runat="server" ControlToValidate="MaxMemoryCapacityTextBox"
                                                ErrorMessage="Invalid value" ValidationExpression="0|[1-9]+[0-9]*"></asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <td>
                                            <asp:Button ID="FilterButton" runat="server" Text="Filter" OnClick="FilterButton_Click" />
                                        </td>
                                        <td>
                                            <asp:Button ID="ClearButton" runat="server" Text="Clear" OnClick="ClearButton_Click" />
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <td>
                                            Get products sorted by memory capacity
                                        </td>
                                        <td>
                                            <asp:TextBox ID="SmalestTextBox" runat="server">3</asp:TextBox>
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator6" runat="server" ErrorMessage="Invalid value"
                                                ControlToValidate="SmalestTextBox" ValidationExpression="0|[1-9]+[0-9]*"></asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Button ID="SmallestButton" runat="server" Text="Go" OnClick="SmallestButton_Click" />
                            </td>
                        </tr>
                    </table>
                </td>
                <td>
                    <table>
                        <tr>
                            <td>
                                <asp:GridView ID="ProductsGridView" runat="server" AutoGenerateColumns="False" DataSourceID="ProductsObjectDataSource">
                                    <Columns>
                                        <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                                        <asp:BoundField DataField="Producer" HeaderText="Producer" SortExpression="Producer" />
                                        <asp:BoundField DataField="MemoryCapacity" HeaderText="MemoryCapacity" SortExpression="MemoryCapacity" />
                                        <asp:BoundField DataField="Warranty" HeaderText="Warranty" SortExpression="Warranty" />
                                        <asp:CheckBoxField DataField="HasDisplay" HeaderText="HasDisplay" SortExpression="HasDisplay" />
                                    </Columns>
                                </asp:GridView>
                                <asp:ObjectDataSource ID="ProductsObjectDataSource" runat="server" OnSelecting="ProductsObjectDataSource_Selecting"
                                    SelectMethod="GetFilteredProducts" TypeName="JanuszLembicz.PW.BusinessLogic">
                                    <SelectParameters>
                                        <asp:Parameter Name="filter" Type="Object" />
                                    </SelectParameters>
                                </asp:ObjectDataSource>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:GridView ID="SmalestProductsGridView" runat="server" AutoGenerateColumns="False"
                                    DataSourceID="SmalestProductsDataSource">
                                    <Columns>
                                        <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                                        <asp:BoundField DataField="Producer" HeaderText="Producer" SortExpression="Producer" />
                                        <asp:BoundField DataField="MemoryCapacity" HeaderText="MemoryCapacity" SortExpression="MemoryCapacity" />
                                        <asp:BoundField DataField="Warranty" HeaderText="Warranty" SortExpression="Warranty" />
                                        <asp:CheckBoxField DataField="HasDisplay" HeaderText="HasDisplay" SortExpression="HasDisplay" />
                                    </Columns>
                                </asp:GridView>
                                <asp:ObjectDataSource ID="SmalestProductsDataSource" runat="server" OnSelecting="SmalestProductsDataSource_Selecting"
                                    SelectMethod="GetSmallestProducts" TypeName="JanuszLembicz.PW.BusinessLogic">
                                    <SelectParameters>
                                        <asp:Parameter DefaultValue="0" Name="n" Type="Int32" />
                                    </SelectParameters>
                                </asp:ObjectDataSource>
                            </td>
                        </tr>
                    </table>
                </td>
                <td>
                    <asp:GridView ID="ProducersGridView" runat="server" AutoGenerateColumns="False" DataSourceID="PrducersDataSource">
                        <Columns>
                            <asp:BoundField DataField="ProducerID" HeaderText="ProducerID" SortExpression="ProducerID" />
                            <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="Name" />
                        </Columns>
                    </asp:GridView>
                    <asp:ObjectDataSource ID="PrducersDataSource" runat="server" SelectMethod="GetAllProducers"
                        TypeName="JanuszLembicz.PW.BusinessLogic"></asp:ObjectDataSource>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
