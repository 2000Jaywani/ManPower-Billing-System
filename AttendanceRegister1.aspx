<%@ Page Title="Attendance Register" Language="C#" AutoEventWireup="true" CodeFile="AttendanceRegister1.aspx.cs" Inherits="AttendanceRegister1" EnableEventValidation="false" %>

<%--<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>--%>
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

    <link href="css/GridViewFreezeStyle.css" rel="stylesheet" type="text/css" />


    <link href="datatable/dataTables.bootstrap.min.css" rel="stylesheet" />
    <link href="datatable/buttons.bootstrap.min.css" rel="stylesheet" />
    <%--<script src="datatable/jquery-1.12.3.js"></script>--%>
    <script src="datatable/jquery.dataTables.min.js"></script>
    <script src="datatable/dataTables.bootstrap.min.js"></script>
    <script src="datatable/dataTables.buttons.min.js"></script>
    <script src="datatable/buttons.bootstrap.min.js"></script>
    <%--        <script src="datatable/jszip.min.js"></script>--%>
    <%--    <script src="datatable/pdfmake.min.js"></script>--%>
    <script src="datatable/vfs_fonts.js"></script>
    <script src="datatable/buttons.html5.min.js"></script>
    <script src="datatable/buttons.print.min.js"></script>
    <script src="datatable/buttons.colVis.min.js"></script>
    <script src="datatable/pdfmake.min.js"></script>

    <script type="text/javascript">

        $(document).ready(function () {
            var table = $('#<%=gv_attendance.ClientID%>').DataTable({
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
               .appendTo('#<%=gv_attendance.ClientID%>_wrapper .col-sm-6:eq(0)');

            $.fn.dataTable.ext.errMode = 'none';

        });

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
    </script>
    <style>
        .container {
            max-width: 99%;
        }

        .grid-view {
            height: auto;
            overflow: scroll;
        }
    </style>
    <style>
        .container {
            max-width: 99%;
        }

        .label_text {
            font-size: 14px;
            font-weight: bold;
            margin-bottom: 10px;
        }


        .panel-height {
            height: 100px;
        }

        .text_box {
            margin-top: 7px;
        }

        .auto-style1 {
            color: #FFFFFF;
        }

        h2 {
            border-radius: 5px;
        }

        h5 {
            font-weight: bold;
            font-size: 15px;
        }

        .row {
            margin: 0px;
        }

        date-picker .ui-datepicker-calendar {
            display: none;
        }
    </style>

    <script type="text/javascript">

        function pageLoad() {

            $('.date-picker2').datepicker({
                changeMonth: true,
                changeYear: true,
                showButtonPanel: true,
                dateFormat: 'dd/mm/yy',
                onClose: function (dateText, inst) {
                    var month = $("#ui-datepicker-div .ui-datepicker-date .ui-datepicker-month .ui-datepicker-year :selected").val();

                }
            });

            $('.date-picker').datepicker({
                changeMonth: true,
                changeYear: true,
                showButtonPanel: true,
                dateFormat: 'mm/yy',
                onClose: function (dateText, inst) {
                    var month = $("#ui-datepicker-div .ui-datepicker-month :selected").val();
                    var year = $("#ui-datepicker-div .ui-datepicker-year :selected").val();
                    $(this).datepicker('setDate', new Date(year, month, 1));
                }
            });
        }

        window.onfocus = function () {

            $.unblockUI();

        }

        function openWindow() {

            window.open("html/AttendanceRegister.html", 'popUpWindow', 'height=500,width=600,left=100,top=100,toolbar=no,menubar=no,location=no,directories=no,scrollbars=yes, status=No');

        }
    </script>
</head>
<body>
    <form runat="server">

        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
            <ContentTemplate>
                <asp:Panel ID="Panel4" runat="server">


                    <%--<div class="row text-center">
                    <%-- <asp:TextBox ID="txt1" runat="server" Text=" P : Present, A : Absent, F : Half Day, L : Leave, W : Weekly  Off, H : Holiday"  ></asp:TextBox>--%>
                    <%-- <h5><b>P : Present, A : Absent, F : Half Day, CL : Casual Leave, PH : Paternity Leaves, ML : Maternity Leave , PL : Privilege Leave, W : Weekly  Off, H : Holiday,CO : Company Off </b></h5>
                </div>--%>

                    <%--  <div class="row text-center">
                            <h3><b>Attendance by :</b></h3>
                </div>--%>
                    <%--<div class="row" style="display: none;">
                    <div class="col-sm-2 col-xs-12">
                        Select Unit :
                        <asp:DropDownList ID="ddl_unitcode" runat="server" class="form-control">
                        </asp:DropDownList>
                    </div>
                    <div class="col-sm-2 col-xs-12">
                        <br />
                        <asp:RadioButton ID="rdbmonth" runat="server" Text="  &nbsp;&nbsp;Full Month" GroupName="radio1" />
                        <asp:RadioButton ID="rdbindividual" runat="server" Text=" &nbsp&nbspDaily" GroupName="radio1" />
                    </div>
                    <div class="col-sm-2 col-xs-12">
                        <asp:Label ID="lbl_month" runat="server" Text="Month/Year :"></asp:Label>
                        <asp:TextBox ID="txt_month" runat="server" Visible="true" class="form-control date-picker"></asp:TextBox>
                    </div>
                    <div class="col-sm-2 col-xs-12">
                        <br />
                        <asp:Button ID="btn_process" runat="server" class="btn btn-primary" OnClick="btn_process_Click"
                            OnClientClick="return Req_validation();" Text="Process" />
                        <asp:Button ID="bntclose" runat="server" class="btn btn-danger" OnClick="bntclose_Click" Text="Close" />
                    </div>

                </div>--%>
                    <br />
                    <div class="row">
                        <table class="table table-bordered">
                            <tr style="background-color: #d0cdcd; text-align: center; font-weight: bold;">
                                <td style="background-color: #fff;">Status</td>
                                <td>Present </td>
                                <td>Absent  </td>
                                <td>Half Day  </td>
                                <td>Casual Leave  </td>
                                <td>Paternity Leaves </td>
                                <td>Maternity Leave  </td>
                                <td>Privilege Leave  </td>
                                <td>Weekly Off  </td>
                                <td>Holiday </td>
                                <%--<td>Company Off    </td>--%>
                            </tr>
                            <tr style="text-align: center; font-weight: bold;">
                                <td style="background-color: #fff;">Symbol</td>
                                <td style="background-color: LimeGreen">P</td>
                                <td style="background-color: red;">A</td>
                                <td style="background-color: orange;">HD</td>
                                <td style="background-color: yellow;">CL</td>
                                <td style="background-color: Aqua;">PH</td>
                                <td style="background-color: Wheat;">ML</td>
                                <td style="background-color: YellowGreen;">PL</td>
                                <td style="background-color: Violet;">W</td>
                                <td style="background-color: Pink;">H</td>
                                <%--<td style="background-color: Peru;">CO</td>--%>
                            </tr>

                        </table>
                    </div>
                    <%--<asp:Panel ID="Panel3" runat="server">
                <div class="row">
                    <div class="col-sm-2 col-xs-12">
                         <asp:Label ID="lbl_date" runat="server" Text="Select Date :"></asp:Label>
                         <asp:TextBox ID="txt_date" runat="server" CssClass="form-control date-picker2"></asp:TextBox>
                    </div>
                    <div class="col-sm-2 col-xs-12">
                        <asp:Label ID="lbl_status" runat="server" Text="Select Status"></asp:Label>
                                    <asp:DropDownList ID="ddl_status" runat="server" class="form-control" OnSelectedIndexChanged="ddl_status_SelectedIndexChanged">
                                        <asp:ListItem>Select Status</asp:ListItem>
                                        <asp:ListItem>Present</asp:ListItem>
                                        <asp:ListItem>Absent</asp:ListItem>
                                        <asp:ListItem>Half Day</asp:ListItem>
                                        <asp:ListItem>Cl Leave</asp:ListItem>
                                        <asp:ListItem>Pl Leave</asp:ListItem>
                                        <asp:ListItem>Ml Leave</asp:ListItem>
                                        <asp:ListItem>Previlage Leave</asp:ListItem>
                                        <asp:ListItem>Leave</asp:ListItem>
                                        <asp:ListItem>Weekly Off</asp:ListItem>
                                        <asp:ListItem>Holiday</asp:ListItem>
                                        <asp:ListItem>Company Off</asp:ListItem>
                                    </asp:DropDownList>
                    </div>
                    <div class="col-sm-1 col-xs-12"><br />
                          <asp:Button ID="btn_add" runat="server" class="btn btn-primary" OnClientClick="return add_validate();" 
                                      OnClick="btn_add_Click" Text="Add" />
                    </div>
                     <div class="col-sm-2 col-xs-12"><br />
                          <asp:FileUpload ID="FileUpload1" runat="server" Visible="false" CssClass="text_box" />
                    </div>
                    <div class="col-sm-4 col-xs-12"><br />
                          <asp:Button ID="btn_upload" runat="server" CssClass="btn btn-primary" OnClick="btn_Upload_Click" 
                              Text="Upload" Visible="false" />
                        
                          <asp:Button ID="btn_Export" runat="server" CssClass="btn btn-primary" OnClick="btn_Export_CheckedChanged"
                                  Text="Export Format" Visible="false" />
                          
                    </div>
                </div>
             
                   <br />
                   <div class="row">
                       <div class="col-sm-12 col-xs-12 text-center">
                             <asp:Button ID="btn_save" runat="server" class="btn btn-primary" OnClick="btn_save_Click" 
                            OnClientClick="return Req_validation();" Text="Save" />
                           <asp:Button ID="btn_print" runat="server" class="btn btn-primary" OnClick="btn_print_Click" 
                                 Text="Excel Output" />
                       </div>
                      
                   </div>
                          
                          
                    </asp:Panel>--%>

                    <%--&nbsp&nbsp;<asp:Label ID="Label1" runat="server" Text="Total Present Day : "></asp:Label>--%>
                    <%--<div class="row ">--%>
                    <%--<div class="col-sm-1"></div>
              
                       
                         
                        <div class="row text-center">
                            <div class="col-sm-3">
                                <asp:Label ID="lbl_month_year" runat="server" Font-Bold="True" Font-Size="Small" Visible="false"></asp:Label>
                            </div>
                            <div class="col-sm-6">
                                <h2>
                                <asp:Label ID="lblheader" runat="server" Font-Bold="True" Font-Size="Large"></asp:Label>
                                </h2>
                            </div>
                            <div class="col-sm-3">
                                <asp:Label ID="lbl_units" runat="server" Font-Bold="True" Font-Size="Small" Visible="false"></asp:Label>
                            </div>
                        </div>--%>

                    <%--  &nbsp&nbsp;<asp:Label ID="Label2" runat="server" Text="Total Present Day : "></asp:Label>
                        <asp:Label ID="lbl_totpresent" runat="server"></asp:Label>
                    --%>
                    <%--</div>--%>
                    <%-- <asp:Panel ID="Panel4" runat="server" Visible="False">
                        <table>
                           <tr>
                                <td bgcolor="#5D7B9D" style="color: #FFFFFF">
                                    SR NO&nbsp;&nbsp; EMP CODE&nbsp; EMP NAME&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 01&nbsp;&nbsp; 02&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 03&nbsp;&nbsp;&nbsp;&nbsp; 04&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 05&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 06&nbsp;&nbsp;&nbsp;&nbsp; 07&nbsp;&nbsp;&nbsp;&nbsp; 08&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 09&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 10&nbsp;&nbsp;&nbsp;&nbsp; 11&nbsp;&nbsp;&nbsp;&nbsp; 12&nbsp;&nbsp;&nbsp; 13&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 14&nbsp;&nbsp;&nbsp;&nbsp; 15&nbsp;&nbsp;&nbsp; 16&nbsp;&nbsp;&nbsp;&nbsp; 17&nbsp;&nbsp;&nbsp; 18&nbsp;&nbsp;&nbsp; 19&nbsp;&nbsp;&nbsp;&nbsp; 20&nbsp;&nbsp;&nbsp;&nbsp; 21&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 22&nbsp;&nbsp;&nbsp;&nbsp; 23&nbsp;&nbsp;&nbsp;&nbsp; 24&nbsp;&nbsp; &nbsp; 25&nbsp;&nbsp;&nbsp;&nbsp; 26&nbsp;&nbsp;&nbsp;&nbsp; 27&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; 28&nbsp;&nbsp;&nbsp;&nbsp; 29&nbsp;&nbsp; &nbsp;&nbsp; 30&nbsp;&nbsp;&nbsp; &nbsp; 31&nbsp;&nbsp;PDAYS&nbsp;&nbsp; ADAYS&nbsp; LEAVE&nbsp;&nbsp;
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>--%>

                    <asp:Panel ID="Panel1" runat="server" ScrollBars="Auto" CssClass="grid-view" class="panel-body">

                        <asp:GridView ID="gv_attendance" runat="server" class="table" AutoGenerateColumns="False" CellPadding="4" ForeColor="#333333" OnPreRender="gv_attendance_PreRender" OnRowDataBound="shiftcalendar_RowDataBound" DataKeyNames="emp_code">
                            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" Wrap="True" />
                            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                            <RowStyle BackColor="#EFF3FB" />
                            <EditRowStyle BackColor="#2461BF" />

                            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                            <SortedAscendingCellStyle BackColor="#E9E7E2" />
                            <SortedAscendingHeaderStyle BackColor="#506C8C" />
                            <SortedDescendingCellStyle BackColor="#FFFDF8" />
                            <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
                            <Columns>
                                <asp:TemplateField HeaderText="Sr. No.">
                                    <ItemTemplate>
                                        <asp:Label ID="lbl_srnumber" runat="server" Text="<%# Container.DataItemIndex+1 %>"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField ControlStyle-Width="45px" HeaderText="Employee Code">
                                    <ItemTemplate>
                                        <asp:Label ID="lblempcode" runat="server" Text='<%# Eval("EMP_CODE")%>'></asp:Label>
                                    </ItemTemplate>
                                    <ControlStyle Width="50px" />
                                </asp:TemplateField>
                                <asp:BoundField DataField="EMP_NAME" HeaderText="Employee Name" SortExpression="EMP_NAME" />
                                <asp:BoundField DataField="TOT_DAYS_PRESENT" HeaderText="Present Days" SortExpression="TOT_DAYS_PRESENT" />
                                <asp:BoundField DataField="TOT_DAYS_ABSENT" HeaderText="Absent Days" SortExpression="TOT_DAYS_ABSENT" />
                                <asp:BoundField DataField="TOT_LEAVES" HeaderText="Leaves" SortExpression="TOT_LEAVES" />
                                <asp:BoundField DataField="WEEKLY_OFF" HeaderText="Weeks Off" SortExpression="WEEKLY_OFF" />
                                <asp:BoundField DataField="HOLIDAYS" HeaderText="Holidays" SortExpression="HOLIDAYS" />
                                <asp:BoundField DataField="TOT_WORKING_DAYS" HeaderText="Working Days" SortExpression="TOT_WORKING_DAYS" />

                                <asp:TemplateField HeaderStyle-BackColor="#0066FF" HeaderText="OT Hours">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txt_ot_hours" runat="server" Text='<%# Eval("ot_hours")%>' Width="50" onkeypress="return isNumber(event)" OnTextChanged="shiftcalendar_SelectedIndexChanged" AutoPostBack="true"></asp:TextBox>
                                    </ItemTemplate>
                                    <HeaderStyle BackColor="#0066FF" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="id1">
                                    <ItemTemplate>
                                        <asp:Label ID="lblid1" runat="server" Text='<%# Eval("DAY01") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="id2">
                                    <ItemTemplate>
                                        <asp:Label ID="lblid2" runat="server" Text='<%# Eval("DAY02") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="id3">
                                    <ItemTemplate>
                                        <asp:Label ID="lblid3" runat="server" Text='<%# Eval("DAY03") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="id4">
                                    <ItemTemplate>
                                        <asp:Label ID="lblid4" runat="server" Text='<%# Eval("DAY04") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="id5">
                                    <ItemTemplate>
                                        <asp:Label ID="lblid5" runat="server" Text='<%# Eval("DAY05") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="id6">
                                    <ItemTemplate>
                                        <asp:Label ID="lblid6" runat="server" Text='<%# Eval("DAY06") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="id7">
                                    <ItemTemplate>
                                        <asp:Label ID="lblid7" runat="server" Text='<%# Eval("DAY07") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="id8">
                                    <ItemTemplate>
                                        <asp:Label ID="lblid8" runat="server" Text='<%# Eval("DAY08") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="id9">
                                    <ItemTemplate>
                                        <asp:Label ID="lblid9" runat="server" Text='<%# Eval("DAY09") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="id10">
                                    <ItemTemplate>
                                        <asp:Label ID="lblid10" runat="server" Text='<%# Eval("DAY10") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="id11">
                                    <ItemTemplate>
                                        <asp:Label ID="lblid11" runat="server" Text='<%# Eval("DAY11") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="id12">
                                    <ItemTemplate>
                                        <asp:Label ID="lblid12" runat="server" Text='<%# Eval("DAY12") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="id13">
                                    <ItemTemplate>
                                        <asp:Label ID="lblid13" runat="server" Text='<%# Eval("DAY13") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="id14">
                                    <ItemTemplate>
                                        <asp:Label ID="lblid14" runat="server" Text='<%# Eval("DAY14") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="id15">
                                    <ItemTemplate>
                                        <asp:Label ID="lblid15" runat="server" Text='<%# Eval("DAY15") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="id16">
                                    <ItemTemplate>
                                        <asp:Label ID="lblid16" runat="server" Text='<%# Eval("DAY16") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="id17">
                                    <ItemTemplate>
                                        <asp:Label ID="lblid17" runat="server" Text='<%# Eval("DAY17") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="id18">
                                    <ItemTemplate>
                                        <asp:Label ID="lblid18" runat="server" Text='<%# Eval("DAY18") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="id19">
                                    <ItemTemplate>
                                        <asp:Label ID="lblid19" runat="server" Text='<%# Eval("DAY19") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="id20">
                                    <ItemTemplate>
                                        <asp:Label ID="lblid20" runat="server" Text='<%# Eval("DAY20") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="id21">
                                    <ItemTemplate>
                                        <asp:Label ID="lblid21" runat="server" Text='<%# Eval("DAY21") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="id22">
                                    <ItemTemplate>
                                        <asp:Label ID="lblid22" runat="server" Text='<%# Eval("DAY22") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="id23">
                                    <ItemTemplate>
                                        <asp:Label ID="lblid23" runat="server" Text='<%# Eval("DAY23") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="id24">
                                    <ItemTemplate>
                                        <asp:Label ID="lblid24" runat="server" Text='<%# Eval("DAY24") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="id25">
                                    <ItemTemplate>
                                        <asp:Label ID="lblid25" runat="server" Text='<%# Eval("DAY25") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="id26">
                                    <ItemTemplate>
                                        <asp:Label ID="lblid26" runat="server" Text='<%# Eval("DAY26") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="id27">
                                    <ItemTemplate>
                                        <asp:Label ID="lblid27" runat="server" Text='<%# Eval("DAY27") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="id28">
                                    <ItemTemplate>
                                        <asp:Label ID="lblid28" runat="server" Text='<%# Eval("DAY28") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="id29">
                                    <ItemTemplate>
                                        <asp:Label ID="lblid29" runat="server" Text='<%# Eval("DAY29") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="id30">
                                    <ItemTemplate>
                                        <asp:Label ID="lblid30" runat="server" Text='<%# Eval("DAY30") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="id31">
                                    <ItemTemplate>
                                        <asp:Label ID="lblid31" runat="server" Text='<%# Eval("DAY31") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="1">
                                    <ItemTemplate>
                                        <asp:DropDownList AppendDataBoundItems="true" ID="DropDownList1" runat="server" SelectedValue='<%# Bind("DAY01") %>' OnSelectedIndexChanged="shiftcalendar_SelectedIndexChanged" AutoPostBack="true">
                                            <asp:ListItem Value="0" Text="Set" />
                                            <asp:ListItem Value="P" Text="P" />
                                            <asp:ListItem Value="A" Text="A" />
                                            <asp:ListItem Value="HD" Text="HD" />
                                            <asp:ListItem Value="CL" Text="CL" />
                                            <asp:ListItem Value="PH" Text="PH" />
                                            <asp:ListItem Value="ML" Text="ML" />
                                            <asp:ListItem Value="PL" Text="PL" />
                                            <asp:ListItem Value="W" Text="W" />
                                            <asp:ListItem Value="H" Text="H" />
                                            <%--<asp:ListItem Value="CO" Text="CO" />--%>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="2">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="DropDownList2" runat="server" SelectedValue='<%# Bind("DAY02") %>' OnSelectedIndexChanged="shiftcalendar_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true">
                                            <asp:ListItem Value="0" Text="Set" />
                                            <asp:ListItem Value="P" Text="P" />
                                            <asp:ListItem Value="A" Text="A" />
                                            <asp:ListItem Value="HD" Text="HD" />
                                            <asp:ListItem Value="CL" Text="CL" />
                                            <asp:ListItem Value="PH" Text="PH" />
                                            <asp:ListItem Value="ML" Text="ML" />
                                            <asp:ListItem Value="PL" Text="PL" />
                                            <asp:ListItem Value="W" Text="W" />
                                            <asp:ListItem Value="H" Text="H" />
                                            <%--<asp:ListItem Value="CO" Text="CO" />--%>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="3">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="DropDownList3" runat="server" SelectedValue='<%# Bind("DAY03") %>' OnSelectedIndexChanged="shiftcalendar_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true">
                                            <asp:ListItem Value="0" Text="Set" />
                                            <asp:ListItem Value="P" Text="P" />
                                            <asp:ListItem Value="A" Text="A" />
                                            <asp:ListItem Value="HD" Text="HD" />
                                            <asp:ListItem Value="CL" Text="CL" />
                                            <asp:ListItem Value="PH" Text="PH" />
                                            <asp:ListItem Value="ML" Text="ML" />
                                            <asp:ListItem Value="PL" Text="PL" />
                                            <asp:ListItem Value="W" Text="W" />
                                            <asp:ListItem Value="H" Text="H" />
                                            <%--<asp:ListItem Value="CO" Text="CO" />--%>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="4">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="DropDownList4" runat="server" SelectedValue='<%# Bind("DAY04") %>' OnSelectedIndexChanged="shiftcalendar_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true">
                                            <asp:ListItem Value="0" Text="Set" />
                                            <asp:ListItem Value="P" Text="P" />
                                            <asp:ListItem Value="A" Text="A" />
                                            <asp:ListItem Value="HD" Text="HD" />
                                            <asp:ListItem Value="CL" Text="CL" />
                                            <asp:ListItem Value="PH" Text="PH" />
                                            <asp:ListItem Value="ML" Text="ML" />
                                            <asp:ListItem Value="PL" Text="PL" />
                                            <asp:ListItem Value="W" Text="W" />
                                            <asp:ListItem Value="H" Text="H" />
                                            <%--<asp:ListItem Value="CO" Text="CO" />--%>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="5">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="DropDownList5" runat="server" SelectedValue='<%# Bind("DAY05") %>' OnSelectedIndexChanged="shiftcalendar_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true">
                                            <asp:ListItem Value="0" Text="Set" />
                                            <asp:ListItem Value="P" Text="P" />
                                            <asp:ListItem Value="A" Text="A" />
                                            <asp:ListItem Value="HD" Text="HD" />
                                            <asp:ListItem Value="CL" Text="CL" />
                                            <asp:ListItem Value="PH" Text="PH" />
                                            <asp:ListItem Value="ML" Text="ML" />
                                            <asp:ListItem Value="PL" Text="PL" />
                                            <asp:ListItem Value="W" Text="W" />
                                            <asp:ListItem Value="H" Text="H" />
                                            <%--<asp:ListItem Value="CO" Text="CO" />--%>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="6">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="DropDownList6" runat="server" SelectedValue='<%# Bind("DAY06") %>' OnSelectedIndexChanged="shiftcalendar_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true">
                                            <asp:ListItem Value="0" Text="Set" />
                                            <asp:ListItem Value="P" Text="P" />
                                            <asp:ListItem Value="A" Text="A" />
                                            <asp:ListItem Value="HD" Text="HD" />
                                            <asp:ListItem Value="CL" Text="CL" />
                                            <asp:ListItem Value="PH" Text="PH" />
                                            <asp:ListItem Value="ML" Text="ML" />
                                            <asp:ListItem Value="PL" Text="PL" />
                                            <asp:ListItem Value="W" Text="W" />
                                            <asp:ListItem Value="H" Text="H" />
                                            <%--<asp:ListItem Value="CO" Text="CO" />--%>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>


                                <asp:TemplateField HeaderText="7">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="DropDownList7" runat="server" SelectedValue='<%# Bind("DAY07") %>' OnSelectedIndexChanged="shiftcalendar_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true">
                                            <asp:ListItem Value="0" Text="Set" />
                                            <asp:ListItem Value="P" Text="P" />
                                            <asp:ListItem Value="A" Text="A" />
                                            <asp:ListItem Value="HD" Text="HD" />
                                            <asp:ListItem Value="CL" Text="CL" />
                                            <asp:ListItem Value="PH" Text="PH" />
                                            <asp:ListItem Value="ML" Text="ML" />
                                            <asp:ListItem Value="PL" Text="PL" />
                                            <asp:ListItem Value="W" Text="W" />
                                            <asp:ListItem Value="H" Text="H" />
                                            <%--<asp:ListItem Value="CO" Text="CO" />--%>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="8">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="DropDownList8" runat="server" SelectedValue='<%# Bind("DAY08") %>' OnSelectedIndexChanged="shiftcalendar_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true">
                                            <asp:ListItem Value="0" Text="Set" />
                                            <asp:ListItem Value="P" Text="P" />
                                            <asp:ListItem Value="A" Text="A" />
                                            <asp:ListItem Value="HD" Text="HD" />
                                            <asp:ListItem Value="CL" Text="CL" />
                                            <asp:ListItem Value="PH" Text="PH" />
                                            <asp:ListItem Value="ML" Text="ML" />
                                            <asp:ListItem Value="PL" Text="PL" />
                                            <asp:ListItem Value="W" Text="W" />
                                            <asp:ListItem Value="H" Text="H" />
                                            <%--<asp:ListItem Value="CO" Text="CO" />--%>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="9">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="DropDownList9" runat="server" SelectedValue='<%# Bind("DAY09") %>' OnSelectedIndexChanged="shiftcalendar_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true">
                                            <asp:ListItem Value="0" Text="Set" />
                                            <asp:ListItem Value="P" Text="P" />
                                            <asp:ListItem Value="A" Text="A" />
                                            <asp:ListItem Value="HD" Text="HD" />
                                            <asp:ListItem Value="CL" Text="CL" />
                                            <asp:ListItem Value="PH" Text="PH" />
                                            <asp:ListItem Value="ML" Text="ML" />
                                            <asp:ListItem Value="PL" Text="PL" />
                                            <asp:ListItem Value="W" Text="W" />
                                            <asp:ListItem Value="H" Text="H" />
                                            <%--<asp:ListItem Value="CO" Text="CO" />--%>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="10">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="DropDownList10" runat="server" SelectedValue='<%# Bind("DAY10") %>' OnSelectedIndexChanged="shiftcalendar_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true">
                                            <asp:ListItem Value="0" Text="Set" />
                                            <asp:ListItem Value="P" Text="P" />
                                            <asp:ListItem Value="A" Text="A" />
                                            <asp:ListItem Value="HD" Text="HD" />
                                            <asp:ListItem Value="CL" Text="CL" />
                                            <asp:ListItem Value="PH" Text="PH" />
                                            <asp:ListItem Value="ML" Text="ML" />
                                            <asp:ListItem Value="PL" Text="PL" />
                                            <asp:ListItem Value="W" Text="W" />
                                            <asp:ListItem Value="H" Text="H" />
                                            <%--<asp:ListItem Value="CO" Text="CO" />--%>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="11">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="DropDownList11" runat="server" SelectedValue='<%# Bind("DAY11") %>' OnSelectedIndexChanged="shiftcalendar_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true">
                                            <asp:ListItem Value="0" Text="Set" />
                                            <asp:ListItem Value="P" Text="P" />
                                            <asp:ListItem Value="A" Text="A" />
                                            <asp:ListItem Value="HD" Text="HD" />
                                            <asp:ListItem Value="CL" Text="CL" />
                                            <asp:ListItem Value="PH" Text="PH" />
                                            <asp:ListItem Value="ML" Text="ML" />
                                            <asp:ListItem Value="PL" Text="PL" />
                                            <asp:ListItem Value="W" Text="W" />
                                            <asp:ListItem Value="H" Text="H" />
                                            <%--<asp:ListItem Value="CO" Text="CO" />--%>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="12">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="DropDownList12" runat="server" SelectedValue='<%# Bind("DAY12") %>' OnSelectedIndexChanged="shiftcalendar_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true">
                                            <asp:ListItem Value="0" Text="Set" />
                                            <asp:ListItem Value="P" Text="P" />
                                            <asp:ListItem Value="A" Text="A" />
                                            <asp:ListItem Value="HD" Text="HD" />
                                            <asp:ListItem Value="CL" Text="CL" />
                                            <asp:ListItem Value="PH" Text="PH" />
                                            <asp:ListItem Value="ML" Text="ML" />
                                            <asp:ListItem Value="PL" Text="PL" />
                                            <asp:ListItem Value="W" Text="W" />
                                            <asp:ListItem Value="H" Text="H" />
                                            <%--<asp:ListItem Value="CO" Text="CO" />--%>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="13">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="DropDownList13" runat="server" SelectedValue='<%# Bind("DAY13") %>' OnSelectedIndexChanged="shiftcalendar_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true">
                                            <asp:ListItem Value="0" Text="Set" />
                                            <asp:ListItem Value="P" Text="P" />
                                            <asp:ListItem Value="A" Text="A" />
                                            <asp:ListItem Value="HD" Text="HD" />
                                            <asp:ListItem Value="CL" Text="CL" />
                                            <asp:ListItem Value="PH" Text="PH" />
                                            <asp:ListItem Value="ML" Text="ML" />
                                            <asp:ListItem Value="PL" Text="PL" />
                                            <asp:ListItem Value="W" Text="W" />
                                            <asp:ListItem Value="H" Text="H" />
                                            <%--<asp:ListItem Value="CO" Text="CO" />--%>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="14">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="DropDownList14" runat="server" SelectedValue='<%# Bind("DAY14") %>' OnSelectedIndexChanged="shiftcalendar_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true">
                                            <asp:ListItem Value="0" Text="Set" />
                                            <asp:ListItem Value="P" Text="P" />
                                            <asp:ListItem Value="A" Text="A" />
                                            <asp:ListItem Value="HD" Text="HD" />
                                            <asp:ListItem Value="CL" Text="CL" />
                                            <asp:ListItem Value="PH" Text="PH" />
                                            <asp:ListItem Value="ML" Text="ML" />
                                            <asp:ListItem Value="PL" Text="PL" />
                                            <asp:ListItem Value="W" Text="W" />
                                            <asp:ListItem Value="H" Text="H" />
                                            <%--<asp:ListItem Value="CO" Text="CO" />--%>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="15">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="DropDownList15" runat="server" SelectedValue='<%# Bind("DAY15") %>' OnSelectedIndexChanged="shiftcalendar_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true">
                                            <asp:ListItem Value="0" Text="Set" />
                                            <asp:ListItem Value="P" Text="P" />
                                            <asp:ListItem Value="A" Text="A" />
                                            <asp:ListItem Value="HD" Text="HD" />
                                            <asp:ListItem Value="CL" Text="CL" />
                                            <asp:ListItem Value="PH" Text="PH" />
                                            <asp:ListItem Value="ML" Text="ML" />
                                            <asp:ListItem Value="PL" Text="PL" />
                                            <asp:ListItem Value="W" Text="W" />
                                            <asp:ListItem Value="H" Text="H" />
                                            <%--<asp:ListItem Value="CO" Text="CO" />--%>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="16">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="DropDownList16" runat="server" SelectedValue='<%# Bind("DAY16") %>' OnSelectedIndexChanged="shiftcalendar_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true">
                                            <asp:ListItem Value="0" Text="Set" />
                                            <asp:ListItem Value="P" Text="P" />
                                            <asp:ListItem Value="A" Text="A" />
                                            <asp:ListItem Value="HD" Text="HD" />
                                            <asp:ListItem Value="CL" Text="CL" />
                                            <asp:ListItem Value="PH" Text="PH" />
                                            <asp:ListItem Value="ML" Text="ML" />
                                            <asp:ListItem Value="PL" Text="PL" />
                                            <asp:ListItem Value="W" Text="W" />
                                            <asp:ListItem Value="H" Text="H" />
                                            <%--<asp:ListItem Value="CO" Text="CO" />--%>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="17">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="DropDownList17" runat="server" SelectedValue='<%# Bind("DAY17") %>' OnSelectedIndexChanged="shiftcalendar_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true">
                                            <asp:ListItem Value="0" Text="Set" />
                                            <asp:ListItem Value="P" Text="P" />
                                            <asp:ListItem Value="A" Text="A" />
                                            <asp:ListItem Value="HD" Text="HD" />
                                            <asp:ListItem Value="CL" Text="CL" />
                                            <asp:ListItem Value="PH" Text="PH" />
                                            <asp:ListItem Value="ML" Text="ML" />
                                            <asp:ListItem Value="PL" Text="PL" />
                                            <asp:ListItem Value="W" Text="W" />
                                            <asp:ListItem Value="H" Text="H" />
                                            <%--<asp:ListItem Value="CO" Text="CO" />--%>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="18">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="DropDownList18" runat="server" SelectedValue='<%# Bind("DAY18") %>' OnSelectedIndexChanged="shiftcalendar_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true">
                                            <asp:ListItem Value="0" Text="Set" />
                                            <asp:ListItem Value="P" Text="P" />
                                            <asp:ListItem Value="A" Text="A" />
                                            <asp:ListItem Value="HD" Text="HD" />
                                            <asp:ListItem Value="CL" Text="CL" />
                                            <asp:ListItem Value="PH" Text="PH" />
                                            <asp:ListItem Value="ML" Text="ML" />
                                            <asp:ListItem Value="PL" Text="PL" />
                                            <asp:ListItem Value="W" Text="W" />
                                            <asp:ListItem Value="H" Text="H" />
                                            <%--<asp:ListItem Value="CO" Text="CO" />--%>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="19">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="DropDownList19" runat="server" SelectedValue='<%# Bind("DAY19") %>' OnSelectedIndexChanged="shiftcalendar_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true">
                                            <asp:ListItem Value="0" Text="Set" />
                                            <asp:ListItem Value="P" Text="P" />
                                            <asp:ListItem Value="A" Text="A" />
                                            <asp:ListItem Value="HD" Text="HD" />
                                            <asp:ListItem Value="CL" Text="CL" />
                                            <asp:ListItem Value="PH" Text="PH" />
                                            <asp:ListItem Value="ML" Text="ML" />
                                            <asp:ListItem Value="PL" Text="PL" />
                                            <asp:ListItem Value="W" Text="W" />
                                            <asp:ListItem Value="H" Text="H" />
                                            <%--<asp:ListItem Value="CO" Text="CO" />--%>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="20">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="DropDownList20" runat="server" SelectedValue='<%# Bind("DAY20") %>' OnSelectedIndexChanged="shiftcalendar_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true">
                                            <asp:ListItem Value="0" Text="Set" />
                                            <asp:ListItem Value="P" Text="P" />
                                            <asp:ListItem Value="A" Text="A" />
                                            <asp:ListItem Value="HD" Text="HD" />
                                            <asp:ListItem Value="CL" Text="CL" />
                                            <asp:ListItem Value="PH" Text="PH" />
                                            <asp:ListItem Value="ML" Text="ML" />
                                            <asp:ListItem Value="PL" Text="PL" />
                                            <asp:ListItem Value="W" Text="W" />
                                            <asp:ListItem Value="H" Text="H" />
                                            <%--<asp:ListItem Value="CO" Text="CO" />--%>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="21">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="DropDownList21" runat="server" SelectedValue='<%# Bind("DAY21") %>' OnSelectedIndexChanged="shiftcalendar_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true">
                                            <asp:ListItem Value="0" Text="Set" />
                                            <asp:ListItem Value="P" Text="P" />
                                            <asp:ListItem Value="A" Text="A" />
                                            <asp:ListItem Value="HD" Text="HD" />
                                            <asp:ListItem Value="CL" Text="CL" />
                                            <asp:ListItem Value="PH" Text="PH" />
                                            <asp:ListItem Value="ML" Text="ML" />
                                            <asp:ListItem Value="PL" Text="PL" />
                                            <asp:ListItem Value="W" Text="W" />
                                            <asp:ListItem Value="H" Text="H" />
                                            <%--<asp:ListItem Value="CO" Text="CO" />--%>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="22">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="DropDownList22" runat="server" SelectedValue='<%# Bind("DAY22") %>' OnSelectedIndexChanged="shiftcalendar_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true">
                                            <asp:ListItem Value="0" Text="Set" />
                                            <asp:ListItem Value="P" Text="P" />
                                            <asp:ListItem Value="A" Text="A" />
                                            <asp:ListItem Value="HD" Text="HD" />
                                            <asp:ListItem Value="CL" Text="CL" />
                                            <asp:ListItem Value="PH" Text="PH" />
                                            <asp:ListItem Value="ML" Text="ML" />
                                            <asp:ListItem Value="PL" Text="PL" />
                                            <asp:ListItem Value="W" Text="W" />
                                            <asp:ListItem Value="H" Text="H" />
                                            <%--<asp:ListItem Value="CO" Text="CO" />--%>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="23">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="DropDownList23" runat="server" SelectedValue='<%# Bind("DAY23") %>' OnSelectedIndexChanged="shiftcalendar_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true">
                                            <asp:ListItem Value="0" Text="Set" />
                                            <asp:ListItem Value="P" Text="P" />
                                            <asp:ListItem Value="A" Text="A" />
                                            <asp:ListItem Value="HD" Text="HD" />
                                            <asp:ListItem Value="CL" Text="CL" />
                                            <asp:ListItem Value="PH" Text="PH" />
                                            <asp:ListItem Value="ML" Text="ML" />
                                            <asp:ListItem Value="PL" Text="PL" />
                                            <asp:ListItem Value="W" Text="W" />
                                            <asp:ListItem Value="H" Text="H" />
                                            <%--<asp:ListItem Value="CO" Text="CO" />--%>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="24">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="DropDownList24" runat="server" SelectedValue='<%# Bind("DAY24") %>' OnSelectedIndexChanged="shiftcalendar_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true">
                                            <asp:ListItem Value="0" Text="Set" />
                                            <asp:ListItem Value="P" Text="P" />
                                            <asp:ListItem Value="A" Text="A" />
                                            <asp:ListItem Value="HD" Text="HD" />
                                            <asp:ListItem Value="CL" Text="CL" />
                                            <asp:ListItem Value="PH" Text="PH" />
                                            <asp:ListItem Value="ML" Text="ML" />
                                            <asp:ListItem Value="PL" Text="PL" />
                                            <asp:ListItem Value="W" Text="W" />
                                            <asp:ListItem Value="H" Text="H" />
                                            <%--<asp:ListItem Value="CO" Text="CO" />--%>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="25">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="DropDownList25" runat="server" SelectedValue='<%# Bind("DAY25") %>' OnSelectedIndexChanged="shiftcalendar_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true">
                                            <asp:ListItem Value="0" Text="Set" />
                                            <asp:ListItem Value="P" Text="P" />
                                            <asp:ListItem Value="A" Text="A" />
                                            <asp:ListItem Value="HD" Text="HD" />
                                            <asp:ListItem Value="CL" Text="CL" />
                                            <asp:ListItem Value="PH" Text="PH" />
                                            <asp:ListItem Value="ML" Text="ML" />
                                            <asp:ListItem Value="PL" Text="PL" />
                                            <asp:ListItem Value="W" Text="W" />
                                            <asp:ListItem Value="H" Text="H" />
                                            <%--<asp:ListItem Value="CO" Text="CO" />--%>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="26">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="DropDownList26" runat="server" SelectedValue='<%# Bind("DAY26") %>' OnSelectedIndexChanged="shiftcalendar_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true">
                                            <asp:ListItem Value="0" Text="Set" />
                                            <asp:ListItem Value="P" Text="P" />
                                            <asp:ListItem Value="A" Text="A" />
                                            <asp:ListItem Value="HD" Text="HD" />
                                            <asp:ListItem Value="CL" Text="CL" />
                                            <asp:ListItem Value="PH" Text="PH" />
                                            <asp:ListItem Value="ML" Text="ML" />
                                            <asp:ListItem Value="PL" Text="PL" />
                                            <asp:ListItem Value="W" Text="W" />
                                            <asp:ListItem Value="H" Text="H" />
                                            <%--<asp:ListItem Value="CO" Text="CO" />--%>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="27">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="DropDownList27" runat="server" SelectedValue='<%# Bind("DAY27") %>' OnSelectedIndexChanged="shiftcalendar_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true">
                                            <asp:ListItem Value="0" Text="Set" />
                                            <asp:ListItem Value="P" Text="P" />
                                            <asp:ListItem Value="A" Text="A" />
                                            <asp:ListItem Value="HD" Text="HD" />
                                            <asp:ListItem Value="CL" Text="CL" />
                                            <asp:ListItem Value="PH" Text="PH" />
                                            <asp:ListItem Value="ML" Text="ML" />
                                            <asp:ListItem Value="PL" Text="PL" />
                                            <asp:ListItem Value="W" Text="W" />
                                            <asp:ListItem Value="H" Text="H" />
                                            <%--<asp:ListItem Value="CO" Text="CO" />--%>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="28">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="DropDownList28" runat="server" SelectedValue='<%# Bind("DAY28") %>' OnSelectedIndexChanged="shiftcalendar_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true">
                                            <asp:ListItem Value="0" Text="Set" />
                                            <asp:ListItem Value="P" Text="P" />
                                            <asp:ListItem Value="A" Text="A" />
                                            <asp:ListItem Value="HD" Text="HD" />
                                            <asp:ListItem Value="CL" Text="CL" />
                                            <asp:ListItem Value="PH" Text="PH" />
                                            <asp:ListItem Value="ML" Text="ML" />
                                            <asp:ListItem Value="PL" Text="PL" />
                                            <asp:ListItem Value="W" Text="W" />
                                            <asp:ListItem Value="H" Text="H" />
                                            <%--<asp:ListItem Value="CO" Text="CO" />--%>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="29">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="DropDownList29" runat="server" SelectedValue='<%# Bind("DAY29") %>' OnSelectedIndexChanged="shiftcalendar_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true">
                                            <asp:ListItem Value="0" Text="Set" />
                                            <asp:ListItem Value="P" Text="P" />
                                            <asp:ListItem Value="A" Text="A" />
                                            <asp:ListItem Value="HD" Text="HD" />
                                            <asp:ListItem Value="CL" Text="CL" />
                                            <asp:ListItem Value="PH" Text="PH" />
                                            <asp:ListItem Value="ML" Text="ML" />
                                            <asp:ListItem Value="PL" Text="PL" />
                                            <asp:ListItem Value="W" Text="W" />
                                            <asp:ListItem Value="H" Text="H" />
                                            <%--<asp:ListItem Value="CO" Text="CO" />--%>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="30">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="DropDownList30" runat="server" SelectedValue='<%# Bind("DAY30") %>' OnSelectedIndexChanged="shiftcalendar_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true">
                                            <asp:ListItem Value="0" Text="Set" />
                                            <asp:ListItem Value="P" Text="P" />
                                            <asp:ListItem Value="A" Text="A" />
                                            <asp:ListItem Value="HD" Text="HD" />
                                            <asp:ListItem Value="CL" Text="CL" />
                                            <asp:ListItem Value="PH" Text="PH" />
                                            <asp:ListItem Value="ML" Text="ML" />
                                            <asp:ListItem Value="PL" Text="PL" />
                                            <asp:ListItem Value="W" Text="W" />
                                            <asp:ListItem Value="H" Text="H" />
                                            <%--<asp:ListItem Value="CO" Text="CO" />--%>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="31">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="DropDownList31" runat="server" SelectedValue='<%# Bind("DAY31") %>' OnSelectedIndexChanged="shiftcalendar_SelectedIndexChanged" AutoPostBack="true" AppendDataBoundItems="true">
                                            <asp:ListItem Value="0" Text="Set" />
                                            <asp:ListItem Value="P" Text="P" />
                                            <asp:ListItem Value="A" Text="A" />
                                            <asp:ListItem Value="HD" Text="HD" />
                                            <asp:ListItem Value="CL" Text="CL" />
                                            <asp:ListItem Value="PH" Text="PH" />
                                            <asp:ListItem Value="ML" Text="ML" />
                                            <asp:ListItem Value="PL" Text="PL" />
                                            <asp:ListItem Value="W" Text="W" />
                                            <asp:ListItem Value="H" Text="H" />
                                            <%--<asp:ListItem Value="CO" Text="CO" />--%>
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </asp:Panel>

                    <%--<asp:Panel ID="Panel2" runat="server" ScrollBars="Auto" CssClass="grid-view">
                    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CellPadding="4" ForeColor="#333333">
                        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                        <EditRowStyle BackColor="#999999" />
                        <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                        <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                        <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                        <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                        <SortedAscendingCellStyle BackColor="#E9E7E2" />
                        <SortedAscendingHeaderStyle BackColor="#506C8C" />
                        <SortedDescendingCellStyle BackColor="#FFFDF8" />
                        <SortedDescendingHeaderStyle BackColor="#6F8DAE" />
                        <Columns>
                            <asp:TemplateField HeaderText="Sr. No.">
                                <ItemTemplate>
                                    <asp:Label ID="lbl_srnumber" runat="server" Text="<%# Container.DataItemIndex+1 %>"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ControlStyle-Width="50px" HeaderText="Emp Code">
                                <ItemTemplate>
                                    <asp:Label ID="lblempcode" runat="server" Text='<%# Eval("EMP_CODE")%>' Width="50"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField ControlStyle-Width="100px" HeaderText="Emp Name">
                                <ItemTemplate>
                                    <asp:Label ID="lblempname" runat="server" Text='<%# Eval("EMP_NAME")%>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Day">
                                <ItemTemplate>
                                    <asp:TextBox ID="txtday" runat="server" Text='<%# Eval("TEMP_DAY")%>' Width="50"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </asp:Panel>--%>
                    <%--<div class="row text-center">
                     <h3><b> P : Present, A : Absent, F : Half Day, L : Leave, W : Weekly  Off, H : Holiday </b></h3>
            </div>--%>
                    <%-- <br />
             <div class="row text-center">
                    <%-- <asp:TextBox ID="txt1" runat="server" Text=" P : Present, A : Absent, F : Half Day, L : Leave, W : Weekly  Off, H : Holiday"  ></asp:TextBox>--%>
                    <%--<h5><b>Note = P : Present, A : Absent, F : Half Day, CL : Casual Leave, PH : Paternity Leaves, ML : Maternity Leave , PL : Privilege Leave, W : Weekly  Off, H : Holiday,CO : Company Off </b></h5>
                </div>--%>
                </asp:Panel>
                </div>
            </ContentTemplate>
            <%--  <Triggers>
            <asp:PostBackTrigger ControlID="btn_print" />
       </Triggers>--%>
        </asp:UpdatePanel>
    </form>
</body>
</html>

