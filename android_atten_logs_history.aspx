<%@ Page Language="C#" AutoEventWireup="true" CodeFile="android_atten_logs_history.aspx.cs" Inherits="android_atten_logs_history" EnableEventValidation="false"%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1" />
    <meta charset="utf-8" />
    <script src="js/jquery-1.12.3.min.js"></script>
    <script src="Scripts/jquery-1.11.3.js"></script>
    <script src="js/bootstrap.js" type="text/javascript"></script>
    <script src="Scripts/datetimepicker.js"></script>
    <script src="Scripts/jquery-ui-1.8.20.min.js"></script>
    <script src="Scripts/jquery-ui-1.8.20.js"></script>
    <script src="Scripts/jquery-1.7.1.js"></script>
    <script src="Scripts/jquery-ui.min.js"></script>
    <script src="js/bootstrap.min.js"></script>
    <script src="js/jquery.blockUI.js"></script>
    <link href="Scripts/bootstrap.min.css" rel="stylesheet" />
    <link href="Scripts/jquery-ui.css" rel="stylesheet" />
    <link href="css/new_stylesheet.css" rel="stylesheet" />
    <link href="css/GridViewFreezeStyle.css" rel="stylesheet" type="text/css" />
    <link href="css/select2.min.css" rel="stylesheet" />
    <script src="js/select2.min.js"></script>


</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>

        <br />
        <%--<div class="row">--%>
        <div class="panel-body">
            <asp:Panel ID="Panel2" runat="server" ScrollBars="auto" class="grid-view">

                <asp:GridView ID="GradeGridView" class="table" runat="server" Font-Size="X-Small"
                    AutoGenerateColumns="False" BackColor="White" BorderColor="#CCCCCC" OnPreRender="GradeGridView_PreRender"
                    BorderStyle="None" BorderWidth="1px" CellPadding="3" OnRowDataBound="GradeGridView_RowDataBound">
                    <RowStyle ForeColor="#000066" />
                    <Columns>
                        <asp:TemplateField HeaderText="Sr No.">
                            <ItemStyle Width="20px" />
                            <ItemTemplate>
                                <%# Container.DataItemIndex+1 %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="EMP-NAME" DataField="EMP_NAME" SortExpression="EMP_NAME" />
                        <asp:BoundField HeaderText="CLIENT NAME" DataField="client_name" SortExpression="client_name" />
                        <asp:BoundField HeaderText="STATE NAME" DataField="state_name" SortExpression="state_name" />
                        <asp:BoundField HeaderText="UNIT NAME" DataField="unit_name" SortExpression="unit_name" />
                        <asp:BoundField HeaderText="ADDRESS" DataField="ADDRESS" SortExpression="ADDRESS" />
                        <asp:BoundField HeaderText="BRANCH IN-TIME" DataField="attendances_intime" SortExpression="attendances_intime" />
                        <asp:BoundField HeaderText="BRANCH OUT_TIME" DataField="attendances_outtime" SortExpression="attendances_outtime" />
                        <asp:BoundField HeaderText="OUTSIDE IN-TIME" DataField="camera_intime" SortExpression="camera_intime" />
                        <asp:BoundField HeaderText="OUTSIDE OUT-TIME" DataField="camera_outtime" SortExpression="camera_outtime" />
                        <asp:TemplateField HeaderText="IN">
                            <ItemTemplate>
                                <asp:Image ID="Camera_Image1" runat="server" Height="50" Width="50" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="OUT">
                            <ItemTemplate>
                                <asp:Image ID="Camera_Image2" runat="server" Height="50" Width="50" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="Emp Code" DataField="emp_code" SortExpression="emp_code" />

                    </Columns>
                    <FooterStyle BackColor="White" ForeColor="#000066" />
                    <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
                    <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" />
                </asp:GridView>
            </asp:Panel>
            <asp:Panel ID="pnl1" runat="server" ScrollBars="auto" class="grid-view">
                <asp:GridView ID="grd_current_location" class="table" runat="server" Font-Size="X-Small" OnPreRender="grd_location_PreRender"
                    OnSelectedIndexChanged="Location_SelectedIndexChanged"
                    AutoGenerateColumns="False" BackColor="White" BorderColor="#CCCCCC" OnRowDataBound="grd_current_location_RowDataBound"
                    BorderStyle="None" BorderWidth="1px" CellPadding="3">
                    <RowStyle ForeColor="#000066" />
                    <Columns>
                        <asp:TemplateField HeaderText="Sr No.">
                            <ItemStyle Width="20px" />
                            <ItemTemplate>
                                <%# Container.DataItemIndex+1 %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="ID" DataField="id" SortExpression="id" ItemStyle-CssClass="Hide" HeaderStyle-CssClass="Hide" />
                        <asp:BoundField HeaderText="Emp-Name" DataField="emp_code" SortExpression="emp_code" />
                        <asp:BoundField HeaderText="State-Name" DataField="state_name" SortExpression="state_name" />
                        <asp:BoundField HeaderText="Current-Latitude" DataField="cur_latitude" SortExpression="cur_latitude" />
                        <asp:BoundField HeaderText="Current-Longitude" DataField="cur_longtitude" SortExpression="cur_longtitude" />
                        <asp:BoundField HeaderText="Current-Date" DataField="cur_date" SortExpression="cur_date" />
                        <asp:BoundField HeaderText="Address" DataField="cur_address" SortExpression="cur_address" />
                    </Columns>
                    <FooterStyle BackColor="White" ForeColor="#000066" />
                    <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
                    <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" />
                </asp:GridView>

            </asp:Panel>
        </div>
        <%--</div>--%>
    </form>
</body>
</html>
