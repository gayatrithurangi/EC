///// <reference path="jquery-1.10.2.js" />
// <reference path="bootstrap.min.js" />


var TimesheetID = '', fulldate = ''; var ClientProjId = '';
var resultDataArray; var objUserSessionId = '';
var resultHoursWorkedColour; var ApporRejprojectid; var SessionUsrid;
var empname = '', empworkedhrs = '', empprojectname = '', empactualhrs = '', attchemntid;
var uploadedImage = ""; var resultDataArrayUploadimages; var colourcode = '';
var TimesheetPeriod = ""; var TimesheetPeriodname = ""; var Tid;
var cid = ""; var TimeSheetMode = "";
var RoleStar = $("p#PFname").next("p").text().toUpperCase();

$(document).ready(function () {
    

       
    SessionUsrid = Userid;

    $("#tabCon").empty();
    $("#timesheet_management").addClass("active");
    $("#timesheet_management").siblings().removeClass("active");
    $("#body_ClientDetails").attr("style", "display: none;");
    $("#divEditTimesheetData").attr("style", "display: none;");
    $('#loading-image').attr("style", "display: block;");
    $.ajax({

        url: "/Timesheet/GetPreviewTimesheets",
        type: "GET",
        contentType: 'application/json; charset=utf-8',
        dataType: 'JSON',
        // cache: false,
        success: function (resultData) {
            
            console.log(resultData);
            //TimeSheetMode = resultData
            if (resultData.roleid) {
                objUserSessionId = resultData.roleid

                if ((objUserSessionId == '1007') || (objUserSessionId === '1002') || (objUserSessionId == "1053") || (objUserSessionId == "1061") || (objUserSessionId == "1006")) {

                    if ((objUserSessionId === '1007')|| (objUserSessionId == "1053") || (objUserSessionId == "1061") || (objUserSessionId == "1006")) {
                        $("#UserGridData").attr("style", "display: table;");
                        $("#ManagerGridData").attr("style", "display: table;");

                    }
                    else {

                        $("#UserGridData").attr("style", "display: none;");
                        $("#ManagerGridData").attr("style", "display: none;");
                        $("#UserGridPanel").attr("style", "display: table;");

                    }
                    if (resultData.mytimesheets) {
                        var objUsertimesheets1 = resultData.mytimesheets;
                       
                        $('#UserGridData').DataTable({
                            // 'destroy': true,

                            'data': objUsertimesheets1,
                            'paginate': true,
                            'sort': true,
                            'Processing': true,
                            'columns': [

                                { 'data': 'Usr_UserID', 'visible': false },
                                { 'data': 'TimesheetID', 'visible': false },
                                { 'data': 'ProjectId', 'visible': false },
                                { 'data': 'ClientprojectId', 'visible': false },
                                { 'data': 'TimesheetMode', 'visible': false },
                                { 'data': 'ProjectName' },
                                { 'data': 'ClientProjectName' },

                                //ClientProjectName = dr["ClientProjTitle"].ToString(),
                                { 'data': 'userName' },
                                { 'data': 'Month_Year' },
                                { 'data': 'CompanyBillingHours' },
                                { 'data': 'ResourceWorkingHours' },

                                {
                                    "data": "TimesheetApprovalStatus",
                                    "data": "ManagerApprovalStatus",
                                    "data": function (data) {
                                        return '<span class="badge badge-radius" data-toggle="tooltip" id="' + data.TimesheetApprovalStatus + '" title="' + data.ManagerApprovalStatus + '" ></span>'
                                    }
                                },
                                {
                                    "render": function (TimesheetID, type, full, meta) {


                                        if ((full.TimesheetApprovalStatus == 'Saved Timesheet') || (full.TimesheetApprovalStatus == 'Rejected') || (full.TimesheetApprovalStatus == 'Rejected at Level_1 Manager') || (full.TimesheetApprovalStatus == 'Rejected at Level_2 Manager') || (full.TimesheetApprovalStatus == 'Revoked at Admin')) {

                                            return '<a class="btn btn-icn"   data-MonthYear="' + full.Month_Year +
                                                '"data-TimesheetID="' + full.TimesheetID +
                                                '"data-ClientprojectId="' + full.ClientprojectId +
                                                '" data-ProjectId="' + full.ProjectId +
                                                '"data-Usr_UserID="' + full.Usr_UserID +
                                                '"data-TimesheetMode="' + full.TimesheetMode +
                                                '"onclick="EditUser(this,' + full.ProjectId + ')" ><i  class="fa fa-edit" title="Edit"></i></a>';
                                        }
                                        else {
                                            return '<a class="btn btn-icn"  id="PreviewManagerUser"   data-MonthYear="' + full.Month_Year +
                                                '"data-TimesheetID="' + full.TimesheetID +
                                                '"data-ProjectId="' + full.ProjectId +
                                                '"data-Usr_UserID="' + full.Usr_UserID +
                                                '"data-ClientprojectId="' + full.ClientprojectId +
                                                '"data-TimesheetMode="' + full.TimesheetMode +
                                                '"title="Preview" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus +
                                                '"onclick="PreviewSubmitTimesheet(this,' + full.TimesheetMode + ',' + full.ProjectId+')" ><i class="fa fa-eye"></i></a>';
                                        }

                                    }
                                }
                            ],
                            'initComplete': function () {
                                this.api().columns([5, 6]).every(function () {
                                    
                                    var column = this;
                                    var select = $('<select class="form-control form-control-lg"><option value="">Select ' + $("#UserGridData tfoot th:nth-child(" + Math.round(this[0][0] - 4) + ")").text() + '</option></select>')
                                        .appendTo($(column.footer()).empty())
                                        .on('change', function () {
                                            var val = $.fn.dataTable.util.escapeRegex(
                                                $(this).val()
                                            );

                                            column
                                                .search(val ? '^' + val + '$' : '', true, false)
                                                .draw();
                                        });

                                    column.data().unique().sort().each(function (d, j) {
                                        select.append('<option value="' + d + '">' + d + '</option>')
                                    });
                                });
                            }
                        
                        });

                    }

                    if (resultData.timesheetsforapproval) {
                        console.log(resultData.timesheetsforapproval);
                        var objManagertimesheets2 = resultData.timesheetsforapproval;
                       
                        $('#ManagerGridData').DataTable({
                            //   'destroy': true,
                            'data': objManagertimesheets2,
                            'paginate': true,
                            'sort': false,
                            'Processing': true,
                            'columns': [
                                { 'data': 'Usr_UserID', 'visible': false },
                                { 'data': 'TimesheetID', 'visible': false },
                                { 'data': 'ProjectId', 'visible': false },
                                { 'data': 'ClientprojectId', 'visible': false },
                                { 'data': 'ProjectName'},
                                { 'data': 'ClientProjectName' },
                                { 'data': 'userName' },
                                { 'data': 'TimesheetDuration' },
                                { 'data': 'TimesheetType' },
                                {
                                    'data': 'CompanyBillingHours',
                                    'data': function (data, full) {
                                        if (data.CompanyBillingHours != 0) {
                                            return data.CompanyBillingHours;
                                        }
                                        else {
                                            return '';
                                        }
                                    }

                                },
                                {
                                    'data': 'ResourceWorkingHours',
                                    'data': function (data, full) {
                                        if (data.ResourceWorkingHours != 0) {
                                            return data.ResourceWorkingHours;
                                        }
                                        else {
                                            return '';
                                        }
                                    }

                                },
                                {
                                    "data": "TimesheetApprovalStatus",
                                    "data": "ManagerApprovalStatus",
                                    "data": function (data, full) {
                                       
                                        if (data.ManagerApprovalStatus != null && data.TimesheetApprovalStatus != null) {
                                            return '<span class="badge badge-radius" data-toggle="tooltip" id="' + data.TimesheetApprovalStatus +
                                                '" title="' + data.ManagerApprovalStatus + '"></span>'
                                        }
                                        else {
                                            return '';
                                        }
                                        


                                    }
                                },
                                {

                                    "render": function (TimesheetID, type, full, meta) {
                                        
                                        if (full.UProj_L1ManagerId == SessionUsrid && full.UProj_L1ManagerId != '') {

                                            if ((full.TimesheetApprovalStatus == "Approved By Level_1 Manager")
                                                || (full.TimesheetApprovalStatus == "Approved By Level_2 Manager")
                                                || (full.TimesheetApprovalStatus == "Approved By Level_1 Manager and Pending at Level_2 Manager")
                                                || (full.TimesheetApprovalStatus == "Approved by Both Managers")) {

                                                if (full.TimesheetType == "Bi-Weekly") {
                                                    return '<a class="btn btn-icn"   id="PreviewManager" data-ProjectName="' + full.ProjectName +
                                                        '"  data-userName="' + full.userName +
                                                        '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                        '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                        '"  data-MonthYear="' + full.Month_Year +
                                                        '"  data-TimesheetMonth="' + full.TimeSheetMonth +
                                                        '" data-TimesheetID="' + full.TimesheetID +
                                                        '"  data-Usr_UserID="' + full.Usr_UserID +
                                                        '" data-TimesheetApprovalStatus="'
                                                        + full.TimesheetApprovalStatus +
                                                        '"  data-ProjectId="' + full.ProjectId +
                                                        '"data-ClientprojectId="' + full.ClientprojectId +
                                                        '" data-ByMonthlyDates="' + full.ByMonthlyDates +
                                                        '"  data-UProj_L1ManagerId="' + full.UProj_L1ManagerId +
                                                        '" title="Preview" onclick="ByWeeklyPreviewManagerSubmitTimesheet(this)" ><i class="fa fa-eye"></i> </a><a class="btn btn-icn"   id="Accepttimesheet"  data-MonthYear="'
                                                        + full.Month_Year +

                                                        '" data-TimesheetID="' + full.TimesheetID +
                                                        '"  data-Usr_UserID="' + full.Usr_UserID +
                                                        '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus +
                                                        '"data-ClientprojectId="' + full.ClientprojectId +
                                                        '"   data-Status="3" data-ProjectId="' + full.ProjectId +
                                                        '" data-ByMonthlyDates="' + full.ByMonthlyDates +
                                                        '" data-TimesheetMonth="' + full.TimeSheetMonth +
                                                        '" data-ProjectName="' + full.ProjectName +
                                                        '"  data-userName="' + full.userName +
                                                        '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                        '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                        '"data-ClientprojectId="' + full.ClientprojectId +
                                                        '" title="Preview" onclick="ApprovalRejectTimesheetByWeekly(this)" ></a><a class="btn btn-icn"    id="Rejecttimesheet"  data-MonthYear="' + full.Month_Year +
                                                        '" data-TimesheetID="' + full.TimesheetID +
                                                        '"  data-Usr_UserID="' + full.Usr_UserID +
                                                        '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus +
                                                        '"  data-ProjectId="' + full.ProjectId +
                                                        '"  data-ByMonthlyDates="' + full.ByMonthlyDates +
                                                        '"  data-TimesheetMonth="' + full.TimeSheetMonth +
                                                        '"data-ClientprojectId="' + full.ClientprojectId +
                                                        '" data-Status="4" onclick="ApprovalRejectTimesheetByWeekly(this)" > </a>';

                                                }
                                                if (full.TimesheetType == "Bi-Monthly") {
                                                    return '<a class="btn btn-icn"   id="PreviewManager" data-ProjectName="' + full.ProjectName +
                                                        '"  data-userName="' + full.userName +
                                                        '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                        '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                        '"  data-MonthYear="' + full.Month_Year +
                                                        '"  data-TimesheetMonth="' + full.TimeSheetMonth +
                                                        '" data-TimesheetID="' + full.TimesheetID +
                                                        '"  data-Usr_UserID="' + full.Usr_UserID +
                                                        '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus +
                                                        '"  data-ProjectId="' + full.ProjectId +
                                                        '"data-ClientprojectId="' + full.ClientprojectId +
                                                        '" data-ByMonthlyDates="' + full.ByMonthlyDates +
                                                        '"  data-UProj_L1ManagerId="' + full.UProj_L1ManagerId +
                                                        '" title="Preview" onclick="ByMonthlyPreviewManagerSubmitTimesheet(this,' + full.ProjectId + ')" ><i class="fa fa-eye"></i> </a><a class="btn btn-icn"   id="Accepttimesheet"  data-MonthYear="' + full.Month_Year +
                                                        '" data-TimesheetID="' + full.TimesheetID + '"  data-Usr_UserID="' + full.Usr_UserID +
                                                        '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus +
                                                        '"   data-Status="3" data-ProjectId="' + full.ProjectId + '"data-ClientprojectId="' + full.ClientprojectId +
                                                        '" data-ByMonthlyDates="' + full.ByMonthlyDates + '" data-TimesheetMonth="' + full.TimeSheetMonth +
                                                        '" data-ProjectName="' + full.ProjectName + '"  data-userName="' + full.userName +
                                                        '" data-CompanyBillingHours="' + full.CompanyBillingHours + '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                        '" title="Preview" onclick="ApprovalRejectTimesheetByWeekly(this)" ></a><a class="btn btn-icn"    id="Rejecttimesheet"  data-MonthYear="' + full.Month_Year +
                                                        '" data-TimesheetID="' + full.TimesheetID + '"  data-Usr_UserID="' + full.Usr_UserID +
                                                        '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus + '"  data-ProjectId="' + full.ProjectId + '"data-ClientprojectId="' + full.ClientprojectId +
                                                        '"  data-ByMonthlyDates="' + full.ByMonthlyDates + '"  data-TimesheetMonth="' + full.TimeSheetMonth + '"data-ClientprojectId="' + full.ClientprojectId + '" data-Status="4" onclick="ApprovalRejectTimesheetByWeekly(this)" > </a>';

                                                }

                                                if (full.TimesheetType == "Weekly") {
                                                    return '<a class="btn btn-icn"   id="PreviewManager" data-ProjectName="' + full.ProjectName +
                                                        '"  data-userName="' + full.userName +
                                                        '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                        '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                        '"  data-MonthYear="' + full.Month_Year +
                                                        '"  data-TimesheetMonth="' + full.TimeSheetMonth +
                                                        '" data-TimesheetID="' + full.TimesheetID +
                                                        '"  data-Usr_UserID="' + full.Usr_UserID +
                                                        '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus +
                                                        '"  data-ProjectId="' + full.ProjectId +
                                                        '"data-ClientprojectId="' + full.ClientprojectId +
                                                        '" data-ByMonthlyDates="' + full.ByMonthlyDates +
                                                        '"  data-UProj_L1ManagerId="' + full.UProj_L1ManagerId +
                                                        '" title="Preview" onclick="WeeklyPreviewManagerSubmitTimesheet(this)" ><i class="fa fa-eye"></i> </a><a class="btn btn-icn"   id="Accepttimesheet"  data-MonthYear="' + full.Month_Year +
                                                        '" data-TimesheetID="' + full.TimesheetID +
                                                        '"  data-Usr_UserID="' + full.Usr_UserID +
                                                        '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus +
                                                        '"   data-Status="3" data-ProjectId="' + full.ProjectId +
                                                        '"data-ClientprojectId="' + full.ClientprojectId +
                                                        '" data-ByMonthlyDates="' + full.ByMonthlyDates +
                                                        '" data-TimesheetMonth="' + full.TimeSheetMonth +
                                                        '" data-ProjectName="' + full.ProjectName +
                                                        '"  data-userName="' + full.userName +
                                                        '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                        '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                        '" title="Preview" onclick="ApprovalRejectTimesheetWeekly(this)" ></a><a class="btn btn-icn"    id="Rejecttimesheet"  data-MonthYear="' + full.Month_Year +
                                                        '" data-TimesheetID="' + full.TimesheetID +
                                                        '"  data-Usr_UserID="' + full.Usr_UserID +
                                                        '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus +
                                                        '"  data-ProjectId="' + full.ProjectId +
                                                        '"  data-ByMonthlyDates="' + full.ByMonthlyDates +
                                                        '"  data-TimesheetMonth="' + full.TimeSheetMonth +
                                                        '"data-ClientprojectId="' + full.ClientprojectId +
                                                        '" data-Status="4" onclick="ApprovalRejectTimesheetWeekly(this)" > </a>';

                                                }
                                                else {
                                                    
                                                    console.log(full.TimesheetMode);

                                                    return '<a class="btn btn-icn"   id="PreviewManager"  data-MonthYear="' + full.Month_Year +
                                                        '" data-TimesheetID="' + full.TimesheetID +
                                                        '"  data-Usr_UserID="' + full.Usr_UserID +
                                                        '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus +
                                                        '"  data-ProjectId="' + full.ProjectId +
                                                        '" data-UProj_L1ManagerId="' + full.UProj_L1ManagerId +
                                                        '" data-ProjectName="' + full.ProjectName +
                                                        '"  data-userName="' + full.userName +
                                                        '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                        '"data-ClientprojectId="' + full.ClientprojectId +
                                                        '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                        '" title="Preview" onclick="PreviewManagerSubmitTimesheet(this,' + full.TimesheetMode + ')" ><i class="fa fa-eye"></i> </a><a class="btn btn-icn"   id="Accepttimesheet"  data-MonthYear="' + full.Month_Year +
                                                        '" data-TimesheetID="' + full.TimesheetID +
                                                        '"  data-Usr_UserID="' + full.Usr_UserID +
                                                        '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus +
                                                        '"    data-ProjectId="' + full.ProjectId +
                                                        '"data-ClientprojectId="' + full.ClientprojectId +
                                                        '"  data-ProjectName="' + full.ProjectName +
                                                        '"  data-userName="' + full.userName +
                                                        '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                        '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                        '"  data-TotalMonthName="' + full.TotalMonthName +
                                                        '"  data-Status="3"  title="Preview" onclick="ApprovalRejectTimesheet(this)" ></a><a class="btn btn-icn"    id="Rejecttimesheet"  data-ProjectName="' + full.ProjectName +
                                                        '"  data-userName="' + full.userName +
                                                        '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                        '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                        '"  data-MonthYear="' + full.Month_Year +
                                                        '" data-TimesheetID="' + full.TimesheetID +
                                                        '"  data-Usr_UserID="' + full.Usr_UserID +
                                                        '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus +
                                                        '"  data-ProjectId="' + full.ProjectId +
                                                        '"data-ClientprojectId="' + full.ClientprojectId +
                                                        '"  data-TotalMonthName="' + full.TotalMonthName +
                                                        '" data-Status="4" onclick="ApprovalRejectTimesheet(this)" > </a>';
                                                }


                                            }

                                            else {

                                                if (full.TimesheetType == "Bi-Weekly") {

                                                    return '<a class="btn btn-icn"    id="PreviewManager" data-ProjectName="' + full.ProjectName +
                                                        '"  data-userName="' + full.userName +
                                                        '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                        '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                        '"  data-MonthYear="' + full.Month_Year +
                                                        '" data-TimesheetMonth="' + full.TimeSheetMonth +
                                                        '" data-TimesheetID="' + full.TimesheetID +
                                                        '"  data-Usr_UserID="' + full.Usr_UserID +
                                                        '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus +
                                                        '" data-ProjectId="' + full.ProjectId +
                                                        '"data-ClientprojectId="' + full.ClientprojectId +
                                                        '"  data-ByMonthlyDates="' + full.ByMonthlyDates +
                                                        '"   data-UProj_L1ManagerId="' + full.UProj_L1ManagerId +
                                                        '" title="Preview" onclick="ByWeeklyPreviewManagerSubmitTimesheet(this)" ><i class="fa fa-eye"></i> </a><a class="btn btn-icn"   id="Accepttimesheet" data-MonthYear="' + full.Month_Year +
                                                        '" data-TimesheetID="' + full.TimesheetID +
                                                        '" data-Usr_UserID="' + full.Usr_UserID +
                                                        '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus +
                                                        '" data-Status="3" data-ProjectId="' + full.ProjectId +
                                                        '"data-ClientprojectId="' + full.ClientprojectId +
                                                        '" data-ByMonthlyDates="' + full.ByMonthlyDates +
                                                        '" data-TimesheetMonth="' + full.TimeSheetMonth +
                                                        '"  data-ProjectName="' + full.ProjectName +
                                                        '"  data-userName="' + full.userName +
                                                        '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                        '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                        '"  title="Preview" onclick="ManagerApprOrRejByWeekly(this)" ><i id="ApproveID" class="fa fa-check" title="Approve"></i> </a><a class="btn btn-icn"    id="Rejecttimesheet" data-ProjectName="' + full.ProjectName +
                                                        '"  data-userName="' + full.userName +
                                                        '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                        '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                        '" data-ProjectName="' + full.ProjectName +
                                                        '"  data-userName="' + full.userName +
                                                        '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                        '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                        '" data-MonthYear="' + full.Month_Year +
                                                        '" data-TimesheetID="' + full.TimesheetID +
                                                        '"  data-Usr_UserID="' + full.Usr_UserID +
                                                        '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus +
                                                        '"  data-ProjectId="' + full.ProjectId +
                                                        '"data-ClientprojectId="' + full.ClientprojectId +
                                                        '"  data-ByMonthlyDates="' + full.ByMonthlyDates +
                                                        '" data-TimesheetMonth="' + full.TimeSheetMonth +
                                                        '" data-Status="4"  onclick="ManagerApprOrRejByWeekly(this)" ><i id="RejectId" title="Reject" class="fa fa-times" ></i> </a>';
                                                }
                                                if (full.TimesheetType == "Bi-Monthly") {
                                                    return '<a class="btn btn-icn"   id="PreviewManager" data-ProjectName="' + full.ProjectName +
                                                        '"  data-userName="' + full.userName +
                                                        '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                        '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                        '"  data-MonthYear="' + full.Month_Year +
                                                        '" data-TimesheetMonth="' + full.TimeSheetMonth +
                                                        '" data-TimesheetID="' + full.TimesheetID +
                                                        '"  data-Usr_UserID="' + full.Usr_UserID +
                                                        '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus +
                                                        '" data-ProjectId="' + full.ProjectId +
                                                        '"data-ClientprojectId="' + full.ClientprojectId +
                                                        '"  data-ByMonthlyDates="' + full.ByMonthlyDates +
                                                        '"   data-UProj_L2ManagerId="' + full.UProj_L1ManagerId +
                                                        '" title="Preview" onclick="ByMonthlyPreviewManagerSubmitTimesheet(this,' + full.ProjectId + ')" ><i class="fa fa-eye"></i> </a><a class="btn btn-icn"   id="Accepttimesheet" data-MonthYear="' + full.Month_Year +
                                                        '" data-TimesheetID="' + full.TimesheetID +
                                                        '" data-Usr_UserID="' + full.Usr_UserID +
                                                        '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus +
                                                        '" data-Status="3" data-ProjectId="' + full.ProjectId +
                                                        '" data-ByMonthlyDates="' + full.ByMonthlyDates +
                                                        '" data-TimesheetMonth="' + full.TimeSheetMonth +
                                                        '"  data-ProjectName="' + full.ProjectName +
                                                        '"  data-userName="' + full.userName +
                                                        '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                        '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                        '"  title="Preview" onclick="ManagerApprOrRejByWeekly(this)" ><i id="ApproveID" class="fa fa-check" title="Approve"></i> </a><a class="btn btn-icn"    id="Rejecttimesheet" data-ProjectName="' + full.ProjectName +
                                                        '"  data-userName="' + full.userName +
                                                        '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                        '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                        '" data-ProjectName="' + full.ProjectName +
                                                        '"  data-userName="' + full.userName +
                                                        '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                        '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                        '" data-MonthYear="' + full.Month_Year +
                                                        '" data-TimesheetID="' + full.TimesheetID +
                                                        '"  data-Usr_UserID="' + full.Usr_UserID +
                                                        '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus +
                                                        '"  data-ProjectId="' + full.ProjectId + '"data-ClientprojectId="' + full.ClientprojectId +
                                                        '"  data-ByMonthlyDates="' + full.ByMonthlyDates +
                                                        '" data-TimesheetMonth="' + full.TimeSheetMonth +
                                                        '" data-Status="4" onclick="ManagerApprOrRejByWeekly(this)" ><i id="RejectId" title="Reject" class="fa fa-times" ></i> </a>';
                                                }

                                                if (full.TimesheetType == "Weekly") {
                                                    return '<a class="btn btn-icn"   id="PreviewManager" data-ProjectName="' + full.ProjectName +
                                                        '"  data-userName="' + full.userName +
                                                        '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                        '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                        '"  data-MonthYear="' + full.Month_Year +
                                                        '" data-TimesheetMonth="' + full.TimeSheetMonth +
                                                        '" data-TimesheetID="' + full.TimesheetID +
                                                        '"  data-Usr_UserID="' + full.Usr_UserID +
                                                        '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus +
                                                        '" data-ProjectId="' + full.ProjectId +
                                                        '"data-ClientprojectId="' + full.ClientprojectId +
                                                        '"  data-ByMonthlyDates="' + full.ByMonthlyDates +
                                                        '"   data-UProj_L1ManagerId="' + full.UProj_L1ManagerId +
                                                        '" title="Preview" onclick="WeeklyPreviewManagerSubmitTimesheet(this)" ><i class="fa fa-eye"></i> </a><a class="btn btn-icn"   id="Accepttimesheet" data-MonthYear="' + full.Month_Year +
                                                        '" data-TimesheetID="' + full.TimesheetID +
                                                        '" data-Usr_UserID="' + full.Usr_UserID +
                                                        '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus +
                                                        '" data-Status="3" data-ProjectId="' + full.ProjectId +
                                                        '" data-ByMonthlyDates="' + full.ByMonthlyDates +
                                                        '" data-TimesheetMonth="' + full.TimeSheetMonth +
                                                        '"  data-ProjectName="' + full.ProjectName +
                                                        '"  data-userName="' + full.userName +
                                                        '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                        '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                        '"  title="Preview" onclick="ManagerApprOrRejWeekly(this)" ><i id="ApproveID"  class="fa fa-check" title="Approve"></i> </a><a class="btn btn-icn"    id="Rejecttimesheet" data-ProjectName="' + full.ProjectName +
                                                        '"  data-userName="' + full.userName +
                                                        '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                        '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                        '" data-ProjectName="' + full.ProjectName +
                                                        '"  data-userName="' + full.userName +
                                                        '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                        '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                        '" data-MonthYear="' + full.Month_Year +
                                                        '" data-TimesheetID="' + full.TimesheetID +
                                                        '"  data-Usr_UserID="' + full.Usr_UserID +
                                                        '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus +
                                                        '"  data-ProjectId="' + full.ProjectId +
                                                        '"data-ClientprojectId="' + full.ClientprojectId +
                                                        '"  data-ByMonthlyDates="' + full.ByMonthlyDates +
                                                        '" data-TimesheetMonth="' + full.TimeSheetMonth +
                                                        '" data-Status="4" onclick="ManagerApprOrRejWeekly(this)" ><i id="RejectId" title="Reject" class="fa fa-times" ></i> </a>';

                                                }

                                                else {
                                                    return '<a class="btn btn-icn"   id="PreviewManager"  data-MonthYear="' + full.Month_Year +
                                                        '" data-TimesheetID="' + full.TimesheetID +
                                                        '"  data-Usr_UserID="' + full.Usr_UserID +
                                                        '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus +
                                                        '" data-ProjectId="' + full.ProjectId +
                                                        '"data-ClientprojectId="' + full.ClientprojectId +
                                                        '"  data-UProj_L1ManagerId="' + full.UProj_L1ManagerId +
                                                        '" data-ProjectName="' + full.ProjectName +
                                                        '"  data-userName="' + full.userName +
                                                        '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                        '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                        '"  title="Preview" onclick="PreviewManagerSubmitTimesheet(this,' + full.TimesheetMode + ')" ><i class="fa fa-eye"></i> </a><a class="btn btn-icn"   id="Accepttimesheet" data-MonthYear="' + full.Month_Year +
                                                        '" data-TimesheetID="' + full.TimesheetID +
                                                        '"  data-Usr_UserID="' + full.Usr_UserID +
                                                        '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus +
                                                        '"  data-ProjectId="' + full.ProjectId +
                                                        '"data-ClientprojectId="' + full.ClientprojectId +
                                                        '" data-ProjectName="' + full.ProjectName +
                                                        '"  data-userName="' + full.userName +
                                                        '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                        '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                        '"  data-TotalMonthName="' + full.TotalMonthName +
                                                        '"     data-Status="3" title="Preview" onclick="ManagerApprOrRej(this)" ><i id="ApproveID" class="fa fa-check" title="Approve"></i> </a><a class="btn btn-icn"    id="Rejecttimesheet" data-ProjectName="' + full.ProjectName +
                                                        '"  data-userName="' + full.userName +
                                                        '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                        '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                        '" data-MonthYear="' + full.Month_Year +
                                                        '" data-TimesheetID="' + full.TimesheetID +
                                                        '"  data-Usr_UserID="' + full.Usr_UserID +
                                                        '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus +
                                                        '"  data-ProjectId="' + full.ProjectId +
                                                        '"data-ClientprojectId="' + full.ClientprojectId +
                                                        '" data-TotalMonthName="' + full.TotalMonthName +
                                                        '"  data-Status="4" onclick="ManagerApprOrRej(this)" ><i id="RejectId" title="Reject" class="fa fa-times" ></i> </a>';
                                                }

                                            }
                                        }

                                        else if (full.UProj_L2ManagerId == SessionUsrid && full.UProj_L2ManagerId != '') {

                                            if ((full.TimesheetApprovalStatus == "Approved By Level_1 Manager")
                                                || (full.TimesheetApprovalStatus == "Approved By Level_2 Manager")
                                                //|| (full.TimesheetApprovalStatus == "Approved By Level_1 Manager and Pending at Level_2 Manager")
                                                || (full.TimesheetApprovalStatus == "Approved by Both Managers")) {

                                                if (full.TimesheetType == "Bi-Weekly") {
                                                    return '<a class="btn btn-icn"   id="PreviewManager" data-ProjectName="' + full.ProjectName +
                                                        '"  data-userName="' + full.userName +
                                                        '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                        '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                        '"  data-MonthYear="' + full.Month_Year +
                                                        '"  data-TimesheetMonth="' + full.TimeSheetMonth +
                                                        '" data-TimesheetID="' + full.TimesheetID +
                                                        '"  data-Usr_UserID="' + full.Usr_UserID +
                                                        '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus +
                                                        '"  data-ProjectId="' + full.ProjectId +
                                                        '"data-ClientprojectId="' + full.ClientprojectId +
                                                        '" data-ByMonthlyDates="' + full.ByMonthlyDates +
                                                        '"  data-UProj_L2ManagerId="' + full.UProj_L2ManagerId +
                                                        '" title="Preview" onclick="ByWeeklyPreviewManagerSubmitTimesheet(this)" ><i class="fa fa-eye"></i> </a><a class="btn btn-icn"   id="Accepttimesheet"  data-MonthYear="' + full.Month_Year +
                                                        '" data-TimesheetID="' + full.TimesheetID +
                                                        '"  data-Usr_UserID="' + full.Usr_UserID +
                                                        '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus +
                                                        '"   data-Status="3" data-ProjectId="' + full.ProjectId +
                                                        '"data-ClientprojectId="' + full.ClientprojectId +
                                                        '" data-ByMonthlyDates="' + full.ByMonthlyDates +
                                                        '" data-TimesheetMonth="' + full.TimeSheetMonth +
                                                        '" data-ProjectName="' + full.ProjectName +
                                                        '"  data-userName="' + full.userName +
                                                        '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                        '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                        '" title="Preview" onclick="ApprovalRejectTimesheetByWeekly(this)" ></a><a class="btn btn-icn"    id="Rejecttimesheet"  data-MonthYear="' + full.Month_Year + '" data-TimesheetID="' + full.TimesheetID + '"  data-Usr_UserID="' + full.Usr_UserID + '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus + '"  data-ProjectId="' + full.ProjectId + '"data-ClientprojectId="' + full.ClientprojectId + '"  data-ByMonthlyDates="' + full.ByMonthlyDates + '"  data-TimesheetMonth="' + full.TimeSheetMonth + '" data-Status="4" onclick="ApprovalRejectTimesheetByWeekly(this)" > </a>';

                                                }
                                                if (full.TimesheetType == "Bi-Monthly") {
                                                    return '<a class="btn btn-icn"   id="PreviewManager" data-ProjectName="' + full.ProjectName +
                                                        '"  data-userName="' + full.userName +
                                                        '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                        '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                        '"  data-MonthYear="' + full.Month_Year +
                                                        '"  data-TimesheetMonth="' + full.TimeSheetMonth +
                                                        '" data-TimesheetID="' + full.TimesheetID +
                                                        '"  data-Usr_UserID="' + full.Usr_UserID +
                                                        '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus +
                                                        '"  data-ProjectId="' + full.ProjectId + '"data-ClientprojectId="' + full.ClientprojectId +
                                                        '" data-ByMonthlyDates="' + full.ByMonthlyDates +
                                                        '"  data-UProj_L1ManagerId="' + full.UProj_L1ManagerId +
                                                        '" title="Preview" onclick="ByMonthlyPreviewManagerSubmitTimesheet(this,' + full.ProjectId + ')" ><i class="fa fa-eye"></i> </a><a class="btn btn-icn"   id="Accepttimesheet"  data-MonthYear="' + full.Month_Year + '" data-TimesheetID="' + full.TimesheetID + '"  data-Usr_UserID="' + full.Usr_UserID + '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus + '"   data-Status="3" data-ProjectId="' + full.ProjectId + '"data-ClientprojectId="' + full.ClientprojectId + '" data-ByMonthlyDates="' + full.ByMonthlyDates + '" data-TimesheetMonth="' + full.TimeSheetMonth + '" data-ProjectName="' + full.ProjectName + '"  data-userName="' + full.userName + '" data-CompanyBillingHours="' + full.CompanyBillingHours + '" data-ResourceWorkingHours="' + full.ResourceWorkingHours + '" title="Preview" onclick="ApprovalRejectTimesheetByWeekly(this)" ></a><a class="btn btn-icn"    id="Rejecttimesheet"  data-MonthYear="' + full.Month_Year + '" data-TimesheetID="' + full.TimesheetID + '"  data-Usr_UserID="' + full.Usr_UserID + '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus + '"  data-ProjectId="' + full.ProjectId + '"data-ClientprojectId="' + full.ClientprojectId + '"  data-ByMonthlyDates="' + full.ByMonthlyDates + '"  data-TimesheetMonth="' + full.TimeSheetMonth + '" data-Status="4" onclick="ApprovalRejectTimesheetByWeekly(this)" > </a>';

                                                }

                                                if (full.TimesheetType == "Weekly") {
                                                    return '<a class="btn btn-icn"   id="PreviewManager" data-ProjectName="' + full.ProjectName + '"  data-userName="' + full.userName + '" data-CompanyBillingHours="' + full.CompanyBillingHours + '" data-ResourceWorkingHours="' + full.ResourceWorkingHours + '"  data-MonthYear="' + full.Month_Year + '"  data-TimesheetMonth="' + full.TimeSheetMonth + '" data-TimesheetID="' + full.TimesheetID + '"  data-Usr_UserID="' + full.Usr_UserID + '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus + '"  data-ProjectId="' + full.ProjectId + '"data-ClientprojectId="' + full.ClientprojectId + '" data-ByMonthlyDates="' + full.ByMonthlyDates + '"  data-UProj_L2ManagerId="' + full.UProj_L2ManagerId + '" title="Preview" onclick="WeeklyPreviewManagerSubmitTimesheet(this)" ><i class="fa fa-eye"></i> </a><a class="btn btn-icn"   id="Accepttimesheet"  data-MonthYear="' + full.Month_Year + '" data-TimesheetID="' + full.TimesheetID + '"  data-Usr_UserID="' + full.Usr_UserID + '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus + '"   data-Status="3" data-ProjectId="' + full.ProjectId + '"data-ClientprojectId="' + full.ClientprojectId + '" data-ByMonthlyDates="' + full.ByMonthlyDates + '" data-TimesheetMonth="' + full.TimeSheetMonth + '" data-ProjectName="' + full.ProjectName + '"  data-userName="' + full.userName + '" data-CompanyBillingHours="' + full.CompanyBillingHours + '" data-ResourceWorkingHours="' + full.ResourceWorkingHours + '" title="Preview" onclick="ApprovalRejectTimesheetWeekly(this)" ></a><a class="btn btn-icn"    id="Rejecttimesheet"  data-MonthYear="' + full.Month_Year + '" data-TimesheetID="' + full.TimesheetID + '"  data-Usr_UserID="' + full.Usr_UserID + '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus + '"  data-ProjectId="' + full.ProjectId + '"data-ClientprojectId="' + full.ClientprojectId + '"  data-ByMonthlyDates="' + full.ByMonthlyDates + '"  data-TimesheetMonth="' + full.TimeSheetMonth + '" data-Status="4" onclick="ApprovalRejectTimesheetWeekly(this)" > </a>';

                                                }
                                                else {


                                                    return '<a class="btn btn-icn"   id="PreviewManager"  data-MonthYear="'
                                                        + full.Month_Year +
                                                        '" data-TimesheetID="' + full.TimesheetID +
                                                        '"  data-Usr_UserID="' + full.Usr_UserID +
                                                        '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus +
                                                        '"  data-ProjectId="' + full.ProjectId +
                                                        '"data-ClientprojectId="' + full.ClientprojectId +
                                                        '" data-UProj_L2ManagerId="' + full.UProj_L2ManagerId +
                                                        '" data-ProjectName="' + full.ProjectName +
                                                        '"  data-userName="' + full.userName +
                                                        '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                        '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                        '" title="Preview" onclick="PreviewManagerSubmitTimesheet(this,' + full.TimesheetMode + ')" ><i class="fa fa-eye"></i> </a><a class="btn btn-icn"   id="Accepttimesheet" data-MonthYear="' + full.Month_Year + '" data-TimesheetID="' + full.TimesheetID + '"  data-Usr_UserID="' + full.Usr_UserID + '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus + '" data-ProjectId="' + full.ProjectId + '"  data-ProjectName="' + full.ProjectName + '"data-ClientprojectId="' + full.ClientprojectId + '"  data-userName="' + full.userName + '" data-CompanyBillingHours="' + full.CompanyBillingHours + '" data-ResourceWorkingHours="' + full.ResourceWorkingHours + '"  data-TotalMonthName="' + full.TotalMonthName + '"     data-Status="3"  title="Preview" onclick="ApprovalRejectTimesheet(this)" ></a><a class="btn btn-icn"    id="Rejecttimesheet"  data-ProjectName="' + full.ProjectName + '"  data-userName="' + full.userName + '" data-CompanyBillingHours="' + full.CompanyBillingHours + '" data-ResourceWorkingHours="' + full.ResourceWorkingHours + '"  data-MonthYear="' + full.Month_Year + '" data-TimesheetID="' + full.TimesheetID + '"  data-Usr_UserID="' + full.Usr_UserID + '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus + '"  data-ProjectId="' + full.ProjectId + '" data-TotalMonthName="' + full.TotalMonthName + '"  data-Status="4" onclick="ApprovalRejectTimesheet(this)" > </a>';
                                                    // return '<a class="btn btn-icn"   id="PreviewManager"  data-MonthYear="' + full.Month_Year + '" data-TimesheetID="' + full.TimesheetID + '"  data-Usr_UserID="' + full.Usr_UserID + '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus + '"  data-ProjectId="' + full.ProjectId + '" data-UProj_L1ManagerId="' + full.UProj_L1ManagerId + '" data-ProjectName="' + full.ProjectName + '"  data-userName="' + full.userName + '" data-CompanyBillingHours="' + full.CompanyBillingHours + '" data-ResourceWorkingHours="' + full.ResourceWorkingHours + '" title="Preview" onclick="PreviewManagerSubmitTimesheet(this)" ><i class="fa fa-eye"></i> </a><a class="btn btn-icn"   id="Accepttimesheet"  data-MonthYear="' + full.Month_Year + '" data-TimesheetID="' + full.TimesheetID + '"  data-Usr_UserID="' + full.Usr_UserID + '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus + '"    data-ProjectId="' + full.ProjectId + '"  data-ProjectName="' + full.ProjectName + '"  data-userName="' + full.userName + '" data-CompanyBillingHours="' + full.CompanyBillingHours + '" data-ResourceWorkingHours="' + full.ResourceWorkingHours + '"  data-TotalMonthName="' + full.TotalMonthName + '"  data-Status="3"  title="Preview" onclick="ApprovalRejectTimesheet(this)" ></a><a class="btn btn-icn"    id="Rejecttimesheet"  data-ProjectName="' + full.ProjectName + '"  data-userName="' + full.userName + '" data-CompanyBillingHours="' + full.CompanyBillingHours + '" data-ResourceWorkingHours="' + full.ResourceWorkingHours + '"  data-MonthYear="' + full.Month_Year + '" data-TimesheetID="' + full.TimesheetID + '"  data-Usr_UserID="' + full.Usr_UserID + '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus + '"  data-ProjectId="' + full.ProjectId + '"  data-TotalMonthName="' + full.TotalMonthName + '" data-Status="4" onclick="ApprovalRejectTimesheet(this)" > </a>';
                                                }



                                            }

                                            else if ((full.TimesheetApprovalStatus == "Approved By Level_1 Manager and Pending at Level_2 Manager")
                                                || (full.TimesheetApprovalStatus == "Waiting for Approval at Level_2 Manager")) {

                                                if (full.TimesheetType == "Bi-Weekly") {

                                                    return '<a class="btn btn-icn"   id="PreviewManager" data-ProjectName="' + full.ProjectName +
                                                        '"  data-userName="' + full.userName +
                                                        '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                        '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                        '"  data-MonthYear="' + full.Month_Year +
                                                        '" data-TimesheetMonth="' + full.TimeSheetMonth +
                                                        '" data-TimesheetID="' + full.TimesheetID +
                                                        '"  data-Usr_UserID="' + full.Usr_UserID +
                                                        '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus +
                                                        '" data-ProjectId="' + full.ProjectId +
                                                        '"data-ClientprojectId="' + full.ClientprojectId +
                                                        '" data-ByMonthlyDates="' + full.ByMonthlyDates +
                                                        '"   data-UProj_L2ManagerId="' + full.UProj_L1ManagerId +
                                                        '" title="Preview" onclick="ByWeeklyPreviewManagerSubmitTimesheet(this)" ><i class="fa fa-eye"></i> </a><a class="btn btn-icn"   id="Accepttimesheet" data-MonthYear="' + full.Month_Year +
                                                        '" data-TimesheetID="' + full.TimesheetID +
                                                        '" data-Usr_UserID="' + full.Usr_UserID +
                                                        '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus +
                                                        '" data-Status="3" data-ProjectId="' + full.ProjectId +
                                                        '"data-ClientprojectId="' + full.ClientprojectId +
                                                        '" data-ByMonthlyDates="' + full.ByMonthlyDates +
                                                        '" data-TimesheetMonth="' + full.TimeSheetMonth +
                                                        '"  data-ProjectName="' + full.ProjectName +
                                                        '"  data-userName="' + full.userName +
                                                        '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                        '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                        '"  title="Preview" onclick="ManagerApprOrRejByWeekly(this)" ><i id="ApproveID" class="fa fa-check" title="Approve"></i> </a><a class="btn btn-icn"    id="Rejecttimesheet" data-ProjectName="' + full.ProjectName +
                                                        '"  data-userName="' + full.userName +
                                                        '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                        '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                        '" data-ProjectName="' + full.ProjectName +
                                                        '"  data-userName="' + full.userName +
                                                        '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                        '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                        '" data-MonthYear="' + full.Month_Year +
                                                        '" data-TimesheetID="' + full.TimesheetID +
                                                        '"  data-Usr_UserID="' + full.Usr_UserID +
                                                        '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus +
                                                        '"  data-ProjectId="' + full.ProjectId +
                                                        '"data-ClientprojectId="' + full.ClientprojectId +
                                                        '"  data-ByMonthlyDates="' + full.ByMonthlyDates +
                                                        '" data-TimesheetMonth="' + full.TimeSheetMonth +
                                                        '" data-Status="4" onclick="ManagerApprOrRejByWeekly(this)" ><i id="RejectId" title="Reject" class="fa fa-times" ></i> </a>';
                                                }
                                                if (full.TimesheetType == "Bi-Monthly") {
                                                    return '<a class="btn btn-icn"   id="PreviewManager" data-ProjectName="' + full.ProjectName +
                                                        '"  data-userName="' + full.userName +
                                                        '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                        '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                        '"  data-MonthYear="' + full.Month_Year +
                                                        '" data-TimesheetMonth="' + full.TimeSheetMonth +
                                                        '" data-TimesheetID="' + full.TimesheetID +
                                                        '"  data-Usr_UserID="' + full.Usr_UserID +
                                                        '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus +
                                                        '" data-ProjectId="' + full.ProjectId +
                                                        '"data-ClientprojectId="' + full.ClientprojectId +
                                                        '"  data-ByMonthlyDates="' + full.ByMonthlyDates +
                                                        '"   data-UProj_L2ManagerId="' + full.UProj_L1ManagerId +
                                                        '" title="Preview" onclick="ByMonthlyPreviewManagerSubmitTimesheet(this,' + full.ProjectId + ')" ><i class="fa fa-eye"></i> </a><a class="btn btn-icn"   id="Accepttimesheet" data-MonthYear="' + full.Month_Year +
                                                        '" data-TimesheetID="' + full.TimesheetID +
                                                        '" data-Usr_UserID="' + full.Usr_UserID +
                                                        '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus +
                                                        '" data-Status="3" data-ProjectId="' + full.ProjectId +
                                                        '"data-ClientprojectId="' + full.ClientprojectId +
                                                        '" data-ByMonthlyDates="' + full.ByMonthlyDates +
                                                        '" data-TimesheetMonth="' + full.TimeSheetMonth +
                                                        '"  data-ProjectName="' + full.ProjectName +
                                                        '"  data-userName="' + full.userName +
                                                        '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                        '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                        '"  title="Preview" onclick="ManagerApprOrRejByWeekly(this)" ><i id="ApproveID" class="fa fa-check" title="Approve"></i> </a><a class="btn btn-icn"    id="Rejecttimesheet" data-ProjectName="' + full.ProjectName +
                                                        '"  data-userName="' + full.userName +
                                                        '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                        '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                        '" data-ProjectName="' + full.ProjectName +
                                                        '"  data-userName="' + full.userName +
                                                        '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                        '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                        '" data-MonthYear="' + full.Month_Year +
                                                        '" data-TimesheetID="' + full.TimesheetID +
                                                        '"  data-Usr_UserID="' + full.Usr_UserID +
                                                        '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus +
                                                        '"  data-ProjectId="' + full.ProjectId +
                                                        '"data-ClientprojectId="' + full.ClientprojectId +
                                                        '"  data-ByMonthlyDates="' + full.ByMonthlyDates +
                                                        '" data-TimesheetMonth="' + full.TimeSheetMonth +
                                                        '" data-Status="4" onclick="ManagerApprOrRejByWeekly(this)" ><i id="RejectId" title="Reject" class="fa fa-times" ></i> </a>';

                                                }
                                                if (full.TimesheetType == "Weekly") {
                                                    return '<a class="btn btn-icn"   id="PreviewManager" data-ProjectName="' + full.ProjectName + '"  data-userName="' + full.userName + '" data-CompanyBillingHours="' + full.CompanyBillingHours + '" data-ResourceWorkingHours="' + full.ResourceWorkingHours + '"  data-MonthYear="' + full.Month_Year + '" data-TimesheetMonth="' + full.TimeSheetMonth + '" data-TimesheetID="' + full.TimesheetID + '"  data-Usr_UserID="' + full.Usr_UserID + '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus + '" data-ProjectId="' + full.ProjectId + '"data-ClientprojectId="' + full.ClientprojectId + '"  data-ByMonthlyDates="' + full.ByMonthlyDates + '"   data-UProj_L2ManagerId="' + full.UProj_L1ManagerId + '" title="Preview" onclick="WeeklyPreviewManagerSubmitTimesheet(this)" ><i class="fa fa-eye"></i> </a><a class="btn btn-icn"   id="Accepttimesheet" data-MonthYear="' + full.Month_Year + '" data-TimesheetID="' + full.TimesheetID + '" data-Usr_UserID="' + full.Usr_UserID + '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus + '" data-Status="3" data-ProjectId="' + full.ProjectId + '"data-ClientprojectId="' + full.ClientprojectId + '" data-ByMonthlyDates="' + full.ByMonthlyDates + '" data-TimesheetMonth="' + full.TimeSheetMonth + '"  data-ProjectName="' + full.ProjectName + '"  data-userName="' + full.userName + '" data-CompanyBillingHours="' + full.CompanyBillingHours + '" data-ResourceWorkingHours="' + full.ResourceWorkingHours + '"  title="Preview" onclick="ManagerApprOrRejWeekly(this)" ><i id="ApproveID" class="fa fa-check" title="Approve"></i> </a><a class="btn btn-icn"    id="Rejecttimesheet" data-ProjectName="' + full.ProjectName + '"  data-userName="' + full.userName + '" data-CompanyBillingHours="' + full.CompanyBillingHours + '" data-ResourceWorkingHours="' + full.ResourceWorkingHours + '" data-ProjectName="' + full.ProjectName + '"  data-userName="' + full.userName + '" data-CompanyBillingHours="' + full.CompanyBillingHours + '" data-ResourceWorkingHours="' + full.ResourceWorkingHours + '" data-MonthYear="' + full.Month_Year + '" data-TimesheetID="' + full.TimesheetID + '"  data-Usr_UserID="' + full.Usr_UserID + '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus + '"  data-ProjectId="' + full.ProjectId + '"data-ClientprojectId="' + full.ClientprojectId + '"  data-ByMonthlyDates="' + full.ByMonthlyDates + '" data-TimesheetMonth="' + full.TimeSheetMonth + '" data-Status="4" onclick="ManagerApprOrRejWeekly(this)" ><i id="RejectId" title="Reject" class="fa fa-times" ></i> </a>';

                                                }

                                                else {

                                                    return '<a class="btn btn-icn"   id="PreviewManager"  data-MonthYear="' + full.Month_Year +
                                                        '" data-TimesheetID="' + full.TimesheetID +
                                                        '"  data-Usr_UserID="' + full.Usr_UserID +
                                                        '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus +
                                                        '" data-ProjectId="' + full.ProjectId +
                                                        '"  data-UProj_L2ManagerId="' + full.UProj_L2ManagerId +
                                                        '" data-ProjectName="' + full.ProjectName +
                                                        '"  data-userName="' + full.userName +
                                                        '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                        '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                        '"  title="Preview" onclick="PreviewManagerSubmitTimesheet(this,' + full.TimesheetMode + ')" ><i class="fa fa-eye"></i> </a><a class="btn btn-icn"   id="Accepttimesheet" data-ProjectName="' + full.ProjectName +
                                                        '"  data-userName="' + full.userName +
                                                        '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                        '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                        '" data-MonthYear="' + full.Month_Year +
                                                        '" data-TimesheetID="' + full.TimesheetID +
                                                        '"  data-Usr_UserID="' + full.Usr_UserID +
                                                        '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus +
                                                        '"  data-ProjectId="' + full.ProjectId +
                                                        '"data-ClientprojectId="' + full.ClientprojectId +
                                                        '"  data-TotalMonthName="' + full.TotalMonthName +
                                                        '"   data-Status="3" title="Preview" onclick="ManagerApprOrRej(this)" ><i id="ApproveID" class="fa fa-check" title="Approve"></i> </a><a class="btn btn-icn"    id="Rejecttimesheet" data-ProjectName="' + full.ProjectName +
                                                        '"data-ClientprojectId="' + full.ClientprojectId +
                                                        '" data-userName="' + full.userName +
                                                        '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                        '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                        '" data-MonthYear="' + full.Month_Year +
                                                        '" data-TimesheetID="' + full.TimesheetID +
                                                        '"  data-Usr_UserID="' + full.Usr_UserID +
                                                        '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus +
                                                        '"  data-ProjectId="' + full.ProjectId +
                                                        '"data-ClientprojectId="' + full.ClientprojectId +
                                                        '" data-TotalMonthName="' + full.TotalMonthName +
                                                        '"   data-Status="4" onclick="ManagerApprOrRej(this)" ><i id="RejectId" title="Reject" class="fa fa-times" ></i> </a>';

                                                    //return '<a class="btn btn-icn"   id="PreviewManager"  data-MonthYear="' + full.Month_Year + '" data-TimesheetID="' + full.TimesheetID + '"  data-Usr_UserID="' + full.Usr_UserID + '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus + '" data-ProjectId="' + full.ProjectId + '"  data-UProj_L1ManagerId="' + full.UProj_L1ManagerId + '" data-ProjectName="' + full.ProjectName + '"  data-userName="' + full.userName + '" data-CompanyBillingHours="' + full.CompanyBillingHours + '" data-ResourceWorkingHours="' + full.ResourceWorkingHours + '"  title="Preview" onclick="PreviewManagerSubmitTimesheet(this)" ><i class="fa fa-eye"></i> </a><a class="btn btn-icn"   id="Accepttimesheet" data-MonthYear="' + full.Month_Year + '" data-TimesheetID="' + full.TimesheetID + '"  data-Usr_UserID="' + full.Usr_UserID + '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus + '"  data-ProjectId="' + full.ProjectId + '" data-ProjectName="' + full.ProjectName + '"  data-userName="' + full.userName + '" data-CompanyBillingHours="' + full.CompanyBillingHours + '" data-ResourceWorkingHours="' + full.ResourceWorkingHours + '"  data-TotalMonthName="' + full.TotalMonthName + '"     data-Status="3" title="Preview" onclick="ManagerApprOrRej(this)" ><i id="ApproveID" class="fa fa-check" title="Approve"></i> </a><a class="btn btn-icn"    id="Rejecttimesheet" data-ProjectName="' + full.ProjectName + '"  data-userName="' + full.userName + '" data-CompanyBillingHours="' + full.CompanyBillingHours + '" data-ResourceWorkingHours="' + full.ResourceWorkingHours + '" data-MonthYear="' + full.Month_Year + '" data-TimesheetID="' + full.TimesheetID + '"  data-Usr_UserID="' + full.Usr_UserID + '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus + '"  data-ProjectId="' + full.ProjectId + '" data-TotalMonthName="' + full.TotalMonthName + '"  data-Status="4" onclick="ManagerApprOrRej(this)" ><i id="RejectId" title="Reject" class="fa fa-times" ></i> </a>';



                                                }





                                            }
                                            else {

                                                return '<a class="btn btn-icn"   id="PreviewManager"  data-MonthYear="' + full.Month_Year +
                                                    '" data-TimesheetID="' + full.TimesheetID +
                                                    '"  data-Usr_UserID="' + full.Usr_UserID +
                                                    '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus +
                                                    '" data-ProjectId="' + full.ProjectId +
                                                    '"data-ClientprojectId="' + full.ClientprojectId +
                                                    '"  data-UProj_L1ManagerId="' + full.UProj_L1ManagerId +
                                                    '" data-ProjectName="' + full.ProjectName +
                                                    '"  data-userName="' + full.userName +
                                                    '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                    '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                    '"  title="Preview" onclick="PreviewManagerSubmitTimesheet(this,' + full.TimesheetMode + ')" ><i class="fa fa-eye"></i> </a><a class="btn btn-icn"   id="Accepttimesheet" data-ProjectName="' + full.ProjectName +
                                                    '"  data-userName="' + full.userName +
                                                    '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                    '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                    '" data-MonthYear="' + full.Month_Year +
                                                    '" data-TimesheetID="' + full.TimesheetID +
                                                    '"  data-Usr_UserID="' + full.Usr_UserID +
                                                    '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus +
                                                    '"  data-ProjectId="' + full.ProjectId +
                                                    '"data-ClientprojectId="' + full.ClientprojectId +
                                                    '"  data-TotalMonthName="' + full.TotalMonthName +
                                                    '"   data-Status="3" title="Preview" onclick="ManagerApprOrRej(this)" ><i id="ApproveID" class="fa fa-check" title="Approve"></i> </a><a class="btn btn-icn"    id="Rejecttimesheet" data-ProjectName="' + full.ProjectName +
                                                    '"data-ClientprojectId="' + full.ClientprojectId +
                                                    '" data-userName="' + full.userName +
                                                    '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                    '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                    '" data-MonthYear="' + full.Month_Year +
                                                    '" data-TimesheetID="' + full.TimesheetID +
                                                    '"  data-Usr_UserID="' + full.Usr_UserID +
                                                    '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus +
                                                    '"  data-ProjectId="' + full.ProjectId +
                                                    '"data-ClientprojectId="' + full.ClientprojectId +
                                                    '" data-TotalMonthName="' + full.TotalMonthName +
                                                    '"   data-Status="4" onclick="ManagerApprOrRej(this)" ><i id="RejectId" title="Reject" class="fa fa-times" ></i> </a>';

                                            }

                                        }
                                        else if (full.UProj_L1ManagerId == null || full.UProj_L1ManagerId == '' || full.UProj_L2ManagerId == null || full.UProj_L2ManagerId == '') {
                                          
                                            return '';
                                        }


                                        else {

                                            //return '';


                                            return '<a class="btn btn-icn"   id="PreviewManager"  data-MonthYear="' + full.Month_Year +
                                                '" data-TimesheetID="' + full.TimesheetID +
                                                '"  data-Usr_UserID="' + full.Usr_UserID +
                                                '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus +
                                                '" data-ProjectId="' + full.ProjectId +
                                                '"data-ClientprojectId="' + full.ClientprojectId +
                                                '"  data-UProj_L1ManagerId="' + full.UProj_L1ManagerId +
                                                '" data-ProjectName="' + full.ProjectName +
                                                '"  data-userName="' + full.userName +
                                                '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                '"  title="Preview" onclick="PreviewManagerSubmitTimesheet(this,' + full.TimesheetMode + ')" ><i class="fa fa-eye"></i> </a><a class="btn btn-icn"   id="Accepttimesheet" data-ProjectName="' + full.ProjectName +
                                                '"  data-userName="' + full.userName +
                                                '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                '" data-MonthYear="' + full.Month_Year +
                                                '" data-TimesheetID="' + full.TimesheetID +
                                                '"  data-Usr_UserID="' + full.Usr_UserID +
                                                '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus +
                                                '"  data-ProjectId="' + full.ProjectId +
                                                '"data-ClientprojectId="' + full.ClientprojectId +
                                                '"  data-TotalMonthName="' + full.TotalMonthName +
                                                '"   data-Status="3" title="Preview" onclick="ManagerApprOrRej(this)" ><i id="ApproveID" class="fa fa-check" title="Approve"></i> </a><a class="btn btn-icn"    id="Rejecttimesheet" data-ProjectName="' + full.ProjectName +
                                                '"data-ClientprojectId="' + full.ClientprojectId +
                                                '" data-userName="' + full.userName +
                                                '" data-CompanyBillingHours="' + full.CompanyBillingHours +
                                                '" data-ResourceWorkingHours="' + full.ResourceWorkingHours +
                                                '" data-MonthYear="' + full.Month_Year +
                                                '" data-TimesheetID="' + full.TimesheetID +
                                                '"  data-Usr_UserID="' + full.Usr_UserID +
                                                '" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus +
                                                '"  data-ProjectId="' + full.ProjectId +
                                                '"data-ClientprojectId="' + full.ClientprojectId +
                                                '" data-TotalMonthName="' + full.TotalMonthName +
                                                '"   data-Status="4" onclick="ManagerApprOrRej(this)" ><i id="RejectId" title="Reject" class="fa fa-times" ></i> </a>';


                                        }


                                    }

                                }

                            ],
                            'initComplete': function () {
                                this.api().columns([4,5]).every(function () {
                                    
                                    var column = this;
                                    var select = $('<select class="form-control form-control-lg"><option value="">Select ' + $("#ManagerGridData tfoot th:nth-child(" + Math.round(this[0][0] - 3) + ")").text() + '</option></select>')
                                        .appendTo($(column.footer()).empty())
                                        .on('change', function () {
                                            var val = $.fn.dataTable.util.escapeRegex(
                                                $(this).val()
                                            );

                                            column
                                                .search(val ? '^' + val + '$' : '', true, false)
                                                .draw();
                                        });

                                    column.data().unique().sort().each(function (d, j) {
                                        select.append('<option value="' + d + '">' + d + '</option>')
                                    });
                                });
                            }
                        });
                    }
                }
                else {

                    $("#ManagerGridData").attr("style", "display: none;");
                    $("#UserGridData").attr("style", "display: table;");
                    $("#ManagerGridPanel").attr("style", "display: none;");
                    if (resultData.mytimesheets) {
                       
                        var objUsertimesheets3 = resultData.mytimesheets;
                        $('#UserGridData').DataTable({
                            // 'destroy': true,
                            'data': objUsertimesheets3,
                            'paginate': true,
                            'sort': true,
                            'Processing': true,
                            'columns': [
                                { 'data': 'Usr_UserID', 'visible': false },
                                { 'data': 'TimesheetID', 'visible': false },
                                { 'data': 'ProjectId', 'visible': false },
                                { 'data': 'ClientprojectId', 'visible': false },
                                { 'data': 'TimesheetMode', 'visible': false },
                                { 'data': 'ProjectName' },
                                { 'data': 'ClientProjectName' },
                                { 'data': 'userName' },
                                { 'data': 'Month_Year' },
                                { 'data': 'CompanyBillingHours' },
                                { 'data': 'ResourceWorkingHours' },
                                {
                                    "data": "TimesheetApprovalStatus",
                                    "data": "ManagerApprovalStatus",
                                    "data": function (data) {

                                        return '<span class="badge badge-radius" data-toggle="tooltip" id="' + data.TimesheetApprovalStatus +
                                            '" title="' + data.ManagerApprovalStatus + '"></span>'
                                    }
                                },
                                {
                                    "render": function (TimesheetID, type, full, meta) {
                                        console.log(full.TimesheetMode);

                                        if ((full.TimesheetApprovalStatus == 'Saved Timesheet') || (full.TimesheetApprovalStatus == 'Rejected') || (full.TimesheetApprovalStatus == 'Rejected at Level_1 Manager') || (full.TimesheetApprovalStatus == 'Rejected at Level_2 Manager') || (full.TimesheetApprovalStatus == 'Revoked at Admin')) {

                                            return '<a class="btn btn-icn"   data-MonthYear="' + full.Month_Year +
                                                '" data-TimesheetID="' + full.TimesheetID +
                                                '"  data-Usr_UserID="' + full.Usr_UserID +
                                                '" data-ProjectId="' + full.ProjectId +
                                                '"data-ClientprojectId="' + full.ClientprojectId +
                                                '"data-TimesheetMode="' + full.TimesheetMode +
                                                '" onclick="EditUser(this,' + full.ProjectId + ')"><i class="fa fa-edit" title="Edit"></i></a>';
                                        }

                                        else {

                                            return '<a class="btn btn-icn" id="PreviewUser"   data-MonthYear="' + full.Month_Year +
                                                '" data-TimesheetID="' + full.TimesheetID +
                                                '" data-Usr_UserID="' + full.Usr_UserID +
                                                '" data-ProjectId="' + full.ProjectId +
                                                '"data-ClientprojectId="' + full.ClientprojectId +
                                                '" data-UpImage="' + full.Uploadedimages + '"data-TimesheetMode="' + full.TimesheetMode +
                                                '" data-attachId="' + full.Attachmentid +
                                                '"  title="Preview"  onclick="PreviewSubmitTimesheet(this,' + full.TimesheetMode + ', ' + full.ProjectId+')" ><i class="fa fa-eye"></i></a>';

                                        }

                                    },

                                },

                            ],
                            'initComplete': function () {
                                this.api().columns([5, 6]).every(function () {
                                    
                                    var column = this;
                                    var select = $('<select class="form-control form-control-lg"><option value="">Select ' + $("#UserGridData tfoot th:nth-child(" + Math.round(this[0][0] - 4) + ")").text() + '</option></select>')
                                        .appendTo($(column.footer()).empty())
                                        .on('change', function () {
                                            var val = $.fn.dataTable.util.escapeRegex(
                                                $(this).val()
                                            );

                                            column
                                                .search(val ? '^' + val + '$' : '', true, false)
                                                .draw();
                                        });

                                    column.data().unique().sort().each(function (d, j) {
                                        select.append('<option value="' + d + '">' + d + '</option>')
                                    });
                                });
                            }
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

});

var TaskDetailsid = ''; var MonthTimesheet = ''; var tsuserid = '', AppRejectstatus = '';

var loadFile = function (event) {
    var output = document.getElementById('imgLogo');

    output.src = URL.createObjectURL(event.target.files[0]);
    $('#imgLogo').show();
};
function EditUser(id,pr_id) {
    
    TimeSheetMode = id.getAttribute("data-TimesheetMode");
    cid = id.getAttribute("data-ClientprojectId");
    if (TimeSheetMode == "1") {
        $("#body_ClientDetails").attr("style", "display: table;");
        TimesheetID = id.getAttribute("data-TimesheetID");
        MonthTimesheet = id.getAttribute("data-MonthYear");
        tsuserid = id.getAttribute("data-Usr_UserID");


        $("#divPreviewTimesheet").attr("style", "display: none;");
        $("#divEditTimesheetData .btn-app").attr("style", "display: none;");
        $("#divEditTimesheetData").attr("style", "display: block;");
        $('#loading-image').attr("style", "display: block;");
        $.ajax({

            url: "/Timesheet/ViewSubmitedTimesheet",
            type: "GET",
            data: { TimesheetMonth: MonthTimesheet, TimesheetUserid: tsuserid, clientProjectid: cid, TimesheetID: TimesheetID },
            contentType: 'application/json; charset=utf-8',
            dataType: 'JSON',
            async: false,
            success: function (resultData) {
                
                resultDataArray = resultData.timeSheetDetails;
                resultDataArrayUploadimages = resultData.UploadTimesheetimage;
                LoadColoursonLeavesandHolidays(MonthTimesheet, tsuserid);
                ViewTimesheetByMonth(MonthTimesheet, resultDataArray, TimeSheetMode);
                LoadPreviewTasklookups();
            },
            complete: function () {
                $('#loading-image').attr("style", "display: none;");
            }

        });
        LoadPreviewTaskData();
    }

    else if (TimeSheetMode == "2") {

        $.ajax({
            url: "/Timesheet/GetByMonthlyPreviewTimesheets",
            type: "GET",
            data: { mode: TimeSheetMode },
            contentType: 'application/json; charset=utf-8',
            dataType: 'JSON',
            // cache: false,
            success: function (resultData) {
                console.log(resultData);
                TimesheetID = id.getAttribute("data-TimesheetID");
                TSID = TimesheetID;
                let dat = resultData.mytimesheets.filter(function (e) {
                    return (e.TimesheetID == TSID);

                });


                $("#body_ClientDetails").attr("style", "display: table;");
                $("#Timesheet_dvTSMode").html("ByWeekly");
                

                MonthTimesheet = dat[0].Month_Year;
                tsuserid = id.getAttribute("data-Usr_UserID");
                ByMonthlydates = dat[0].ByMonthlyDates;
                cid = id.getAttribute("data-ClientprojectId");
                var Timesheetmonarray3 = new Array();
                Timesheetmonarray3 = ByMonthlydates.split("-");
                TimesheetMonth = Timesheetmonarray3[1];
                ByMonthlystartdate = Timesheetmonarray3[0]; ByMonthlyenddate = Timesheetmonarray3[1];
                $("#divPreviewTimesheet").attr("style", "display: none;");
                $("#divEditTimesheetData .btn-app").attr("style", "display: none;");
                $("#divEditTimesheetData").attr("style", "display: block;");
                $('#loading-image').attr("style", "display: block;");
                $.ajax({
                    url: "/Timesheet/ViewByWeeklySubmitedTimesheet",
                    type: "GET",
                    data: {
                        TimesheetUserid: tsuserid, Timesheetstartdate: Timesheetmonarray3[0],
                        TimesheetEnddate: Timesheetmonarray3[1], Accountid: SessionAccountid,
                        Projectid: pr_id,
                        ClientProjectId: cid
                    },
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'JSON',
                    async: false,
                    success: function (resultData) {
                        resultDataArray = resultData.timeSheetDetails;
                        resultDataArrayUploadimages = resultData.UploadTimesheetimage;
                        PreviewByMonthlyLoadHoursWorkedAcctoDate(tsuserid, Timesheetmonarray3[0], Timesheetmonarray3[1], SessionAccountid, SessionProjectid);
                        LoadColoursonLeavesandHolidays(TimesheetMonth, tsuserid);
                        ViewTimesheetByWeekly(TimesheetMonth, resultDataArray);
                        LoadPreviewTasklookups();
                    },
                    complete: function () {
                        $('#loading-image').attr("style", "display: none;");
                    }

                });

                LoadPreviewTaskData();

            },
            complete: function () {
                $('#loading-image').attr("style", "display: none;");
            },


        });






    }
    else if (TimeSheetMode == "3") {


        $.ajax({
            url: "/Timesheet/GetWeeklyPreviewTimesheets",
            type: "GET",
            contentType: 'application/json; charset=utf-8',
            dataType: 'JSON',
            success: function (resultData) {
                
                $("#body_ClientDetails").attr("style", "display: table;");
                TimesheetID = id.getAttribute("data-TimesheetID");
                TSID = TimesheetID;

                let dat = resultData.mytimesheets.filter(function (e) {
                    return (e.TimesheetID == TSID);

                });

                var proj_id = pr_id;//id.getAttribute("data-ProjectId");
                MonthTimesheet = dat[0].Month_Year;
                tsuserid = id.getAttribute("data-Usr_UserID");
                ByMonthlydates = dat[0].ByMonthlyDates;
                cid = id.getAttribute("data-ClientprojectId");
                var Timesheetmonarray3 = new Array();
                Timesheetmonarray3 = ByMonthlydates.split("-");
                TimesheetMonth = Timesheetmonarray3[1];
                ByMonthlystartdate = Timesheetmonarray3[0]; ByMonthlyenddate = Timesheetmonarray3[1];
                $("#divPreviewTimesheet").attr("style", "display: none;");
                $("#divEditTimesheetData .btn-app").attr("style", "display: none;");
                $("#divEditTimesheetData").attr("style", "display: block;");
                $('#loading-image').attr("style", "display: block;");

                // SessionUsrid, Timesheetmonarray2[0], Timesheetmonarray2[1], SessionAccountid, SessionProjectid
                $.ajax({
                    url: "/Timesheet/ViewByWeeklySubmitedTimesheet",
                    type: "GET",
                    data: {
                        TimesheetUserid: tsuserid, Timesheetstartdate: Timesheetmonarray3[0],
                        TimesheetEnddate: Timesheetmonarray3[1], Accountid: SessionAccountid,
                        //Projectid: SessionProjectid,
                        Projectid: proj_id,
                        ClientProjectId: cid
                    },
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'JSON',
                    async: false,
                    success: function (resultData) {
                        
                        resultDataArray = resultData.timeSheetDetails;
                        resultDataArrayUploadimages = resultData.UploadTimesheetimage;
                        PreviewByMonthlyLoadHoursWorkedAcctoDate(tsuserid, Timesheetmonarray3[0], Timesheetmonarray3[1], SessionAccountid, SessionProjectid);
                        LoadColoursonLeavesandHolidays(TimesheetMonth, tsuserid)
                        ViewTimesheetByMonthWeekly(TimesheetMonth, resultDataArray);
                        LoadPreviewTasklookups();
                    },
                    complete: function () {
                        $('#loading-image').attr("style", "display: none;");
                    }

                });

                LoadPreviewTaskData();
            },

            error: function (data) {
                //  alert(data.responseText);
            }
        });

    }
    else if (TimeSheetMode == "4") {
        
        $.ajax({
            url: "/Timesheet/GetByMonthlyPreviewTimesheets15day",
            type: "GET",
            contentType: 'application/json; charset=utf-8',
            dataType: 'JSON',
            // cache: false,
            success: function (resultData) {
                

                TimesheetID = id.getAttribute("data-TimesheetID");
                TSID = TimesheetID;

                let dat = resultData.mytimesheets.filter(function (e) {
                    return (e.TimesheetID == TimesheetID);

                });

                $("#body_ClientDetails").attr("style", "display: table;");
                $("#Timesheet_dvTSMode").html("Bi-Monthly");
                var proj_id = pr_id;

                MonthTimesheet = dat[0].Month_Year;
                tsuserid = id.getAttribute("data-Usr_UserID");
                ByMonthlydates = dat[0].ByMonthlyDates;

                var Timesheetmonarray3 = new Array();
                Timesheetmonarray3 = ByMonthlydates.split("-");
                TimesheetMonth = Timesheetmonarray3[1];
                ByMonthlystartdate = Timesheetmonarray3[0]; ByMonthlyenddate = Timesheetmonarray3[1];
                $("#divPreviewTimesheet").attr("style", "display: none;");
                $("#divEditTimesheetData .btn-app").attr("style", "display: none;");
                $("#divEditTimesheetData").attr("style", "display: block;");
                $('#loading-image').attr("style", "display: block;");

                // SessionUsrid, Timesheetmonarray2[0], Timesheetmonarray2[1], SessionAccountid, SessionProjectid
                $.ajax({
                    url: "/Timesheet/ViewByWeeklySubmitedTimesheet",
                    type: "GET",
                    data: {
                        TimesheetUserid: tsuserid, Timesheetstartdate: Timesheetmonarray3[0],
                        TimesheetEnddate: Timesheetmonarray3[1], Accountid: SessionAccountid,
                        Projectid: pr_id, ClientProjectId: cid
                    },
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'JSON',
                    async: false,
                    success: function (resultData) {
                        
                        resultDataArray = resultData.timeSheetDetails;
                        resultDataArrayUploadimages = resultData.UploadTimesheetimage;
                        PreviewByMonthlyLoadHoursWorkedAcctoDate(tsuserid, Timesheetmonarray3[0], Timesheetmonarray3[1], SessionAccountid, SessionProjectid);
                        LoadColoursonLeavesandHolidays(TimesheetMonth, tsuserid);
                        ViewTimesheetsByWeekly(TimesheetMonth, resultDataArray);
                        LoadPreviewTasklookups();
                    },
                    complete: function () {
                        $('#loading-image').attr("style", "display: none;");
                    }

                });

                LoadPreviewTaskData();



            },

            error: function (data) {
                // alert(data.responseText);
            }
        });


    }

}

var Submittedtype = ""; var TaskLookupId = "", workingHours = "", Commentss = "";
function PConfirmSendTimesheet(id) {
    
    var savetimesheetid = TimesheetID;
    var files = $("#flImage").get(0).files;
    var formData = new FormData();

    for (var i = 0; i < files.length; i++) {
        formData.append("fileInput", files[i]);
    }
    var files1 = $("#flImage1").get(0).files;
    for (var i = 0; i < files1.length; i++) {
        formData.append("fileInput", files1[i]);
    }
    $.ajax({
        type: 'post',
        url: '/Timesheet/SaveToTemp',
        data: formData,
        success: function (response) {
            if (response != null) {
                var my_path = "/temp/" + response;
                var image = '<img src="' + my_path + '" alt="image" style="width:150px">';
                $("#imgPreview").append(image);
            }
        },
        processData: false,
        contentType: false,
        error: function () {
            alert("Whoops something went wrong!");
        }
    });


    if (id == '1') {
        Submittedtype = 'Save';
    }
    if (id == '2') {

        Submittedtype = 'Submit';
    }

    Commentss = $.trim($("#txtDescription").val());



    var rows = $("#SubTable,#SubTable2").find("tr");

    var listtimesheetdetails = []; var rowData = {}; var Date, TaskId, hours;
    var satdates = [];
    for (var rowOn = 1; rowOn < rows.length; rowOn++) {

        TaskLookupId = $(rows[rowOn]).find('.lookup').attr('id');
        workingHours = $(rows[rowOn]).find('.uc1txtHours').attr('id');
        colourcode = $(rows[rowOn]).find('span').attr('title');
        Date = $(rows[rowOn]).find("td").eq(0).text();
        TaskId = $("#" + TaskLookupId).val();
        hours = $("#" + workingHours).val();

        if (hours == "") {
            if (!$.trim(this.value)) {
                alert("Please enter working hours");
                $(".uc1txtHours").focus();
                calculateSum();
                return false;
            }
        }

        if (hours != "") {
            if (hours < 0) {
                alert("Working hours should be not be less than zero");
                $("#" + workingHours).val('');
                $("#uc1_txtHours").focus();
                calculateSum();
                return false;
            }
        }



        if (hours != "") {
            if (hours > 24) {
                alert("Working hours should not be greater than 24 hours");
                $("#" + workingHours).val('');
                $("#uc1_txtHours").focus();
                calculateSum();
                return false;
            }
        }


        if ((colourcode == "blue" && colourcode != undefined) || (colourcode == "black" && colourcode != undefined) || (colourcode == "Red" && colourcode != undefined)) {

            if (hours > 0) {
                satdates.push(Date);


                hours = $("#" + workingHours).val();
                calculateSum();




            }
        }



        if (rowOn != 17) {
            if (TaskId != undefined && hours != undefined && Date != null) {
                rowData = { taskDate: Date, taskid: TaskId, hoursWorked: hours }
            }
        }
        else {
            continue;
        }

        listtimesheetdetails.push(rowData);
    }

    //alert("Do you want to fill holidaydate of " + satdates + " ?");
    if (satdates.length != 0) {
        if (confirm("Do you want to enter hours for " + satdates + " ?")) {

            $("span[id^='HoursColours'][title='black']").closest('tr').removeClass("brown");

        } else {
            $("span[id^='HoursColours'][title='black']").closest('tr').addClass("brown");
            return false;
        }
    }


    var timesheets = {
        TimesheetID: TimesheetID,
        TimeSheetMonth: ByMonthlystartdate,
        ByWeeklyStartDate: ByMonthlystartdate,
        ByWeeklyEndDate: ByMonthlyenddate,
        Comments: Commentss,
        SubmittedType: Submittedtype,
        TimesheetMode: '3',
        ClientProjectId: cid
    }

    var sheetObj = {

        timesheets: timesheets,
        listtimesheetdetails: listtimesheetdetails

    };

    $('#loading-image').attr("style", "display: block;");
    $.ajax({
        type: "POST",
        url: "/Timesheet/updateTimesheetTaskDetails",
        data: JSON.stringify(sheetObj),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            alert(data);
            window.location.reload();
        },
        complete: function () {
            $('#loading-image').attr("style", "display: none;");
        }
    });


}

function ViewTimesheetByMonth(MonthTimesheet, resultData, TimeSheetMode) {
    
    $("#body_ClientDetails").attr("style", "display: table;");
    $("#btnPrint").hide();
    $("#btnpdf").hide();
    $("#btnShare").hide();
    $("#tabCon").empty();
    $("#MainTable").empty();
    $("#SubTable").empty();
    // $("#flImage").show();
    $("#imagelbl, #imagelbl1").show();
    $("#images, #imagestwo").show();

    // TimeSheetMode = id.getAttribute("data-TimesheetMode");
    if (TimeSheetMode == "1") {
        $("#Timesheet_dvTSMode").html("Monthly");
    }
    else if (TimeSheetMode == "2") {
        $("#Timesheet_dvTSMode").html("Bi-Weekly");
    }
    else if (TimeSheetMode == "3") {
        $("#Timesheet_dvTSMode").html("Weekly");
    }
    else if (TimeSheetMode == "4") {
        $("#Timesheet_dvTSMode").html("Bi-Monthly");
    }
    //$("#Timesheet_dvTSMode").html("Monthly");
    var tbltask = ''; var objSelect = '';
    $('#Cmtsave').show();
    var counter = -1;
    var counter15 = -1;
    $("#ClientDetails2").show(true);
    $("#tabCon").append("<table name='maintbl' id='MainTable' width='100%'/*style='position:relative;left:10%;'*/>");

    $.each(resultData, function (k, v) {
        
        $("#StardateToEnddate").html("<span id='StartToEnd'><b>Timesheet of " + v.UserName + " for the Period of " + v.TotalMonthName + "</b></span>");
       
        if (v.Submitted_Type == 1 && v.L1_ApproverStatus == 1 && v.L2_ApproverStatus == 1) {
            $("#UserStatus").html("Approved by both Managers").css("color", "Green");
        }
        else if (v.Submitted_Type == 1 && v.L1_ApproverStatus == 1 && v.L2_ApproverStatus == 0) {
            if (v.ManagerName2 != '0') {
                $("#UserStatus").html("Approved by " + v.ManagerName1 + " Pending at " + v.ManagerName2).css("color", "Green");

            }
            else {
                $("#UserStatus").html("Approved by " + v.ManagerName1).css("color", "Green");

            }
        }

        //else if(v.Submitted_Type == 1 && v.L1_ApproverStatus == 1 && v.L2_ApproverStatus == 0 && v.)
        else if (v.Submitted_Type == 1 && v.L1_ApproverStatus == 0 && v.L2_ApproverStatus == 0) {

            $("#UserStatus").html("Waiting for both Managers").css("color", "deepskyblue");

        }
        else if (v.Submitted_Type == 0 && v.L1_ApproverStatus == 1 && v.L2_ApproverStatus == 0) {
            $("#UserStatus").html("Approved By " + v.ManagerName1 + " Pending at " + v.ManagerName2).css("color", "#6e3400");
        }
        else if (v.Submitted_Type == 0 && v.L1_ApproverStatus == 0 && v.L2_ApproverStatus == 2) {
            $("#UserStatus").html("Reject/Revoke By " + v.ManagerName2).css("color", "Red");
        }
        else {

            $("#UserStatus").html("Reject/Revoke").css("color", "Red");
        }
        $("#txtDescription").val(v.Comments);
        $('#ts_ManagerNamesid1').html(v.ManagerName1);
        $("#Timesheet_Duration").html(v.TotalMonthName);
        if (v.ManagerName2 != '0') {
            $('#ts_ManagerNamesid2').html(v.ManagerName2);
            $("b.ts_ManagerNamesid2").show();
            $("#ts_Manager2").show();
            //$('#ts_ManagerNamesid2').closest("b.ts_ManagerNamesid2").show();
        }
        else {
            $('#ts_ManagerNamesid2').html("");
            $("b.ts_ManagerNamesid2").hide();
            $("#ts_Manager2").hide();
        }
        $("#ts_Approveid").hide();
        $("#ts_Submittedid").hide();
        $("#ts_approid").hide();
        $("#ts_submited").hide();

        if (k <= 15) {
            tbltask = 'uc1_ddlTask' + k;
            if (counter15 == -1) {

                $("#MainTable").append("<td id='td1'>" +
                    "<table id='SubTable' width='100%' class='evAdminTable1'><tr style='background-color:black;color:white'><th width='160' text-align='center' class='evAdminTable'>Date </th> <th class='evAdminTable'>Task </th> <th width='50' class='evAdminTable'>Hours </th></tr>");

            }

            //$("#SubTable").append("<tr><td>" + v.MonthYearName + "</td><td><select id='uc1_ddlTask" + k + "' class='form-control lookup' selected  ></select></td><td><input class='form-control uc1txtHours' name='hrs' type='number' title='Enter hours' maxlength='2'  min='0' max='8' id='uc1_txtHours" + k + "' value=" + v.NoofHoursWorked + "  style='width: 50px;'></td><td >&nbsp</td></td>");
            $("#SubTable").append("<tr><td>" + v.MonthYearName + "</td><td><select id='uc1_ddlTask" + k + "' class='form-control lookup' selected  ></select></td><td><div class='flex'><input class='form-control uc1txtHours' name='hrs' type='number' title='Enter hours' min='0' max='24' maxlength='2' id='uc1_txtHours" + k + "' value=" + v.NoofHoursWorked + "  style='width: 56px;'><span id='HoursColours" + k + "'  title=" + v.colour + " style='font-size: 25px;line-height: 25px;color:" + v.colour + "'>* </span></div></td><td >&nbsp</td></td>");
            counter15 = counter15 + 1;


        }
        else {

            tbltask = 'uc1_ddlTask' + k;
            if (counter == -1) {
                $("#SubTable").append("</table>")
                $("#SubTable").append("</td>")
                $("#MainTable").append("<td id='td2'><table id='SubTable2' /*style='position:relative;top:19px;'*/ width='100%' class='evAdminTable1'><tr style='background-color:black;color:white'><th width='160' text-align='center' class='evAdminTable'>Date </th> <th class='evAdminTable'>Task </th> <th width='50'class='evAdminTable'>Hours</th></tr>");
            }

            $("#SubTable2").append("<tr><td>" + v.MonthYearName + "</td><td><select id='uc1_ddlTask" + k + "' class='form-control lookup' selected  ></select></td><td><div class='flex'><input class='form-control uc1txtHours' name='hrs' type='number' title='Enter hours' min='0' max='24' maxlength='2' id='uc1_txtHours" + k + "' value=" + v.NoofHoursWorked + "  style='width: 56px;'><span id='HoursColours" + k + "'  title=" + v.colour + " style='font-size: 25px;line-height: 25px;color:" + v.colour + "'>* </span></div></td><td >&nbsp</td></td>");
            // $('#uc1_ddlTask' + k).append('<option value="' + v.Taskid + '">' + v.Taskname + '</option>');
            counter = counter + 1;

        }
        if (RoleStar == "MANAGER") {

            $('table [id^="SubTable"] tr > td:last-child > span[id^="HoursColours"]').hide();
        }
    });
    HoursDataColoursPreview();
    calculateSum();
    $(":input").bind('keyup mouseup', function () {
        calculateSum();

    });
    if (resultDataArrayUploadimages.length == 0) {
        $("#images,#previewFlImage,#imagestwo,#previewFlImage1").hide();
    }
    else if (resultDataArrayUploadimages.length == 1) {
        var url;
        url = "/TimeSheetimages/" + resultDataArrayUploadimages[0].Uploadedimages;
        if (url !== null && url !== "") {
            $("#images").show();
            $('#previewFlImage').prop('src', url);
            $("#imagestwo,#previewFlImage1").hide();
        } else {
            $("#images,#previewFlImage").hide();
        }

    }
    else {

        $.each(resultDataArrayUploadimages, function (k, v) {
            
            var url;
            if (k == 0) {
                url = "/TimeSheetimages/" + v.Uploadedimages;
                if (url !== null && url !== "") {
                    $("#images").show();
                    $('#previewFlImage').prop('src', url);
                } else {
                    $("#images,#previewFlImage").hide();
                }
            }
            else {
                url = "/TimeSheetimages/" + v.Uploadedimages;
                if (url !== null && url !== "") {
                    $("#imagestwo").show();
                    $('#previewFlImage1').prop('src', url);
                } else {
                    $("#imagestwo,#previewFlImage1").hide();
                }
            }

        });
    }

    "</table>" + + "<hr />"
    $("#MainTable").append("</tr></table>") + "<hr />";

}



function PreviewSubmitTimesheet(id, mode, pro_id) {
    
    $("#body_ClientDetails").attr("style", "display: table;");
    $("#btnPrint").show();
    $("#btnpdf").show();
    $("#btnShare").show();
    $("#tabCon").empty();
    $("#MainTable").empty();
    $("#SubTable").empty();
    TimeSheetMode = mode//id.getAttribute("data-TimesheetMode");
    if (TimeSheetMode == "1") {
        $("#Timesheet_dvTSMode").html("Monthly");
    }
    else if (TimeSheetMode == "2") {
        $("#Timesheet_dvTSMode").html("Bi-Weekly");
    }
    else if (TimeSheetMode == "3") {
        $("#Timesheet_dvTSMode").html("Weekly");
    }
    else if (TimeSheetMode == "4") {
        $("#Timesheet_dvTSMode").html("Bi-Monthly");
    }

    //LoadClientDetails();
    //$("#ClientDetails2").show(true);
    if (TimeSheetMode == "1") {
        TimesheetID = id.getAttribute("data-TimesheetID");


        TSID = TimesheetID;
        MonthTimesheet = id.getAttribute("data-MonthYear");
        AppRejectstatus = id.getAttribute("data-TimesheetApprovalStatus");
        cid = id.getAttribute("data-ClientprojectId");
        empprojectname = id.getAttribute("data-ProjectName");
        empname = id.getAttribute("data-userName");
        empactualhrs = id.getAttribute("data-CompanyBillingHours");
        empworkedhrs = id.getAttribute("data-ResourceWorkingHours");
        attchemntid = id.getAttribute("data-attachId");
        uploadedImage = id.getAttribute("data-UpImage");
        $("#divEditTimesheetData .btn-app").attr("style", "display: none;");
        $("#divPreviewTimesheet").attr("style", "display: none;");
        $("#divEditTimesheetData").attr("style", "display: block;");
        tsuserid = id.getAttribute("data-Usr_UserID");
        $('#loading-image').attr("style", "display: block;");
        $.ajax({
            url: "/Timesheet/ViewSubmitedTimesheet",
            type: "GET",
            data: { TimesheetMonth: MonthTimesheet, TimesheetUserid: tsuserid, clientProjectid: cid, TimesheetID: TimesheetID },
            contentType: 'application/json; charset=utf-8',
            dataType: 'JSON',
            async: false,
            // cache: false,

            success: function (resultData) {
                
                resultDataArray = resultData.timeSheetDetails;
                resultDataArrayUploadimages = resultData.UploadTimesheetimage;
                LoadColoursonLeavesandHolidays(MonthTimesheet, tsuserid);
                PreviewTimesheetsByMonth(MonthTimesheet, resultDataArray);
                LoadPreviewTasklookups();
            }
            , complete: function () {
                $('#loading-image').attr("style", "display: none;");
            }

        });
        LoadData();
    }
    else if (TimeSheetMode == "2") {

        $("#body_ClientDetails").attr("style", "display: table;");
        $("#Timesheet_dvTSMode").html("Bi-Weekly");
        $("#btnPrint").show();
        $("#btnpdf").show();
        $("#btnShare").show();
        $("#tabCon").empty();
        $("#MainTable").empty();
        $("#SubTable").empty();
        tsuserid = id.getAttribute("data-Usr_UserID");
        TimesheetID = id.getAttribute("data-TimesheetID");
        TSID = TimesheetID;
        AppRejectstatus = id.getAttribute("data-TimesheetApprovalStatus");

        cid = id.getAttribute("data-ClientprojectId");

        $.ajax({
            url: "/Timesheet/GetByMonthlyPreviewTimesheets",
            type: "GET",
            data: {
                mode:TimeSheetMode,

            },
            contentType: 'application/json; charset=utf-8',
            dataType: 'JSON',
            // cache: false,
            success: function (resultData) {
                console.log(resultData);
                let dat = resultData.mytimesheets.filter(function (e) {
                    return (e.TimesheetID == TSID);

                });
                console.log(dat);
                MonthTimesheet = dat[0].Month_Year;//id.getAttribute("data-MonthYear");
                ByMonthlydates = dat[0].ByMonthlyDates;
                var Timesheetmonarray4 = new Array();
                Timesheetmonarray4 = ByMonthlydates.split("-");
                TimesheetMonth = Timesheetmonarray4[1];
                ByMonthlystartdate = Timesheetmonarray4[0]; ByMonthlyenddate = Timesheetmonarray4[1];
                $("#divEditTimesheetData .btn-app").attr("style", "display: none;");
                $("#divPreviewTimesheet").attr("style", "display: none;");
                $("#divEditTimesheetData").attr("style", "display: block;");
                $('#loading-image').attr("style", "display: block;");
                $.ajax({
                    url: "/Timesheet/ViewByWeeklySubmitedTimesheet",
                    type: "GET",
                    data: {
                        TimesheetUserid: tsuserid, Timesheetstartdate: Timesheetmonarray4[0],
                        TimesheetEnddate: Timesheetmonarray4[1], Accountid: SessionAccountid,
                        Projectid: pro_id, ClientProjectId: cid

                    },
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'JSON',
                    async: false,
                    // cache: false,

                    success: function (resultData) {
                        resultDataArray = resultData.timeSheetDetails;
                        resultDataArrayUploadimages = resultData.UploadTimesheetimage;
                        PreviewByMonthlyLoadHoursWorkedAcctoDate(tsuserid, Timesheetmonarray4[0], Timesheetmonarray4[1], SessionAccountid, SessionProjectid);
                        PreviewTimesheetsByWeekly(Timesheetmonarray4[1], resultDataArray);
                        LoadPreviewTasklookups();
                    },
                    complete: function () {
                        $('#loading-image').attr("style", "display: none;");
                    }

                });
                LoadData();

            },
            complete: function () {
                $('#loading-image').attr("style", "display: none;");
            },


        });






    }
    else if (TimeSheetMode == "3") {

        $("#body_ClientDetails").attr("style", "display: table;");
        $("#btnPrint").show();
        $("#btnpdf").show();
        $("#btnShare").show();
        $("#tabCon").empty();
        $("#MainTable").empty();
        $("#SubTable").empty();
        $("#Timesheet_dvTSMode").html("Weekly");
        tsuserid = id.getAttribute("data-Usr_UserID");
        TimesheetID = id.getAttribute("data-TimesheetID");
        TSID = TimesheetID;
        cid = id.getAttribute("data-ClientprojectId");
        //var pro_id = id.getAttribute("data-ProjectId");
        $.ajax({
            url: "/Timesheet/GetWeeklyPreviewTimesheets",
            type: "GET",
            contentType: 'application/json; charset=utf-8',
            dataType: 'JSON',
            success: function (resultData) {
                
                let dat = resultData.mytimesheets.filter(function (e) {
                    return (e.TimesheetID == TSID);

                });
                MonthTimesheet = dat[0].Month_Year;//id.getAttribute("data-MonthYear");
                ByMonthlydates = dat[0].ByMonthlyDates;

                ByMonthlydates = dat[0].ByMonthlyDates;
                AppRejectstatus = id.getAttribute("data-TimesheetApprovalStatus");


                var Timesheetmonarray4 = new Array();
                Timesheetmonarray4 = ByMonthlydates.split("-");
                TimesheetMonth = Timesheetmonarray4[1];
                ByMonthlystartdate = Timesheetmonarray4[0]; ByMonthlyenddate = Timesheetmonarray4[1];
                $("#divEditTimesheetData .btn-app").attr("style", "display: none;");
                $("#divPreviewTimesheet").attr("style", "display: none;");
                $("#divEditTimesheetData").attr("style", "display: block;");
                $('#loading-image').attr("style", "display: block;");
                $.ajax({
                    url: "/Timesheet/ViewByWeeklySubmitedTimesheet",
                    type: "GET",
                    data: {
                        TimesheetUserid: tsuserid, Timesheetstartdate: Timesheetmonarray4[0],
                        TimesheetEnddate: Timesheetmonarray4[1], Accountid: SessionAccountid,
                        Projectid: pro_id,
                        
                        //Projectid: SessionProjectid,
                        ClientProjectId: cid
                    },
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'JSON',
                    async: false,
                    // cache: false,

                    success: function (resultData) {
                        
                        resultDataArray = resultData.timeSheetDetails;
                        resultDataArrayUploadimages = resultData.UploadTimesheetimage;
                        PreviewByMonthlyLoadHoursWorkedAcctoDate(tsuserid, Timesheetmonarray4[0], Timesheetmonarray4[1], SessionAccountid, SessionProjectid);
                        PreviewTimesheetsWeekly(Timesheetmonarray4[1], resultDataArray);
                        LoadPreviewTasklookups();




                    },
                    complete: function () {
                        $('#loading-image').attr("style", "display: none;");
                    }

                });
                LoadData();
            },

            error: function (data) {
                //  alert(data.responseText);
            }
        });






    }
    else if (TimeSheetMode == "4") {
        
        $("#body_ClientDetails").attr("style", "display: table;");
        $("#Timesheet_dvTSMode").html("Bi-Monthly");
        $("#btnPrint").show();
        $("#btnpdf").show();
        $("#btnShare").show();
        $("#tabCon").empty();
        $("#MainTable").empty();
        $("#SubTable").empty();

        tsuserid = id.getAttribute("data-Usr_UserID");
        TimesheetID = id.getAttribute("data-TimesheetID");
        TSID = TimesheetID;
        cid = id.getAttribute("data-ClientprojectId");
        //pro_id = id.getAttribute("data-ProjectId");

        $.ajax({
            url: "/Timesheet/GetByMonthlyPreviewTimesheets15day",
            type: "GET",
            contentType: 'application/json; charset=utf-8',
            dataType: 'JSON',
            // cache: false,
            success: function (resultData) {

                console.log(resultData);
                let dat = resultData.mytimesheets.filter(function (e) {
                    return (e.TimesheetID == TimesheetID);

                });

                MonthTimesheet = dat[0].Month_Year;//id.getAttribute("data-MonthYear");
                ByMonthlydates = dat[0].ByMonthlyDates; //id.getAttribute("data-ByMonthlyDates");

                AppRejectstatus = id.getAttribute("data-TimesheetApprovalStatus");

                var Timesheetmonarray45 = new Array();
                Timesheetmonarray45 = ByMonthlydates.split("-");
                TimesheetMonth = Timesheetmonarray45[1];
                ByMonthlystartdate = Timesheetmonarray45[0]; ByMonthlyenddate = Timesheetmonarray45[1];
                $("#divEditTimesheetData .btn-app").attr("style", "display: none;");
                $("#divPreviewTimesheet").attr("style", "display: none;");
                $("#divEditTimesheetData").attr("style", "display: block;");
                $('#loading-image').attr("style", "display: block;");
                $.ajax({
                    //url: "/Timesheet/GetByMonthlyPreviewTimesheets15day",

                    url: "/Timesheet/ViewByWeeklySubmitedTimesheet",
                    type: "GET",
                    data: {
                        
                        TimesheetUserid: tsuserid, Timesheetstartdate: Timesheetmonarray45[0],
                        TimesheetEnddate: Timesheetmonarray45[1],
                        Accountid: SessionAccountid,
                        Projectid: pro_id,
                        ClientProjectId: cid
                    },
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'JSON',
                    async: false,
                    // cache: false,

                    success: function (resultData) {
                        
                        resultDataArray = resultData.timeSheetDetails;
                        resultDataArrayUploadimages = resultData.UploadTimesheetimage;
                        PreviewByMonthlyLoadHoursWorkedAcctoDate(tsuserid, Timesheetmonarray45[0], Timesheetmonarray45[1], SessionAccountid, SessionProjectid);
                        PreviewTimesheetByWeekly(Timesheetmonarray45[1], resultDataArray);
                        LoadPreviewTasklookups();
                    },
                    complete: function () {
                        $('#loading-image').attr("style", "display: none;");
                    }

                });
                LoadData();
            },

            error: function (data) {
                // alert(data.responseText);
            }
        });







    }


}

function PreviewTimesheetsByMonth(MonthTimesheet, resultData, ) {
    
    $("#images").show();
    $("#tabCon").empty();
    $("#MainTable").empty();
    $("#SubTable").empty();
    var tbltask = ''; var objSelect = '';
    $('#Cmtsave').show();
    $("#ClientDetails2").show(true);
    var counter = -1;
    var counter15 = -1;

    $("#tabCon").append("<table name='maintbl' id='MainTable' width='100%' /*style='position:relative;left:10%;'*/>");


    $.each(resultData, function (k, v) {
        
        console.log(v);
        $("#txtDescription").html(v.Comments);
        $("#Timesheet_Duration").html(v.TotalMonthName);
        if (v.ApprovedDate != '') {
            $("#ts_Approveid").html(v.ApprovedDate);
            $("#ts_approid").show();
        }
        else {
            $("#ts_Approveid").hide(v.ApprovedDate);
            $("#ts_approid").hide();
        }

        $("#ts_Submittedid").html(v.SubmittedDate);
        $("#ts_submited").show();
        $('#ts_ManagerNamesid1').html(v.ManagerName1);
        if (v.ManagerName2 != '0') {
            $('#ts_ManagerNamesid2').html(v.ManagerName2);
            $("b.ts_ManagerNamesid2").show();
            $("#ts_Manager2").show();
        }
        else {
            $("#ts_ManagerNamesid2").hide();
            $('#ts_ManagerNamesid2').html("");
            $("#ts_Manager2").hide();
        }

        $("#StardateToEnddate").html("<span id='StartToEnd'><b>Timesheet of " + v.UserName + " for the Period of " + v.TotalMonthName + "</b></span>");
        
        if (v.Submitted_Type == 1 && v.L1_ApproverStatus == 1 && v.L2_ApproverStatus == 1) {
            $("#UserStatus").html("Approved by both Managers").css("color", "Green");
        }
        else if (v.Submitted_Type == 1 && v.L1_ApproverStatus == 1 && v.L2_ApproverStatus == 0) {
            if (v.ManagerName2 != '0') {
                $("#UserStatus").html("Approved by " + v.ManagerName1 + " Pending at " + v.ManagerName2).css("color", "Green");

            }
            else {
                $("#UserStatus").html("Approved by " + v.ManagerName1).css("color", "Green");

            }
        }
        else if (v.Submitted_Type == 1 && v.L1_ApproverStatus == 0 && v.L2_ApproverStatus == 0) {

            $("#UserStatus").html("Waiting for both Managers").css("color", "deepskyblue");

        }
        else if (v.Submitted_Type == 0 && v.L1_ApproverStatus == 1 && v.L2_ApproverStatus == 0) {
            $("#UserStatus").html("Approved By " + v.ManagerName1 + " Pending at " + v.ManagerName2).css("color", "#6e3400");
        }
        else if (v.Submitted_Type == 0 && v.L1_ApproverStatus == 0 && v.L2_ApproverStatus == 2) {
            $("#UserStatus").html("Reject/Revoke By " + v.ManagerName2).css("color", "Red");
        }
        else {

            $("#UserStatus").html("Reject/Revoke").css("color", "Red");
        }
        if (k <= 15) {
            tbltask = 'uc1_ddlTask' + k;
            if (counter15 == -1) {

                $("#MainTable").append("<td id='td1'>" +
                    "<table id='SubTable' width='100%' class='evAdminTable1'><tr style='background-color:black;color:white'><th width='160' text-align='center' class='evAdminTable'>Date </th> <th class='evAdminTable'>Task </th> <th width='50' class='evAdminTable'>Hours </th></tr>");

            }
            $("#SubTable").append("<tr><td class='colored'>" + v.MonthYearName + "</td><td class='colored' id='uc1_ddlTask" + k + "'>" + v.Taskname + "</td><div class='flex'><td class='colored' name='uc1txtHours'  id='uc1_txtHours" + k + "'><font>" + v.NoofHoursWorked + "</font><span id='HoursColours" + k + "'  title=" + v.colour + " style='font-size: 25px;line-height: 25px;color:" + v.colour + "'>* </span></div></td></td>");
            //   $("#SubTable").append("<tr><td>" + v.MonthYearName + "</td><td><select id='uc1_ddlTask" + k + "' class='form-control lookup' selected  ></select></td><td><div class='flex'><input class='form-control uc1txtHours' name='hrs' type='number' title='Enter hours' min='0' max='24' maxlength='2' id='uc1_txtHours" + k + "' readonly value=" + v.NoofHoursWorked + "  style='width: 56px;'><span id='HoursColours" + k + "' style='font-size: 25px;line-height: 25px;color:" + v.colour + "'>* </span></div></td><td >&nbsp</td></td>");


            counter15 = counter15 + 1;

        }
        else {
            tbltask = 'uc1_ddlTask' + k;
            if (counter == -1) {
                $("#SubTable").append("</table>")
                $("#SubTable").append("</td>")
                $("#MainTable").append("<td id='td2'><table id='SubTable2' /*style='position:relative;top:19px;'*/ width='100%' class='evAdminTable1'><tr style='background-color:black;color:white'><th width='160' text-align='center' class='evAdminTable'>Date </th> <th class='evAdminTable'>Task </th> <th width='50'class='evAdminTable'>Hours</th></tr>");
            }
            $("#SubTable2").append("<tr><td class='colored'>" + v.MonthYearName + "</td><td class='colored' id='uc1_ddlTask" + k + "'>" + v.Taskname + "</td><div class='flex'><td class='colored' name='uc1txtHours'  id='uc1_txtHours" + k + "'><font>" + v.NoofHoursWorked + "</font><span id='HoursColours" + k + "' title=" + v.colour + " style='font-size: 25px;line-height: 25px;color:" + v.colour + "'>* </span></div></td></td>");
            // $("#SubTable2").append("<tr><td>" + v.MonthYearName + "</td><td><select id='uc1_ddlTask" + k + "' class='form-control lookup' selected  ></select></td><td><div class='flex'><input class='form-control uc1txtHours' name='hrs' type='number' title='Enter hours' min='0' max='24' maxlength='2' id='uc1_txtHours" + k + "' readonly value=" + v.NoofHoursWorked + "  style='width: 56px;'><span id='HoursColours" + k + "' style='font-size: 25px;line-height: 25px;color:" + v.colour + "'>* </span></div></td><td >&nbsp</td></td>");
            counter = counter + 1;

        }
        if (RoleStar == "MANAGER") {
            $('table [id^="SubTable"] tr > td:last-child > span[id^="HoursColours"]').hide();
        }
        //$("#MainTable").append(v.Uploadedimages);



    });


    HoursDataColoursPreview();
    CalculatePreview();
    "</table>" + + "<hr />";
    $("#MainTable").append("</tr></table>") + "<hr />";
    $('table[id="SubTable"] tr > td:last-child').css("display", "table-cell");
    $('table[id="SubTable2"] tr > td:last-child').css("display", "table-cell");

    if (resultDataArrayUploadimages.length == 0) {
        $("#images,#previewFlImage,#imagestwo,#previewFlImage1").hide();
    }
    else if (resultDataArrayUploadimages.length == 1) {
        var url;
        url = "/TimeSheetimages/" + resultDataArrayUploadimages[0].Uploadedimages;
        if (url !== null && url !== "") {
            $("#images").show();
            $('#previewFlImage').prop('src', url);
            $("#imagestwo,#previewFlImage1").hide();
        } else {
            $("#images,#previewFlImage").hide();
        }

    }
    else {

        $.each(resultDataArrayUploadimages, function (k, v) {
            
            var url;
            if (k == 0) {
                url = "/TimeSheetimages/" + v.Uploadedimages;
                if (url !== null && url !== "") {
                    $("#images").show();
                    $('#previewFlImage').prop('src', url);
                } else {
                    $("#images,#previewFlImage").hide();
                }
            }
            else {
                url = "/TimeSheetimages/" + v.Uploadedimages;
                if (url !== null && url !== "") {
                    $("#imagestwo").show();
                    $('#previewFlImage1').prop('src', url);
                } else {
                    $("#imagestwo,#previewFlImage1").hide();
                }
            }

        });
    }



   

}

var TSID;
function PreviewManagerSubmitTimesheet(id, mode) {
    
    $("#body_ClientDetails").attr("style", "display: table;");
    $("#btnPrint").show();
    $("#btnpdf").show();
    $("#btnShare").show();
    $("#tabCon").empty();
    $("#MainTable").empty();
    $("#SubTable").empty();
    //$("#Timesheet_dvTSMode").html("Monthly");
    //LoadClientDetails();
    //$("#ClientDetails2").show(true);

    TimeSheetMode = mode//id.getAttribute("data-TimesheetMode");
    if (TimeSheetMode == "1") {
        $("#Timesheet_dvTSMode").html("Monthly");
    }
    else if (TimeSheetMode == "2") {
        $("#Timesheet_dvTSMode").html("Bi-Weekly");
    }
    else if (TimeSheetMode == "3") {
        $("#Timesheet_dvTSMode").html("Weekly");
    }
    else if (TimeSheetMode == "4") {
        $("#Timesheet_dvTSMode").html("Bi-Monthly");
    }
    TimesheetID = id.getAttribute("data-TimesheetID");
    TSID = TimesheetID;
    MonthTimesheet = id.getAttribute("data-MonthYear");
    AppRejectstatus = id.getAttribute("data-TimesheetApprovalStatus");
    cid = id.getAttribute("data-ClientprojectId");
    empprojectname = id.getAttribute("data-ProjectName");
    empname = id.getAttribute("data-userName");
    empactualhrs = id.getAttribute("data-CompanyBillingHours");
    empworkedhrs = id.getAttribute("data-ResourceWorkingHours");

    $("#divEditTimesheetData .btn-app").attr("style", "display: none;");
    $("#divPreviewTimesheet").attr("style", "display: none;");
    $("#divEditTimesheetData").attr("style", "display: block;");
    tsuserid = id.getAttribute("data-Usr_UserID");
    ApporRejprojectid = id.getAttribute("data-ProjectId");
    var myvariable = $('#UserGridData a').attr('id');
    var myManagerGridData = $('#ManagerGridData a').attr('id');
    var L1Managerid = id.getAttribute("data-UProj_L1ManagerId");
    var L2Managerid = id.getAttribute("data-UProj_L2ManagerId");

    if (myManagerGridData) {
        if (L1Managerid) {
            if ((AppRejectstatus == "Approved By Level_1 Manager") || (AppRejectstatus == "Approved By Level_2 Manager")
                || (AppRejectstatus == "Approved By Level_1 Manager and Pending at Level_2 Manager") || (AppRejectstatus == "Pending at Level_1 Manager and Approved by Level_2 Manager")
                || (AppRejectstatus == "Approved by Both Managers")) {

                $("#btnApprove").attr("style", "display:none;");
                $("#btnReject").attr("style", "display:none;");

            }

            else {
                $("#btnApprove").attr("style", "display: inline-block;");
                $("#btnReject").attr("style", "display: inline-block;");
            }
        }

        if (L2Managerid) {

            if ((AppRejectstatus == "Approved By Level_1 Manager") || (AppRejectstatus == "Approved By Level_2 Manager")
                //|| (AppRejectstatus == "Approved By Level_1 Manager and Pending at Level_2 Manager") || (AppRejectstatus == "Pending at Level_1 Manager and Approved by Level_2 Manager")
                || (AppRejectstatus == "Approved by Both Managers")) {

                $("#btnApprove").attr("style", "display:none;");
                $("#btnReject").attr("style", "display:none;");

            }

            else {
                $("#btnApprove").attr("style", "display: inline-block;");
                $("#btnReject").attr("style", "display: inline-block;");
            }
        }

    }
    $('#loading-image').attr("style", "display: block;");
    $.ajax({
        url: "/Timesheet/ViewSubmitedTimesheet",
        type: "GET",
        data: { TimesheetMonth: MonthTimesheet, TimesheetUserid: tsuserid, clientProjectid: cid, TimesheetID: TimesheetID },
        contentType: 'application/json; charset=utf-8',
        dataType: 'JSON',
        async: false,
        success: function (resultData) {
            resultDataArray = resultData.timeSheetDetails;
            resultDataArrayUploadimages = resultData.UploadTimesheetimage;
            LoadColoursonLeavesandHolidays(MonthTimesheet, tsuserid);
            PreviewTimesheetsByMonth(MonthTimesheet, resultDataArray);

            LoadTasklookupsByUser(tsuserid);
        },
        complete: function () {
            $('#loading-image').attr("style", "display: none;");
        }


    });
    LoadManagerData();

}

function ByWeeklyPreviewSubmitTimesheet(id) {

    $("#body_ClientDetails").attr("style", "display: table;");
    $("#Timesheet_dvTSMode").html("ByWeekly");
    $("#btnPrint").show();
    $("#btnpdf").show();
    $("#btnShare").show();
    $("#tabCon").empty();
    $("#MainTable").empty();
    $("#SubTable").empty();
    tsuserid = id.getAttribute("data-Usr_UserID");
    TimesheetID = id.getAttribute("data-TimesheetID");
    TSID = TimesheetID;
    MonthTimesheet = id.getAttribute("data-MonthYear");
    AppRejectstatus = id.getAttribute("data-TimesheetApprovalStatus");
    ByMonthlydates = id.getAttribute("data-ByMonthlyDates");
    cid = id.getAttribute("data-ClientprojectId");
    var Timesheetmonarray4 = new Array();
    Timesheetmonarray4 = ByMonthlydates.split("-");
    TimesheetMonth = Timesheetmonarray4[1];
    ByMonthlystartdate = Timesheetmonarray4[0]; ByMonthlyenddate = Timesheetmonarray4[1];
    $("#divEditTimesheetData .btn-app").attr("style", "display: none;");
    $("#divPreviewTimesheet").attr("style", "display: none;");
    $("#divEditTimesheetData").attr("style", "display: block;");
    var myManagerGridData = $('#ManagerGridData a').attr('id');
    var L1Managerid = id.getAttribute("data-UProj_L1ManagerId");
    var L2Managerid = id.getAttribute("data-UProj_L2ManagerId");
    if (myManagerGridData) {
        if (L1Managerid) {
            if ((AppRejectstatus == "Approved By Level_1 Manager") || (AppRejectstatus == "Approved By Level_2 Manager")
                || (AppRejectstatus == "Approved By Level_1 Manager and Pending at Level_2 Manager") || (AppRejectstatus == "Pending at Level_1 Manager and Approved by Level_2 Manager")
                || (AppRejectstatus == "Approved by Both Managers")) {

                $("#btnApprove").attr("style", "display:none;");
                $("#btnReject").attr("style", "display:none;");

            }

            else {
                $("#btnApprove").attr("style", "display: inline-block;");
                $("#btnReject").attr("style", "display: inline-block;");
            }
        }

        if (L2Managerid) {
            if ((AppRejectstatus == "Approved By Level_1 Manager") || (AppRejectstatus == "Approved By Level_2 Manager")
                //|| (AppRejectstatus == "Approved By Level_1 Manager and Pending at Level_2 Manager") || (AppRejectstatus == "Pending at Level_1 Manager and Approved by Level_2 Manager")
                || (AppRejectstatus == "Approved by Both Managers")) {

                $("#btnApprove").attr("style", "display:none;");
                $("#btnReject").attr("style", "display:none;");

            }

            else {
                $("#btnApprove").attr("style", "display: inline-block;");
                $("#btnReject").attr("style", "display: inline-block;");
            }
        }

    }
    $('#loading-image').attr("style", "display: block;");
    $.ajax({
        url: "/Timesheet/ViewByWeeklySubmitedTimesheet",
        type: "GET",
        data: { TimesheetUserid: tsuserid, Timesheetstartdate: Timesheetmonarray4[0], TimesheetEnddate: Timesheetmonarray4[1], Accountid: SessionAccountid, Projectid: SessionProjectid },
        contentType: 'application/json; charset=utf-8',
        dataType: 'JSON',
        async: false,
        // cache: false,

        success: function (resultData) {
            resultDataArray = resultData.timeSheetDetails;
            resultDataArrayUploadimages = resultData.UploadTimesheetimage;
            PreviewByMonthlyLoadHoursWorkedAcctoDate(tsuserid, Timesheetmonarray4[0], Timesheetmonarray4[1], SessionAccountid, SessionProjectid);
            PreviewTimesheetsByWeekly(Timesheetmonarray4[1], resultDataArray);
            LoadPreviewTasklookups();
        },
        complete: function () {
            $('#loading-image').attr("style", "display: none;");
        }

    });
    LoadManagerData();
}


function ViewTimesheetByMonthWeekly(MonthTimesheet, resultData) {
    
    $("#body_ClientDetails").attr("style", "display: table;");
    $("#Timesheet_dvTSMode").html("Weekly");
    $("#btnPrint").hide();
    $("#btnpdf").hide();
    $("#btnShare").hide();
    $("#tabCon").empty();
    $("#MainTable").empty();
    $("#SubTable").empty();
    // $("#flImage").show();
    $("#imagelbl, #imagelbl1").show();
    $("#images, #imagestwo").show();
    var tbltask = ''; var objSelect = '';
    $('#Cmtsave').show();
    var counter = -1;
    var counter15 = -1;
    $("#ClientDetails2").show(true);
    $("#tabCon").append("<table name='maintbl' id='MainTable' width='100%'/*style='position:relative;left:10%;'*/>");
    $('#Timesheet_Duration').html(ByMonthlydates);
    $.each(resultData, function (k, v) {

        $("#StardateToEnddate").html("<span id='StartToEnd'><b>Timesheet of " + v.UserName + " for the Period of " + ByMonthlydates + "</b></span>");
        $("#txtDescription").val(v.Comments);
        $('#ts_ManagerNamesid1').html(v.ManagerName1);

        if (v.ManagerName2 != '0') {
            $('#ts_ManagerNamesid2').html(v.ManagerName2);
            $("b.ts_ManagerNamesid2").show();
            $("#ts_Manager2").show();
            //$('#ts_ManagerNamesid2').closest("b.ts_ManagerNamesid2").show();
        }
        else {
            $('#ts_ManagerNamesid2').html("");
            $("b.ts_ManagerNamesid2").hide();
            $("#ts_Manager2").hide();
        }
        $("#ts_Approveid").hide();
        $("#ts_Submittedid").hide();
        $("#ts_approid").hide();
        $("#ts_submited").hide();

        if (k <= 6) {
            tbltask = 'uc1_ddlTask' + k;
            if (counter15 == -1) {

                $("#MainTable").append("<td id='td1'>" +
                    "<table id='SubTable' width='100%' class='evAdminTable1'><tr style='background-color:black;color:white'><th width='160' text-align='center' class='evAdminTable'>Date </th> <th class='evAdminTable'>Task </th> <th width='50' class='evAdminTable'>Hours </th></tr>");

            }

            //$("#SubTable").append("<tr><td>" + v.MonthYearName + "</td><td><select id='uc1_ddlTask" + k + "' class='form-control lookup' selected  ></select></td><td><input class='form-control uc1txtHours' name='hrs' type='number' title='Enter hours' maxlength='2'  min='0' max='8' id='uc1_txtHours" + k + "' value=" + v.NoofHoursWorked + "  style='width: 50px;'></td><td >&nbsp</td></td>");
            $("#SubTable").append("<tr><td>" + v.MonthYearName + "</td><td><select id='uc1_ddlTask" + k + "' class='form-control lookup' selected  ></select></td><td><div class='flex'><input class='form-control uc1txtHours' name='hrs' type='number' title='Enter hours' min='0' max='24' maxlength='2' id='uc1_txtHours" + k + "' value=" + v.NoofHoursWorked + "  style='width: 56px;'><span id='HoursColours" + k + "' title=" + v.colour + " style='font-size: 25px;line-height: 25px;color:" + v.colour + "'>* </span></div></td><td >&nbsp</td></td>");
            counter15 = counter15 + 1;


        }
        else {

            tbltask = 'uc1_ddlTask' + k;
            if (counter == -1) {
                $("#SubTable").append("</table>")
                $("#SubTable").append("</td>")
                $("#MainTable").append("<td id='td2'><table id='SubTable2' /*style='position:relative;top:19px;'*/ width='100%' class='evAdminTable1'><tr style='background-color:black;color:white'><th width='160' text-align='center' class='evAdminTable'>Date </th> <th class='evAdminTable'>Task </th> <th width='50'class='evAdminTable'>Hours</th></tr>");
            }

            $("#SubTable2").append("<tr><td>" + v.MonthYearName + "</td><td><select id='uc1_ddlTask" + k + "' class='form-control lookup' selected  ></select></td><td><div class='flex'><input class='form-control uc1txtHours' name='hrs' type='number' title='Enter hours' min='0' max='24' maxlength='2' id='uc1_txtHours" + k + "' value=" + v.NoofHoursWorked + "  style='width: 56px;'><span id='HoursColours" + k + "' title=" + v.colour + " style='font-size: 25px;line-height: 25px;color:" + v.colour + "'>* </span></div></td><td >&nbsp</td></td>");
            // $('#uc1_ddlTask' + k).append('<option value="' + v.Taskid + '">' + v.Taskname + '</option>');
            counter = counter + 1;

        }
        if (RoleStar == "MANAGER") {
            $('table [id^="SubTable"] tr > td:last-child > span[id^="HoursColours"]').hide();
        }
    });
    PreviewByMonthlyHoursDataColours();
    calculateSum();
    $(":input").bind('keyup mouseup', function () {
        calculateSum();

    });
    
    if (resultDataArrayUploadimages.length == 0) {
        $("#images,#previewFlImage,#imagestwo,#previewFlImage1").hide();
    }
    else if (resultDataArrayUploadimages.length == 1) {
        var url;
        url = "/TimeSheetimages/" + resultDataArrayUploadimages[0].Uploadedimages;
        if (url !== null && url !== "") {
            $("#images").show();
            $('#previewFlImage').prop('src', url);
            $("#imagestwo,#previewFlImage1").hide();
        } else {
            $("#images,#previewFlImage").hide();
        }

    }
    else {

        $.each(resultDataArrayUploadimages, function (k, v) {
            
            var url;
            if (k == 0) {
                url = "/TimeSheetimages/" + v.Uploadedimages;
                if (url !== null && url !== "") {
                    $("#images").show();
                    $('#previewFlImage').prop('src', url);
                } else {
                    $("#images,#previewFlImage").hide();
                }
            }
            else {
                url = "/TimeSheetimages/" + v.Uploadedimages;
                if (url !== null && url !== "") {
                    $("#imagestwo").show();
                    $('#previewFlImage1').prop('src', url);
                } else {
                    $("#imagestwo,#previewFlImage1").hide();
                }
            }

        });
    }



    "</table>" + + "<hr />"
    $("#MainTable").append("</tr></table>") + "<hr />";

}
//var loadFile = function (event) {
//    var output = document.getElementById('imgLogo');

//    output.src = URL.createObjectURL(event.target.files[0]);
//    $('#imgLogo').show();
//};

function PreviewByMonthlyHoursDataColours() {
    $.each(resultHoursWorkedColourArray, function (index, value) {
        //if (value.colour == 'black') {
        //    value.colour = 'white';
        //}
        $('#HoursColours' + index).attr('style', 'font-size:25px;line-height:25px;color:' + value.colour);
        $('#HoursColours' + index).attr('title', value.colour);
    });

}

function calculateSum() {

    var sum = 0;
    $(".uc1txtHours").each(function () {
        //add only if the value is number
        if (!isNaN(this.value) && this.value.length != 0) {
            sum += parseFloat(this.value);
        }
    });
    //.toFixed() method will roundoff the final sum to 2 decimal places
    $("#TotalHoursCount").html(sum.toFixed(2));
}

function LoadPreviewTasklookups() {

    $.ajax({
        type: "GET",
        url: "/Admin/getLookUp",
        datatype: "Json",
        async: false,
        //data: { id: id },
        success: function (data) {

            $(data).each(function () {

                $(".lookup").append($("<option></option>").val(this.tsk_TaskID).html(this.tsk_TaskName));

            });


        }
    });
}
function PreviewTimesheetsWeekly(MonthTimesheet, resultData) {
    
    $("#tabCon").empty();
    $("#MainTable").empty();
    $("#SubTable").empty();
    var tbltask = ''; var objSelect = '';
    $('#Cmtsave').show();
    $("#ClientDetails2").show(true);
    $("#Timesheet_dvTSMode").html("Weekly");
    var counter = -1;
    var counter15 = -1;

    $("#tabCon").append("<table name='maintbl' id='MainTable' width='100%' /*style='position:relative;left:10%;'*/>");
    $('#Timesheet_Duration').html(ByMonthlydates);

    $.each(resultData, function (k, v) {
        
        $("#txtDescription").html(v.Comments);

        if (v.ApprovedDate != '') {
            $("#ts_Approveid").html(v.ApprovedDate);
            $("#ts_approid").show();
        }
        else {
            $("#ts_Approveid").hide(v.ApprovedDate);
            $("#ts_approid").hide();
        }

        $("#ts_Submittedid").html(v.SubmittedDate);
        $("#ts_submited").show();
        $('#ts_ManagerNamesid1').html(v.ManagerName1);
        if (v.ManagerName2 != '0') {
            $('#ts_ManagerNamesid2').html(v.ManagerName2);
            $("b.ts_ManagerNamesid2").show();
            $("#ts_Manager2").show();
        }
        else {
            $("#ts_ManagerNamesid2").hide();
            $('#ts_ManagerNamesid2').html("");
            $("#ts_Manager2").hide();
        }

        $("#StardateToEnddate").html("<span id='StartToEnd'><b>Timesheet of " + v.UserName + " for the Period of " + ByMonthlydates + "</b></span>");
        if (k <= 6) {
            tbltask = 'uc1_ddlTask' + k;
            if (counter15 == -1) {

                $("#MainTable").append("<td id='td1'>" +
                    "<table id='SubTable' width='100%' class='evAdminTable1'><tr style='background-color:black;color:white'><th width='160' text-align='center' class='evAdminTable'>Date </th> <th class='evAdminTable'>Task </th> <th width='50' class='evAdminTable'>Hours </th></tr>");

            }
            $("#SubTable").append("<tr><td class='colored'>" + v.MonthYearName + "</td><td class='colored' id='uc1_ddlTask" + k + "'>" + v.Taskname + "</td><div class='flex'><td class='colored' name='uc1txtHours'  id='uc1_txtHours" + k + "'><font>" + v.NoofHoursWorked + "</font><span id='HoursColours" + k + "' title=" + v.colour + " style='font-size: 25px;line-height: 25px;color:" + v.colour + "'>* </span></div></td></td>");
            //   $("#SubTable").append("<tr><td>" + v.MonthYearName + "</td><td><select id='uc1_ddlTask" + k + "' class='form-control lookup' selected  ></select></td><td><div class='flex'><input class='form-control uc1txtHours' name='hrs' type='number' title='Enter hours' min='0' max='24' maxlength='2' id='uc1_txtHours" + k + "' readonly value=" + v.NoofHoursWorked + "  style='width: 56px;'><span id='HoursColours" + k + "' style='font-size: 25px;line-height: 25px;color:" + v.colour + "'>* </span></div></td><td >&nbsp</td></td>");


            counter15 = counter15 + 1;

        }
        else {
            tbltask = 'uc1_ddlTask' + k;
            if (counter == -1) {
                $("#SubTable").append("</table>")
                $("#SubTable").append("</td>")
                $("#MainTable").append("<td id='td2'><table id='SubTable2' /*style='position:relative;top:19px;'*/ width='100%' class='evAdminTable1'><tr style='background-color:black;color:white'><th width='160' text-align='center' class='evAdminTable'>Date </th> <th class='evAdminTable'>Task </th> <th width='50'class='evAdminTable'>Hours</th></tr>");
            }
            $("#SubTable2").append("<tr><td class='colored'>" + v.MonthYearName + "</td><td class='colored' id='uc1_ddlTask" + k + "'>" + v.Taskname + "</td><div class='flex'><td class='colored' name='uc1txtHours'  id='uc1_txtHours" + k + "'><font>" + v.NoofHoursWorked + "</font><span id='HoursColours" + k + "' title=" + v.colour + " style='font-size: 25px;line-height: 25px;color:" + v.colour + "'>* </span></div></td></td>");
            // $("#SubTable2").append("<tr><td>" + v.MonthYearName + "</td><td><select id='uc1_ddlTask" + k + "' class='form-control lookup' selected  ></select></td><td><div class='flex'><input class='form-control uc1txtHours' name='hrs' type='number' title='Enter hours' min='0' max='24' maxlength='2' id='uc1_txtHours" + k + "' readonly value=" + v.NoofHoursWorked + "  style='width: 56px;'><span id='HoursColours" + k + "' style='font-size: 25px;line-height: 25px;color:" + v.colour + "'>* </span></div></td><td >&nbsp</td></td>");
            counter = counter + 1;

        }
        if (RoleStar == "MANAGER") {
            $('table [id^="SubTable"] tr > td:last-child > span[id^="HoursColours"]').hide();
        }
    });

    PreviewByMonthlyHoursDataColours();
    CalculatePreview();

    "</table>" + + "<hr />"
    $("#MainTable").append("</tr></table>") + "<hr />";
    $('table[id="SubTable"] tr > td:last-child').css("display", "table-cell");
    $('table[id="SubTable2"] tr > td:last-child').css("display", "table-cell");

    
    if (resultDataArrayUploadimages.length == 0) {
        $("#images,#previewFlImage,#imagestwo,#previewFlImage1").hide();
    }
    else {

        $.each(resultDataArrayUploadimages, function (k, v) {
            
            var url;
            if (k == 0) {
                url = "/TimeSheetimages/" + v.Uploadedimages;
                if (url !== null && url !== "") {
                    $("#images").show();
                    $('#previewFlImage').prop('src', url);
                } else {
                    $("#images,#previewFlImage").hide();
                }
            }
            else {
                url = "/TimeSheetimages/" + v.Uploadedimages;
                if (url !== null && url !== "") {
                    $("#imagestwo").show();
                    $('#previewFlImage1').prop('src', url);
                } else {
                    $("#imagestwo,#previewFlImage1").hide();
                }
            }

        });
    }


}

function LoadTasklookupsByUser(Userid) {

    $.ajax({
        type: "Post",
        url: "/Admin/GetLookUpByEmpId",
        datatype: "Json",
        async: false,
        data: { "Userid": Userid },
        success: function (data) {
            $(data).each(function () {
                $(".lookup").append($("<option></option>").val(this.tsk_TaskID).html(this.tsk_TaskName));
            });
        }
    });
}


//function daysInMonth(month, year) {
//    return new Date(year, month, 0).getDate();
//}


function openNav() {
    document.getElementById("mySidenav").style.width = "250px";
}

function closeNav() {
    document.getElementById("mySidenav").style.width = "0";
}

/////////////////////////////////////////////////////EditsubmitData////////////////////////////////////////////////////////



function ManagerApprOrRej(id) {

    TimesheetID = id.getAttribute("data-TimesheetID");

    MonthTimesheet = id.getAttribute("data-MonthYear");
    tsuserid = id.getAttribute("data-Usr_UserID");
    ApporRejprojectid = id.getAttribute("data-ProjectId");
    cid = id.getAttribute("data-ClientprojectId");
    empprojectname = id.getAttribute("data-ProjectName");
    empname = id.getAttribute("data-userName");
    empactualhrs = id.getAttribute("data-CompanyBillingHours");
    empworkedhrs = id.getAttribute("data-ResourceWorkingHours");
    TimesheetPeriod = id.getAttribute("data-TotalMonthName");
    var ApproveRejectstatus = id.getAttribute("data-Status");
    id = ApproveRejectstatus;
    TimesheetPeriodname = TimesheetPeriod.replace('-', ' to ');

    if (id == 3) {
        $("#btnAdd").show();
        $("#btnRej").hide();
        $('#PopupAppRejId').html("Approve Timesheet");
        $("#ProjectNameid").html("Do you want to approve <b> " + empname + "'s </b>timesheets within <b>" + empworkedhrs + "</b> hours for duration <b>" + TimesheetPeriodname + "</b> ");
    }
    if (id == 4) {
        $("#btnRej").show();
        $("#btnAdd").hide();
        $('#PopupAppRejId').html("Reject Timesheet");
        $("#ProjectNameid").html("Do you want to reject <b> " + empname + "'s </b>timesheets within <b>" + empworkedhrs + "</b> hours for duration <b>" + TimesheetPeriodname + "</b> ");
    }
    $('#ContainerGridDetail').show();


}

function ManagerApprOrRejFromWeb(id) {



    TimesheetPeriod = $("#Timesheet_Duration").html();


    if (id == '3' || id == '4') {

        id = id;
        if (id == 3) {
            $("#btnAdd").show();
            $("#btnRej").hide();
            $('#PopupAppRejId').html("Approve Timesheet");
            // $("#ProjectNameid").html("Do you want to approve <b> " + empname + "'s </b>  <b>" + MonthTimesheet + "</b> timesheets within <b>" + empworkedhrs + "</b> hours for <b>" + empprojectname + "</b>");
            $("#ProjectNameid").html("Do you want to approve <b> " + empname + "'s </b>timesheets within <b>" + empworkedhrs + "</b> hours for duration <b>" + TimesheetPeriod.replace('-', ' to ') + "</b> ");


        }
        if (id == 4) {
            $("#btnRej").show();
            $("#btnAdd").hide();
            $('#PopupAppRejId').html("Reject Timesheet");
            $("#ProjectNameid").html("Do you want to reject <b> " + empname + "'s </b>timesheets within <b>" + empworkedhrs + "</b> hours for duration <b>" + TimesheetPeriod.replace('-', ' to ') + "</b> ");
        }
        $('#ContainerGridDetail').show();
    }

}


function ApprovalRejectTimesheet(id) {


    if (id == '3' || id == '4') {

        id = id;
        Commentss = $.trim($("#txtDescription").val());

    }
    else {
        TimesheetID = id.getAttribute("data-TimesheetID");
        MonthTimesheet = id.getAttribute("data-MonthYear");
        tsuserid = id.getAttribute("data-Usr_UserID");
        Commentss = $.trim($("#txtDescription").val());
        var ApproveRejectstatus = id.getAttribute("data-Status");
        id = ApproveRejectstatus;

    }

    var projectid = ApporRejprojectid;
    var actualhrs = empactualhrs;

    var timesheets =

    {
        TimesheetID: TimesheetID,
        SubmittedType: id,
        TimeSheetMonth: MonthTimesheet,
        Comments: Commentss,
        UserID: tsuserid,
        ProjectID: projectid

    }
    var sheetObj = {
        timesheets: timesheets,
    };




    $('#loading-image').attr("style", "display: block;");
    $.ajax({
        type: "POST",
        url: "/Timesheet/TimeSheetManagerActionWeb",
        data: JSON.stringify(sheetObj),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {

            //   sendmailsbyapp(sheetObj);
            alert(data);
            window.location.reload();
        },
        complete: function () {
            $('#loading-image').attr("style", "display: none;");
        }
    });

}

function sendmailsbyapp(sheetObj) {
    $.ajax({
        type: "POST",
        url: "/Timesheet/SendMailsForApprovals",
        data: JSON.stringify(sheetObj),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            //alert(data);
        }
    });
}

function LoadPreviewTaskData() {

    if (resultDataArray && resultDataArray.length > 0) {
        $.each(resultDataArray, function (index, value) {

            $('#uc1_ddlTask' + index).val(value.Taskid).attr("selected", "selected");
            $('#Timesheet_Duration').html(value.TotalMonthName);
            $('#ts_ClientNameid').html(value.ProjectName);
            $('#ts_ProjectNameid').html(value.ProjectClientName);
        });
    }
}

function LoadData() {

    if (resultDataArray && resultDataArray.length > 0) {
        $.each(resultDataArray, function (index, value) {

            //$('#uc1_ddlTask' + index).val(value.Taskid).attr("selected", "selected");//
            //$('#uc1_ddlTask' + index).val(value.Taskid).attr("disabled", "true");//
            $('#uc1_ddlTask' + index).val(value.Taskid).attr("selected", "true");
            $('#uc1_txtHours' + index).val(value.NoofHoursWorked).attr("selected", "true");
            $('#txtDescription').val(value.Comments).attr("disabled", "true");
            //$('#Timesheet_ProjectName').html(value.ProjectName);
            $('#Timesheet_Duration').html(value.TotalMonthName);
            $('#Timesheet_EmployeeName').html(value.UserName);
            $('#ts_ClientNameid').html(value.ProjectName);
            $('#ts_ProjectNameid').html(value.ProjectClientName);
            $('#btnSave').hide();
            $('#btnSend').hide();

        });
    }
}
function LoadManagerData() {

    if (resultDataArray && resultDataArray.length > 0) {
        $.each(resultDataArray, function (index, value) {

            $('#uc1_ddlTask' + index).val(value.Taskid).attr("selected", "selected");
            $('#uc1_ddlTask' + index).val(value.Taskid).attr("disabled", "true");
            // $('#txtDescription').val(value.Comments).attr("disabled", "true");
            $('#btnSave').hide();
            $('#btnSend').hide();
            //   $('#Timesheet_ProjectName').html(value.ProjectName);
            $('#ts_ClientNameid').html(value.ProjectName);
            $('#ts_ProjectNameid').html(value.ProjectClientName);
            $('#Timesheet_Duration').html(value.TotalMonthName);
            $('#Timesheet_EmployeeName').html(value.UserName);

        });
    }
}

function LoadClientDetails() {
    $.ajax({
        type: "GET",
        url: "/Timesheet/Usertimesheet",

        datatype: "Json",

        success: function (result) {


        }
    });
}

function AccSpecificTasksLoad() {

    $.ajax({
        type: "GET",
        url: "/ClientComponent/Getaccountspecifictasks",
        //contentType: "application/json; charset=utf-8",
        //dataType: "json",
        async: false,
        //data: { id: id },
        success: function (data) {

            //$(data).each(function () {

            //    $(".lookup").append($("<option></option>").val(this.Acc_SpecificTaskId).html(this.Acc_SpecificTaskName));

            //});


        }
    });
}

function LoadColoursonLeavesandHolidays(MonthTimesheet, tsuserid) {
    $.ajax({
        url: "/Timesheet/HoursWorkedTimesheet",
        type: "GET",
        data: { TimesheetMonth: MonthTimesheet, TimesheetUserid: tsuserid },
        contentType: 'application/json; charset=utf-8',
        dataType: 'JSON',
        async: false,
        // cache: false,

        success: function (resultData) {
            resultHoursWorkedColour = resultData

        },

    });
}

function HoursDataColoursPreview() {
    $.each(resultHoursWorkedColour, function (index, value) {
        //if (value.colour == 'black') {
        //    value.colour = 'white';
        //}
        $('#HoursColours' + index).attr('style', 'font-size:25px;line-height:25px;color:' + value.colour);
        $('#HoursColours' + index).attr('title', value.colour);
    });

}

function print_page() {
   
    
    /*var be4print = function () {
        $("#divEditTimesheetData").addClass("pdfHide");
    };
    var beAfterprint = function () {
        $("#divEditTimesheetData").removeClass("pdfHide");
    };

    if (window.matchMedia) {
        var mediaQueryList = window.matchMedia('print');

        mediaQueryList.addListener(function (mql) {

            if (mql.matches) {
                be4print();
            } else {
                beAfterprint();
            }
        });
    }

    window.onbeforeprint = be4print;
    window.onafterprint = beAfterprint;*/

    $("#divEditTimesheetData").addClass("pdfHide");
    if (window.print()) {
        console.log("print");
    } else {
        $("#divEditTimesheetData").removeClass("pdfHide");
    }
}

$("body").unbind().on("click", "#btnpdf", function () {

    //$("#images").hide();
    html2canvas($('#divEditTimesheetData')[0], {
        onrendered: function (canvas) {
            var data = canvas.toDataURL();
            var docDefinition = {
                content: [{
                    image: data,
                    width: 500
                }]
            };
            pdfMake.createPdf(docDefinition).download("Timesheet.pdf");
        }
    });
    //setTimeout(function () {

    //    $("#images").show();

    //}, 1000);

});


function CalculatePreview() {

    var sum = 0;
    $("td.colored[name='uc1txtHours'] > font").each(function () {

        //sum += Number($(this).val()) || 0;
        sum += parseInt($(this).text());


    });

    $("#TotalHoursCount").html(sum);
}


function ShareTimesheet() {

    var tomail = $("#emailto").val();
    var Subj = $("#subject").val();
    var sectomail = $("#emailto2").val();
    var formData = new FormData();

    formData.append("mail", tomail);
    formData.append("mail", sectomail);
    formData.append("Subject", Subj);
    formData.append("TimeSheetId", TimesheetID);
    formData.append("ClientProjId", cid);

    $.ajax({
        type: "POST",
        url: "/Timesheet/ShareTimesheets",

        data: formData,
        contentType: false,
        processData: false,
        success: function (data, status) {
            window.location.reload();
            alert("TimeSheet Shared Sucessfully");
        },
        error: function (xhr, data, status) {

        }

    });
}


function ApprovalRejectTimesheetWeekly(id) {


    if (id == '3' || id == '4') {

        id = id;
        Commentss = $.trim($("#txtDescription").val());
    }
    else {
        TimesheetID = id.getAttribute("data-TimesheetID");
        MonthTimesheet = id.getAttribute("data-MonthYear");

        tsuserid = id.getAttribute("data-Usr_UserID");
        Commentss = $.trim($("#txtDescription").val());
        var ApproveRejectstatus = id.getAttribute("data-Status");
        id = ApproveRejectstatus;
        ByMonthlydates = id.getAttribute("data-ByMonthlyDates");
        var Timesheetmonarray8 = new Array();
        Timesheetmonarray8 = ByMonthlydates.split("-");
        TimesheetMonth = Timesheetmonarray8[0];

    }

    var projectid = ApporRejprojectid;
    var timesheets =

    {
        TimesheetID: TimesheetID,
        SubmittedType: id,
        TimeSheetMonth: TimesheetMonth,
        ByWeeklyStartDate: ByMonthlystartdate,
        ByWeeklyEndDate: ByMonthlyenddate,
        Comments: Commentss,
        UserID: tsuserid,
        ProjectID: projectid,
        TimesheetMode: '3',



    }
    var sheetObj = {
        timesheets: timesheets,


    };
    $('#loading-image').attr("style", "display: block;");
    $.ajax({
        type: "POST",
        url: "/Timesheet/TimeSheetManagerActionWeb",
        data: JSON.stringify(sheetObj),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            // sendmailsbyapp(sheetObj);
            alert(data);
            window.location.reload();
        },
        complete: function () {
            $('#loading-image').attr("style", "display: none;");
        }
    });

}
function ApprovalRejectTimesheetByWeekly(id) {


    if (id == '3' || id == '4') {

        id = id;
        Commentss = $.trim($("#txtDescription").val());
    }
    else {
        TimesheetID = id.getAttribute("data-TimesheetID");
        MonthTimesheet = id.getAttribute("data-MonthYear");
        tsuserid = id.getAttribute("data-Usr_UserID");
        Commentss = $.trim($("#txtDescription").val());
        var ApproveRejectstatus = id.getAttribute("data-Status");
        id = ApproveRejectstatus;
        ByMonthlydates = id.getAttribute("data-ByMonthlyDates");
        var Timesheetmonarray8 = new Array();
        Timesheetmonarray8 = ByMonthlydates.split("-");
        TimesheetMonth = Timesheetmonarray8[0];

    }

    var projectid = ApporRejprojectid;
    var timesheets =

    {
        TimesheetID: TimesheetID,
        SubmittedType: id,
        TimeSheetMonth: TimesheetMonth,
        ByWeeklyStartDate: ByMonthlystartdate,
        ByWeeklyEndDate: ByMonthlyenddate,
        Comments: Commentss,
        UserID: tsuserid,
        ProjectID: projectid,
        TimesheetMode: '2',



    }
    var sheetObj = {
        timesheets: timesheets,


    };
    $('#loading-image').attr("style", "display: block;");
    $.ajax({
        type: "POST",
        url: "/Timesheet/TimeSheetManagerActionWeb",
        data: JSON.stringify(sheetObj),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            // sendmailsbyapp(sheetObj);
            alert(data);
            window.location.reload();
        },
        complete: function () {
            $('#loading-image').attr("style", "display: none;");
        }
    });

    

}

function ByMonthlyPreviewManagerSubmitTimesheet(id, prj_id) {
    
    $("#body_ClientDetails").attr("style", "display: table;");
    $("#Timesheet_dvTSMode").html("Bi-Monthly");
    $("#btnPrint").show();
    $("#btnpdf").show();
    $("#btnShare").show();
    $("#tabCon").empty();
    $("#MainTable").empty();
    $("#SubTable").empty();
    TimesheetID = id.getAttribute("data-TimesheetID");
    TSID = TimesheetID;
    MonthTimesheet = id.getAttribute("data-MonthYear");
    AppRejectstatus = id.getAttribute("data-TimesheetApprovalStatus");
    ByMonthlydates = id.getAttribute("data-ByMonthlyDates");
    empprojectname = id.getAttribute("data-ProjectName");
    empname = id.getAttribute("data-userName");
    empactualhrs = id.getAttribute("data-CompanyBillingHours");
    empworkedhrs = id.getAttribute("data-ResourceWorkingHours");
    cid = id.getAttribute("data-ClientprojectId");
    var Timesheetmonarray5 = new Array();
    Timesheetmonarray5 = ByMonthlydates.split("-");
    $("#divEditTimesheetData .btn-app").attr("style", "display: none;");
    $("#divPreviewTimesheet").attr("style", "display: none;");
    $("#divEditTimesheetData").attr("style", "display: block;");
    tsuserid = id.getAttribute("data-Usr_UserID");
    ApporRejprojectid = id.getAttribute("data-ProjectId");
    var myvariable = $('#UserGridData a').attr('id');
    var myManagerGridData = $('#ManagerGridData a').attr('id');
    var L1Managerid = id.getAttribute("data-UProj_L1ManagerId");
    var L2Managerid = id.getAttribute("data-UProj_L2ManagerId");

    TimesheetMonth = Timesheetmonarray5[1];
    ByMonthlystartdate = Timesheetmonarray5[0]; ByMonthlyenddate = Timesheetmonarray5[1];

    if (myManagerGridData) {
        if (L1Managerid) {
            if ((AppRejectstatus == "Approved By Level_1 Manager") || (AppRejectstatus == "Approved By Level_2 Manager")
                || (AppRejectstatus == "Approved By Level_1 Manager and Pending at Level_2 Manager") || (AppRejectstatus == "Pending at Level_1 Manager and Approved by Level_2 Manager")
                || (AppRejectstatus == "Approved by Both Managers")) {

                $("#btnApprove").attr("style", "display:none;");
                $("#btnReject").attr("style", "display:none;");

            }

            else {
                $("#btnApprove").attr("style", "display: inline-block;");
                $("#btnReject").attr("style", "display: inline-block;");
            }
        }

        if (L2Managerid) {
            if ((AppRejectstatus == "Approved By Level_1 Manager") || (AppRejectstatus == "Approved By Level_2 Manager")
                //|| (AppRejectstatus == "Approved By Level_1 Manager and Pending at Level_2 Manager") || (AppRejectstatus == "Pending at Level_1 Manager and Approved by Level_2 Manager")
                || (AppRejectstatus == "Approved by Both Managers")) {

                $("#btnApprove").attr("style", "display:none;");
                $("#btnReject").attr("style", "display:none;");

            }

            else {
                $("#btnApprove").attr("style", "display: inline-block;");
                $("#btnReject").attr("style", "display: inline-block;");
            }
        }

    }
    $('#loading-image').attr("style", "display: block;");
    $.ajax({
        url: "/Timesheet/ViewByWeeklySubmitedTimesheet",
        type: "GET",
        data: {
            TimesheetUserid: tsuserid, Timesheetstartdate: Timesheetmonarray5[0],
            TimesheetEnddate: Timesheetmonarray5[1], Accountid: SessionAccountid,
            Projectid: prj_id, ClientProjectId: cid
        },
        contentType: 'application/json; charset=utf-8',
        dataType: 'JSON',
        async: false,
        success: function (resultData) {
            
            resultDataArray = resultData.timeSheetDetails;
            resultDataArrayUploadimages = resultData.UploadTimesheetimage;
            PreviewByMonthlyLoadHoursWorkedAcctoDate(tsuserid, Timesheetmonarray5[0], Timesheetmonarray5[1], SessionAccountid, prj_id);
            PreviewTimesheetsByWeekly(Timesheetmonarray5[1], resultDataArray);
            LoadTasklookupsByUser(tsuserid);
        },
        complete: function () {
            $('#loading-image').attr("style", "display: none;");
        }


    });
    LoadManagerData();

    
}

function ViewTimesheetsByWeekly(MonthTimesheet, resultData) {

    
    $("#body_ClientDetails").attr("style", "display: table;");
    $("#Timesheet_dvTSMode").html("Bi-Monthly");
    $("#btnPrint").hide();
    $("#btnpdf").hide();
    $("#btnShare").hide();
    $("#tabCon").empty();
    $("#MainTable").empty();
    $("#SubTable").empty();
    // $("#flImage").show();
    $("#imagelbl").show();
    $("#images").show();
    // $("#flImage1").show();
    $("#imagelbl1").show();
    $("#images1").show();
    var tbltask = ''; var objSelect = '';
    $('#Cmtsave').show();
    var counter = -1;
    var counter15 = -1;
    $("#ClientDetails2").show(true);
    $("#tabCon").append("<table name='maintbl' id='MainTable' width='100%'/*style='position:relative;left:10%;'*/>");
    $('#Timesheet_Duration').html(ByMonthlydates);
    $.each(resultData, function (k, v) {

        $("#StardateToEnddate").html("<span id='StartToEnd'><b>Timesheet of " + v.UserName + " for the Period of " + ByMonthlydates + "</b></span>");
        $("#txtDescription").val(v.Comments);
        if (v.ApprovedDate != '') {
            $("#ts_Approveid").html(v.ApprovedDate);
            $("#ts_approid").show();
        }
        else {
            $("#ts_Approveid").hide(v.ApprovedDate);
            $("#ts_approid").hide();
        }
        $('#ts_ManagerNamesid1').html(v.ManagerName1);

        if (v.ManagerName2 != '0') {
            $('#ts_ManagerNamesid2').html(v.ManagerName2);
            $("b.ts_ManagerNamesid2").show();
            $("#ts_Manager2").show();
            //$('#ts_ManagerNamesid2').closest("b.ts_ManagerNamesid2").show();
        }
        else {
            $('#ts_ManagerNamesid2').html("");
            $("b.ts_ManagerNamesid2").hide();
            $("#ts_Manager2").hide();
        }
        $("#ts_Approveid").hide();
        $("#ts_Submittedid").hide();
        $("#ts_approid").hide();
        $("#ts_submited").hide();

        if (k <= 6) {
            tbltask = 'uc1_ddlTask' + k;
            if (counter15 == -1) {

                $("#MainTable").append("<td id='td1'>" +
                    "<table id='SubTable' width='100%' class='evAdminTable1'><tr style='background-color:black;color:white'><th width='160' text-align='center' class='evAdminTable'>Date </th> <th class='evAdminTable'>Task </th> <th width='50' class='evAdminTable'>Hours </th></tr>");

            }

            //$("#SubTable").append("<tr><td>" + v.MonthYearName + "</td><td><select id='uc1_ddlTask" + k + "' class='form-control lookup' selected  ></select></td><td><input class='form-control uc1txtHours' name='hrs' type='number' title='Enter hours' maxlength='2'  min='0' max='8' id='uc1_txtHours" + k + "' value=" + v.NoofHoursWorked + "  style='width: 50px;'></td><td >&nbsp</td></td>");
            $("#SubTable").append("<tr><td>" + v.MonthYearName + "</td><td><select id='uc1_ddlTask" + k + "' class='form-control lookup' selected  ></select></td><td><div class='flex'><input class='form-control uc1txtHours' name='hrs' type='number' title='Enter hours' min='0' max='24' maxlength='2' id='uc1_txtHours" + k + "' value=" + v.NoofHoursWorked + "  style='width: 56px;'><span id='HoursColours" + k + "' title=" + v.colour + " style='font-size: 25px;line-height: 25px;color:" + v.colour + "'>* </span></div></td><td >&nbsp</td></td>");
            counter15 = counter15 + 1;


        }
        else {

            tbltask = 'uc1_ddlTask' + k;
            if (counter == -1) {
                $("#SubTable").append("</table>")
                $("#SubTable").append("</td>")
                $("#MainTable").append("<td id='td2'><table id='SubTable2' /*style='position:relative;top:19px;'*/ width='100%' class='evAdminTable1'><tr style='background-color:black;color:white'><th width='160' text-align='center' class='evAdminTable'>Date </th> <th class='evAdminTable'>Task </th> <th width='50'class='evAdminTable'>Hours</th></tr>");
            }

            $("#SubTable2").append("<tr><td>" + v.MonthYearName + "</td><td><select id='uc1_ddlTask" + k + "' class='form-control lookup' selected  ></select></td><td><div class='flex'><input class='form-control uc1txtHours' name='hrs' type='number' title='Enter hours' min='0' max='24' maxlength='2' id='uc1_txtHours" + k + "' value=" + v.NoofHoursWorked + "  style='width: 56px;'><span id='HoursColours" + k + "' title=" + v.colour + " style='font-size: 25px;line-height: 25px;color:" + v.colour + "'>* </span></div></td><td >&nbsp</td></td>");
            // $('#uc1_ddlTask' + k).append('<option value="' + v.Taskid + '">' + v.Taskname + '</option>');
            counter = counter + 1;

        }
        if (RoleStar == "MANAGER") {

            $('table [id^="SubTable"] tr > td:last-child > span[id^="HoursColours"]').hide();
        }
    });
    PreviewByMonthlyHoursDataColours();
    calculateSum();
    $(":input").bind('keyup mouseup', function () {
        calculateSum();

    });
    if (resultDataArrayUploadimages.length == 0) {
        $("#images,#previewFlImage,#imagestwo,#previewFlImage1").hide();
    }
    else if (resultDataArrayUploadimages.length == 1) {
        var url;
        url = "/TimeSheetimages/" + resultDataArrayUploadimages[0].Uploadedimages;
        if (url !== null && url !== "") {
            $("#images").show();
            $('#previewFlImage').prop('src', url);
            $("#imagestwo,#previewFlImage1").hide();
        } else {
            $("#images,#previewFlImage").hide();
        }

    }
    else {

        $.each(resultDataArrayUploadimages, function (k, v) {
            
            var url;
            if (k == 0) {
                url = "/TimeSheetimages/" + v.Uploadedimages;
                if (url !== null && url !== "") {
                    $("#images").show();
                    $('#previewFlImage').prop('src', url);
                } else {
                    $("#images,#previewFlImage").hide();
                }
            }
            else {
                url = "/TimeSheetimages/" + v.Uploadedimages;
                if (url !== null && url !== "") {
                    $("#imagestwo").show();
                    $('#previewFlImage1').prop('src', url);
                } else {
                    $("#imagestwo,#previewFlImage1").hide();
                }
            }

        });
    }
    // }


    "</table>" + + "<hr />"
    $("#MainTable").append("</tr></table>") + "<hr />";

}

function PreviewTimesheetByWeekly(MonthTimesheet, resultData) {
    
    $("#tabCon").empty();
    $("#MainTable").empty();
    $("#SubTable").empty();
    var tbltask = ''; var objSelect = '';
    $('#Cmtsave').show();
    $("#ClientDetails2").show(true);
    var counter = -1;
    var counter15 = -1;

    $("#tabCon").append("<table name='maintbl' id='MainTable' width='100%' /*style='position:relative;left:10%;'*/>");
    $('#Timesheet_Duration').html(ByMonthlydates);

    
    $.each(resultData, function (k, v) {
        
        $("#txtDescription").html(v.Comments);

        if (v.ApprovedDate != '') {
            $("#ts_Approveid").html(v.ApprovedDate);
            $("#ts_approid").show();
        }
        else {
            $("#ts_Approveid").hide(v.ApprovedDate);
            $("#ts_approid").hide();
        }

        $("#ts_Submittedid").html(v.SubmittedDate);
        $("#ts_submited").show();
        $('#ts_ManagerNamesid1').html(v.ManagerName1);
        if (v.ManagerName2 != '0') {
            $('#ts_ManagerNamesid2').html(v.ManagerName2);
            $("b.ts_ManagerNamesid2").show();
            $("#ts_Manager2").show();
        }
        else {
            $("#ts_ManagerNamesid2").hide();
            $('#ts_ManagerNamesid2').html("");
            $("#ts_Manager2").hide();
        }


        //if (v.Submitted_Type == 0 && v.L1_ApproverStatus == 1 && v.L2_ApproverStatus == 0) {
        //    $("#TimesheetApprovalStatus").html("Approved By "  + v.ManagerName1 + " Pending at " + v.ManagerName2);
        //}
        //else if (v.Submitted_Type == 0 && v.L1_ApproverStatus == 0 && v.L2_ApproverStatus == 0) {
        //    $("#TimesheetApprovalStatus").html("Waiting for approval at both managers")
        //}
        //else if (v.Submitted_Type == 0) {
        //    $("#TimesheetApprovalStatus").html("Waithing for Approval");
        //}
        //else if (v.Submitted_Type == 1) {
        //    $("#TimesheetApprovalStatus").html("Approval by both managers");
        //}


        $("#StardateToEnddate").html("<span id='StartToEnd'><b>Timesheet of " + v.UserName + " for the Period of " + ByMonthlydates + "</b></span>");
        
        if (v.Submitted_Type == 1 && v.L1_ApproverStatus == 1 && v.L2_ApproverStatus == 1) {
            $("#UserStatus").html("Approved by both Managers").css("color", "Green");
        }

        else if (v.Submitted_Type == 1 && v.L1_ApproverStatus == 0 && v.L2_ApproverStatus == 0) {

            $("#UserStatus").html("Waiting for both Managers").css("color", "deepskyblue");

        }
        else if (v.Submitted_Type == 1 && v.L1_ApproverStatus == 1 && v.L2_ApproverStatus == 0) {
            $("#UserStatus").html("Approved By " + v.ManagerName1 + " Pending at " + v.ManagerName2).css("color", "rgb(204 102 11)");
        }
        else if (v.Submitted_Type == 0 && v.L1_ApproverStatus == 0 && v.L2_ApproverStatus == 2) {
            $("#UserStatus").html("Reject/Revoke By " + v.ManagerName2).css("color", "Red");
        }
        //else {

        //    $("#UserStatus").html("Reject/Revoke").css("color", "Red");
        //}

        if (k <= 6) {
            tbltask = 'uc1_ddlTask' + k;
            if (counter15 == -1) {

                $("#MainTable").append("<td id='td1'>" +
                    "<table id='SubTable' width='100%' class='evAdminTable1'><tr style='background-color:black;color:white'><th width='160' text-align='center' class='evAdminTable'>Date </th> <th class='evAdminTable'>Task </th> <th width='50' class='evAdminTable'>Hours </th></tr>");

            }
            $("#SubTable").append("<tr><td class='colored'>" + v.MonthYearName + "</td><td class='colored' id='uc1_ddlTask" + k + "'>" + v.Taskname + "</td><div class='flex'><td class='colored' name='uc1txtHours'  id='uc1_txtHours" + k + "'><font>" + v.NoofHoursWorked + "</font><span id='HoursColours" + k + "' title=" + v.colour + " style='display: none;font-size: 25px;line-height: 25px;color:" + v.colour + "'>* </span></div></td></td>");
            //   $("#SubTable").append("<tr><td>" + v.MonthYearName + "</td><td><select id='uc1_ddlTask" + k + "' class='form-control lookup' selected  ></select></td><td><div class='flex'><input class='form-control uc1txtHours' name='hrs' type='number' title='Enter hours' min='0' max='24' maxlength='2' id='uc1_txtHours" + k + "' readonly value=" + v.NoofHoursWorked + "  style='width: 56px;'><span id='HoursColours" + k + "' style='font-size: 25px;line-height: 25px;color:" + v.colour + "'>* </span></div></td><td >&nbsp</td></td>");


            counter15 = counter15 + 1;

        }
        else {
            tbltask = 'uc1_ddlTask' + k;
            if (counter == -1) {
                $("#SubTable").append("</table>")
                $("#SubTable").append("</td>")
                $("#MainTable").append("<td id='td2'><table id='SubTable2' /*style='position:relative;top:19px;'*/ width='100%' class='evAdminTable1'><tr style='background-color:black;color:white'><th width='160' text-align='center' class='evAdminTable'>Date </th> <th class='evAdminTable'>Task </th> <th width='50'class='evAdminTable'>Hours</th></tr>");
            }
            $("#SubTable2").append("<tr><td class='colored'>" + v.MonthYearName + "</td><td class='colored' id='uc1_ddlTask" + k + "'>" + v.Taskname + "</td><div class='flex'><td class='colored' name='uc1txtHours'  id='uc1_txtHours" + k + "'><font>" + v.NoofHoursWorked + "</font><span id='HoursColours" + k + "' title=" + v.colour + " style='display: none;font-size: 25px;line-height: 25px;color:" + v.colour + "'>* </span></div></td></td>");
            // $("#SubTable2").append("<tr><td>" + v.MonthYearName + "</td><td><select id='uc1_ddlTask" + k + "' class='form-control lookup' selected  ></select></td><td><div class='flex'><input class='form-control uc1txtHours' name='hrs' type='number' title='Enter hours' min='0' max='24' maxlength='2' id='uc1_txtHours" + k + "' readonly value=" + v.NoofHoursWorked + "  style='width: 56px;'><span id='HoursColours" + k + "' style='font-size: 25px;line-height: 25px;color:" + v.colour + "'>* </span></div></td><td >&nbsp</td></td>");
            counter = counter + 1;

        }

    });

    PreviewByMonthlyHoursDataColours();
    CalculatePreview();
    "</table>" + + "<hr />"
    $("#MainTable").append("</tr></table>") + "<hr />";
    $('table[id="SubTable"] tr > td:last-child').css("display", "table-cell");
    $('table[id="SubTable2"] tr > td:last-child').css("display", "table-cell");

    if (resultDataArrayUploadimages.length == 0) {
        $("#images,#previewFlImage,#imagestwo,#previewFlImage1").hide();
    }
    else if (resultDataArrayUploadimages.length == 1) {
        var url;
        url = "/TimeSheetimages/" + resultDataArrayUploadimages[0].Uploadedimages;
        if (url !== null && url !== "") {
            $("#images").show();
            $('#previewFlImage').prop('src', url);
            $("#imagestwo,#previewFlImage1").hide();
        } else {
            $("#images,#previewFlImage").hide();
        }

    }
    else {

        $.each(resultDataArrayUploadimages, function (k, v) {
            
            var url;
            if (k == 0) {
                url = "/TimeSheetimages/" + v.Uploadedimages;
                if (url !== null && url !== "") {
                    $("#images").show();
                    $('#previewFlImage').prop('src', url);
                } else {
                    $("#images,#previewFlImage").hide();
                }
            }
            else {
                url = "/TimeSheetimages/" + v.Uploadedimages;
                if (url !== null && url !== "") {
                    $("#imagestwo").show();
                    $('#previewFlImage1').prop('src', url);
                } else {
                    $("#imagestwo,#previewFlImage1").hide();
                }
            }

        });
    }
   

}