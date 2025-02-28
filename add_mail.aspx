<%@ Page Language="C#" AutoEventWireup="true" CodeFile="add_mail.aspx.cs" Inherits="add_mail" EnableEventValidation="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
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
    <script>
        $(document).ready(function () {
            $.fn.dataTable.ext.errMode = 'none';
            var table = $('#<%=AddMailGridView.ClientID%>').DataTable({
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
               .appendTo('#<%=AddMailGridView.ClientID%>_wrapper .col-sm-6:eq(0)');
        });
         function req_validation() {
             var ddl_unitcode = document.getElementById('<%=ddl_unitcode.ClientID %>');
            var Selected_ddl_unitcode = ddl_unitcode.options[ddl_unitcode.selectedIndex].text;
            if (Selected_ddl_unitcode == "Select") {
                alert("Please Select Branch Name");
                ddl_unitcode.focus();
                return false;
            }

            var ddl_head_type = document.getElementById('<%=ddl_head_type.ClientID %>');
            var Selected_ddl_head_type = ddl_head_type.options[ddl_head_type.selectedIndex].text;
            if (Selected_ddl_head_type == "Select") {
                alert("Please Select Head Type");
                ddl_head_type.focus();
                return false;
            }
            var txt_head = document.getElementById('<%=txt_head.ClientID %>');
             if (txt_head.value == "") {
                 alert("Please Enter Head Name");
                 txt_head.focus();
                 return false;
             }
             var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/;
             var txt_email = document.getElementById('<%=txt_email.ClientID %>');
             if (reg.test(txt_email.value) == false) {
                 alert('Please enter valid Email Address');
                 txt_email.value = "";
                 return false;
             }
            $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
            return true;


        }
        function validateEmail(emailField) {
            var reg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/;
            var txt_cc_emailid = document.getElementById('<%=txt_cc_emailid.ClientID %>');
            if (reg.test(txt_cc_emailid.value) == false) {
                alert('Invalid Email Address');
                txt_cc_emailid.value = "";
                return false;
            }



        }
        //MD change
        function isNumber(evt) {
            if (null != evt) {
                evt = (evt) ? evt : window.event;
                var charCode = (evt.which) ? evt.which : evt.keyCode;
                if (charCode > 31 && (charCode < 48 || charCode > 57)) {
                    return false;
                }
            }
            return true;
        }
        function AllowAlphabet(e) {
            isIE = document.all ? 1 : 0
            keyEntry = !isIE ? e.which : event.keyCode;
            if (((keyEntry >= '65') && (keyEntry <= '90')) || ((keyEntry >= '97') && (keyEntry <= '122')) || (keyEntry == '32') || keyEntry == '45' || (keyEntry == '8') || (keyEntry == '9') || (keyEntry == '46') || (keyEntry == '44'))
                return true;
            else {
                // alert('Please Enter Only Character values.');
                return false;
            }
        }
        function R_validation() {

            var r = confirm("Are you Sure You Want to Delete Record");
            if (r == true) {

                $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });

            }
            else {
                alert("Record not Available");
            }
            return r;
        }
    </script>
    <style>
        .grid-view {
            max-height: 400px;
            overflow-x: hidden;
            overflow-y: auto;
        }

        .row {
            margin-right: 0px;
            margin-left: 0px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <br />
        <div class="container-fluid" style="overflow-x: hidden;">
            <div class="container">
                <div class="row">
                    <div class="col-md-1 col-xs-12"></div>
                    <div class="col-md-3 col-xs-12">
                        Branch name:<span style="color: red">*</span>
                        <asp:DropDownList ID="ddl_unitcode" DataValueField="unit_code" DataTextField="unit_name" runat="server" CssClass="form-control">
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-3 col-xs-12">
                        Head type:<span style="color: red">*</span>
                        <asp:DropDownList ID="ddl_head_type" runat="server" class="form-control">
                            <asp:ListItem Value="Select">Select</asp:ListItem>
                            <asp:ListItem Value="Admin_Head">Admin_Head</asp:ListItem>
                            <asp:ListItem Value="Location_Head">Location_Head</asp:ListItem>
                            <asp:ListItem Value="Procurment_Head">Procurment_Head</asp:ListItem>
                            <asp:ListItem Value="Operation_Head">Operation_Head</asp:ListItem>
                            <asp:ListItem Value="Finance_Head">Finance_Head</asp:ListItem>
                            <asp:ListItem Value="Other_Head">Other_Head</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-3 col-xs-12">
                        Head name:<span style="color: red">*</span>
                        <asp:TextBox runat="server" CssClass="form-control" ID="txt_head" onkeypress="return AllowAlphabet(event)"></asp:TextBox>
                    </div>

                </div>
                <br />
                <div class="row">
                    <div class="col-md-1 col-xs-12"></div>
                    <div class="col-md-3 col-xs-12">
                        Email-id:<span style="color: red">*</span>
                        <asp:TextBox runat="server" CssClass="form-control" ID="txt_email"></asp:TextBox>
                    </div>
                    <div class="col-md-3 col-xs-12">
                        Cc Email-id:
                        <asp:TextBox runat="server" CssClass="form-control" ID="txt_cc_emailid" onblur="validateEmail(this);"></asp:TextBox>
                    </div>
                    <div class="col-md-3 col-xs-12">
                        Mobile No:
                        <asp:TextBox runat="server" CssClass="form-control" ID="txt_mobileno" onkeypress="return isNumber(event)" MaxLength="10"> 
      

                        </asp:TextBox>
                    </div>
                </div>
                <br />
                <div class="row text-center">
                    <asp:Button ID="btn_save" runat="server" CssClass="btn btn-primary" Text="Save" OnClick="btn_save_Click" OnClientClick="return req_validation();" />
                    <asp:Button ID="btn_update" runat="server" class="btn btn-primary" OnClick="btnupdate_Click" Text="Update" CausesValidation="False" meta:resourcekey="btndeleteResource1" OnClientClick="return req_validation();" />
                    <asp:Button ID="btndelete" runat="server" class="btn btn-primary" OnClick="btndelete_Click" Text="Delete" CausesValidation="False" meta:resourcekey="btndeleteResource1" OnClientClick="return R_validation()();" />


                    <asp:TextBox runat="server" CssClass="form-control" ID="txt_id" Visible="false"></asp:TextBox>
                </div>
            </div>

            <br />
            <br />
            <div class="container">
                <asp:Panel ID="Panel2" runat="server" CssClass="grid-view" Style="overflow-x: hidden;">

                    <asp:GridView ID="AddMailGridView" class="table" runat="server"
                        AutoGenerateColumns="False" BackColor="White" BorderColor="#000"
                        BorderStyle="None" BorderWidth="1px" CellPadding="3" OnSelectedIndexChanged="AddMailGridView_OnSelectedIndexChanged"
                        OnRowDataBound="DesignationGridView_RowDataBound"
                        Width="100%" OnPreRender="AddMailGridView_PreRender" Font-Size="X-Small">
                        <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                        <AlternatingRowStyle BackColor="White" />
                        <HeaderStyle BackColor="#337AB7" Font-Bold="True" ForeColor="White" Font-Size="X-Small" />
                        <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                        <RowStyle BackColor="#EFF3FB" />
                        <EditRowStyle BackColor="#2461BF" />
                        <FooterStyle BackColor="#337AB7" Font-Bold="True" ForeColor="White" />

                        <Columns>
                            <asp:BoundField DataField="Id" HeaderText="Client Name"
                                SortExpression="Id" />
                            <%-- <asp:BoundField DataField="client_code" HeaderText="Client Name" 
                                                SortExpression="client_code" />
                                            <asp:BoundField DataField="state" HeaderText="State" 
                                                SortExpression="state" />--%>

                            <asp:BoundField DataField="unit_code" HeaderText="Branch Name"
                                SortExpression="unit_code" />
                            <asp:BoundField DataField="head_name" HeaderText="Head Name"
                                SortExpression="head_name" />
                            <asp:BoundField DataField="head_type" HeaderText="Type"
                                SortExpression="head_type" />
                            <asp:BoundField DataField="head_email_id" HeaderText="EmailId"
                                SortExpression="head_email_id" />

                            <asp:BoundField DataField="cc_emailid" HeaderText="Cc_Email_Id"
                                SortExpression="`cc_emailid`" />
                        </Columns>
                        <FooterStyle BackColor="White" ForeColor="#000066" />
                        <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
                        <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#337AB7" Font-Bold="True" ForeColor="White" />
                    </asp:GridView>


                </asp:Panel>
            </div>
        </div>
    </form>
</body>
</html>
