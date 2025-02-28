<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="apply_expencess.aspx.cs" Inherits="apply_expencess" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_title" runat="Server">
    <title>Add Travel Plan</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_header" runat="Server">
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1" />
    <meta charset="utf-8" />
    <script src="js/jquery-1.12.3.min.js"></script>
    <script src="Scripts/jquery-1.11.3.js"></script>
    <script src="js/bootstrap.js" type="text/javascript"></script>
    <script src="Scripts/datetimepicker.js"></script>
    <script src="Scripts/jquery-ui.min.js"></script>
    <%-- <script src="js/bootstrap.min.js"></script>--%>
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
            var table = $('#<%=gv_expeness.ClientID%>').DataTable({
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
              .appendTo('#<%=gv_expeness.ClientID%>_wrapper .col-sm-6:eq(0)');

           $.fn.dataTable.ext.errMode = 'none';

       });


    </script>


    <script type="text/javascript">
        function ShowPopup() {
            $("#btnShowPopup").click();
        }

        function openWindow() {
            window.open("html/add_travel_plan.html", 'popUpWindow', 'height=500,width=600,left=100,top=100,toolbar=no,menubar=no,location=no,directories=no,scrollbars=yes, status=No');
        }


        function callfnc() {

            document.getElementById('<%= Button5.ClientID %>').click();

        }
        $(function () {

            $('#<%=lnkaddtravelplan.ClientID%>').click(function () {

                $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });

            });

            //$('.edit').click(function () {


            //        $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });

            //});

            //$('.delete').click(function () {

            //    $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });

            //});

            //$('.claim').click(function () {

            //    $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });

            //});


            $('#<%=lnkclaimexpense.ClientID%>').click(function () {

                $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });

            });

        });

    </script>
    <style type="text/css">
        element {
            position: fixed;
            z-index: 8001;
            left: -60.5px;
            top: 34.5px;
        }

        .Background {
            background-color: Black;
            filter: alpha(opacity=90);
            opacity: 0.8;
        }

        .Popup {
            background-color: #FFFFFF;
            border-width: 3px;
            border-style: solid;
            border-color: black;
            padding-top: 10px;
            padding-left: 10px;
            z-index: 101;
        }

        .lbl {
            font-size: 16px;
            font-style: italic;
            font-weight: bold;
        }

        .grid-view {
            height: auto;
            max-height: 300px;
            overflow-y: auto;
            overflow-x: hidden;
        }

        .row {
            margin: 0px;
        }
    </style>

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
                        <div style="color: #fff; font-size: small;" class="text-center text-uppercase; font-size:30px;"><b>Add Travel Plan</b></div>
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
                        <div style="text-align: left; color:white; font-size: small;"><b>Add Travel Plan Details</b></div>
                    </div>
                    <div class="col-sm-2 text-left">
                        
                    </div>
                </div>
            </div>
            <br />
            
            <div class="panel-body">

                <div class="row">

                    <div class="col-md-2 col-sm-2 col-xs-12">

                        <asp:TextBox ID="txt_emp_name" runat="server" class="form-control text_box" ReadOnly="true"
                            MaxLength="10" Visible="false"></asp:TextBox>

                    </div>

                </div>



                <div class="row text-center">
                    <%--Please dont delete this 3 Buttons...Vinod --%>
                    <asp:Button ID="Button1" runat="server" CssClass="hidden" Text="Claim Expense" />
                    <asp:Button ID="Button3" runat="server" CssClass="hidden" Text="Add New Travelling Plan" />
                    <asp:Button ID="Button5" runat="server" CssClass="hidden" OnClick="Button4_Click" />

                    <cc1:ModalPopupExtender ID="mp1" runat="server" PopupControlID="Panl1" TargetControlID="Button1"
                        CancelControlID="Button2" BackgroundCssClass="Background">
                    </cc1:ModalPopupExtender>

                    <asp:Panel ID="Panl1" runat="server" CssClass="Popup" align="center" Style="display: none">

                        <iframe style="width: 800px; height: 450px; background-color: #fff;" id="irm1" src="p_add_expencess1.aspx" runat="server"></iframe>
                        <div class="row text-center" style="width: 100%;">
                            <asp:Button ID="Button2" CssClass="btn btn-danger" OnClientClick="callfnc()" runat="server" Text="Close" />
                        </div>

                        <br />

                    </asp:Panel>

                    <cc1:ModalPopupExtender ID="ModalPopupExtender1" runat="server" PopupControlID="Panel4" TargetControlID="Button3"
                        CancelControlID="Button4" BackgroundCssClass="Background">
                    </cc1:ModalPopupExtender>
                    <asp:Panel ID="Panel4" runat="server" CssClass="Popup" Style="display: none">
                        <iframe style="width: 1000px; height: 500px; background-color: #fff;" id="Iframe1" src="p_add_new_travel_plan2.aspx" runat="server"></iframe>
                        <div class="row text-center">
                            <asp:Button ID="Button4" CssClass="btn btn-danger" OnClientClick="callfnc()" runat="server" Text="Close" />
                        </div>

                        <br />

                    </asp:Panel>
                </div>
                <div class="row" style="border: 1px solid #000; border-bottom: none; background-color: #eee;">
                    <div class="col-sm-6 col-xs-12">
                        <span class="glyphicon glyphicon-plus" style="font-size: 30px; font-weight: bolder; height: 30px; padding-left: 5px; padding-right: 5px; color: #337ab7; border-radius: 20%;"></span>
                        <asp:LinkButton ID="lnkaddtravelplan" runat="server" OnClick="lnkaddtravelplan_Click"
                            Text="New Travelling Plan" Font-Bold="true" Style="color: #000;"></asp:LinkButton>
                    </div>
                    <div class="col-sm-6 col-xs-12 text-right">
                        <asp:LinkButton ID="lnkclaimexpense" runat="server" OnClick="lnkclaimexpense_Click1"
                            Text="New Claim Expense" Font-Bold="true" CssClass="text-right" Style="color: #000;"></asp:LinkButton>
                        <span class="glyphicon glyphicon-plus" style="font-size: 30px; font-weight: bolder; height: 30px; padding-left: 5px; padding-right: 5px; color: #337ab7; border-radius: 20%"></span>
                    </div>
                </div>
                <br />
                <br />
                <asp:Panel ID="Panel2" runat="server" CssClass="grid-view show_grid_exp" class="panel-body">

                    <asp:GridView ID="gv_expeness" runat="server" CssClass="table table-striped table-bordered table-responsive"
                        AutoGenerateColumns="False"
                        BackColor="White" BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px"
                        CellPadding="3" OnRowDataBound="gv_expeness_RowDataBound" OnPreRender="gv_expeness_PreRender"
                        AllowPaging="True" ShowFooter="false">

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
                            <asp:BoundField DataField="expenses_id" HeaderText="expenses_id"
                                SortExpression="expenses_id" />
                            <asp:BoundField DataField="travel_mode" HeaderText="Mode"
                                SortExpression="travel_mode" />
                            <asp:BoundField DataField="from_designation" HeaderText="From"
                                SortExpression="from_designation" />
                            <asp:BoundField DataField="to_designation" HeaderText="To"
                                SortExpression="to_designation" />
                            <asp:BoundField DataField="from_date" HeaderText="From Date"
                                SortExpression="from_date" />
                            <asp:BoundField DataField="to_date" HeaderText="To Date"
                                SortExpression="to_date" />
                             <asp:BoundField DataField="Comments" HeaderText="Reject Reason of Travel"
                                SortExpression="Comments" />
                            <%--<asp:BoundField DataField="Add_Description" HeaderText="Description"
                                        SortExpression="Add_Description" />--%>
                            <asp:BoundField DataField="expense_status" HeaderText="Travel Status"
                                SortExpression="expense_status" />
                            <asp:BoundField DataField="claim_amount" HeaderText="Claim Amt"
                                SortExpression="claim_amount" />
                            <asp:BoundField DataField="app_amount" HeaderText="Approved Amt"
                                SortExpression="app_amount" />
                            <asp:BoundField DataField="Claim_Status" HeaderText="Claim Status"
                                SortExpression="Claim_Status" />
                            <asp:BoundField DataField="Comments1" HeaderText="Reject Reason of Claim"
                                SortExpression="Comments1" />
                            <%--<asp:BoundField DataField="modified_by" HeaderText="Last Modified"
                                        SortExpression="modified_by" />--%>
                            <asp:BoundField DataField="NowDate" HeaderText="Claim Date"
                                SortExpression="NowDate" />
                            <asp:TemplateField HeaderText="Travelling Expense">
                                <ItemTemplate>
                                    <asp:LinkButton ID="lnkView" runat="server" OnClick="lnkView_Click">Edit Travel Plan</asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Delete">
                                <ItemTemplate>
                                    <asp:LinkButton runat="server" ID="lnkdelete" OnClick="lnkdelete_Click" OnClientClick="return confirm('Are you sure you want to delete this Travel Plan?');">Delete</asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Claim Expense">
                                <ItemTemplate>
                                    <asp:LinkButton runat="server" ID="lnkclaimexpense" CssClass="claim" OnClick="lnkclaimexpense_Click">Edit Claim Expense</asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
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
                <br />
                <div class="row text-center">
                    <asp:Button ID="btnclose" runat="server" class="btn btn-danger" Text="Close" OnClick="btn_close_Click" />
                </div>
            </div>
              
        </asp:Panel>

    </div>


</asp:Content>

