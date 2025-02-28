<%@ Page Title="Attendance History" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="AttendanceHistory.aspx.cs" Inherits="AttendanceHistory" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph_title" runat="Server">
    <title>Attendance History</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_header" runat="Server">
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
    <link href="css/select2.min.css" rel="stylesheet" />
    <script src="js/select2.min.js"></script>
    <link href="css/GridViewFreezeStyle.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .container {
            max-width: 99%;
        }

        .grid-view {
            height: auto;
            max-height: 500px;
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

        .text_box {
            margin-top: 7px;
        }

        .auto-style1 {
            color: #FFFFFF;
        }

        h2 {
            border-radius: 5px;
        }

        .grid-view {
            height: auto;
            max-height: 300px;
            overflow: scroll;
        }

        h5 {
            font-weight: bold;
            font-size: 15px;
        }
    </style>

    <script>
        function pageLoad() {

            $(".date-picker1").datepicker({
                changeMonth: true,
                changeYear: true,
                showButtonPanel: true,
                dateFormat: 'dd/mm/yy',
                minDate: 0,
                onSelect: function (selected) {
                    $(".date-picker2").datepicker("option", "minDate", selected)
                }
            });

            $(".date-picker2").datepicker({
                changeMonth: true,
                changeYear: true,
                showButtonPanel: true,
                dateFormat: 'dd/mm/yy',
                minDate: 0,
                onSelect: function (selected) {
                    $(".date-picker1").datepicker("option", "maxDate", selected)
                }
            });

            $('.date-picker').datepicker({
                changeYear: true,
                showButtonPanel: true,
                dateFormat: 'yy',
                maxDate: 0,
                yearRange: '1950',
                onClose: function (dateText, inst) {
                    var year = $("#ui-datepicker-div .ui-datepicker-year :selected").val();
                    $(this).datepicker('setDate', new Date(year, 1));
                }

            });

            $(".date-picker").focus(function () {
                $(".ui-datepicker-month").hide();
                $(".ui-datepicker-calendar").hide();
            });

            $(".date-picker1").attr("readonly", "true");
            $(".date-picker2").attr("readonly", "true");
        }


        $(function () {
            $('#<%=btn_emp.ClientID%>').click(function () {
                $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
            });
            $('#<%=btn_admin.ClientID%>').click(function () {
                $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
            });

            $('#<%=btn_exceloutput.ClientID%>').click(function () {

                $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });

            });

            $('#<%=btn_save.ClientID%>').click(function () {
                if (reg_validate()) {
                    $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
                }
            });

            $('#<%=btn_exceloutput.ClientID%>').click(function () {

                $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });

            });



            $('#<%=gv_attendance.ClientID%> td').click(function () {

                $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });

            });

        });

        function reg_validate() {

            var txtfrom_date = document.getElementById('<%=from_date.ClientID %>');
                var txtto_date = document.getElementById('<%=to_date.ClientID %>');

                var dept = document.getElementById('<%=ddl_department.ClientID %>');
                var SelectedText11 = dept.options[dept.selectedIndex].text;



                // from date
                if (txtfrom_date.value == "") {
                    alert("Please Select Form Date");
                    txtfrom_date.focus();
                    return false;
                }

                if (txtto_date.value == "") {
                    alert("Please Select To Date");
                    txtto_date.focus();
                    return false;
                }

                if (SelectedText11 == "--Select Department--") {
                    alert("Please Select Department !!!");
                    dept.focus();
                    return false;
                }

                //if (SelectedText11 == "--Select Grade--") {
                //    alert("Please Select Grade !!!");
                //    grade.focus();
                //    return false;
                //}


                //if (SelectedText11 == "--Select Employee--") {
                //    alert("Please Select Grade !!!");
                //    empname.focus();
                //    return false;
                //}
                return true;

            }

            function employee_attendance() {
                var unitcode = document.getElementById('<%= ddl_unitcode_att.ClientID %>');
            var selectunit = unitcode.options[unitcode.selectedIndex].text;
            var txtyear = document.getElementById('<%= txt_from_att.ClientID%>');


            if (selectunit == "Select Unit Code") {
                alert("Please Select Unit !!!");
                unitcode.focus();
                return false;
            }

            if (txtyear.value == "") {
                alert("Please Select To year");
                txtyear.focus();
                return false;
            }

        }

        $(document).ready(function () {

            $(".js-example-basic-single").select2();

        });

        window.onfocus = function () {
            $.unblockUI();

        }
        function openWindow() {

            window.open("html/AttandanceHistory.html", 'popUpWindow', 'height=500,width=600,left=100,top=100,toolbar=no,menubar=no,location=no,directories=no,scrollbars=yes, status=No');

        }
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph_righrbody" runat="Server">
    <div class="container-fluid">

        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <asp:Panel ID="Panel3" runat="server" CssClass="panel panel-primary">
            <div class="panel-heading">
                <div class="row">
                    <div class="col-sm-1"></div>
                    <div class="col-sm-9">
                        <div style="color: #fff; font-size: small;" class="text-center text-uppercase"><b>Attendance History</b></div>
                    </div>
                    <div class="col-sm-2 text-right">

                        <asp:LinkButton ID="LinkButton1" runat="server" OnClientClick="openWindow();return false;" Style="font-size: 10px;">
                            <asp:Image runat="server" ID="Image1" Width="20" Height="20" ToolTip="Help" ImageUrl="Images/help_ico.png" />
                        </asp:LinkButton>
                    </div>
                </div>
            </div>
            <br />
            <div class="panel-heading">
                <div class="row">
                    <div class="col-sm-4"></div>
                    <div class="col-sm-9">
                        <div style="text-align: left; color:white; font-size: small;"><b>Attendance History Details</b></div>
                    </div>
                    <div class="col-sm-2 text-left">
                        
                    </div>
                </div>
            </div>


            <div class="panel-body">
                <div class="container-fluid" style="background: #f3f1fe; border: 1px solid #e2e2dd; border-radius: 10px; padding:25px 25px 25px 25px; margin-bottom:20px; margin-top:20px">
                <div class="row text-center">

                    <asp:Button ID="btn_admin" runat="server" class="btn btn-primary admin_btn" OnClick="btn_attendace_all"
                        Text=" Admin " />

                    <asp:Button ID="btn_emp" runat="server" class="btn btn-large emp_btn" Width="8%" OnClick="btn_attendace_employee"
                        Text=" Employee " />
                    <asp:Button ID="btn_close_employee2" runat="server" class="btn btn-danger" OnClick="btn_close_click"
                        Text=" Close " />
                </div>
                <br />
                
                <div id="admin_panel" visible="false" runat="server">
                    <div class="row">
                        <div class="col-sm-2">
                            From Date :
                                <asp:TextBox ID="from_date" runat="server" class="form-control date-picker1"></asp:TextBox>
                        </div>
                        <div class="col-sm-2">
                            To Date :
                                <asp:TextBox ID="to_date" runat="server" class="form-control date-picker2"></asp:TextBox>
                        </div>
                        <div class="col-sm-2">
                            Department :
                                <asp:DropDownList ID="ddl_department" runat="server" AutoPostBack="True" Width="100%" class="js-example-basic-single" meta:resourceKey="ddl_genderResource1" OnSelectedIndexChanged="ddl_department_SelectedIndexChanged"></asp:DropDownList>
                        </div>
                        <div class="col-sm-2">
                            Grade :
                                <asp:DropDownList ID="ddl_grade" runat="server" AutoPostBack="True" Width="100%" class="js-example-basic-single" meta:resourceKey="ddl_genderResource1" OnSelectedIndexChanged="ddl_grade_SelectedIndexChanged">
                                </asp:DropDownList>
                        </div>
                        <div class="col-sm-2">
                            Employee Name :
                                <asp:DropDownList ID="ddl_empname" runat="server" class="form-control" meta:resourceKey="ddl_genderResource1"></asp:DropDownList>
                        </div>
                    </div>
                    <br />
                    <div class="row text-center">

                        <asp:Button ID="btn_save" runat="server" class="btn btn-primary" OnClientClick="return reg_validate();" OnClick="btn_save_Click"
                            Text=" Submit " />

                        <asp:Button ID="btn_exceloutput" runat="server" class="btn btn-primary" OnClick="btn_exceloutput_Click"
                            Text=" ExcelOutput " />

                        <asp:Button ID="btn_close" runat="server" class="btn btn-danger" OnClick="btn_close_click"
                            Text=" Close " />


                    </div>
                    <br />
                    <asp:Panel ID="Panel2" runat="server" ScrollBars="Auto" CssClass="grid-view">
                        <asp:GridView ID="attendance_gridview" class="table" AlternatingRowStyle-CssClass="table" runat="server" AllowPaging="False"
                            AutoGenerateColumns="False" BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3" Font-Size="X-Small">
                            <RowStyle ForeColor="#000066" />
                            <Columns>
                                <asp:BoundField DataField="emp_name" HeaderText="NAME"
                                    SortExpression="emp_name" />
                                <asp:BoundField DataField="logdate" HeaderText="DATE"
                                    SortExpression="logdate" />
                                <asp:BoundField DataField="intime" HeaderText="IN TIME"
                                    SortExpression="intime" />
                                <asp:BoundField DataField="outtime" HeaderText="OUT TIME"
                                    SortExpression="outtime" />
                                <asp:BoundField DataField="WH" HeaderText="WH"
                                    SortExpression="WH" />
                            </Columns>
                           <FooterStyle BackColor="White" ForeColor="#000066" />
                            <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" CssClass="text-uppercase" />
                            <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Center" />
                            <RowStyle ForeColor="#000066" BackColor="#ffffff" />
                            <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                            <SortedAscendingCellStyle BackColor="#F1F1F1" />
                            <SortedAscendingHeaderStyle BackColor="#007DBB" />
                            <SortedDescendingCellStyle BackColor="#CAC9C9" />
                            <SortedDescendingHeaderStyle BackColor="#00547E" />
                        </asp:GridView>
                    </asp:Panel>
                </div>
                <br />
                <div class="emp_panel" id="emp_panel" visible="false" runat="server">
                    <div class="row">

                        <div class="col-sm-2">
                            Select Unit:
                            <asp:DropDownList ID="ddl_unitcode_att" runat="server" class="form-control">
                            </asp:DropDownList>
                        </div>
                        <div class="col-sm-2">
                            Month :
                       
                            <asp:DropDownList ID="txtcurrentmonth" runat="server" CssClass="form-control">
                                <asp:ListItem Value="0">Select Month</asp:ListItem>
                                <asp:ListItem Value="01">JAN</asp:ListItem>
                                <asp:ListItem Value="02">FEB</asp:ListItem>
                                <asp:ListItem Value="03">MAR</asp:ListItem>
                                <asp:ListItem Value="04">APR</asp:ListItem>
                                <asp:ListItem Value="05">MAY</asp:ListItem>
                                <asp:ListItem Value="06">JUN</asp:ListItem>
                                <asp:ListItem Value="07">JUL</asp:ListItem>
                                <asp:ListItem Value="08">AUG</asp:ListItem>
                                <asp:ListItem Value="09">SEP</asp:ListItem>
                                <asp:ListItem Value="10">OCT</asp:ListItem>
                                <asp:ListItem Value="11">NOV</asp:ListItem>
                                <asp:ListItem Value="12">DEC</asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="col-sm-1">
                            Year:
                            <asp:TextBox ID="txt_from_att" runat="server" class="form-control date-picker"></asp:TextBox>
                        </div>
                        <br />
                        <asp:Button ID="btn_employee_att" runat="server" class="btn btn-primary" OnClientClick="return employee_attendance();" OnClick="btn_empployee_attendace"
                            Text=" Submit " />
                        <asp:Button ID="btn_close_employee1" runat="server" class="btn btn-danger" OnClick="btn_close_click"
                            Text=" Close " />

                        <%--<div class="col-sm-1">
                            To Date :
                        </div>
                        <div class="col-sm-3 ">
                            <asp:TextBox ID="txt_to_att" runat="server" class="form-control date-picker"></asp:TextBox>
                        </div>--%>
                    </div>
                    <br />

                    <asp:Panel ID="Panel1" runat="server" ScrollBars="Auto" CssClass="grid-view">
                        <%--  OnRowDataBound="gv_attendance_RowDataBound" OnRowCreated="gv_attendance_RowCreated"--%>
                        <asp:GridView ID="gv_attendance" runat="server" class="table" AutoGenerateColumns="False"
                            CellPadding="4" ForeColor="#333333">

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
                                <asp:TemplateField ControlStyle-Width="100px" HeaderText="Employee Name">
                                    <ItemTemplate>
                                        <asp:Label ID="lblempname" runat="server" Text='<%# Eval("EMP_NAME")%>'></asp:Label>
                                    </ItemTemplate>
                                    <ControlStyle Width="100px" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="01">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtday01" runat="server" Text='<%# Eval("DAY01")%>' Width="50"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="02">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtday02" runat="server" Text='<%# Eval("DAY02")%>' Width="50"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="03">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtday03" runat="server" Text='<%# Eval("DAY03")%>' Width="50"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="04">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtday04" runat="server" Text='<%# Eval("DAY04")%>' Width="50"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="05">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtday05" runat="server" Text='<%# Eval("DAY05")%>' Width="50"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="06">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtday06" runat="server" Text='<%# Eval("DAY06")%>' Width="50"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="07">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtday07" runat="server" Text='<%# Eval("DAY07")%>' Width="50"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="08">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtday08" runat="server" Text='<%# Eval("DAY08")%>' Width="50"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="09">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtday09" runat="server" Text='<%# Eval("DAY09")%>' Width="50"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="10">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtday10" runat="server" Text='<%# Eval("DAY10")%>' Width="50"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="11">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtday11" runat="server" Text='<%# Eval("DAY11")%>' Width="50"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="12">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtday12" runat="server" Text='<%# Eval("DAY12")%>' Width="50"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="13">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtday13" runat="server" Text='<%# Eval("DAY13")%>' Width="50"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="14">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtday14" runat="server" Text='<%# Eval("DAY14")%>' Width="50"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="15">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtday15" runat="server" Text='<%# Eval("DAY15")%>' Width="50"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="16">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtday16" runat="server" Text='<%# Eval("DAY16")%>' Width="50"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="17">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtday17" runat="server" Text='<%# Eval("DAY17")%>' Width="50"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="18">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtday18" runat="server" Text='<%# Eval("DAY18")%>' Width="50"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="19">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtday19" runat="server" Text='<%# Eval("DAY19")%>' Width="50"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="20">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtday20" runat="server" Text='<%# Eval("DAY20")%>' Width="50"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="21">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtday21" runat="server" Text='<%# Eval("DAY21")%>' Width="50"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="22">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtday22" runat="server" Text='<%# Eval("DAY22")%>' Width="50"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="23">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtday23" runat="server" Text='<%# Eval("DAY23")%>' Width="50"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="24">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtday24" runat="server" Text='<%# Eval("DAY24")%>' Width="50"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="25">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtday25" runat="server" Text='<%# Eval("DAY25")%>' Width="50"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="26">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtday26" runat="server" Text='<%# Eval("DAY26")%>' Width="50"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="27">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtday27" runat="server" Text='<%# Eval("DAY27")%>' Width="50"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="28">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtday28" runat="server" Text='<%# Eval("DAY28")%>' Width="50"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="29">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtday29" runat="server" Text='<%# Eval("DAY29")%>' Width="50"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="30">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtday30" runat="server" Text='<%# Eval("DAY30")%>' Width="50"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="31">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtday31" runat="server" Text='<%# Eval("DAY31")%>' Width="50"></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderStyle-BackColor="#339933" HeaderText="Present Days">
                                    <ItemTemplate>
                                        <asp:Label ID="lbl_totPdays" runat="server" Text='<%# Eval("TOT_DAYS_PRESENT")%>' Width="50"></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle BackColor="Green" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderStyle-BackColor="Red" HeaderText="Absent Days">
                                    <ItemTemplate>
                                        <asp:Label ID="lbl_totAdays" runat="server" Text='<%# Eval("TOT_DAYS_ABSENT")%>' Width="50"></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle BackColor="Red" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderStyle-BackColor="#0066FF" HeaderText="Casual Leaves">
                                    <ItemTemplate>
                                        <asp:Label ID="lbl_totcLeaves" runat="server" Text='<%# Eval("TOT_CL")%>' Width="50"></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle BackColor="#0066FF" />
                                </asp:TemplateField>

                                <asp:TemplateField HeaderStyle-BackColor="#0066FF" HeaderText="Previlege Leaves">
                                    <ItemTemplate>
                                        <asp:Label ID="lbl_totpLeaves" runat="server" Text='<%# Eval("TOT_PL")%>' Width="50"></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle BackColor="#0066FF" />
                                </asp:TemplateField>

                                <asp:TemplateField HeaderStyle-BackColor="#0066FF" HeaderText="Maternity leave">
                                    <ItemTemplate>
                                        <asp:Label ID="lbl_totmLeaves" runat="server" Text='<%# Eval("TOT_MATERNITY")%>' Width="50"></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle BackColor="#0066FF" />
                                </asp:TemplateField>

                                <asp:TemplateField HeaderStyle-BackColor="#0066FF" HeaderText="Paternity Leave">
                                    <ItemTemplate>
                                        <asp:Label ID="lbl_totprevilageLeaves" runat="server" Text='<%# Eval("TOT_PATERNITY")%>' Width="50"></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle BackColor="#0066FF" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderStyle-BackColor="#0066FF" HeaderText="Cl Balance Leave">
                                    <ItemTemplate>
                                        <asp:Label ID="lbl_totcbalLeaves" runat="server" Text='<%# Eval("CL_BALANCE")%>' Width="50" Enabled="false"></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle BackColor="#0066FF" />
                                </asp:TemplateField>

                                <asp:TemplateField HeaderStyle-BackColor="#0066FF" HeaderText="Pl Balance Leaves">
                                    <ItemTemplate>
                                        <asp:Label ID="lbl_totpbalLeaves" runat="server" Text='<%# Eval("PL_BALANCE")%>' Width="50" Enabled="false"></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle BackColor="#0066FF" />
                                </asp:TemplateField>

                                <asp:TemplateField HeaderStyle-BackColor="#0066FF" HeaderText="Leaves">
                                    <ItemTemplate>
                                        <asp:Label ID="lbl_totLeaves" runat="server" Text='<%# Eval("TOT_LEAVES")%>' Width="50"></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle BackColor="#0066FF" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderStyle-BackColor="#0066FF" HeaderText="Weeks Of">
                                    <ItemTemplate>
                                        <asp:Label ID="lbl_totWoff" runat="server" Text='<%# Eval("WEEKLY_OFF")%>' Width="50"></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle BackColor="#0066FF" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderStyle-BackColor="#0066FF" HeaderText="Holidays">
                                    <ItemTemplate>
                                        <asp:Label ID="lbl_totHolidays" runat="server" Text='<%# Eval("HOLIDAYS")%>' Width="50"></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle BackColor="#0066FF" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderStyle-BackColor="#0066FF" HeaderText="Payable Days">
                                    <ItemTemplate>
                                        <asp:Label ID="lbl_totWdays" runat="server" Text='<%# Eval("TOT_WORKING_DAYS")%>' Width="50"></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle BackColor="#0066FF" />
                                </asp:TemplateField>

                                <asp:TemplateField HeaderStyle-BackColor="#0066FF" HeaderText="Month">
                                    <ItemTemplate>
                                        <asp:Label ID="lbl_month" runat="server" Text='<%# Eval("MONTH")%>' Width="50"></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle BackColor="#0066FF" />
                                </asp:TemplateField>

                                <asp:TemplateField HeaderStyle-BackColor="#0066FF" HeaderText="Year">
                                    <ItemTemplate>
                                        <asp:Label ID="lbl_year" runat="server" Text='<%# Eval("YEAR")%>' Width="50"></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle BackColor="#0066FF" />
                                </asp:TemplateField>

                            </Columns>
                        </asp:GridView>
                    </asp:Panel>
                </div>
                    </div>

            </div>
        </asp:Panel>


    </div>

</asp:Content>

