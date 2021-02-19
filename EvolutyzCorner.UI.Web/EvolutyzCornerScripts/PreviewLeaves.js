
var TimesheetID = '', fulldate = '';
var resultDataArray; var objUserSessionId = '';

$(document).ready(function () {

    $("#PreviewLeaves").addClass("active");
    $("#PreviewLeaves").siblings().removeClass("active");


    $("#from_date").datepicker({
        format: 'yyyy/mm/dd'
    });
    $("#to_date").datepicker({
        format: 'yyyy/mm/dd'
    });
    $("#cancelLeavecmnts").click(function () {
        window.location.reload();
    });


    $("#tabCon").empty();
    $("#body_ClientDetails").attr("style", "display: none;");
    $("#divEditTimesheetData").attr("style", "display: none;");
    $('#loading-image').attr("style", "display: block;");
    $.ajax({
        url: "/LeaveApplicationManagement/GetLeavePreview",
        type: "GET",
        contentType: 'application/json; charset=utf-8',
        dataType: 'JSON',
        success: function (resultData) {


            if (resultData.Roleid) {
                //objUserSessionId = resultData.RoleName;
                objUserSessionId = resultData.Roleid;

                if ((objUserSessionId === '1001') || (objUserSessionId === '1002') || (objUserSessionId === '1007')) {


                    if ((objUserSessionId === '1007')) {
                        $('#ManagerGridData').removeClass('actions');
                        $("#UserGridData").attr("style", "display: table;");
                        $("#ManagerGridData").attr("style", "display: table;");

                        if (resultData.myleaves) {
                            var objUsertimesheets = resultData.myleaves;
                            $('#UserGridData').DataTable({
                                'data': objUsertimesheets,
                                'paginate': true,
                                'sort': true,
                                'Processing': true,
                                'columns': [
                                    { 'data': 'Usrl_UserId', 'visible': false },
                                    { 'data': 'Usrl_LeaveId', 'visible': false },
                                    { 'data': 'accntmail', 'visible': false },
                                    { 'data': 'empmailid', 'visible': false },
                                    { 'data': 'ProjectName' },
                                    { 'data': 'userName' },

                                    {
                                        'data': 'LeaveStartDate'

                                    },
                                    {
                                        'data': 'LeaveEndDate'

                                    },
                                    { 'data': 'No_Of_Days' },
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

                        if (resultData.leavesforapproval) {
                            var objManagertimesheets = resultData.leavesforapproval;

                            $('#ManagerGridData').DataTable({
                                'data': objManagertimesheets,
                                'paginate': true,
                                'sort': true,
                                'Processing': true,
                                'columns': [
                                    { 'data': 'Usrl_UserId', 'visible': false },
                                    { 'data': 'Usrl_LeaveId', 'visible': false },
                                    { 'data': 'accntmail', 'visible': false },
                                    { 'data': 'ProjectName' },
                                    { 'data': 'userName' },
                                    { 'data': 'LeaveStartDate' },
                                    { 'data': 'LeaveEndDate' },
                                    { 'data': 'No_Of_Days' },
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


                                            }
                                            else {

                                                return '<a class="btn btn-icn"   id="AcceptLeave" data-accountmail="' + full.accntmail + '" data-username="' + full.userName + '" data-mngrid="' + full.ManagerID1 + '" data-mngrname= "' + full.ManagerName1 + '" data-mngrmail="' + full.ManagerEmail1 + '"  data-UserId="' + full.Usrl_UserId + '" data-status="4"  data-LeaveStartDate="' + full.LeaveStartDate + '" data-LeaveEndDate="' + full.LeaveEndDate + '"  data-LeaveId="' + full.Usrl_LeaveId + '" data-usermail="' + full.UserEmail + '"   onclick="userleaveconsumed(this)" ><i id="Rejected" class="fa fa-check" title="Approve"></i> </a><a class="btn btn-icn"   id="RejectLeave"   data-accountmail="' + full.accntmail + '" data-username="' + full.userName + '" data-mngrid="' + full.ManagerID1 + '" data-mngrname= "' + full.ManagerName1 + '" data-mngrmail="' + full.ManagerEmail1 + '"  data-UserId="' + full.Usrl_UserId + '" data-status="5"  data-LeaveStartDate="' + full.LeaveStartDate + '" data-LeaveEndDate="' + full.LeaveEndDate + '"  data-LeaveId="' + full.Usrl_LeaveId + '" data-usermail="' + full.UserEmail + '"   onclick="userleaveconsumed(this)" ><i id="RejectId" title="Reject" class="fa fa-times" ></i></a>';
                                            }
                                        },
                                    },

                                ]
                            });
                        }

                    }
                    //admin(<a class="btn btn-icn"   id="OnHoldLeave"   data-accountmail="' + full.accntmail + '" data-username="' + full.userName + '" data-mngrid="' + full.ManagerID1 + '" data-mngrname= "' + full.ManagerName1 + '" data-mngrmail="' + full.ManagerEmail1 + '"  data-UserId="' + full.Usrl_UserId + '" data-status="6"  data-LeaveStartDate="' + full.LeaveStartDate + '" data-LeaveEndDate="' + full.LeaveEndDate + '"  data-LeaveId="' + full.Usrl_LeaveId + '" data-usermail="' + full.UserEmail + '"   onclick="userleaveconsumed(this)" ><i id="RejectId" title="OnHold" class="fa fa-pause" ></i>)
                    else {
                        $("#ManagerGridData").addClass("actions");
                        $("#ManagerGridData").attr("style", "display: table;");
                        $("#UserGridData").attr("style", "display: none;");
                        $("#UserGridPanel").attr("style", "display: none;");
                        if (resultData.leavesforapproval) {
                            var objAdminLeaves = resultData.leavesforapproval;
                            $('#ManagerGridData').DataTable({
                                'data': objAdminLeaves,
                                'paginate': true,
                                'sort': true,
                                'Processing': true,
                                'columns': [
                                    { 'data': 'Usrl_UserId', 'visible': false },
                                    { 'data': 'Usrl_LeaveId', 'visible': false },
                                    { 'data': 'accntmail', 'visible': false },
                                    { 'data': 'ProjectName' },
                                    { 'data': 'userName' },
                                    { 'data': 'LeaveStartDate' },
                                    { 'data': 'LeaveEndDate' },
                                    { 'data': 'No_Of_Days' },
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
                                    },

                                ]
                            });
                        }


                    }



                }



                else {
                    $("#ManagerGridData").attr("style", "display: none;");
                    $("#UserGridData").attr("style", "display: table;");
                    $("#ManagerGridPanel").attr("style", "display: none;");
                    if (resultData.myleaves) {
                        var objUsertimesheet = resultData.myleaves;
                        $('#UserGridData').DataTable({
                            'data': objUsertimesheet,
                            'paginate': true,
                            'sort': true,
                            'Processing': true,
                            'columns': [
                                { 'data': 'Usrl_UserId', 'visible': false },
                                { 'data': 'Usrl_LeaveId', 'visible': false },
                                { 'data': 'accntmail', 'visible': false },
                                { 'data': 'empmailid', 'visible': false },
                                { 'data': 'ProjectName' },
                                { 'data': 'userName' },
                                {
                                    'data': 'LeaveStartDate'


                                },
                                {
                                    'data': 'LeaveEndDate'

                                },
                                { 'data': 'No_Of_Days' },
                                {
                                    'data': "LeaveApprovalStatus",
                                    'data': function (data) {
                                        return '<span class="badge badge-radius" data-toggle="tooltip" title="' + data.Leavestatus + '"></span>';
                                    }
                                },
                                {
                                    "render": function (Usrl_LeaveId, type, full, meta) {
                                        if (full.Leavestatus === 'Rejected') {
                                            return '<a class="btn btn-icn"   id="AcceptLeave" data-target = "#ReApplyleave" data-toggle = "modal" data-EmpEmailId = "' + full.empmailid + '" data-LeaveEndDate="' + full.LeaveEndDate + '"  data-LeaveStartDate="' + full.LeaveStartDate + '"   data-projname="' + full.ProjectName + '" data-LeaveId="' + full.Usrl_LeaveId + '" data-accountmail="' + full.accntmail + '" data-username="' + full.userName + '"   data-UserId="' + full.Usrl_UserId + '"   onclick="reapplyLeave(this)" ><i id="Rejected" class="fa fa-repeat" title="Reapply Leave"></i> </a>';

                                        }
                                        else {

                                            return null;
                                        }
                                    }
                                }
                                //{
                                //    "render": function (Usrl_LeaveId, type, full, meta) {
                                //        if (full.Leavestatus === 'On Hold') {
                                //            return '<a class="btn btn-icn"   id="AcceptLeave" data-target = "#ReApplyleave" data-toggle = "modal" data-EmpEmailId = "' + full.empmailid + '" data-LeaveEndDate="' + full.LeaveEndDate + '"  data-LeaveStartDate="' + full.LeaveStartDate + '"   data-projname="' + full.ProjectName + '" data-LeaveId="' + full.Usrl_LeaveId + '" data-accountmail="' + full.accntmail + '" data-username="' + full.userName + '"   data-UserId="' + full.Usrl_UserId + '"   onclick="reapplyLeave(this)" ><i id="Rejected" class="fa fa-repeat" title="Reapply Leave"></i> </a>';

                                //        }
                                //        else {

                                //            return null;
                                //        }
                                //    }
                                //}

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
    var LeaveStartDate = id.getAttribute("data-LeaveStartDate");
    var LeaveEndDate = id.getAttribute("data-LeaveEndDate");
    var LeaveId = id.getAttribute("data-LeaveId");
    var accountmail = id.getAttribute("data-accountmail");
    var Leavestatusid = id.getAttribute("data-status");
    var mgrid = id.getAttribute("data-mngrid");
    var mgrname = id.getAttribute("data-mngrname");
    var mgrmail = id.getAttribute("data-mngrmail");
    var usrmail = id.getAttribute("data-usermail");

    $.ajax({

        type: "POST",
        url: "/LeaveApplicationManagement/WebLeaveApproval",
        data: {
            "Userid": Userid,
            "LeaveStartDate": LeaveStartDate,
            "LeaveEndDate": LeaveEndDate,
            "leaveid": LeaveId,
            "accntmail": accountmail,
            "Leavestatus": Leavestatusid,
            "ManagerId": mgrid,
            "ManagerName": mgrname,
            "ManagerMail": mgrmail,
            "UserMail": usrmail
        },
        success: function (data) {

            alert(data);
            window.location = '/LeaveApplicationManagement/PreviewLeaves';

        }
    });


}

function reapplyLeave(id) {
    var userid = id.getAttribute("data-UserId");
    var leaveid = id.getAttribute("data-LeaveId");
    var accmailid = id.getAttribute("data-accountmail");
    var username = id.getAttribute("data-username");
    var projname = id.getAttribute("data-projname");
    var Leavestartdate = id.getAttribute("data-LeaveStartDate");
    var Leaveenddate = id.getAttribute("data-LeaveEndDate");
    var usersemailid = id.getAttribute("data-EmpEmailId");
    $("#Emp_Names").val(username);
    $("#from_date").val(Leavestartdate);
    $("#to_date").val(Leaveenddate);
    $("#from_address").val(usersemailid);
    $("#getEmpIds").val(userid);
    $("#accntmailid").val(accmailid);
    $("#empleaveid").val(leaveid);



    //$.ajax({

    //    type: "POST",
    //    url: "/LeaveApplicationManagement/saveleavecountForUS",
    //    data: {
    //        "Usrl_UserId": userid,
    //        "LeaveStartDate": Leavestartdate,
    //        "LeaveEndDate": Leaveenddate,
    //        "leaveid": leaveid,
    //        "accntmail": accmailid,
    //        "Leavestatus": Leavestatusid,
    //        "Username": username,
    //        "Projectname": projname
    //    },
    //    success: function (data) {

    //        alert(data);
    //        window.location = '/LeaveApplicationManagement/PreviewLeaves';

    //    }
    //});
}



