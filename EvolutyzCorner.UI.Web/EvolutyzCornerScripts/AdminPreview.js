
var resultDataArray;
var timesheetProjectid, MonthYear;
var dt = new Date();
var presentyear = dt.getFullYear();
var currMon = dt.getMonth();

$(document).ready(function (e) {
    $.ajax({
        type: "GET",
        url: "/Admin/getLoadProjects",
        datatype: "Json",
        //data: { id: id },
        success: function (data) {
            resultDataArray = data[0].Proj_ProjectID;

            $.each(data, function (index, value) {

                $('.Pro').append('<option value="' + value.Proj_ProjectID + '">' + value.Proj_ProjectName + '</option>');

            });
        },
    });

    var years = [presentyear - 5, presentyear - 4, presentyear - 3, presentyear - 2, presentyear - 1, presentyear];
    var option = '';
    for (i = 0; i < years.length; i++) {
        option = document.createElement("OPTION");
        option.text = years[i];
        option.value = years[i];
        // // if year is 2019 selected
        if (years[i] == presentyear) {
            option.selected = true;
        }
        $('#year').append(option);
    }

    LoadMonthAccToYear();

    $('#year').on('change', function () {
        LoadMonthAccToYear();

    });

});


function LoadMonthAccToYear() {
    var options = ["1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12"];
    var option = ["Jan", "Feb", "Mar", "Apr", "May", "June", "July", "Aug", "Sep", "Oct", "Nov", "Dec"];
    var currMon = (new Date).getMonth();
    var currYear = (new Date).getFullYear();
    var id = $('#year').val();
    $('#month').html('');

    if (currYear == id && currMon) {

        for (i = 0; i <= currMon; i++) {
            $('#month').append($("<option />").val(options[i]).text(option[i]));
            //option.text =i;
            //option.value = i;

            if (i == currMon) {
                $('#month').val(options[i]);
            }

        }


    }
    else {
        for (i = 0; i <= 11; i++) {
            $('#month').append($("<option />").val(options[i]).text(option[i]));
            if (i == currMon) {
                $('#month').val(options[i]);
            }
        }

    }

}




$(document).ready(function () {


    //LoadProjectsofAllusers();
});



