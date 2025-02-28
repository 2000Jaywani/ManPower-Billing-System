<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="authorised_signatory.aspx.cs" Inherits="authorised_signatory" EnableEventValidation="false" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cph_title" runat="Server">
    <title>Authorised Signatory Approval</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cph_righrbody" runat="Server">
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

        $(document).ready(function () {
            $(".date-pickers").datepicker({
                changeMonth: true,
                changeYear: true,
                showButtonPanel: true,
                dateFormat: 'dd/mm/yy',
                yearRange: '1950',
                minDate: 0
                //onSelect: function (selected) {
                //    $(".date_join").datepicker("option", "minDate", selected)
                //}

            });


            $(".date-pickers").attr("readonly", "true");


            // prospect meet report

            var table = $('#<%=gv_cash_voucher.ClientID%>').DataTable({
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
            .appendTo('#<%=gv_cash_voucher.ClientID%>_wrapper .col-sm-6:eq(0)');
            $.fn.dataTable.ext.errMode = 'none';



            /////




        });




        function isnumber(e) {
            if (null != e) {
                isIE = document.all ? 1 : 0
                keyEntry = !isIE ? e.which : e.keyCode;
                if (((keyEntry >= '48') && (keyEntry <= '57')) || ((keyEntry == '39') && (keyEntry == '34')))

                    return true;
                else {
                    // alert('Please Enter Only Character values.');
                    return false;
                }
            }
        }
        function alphabet(e) {

            if (null != e) {
                isIE = document.all ? 1 : 0
                keyEntry = !isIE ? e.which : e.keyCode;
                if (((keyEntry >= '65') && (keyEntry <= '90')) || ((keyEntry >= '97') && (keyEntry <= '122')) || (keyEntry < '31') ||
                    (keyEntry == '32') || (keyEntry == '38') || ((keyEntry == '39') && (keyEntry == '34')))

                    return true;
                else {
                    // alert('Please Enter Only Character values.');
                    return false;
                }
            }
        }

        function alphanumeric(e) {

            if (null != e) {
                isIE = document.all ? 1 : 0
                keyEntry = !isIE ? e.which : e.keyCode;
                if (((keyEntry >= '65') && (keyEntry <= '90')) || ((keyEntry >= '97') && (keyEntry <= '122')) || ((keyEntry >= '48') && (keyEntry <= '57')) || ((keyEntry == '39') && (keyEntry == '34')) || (keyEntry < '31') ||
                    (keyEntry == '32') || (keyEntry == '38') || ((keyEntry == '39') && (keyEntry == '34')))

                    return true;
                else {
                    // alert('Please Enter Only Alphanumeric values.');
                    return false;
                }
            }
        }


        function cash_voucher() {

            $.blockUI({ overlayCSS: { backgroundColor: '#CCCCCC' } });
            return true;

        }

        </script>
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div class="container-fluid">
        <asp:Panel ID="Panel3" runat="server" CssClass="panel panel-primary" Style="background-color: white;">
            <div class="panel-heading">
                <div class="row">
                    <div class="col-sm-1"></div>
                    <div class="col-sm-9">
                        <div style="color: #fff; font-size: small;" class="text-center text-uppercase"><b>Authorised Signatory Approval</b></div>
                    </div>
                    <div class="col-sm-2 text-right">
                        <asp:LinkButton ID="LinkButton1" runat="server" OnClientClick="openWindow();return false;">
                            <asp:Image runat="server" ID="Image1" Width="20" Height="20" ToolTip="Help" ImageUrl="Images/help_ico.png" />
                        </asp:LinkButton>
                    </div>
                </div>
            </div>
            <br />
                <%--<div class="container-fluid" style="background: #f3f1fe; border-radius: 10px; border: 1px solid white">
                    <br />
                    <div class="row">
                        <div class="col-sm-2 col-xs-12" runat="server" id="bill">
                            <b>From Date :</b>
                            <asp:TextBox ID="txt_date1" runat="server" MaxLength="10" class="form-control date-pickers"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txt_date1"  ErrorMessage="Please select current Date" ForeColor="Red" Font-Size="10px" SetFocusOnError="True"></asp:RequiredFieldValidator>
                        </div>
                        <div class="col-sm-2 col-xs-12">
                            <b>To Date : <span class="text-red">*</span></b>
                            <asp:TextBox ID="txt_date2" runat="server" MaxLength="10" class="form-control date-pickers"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequirePreDate" runat="server" ControlToValidate="txt_date2"  ErrorMessage="Please select current Date" ForeColor="Red" Font-Size="10px" SetFocusOnError="True"></asp:RequiredFieldValidator>
                        </div>
                    </div>--%>
            <asp:Panel ID="Panel20" runat="server" Style="overflow-x: auto;" >
        <asp:GridView ID="gv_cash_voucher" class="table" runat="server" BackColor="White"
                        BorderColor="#CCCCCC" BorderStyle="None" BorderWidth="1px" CellPadding="1"
                         DataKeyNames="soft_copy_file" OnPreRender="gv_cash_voucher_PreRender"
                        OnRowDataBound="gv_cash_voucher_RowDataBound" 
                        AutoGenerateColumns="False" Width="100%" >
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
                            <%--<asp:TemplateField>
                                <%--<HeaderTemplate>
                                    <asp:CheckBox ID="chk_gv_header" runat="server" Text="SELECT "  />
                                </HeaderTemplate>--%>
                                <%--<ItemTemplate>
                                    <asp:CheckBox ID="chk_client" runat="server" CssClass="center-block" />
                                </ItemTemplate>
                            </asp:TemplateField>--%>
                            <asp:TemplateField HeaderText="Sr No.">
                                <ItemTemplate>
                                    <asp:Label ID="Label1" runat="server" Text='<%# Container.DataItemIndex+1 %>' Width="20px"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                            
                            <asp:BoundField DataField="id" HeaderText="Id" SortExpression="id" />
                            <asp:BoundField DataField="cash_rs" HeaderText="Amount" SortExpression="cash_rs" />
                            <asp:BoundField DataField="receiver_name" HeaderText="Receiver Name" SortExpression="receiver_name" />
                            <asp:BoundField DataField="debited_to" HeaderText="Debited  To" SortExpression="debited_to" />

                            <asp:BoundField DataField="request_date" HeaderText="Request Date" SortExpression="request_date"/>
                            <asp:BoundField DataField="narration" HeaderText="Narration" SortExpression="narration" />
                            <asp:BoundField DataField="status" HeaderText="Status" SortExpression="status" />
                             <asp:TemplateField HeaderText="Image">
                                            <ItemTemplate>
                                                <asp:Image ID="soft_copy_file" runat="server" Height="50" Width="50" />
                                                <br />
                                                <asp:LinkButton ID="soft_copy_file1" Text="Download" runat="server" OnClick="link_soft_copy_Click"  ForeColor="Red"></asp:LinkButton>
                                            </ItemTemplate>
                            </asp:TemplateField>
                           
                            <asp:TemplateField >
                                     <ItemTemplate>    
                                     <asp:LinkButton ID="btn_approve" Text="Approve" runat="server" CssClass="btn btn-primary" Style="color:white" CommandArgument='<%# Eval("id") %>'  OnCommand="btn_approve_Command" OnClientClick="return confirm('Are you sure you want to Approve this Record?');"></asp:LinkButton>
                                      </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField >
                                     <ItemTemplate>
                                     <asp:LinkButton ID="btn_reject" Text="Reject" runat="server" CssClass="btn btn-primary" Style="color:white" CommandArgument='<%# Eval("id") %>'   OnCommand="btn_reject_Command" OnClientClick="return confirm('Are you sure you want to Reject this Record?');"></asp:LinkButton>     
                                      </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="reason Of rejection">
                                        <ItemTemplate>
                                            <asp:TextBox runat="server" ID="rejected_reason" Text='<%# Eval("rejection_reason") %>' Width="180px" CssClass="form-control" onkeypress=" return alphanumeric(event)"></asp:TextBox>
                                        </ItemTemplate>
                            </asp:TemplateField>


                        </Columns>
                    </asp:GridView>
                <div class="row text-center">
                     <asp:Button ID="Button1" runat="server" class="btn btn-large" OnClick="btn_tally_report_Click"   Text="Report" OnClientClick="return Valid_date()" />

                 </div>
                </asp:Panel>
        </asp:Panel>
    </div>
    
</asp:Content>



