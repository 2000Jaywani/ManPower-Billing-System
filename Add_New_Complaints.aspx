<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Add_New_Complaints.aspx.cs" Inherits="Add_New_Complaints" EnableEventValidation="false" %>


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
    <link href="datatable/dataTables.bootstrap.min.css" rel="stylesheet" />
    <link href="datatable/buttons.bootstrap.min.css" rel="stylesheet" />
    <script src="datatable/jquery.dataTables.min.js"></script>
    <script src="datatable/dataTables.bootstrap.min.js"></script>
    <script src="datatable/dataTables.buttons.min.js"></script>
    <script src="datatable/buttons.bootstrap.min.js"></script>
    <script src="datatable/vfs_fonts.js"></script>
    <script src="datatable/buttons.html5.min.js"></script>
    <script src="datatable/buttons.print.min.js"></script>
    <script src="datatable/buttons.colVis.min.js"></script>
    <script src="datatable/pdfmake.min.js"></script>
    <script>
        $(function () {
            var table = $('#<%=Grid_gst_rate.ClientID%>').DataTable({
                "responsive": true,
                "sPaginationType": "full_numbers",
                buttons: [
                  {
                      extend: 'csv',
                      exportOptions: {
                          columns: ':visible'
                      }
                  },
                  {
                      extend: 'print',
                      exportOptions: {
                          columns: ':visible'
                      }
                  },
                  {
                      extend: 'copyHtml5',
                      exportOptions: {
                          columns: ':visible'
                      }
                  },
                  'colvis'
                ]

            });

            table.buttons().container()
              .appendTo('#<%=Grid_gst_rate.ClientID%>_wrapper .col-sm-6:eq(0)');

        });



        function Req_validation() {

            var txt_gst_rate = document.getElementById('<%=txt_name.ClientID %>');

            var ddl_complaint = document.getElementById('<%=ddl_complaint.ClientID %>');
            var Selected_ddl_complaint = ddl_complaint.options[ddl_complaint.selectedIndex].text;

            var dll_Priority = document.getElementById('<%=dll_Priority.ClientID %>');
            var Selected_dll_Priority = dll_Priority.options[dll_Priority.selectedIndex].text;
            // Dept Code

            if (txt_gst_rate.value == "") {
                alert("Please Enter Complaints!!!");
                txt_gst_rate.focus();
                return false;
            }
            if (Selected_ddl_complaint == "Select") {
                alert("Please Select Complaint Category !!!");
                ddl_complaint.focus();
                return false;
            }
            if (Selected_dll_Priority == "Select") {
                alert("Please Select Complaint Priority !!!");
                dll_Priority.focus();
                return false;
            }
        }

        function Number_Percent(e) {
            if (null != e) {

                isIE = document.all ? 1 : 0
                keyEntry = !isIE ? e.which : e.keyCode;
                if (((keyEntry >= '48') && (keyEntry <= '57')) || (keyEntry < '31') || (keyEntry == '32') || (keyEntry == '46'))

                    return true;
                else {
                    // alert('Please Enter Only Character values.');
                    return false;
                }
            }
        }
        function AllowAlphabet_address(e) {
            if (null != e) {
                isIE = document.all ? 1 : 0
                keyEntry = !isIE ? e.which : e.keyCode;
                if (((keyEntry >= '65') && (keyEntry <= '90')) || ((keyEntry >= '97') && (keyEntry <= '122')) || (keyEntry < '31') || ((keyEntry >= '48') && (keyEntry <= '57')) ||
                    (keyEntry == '32') || (keyEntry == '38') || ((keyEntry == '39') && (keyEntry == '34')) || (keyEntry == '44') || ((keyEntry >= '45') && (keyEntry <= '47')) ||
                    (keyEntry == '58') || (keyEntry == '59') || (keyEntry == '61') || (keyEntry == '92'))
                    return true;
                else {
                    // alert('Please Enter Only Character values.');
                    return false;
                }
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container-fluid">
            <div class="row">
                <br />
                <div class="col-sm-3 col-xs-12">
                    Add Complaints:  <span style="color: red">*</span>

                    <asp:TextBox ID="txt_name" runat="server" class="form-control" onkeypress="return AllowAlphabet_address(event)"></asp:TextBox>

                </div>
                <div class="col-sm-3 col-xs-12">
                    Complaint Category :<span style="color: red">* </span>
                    <asp:DropDownList ID="ddl_complaint" runat="server" class="form-control">
                        <asp:ListItem Value="Select">Select</asp:ListItem>
                        <%--<asp:ListItem Value="TOP">TOP</asp:ListItem>--%>
                    </asp:DropDownList>
                </div>
                <div class="col-sm-3 col-xs-12">
                    Priority : <span style="color: red">*</span>
                    <asp:DropDownList ID="dll_Priority" runat="server" class="form-control">
                        <asp:ListItem Value="Select">Select</asp:ListItem>
                        <asp:ListItem Value="TOP">TOP</asp:ListItem>
                        <asp:ListItem Value="MEDIUM">MEDIUM</asp:ListItem>
                        <asp:ListItem Value="LOW">LOW</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="col-sm-1 col-xs-12" style="margin-top: 15px;">
                    <asp:Button ID="btn_save" runat="server" CssClass="btn btn-primary"
                        Text="Save" OnClientClick="return Req_validation();" OnClick="btn_save_Click" />
                </div>

            </div>
            <br />
            <br />
            <br />
            <br />
            <asp:Panel ID="Panel2" runat="server" CssClass="grid-view">

                <asp:GridView ID="Grid_gst_rate" class="table" runat="server"
                    AutoGenerateColumns="False" BackColor="White" BorderColor="#000"
                    BorderStyle="None" BorderWidth="1px" CellPadding="3"
                    Width="100%" OnPreRender="Grid_gst_rate_PreRender">
                    <RowStyle ForeColor="#000066" />

                    <Columns>

                        <asp:TemplateField>
                            <ItemStyle Width="20px" />
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkbtn_removeitem" runat="server" CausesValidation="false" OnClick="lnkbtn_removeitem_Click" Text="Delete" CssClass="btn btn-primary"></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Sr No.">
                            <ItemStyle Width="20px" />
                            <ItemTemplate>
                                <asp:Label ID="lbl_srnumber" runat="server" Text='<%# Container.DataItemIndex+1 %>' Width="20px"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="name" HeaderText="Complaint" SortExpression="name" />
                        <asp:BoundField DataField="comp_category" HeaderText="Category" SortExpression="categorycomp_category" />
                        <asp:BoundField DataField="priority" HeaderText="Priority" SortExpression="priority" />

                    </Columns>
                    <FooterStyle BackColor="White" ForeColor="#000066" />
                    <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
                    <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle BackColor="#337AB7" Font-Bold="True" ForeColor="White" />
                </asp:GridView>
            </asp:Panel>
        </div>
    </form>
</body>
</html>