function LoadProjectsofAllusers() {
    
    var timesheetProjectid = $("#AdminProjectsid").val();
    var year = $('#year').val();
    var month = $('#month').val();
    var adminmonth = '';
    var adminYear = '';
    if (month <= 9) {

        adminmonth = "0" + month;
    }
    else {

        adminmonth = month;
    }
    adminmonth = adminmonth;
    adminYear = year;
    $("#tabCon").empty();

    $.ajax({
        url: "/Timesheet/AdminPreviewaRevokeTimesheets",
        type: "GET",
        data: { TimesheetProjectid: timesheetProjectid, Year: adminYear, Month: adminmonth },
        contentType: 'application/json; charset=utf-8',
        dataType: 'JSON',
        // cache: false,
        success: function (resultData) {
            
            console.resultData;
            if (resultData.roleid) {
                objUserSessionId = resultData.roleid;

                if ((objUserSessionId == '1002')) {

                    if (resultData.timesheetsforapproval.length > 0) {

                        $("#ManagerGridPanel").attr("style", "display: block;");

                        var objManagertimesheets = resultData.timesheetsforapproval;
                        $('#ManagerGridData').DataTable({
                            'data': objManagertimesheets,
                            'bDestroy': true,
                            'paginate': true,
                            'sort': false,
                            'Processing': true,
                            'columns': [
                                { 'data': 'Usr_UserID', 'visible': false, },
                                { 'data': 'TimesheetMonth', 'visible': false, },
                                { 'data': 'TimesheetID', 'visible': false, },
                                { 'data': 'ProjectId', 'visible': false, },
                                { 'data': 'AdminEmailid', 'visible': false, },
                                { 'data': 'AdminName', 'visible': false, },
                                { 'data': 'ProjectName' },
                                { 'data': 'userName' },
                                { 'data': 'Month_Year' },
                                { 'data': 'CompanyBillingHours' },
                                { 'data': 'ResourceWorkingHours' },
                                {
                                    "data": "TimesheetApprovalStatus",
                                    "data": "ManagerApprovalStatus",
                                    "data": function (data) {
                                        return '<span class="badge badge-radius" data-toggle="tooltip" id="' + data.TimesheetApprovalStatus + '" title="' + data.TimesheetApprovalStatus + '"></span>'
                                    }
                                },
                                {
                                    "render": function (TimesheetID, type, full, meta) {


                                        if ((full.TimesheetApprovalStatus == "No Action") || (full.TimesheetApprovalStatus == "Saved Timesheet")
                                            || (full.TimesheetApprovalStatus == "Rejected") || (full.TimesheetApprovalStatus == "Rejected at Level_1 Manager")
                                            || (full.TimesheetApprovalStatus == "Rejected at Level_2 Manager") || (full.TimesheetApprovalStatus == "Revoked at Admin")) {
                                            // return '<a class="btn btn-icn" id="Revoketimesheet" data-ProjectName="' + full.ProjectName + '" data-userName="' + full.userName + '" data-CompanyBillingHours="' + full.CompanyBillingHours + '" data-ResourceWorkingHours="' + full.ResourceWorkingHours + '" data-MonthYear="' + full.Month_Year + '" data-TimesheetMonth="' + full.TimesheetMonth + '" data-TimesheetID="' + full.TimesheetID + '" data-Usr_UserID="' + full.Usr_UserID + '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus + '" data-ProjectId="' + full.ProjectId + '" data-Status="5" ></a>';
                                            return '<a class="btn btn-icn" id="Revoketimesheet" data-ProjectName="' + full.ProjectName + '" data-userName="' + full.userName + '" data-CompanyBillingHours="' + full.CompanyBillingHours + '" data-ResourceWorkingHours="' + full.ResourceWorkingHours + '" data-MonthYear="' + full.Month_Year + '" data-TimesheetMonth="' + full.TimesheetMonth + '" data-TimesheetID="' + full.TimesheetID + '" data-Usr_UserID="' + full.Usr_UserID + '" data-AdminEmailid="' + full.AdminEmailid + '"  data-AdminName="' + full.AdminName + '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus + '" data-ProjectId="' + full.ProjectId + '" data-TimesheetDuration="' + full.TimesheetDuration + '" data-Status="5" onclick="PreviewAdminRevokeTimesheet(this)" ><i class="fa fa-rotate-right"></i> </a>';


                                        }

                                        else {

                                            return '<a class="btn btn-icn" id="Revoketimesheet" data-ProjectName="' + full.ProjectName + '" data-userName="' + full.userName + '" data-CompanyBillingHours="' + full.CompanyBillingHours + '" data-ResourceWorkingHours="' + full.ResourceWorkingHours + '" data-MonthYear="' + full.Month_Year + '" data-TimesheetMonth="' + full.TimesheetMonth + '" data-TimesheetID="' + full.TimesheetID + '" data-Usr_UserID="' + full.Usr_UserID + '" data-AdminEmailid="' + full.AdminEmailid + '"  data-AdminName="' + full.AdminName + '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus + '" data-ProjectId="' + full.ProjectId + '" data-TimesheetDuration="' + full.TimesheetDuration + '" data-Status="5" onclick="PreviewAdminRevokeTimesheet(this)" ><i class="fa fa-rotate-right"></i> </a>';

                                        }

                                    },
                                },

                            ]
                        });
                    }

                    else {
                        alert("No records found");
                        $("#tabCon").empty();
                        $("#ManagerGridPanel").attr("style", "display: none;");
                        $('#ManagerGridData').destroy();
                        $('#ManagerGridData').DataTable({

                            'destroy': true,
                            'searching': false

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
    })
}




var TimesheetID = '', MonthYearname = '', tsuserid = '', ApporRejprojectid = '', Commentss = '', TimesheetMonth = '';
var AdminEmailid = '', AdminName = '', TimesheetDuration = '';

function PreviewAdminRevokeTimesheet(id) {
    
    TimesheetID = id.getAttribute("data-TimesheetID");
    MonthYearname = id.getAttribute("data-MonthYear");
    tsuserid = id.getAttribute("data-Usr_UserID");
    ApporRejprojectid = id.getAttribute("data-ProjectId");
    TimesheetMonth = id.getAttribute("data-TimesheetMonth");
    AdminEmailid = id.getAttribute("data-AdminEmailid");
    AdminName = id.getAttribute("data-AdminName");
    TimesheetDuration = id.getAttribute("data-TimesheetDuration");
    var projectname = id.getAttribute("data-ProjectName");
    var empname = id.getAttribute("data-userName");
    var Actualhrs = id.getAttribute("data-CompanyBillingHours");
    var Workedhrs = id.getAttribute("data-ResourceWorkingHours");
    var ApproveRejectstatus = id.getAttribute("data-Status");
    id = ApproveRejectstatus;

    if (id == 5) {
        $("#btnRej").show();
        $("#btnAdd").hide();
        $('#PopupAppRejId').html("Revoke Timesheet");
        $("#ProjectNameid").html("Do you want to revoke <b> " + empname + "'s </b>timesheets within <b>" + Workedhrs + "</b> hours for duration <b>" + TimesheetDuration.replace('-', ' to ') + "</b> ");
        // $("#ProjectNameid").html("Do you want to revoke <b> " + empname + "'s </b>  <b>" + MonthYearname + "</b> timesheets within <b>" + Workedhrs + "</b> hours for <b>" + projectname + "</b>");
    }
    $('#ContainerGridDetail').show();

    //$("#ProjectNameid").html("Do you want to approve<b>" + empname + " </b> of Project <b>" + projectname + "</b> and  Worked Hours is <b>" + Workedhrs + "</b>  for Month of <b>" + MonthTimesheet + "</b>");



}
function LoadAllTimesheets() {

    window.location = '/Timesheet/AdminPreviewTimesheets';

}
function RevokeTimesheet(id) {
    
    if (id === '5') {
        id = id;
        Commentss = '';
    }
    alert(TimesheetMonth);
    //else {
    //    TimesheetID = id.getAttribute("data-TimesheetID");
    //    MonthTimesheet = id.getAttribute("data-MonthYear");
    //    tsuserid = id.getAttribute("data-Usr_UserID");

    //    var ApproveRejectstatus = id.getAttribute("data-Status");
    //    id = ApproveRejectstatus;

    //}

    var projectid = ApporRejprojectid;
    var timesheets =

    {
        TimesheetID: TimesheetID,
        SubmittedType: id,
        TimeSheetMonth: TimesheetMonth,
        Comments: Commentss,
        UserID: tsuserid,
        ProjectID: projectid,
        Admin_Mailid: AdminEmailid,
        Admin_Name: AdminName

    }
    var sheetObj = {
        timesheets: timesheets,


    };

    $.ajax({
        type: "POST",
        url: "/Timesheet/TimeSheetManagerActionWeb",
        data: JSON.stringify(sheetObj),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            //sendmailsbyapp(sheetObj);
            alert(data);
            window.location.reload();
        }
    });

}


