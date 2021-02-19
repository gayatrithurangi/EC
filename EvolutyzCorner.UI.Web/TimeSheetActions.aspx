<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TimeSheetActions.aspx.cs" Inherits="EvolutyzCorner.UI.Web.Models.TimeSheetActions" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

    <style type="text/css">
        table.statusfinder > tbody > tr:first-child {
            text-align: center;
        }

        table.statusfinder {
            background-color: #eee;
            border: 1px dashed #9e9e9e;
        }
            /*#lblEmailstatus {
            color: #00bd00;
        }
        #lblEmailstatus.reject {
            color: #f44336;
        }*/
            table.statusfinder > tbody > tr:first-child > td {
                padding: 15px 15px 0;
            }

            table.statusfinder > tbody > tr:last-child #divEmailid > section > div {
                border: 0 !important;
            }

                table.statusfinder > tbody > tr:last-child #divEmailid > section > div > div:first-child {
                    background: #7d7d7d !important;
                    color: #eee !important;
                    text-shadow: 1px 1px 1px #555 !important;
                }
                
.content-style {
    max-width:50%;
    margin: auto;
    background-color: #fff;
    border: 1px solid #ffe6db;
    border-radius: 4px;
    -webkit-box-shadow: 0 1px 1px rgba(0,0,0,.05);
    box-shadow: 0 1px 1px rgba(0,0,0,.05);
}
.content-style > .content-header {
    color: #903600;
    background-color: #e4b49e;
    padding: 10px 15px;
    border-bottom: 1px solid #d48059;
    border-top-left-radius: 3px;
    border-top-right-radius: 3px;
}
.content-style > .content-body {
    padding: 15px;
}
.content-style > .content-footer {
    padding: 10px 15px;
    background-color: #fff8f5;
    border-top: 1px solid #ddd;
    border-bottom-right-radius: 3px;
    border-bottom-left-radius: 3px;
    text-align:right;
}
        .form-control {
    display: block;
    width: calc(100% - 2em);
    height: auto;
    padding: 6px 12px;
    font-size: 14px;
    line-height: 1.42857143;
    color: #555;
    /*resize:none;*/
    background-color: #fff;
    background-image: none;
    border: 1px solid #ccc;
    border-radius: 4px;
    -webkit-box-shadow: inset 0 1px 1px rgba(0,0,0,.075);
    box-shadow: inset 0 1px 1px rgba(0,0,0,.075);
    -webkit-transition: border-color ease-in-out .15s,box-shadow ease-in-out .15s;
    -o-transition: border-color ease-in-out .15s,box-shadow ease-in-out .15s;
    -webkit-transition: border-color ease-in-out .15s,-webkit-box-shadow ease-in-out .15s;
    transition: border-color ease-in-out .15s,-webkit-box-shadow ease-in-out .15s;
    transition: border-color ease-in-out .15s,box-shadow ease-in-out .15s;
    transition: border-color ease-in-out .15s,box-shadow ease-in-out .15s,-webkit-box-shadow ease-in-out .15s;
}
        .form-control:focus{border-color:#66afe9;outline:0;-webkit-box-shadow:inset 0 1px 1px rgba(0,0,0,.075),0 0 8px rgba(102,175,233,.6);box-shadow:inset 0 1px 1px rgba(0,0,0,.075),0 0 8px rgba(102,175,233,.6)}
        .form-control::-moz-placeholder{color:#999;opacity:1}
        .form-control:-ms-input-placeholder{color:#999}
        .form-control::-webkit-input-placeholder{color:#999}
        .form-control::-ms-expand{background-color:transparent;border:0}
.btn-style {
    display: inline-block;
    margin-bottom: 0;
    font-weight: 400;
    text-align: center;
    white-space: nowrap;
    vertical-align: middle;
    -ms-touch-action: manipulation;
    touch-action: manipulation;
    -webkit-appearance: button;
    cursor: pointer;
    background-image: none;
    border: 1px solid transparent;    
    background: linear-gradient(135deg,#f37a41,#f8c26c);
    color: #f7f7f7;
    border-color: #fdae81;
    padding: 6px 12px;
    font-size: 14px;
    line-height: 1.42857143;
    border-radius: 20px;
    outline:none;
    -webkit-user-select: none;
    -moz-user-select: none;
    -ms-user-select: none;
    user-select: none;
    -webkit-transition: all ease-in-out .3s;
    -o-transition: all ease-in-out .3s;
    -webkit-transition: all ease-in-out .3s;
    transition: all ease-in-out .3s;
}
.btn-style:active, 
.btn-style:focus, 
.btn-style:hover {
    color: #fff;
    background: linear-gradient(135deg,#f8c26c,#f37a41);
    border-color: #fb9961;
    text-decoration: none;
}
    </style>
</head>
<body runat="server">
    <form id="form1" runat="server">
        <div id="emailstatus" runat="server">
            <table class="statusfinder" border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td><b>Email Status : </b>
                        <b>
                            <asp:Label ID="lblEmailstatus" runat="server" Text="" /></b>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <div id="divEmailid" runat="server">
                        </div>
                    </td>

                </tr>

            </table>

        </div>
        <div id="Apply_Managercomments" role="dialog" runat="server">   
                <div class="content-style" id="hidecomments">
                    <div class="content-header">Add Comments</div>
                    <div class="content-body">
                        <textarea rows="5" id="txt_comments" name="comments" runat="server" class="form-control"  ></textarea>
                    </div>
                    <div class="content-footer" id="questions">
                        <%-- <button type="button" class="btn btn-clr" id="Managercomments" runat="server" onclick="savecomments">Ok</button>--%>
                        <asp:Button ID="Managercomments" runat="server" OnClick="Managercomments_Click" CssClass="btn-style" Text="Send" />
                    </div>
                </div>
        </div>
    </form>
</body>
</html>
