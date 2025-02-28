<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Reports.aspx.cs" Inherits="Reports" Title="Monthly Report" EnableEventValidation="false" %>


<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph_title" runat="Server">
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
    <script src="datatable/jquery.dataTables.min.js"></script>
    <script src="datatable/dataTables.bootstrap.min.js"></script>
    <script src="datatable/dataTables.buttons.min.js"></script>
    <script src="datatable/buttons.bootstrap.min.js"></script>
    <script src="datatable/vfs_fonts.js"></script>
    <script src="datatable/buttons.html5.min.js"></script>
    <script src="datatable/buttons.print.min.js"></script>
    <script src="datatable/buttons.colVis.min.js"></script>

    <script type="text/javascript">
        function unblock() {
            $.unblockUI();
        }
        function pageLoad() {
            $('#<%=ddl_client.ClientID%>').change(function () {
                $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
            });
            $('#<%=ddl_act.ClientID%>').change(function () {
                $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
            });

            $('#<%=btnclear.ClientID%>').click(function () {
                $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
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

            $(".date-picker").attr("readonly", "true");

            $(".datepicker").datepicker({
                changeMonth: true,
                changeYear: true,
                dateFormat: 'dd/mm/yy',
                yearRange: '1950',
                maxDate: 36,
                onSelect: function (value, ui) {


                }
            }).click(function () {
                $('.ui-datepicker-calendar').show();
            });
            $(".datepicker").attr("readonly", "true");
        }


    </script>

    <link href="css/GridViewFreezeStyle.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .grid-view {
            height: auto;
            max-height: 500px;
            overflow: scroll;
        }
    </style>
    <style>
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

        .ui-datepicker-calendar {
            display: none;
        }
    </style>
    <script type="text/javascript">

        function Req_validation() {

            var ddl_client_name = document.getElementById('<%=ddl_client.ClientID %>');
            var Selected_client = ddl_client_name.options[ddl_client_name.selectedIndex].text;
            var txt_date = document.getElementById('<%=txttodate.ClientID %>');

            if (Selected_client == "Select") {
                alert("Please Select Client Name ");
                ddl_client_name.focus();
                return false;
            }

            if (txt_date.value == "") {
                alert("Please Select Month / Year.");
                txt_date.focus();
                return false;
            }

            $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
            return true;

        }

        function Reg_validate_2() {

            var ddl_client_name1 = document.getElementById('<%=ddl_client.ClientID %>');
            var Selected_client1 = ddl_client_name1.options[ddl_client_name1.selectedIndex].text;
            var txt_date = document.getElementById('<%=txttodate.ClientID %>');

            if (Selected_client1 == "Select") {
                alert("Please Select Client Name ");
                ddl_client_name1.focus();
                return false;
            }

            if (txt_date.value == "") {
                alert("Please Select to Month / Year.");
                txt_date.focus();
                return false;
            }

            return true;

        }

        function validte_form_D() {

            var ddl_client_name = document.getElementById('<%=ddl_client.ClientID %>');
            var Selected_client = ddl_client_name.options[ddl_client_name.selectedIndex].text;

            if (Selected_client == "Select") {
                alert("Please Select Client Name ");
                ddl_client_name.focus();
                return false;
            }


            var txt_state = document.getElementById('<%=ddl_state.ClientID %>');
            var Selected_state = txt_state.options[txt_state.selectedIndex].text;



            if (Selected_state == "Select") {
                alert("Please Select State");
                txt_state.focus();
                return false;
            }

            return true;

        }

        function validte_form_B() {

            var ddl_client_name = document.getElementById('<%=ddl_client.ClientID %>');
            var Selected_client = ddl_client_name.options[ddl_client_name.selectedIndex].text;
            var txt_from = document.getElementById('<%=txttodate.ClientID %>');


            if (Selected_client == "Select") {
                alert("Please Select Client Name ");
                ddl_client_name.focus();
                return false;
            }

            if (txt_from.value == "") {
                alert("Please Select Month / Year.");
                txt_from.focus();
                return false;
            }

            return true;

        }

        function Reg_val_dispatch() {
            var ddl_client_name = document.getElementById('<%=ddl_client.ClientID %>');
            var Selected_client = ddl_client_name.options[ddl_client_name.selectedIndex].text;
            var txt_from = document.getElementById('<%=txt_print_list.ClientID %>');


            if (Selected_client == "Select") {
                alert("Please Select Client Name ");
                ddl_client_name.focus();
                return false;
            }

            if (txt_from.value == "") {
                alert("Please Select Date.");
                txt_from.focus();
                return false;
            }
            var txt_lot = document.getElementById('<%=txt_lot.ClientID %>');
            if (txt_lot.value == "") {
                alert("Please Enter Lot Number.");
                txt_lot.focus();
                return false;
            }

            return true;

        }

        function openWindow() {
            window.open("html/Reports.html", 'popUpWindow', 'height=500,width=600,left=100,top=100,toolbar=no,menubar=no,location=no,directories=no,scrollbars=yes, status=No');
        }
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


</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph_righrbody" runat="Server">

    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="container-fluid">
        <%-- <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>--%>


        <asp:Panel ID="Panel1" runat="server" CssClass="panel panel-primary">

            <div class="panel-heading">
                <div class="row">
                    <div class="col-sm-1"></div>
                    <div class="col-sm-9">
                        <div style="color: #fff; font-size: small;" class="text-center text-uppercase"><b>Monthly Report</b></div>
                    </div>
                    <div class="col-sm-2 text-right">
                        <asp:LinkButton ID="LinkButton1" runat="server" OnClientClick="openWindow();return false;" Style="font-size: 10px;">
                            <asp:Image runat="server" ID="Image1" Width="20" Height="20" ToolTip="Help" ImageUrl="Images/help_ico.png" />
                        </asp:LinkButton>
                    </div>
                </div>
            </div>
            <br />
           <%-- <div class="panel-heading">
                <div class="row">
                    <div class="col-sm-4"></div>
                    <div class="col-sm-9">
                        <div style="text-align: left; color:white; font-size: small;"><b>Monthly Report Details</b></div>
                    </div>
                    <div class="col-sm-2 text-left">
                        
                    </div>
                </div>
            </div>--%>

            <div class="panel-body">
                <div class="container-fluid" style="background: #f3f1fe; border: 1px solid #e2e2dd; border-radius: 10px; padding:25px 25px 25px 25px; margin-bottom:20px; margin-top:20px">
                <asp:UpdatePanel runat="server">
                    <ContentTemplate>
                        <div class="row">
                             <div class="col-sm-1 col-xs-12">
                                    <asp:Label ID="Lbltodate" runat="server" Font-Bold="true" Text="Month/Year :"></asp:Label>
                                    <asp:TextBox ID="txttodate" runat="server" class="form-control date-picker text_box" Width="95%"></asp:TextBox>
                                </div>
                            <div class="col-sm-2 col-xs-12">
                               <b> Client Name:</b>
                        <asp:DropDownList ID="ddl_client" DataValueField="client_code" DataTextField="client_name" OnSelectedIndexChanged="ddl_client_SelectedIndexChanged" AutoPostBack="true" runat="server" CssClass="form-control">
                        </asp:DropDownList>
                            </div>
                            <asp:Panel ID="unit_panel" runat="server">
                                <div class="col-sm-2 col-xs-12 ">
                                   <b> Select State :</b>
                            <asp:DropDownList ID="ddl_state" runat="server" OnSelectedIndexChanged="ddl_state_SelectedIndexChanged" AutoPostBack="true" CssClass="form-control">
                            </asp:DropDownList>
                                </div>
                                <div class="col-sm-2 col-xs-12">
                                    <asp:Label ID="Label1" runat="server" Font-Bold="true" Text="Branch Name :"></asp:Label>
                                    <asp:DropDownList runat="server" ID="ddlunitselect" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlunitselect_SelectedIndexChanged"></asp:DropDownList>
                                </div>
                                <div class="col-sm-2 col-xs-12 ">
                                   <b> Select Designation :</b>
                            <asp:DropDownList ID="ddl_designation" DataValueField="GRADE_CODE" DataTextField="GRADE_DESC" runat="server" CssClass="form-control">
                            </asp:DropDownList>
                                </div>

                                <%--<div class="col-sm-1 col-xs-12 ">
                                    From Month :
                            <asp:TextBox ID="txt_from_month_year" CssClass="form-control date-picker" runat="server"></asp:TextBox>
                                </div>--%>
                               
                            </asp:Panel>
                            <%--<div class="col-sm-2 col-xs-12"><br />
                         <asp:FileUpload ID="FileUpload1" runat="server" />
                        <span ><asp:Label ID="Label2" Visible="false" runat="server" ></asp:Label></span>
                      
                    </div>
                    <div class="col-sm-1 col-xs-12"><br />
                          <asp:Button ID="btn_upload" runat="server" CssClass="btn btn-danger"
                            OnClick="btn_upload_Click" Text="Upload"  /> <br/>--%>
                            <div class="col-sm-2 col-xs-12">
                                <br />
                                <asp:Button ID="btnclear" runat="server" CssClass="btn btn-primary"
                                    OnClick="btnclear_Click" Text="Clear" />
                                <asp:Button ID="btnclose" runat="server" CssClass="btn btn-danger"
                                    OnClick="btnclose_Click" Text="Close" />
                            </div>
                        </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <br />
                <div class="row text-center">
                    <span>
                        <asp:Button ID="btn_pf_challan_dwnld" runat="server" CssClass="btn btn-primary"
                            OnClick="btn_pfchallanD_Click" Text="PF Challan Download" />
                        <asp:Button ID="btn_kyc_Download" runat="server" CssClass="btn btn-primary"
                            OnClick="btnkycdownload_Click" Text="KYC Adhar Download" />
                        <asp:Button ID="btn_kyc_bankdownload" runat="server" CssClass="btn btn-primary"
                            OnClick="btn_kyc_bank_down" Text="KYC Bank Account Download" Visible="false" />
                        <asp:Button ID="btn_kyc_pandownload" runat="server" CssClass="btn btn-primary"
                            OnClick="btn_kyc_pan_down" Text="KYC Pan Account Download" Visible="false" />
                        <asp:Button ID="btnsendemail" runat="server" CssClass="btn btn-primary"
                            OnClick="btn_send_email_Click" Text="Send Email" />
                        <asp:Button ID="btn_pf_challan_Xmls" runat="server" CssClass="btn btn-primary"
                            OnClick="btn_pfchallan_xmls" Text="PF Challan Excel" />
                        <asp:Button ID="btn_uan_xml" runat="server" CssClass="btn btn-primary"
                            OnClick="btn_uan_xml_Click" Text="UAN XL Download" Visible="false" />
                        <asp:Button ID="btn_uan_csv" runat="server" CssClass="btn btn-primary"
                            OnClick="btn_uan_csv_Click" Text="UAN CSV Download" Visible="false" />
                    </span>
                </div>
                <br />
                    <div id="tabs" style="background: #f3f1fe; padding:20px 20px 20px 20px; border-radius:10px">
                <ul class="nav nav-tabs" >
                    <li class="active"><a data-toggle="tab" href="#menu5"><b>Compliance</b></a></li>
                    <li><a data-toggle="tab" href="#home"><b>Salary Details</b></a></li>
                    <li><a data-toggle="tab" href="#menu1"><b>PF Details</b></a></li>
                    <li><a data-toggle="tab" href="#menu2"><b>ESIC Details</b></a></li>
                    <li><a data-toggle="tab" href="#menu3"><b>MLWF Details</b></a></li>
                    <li><a data-toggle="tab" href="#menu4"><b>Other Details</b></a></li>
                    <li><a data-toggle="tab" href="#menu6"><b>ID Cards</b></a></li>
                    <li><a data-toggle="tab" href="#menu7"><b>GST</b></a></li>
                    <li><a data-toggle="tab" href="#menu8"><b>LWF</b></a></li>
                    <li><a data-toggle="tab" href="#menu10"><b>Generate Form 5</b></a></li>
                </ul>
                <div class="tab-content">
                    <div id="home" class="tab-pane fade">
                        <br />
                        <div class="row">
                            <br />
                            <div class="col-sm-12 col-xs-12">
                                <asp:Button runat="server"
                                    Text="1) Salary Statement" ID="btnsalarystatement"
                                    OnClick="btnsalarystatement_Click" CssClass="btn btn-primary"
                                    OnClientClick="return Req_validation();" Width="15%" />

                                <asp:Button ID="btnsalarystaeemntbyunit" runat="server"
                                    OnClick="btnsalarystaeemntbyunit_Click"
                                    Text="2) Salary Statement Unit wise" Width="20%"
                                    CssClass="btn btn-primary" OnClientClick="return Req_validation();" />

                                <asp:Button ID="btn_bank" runat="server"
                                    OnClick="btn_bank_Click" OnClientClick="return Req_validation();"
                                    Text="3) Bank Excel" Width="10%"
                                    class="btn btn-primary" />

                                <asp:Button ID="btnptaxstatement" runat="server"
                                    OnClick="btnptaxstatement_Click" Text="4) PTAX Statement" Width="15%"
                                    CssClass="btn btn-primary" OnClientClick="return Req_validation();" />

                                <asp:Button ID="btnsalaryexcel" runat="server" Width="20%"
                                    OnClick="btnsalaryexcel_Click" Text="5) Salary Statement Excel"
                                    CssClass="btn btn-primary" OnClientClick="return Req_validation();" />
                            </div>
                        </div>
                        <br />
                    </div>
                    <div id="menu1" class="tab-pane fade">
                        <br />
                        <br />
                        <div class="row">
                            <br />
                            <div class="col-sm-12 col-xs-12">
                                <asp:Button ID="btnpfstatement" align="right" runat="server" Width="15%"
                                    Text="1) PF Statement" OnClick="btnpfstatement_Click" CssClass="btn btn-primary"
                                    OnClientClick="return Req_validation();" />

                                <asp:Button ID="btnpfchallan" runat="server"
                                    OnClick="btnpfchallan_Click" Text="2) PF Challan" Width="15%"
                                    CssClass="btn btn-primary" OnClientClick="return Req_validation();" />

                                <asp:Button ID="btnpfchallanunitsumm" runat="server"
                                    OnClick="btnpfchallanunitsumm_Click" Text="3) PF Challan Unitwise" Width="20%"
                                    CssClass="btn btn-primary" OnClientClick="return Req_validation();" />

                                <asp:Button ID="btn_pf_summary" runat="server" CssClass="btn btn-primary"
                                    OnClick="btn_pf_summary_Click" Text="4) PF Summary" Width="15%"
                                    OnClientClick="return Req_validation();" />

                                <asp:Button ID="btn_PF_Challan" runat="server" CssClass="btn btn-primary"
                                    OnClick="btn_pf_challan_Click" Text="5) PF Challan Download" Width="20%"
                                    OnClientClick="return Req_validation();" />
                            </div>
                            <br />
                            <div class="col-sm-12 col-xs-12">
                                <br />
                                <asp:Button ID="btncustomerwisesalarysummary" runat="server"
                                    Text="6) Customerwise Salary Summary" Width="20%"
                                    OnClick="btncustomerwisesalarysummary_Click" Visible="false"
                                    CssClass="btn btn-primary" OnClientClick="return Req_validation();" />

                                <asp:Button ID="btn_pf_return" runat="server" CssClass="btn btn-primary"
                                    OnClick="btn_pf_return_Click" Text="7) PF Return" Width="15%"
                                    OnClientClick="return Req_validation();" />


                            </div>

                        </div>
                        <br />
                    </div>
                    <div id="menu2" class="tab-pane fade">
                        <br />
                        <br />
                        <div class="row">
                            <br />
                            <div class="col-sm-12 col-xs-12">
                                <asp:Button ID="btnesicstatement" runat="server" Width="15%"
                                    OnClick="btnesicstatement_Click" Text="1) ESIC Statement" CssClass="btn btn-primary"
                                    OnClientClick="return Req_validation();" />

                                <asp:Button ID="btnesicexcel" runat="server" Width="20%"
                                    OnClick="btnesicexcel_Click" Text="2) ESIC Statement Excel"
                                    CssClass="btn btn-primary" OnClientClick="return Req_validation();" />


                                <asp:Button ID="btnesicchallan" runat="server" Width="15%"
                                    OnClick="btnesicchallan_Click" Text="3) ESIC Challan"
                                    CssClass="btn btn-primary" OnClientClick="return Req_validation();" />

                                <asp:Button ID="btnesicchallanunit" runat="server" Text="4) Esic Summary Unitwise"
                                    CssClass="btn btn-primary" OnClick="btnesicchallanunit_Click" Width="20%"
                                    OnClientClick="return Req_validation();" />

                                <asp:Button ID="btn_esic_summary" runat="server" CssClass="btn btn-primary"
                                    OnClick="btn_esic_summary_Click" Text="5) ESIC Summary" Width="15%"
                                    OnClientClick="return Req_validation();" />

                            </div>
                        </div>
                        <br />
                    </div>
                    <div id="menu3" class="tab-pane fade">
                        <br />
                        <br />
                        <div class="row">
                            <br />
                            <div class="col-sm-12 col-xs-12">
                                <asp:Button ID="btnmlwfsttement" runat="server" CssClass="btn btn-primary"
                                    OnClick="btnmlwfsttement_Click" Text="1) MLWF Statement" Width="15%"
                                    OnClientClick="return Req_validation();" />


                                <asp:Button ID="btnmlwfstatementdetails" runat="server"
                                    Text="2) MLWF Statement Details" CssClass="btn btn-primary" Width="20%"
                                    OnClick="btnmlwfstatementdetails_Click" OnClientClick="return Req_validation();" />

                                <asp:Button ID="btn_monthly_mlwf" runat="server" CssClass="btn btn-primary"
                                    OnClick="btn_monthly_mlwf_Click" Text="3) Monthly MLWF" Width="15%"
                                    OnClientClick="return Req_validation();" />

                                <asp:Button ID="btn_lwf_chalan" runat="server" CssClass="btn btn-primary"
                                    OnClick="btn_lwf_chalan_Click" Text="4) LWF Chalan" Width="15%"
                                    OnClientClick="return Req_validation();" />



                            </div>
                        </div>
                        <br />
                    </div>
                    <div id="menu5" class="tab-pane fade in active">
                        <br />
                        <br />
                        <div class="row">

                            <br />
                            <div class="col-sm-2 col-xs-12 ">
                               <b>Select Act :</b>
                            <asp:DropDownList ID="ddl_act" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddl_act_SelectedIndexChanged">
                                <asp:ListItem Value="CHILD LABOUR ACT" Text="CHILD LABOUR ACT" />
                                <asp:ListItem Value="CLRA ACT" Text="CLRA ACT" />
                                <asp:ListItem Value="S&E ACT" Text="S&E ACT" />
                                <asp:ListItem Value="MB ACT" Text="MB ACT" />
                                <asp:ListItem Value="MW ACT" Text="MW ACT" />
                                <asp:ListItem Value="PAYMENT WAGES ACT" Text="PAYMENT WAGES ACT" />
                                <asp:ListItem Value="N & FH ACT" Text="N & FH ACT" />
                                <asp:ListItem Value="EPF ACT" Text="EPF ACT" />
                                <asp:ListItem Value="ESI ACT" Text="ESI ACT" />
                                <asp:ListItem Value="EC ACT" Text="EC ACT" />
                                <asp:ListItem Value="LWF ACT" Text="LWF ACT" />
                                <asp:ListItem Value="MH SG ACT" Text="MH SG ACT" />
                            </asp:DropDownList>
                            </div>
                            <div class="col-sm-10 col-xs-12 " style="margin-top: 2em">
                                <asp:Button ID="btn_formno_A" runat="server" OnClick="btn_formno_A_Click" Text="FORM A" CssClass="btn btn-primary" OnClientClick="return Reg_validate_2();" Width="15%" />
                                <asp:Button ID="btn_formno_B" runat="server" OnClick="btn_formno_B_Click" Text="FORM B" CssClass="btn btn-primary" OnClientClick="return validte_form_B();" Width="15%" />
                                <asp:Button ID="btn_formd" runat="server" class="btn btn-primary" OnClick="btn_formd_Click" OnClientClick="return validte_form_D();" Text="FORM D" Width="15%" />
                                <asp:Button ID="btn_form_13" runat="server" class="btn btn-primary" OnClick="btn_form_13_Click" Text="FORM XIII BELOW 18" OnClientClick="return Reg_validate_2();" Width="20%" />
                                <asp:Button ID="btn_form_13_above_18" runat="server" class="btn btn-primary" OnClick="btn_form_13_above_18_Click" Text="FORM XIII Above 18" OnClientClick="return Reg_validate_2();" Width="20%"/>
                                <asp:Button ID="btn_form_14" runat="server" class="btn btn-primary" OnClick="btn_form_14_Click" Text="FORM XIV" OnClientClick="return Reg_validate_2();" Width="15%" />
                                <asp:Button ID="btn_form_15" runat="server" class="btn btn-primary" OnClick="btn_form_15_Click" Text="FORM XV" OnClientClick="return Reg_validate_2();" Width="15%" />
                                <asp:Button ID="Button1" runat="server" class="btn btn-primary" OnClick="btn_form_16_Click1" Text="FORM XVI" OnClientClick="return Reg_validate_2();" Width="15%" />
                                <asp:Button ID="btn_form_20" runat="server" class="btn btn-primary" OnClick="btn_form_20_Click" Text="FORM XX" OnClientClick="return Reg_validate_2();" Width="15%"/>
                                <asp:Button ID="btn_form_21" runat="server" class="btn btn-primary" OnClick="btn_form_21_Click" Text="FORM XXI" OnClientClick="return Reg_validate_2();" Width="15%"/>
                                <asp:Button ID="btn_form_22" runat="server" class="btn btn-primary" OnClick="btn_form_22_Click" Text="FORM XXII" OnClientClick="return Reg_validate_2();" Width="15%"/>
                                <asp:Button ID="btn_form_23" runat="server" class="btn btn-primary" OnClick="btn_form_23_Click" Text="FORM XXIII" OnClientClick="return Reg_validate_2();" Width="15%"/>
                                <asp:Button ID="btn_bonus_register" runat="server" class="btn btn-primary" OnClick="btn_bonus_register_Click" Text="BONUS REGISTER" OnClientClick="return Reg_validate_2();" Width="15%"/>
                                <asp:Button ID="btn_leave_record" runat="server" class="btn btn-primary" OnClick="btn_leave_record_Click" Text="LEAVE RECORD" OnClientClick="return Reg_validate_2();" Width="15%"/>
                                <asp:Button ID="btn_employee_data1" runat="server" class="btn btn-primary" OnClick="btn_employee_data_Click" Text="EMPLOYEE DATA" OnClientClick="return Reg_validate_2();" Width="15%" />

                            </div>
                        </div>
                        <br />
                        <asp:Panel ID="document_upload" runat="server" CssClass="panel panel-primary">
                            <br />
                            <div class="container-fluid">
                                <br />
                                <br />
                                <div class="row" id="files_upload" runat="server">

                                    <div class="col-sm-2 col-xs-12">
                                       <b> Description</b>
                                      <asp:DropDownList ID="ddl_clra_act" runat="server" CssClass="form-control" />
                                    </div>
                                    <div class="col-sm-2 col-xs-12">
                                        <span>File to Upload :</span>
                                        <asp:FileUpload ID="document1_file" runat="server" meta:resourcekey="photo_uploadResource1" CssClass="text_box" />
                                    </div>
                                    <div class="col-sm-2 col-xs-12">
                                        <br />
                                        <asp:Button ID="btn_upload" runat="server" OnClientClick="return Req_validation();" class="btn btn-primary" OnClick="btn_upload_Click" Text="Upload" />
                                    </div>
                                    <br />
                                    <div class="col-sm-3 col-xs-12"><b style="color: #f00; text-align: center">Note :</b> Only JPG, PNG and PDF files will be uploaded.</div>

                                    <br />
                                    <br />
                                    <br />
                                    <div class="row">
                                        <div class="container">
                                            <asp:Panel ID="Panel4" runat="server" CssClass="grd_company">
                                                <asp:GridView ID="grd_company_files" class="table" runat="server" BackColor="White"
                                                    BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="3"
                                                    OnRowDataBound="grd_company_files_RowDataBound" OnRowDeleting="grd_company_files_RowDeleting" AutoGenerateColumns="False" DataKeyNames="id" Width="100%" OnPreRender="grd_company_files_files_PreRender">
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
                                                        <asp:TemplateField HeaderText="No.">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lbl_srnumber" runat="server"
                                                                    Text="<%# Container.DataItemIndex+1 %>"></asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="Id" HeaderText="ID" />
                                                        <asp:BoundField DataField="description" HeaderText="Description"
                                                            SortExpression="description" />
                                                        <asp:TemplateField HeaderText="Download File">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="lnkDownload" Text="Download" CommandArgument='<%# Eval("Value") %>' runat="server" OnClick="DownloadFile"></asp:LinkButton>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:CommandField ButtonType="Button"
                                                            ControlStyle-CssClass="btn btn-primary"
                                                            ShowDeleteButton="true" />
                                                    </Columns>
                                                </asp:GridView>
                                            </asp:Panel>
                                        </div>
                                    </div>
                                    <br />
                                    <br />
                                </div>
                            </div>


                        </asp:Panel>
                    </div>

                    <div id="menu4" class="tab-pane fade">
                        <br />
                        <br />
                        <div class="row">
                            <br />
                            <div class="col-sm-12 col-xs-12">

                                <asp:Button ID="btnformno10" runat="server"
                                    OnClick="btnformno10_Click" Text="1) Form No.10" Width="15%"
                                    CssClass="btn btn-primary" OnClientClick="return Req_validation();" />

                                <asp:Button ID="btnformno5" runat="server" CssClass="btn btn-primary"
                                    OnClick="btnformno5_Click" Text="2) Form No.5"  Width="15%"
                                    OnClientClick="return Req_validation();" />

                                <asp:Button ID="btnemployeeinfostatust" runat="server"
                                    OnClick="btnemployeeinfostatust_Click"  Width="22%"
                                    Text="3) Employee Information Status"
                                    CssClass="btn btn-primary" OnClientClick="return Req_validation();" />


                                <asp:Button ID="btnemployeeinfostatus2" runat="server"
                                    OnClick="btnemployeeinfostatus2_Click"  Width="22%"
                                    Text="4) Employee PF and ESIC Number"
                                    CssClass="btn btn-primary" OnClientClick="return Req_validation();" />

                                <asp:Button ID="btn_kyc_Upload" runat="server" CssClass="btn btn-primary"
                                    OnClick="btn_kyc_upload_Click" Text="5) KYC Download Adhar/Bank/Pan"  Width="25%"
                                    OnClientClick="return Req_validation();" />
                            </div>
                        </div>
                        <br />
                        <br />
                        <div class="row">

                            <div class="col-sm-12 col-xs-12">

                                <asp:Button ID="btn_kyc_bank" runat="server" CssClass="btn btn-primary"
                                    OnClick="btn_kyc_upload_bank" Text="6) KYC Download Bank"  Width="15%"
                                    OnClientClick="return Req_validation();" Style="display: none" />

                                <asp:Button ID="btn_kyc_pan" runat="server" CssClass="btn btn-primary"
                                    OnClick="btn_kyc_upoad_pan" Text="7) KYC Download PAN"  Width="15%"
                                    OnClientClick="return Req_validation();" Style="display: none" />

                                <asp:Button ID="btn_form_no9" runat="server" CssClass="btn btn-primary"
                                    OnClick="btn_form_no9_Click" Text="6) FORM No. 9"  Width="15%"
                                    OnClientClick="return Req_validation();" />

                                <asp:Button ID="btn_form_16" runat="server" CssClass="btn btn-primary"  Width="15%"
                                    OnClick="btn_form_16_Click" Text="7) Form 16" OnClientClick="return Req_validation();" />

                                <asp:Button ID="btnformno11" runat="server"
                                    OnClick="btnformno11_Click" Text="8) Form No.11"  Width="15%"
                                    CssClass="btn btn-primary" OnClientClick="return Req_validation();" />

                                <asp:Button ID="btnformno2" runat="server"
                                    OnClick="btnformno2_Click" Text="9) Form No.2"  Width="15%"
                                    CssClass="btn btn-primary" OnClientClick="return Req_validation();" />

                                <asp:Button ID="btnnoobjecioncerificae" runat="server"
                                    OnClick="btnnoobjecioncerificae_Click" Text="10) No Objection Certificate"  Width="22%"
                                    CssClass="btn btn-primary" OnClientClick="return Req_validation();" />


                                <asp:Button ID="btnotstatement" runat="server"
                                    OnClick="btnotstatement_Click" Text="9) " Visible="false"  Width="15%"
                                    CssClass="btn btn-primary" OnClientClick="return Req_validation();" />

                                <asp:Button ID="btnempgratutystatement" runat="server"
                                    OnClick="btnempgratutystatement_Click"
                                    Text="10)" Visible="false"
                                    CssClass="btn btn-primary" OnClientClick="return Req_validation();" />

                                <asp:Button ID="btn_enquiry_form" runat="server" OnClick="btn_enquiry_form_Click"
                                    Text="13) Enquiry_Police_form"  Width="20%"
                                    CssClass="btn btn-primary" OnClientClick="return Req_validation();" />
                            </div>
                        </div>
                        <br />
                        <div class="row">
                            <br />
                            <div class="col-sm-12 col-xs-12">
                                <asp:Button ID="btnleavestatus" runat="server"
                                    OnClick="btnleavestatus_Click" Text="11) " Visible="false"
                                    CssClass="btn btn-primary" OnClientClick="return Req_validation();" />

                                <asp:Button ID="btnbankwisesalarystamt" runat="server"
                                    OnClick="btnbankwisesalarystamt_Click" Text="12)" Visible="false"
                                    CssClass="btn btn-primary" OnClientClick="return Req_validation();" />

                                <asp:Button ID="btnempinfostatus3" runat="server"
                                    OnClick="btnempinfostatus3_Click" Text="13) DEDUCTION REPORT" Visible="false"
                                    CssClass="btn btn-primary" OnClientClick="return Req_validation();" />

                                <asp:Button ID="btnemployeemusterroll2" runat="server"
                                    OnClick="btnemployeemusterroll2_Click"
                                    Text="14) BANK PAYMENT DETAILS" Visible="false"
                                    CssClass="btn btn-primary" OnClientClick="return Req_validation();" />

                                <asp:Button ID="btnunitwisesalarysummary" runat="server"
                                    Text="15) Unitwise Salary Summary"
                                    OnClick="btnunitwisesalarysummary_Click" Visible="false"
                                    CssClass="btn btn-primary" OnClientClick="return Req_validation();" />


                            </div>
                        </div>

                        <asp:Button ID="btn_ptax_summary" runat="server" CssClass="btn btn-primary"
                            OnClick="btn_ptax_summary_Click" Text="16) PTAX Summary" Visible="false"
                            OnClientClick="return Req_validation();" />

                    </div>
                    <div id="menu6" class="tab-pane fade">
                        <br />
                        
                        <div class="row">
                                <b>ID - Print List</b>
                                <div class="col-sm-2 col-xs-4">
                                <asp:Button ID="btn_generate_id_card" runat="server"
                                    OnClick="btn_generate_id_card_Click" Text="Generate ID Card" Width="80%"
                                    CssClass="btn btn-primary" OnClientClick="return Reg_validate_2();" />
                                    </div>
                                <div class="col-sm-4 col-xs-4">
                                <asp:Button ID="btn_clientwise_all_employee_id" runat="server"
                                    OnClick="btn_clientwiseAllEmployeeID" Text="Client and Branch Wise Id Card "  Width="70%"
                                    CssClass="btn btn-primary" OnClientClick="return Reg_validate_2();" />
                                    </div>
                           
                             </div>
                        <br />
                        <div class="row">
                           <b> Dispatch Address List</b>
                            <br />
                            <div class="col-sm-2 col-xs-6">
                                <asp:TextBox ID="txt_print_list" runat="server" class="form-control datepicker"></asp:TextBox>
                            </div>
                            <div class="col-sm-6 col-xs-12">
                                <asp:Button ID="btn_dispatch_print" runat="server"
                                    OnClick="btn_dispatch_print_Click" Text="Printing List" Width="30%"
                                    CssClass="btn btn-primary" OnClientClick="return Reg_val_dispatch();" />
                            </div>
                            </div>
                        <br />
                        <div class="row">
                            <div class="col-sm-2 col-xs-12">
                                <asp:TextBox ID="txt_lot" runat="server" class="form-control" onkeypress=" return isNumber(event)" Text="1"></asp:TextBox>
                            </div>
                            <div class="col-sm-2 col-xs-12">
                                <asp:Button ID="btn_sales_register" runat="server" Width="70%"
                                    OnClick="btn_sales_register_Click" Text="Sales Register"
                                    CssClass="btn btn-primary" />
                            </div>
                            <div class="col-sm-4 col-xs-12">
                                <asp:Button ID="btn_bank_images" runat="server" Width="70%"
                                    OnClick="btn_bank_images_Click" Text="Bank Passbook Images"
                                    CssClass="btn btn-primary" OnClientClick="return Reg_validate_2();" />
                            </div>
                        </div>
                    </div>
                    <div id="menu7" class="tab-pane fade">
                        <br />
                        <br />
                        <div class="row">
                            <br />
                            <div class="col-sm-12 col-xs-12">

                                <br />
                                <asp:Button ID="btn_gst" runat="server"
                                    OnClick="btn_gst_Click" Text="Get GST"
                                    CssClass="btn btn-primary" OnClientClick="return Reg_validate_2();" />

                            </div>
                        </div>
                    </div>
                    <div id="menu8" class="tab-pane fade">
                        <br />
                        <br />
                        <div class="row">
                            <br />
                            <div class="col-sm-12 col-xs-12">

                                <br />
                                <asp:Button ID="btn_pt" runat="server"
                                    OnClick="btn_pt_Click" Text="Get PT"
                                    CssClass="btn btn-primary" OnClientClick="return Reg_validate_2();" />

                            </div>
                        </div>
                    </div>

                    <div id="menu10" class="tab-pane fade">
                        <br />
                        <br />
                        <div class="row">
                            <br />

                            <div class="col-sm-4 col-xs-12">
                                <br />
                                <asp:Button ID="btn_generateform5" runat="server" Width="35%"
                                    Text="Generate Form 5" OnClick="btn_generateform5_Click" OnClientClick="return Reg_validate_3();"
                                    CssClass="btn btn-primary" />
                            </div>
                            <%-- vikas add /05/03/2019--%>

                            <div class="col-sm-2 col-sm-12" style="margin-top: 2em;">
                                <a data-toggle="modal" href="#remain_admin"><font color="red"><b><%= rem_emp_count %></b></font>Generate Form 5 </a>

                            </div>



                        </div>
                        <%--vikas--%>
                        <div class="modal fade" id="remain_admin" role="dialog" data-dismiss="modal" href="#lost">

                            <div class="modal-dialog">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                                        <h4 style="text-align: center">Labour Regional List </h4>
                                    </div>

                                    <div class="modal-body">
                                        <div class="row">
                                            <div class="col-sm-12" style="padding-left: 1%;">
                                                <asp:GridView ID="gv_genrateform5" class="table" HeaderStyle-CssClass="FixedHeader" runat="server" AutoGenerateColumns="false" ShowFooter="false" data-toggle="modal" href="#Div1">
                                                    <Columns>
                                                        <asp:BoundField DataField="client_code" HeaderText="Client Name" SortExpression="client_code" />
                                                        <asp:BoundField DataField="state_name" HeaderText="State Name" SortExpression="state_name" />
                                                        <asp:BoundField DataField="labour_office" HeaderText="Labour Regional Office" SortExpression="labour_office" />

                                                    </Columns>
                                                </asp:GridView>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="modal-footer">
                                        <div class="row text-center">
                                            <div class="col-sm-4">
                                            </div>
                                            <div class="col-sm-4">
                                                <button type="button" class="btn btn-danger" data-dismiss="modal">CLOSE</button>
                                            </div>
                                            <div class="col-sm-4">
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                        </div>
                    </div>


                </div>
            
            <%-- <div class="col-lg-2 col-md-2 col-sm-3 col-xs-12 ">
                <asp:Label ID="Lblfromdate" runat="server" Text="From Date  :" Visible="false"></asp:Label>
                <asp:TextBox ID="txtfromdate" runat="server" class="form-control date-picker" Width="95%" Visible="false"></asp:TextBox><br />
                <%--<cc1:CalendarExtender ID="txtfromdate_CalendarExtender" runat="server" 
                                                                      Enabled="True" Format="yyyy/MM/dd" TargetControlID="txtfromdate">
                                                            </cc1:CalendarExtender>
            </div>--%>
            <div class="col-lg-2 col-md-2 col-sm-3 col-xs-12 ">
                <br />
            </div>





            <asp:Panel runat="server" CssClass="grid-view panel-body" ScrollBars="Auto">
                <asp:GridView ID="UnitGrid_PF" class="table" runat="server" Width="100%" ForeColor="#333333" ShowHeaderWhenEmpty="True" OnPreRender="UnitGrid_PF_PreRender">
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
            <br />
            <asp:Panel ID="Panel3" runat="server" CssClass="grid-view panel-body" ScrollBars="Auto">
                <asp:GridView ID="GridView2" class="table" runat="server" Width="100%" ForeColor="#333333" ShowHeaderWhenEmpty="True">
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
            <br />
            <asp:Panel ID="Panel2" CssClass="grid-view panel-body" runat="server" ScrollBars="Auto">
                <asp:GridView ID="UnitGridView" class="table" runat="server" AutoGenerateColumns="False"
                    CellPadding="0" ForeColor="#333333"
                    OnRowDataBound="UnitGridView_RowDataBound"
                    OnSelectedIndexChanged="UnitGridView_SelectedIndexChanged"
                    Width="100%" AllowSorting="True" ShowHeaderWhenEmpty="True" OnPreRender="UnitGridView_PreRender">
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
                        <asp:BoundField DataField="UAN" HeaderText="UAN"
                            SortExpression="UAN" ReadOnly="True" />
                        <asp:BoundField DataField="P_TAX_NUMBER" HeaderText="ADHAR_CARD_NO"
                            SortExpression="P_TAX_NUMBER" ReadOnly="True" />
                        <asp:BoundField DataField="EMP_NEW_PAN_NO" HeaderText="PAN_NO"
                            SortExpression="EMP_NEW_PAN_NO" ReadOnly="True" />
                        <asp:BoundField DataField="BANK_EMP_AC_CODE" HeaderText="BANK_ACCOUNT_NO"
                            SortExpression="BANK_EMP_AC_CODE" ReadOnly="True" />
                        <asp:BoundField DataField="DOCUMENT_TYPE" HeaderText="DOCUMENT_TYPE"
                            SortExpression="DOCUMENT_TYPE" ReadOnly="True" />
                        <asp:BoundField DataField="DOCUMENT_NUMBER" HeaderText="DOCUMENT_NUMBER"
                            SortExpression="DOCUMENT_NUMBER" ReadOnly="True" />
                        <asp:BoundField DataField="PF_IFSC_CODE" HeaderText="PF_IFSC_CODE"
                            SortExpression="PF_IFSC_CODE" ReadOnly="True" />
                        <asp:BoundField DataField="EMP_NAME" HeaderText="EMP_NAME"
                            SortExpression="EMP_NAME" ReadOnly="True" />
                        <asp:BoundField DataField="EMP_QUALIFICATION" HeaderText="EMP_QUALIFICATION"
                            SortExpression="EMP_QUALIFICATION" ReadOnly="True" />
                        <asp:BoundField DataField="GENDER" HeaderText="GENDER"
                            SortExpression="GENDER" ReadOnly="True" />
                        <asp:BoundField DataField="EMP_MARRITAL_STATUS" HeaderText="EMP_MARRITAL_STATUS"
                            SortExpression="EMP_MARRITAL_STATUS" ReadOnly="True" />
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

            <asp:Panel ID="panel_ptax" runat="server" CssClass="grid-view panel-body" ScrollBars="Auto">
                <asp:GridView ID="gv_ptax" class="table" runat="server" Width="100%" ForeColor="#333333" ShowHeaderWhenEmpty="True" OnPreRender="gv_ptax_PreRender">
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


            <asp:Panel ID="panel_esic_statement" CssClass="grid-view panel-body" runat="server" ScrollBars="Auto">
                <asp:GridView ID="gv_esic_statement" class="table" runat="server" AutoGenerateColumns="False"
                    CellPadding="0" ForeColor="#333333"
                    OnRowDataBound="UnitGridView_RowDataBound"
                    OnSelectedIndexChanged="UnitGridView_SelectedIndexChanged"
                    Width="100%" AllowSorting="True" ShowHeaderWhenEmpty="True" OnPreRender="gv_esic_statement_PreRender">
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
                        <asp:BoundField DataField="CLIENT_NAME" HeaderText="CLIENT_NAME"
                            SortExpression="CLIENT_NAME" ReadOnly="True" />
                        <asp:BoundField DataField="state_name" HeaderText="STATE"
                            SortExpression="state_name" ReadOnly="True" />
                        <asp:BoundField DataField="ZONE" HeaderText="LOCATION"
                            SortExpression="ZONE" ReadOnly="True" />
                        <asp:BoundField DataField="GRADE_CODE" HeaderText="DESGNATION"
                            SortExpression="GRADE_CODE" ReadOnly="True" />
                        <asp:BoundField DataField="ESIC_REG_NO" HeaderText="ESIC_NO"
                            SortExpression="ESIC_REG_NO" ReadOnly="True" />
                        <asp:BoundField DataField="EMP_NAME" HeaderText="EMPLOYEE_NAME"
                            SortExpression="EMP_NAME" ReadOnly="True" />
                        <asp:BoundField DataField="TOT_ESIC_GROSS" HeaderText="ESIC_BASIC"
                            SortExpression="TOT_ESIC_GROSS" ReadOnly="True" />
                        <asp:BoundField DataField="PRESENT_DAYS" HeaderText="WORKING_DAYS"
                            SortExpression="PRESENT_DAYS" ReadOnly="True" />
                        <asp:BoundField DataField="PAYABLE_DAYS" HeaderText="WORKING_DAYS_CALCULATION"
                            SortExpression="PAYABLE_DAYS" ReadOnly="True" />
                        <asp:BoundField DataField="ESIC_TOT" HeaderText="EMPLOYEE_E_CONTRIBUTION"
                            SortExpression="ESIC_TOT" ReadOnly="True" />
                        <asp:BoundField DataField="ESIC_COMP_CONTRI" HeaderText="EMPLOYEE_R_CONTRIBUTION"
                            SortExpression="ESIC_COMP_CONTRI" ReadOnly="True" />
                        <asp:BoundField DataField="ESIC" HeaderText="TOTAL"
                            SortExpression="ESIC" ReadOnly="True" />
                    </Columns>
                </asp:GridView>
            </asp:Panel>

            <asp:Panel ID="panel_bankexcel" runat="server" CssClass="grid-view panel-body" ScrollBars="Auto">
                <asp:GridView ID="gv_bankexcel" class="table" runat="server" Width="100%" ForeColor="#333333" ShowHeaderWhenEmpty="True" OnPreRender="panel_bankexcel_PreRender">
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


            <asp:Panel ID="panel_employee_information_status" runat="server" CssClass="grid-view panel-body" ScrollBars="Auto">
                <asp:GridView ID="gv_employee_information_status" class="table" runat="server" Width="100%" ForeColor="#333333" ShowHeaderWhenEmpty="True" OnPreRender="employee_information_status_PreRender">
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


            <asp:Panel ID="panel_employee_pf_esic_no" runat="server" CssClass="grid-view panel-body" ScrollBars="Auto">
                <asp:GridView ID="gv_employee_pf_esic_no" class="table" runat="server" Width="100%" ForeColor="#333333" ShowHeaderWhenEmpty="True" OnPreRender="employee_pf_esic_no_PreRender">
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

            <asp:Panel ID="panel_esic_summary_utwise" runat="server" CssClass="grid-view panel-body" ScrollBars="Auto">
                <asp:GridView ID="gv_esic_summary_utwise" class="table" runat="server" Width="100%" ForeColor="#333333" ShowHeaderWhenEmpty="True" OnPreRender="gv_esic_summary_utwise_PreRender">
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
                    
    </div>
    </asp:Panel>
    </div>
</asp:Content>

