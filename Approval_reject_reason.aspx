<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Approval_reject_reason.aspx.cs" Inherits="Approval_reject_reason"
    EnableEventValidation="false" %>

<html>
<head>
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

    <script type="text/javascript">
        function Req_validation() {

            var t_reason = document.getElementById('<%=txt_comments.ClientID %>');
             if (t_reason.value == "") {
                 alert("Please Enter Comment for Rejection !!");
                 t_reason.focus();
                 return false;
             }
             return true;

         }

    </script>

    <script type="text/javascript">
        function approve() {
            alert("Add Travel Plan Approved");
        }

        function Rejected() {
            alert("Add Travel Plan Rejected");
        }
    </script>

    <style>
        .grid-view {
            height: auto;
            max-height: 250px;
            overflow: scroll;
        }

        .table {
            width: 100%;
        }

        .row {
            margin: 0px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>


            <br />
            <div class="row">
                <div class="col-sm-12 col-xs-12">
                    <asp:Panel ID="newpanel" runat="server" CssClass="grid-view text-center" ScrollBars="Auto">
                        <asp:GridView ID="SearchGridView" runat="server" class="table" AutoGenerateColumns="False"
                            CellPadding="3" ForeColor="#333333" Font-Size="X-Small"
                            ShowFooter="True" BorderStyle="None"
                            OnRowDataBound="SearchGridView_RowDataBound">
                            <RowStyle ForeColor="#000066" />
                            <Columns>

                                <asp:BoundField DataField="Id" HeaderText="No." SortExpression="Id" />
                                <asp:BoundField DataField="comp_code" HeaderText="COMP CODE" SortExpression="comp code" />
                                <asp:BoundField DataField="COMPANY_NAME" HeaderText="COMPANY_NAME" SortExpression="COMPANY NAME" />
                                <asp:BoundField DataField="CITY" HeaderText="CITY" SortExpression="CITY" />
                                <asp:BoundField DataField="STATE" HeaderText="STATE" SortExpression="STATE" />

                            </Columns>
                            <FooterStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Right" />
                            <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" />
                            <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" />
                            <HeaderStyle BackColor="#006699" Font-Bold="True" ForeColor="White" HorizontalAlign="Left" />
                        </asp:GridView>
                    </asp:Panel>
                    <br />
                    <div class="col-sm-2 col-xs-12">
                        <asp:Label ID="commenrid" Text="Comments :" runat="server"></asp:Label>
                        <%--Comments:--%>
                    </div>
                    <div class="col-sm-4 col-xs-12">
                        <asp:TextBox ID="txt_comments" runat="server" class="form-control" TextMode="MultiLine"></asp:TextBox>
                    </div>
                    <div class="text-center">
                        <asp:Button ID="btn_reject" runat="server" OnClientClick="return Req_validation();" class="btn btn-danger" OnClick="btn_reject_Click" Text=" Reject " TabIndex="2" />
                    </div>
                </div>

            </div>


        </div>
    </form>
</body>
</html>
