<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Android_Attendances_Details.aspx.cs" MasterPageFile="~/MasterPage.master" Inherits="Android_Attendances_Logs" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_title" runat="Server">
    <title>Android Attendances Details</title>
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

    <link href="css/GridViewFreezeStyle.css" rel="stylesheet" type="text/css" />
    <link href="datatable/dataTables.bootstrap.min.css" rel="stylesheet" />
    <link href="datatable/buttons.bootstrap.min.css" rel="stylesheet" />
    <%--<script src="datatable/jquery-1.12.3.js"></script>--%>
    <script src="datatable/jquery.dataTables.min.js"></script>
    <script src="datatable/dataTables.bootstrap.min.js"></script>
    <script src="datatable/dataTables.buttons.min.js"></script>
    <script src="datatable/buttons.bootstrap.min.js"></script>
    <%-- <script src="datatable/jszip.min.js"></script>--%>
    <%-- <script src="datatable/pdfmake.min.js"></script>--%>
    <script src="datatable/vfs_fonts.js"></script>
    <script src="datatable/buttons.html5.min.js"></script>
    <script src="datatable/buttons.print.min.js"></script>
    <script src="datatable/buttons.colVis.min.js"></script>
    <script src="datatable/pdfmake.min.js"></script>

    <script type="text/javascript">
        $(function () {
            $("#dialog").dialog({

                autoOpen: false,
                modal: true,
                height: 500,
                width: 500,
                title: "Zoomed Image",
                buttons: [{ text: "Close", click: function () { $(this).dialog("close") } }],
            });
            $("[id*=Camera_Image1]").click(function () {
                $('#dialog').html('');
                $('#dialog').append($(this).clone().width(470).height(400));
                $('#dialog').dialog('open');
                //height:200;
                //width: 200;
            });
            $("[id*=Camera_Image3]").click(function () {
                $('#dialog').html('');
                $('#dialog').append($(this).clone().width(470).height(400));
                $('#dialog').dialog('open');
                //height:200;
                //width: 200;
            });
            $("[id*=Camera_Image2]").click(function () {
                $('#dialog').html('');
                $('#dialog').append($(this).clone().width(470).height(400));
                $('#dialog').dialog('open');
                //height:200;
                //width: 200;
            });


            $("[id*=fire_upload_image]").click(function () {
                $('#dialog').html('');
                $('#dialog').append($(this).clone().width(470).height(400));
                $('#dialog').dialog('open');
                //height:200;
                //width: 200;
            });
        });





        $(document).ready(function () {
            var st = $(this).find("input[id*='hidtab']").val();
            if (st == null)
                st = 0;
            $('[id$=tabs]').tabs({ selected: st });
        });
        function unblock() {
            $.unblockUI();
        }


        $(document).ready(function () {
            $(document).on("Keyup", function () {
                SearchGrid('<%=txt_search.ClientID%>', '<%=gv_fire_photo.ClientID%>');
            });


            $('[id*=chk_gv_header]').click(function () {
                $("[id*='chk_client']").attr('checked', this.checked);
            });


        });


        $(document).ready(function () {

            $(".date-pickerfm").datepicker({
                changeMonth: true,
                changeYear: true,
                showButtonPanel: true,
                dateFormat: 'yy-mm-dd',
                onSelect: function (selected) {
                    $(".date-picker2").datepicker("option", "minDate", selected)
                }
            });


            $(".date-picker1").datepicker({
                changeMonth: true,
                changeYear: true,
                showButtonPanel: true,
                dateFormat: 'dd/mm/yy',
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
                changeMonth: true,
                changeYear: true,
                maxDate: 0,
                yearRange: "1990:+100",
                showButtonPanel: true,
                dateFormat: 'mm/yy',
                onClose: function (dateText, inst) {
                    var month = $("#ui-datepicker-div .ui-datepicker-month :selected").val();
                    var year = $("#ui-datepicker-div .ui-datepicker-year :selected").val();
                    $(this).datepicker('setDate', new Date(year, month, 1));
                }
            });
            $('.date-picker').focus(function () {
                $(".ui-datepicker-calendar").hide();

            });
            $(".date-picker").attr("readonly", "true");



            $(".date-picker1").attr("readonly", "true");
            $(".date-picker2").attr("readonly", "true");
            $(".date-pickerfm").attr("readonly", "true");

            var table = $('#<%=GradeGridView.ClientID%>').DataTable({
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
            .appendTo('#<%=GradeGridView.ClientID%>_wrapper .col-sm-6:eq(0)');
            $.fn.dataTable.ext.errMode = 'none';

            var table = $('#<%=gv_attendances_excel.ClientID%>').DataTable({
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
            .appendTo('#<%=gv_attendances_excel.ClientID%>_wrapper .col-sm-6:eq(0)');
            $.fn.dataTable.ext.errMode = 'none';


            var table = $('#<%=grd_feedback.ClientID%>').DataTable({
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
               .appendTo('#<%=grd_feedback.ClientID%>_wrapper .col-sm-6:eq(0)');

            $.fn.dataTable.ext.errMode = 'none';

            var table = $('#<%=gv_fire_photo.ClientID%>').DataTable({
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
               .appendTo('#<%=gv_fire_photo.ClientID%>_wrapper .col-sm-6:eq(0)');

            $.fn.dataTable.ext.errMode = 'none';


            var table1 = $('#<%=gv_attendce_percent_count.ClientID%>').DataTable({
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

            table1.buttons().container()
            .appendTo('#<%=gv_attendce_percent_count.ClientID%>_wrapper .col-sm-6:eq(0)');

            $.fn.dataTable.ext.errMode = 'none';



            var table1 = $('#<%=grd_current_location.ClientID%>').DataTable({
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

            table1.buttons().container()
            .appendTo('#<%=grd_current_location.ClientID%>_wrapper .col-sm-6:eq(0)');

            $.fn.dataTable.ext.errMode = 'none';

        });



        $.fn.dataTable.ext.errMode = 'none';

        function Search_Gridview(strKey) {
            var strData = strKey.value.toLowerCase().split(" ");
            var tblData = document.getElementById("<%=gv_fire_photo.ClientID %>");
            var rowData;
            for (var i = 1; i < tblData.rows.length; i++) {
                rowData = tblData.rows[i].innerHTML;
                var styleDisplay = 'none';
                for (var j = 0; j < strData.length; j++) {
                    if (rowData.toLowerCase().indexOf(strData[j]) >= 0)
                        styleDisplay = '';
                    else {
                        styleDisplay = 'none';
                        break;
                    }
                }
                tblData.rows[i].style.display = styleDisplay;
            }
        }


        function validation() {

            var ddl_state = document.getElementById('<%=ddl_state.ClientID %>');
            var Selected_ddl_state = ddl_state.options[ddl_state.selectedIndex].text;

            var ddlunitselect = document.getElementById('<%= ddlunitselect.ClientID %>');
            var select_ddlunitselect = ddlunitselect.options[ddlunitselect.selectedIndex].text;

            if (Selected_ddl_state == "Select") {
                alert("Please Select State");
                ddl_state.focus();
                return false;
            }

            if (select_ddlunitselect == "Select") {
                alert("Please Select Branch Name");
                ddlunitselect.focus();
                return false;
            }


        }

        function fire_approve() {
            var isValid = false; {
                var gridView = document.getElementById('<%= gv_fire_photo.ClientID %>');
                for (var i = 1; i < gridView.rows.length; i++) {
                    var inputs = gridView.rows[i].getElementsByTagName('input');
                    if (inputs != null) {
                        if (inputs[0].type == "checkbox") {
                            if (inputs[0].checked) {
                                isValid = true;
                                return true;
                            }
                        }
                    }
                }
                alert("Please select atleast one Record ");


            }
            return false;

        }
        function Req_validation1() {
            var t_client = document.getElementById('<%=ddl_client_name.ClientID %>');
            var Selectedclient = t_client.options[t_client.selectedIndex].text;

            if (Selectedclient == "Select") {
                alert("Please Select Client.");
                t_client.focus();
                return false;
            }
            var txt_monthyear = document.getElementById('<%=txt_monthyear.ClientID %>');
            if (txt_monthyear.value == "") {
                alert("Please Select Month/Year.");
                txt_monthyear.focus();
                return false;
            }
            if (R_validation1() == false) { return false; }
            $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
            return true;
        }
        function valid_email() {
            var t_client = document.getElementById('<%=ddl_client_name.ClientID %>');
            var Selectedclient = t_client.options[t_client.selectedIndex].text;

            if (Selectedclient == "Select") {
                alert("Please Select Client.");
                t_client.focus();
                return false;
            }
            $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
            return true;
        }

    </script>

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
            max-height: 400px;
            overflow-x: hidden;
            overflow-y: auto;
        }

        h5 {
            font-weight: bold;
            font-size: 15px;
        }

        .row {
            margin: 0px;
        }



        .modal {
            display: none;
            position: absolute;
            top: 0px;
            left: 0px;
            background-color: black;
            z-index: 100;
            opacity: 0.8;
            filter: alpha(opacity=60);
            -moz-opacity: 0.8;
            min-height: 100%;
        }

        #divImage {
            display: none;
            z-index: 1000;
            position: fixed;
            top: 0;
            left: 0;
            background-color: White;
            height: 550px;
            width: 600px;
            padding: 3px;
            border: solid 1px black;
        }

        .Hide {
            display: none;
        }
    </style>

    <script type="text/javascript">
        function openWindow() {
            window.open("html/Android.html", 'popUpWindow', 'height=500,width=600,left=100,top=100,toolbar=no,menubar=no,location=no,directories=no,scrollbars=yes, status=No');
        }

    </script>
</asp:Content>

<asp:content id="Content4" contentplaceholderid="cph_righrbody" runat="Server">

    <div class="container-fluid">

        <asp:scriptmanager id="ScriptManager1" runat="server">
        </asp:scriptmanager>

        <asp:panel id="Panel3" runat="server" cssclass="panel panel-primary">
            <div class="panel-heading">
                <div class="row">
                    <div class="col-sm-1"></div>
                    <div class="col-sm-9">
                        <div style="color: #fff; font-size: small;" class="text-center text-uppercase"><b>Android Attendances Logs</b></div>
                    </div>
                    <div class="col-sm-2 text-right">
                        <asp:LinkButton ID="LinkButton1" runat="server" OnClientClick="openWindow();return false;" Style="font-size: 10px;">
                            <asp:Image runat="server" ID="Image1" Width="20" Height="20" ToolTip="Help" ImageUrl="Images/help_ico.png" />
                        </asp:LinkButton>
                    </div>
                </div>
            </div>
        </asp:Panel>
        <%--  <div class="panel-body">

                <div class="row">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <div class="col-sm-2 col-xs-12">
                                Type :
   <asp:DropDownList ID="ddl_att_work" runat="server" CssClass="form-control">
       <asp:ListItem Text="Attendance"></asp:ListItem>
       <asp:ListItem Text="Work"></asp:ListItem>
       <asp:ListItem Text="Employee Current Location"></asp:ListItem>
   </asp:DropDownList>
                            </div>
                            <div class="col-sm-2 col-xs-12">
                                Client Name :
   <asp:DropDownList ID="ddl_client" DataValueField="client_code" DataTextField="client_name" OnSelectedIndexChanged="ddl_client_SelectedIndexChanged" AutoPostBack="true" runat="server" CssClass="form-control">
   </asp:DropDownList>
                            </div>
        <div class="col-sm-2 col-xs-12">
                        State :
                            <span class="text-red" style="color: red">*</span>
                        <asp:DropDownList ID="ddl_state" runat="server" class="form-control" Width="100%" OnSelectedIndexChanged="ddl_state_SelectedIndexChanged" AutoPostBack="true">
                        </asp:DropDownList>

                    </div>                   
                             <div class="col-sm-2 col-xs-12 ">
                                Branch Name :
   <asp:DropDownList ID="ddlunitselect" runat="server" class="form-control">
   </asp:DropDownList>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>

                    <div class="col-sm-2 col-xs-12">
                        From Date :
                                        <asp:TextBox ID="txt_satrtdate" runat="server" class="form-control date-picker1"></asp:TextBox>
                    </div>

                    <div class="col-sm-2 col-xs-12">
                        To Date :
                                        <asp:TextBox ID="txt_enddate" runat="server" class="form-control date-picker2"></asp:TextBox>
                    </div>
                     </div>

                    <br />
                    <br />
                    
                    <div class="row text-center">
                        <asp:Button ID="Button3" runat="server" Text="Show" class="btn btn-primary" OnClick="Button3_Click" OnClientClick="return validation();" />
                        &nbsp;&nbsp;&nbsp;<asp:Button ID="btnclose" runat="server" class="btn btn-danger" OnClick="btnclose_Click" Text="Close" CausesValidation="False" />
                   
                        </div>
               
            </div>--%>
        <div id="tabs" style="background: beige;">
            <asp:HiddenField ID="hidtab" Value="0" runat="server" />
            <ul>
                <li id="Li1" class="active"><a data-toggle="tab" href="#menu2">Android Attendance Logs</a></li>
                <li id="Li4" class="active"><a data-toggle="tab" href="#menu4">Monthly Attendance Calendar</a></li>
                <li id="Li2" class="active"><a data-toggle="tab" href="#menu5">Attendance Percentage Count</a></li>
            </ul>
            <div id="menu2">
                <div class="container-fluid">
                    <div class="panel-body">
                        <div class="row">
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                <ContentTemplate>
                                    <div class="col-sm-2 col-xs-12">
                                        <b>Type :</b>
                                        <asp:DropDownList ID="ddl_att_work" runat="server" CssClass="form-control">
                                            <asp:ListItem Text="Attendance"></asp:ListItem>
                                            <asp:ListItem Text="Attendance Excel"></asp:ListItem>
                                            <asp:ListItem Text="Work"></asp:ListItem>
                                            <asp:ListItem Text="Employee Current Location"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-sm-2 col-xs-12">
                                        <b>Client Name :</b>
                                        <asp:DropDownList ID="ddl_client" DataValueField="client_code" DataTextField="client_name" OnSelectedIndexChanged="ddl_client_SelectedIndexChanged" AutoPostBack="true" runat="server" CssClass="form-control">
                                        </asp:DropDownList>
                                    </div>
                                    <div class="col-sm-2 col-xs-12">
                                        <b>State :</b>
                                        <span class="text-red" style="color: red">*</span>
                                        <asp:DropDownList ID="ddl_state" runat="server" class="form-control" Width="100%" OnSelectedIndexChanged="ddl_state_SelectedIndexChanged" AutoPostBack="true">
                                        </asp:DropDownList>

                                        </div>
                                        <div class="col-sm-2 col-xs-12 ">
                                            <b>Branch Name :</b>
                                            <asp:DropDownList ID="ddlunitselect" runat="server" class="form-control">
                                            </asp:DropDownList>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>

                                <div class="col-sm-2 col-xs-12">
                                    <b>From Date :</b>
                                    <asp:TextBox ID="txt_satrtdate" runat="server" class="form-control date-picker1"></asp:TextBox>
                                </div>

                                <div class="col-sm-2 col-xs-12">
                                    <b>To Date :</b>
                                    <asp:TextBox ID="txt_enddate" runat="server" class="form-control date-picker2"></asp:TextBox>
                                </div>
                            </div>

                            <br />
                            <br />

                            <div class="row text-center">
                                <asp:Button ID="Button3" runat="server" Text="Show" class="btn btn-primary" OnClick="Button3_Click" OnClientClick="return validation();" />
                                &nbsp;&nbsp;&nbsp;<asp:Button ID="btnclose" runat="server" class="btn btn-danger" OnClick="btnclose_Click" Text="Close" CausesValidation="False" />

                            </div>

                        </div>

                    </div>


                    <%--   all grideview for attendance logs --%>

                    <%--  this div we use for dialog popup window--%>
                    <div id="dialog"></div>
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
                                </Columns>
                                <FooterStyle BackColor="White" ForeColor="#000066" />
                                <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
                                <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" />
                            </asp:GridView>

                            <asp:GridView ID="grd_work_image" class="table" runat="server" Font-Size="X-Small"
                                AutoGenerateColumns="False" BackColor="White" BorderColor="#CCCCCC" OnPreRender="grd_work_image_PreRender"
                                BorderStyle="None" BorderWidth="1px" CellPadding="3" OnRowDataBound="grd_work_image_RowDataBound">
                                <RowStyle ForeColor="#000066" />
                                <Columns>
                                    <asp:TemplateField HeaderText="Sr No.">
                                        <ItemStyle Width="20px" />
                                        <ItemTemplate>
                                            <%# Container.DataItemIndex+1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField HeaderText="EMP-NAME" DataField="EMP_NAME" SortExpression="EMP_NAME" />
                                    <asp:BoundField HeaderText="STATE-NAME" DataField="state_name" SortExpression="state_name" />
                                    <asp:BoundField HeaderText="BRANCH" DataField="unit_name" SortExpression="unit_name" />
                                    <asp:BoundField HeaderText="DATE-TIME" DataField="datecurrent" SortExpression="datecurrent" />
                                    <asp:TemplateField HeaderText="IMAGE">
                                        <ItemTemplate>
                                            <asp:Image ID="Camera_Image3" runat="server" Height="50" Width="50" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <FooterStyle BackColor="White" ForeColor="#000066" />
                                <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
                                <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" />
                            </asp:GridView>

                            <asp:GridView ID="grd_current_location" class="table" runat="server" Font-Size="X-Small" OnPreRender="grd_location_PreRender"
                                OnSelectedIndexChanged="Location_SelectedIndexChanged"
                                AutoGenerateColumns="False" BackColor="White" BorderColor="#CCCCCC" OnRowDataBound="GradeGridView_RowDataBound_location"
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

                        <div class="container-fluid" runat="server" id="divgv_gstr1_b2b_csv">
                            <br />
                            <asp:Panel ID="panel_gstr1_b2b_csv" runat="server" ScrollBars="Auto" CssClass="grid-view">
                                <asp:GridView ID="gv_attendances_excel" class="table" runat="server"
                                    Font-Size="X-Small"
                                    ForeColor="#333333" OnPreRender="attendances_excel_PreRender" OnRowDataBound="gv_attendances_excel_RowDataBound">
                                    <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                                    <EditRowStyle BackColor="#999999" />
                                    <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                                    <HeaderStyle BackColor="#337AB7" Font-Bold="True" ForeColor="White" Width="50" CssClass="text-uppercase" />
                                    <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                                    <RowStyle BackColor="#ffffff" ForeColor="#333333" />
                                    <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                                    <SortedAscendingCellStyle BackColor="#E9E7E2" />
                                    <SortedAscendingHeaderStyle BackColor="#506C8C" />
                                    <SortedDescendingCellStyle BackColor="#FFFDF8" />
                                    <SortedDescendingHeaderStyle BackColor="#6F8DAE" />

                                </asp:GridView>
                            </asp:Panel>
                        </div>
                    </div>

                    <%--   all grideview for attendance logs end --%>
                </div>

                <br />

                <%-- Android Attendance Report --%>
                <div id="menu4">
                    <div class="container-fluid">
                        <div class="panel-body">

                            <div class="row">
                                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                    <ContentTemplate>
                                        <div class=" col-md-2 col-xs-12">
                                            <b>Select Month :</b>
                                            <span class="text-red" style="color: red">*</span>
                                            <asp:TextBox ID="txt_month_year" Class="form-control date-picker" runat="server"></asp:TextBox>
                                        </div>
                                        <div class="col-sm-2 col-xs-12">
                                            <b>Client Name :</b>
                                            <span class="text-red" style="color: red">*</span>
                                            <asp:DropDownList ID="ddl_clientname" DataValueField="client_code" DataTextField="client_name" OnSelectedIndexChanged="ddl_clientname_SelectedIndexChanged" AutoPostBack="true" runat="server" CssClass="form-control">
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-sm-2 col-xs-12">
                                            <b>State Name :</b>
                                            <asp:DropDownList ID="ddl_statename" runat="server" class="form-control" Width="100%" OnSelectedIndexChanged="ddl_statename_SelectedIndexChanged" AutoPostBack="true">
                                            </asp:DropDownList>

                                        </div>
                                        <div class="col-sm-2 col-xs-12 ">
                                            <b>Branch Name :</b>
                                            <asp:DropDownList ID="ddl_branchname" runat="server" class="form-control">
                                            </asp:DropDownList>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                            <br />
                            <br />
                            <div class="row text-center">
                                <asp:Button ID="btn_show" runat="server" Text="Show" class="btn btn-primary" OnClick="btn_show_Click" OnClientClick="return Req_validation();" />
                            </div>

                        </div>

                    </div>
                    <%--   all grideview for attendance logs --%>

                    <%--  this div we use for dialog popup window--%>
                    <div class="panel-body">
                        <asp:Panel ID="Panel4" runat="server" ScrollBars="auto" class="grid-view">
                            <asp:GridView ID="gv_attendance_report" runat="server" class="table" AutoGenerateColumns="true" CellPadding="1"
                                ForeColor="#333333" OnRowDataBound="gv_attendance_report_RowDataBound" Width="100%" Height="50%" Style="border-collapse: collapse;">
                                <FooterStyle BackColor="White" ForeColor="#000066" />
                                <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" CssClass="text-uppercase" />
                                <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Center" />
                                <RowStyle ForeColor="#000066" BackColor="#ffffff" />
                                <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                                <SortedAscendingCellStyle BackColor="#F1F1F1" />
                                <SortedAscendingHeaderStyle BackColor="#007DBB" />
                                <SortedDescendingCellStyle BackColor="#CAC9C9" />
                                <SortedDescendingHeaderStyle BackColor="#00547E" />
                                <Columns>
                                    <asp:TemplateField ControlStyle-Width="45px" HeaderText="Sr. No.">
                                        <ItemTemplate>
                                            <asp:Label ID="lbl_srnumber" runat="server" Text="<%# Container.DataItemIndex+1 %>"></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </asp:Panel>
                    </div>
                    <%--   all grideview for attendance logs end --%>
                </div>
                <%-- END --%>
                <br />

                <div id="menu1">
                    <div class="container-fluid">
                        <div class="row">
                            <div class="col-sm-2 col-xs-12">

                                <asp:DropDownList ID="ddl_client_fire" Visible="false" DataValueField="client_code" OnSelectedIndexChanged="ddl_client_fire_SelectedIndexChanged" DataTextField="client_name" AutoPostBack="true" runat="server" CssClass="form-control">
                                </asp:DropDownList>
                            </div>

                            <div class="col-sm-2 col-xs-12 ">

                                <asp:DropDownList ID="ddl_state_fire" Visible="false" runat="server" OnSelectedIndexChanged="ddl_state_fire_SelectedIndexChanged" CssClass="form-control">
                                </asp:DropDownList>
                            </div>
                            <br />
                            <asp:Button ID="btn_show_fire" Visible="false" runat="server" OnClick="btn_show_fire_Click" class="btn btn-primary" OnClientClick="return  " Text="SHOW" />
                        </div>
                        <br />



                        <asp:Panel ID="Panel26" runat="server" Visible="false" Style="overflow-x: auto;" CssClass="grid-view">

                            <div class="container-fluid">

                                <div class="row">
                                    <div class="col-sm-10 col-xs-12"></div>
                                    <div class="col-sm-2 col-xs-12" hidden="hidden">
                                        Search :
                        <asp:TextBox runat="server" ID="txt_search" CssClass=" form-control" onkeyup="Search_Gridview(this)" />
                                    </div>
                                </div>

                                <asp:GridView ID="gv_fire_photo" runat="server" OnRowDataBound="gv_fire_photo_RowDataBound" OnPreRender="gv_fire_photo_PreRender" AutoGenerateColumns="false" BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3" class="table" Width="100%">
                                    <FooterStyle BackColor="White" ForeColor="#004C99" />
                                    <SelectedRowStyle BackColor="#d1ddf1" Font-Bold="True" ForeColor="#333333" />
                                    <AlternatingRowStyle BackColor="White" />
                                    <HeaderStyle BackColor="#224173" Font-Bold="True" ForeColor="White" />
                                    <PagerStyle BackColor="#224173" ForeColor="White" HorizontalAlign="Center" />
                                    <RowStyle BackColor="#EFF3FB" />
                                    <EditRowStyle BackColor="#2461BF" />
                                    <SortedAscendingCellStyle BackColor="#F1F1F1" />
                                    <SortedAscendingHeaderStyle BackColor="#007DBB" />
                                    <SortedDescendingCellStyle BackColor="#CAC9C9" />
                                    <SortedDescendingHeaderStyle BackColor="#00547E" />
                                    <Columns>

                                        <asp:TemplateField>
                                            <HeaderTemplate>
                                                <asp:CheckBox ID="chk_gv_header" runat="server" Text="SELECT CLIENT" />

                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chk_client" runat="server" CssClass="center-block" />
                                            </ItemTemplate>
                                        </asp:TemplateField>


                                        <asp:TemplateField>
                                            <ItemStyle Width="20px" />
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnk_remove_fire" runat="server" OnClick="lnk_remove_fire_Click" CausesValidation="false" OnClientClick="return confirm('Are you sure You want to  Delete ?') "><img alt="" height="15"  src="Images/delete_icon.png" width="15" /></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:TemplateField HeaderText="Sr No.">
                                            <ItemStyle Width="20px" />
                                            <ItemTemplate>
                                                <asp:Label ID="lbl_srnumber" runat="server" Text="<%# Container.DataItemIndex+1 %>" Width="20px"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="id" HeaderText="id" SortExpression="id" />
                                        <asp:BoundField DataField="client_name" HeaderText="Client Name" SortExpression="client_name" />
                                        <asp:BoundField DataField="unit_code" HeaderText="unit code" SortExpression="unit_code" />
                                        <asp:BoundField DataField="state_name" HeaderText="State Name" SortExpression="state_name" />
                                        <asp:BoundField DataField="unit_name" HeaderText="Branch Name" SortExpression="unit_name" />
                                        <asp:BoundField DataField="emp_code" HeaderText="Emp Code" SortExpression="emp_code" />
                                        <asp:BoundField DataField="emp_name" HeaderText="Employee Name" SortExpression="emp_name" />


                                        <asp:BoundField DataField="curr_date" HeaderText="Current Date" SortExpression="curr_date" />

                                        <asp:TemplateField HeaderText="Fire Extinguisher Photo">
                                            <ItemTemplate>
                                                <asp:Image ID="fire_upload_image" runat="server" Height="50" Width="50" />
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:BoundField DataField="approve_fire" HeaderText="Status" SortExpression="approve_fire" />

                                        <asp:TemplateField HeaderText="REJECT REASON">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txt_fire_amt" runat="server" CssClass="form-control" Text='<%# Eval("reject_reason")%>' Width="150" onkeypress="return isNumberKey(event,this.id)"></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:BoundField DataField="type_name" HeaderText="Type" SortExpression="type_name" />

                                        <asp:BoundField DataField="client_code" HeaderText="client_code" SortExpression="client_code" />
                                    </Columns>
                                </asp:GridView>

                            </div>
                        </asp:Panel>

                        <br />

                        <div class="row text-center">
                            <asp:Button ID="btn_approve_fire" Visible="false" runat="server" OnClick="btn_approve_fire_Click" class="btn btn-primary" Width="8%" OnClientClick="return  fire_approve(); " Text="APPROVE" />
                            <asp:Button ID="btn_reject_fire" Visible="false" runat="server" OnClick="btn_reject_fire_Click" class="btn btn-primary" OnClientClick="return  fire_approve(); " Text="REJECT" />
                            <asp:Button ID="btn_move_fire" Visible="false" runat="server" OnClick="btn_move_fire_Click" class="btn btn-primary" OnClientClick="return fire_approve();" Text="Move" />


                        </div>


                </div>
            </div>
           
            <div id="menu5">
                <div class="row">
                    <div class="col-sm-2 col-xs-12">
                        <b>Client Name :</b>
                        <asp:DropDownList ID="ddl_clnt_name" DataValueField="client_code" DataTextField="client_name" runat="server" CssClass="form-control">
                        </asp:DropDownList>
                    </div>
                    <div class="col-sm-2 col-xs-12">
                        <b>From Date :</b>
                        <asp:TextBox ID="text_frmdate" runat="server" class="form-control date-pickerfm"></asp:TextBox>
                    </div>
                    <div class="col-sm-2 col-xs-12">
                        <b>To Date :</b>
                        <asp:TextBox ID="text_todate" runat="server" class="form-control date-pickerfm"></asp:TextBox>
                    </div>
                    <br />
                    <div class="col-sm-2 col-xs-12">
                        <asp:Button ID="btn_attpercent" runat="server" OnClick="btn_attpercent_Click" class="btn btn-primary" OnClientClick="return  " Text="SHOW" />
                    </div>
                </div>
                <asp:Panel ID="Panel20" runat="server" Style="overflow-x: auto;">
                    <asp:GridView ID="gv_attendce_percent_count" class="table" runat="server" BackColor="White"
                        BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="1" OnPreRender="gv_attendce_percent_count_PreRender" 
                        AutoGenerateColumns="False" Width="100%">
                        <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
                        <EditRowStyle BackColor="#999999" />
                        <FooterStyle BackColor="White" ForeColor="#000066" />
                        <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" CssClass="text-uppercase" />
                        <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Center" />
                        <RowStyle ForeColor="#000066" BackColor="#ffffff" />
                        <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                        <SortedAscendingCellStyle BackColor="#F1F1F1" />
                        <SortedAscendingHeaderStyle BackColor="#007DBB" />
                        <SortedDescendingCellStyle BackColor="#CAC9C9" />
                        <SortedDescendingHeaderStyle BackColor="#00547E" />
                        <Columns>
                            <asp:TemplateField HeaderText="SR. NO.">
                                <ItemTemplate>
                                    <asp:Label ID="lbl_srnumber" runat="server" Text="<%# Container.DataItemIndex+1 %>"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField DataField="attend_date" HeaderText="Date" SortExpression="attend_date" />
                            <asp:BoundField DataField="emp_cnt" HeaderText="employee count" SortExpression="emp_cnt" />
                            <asp:BoundField DataField="total_attendence" HeaderText="Total Android Attendence" SortExpression="total_attendence" />
                            <asp:BoundField DataField="atte_per" HeaderText="Attendance Percentage" SortExpression="atte_per" />

                        </Columns>
                    </asp:GridView>
                </asp:Panel>
            </div>
       
             <div id="menu3">
                <%--<asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">--%>
                <%-- <ContentTemplate>--%>
                <div class="row">
                    <div class="col-sm-2 col-xs-12 ">
                        <%-- <b>Client Name :</b>--%>
                        <asp:DropDownList ID="ddl_client_name" Visible="false" class="form-control pr_state js-example-basic-single" runat="server" OnSelectedIndexChanged="ddl_client_name_SelectedIndexChanged" AutoPostBack="true">
                        </asp:DropDownList>
                    </div>
                    <div class="col-sm-2 col-xs-12 ">
                        <%--<b>State Name :</b>--%>
                        <asp:DropDownList ID="ddl_state_name" Visible="false" runat="server" class="form-control pr_state js-example-basic-single" OnSelectedIndexChanged="ddl_state_name_SelectedIndexChanged" AutoPostBack="true">
                        </asp:DropDownList>
                    </div>

                    <div class="col-sm-2 col-xs-12 ">
                        <%--<b>Branch Name :   </b>--%>
                        <asp:DropDownList ID="ddl_unitcode" Visible="false" class="form-control pr_state js-example-basic-single" runat="server" OnSelectedIndexChanged="ddl_unitcode_SelectedIndexChanged" AutoPostBack="true" />
                    </div>
                    <div class="col-sm-2 col-xs-12 ">
                        <%-- <b>Feedback Month :   </b>--%>
                        <asp:TextBox ID="txt_monthyear" runat="server" Visible="false" CssClass="form-control date-picker" />
                    </div>
                    <div class="col-sm-2 col-xs-12" style="margin-top: 1.5em">
                        <asp:Button ID="btn_send_feedback_link" Visible="false" runat="server" class="btn btn-primary" OnClick="btn_send_feedback_link_Click" Text="Send Feedback Link" OnClientClick="return Req_validation1();" Style="width: 200px;" />
                    </div>
                    <div class="col-sm-2 col-xs-12" style="margin-top: 1.5em">
                        <asp:Button ID="btn_get_report" runat="server" Visible="false" class="btn btn-primary" OnClick="btn_get_report_Click" Text="Report" OnClientClick="return valid_email();" Style="width: 150px;" />
                    </div>
                    <br />
                </div>
                <asp:Panel ID="Panel1" Visible="false" runat="server">
                    <asp:GridView ID="grd_feedback" class="table" runat="server" BackColor="White"
                        BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3"
                        OnRowDataBound="grd_feedback_RowDataBound" AutoGenerateColumns="False" OnPreRender="grd_feedback_PreRender">
                        <FooterStyle BackColor="White" ForeColor="#000066" />
                        <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" CssClass="text-uppercase" />
                        <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Center" />
                        <RowStyle ForeColor="#000066" BackColor="#ffffff" />
                        <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                        <SortedAscendingCellStyle BackColor="#F1F1F1" />
                        <SortedAscendingHeaderStyle BackColor="#007DBB" />
                        <SortedDescendingCellStyle BackColor="#CAC9C9" />
                        <SortedDescendingHeaderStyle BackColor="#00547E" />
                        <Columns>
                            <asp:TemplateField HeaderText="Sr No.">
                                <ItemTemplate>
                                    <asp:Label ID="lbl_srnumber" runat="server" Text='<%# Container.DataItemIndex+1 %>' Width="20px"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="client_name" HeaderText="CLIENT NAME" SortExpression="client_name" />
                            <asp:BoundField DataField="state_name" HeaderText="STATE" SortExpression="state_name" />
                            <asp:BoundField DataField="month" HeaderText="MONTH" SortExpression="month" />
                            <asp:BoundField DataField="year" HeaderText="YEAR" SortExpression="year" />
                            <asp:BoundField DataField="unit_name" HeaderText="BRANCH" SortExpression="unit_name" />
                            <asp:BoundField DataField="email_sent" HeaderText="EMAIL SENT" SortExpression="email_sent" />
                            <asp:BoundField DataField="percent" HeaderText="FEEDBACK PERCENT" SortExpression="percent" />
                        </Columns>
                    </asp:GridView>
                    <div class="row text-center">
                        <asp:Button ID="btn_download" runat="server" class="btn btn-primary" OnClick="btn_download_Click" Text="Download" Style="width: 150px;" />
                    </div>
                </asp:Panel>
            </div>
            
            
            
            
             </div>
    </div>
</asp:content>
