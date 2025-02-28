<%@ Page Title="Download Invoices" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="reprint_invoice.aspx.cs" Inherits="reprint_invoice" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cph_title" runat="Server">
    <title>Print Invoices</title>
    <style type="text/css">
        .text-red {
            color: #f00;
        }

        .nt_style {
            color: red;
            font: bold;
            text-align: center;
        }

        .HeaderFreez {
            position: relative;
            top: expression(this.offsetParent.scrollTop);
            z-index: 10;
        }

        button, input, optgroup, select, textarea {
            color: inherit;
            margin: 0 0 0 0px;
        }

        * {
            box-sizing: border-box;
        }

        .tab-section {
            background-color: #fff;
        }

        .form-control {
            display: inline;
        }

        .grid-view {
            height: auto;
            overflow-x: hidden;
            overflow-y: auto;
            max-height: 300px;
            width: 100%;
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_header" runat="Server">
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
    <link href="css/select2.min.css" rel="stylesheet" />
    <script src="js/hashfunction.js"></script>
    <link href="css/new_stylesheet.css" rel="stylesheet" />
    <script src="js/select2.min.js"></script>
    <link href="css/GridViewFreezeStyle.css" rel="stylesheet" type="text/css" />
    <link href="css/new_stylesheet.css" rel="stylesheet" />

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

        function pageLoad() {
            $('.date-picker').datepicker({
                changeMonth: true,
                changeYear: true,
                maxDate: 0,
                yearRange: "1990:+100",
                showButtonPanel: true,
                dateFormat: 'mm/yy',
                onClose: function (dateText, inst) {
                    $('.ui-datepicker-calendar').detach();
                    var month = $("#ui-datepicker-div .ui-datepicker-month :selected").val();
                    var year = $("#ui-datepicker-div .ui-datepicker-year :selected").val();
                    $(this).datepicker('setDate', new Date(year, month, 1));

                }
            }).click(function () {
                $('.ui-datepicker-calendar').hide();
            });

            $(".date-picker").attr("readonly", "true");
            support_format1();
        }

        function support_format1() {

            var Selected_ddl_client = ddl_client.options[ddl_client.selectedIndex].text;
            if (Selected_ddl_client == "Dewan Housing Finance Corporation Limited" || Selected_ddl_client == "Piramal Capital & Housing Finance Limited") {
                $(".region").show();
            }

            else { $(".region").hide(); }

            if (Selected_ddl_client == "BAJAJ ALLIANZ GENERAL INSURANCE COMPANY LTD.") {
                $(".region").show();
            }

            if (Selected_ddl_client == "BAJAJ FINANCE LIMITED") {
                $(".billingProcess").show();
            }
            else { $(".billingProcess").hide(); }

        }

        function req_validation_process() {
            var txt_month = document.getElementById('<%=txt_month_year.ClientID %>');
            var txt_client_code = document.getElementById('<%=ddl_client.ClientID %>');
            var Selected_client = txt_client_code.options[txt_client_code.selectedIndex].text;
            var ddl_billing_state = document.getElementById('<%=ddl_billing_state.ClientID %>');
            var Selected_state = ddl_billing_state.options[ddl_billing_state.selectedIndex].text;
            var txt_unitcode = document.getElementById('<%=ddl_unitcode.ClientID %>');
            var Selected_unit = txt_unitcode.options[txt_unitcode.selectedIndex].text;

            if (txt_month.value == "") {
                alert("Please Select month & year ");
                txt_month.focus();
                return false;
            }

            if (Selected_client == "Select") {
                alert("Please Select Client Name ");
                txt_client_code.focus();
                return false;
            }
            if (Selected_client == "ALL") {
                alert("Please Select Client Name ");
                txt_client_code.focus();
                return false;
            }
            if (Selected_state == "Select") {
                alert("Please Select State Name ");
                txt_client_code.focus();
                return false;
            }
            if (Selected_unit == "Select") {
                alert("Please Select Branch Name ");
                txt_unitcode.focus();
                return false;
            }

            $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
            return true;
        }
    </script>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cph_righrbody" runat="Server">

    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="container-fluid">
        <div class="panel panel-primary" style="background: beige;">
            <div class="panel-heading">
                <div class="row">
                    <div class="col-sm-1"></div>
                    <div class="col-sm-9">
                        <div style="text-align: center; color: #fff; font-size: 16px;" class="text-center text-uppercase"><b>Reprint Invoice Detail</b></div>
                    </div>
                    <div class="col-sm-2 text-right">
                        <asp:LinkButton ID="LinkButton1" runat="server" OnClientClick="openWindow();return false;" Style="font-size: 10px;">
                            <asp:Image runat="server" ID="Image1" Width="20" Height="20" ToolTip="Help" ImageUrl="Images/help_ico.png" />
                        </asp:LinkButton>
                    </div>
                </div>
            </div>
            <div class="panel-body">
                <asp:Panel ID="Panel1" runat="server" CssClass="panel panel-primary" Style="border-color: gray; background: #f6f3ff">
                    <br />
                    <div class="container-fluid">
                        <div class="row">
                            <div class="text-center">
                                <div class=" col-md-2 col-xs-12">
                                    <b>Select Month :</b><span class="text-red">*</span>
                                    <asp:TextBox ID="txt_month_year" Class="form-control date-picker" runat="server"></asp:TextBox>
                                </div>
                                <div class="col-lg-2 col-md-2 col-sm-3 col-xs-12">
                                    <b>Client Name :</b><span class="text-red">*</span>
                                    <asp:DropDownList ID="ddl_client" DataValueField="client_code" DataTextField="client_name" OnSelectedIndexChanged="ddl_client_SelectedIndexChanged" AutoPostBack="true" runat="server" CssClass="form-control">
                                    </asp:DropDownList>
                                </div>
                                <div class="col-sm-2 col-xs-12" id="div_region" runat="server" visible="false">
                                    <b>Region :</b>
                                    <asp:DropDownList ID="ddlregion" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddlregion_SelectedIndexChanged" AutoPostBack="true">
                                    </asp:DropDownList>
                                </div>
                                <div class="col-lg-2 col-md-2 col-sm-3 col-xs-12">
                                    <b>State Name:</b><span class="text-red">*</span>
                                    <asp:DropDownList ID="ddl_billing_state" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddl_state_SelectedIndexChanged" AutoPostBack="true" />
                                </div>
                                <div class="col-lg-2 col-md-2 col-sm-3 col-xs-12">
                                    <b>Branch Name :</b>
                                    <asp:DropDownList ID="ddl_unitcode" DataValueField="unit_code" DataTextField="unit_name" runat="server" CssClass="form-control" OnSelectedIndexChanged="ddl_unitcode_SelectedIndexChanged" AutoPostBack="true">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <br />
                        <div class="row">
                            <br />
                            <div class="text-center">
                                <asp:Button ID="btn_show" runat="server" CssClass="btn btn-primary" Text=" Show " OnClick="btn_show_Click" OnClientClick="return req_validation_process();" />
                            </div>
                        </div>
                        <br />
                        <div id="div_gv" class="container-fluid" runat="server" cssclass="grid-view" style="overflow-x: hidden;">
                            <asp:Panel ID="panel_gv" runat="server">
                                <asp:GridView ID="gv_invoice" class="table" runat="server" BackColor="White" ForeColor="#333333" CellPadding="1" Font-Size="X-Small"
                                    BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" DataKeyNames="id" AutoGenerateColumns="False" OnRowCommand="gv_invoice_RowCommand" OnPreRender="gv_invoice_PreRender" OnRowDataBound="gv_invoice_RowDataBound">
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
                                        <asp:BoundField DataField="id" HeaderText="ID" SortExpression="id" />
                                        <asp:TemplateField HeaderText="Sr No.">
                                            <ItemTemplate>
                                                <asp:Label ID="lbl_srnumber" runat="server" Text='<%# Container.DataItemIndex+1 %>' Width="20px"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="invoice_no" HeaderText="Invoice No" SortExpression="invoice_no" />
                                        <asp:BoundField DataField="invoice_date" HeaderText="invoice Date" SortExpression="invoice_date" />
                                        <asp:BoundField DataField="region" HeaderText="Region" SortExpression="region" />
                                        <asp:BoundField DataField="state_name" HeaderText="State Name" SortExpression="state_name" />
                                        <asp:BoundField DataField="unit_name" HeaderText="Branch Name" SortExpression="unit_name" />
                                        <asp:BoundField DataField="type" HeaderText="Billing Type" SortExpression="type" />
                                        <asp:BoundField DataField="month_year" HeaderText="Month - Year" SortExpression="month_year" />
                                        <asp:BoundField DataField="billing_amt" HeaderText="Total Amount" SortExpression="billing_amt" />
                                        <asp:TemplateField HeaderText="DOWNLOAD Invoices">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnk_download_invoices" runat="server" Width="100%" CausesValidation="false" Text="Invoices" Style="color: white" OnCommand="lnk_download_invoices_Command" CssClass="btn btn-primary"></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="DOWNLOAD Financecopy">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnk_download_financecopy" runat="server" Width="100%" CausesValidation="false" Text="Financopy" Style="color: white" OnCommand="lnk_download_financecopy_Command" CssClass="btn btn-primary"></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="DOWNLOAD Breakup">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnk_download_breakup" runat="server" Width="100%" CausesValidation="false" Text="Breakup" Style="color: white" OnCommand="lnk_download_breakup_Command" CssClass="btn btn-primary"></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="DOWNLOAD Attendance">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnk_download_attendance" runat="server" Width="100%" CausesValidation="false" Text="Attendance" Style="color: white" OnCommand="lnk_download_attendance_Command" CssClass="btn btn-primary"></asp:LinkButton>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </asp:Panel>
                        </div>
                        <br />
                    </div>
                </asp:Panel>
            </div>
        </div>
    </div>
</asp:Content>
