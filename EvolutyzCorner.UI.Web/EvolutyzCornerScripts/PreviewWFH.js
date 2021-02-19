

var TimesheetID = '', fulldate = '';
var resultDataArray; var objUserSessionId = '';

$(document).ready(function () {
    $("#tabCon").empty();

    $("#PreviewWorkFromHome").addClass("active");
    $("#PreviewWorkFromHome").siblings().removeClass("active");
    $("#fromdate").datepicker({
        format: 'yyyy/mm/dd'
    });
    $("#todate").datepicker({
        format: 'yyyy/mm/dd'
    });
    $("#cancelwfhcmnts").click(function () {
        window.location.reload();
    });

    $("#body_ClientDetails").attr("style", "display: none;");
    $("#divEditTimesheetData").attr("style", "display: none;");
    $('#loading-image').attr("style", "display: block;");
    $.ajax({
        url: "/LeaveApplicationManagement/GetWrkfrmHomePreview",
        type: "GET",
        contentType: 'application/json; charset=utf-8',
        dataType: 'JSON',
        // cache: false,
        success: function (resultData) {

            if (resultData.Roleid) {

                objUserSessionId = resultData.Roleid;
                if ((objUserSessionId === '1001') || (objUserSessionId === '1002') || (objUserSessionId === '1007')) {

                    if ((objUserSessionId === '1007')) {
                        $("#UserGridData").attr("style", "display: table;");
                        $("#ManagerGridData").attr("style", "display: table;");
                        $('#ManagerGridData').removeClass('actions');
                        if (resultData.UserWrkfrmhome) {
                            var objUsertimesheets = resultData.UserWrkfrmhome;
                            $('#UserGridData').DataTable({
                                // 'destroy': true,
                                'data': objUsertimesheets,
                                'paginate': true,
                                'sort': true,
                                'Processing': true,
                                'columns': [
                                    { 'data': 'Usrl_UserId', 'visible': false },
                                    { 'data': 'UserwfhID', 'visible': false },
                                    { 'data': 'accntmail', 'visible': false },
                                    { 'data': 'wfhempmailid', 'visible': false},
                                    { 'data': 'ProjectName' },
                                    { 'data': 'userName' },
                                    { 'data': 'LeaveStartDate' },
                                    { 'data': 'LeaveEndDate' },
                                    { 'data': 'Tot_No_Days' },
                                    {
                                        'data': "LeaveApprovalStatus",
                                        "data": function (data) {
                                            return '<span class="badge badge-radius" data-toggle="tooltip" title="' + data.Leavestatus + '"></span>';
                                        }
                                    },
                                    {
                                        "render": function (Usrl_LeaveId, type, full, meta) {
                                            return '';
                                        },
                                    },
                                ]
                            });




                        }

                        if (resultData.wrkfrmhmeforapproval) {
                            var objManagertimesheets = resultData.wrkfrmhmeforapproval;
                            $('#ManagerGridData').DataTable({
                                //   'destroy': true,
                                'data': objManagertimesheets,
                                'paginate': true,
                                'sort': true,
                                'Processing': true,
                                'columns': [
                                    { 'data': 'Usrl_UserId', 'visible': false },
                                    { 'data': 'UserwfhID', 'visible': false },
                                    { 'data': 'accntmail', 'visible': false },
                                    { 'data': 'ProjectName' },
                                    { 'data': 'userName' },
                                    { 'data': 'LeaveStartDate' },
                                    { 'data': 'LeaveEndDate' },
                                    { 'data': 'Tot_No_Days' },
                                    { 'data': 'ManagerID1', 'visible': false },
                                    { 'data': 'ManagerName1', 'visible': false },
                                    { 'data': 'ManagerEmail1', 'visible': false },
                                    { 'data': 'UserEmail', 'visible': false },
                                    {
                                        'data': "LeaveApprovalStatus",
                                        "data": function (data) {
                                            return '<span class="badge badge-radius" data-toggle="tooltip" title="' + data.Leavestatus + '"></span>';
                                        }
                                    },
                                    {
                                        "render": function (Usrl_LeaveId, type, full, meta) {
                                            if ((full.Leavestatus === 'Approved') || (full.Leavestatus === 'Rejected') || (full.Leavestatus === 'On Hold')) {
                                                return null;

                                                //return '<i  class="fa fa-edit" title="Edit"></i>';
                                            }
                                            else {
                                                //(<a class="btn btn-icn"   id="OnHoldLeave"   data-accountmail="' + full.accntmail + '" data-username="' + full.userName + '" data-mngrid="' + full.ManagerID1 + '" data-mngrname= "' + full.ManagerName1 + '" data-mngrmail="' + full.ManagerEmail1 + '"  data-UserId="' + full.Usrl_UserId + '" data-status="6"  data-LeaveStartDate="' + full.LeaveStartDate + '" data-LeaveEndDate="' + full.LeaveEndDate + '"  data-Uwfhid="' + full.UserwfhID + '" data-usermail="' + full.UserEmail + '"  onclick="userleaveconsumed(this)" ><i id="Onhold" title="On Hold" class="fa fa-pause" ></i>)return '<i id="ApproveID" class="fa fa-check" title="Approve"></i> <i id="RejectId" title="Reject" class="fa fa-times" ></i> '
                                                return '<a class="btn btn-icn"   id="AcceptLeave" data-accountmail="' + full.accntmail + '" data-username="' + full.userName + '" data-mngrid="' + full.ManagerID1 + '" data-mngrname= "' + full.ManagerName1 + '" data-mngrmail="' + full.ManagerEmail1 + '"  data-UserId="' + full.Usrl_UserId + '" data-status="4"  data-LeaveStartDate="' + full.LeaveStartDate + '" data-LeaveEndDate="' + full.LeaveEndDate + '"  data-Uwfhid="' + full.UserwfhID + '" data-usermail="' + full.UserEmail + '"   onclick="userleaveconsumed(this)" ><i id="Rejected" class="fa fa-check" title="Approve"></i> </a><a class="btn btn-icn"   id="RejectLeave"   data-accountmail="' + full.accntmail + '" data-username="' + full.userName + '" data-mngrid="' + full.ManagerID1 + '" data-mngrname= "' + full.ManagerName1 + '" data-mngrmail="' + full.ManagerEmail1 + '"  data-UserId="' + full.Usrl_UserId + '" data-status="5"  data-LeaveStartDate="' + full.LeaveStartDate + '" data-LeaveEndDate="' + full.LeaveEndDate + '"  data-Uwfhid="' + full.UserwfhID + '" data-usermail="' + full.UserEmail + '"  onclick="userleaveconsumed(this)" ><i id="RejectId" title="Reject" class="fa fa-times" ></i></a>';
                                            }
                                        }
                                    }

                                ]
                            });
                        }
                    }
                    //Admin preview
                    else {
                        $("#ManagerGridData").attr("style", "display: table;");
                        $("#UserGridData").attr("style", "display: none;");
                        $("#UserGridPanel").attr("style", "display: none;");
                        $("#ManagerGridData").addClass("actions");

                        if (resultData.wrkfrmhmeforapproval) {
                            var objAdminwfh = resultData.wrkfrmhmeforapproval;
                            $('#ManagerGridData').DataTable({
                                //   'destroy': true,
                                'data': objAdminwfh,
                                'paginate': true,
                                'sort': true,
                                'Processing': true,
                                'columns': [
                                    { 'data': 'Usrl_UserId', 'visible': false },
                                    { 'data': 'UserwfhID', 'visible': false },
                                    { 'data': 'accntmail', 'visible': false },
                                    { 'data': 'ProjectName' },
                                    { 'data': 'userName' },
                                    { 'data': 'LeaveStartDate' },
                                    { 'data': 'LeaveEndDate' },
                                    { 'data': 'Tot_No_Days' },
                                    { 'data': 'ManagerID1', 'visible': false },
                                    { 'data': 'ManagerName1', 'visible': false },
                                    { 'data': 'ManagerEmail1', 'visible': false },
                                    { 'data': 'UserEmail', 'visible': false },
                                    {
                                        'data': "LeaveApprovalStatus",
                                        "data": function (data) {
                                            return '<span class="badge badge-radius" data-toggle="tooltip" title="' + data.Leavestatus + '"></span>';
                                        }
                                    },
                                    {
                                        "render": function (Usrl_LeaveId, type, full, meta) {
                                            return '';
                                        },
                                    }
                                    //{
                                    //    "render": function (Usrl_LeaveId, type, full, meta) {
                                    //        if ((full.Leavestatus === 'Approved') || (full.Leavestatus === 'Rejected') || (full.Leavestatus === 'On Hold')) {
                                    //            return null;

                                    //            //return '<i  class="fa fa-edit" title="Edit"></i>';
                                    //        }
                                    //        else {
                                    //            //return '<i id="ApproveID" class="fa fa-check" title="Approve"></i> <i id="RejectId" title="Reject" class="fa fa-times" ></i> '
                                    //            // return '<a class="btn btn-icn"   id="AcceptLeave" data-accountmail="' + full.accntmail + '" data-username="' + full.userName + '" data-mngrid="' + full.ManagerID1 + '" data-mngrname= "' + full.ManagerName1 + '" data-mngrmail="' + full.ManagerEmail1 + '"  data-UserId="' + full.Usrl_UserId + '" data-status="4"  data-LeaveStartDate="' + full.LeaveStartDate + '" data-LeaveEndDate="' + full.LeaveEndDate + '"  data-Uwfhid="' + full.UserwfhID + '" data-usermail="' + full.UserEmail + '"   onclick="userleaveconsumed(this)" ><i id="Rejected" class="fa fa-check" title="Approve"></i> </a><a class="btn btn-icn"   id="RejectLeave"   data-accountmail="' + full.accntmail + '" data-username="' + full.userName + '" data-mngrid="' + full.ManagerID1 + '" data-mngrname= "' + full.ManagerName1 + '" data-mngrmail="' + full.ManagerEmail1 + '"  data-UserId="' + full.Usrl_UserId + '" data-status="5"  data-LeaveStartDate="' + full.LeaveStartDate + '" data-LeaveEndDate="' + full.LeaveEndDate + '"  data-Uwfhid="' + full.UserwfhID + '" data-usermail="' + full.UserEmail + '"  onclick="userleaveconsumed(this)" ><i id="RejectId" title="Reject" class="fa fa-times" ></i><a class="btn btn-icn"   id="OnHoldLeave"   data-accountmail="' + full.accntmail + '" data-username="' + full.userName + '" data-mngrid="' + full.ManagerID1 + '" data-mngrname= "' + full.ManagerName1 + '" data-mngrmail="' + full.ManagerEmail1 + '"  data-UserId="' + full.Usrl_UserId + '" data-status="6"  data-LeaveStartDate="' + full.LeaveStartDate + '" data-LeaveEndDate="' + full.LeaveEndDate + '"  data-Uwfhid="' + full.UserwfhID + '" data-usermail="' + full.UserEmail + '"  onclick="userleaveconsumed(this)" ><i id="Onhold" title="On Hold" class="fa fa-pause" ></i>';
                                    //            return '';
                                    //        }
                                    //    }
                                    //}

                                ]
                            });
                        }
                    }

                }

                else {
                    $("#ManagerGridData").attr("style", "display: none;");
                    $("#UserGridData").attr("style", "display: table;");
                    $("#ManagerGridPanel").attr("style", "display: none;");
                    if (resultData.UserWrkfrmhome) {
                        var objUsertimesheet = resultData.UserWrkfrmhome;
                        $('#UserGridData').DataTable({
                            // 'destroy': true,
                            'data': objUsertimesheet,
                            'paginate': true,
                            'sort': true,
                            'Processing': true,
                            'columns': [
                                { 'data': 'Usrl_UserId', 'visible': false },
                                { 'data': 'UserwfhID', 'visible': false },
                                { 'data': 'accntmail', 'visible': false },
                                { 'data': 'wfhempmailid', 'visible': false },
                                { 'data': 'ProjectName' },
                                { 'data': 'userName' },
                                { 'data': 'LeaveStartDate' },
                                { 'data': 'LeaveEndDate' },
                                { 'data': 'Tot_No_Days' },
                                {
                                    'data': "LeaveApprovalStatus",
                                    "data": function (data) {
                                        return '<span class="badge badge-radius" data-toggle="tooltip" title="' + data.Leavestatus + '"></span>';
                                    }
                                },
                                {
                                    "render": function (Usrl_LeaveId, type, full, meta) {
                                        if (full.Leavestatus === 'Rejected') {
                                            return '<a class="btn btn-icn"   id="AcceptLeave" data-target = "#Reapply_Workfrmhome" data-toggle = "modal" data-Leaveenddate = "' + full.LeaveEndDate + '" data-Leavestartdate = "' + full.LeaveStartDate + '" data-username = "' + full.userName + '" data-projname = "' + full.ProjectName + '" data-wfhuseremail = "' + full.wfhempmailid + '" data-userid = "' + full.Usrl_UserId + '" data-wfhid = "' + full.UserwfhID + '" data-accountmail = "' + full.accntmail + '"  onclick="userwfhconsumed(this)" ><i id="Onhold" title="Reapply Work from home" class="fa fa-repeat" ></i>';
                                        }
                                        else {

                                            return null;
                                        }
                                    }
                                }

                            ]
                        });
                    }
                }

            }



        },
        complete: function () {
            $('#loading-image').attr("style", "display: none;");
        },

        error: function (data) {
            //  alert(data.responseText);
        }
    });
    function dateConversion(value) {
        if (value === null) return "";
        var pattern = /Date\(([^)]+)\)/;
        var results = pattern.exec(value);
        var dt = new Date(parseFloat(results[1]));

        return (dt.getMonth() + 1) + "/" + dt.getDate() + "/" + dt.getFullYear();

    }

});


function calculateSum() {

    var sum = 0;
    $(".uc1txtHours").each(function () {
        //add only if the value is number
        if (!isNaN(this.value) && this.value.length !== 0) {
            sum += parseFloat(this.value);
        }
    });
    //.toFixed() method will roundoff the final sum to 2 decimal places
    $("#TotalHoursCount").html(sum.toFixed(2));
}


function userleaveconsumed(id) {
    var Userid = id.getAttribute("data-UserId");
    var userwfhid = id.getAttribute("data-Uwfhid");
    var LeaveStartDate = id.getAttribute("data-LeaveStartDate");
    var LeaveEndDate = id.getAttribute("data-LeaveEndDate");
    var accountmail = id.getAttribute("data-accountmail");
    var Leavestatusid = id.getAttribute("data-status");
    var mgrid = id.getAttribute("data-mngrid");
    var mgrname = id.getAttribute("data-mngrname");
    var mgrmail = id.getAttribute("data-mngrmail");
    var usrmail = id.getAttribute("data-usermail");

    // Leavstatusid = Leavestatus;

    $.ajax({
        // contentType: "application/json",
        type: "POST",
        url: "/LeaveApplicationManagement/WebWrkFrmHmeApproval",
        data: {
            "Userid": Userid,
            "UWFHID": userwfhid,
            "LeaveStartDate": LeaveStartDate,
            "LeaveEndDate": LeaveEndDate,
            "accntmail": accountmail,
            "Leavestatus": Leavestatusid,
            "ManagerId": mgrid,
            "ManagerName": mgrname,
            "ManagerMail": mgrmail,
            "UserMail": usrmail
        },
        success: function (data) {

            alert(data);
            window.location = '/LeaveApplicationManagement/PreviewWorkFromHome';

        }
    });


}


function userwfhconsumed(id) {

    var userid = id.getAttribute("data-userid");
    var wrkfrmhmeid = id.getAttribute("data-wfhid");
    var username = id.getAttribute("data-username");
    var projctname = id.getAttribute("data-projname");
    var accontmail = id.getAttribute("data-accountmail");
    var wrkfrmusrmail = id.getAttribute("data-wfhuseremail");
    var leaveenddate = id.getAttribute("data-Leaveenddate");
    var leavestartadte = id.getAttribute("data-Leavestartdate");

    $("#Emp_Names").val(username);
    $("#fromdate").val(leavestartadte);
    $("#todate").val(leaveenddate);
    $("#accntmailid").val(accontmail);
    $("#empwfhid").val(wrkfrmhmeid);
    $("#from_address").val(wrkfrmusrmail);
    $("#getEmpIds").val(userid);
}



