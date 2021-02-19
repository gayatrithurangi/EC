///// <reference path="jquery-1.10.2.js" />
// <reference path="bootstrap.min.js" />

var TimesheetID = '', fulldate = '';
var resultDataArray; var objUserSessionId = '';
var resultHoursWorkedColour; var ApporRejprojectid;
var SessionUsrid; var sessionProjectName, Sessionclientname, SessionAccountid,
    SessionProjectid, SessionTaskid; var resultHoursWorkedColourArray;
var empname = '', empworkedhrs = '', empprojectname = '', empactualhrs = '';
var ByMonthlystartdate = ''; var ByMonthlyenddate = '';
var colourcode = ''; var resultDataArrayUploadimages;
var TSID = '';
var cid = '';
//var RoleStar = $("p#PFname").next("p").text().toUpperCase();
$(document).ready(function () {

    SessionUsrid = Userid; sessionProjectName = Projectname; SessionProjectid = Projectid;
    Sessionclientname = ClientName, SessionAccountid = accountid; SessionTaskid = Taskid;

    
    $("#tabCon").empty();
    $("#body_ClientDetails").attr("style", "display: none;");
    $("#divEditTimesheetData").attr("style", "display: none;");
    $('#loading-image').attr("style", "display: block;");
    $.ajax({
        url: "/Timesheet/GetByMonthlyPreviewTimesheets",
        type: "GET",
        contentType: 'application/json; charset=utf-8',
        dataType: 'JSON',
        // cache: false,
        success: function (resultData) {
            console.log(resultData);
            if (resultData.roleid) {
                objUserSessionId = resultData.roleid;

                if ((objUserSessionId == '1001') || (objUserSessionId == '1002') || (objUserSessionId == '1007') || (objUserSessionId == "1010") || (objUserSessionId == "1011") || (objUserSessionId == "1053")) {

                    if ((objUserSessionId === '1007') || (objUserSessionId === "1010") || (objUserSessionId === "1011") || (objUserSessionId == "1053")) {
                        $("#UserGridDataByWeekly").attr("style", "display: table;");
                        // $("#ManagerGridData").attr("style", "display: table;");

                    }
                    else {

                        $("#UserGridDataByWeekly").attr("style", "display: none;");
                        // $("#ManagerGridData").attr("style", "display: table;");
                        $("#UserGridPanel").attr("style", "display: none;");

                    }
                    if (resultData.mytimesheets) {
                        
                        var objUsertimesheets2 = resultData.mytimesheets;
                        console.log(objUsertimesheets2);
                        $('#UserGridDataByWeekly').DataTable({
                            // 'destroy': true,
                            'data': objUsertimesheets2,
                            'paginate': true,
                            'sort': true,
                            'Processing': true,
                            'columns': [
                                //{ 'data': 'Usr_UserID', 'visible': false, },
                                //{ 'data': 'TimesheetID', 'visible': false, },
                                //{ 'data': 'ProjectId', 'visible': false, },
                                //{ 'data': 'TimesheetMonth', 'visible': false, },
                                //{ 'data': 'ProjectName' },
                                //{ 'data': 'ClientProjectName' },
                                //{ 'data': 'userName' },
                                //{ 'data': 'Month_Year' },
                                //{ 'data': 'CompanyBillingHours' },
                                //{ 'data': 'ResourceWorkingHours' },
                                //{ 'data': 'ByMonthlyDates' },
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
                                        return '<span class="badge badge-radius" data-toggle="tooltip" id="' + data.TimesheetApprovalStatus + '" title="' + data.ManagerApprovalStatus + '" ></span>'
                                    }
                                },
                                {
                                    "render": function (TimesheetID, type, full, meta) {

                                        
                                        if ((full.TimesheetApprovalStatus == 'Saved Timesheet') || (full.TimesheetApprovalStatus == 'Rejected') || (full.TimesheetApprovalStatus == 'Rejected at Level_1 Manager') || (full.TimesheetApprovalStatus == 'Rejected at Level_2 Manager') || (full.TimesheetApprovalStatus == 'Rejected at Admin')) {

                                            return '<a class="btn btn-icn"   data-MonthYear="' + full.Month_Year + '" data-TimesheetID="' + full.TimesheetID + '"  data-Usr_UserID="' + full.Usr_UserID + '" data-ByMonthlyDates="' + full.ByMonthlyDates + '" data-TimesheetMonth="' + full.TimeSheetMonth + '"  onclick="EditUserByWeekly(this)" ><i  class="fa fa-edit" title="Edit"></i></a>';
                                        }
                                        else {
                                            return '<a class="btn btn-icn"  id="PreviewManagerUser"   data-MonthYear="' + full.Month_Year + '" data-TimesheetID="' + full.TimesheetID + '" data-TimesheetID="' + full.TimesheetID + '" data-ProjectId="' + full.ProjectId + '" data-ClientprojectId="' + full.ClientprojectId + '" data-Usr_UserID="' + full.Usr_UserID + '" data-ByMonthlyDates="' + full.ByMonthlyDates + '"  data-TimesheetMonth="' + full.TimeSheetMonth + '" title="Preview" data-TimesheetApprovalStatus="' + full.TimesheetApprovalStatus +
                                                '"  onclick="ByWeeklyPreviewSubmitTimesheet(this)" ><i class="fa fa-eye"></i></a>';
                                        }
                                    },
                                },
                            ],
                            'initComplete': function () {
                                this.api().columns([5, 6]).every(function () {

                                    var column = this;
                                    var select = $('<select class="form-control form-control-lg"><option value="">Select ' + $("#UserGridDataByWeekly tfoot th:nth-child(" + Math.round(this[0][0] - 4) + ")").text() + '</option></select>')
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
                    $("#UserGridDataByWeekly").attr("style", "display: table;");
                    $("#ManagerGridPanel").attr("style", "display: none;");
                    if (resultData.mytimesheets) {


                        var objUsertimesheets3 = resultData.mytimesheets;
                        $('#UserGridDataByWeekly').DataTable({
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
                                        return '<span class="badge badge-radius" data-toggle="tooltip" id="' + data.TimesheetApprovalStatus + '" title="' + data.ManagerApprovalStatus + '" ></span>'
                                    }
                                },
                                {
                                    "render": function (TimesheetID, type, full, meta) {
                                        if ((full.TimesheetApprovalStatus == 'Saved Timesheet') || (full.TimesheetApprovalStatus == 'Rejected') || (full.TimesheetApprovalStatus == 'Rejected at Level_1 Manager') || (full.TimesheetApprovalStatus == 'Rejected at Level_2 Manager') || (full.TimesheetApprovalStatus == 'Revoked at Admin')) {
                                            return '<a class="btn btn-icn"   data-MonthYear="' + full.Month_Year +
                                                '" data-TimesheetID="' + full.TimesheetID +
                                                '"  data-Usr_UserID="' + full.Usr_UserID +
                                                '" data-ProjectId="' + full.ProjectId +
                                                '" data-ClientprojectId="' + full.ClientprojectId +
                                                '" data-ByMonthlyDates="' + full.ByMonthlyDates +
                                                '" data-TimesheetMonth="' + full.TimeSheetMonth +
                                                '" onclick="EditUserByWeekly(this)" ><i class="fa fa-edit" title="Edit"></i></a>';
                                        }

                                        else {

                                            return '<a class="btn btn-icn" id="PreviewUser"   data-MonthYear="' + full.Month_Year +
                                                '" data-TimesheetID="' + full.TimesheetID +
                                                '" data-Usr_UserID="' + full.Usr_UserID +
                                                '" data-ProjectId="' + full.ProjectId +
                                                '" data-ClientprojectId="' + full.ClientprojectId +
                                                '"  data-ByMonthlyDates="' + full.ByMonthlyDates +
                                                '" title="Preview"  onclick="ByWeeklyPreviewSubmitTimesheet(this)" data-TimesheetMonth="' + full.TimeSheetMonth +
                                                '" ><i class="fa fa-eye"></i></a>';

                                        }

                                    },

                                },

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

    $('a#images.kin').on('click', function () {
        $('#showimage').attr('src', $(this).find('img#previewFlImage').attr('src'));
        // $('#imgModal').modal('show');
    });
    $('a#images1.kin1').on('click', function () {
        $('#showimage1').attr('src', $(this).find("img#previewFlImage1").attr('src'));
        // $('#imgModal').modal('show');
    });

    function readURL(input) {
        //console.log(input);
        var urlInput = $(input).val();
        //console.log(urlInput);
        var ext = urlInput.substring(urlInput.lastIndexOf('.') + 1).toLowerCase();
        if (input.files && input.files[0] && (ext == "png" || ext == "jpeg" || ext == "jpg")) {
            var reader = new FileReader();

            reader.onload = function (e) {
                $('#previewFlImage').attr('src', e.target.result);
                $("#btnSave").removeAttr("disabled", "disabled");
                $("#btnSend").removeAttr("disabled", "disabled");
                $("#btnSaveUpdate").removeAttr("disabled", "disabled");
                $("#btnSubmitUpdate").removeAttr("disabled", "disabled");
            }

            reader.readAsDataURL(input.files[0]);
        } else {
            $('#previewFlImage').removeAttr('src');
            $("#btnSave").attr("disabled", "disabled");
            $("#btnSend").attr("disabled", "disabled");
            $("#btnSaveUpdate").attr("disabled", "disabled");
            $("#btnSubmitUpdate").attr("disabled", "disabled");

        }
    }

    $("#flImage").change(function () {
        readURL(this);
    });
    //function readURL1(input) {
    //    if (input.files && input.files[0]) {
    //        var reader = new FileReader();

    //        reader.onload = function (e) {
    //            $('#previewFlImage1').attr('src', e.target.result);
    //        }

    //        reader.readAsDataURL(input.files[0]);
    //    }
    //}
    function readURL1(input) {
        //console.log(input);
        var urlInput = $(input).val();
        //console.log(urlInput);
        var ext = urlInput.substring(urlInput.lastIndexOf('.') + 1).toLowerCase();
        if (input.files && input.files[0] && (ext == "png" || ext == "jpeg" || ext == "jpg")) {
            var reader = new FileReader();

            reader.onload = function (e) {
                $('#previewFlImage1').attr('src', e.target.result);
                $("#btnSave").removeAttr("disabled", "disabled");
                $("#btnSend").removeAttr("disabled", "disabled");
                $("#btnSaveUpdate").removeAttr("disabled", "disabled");
                $("#btnSubmitUpdate").removeAttr("disabled", "disabled");
            }

            reader.readAsDataURL(input.files[0]);
        } else {
            $('#previewFlImage1').removeAttr('src');
            $("#btnSave").attr("disabled", "disabled");
            $("#btnSend").attr("disabled", "disabled");
            $("#btnSaveUpdate").attr("disabled", "disabled");
            $("#btnSubmitUpdate").attr("disabled", "disabled");

        }
    }
    //$("#flImage1").change(function () {
    //    readURL1(this);
    //});
});

var TaskDetailsid = ''; var MonthTimesheet = '';
var TimesheetMonth; var tsuserid = '', AppRejectstatus = ''; var ByMonthlydates = '';
function EditUserByWeekly(id) {
    
    $("#body_ClientDetails").attr("style", "display: table;");
    $("#Timesheet_dvTSMode").html("Bi-Monthly");
    TimesheetID = id.getAttribute("data-TimesheetID");
    TSID = TimesheetID;
    MonthTimesheet = id.getAttribute("data-MonthYear");
    tsuserid = id.getAttribute("data-Usr_UserID");
    ByMonthlydates = id.getAttribute("data-ByMonthlyDates");
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
            Projectid: SessionProjectid,
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
            ViewTimesheetByWeekly(TimesheetMonth, resultDataArray);
            LoadPreviewTasklookups();
        },
        complete: function () {
            $('#loading-image').attr("style", "display: none;");
        }

    });

    LoadPreviewTaskData();
}


function ByWeeklyPreviewSubmitTimesheet(id) {
    
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
    $('#loading-image').attr("style", "display: block;");
    $.ajax({
        url: "/Timesheet/ViewByWeeklySubmitedTimesheet",
        type: "GET",
        data: {
            TimesheetUserid: tsuserid, Timesheetstartdate: Timesheetmonarray4[0],
            TimesheetEnddate: Timesheetmonarray4[1], Accountid: SessionAccountid,
            Projectid: SessionProjectid, ClientProjectId: cid

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
}


function ByWeeklyPreviewManagerSubmitTimesheet(id) {
    
    $("#body_ClientDetails").attr("style", "display: table;");
    $("#Timesheet_dvTSMode").html("Bi-Weekly");
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
            Projectid: SessionProjectid, ClientProjectId: cid
        },
        contentType: 'application/json; charset=utf-8',
        dataType: 'JSON',
        async: false,
        success: function (resultData) {
            resultDataArray = resultData.timeSheetDetails;
            resultDataArrayUploadimages = resultData.UploadTimesheetimage;
            PreviewByMonthlyLoadHoursWorkedAcctoDate(tsuserid, Timesheetmonarray5[0], Timesheetmonarray5[1], SessionAccountid, SessionProjectid);
            PreviewTimesheetsByWeekly(Timesheetmonarray5[1], resultDataArray);
            LoadTasklookupsByUser(tsuserid);
        },
        complete: function () {
            $('#loading-image').attr("style", "display: none;");
        }


    });
    LoadManagerData();

}


function PreviewTimesheetsByWeekly(MonthTimesheet, resultData) {
    
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

function ViewTimesheetByWeekly(MonthTimesheet, resultData) {
    $("#body_ClientDetails").attr("style", "display: table;");
    $("#Timesheet_dvTSMode").html("Bi-Weekly");
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
        if (v.Submitted_Type == 1 && v.L1_ApproverStatus == 1 && v.L2_ApproverStatus == 1) {
            $("#UserStatus").html("Approved by both Managers").css("color", "Green");
        }
        else if (v.Submitted_Type == 1 && v.L1_ApproverStatus == 1 && v.L2_ApproverStatus == 0) {
            $("#UserStatus").html("Approved by " + v.ManagerName1 + " Pending at " + v.ManagerName2).css("color", "Green");
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
    // }


    "</table>" + + "<hr />"
    $("#MainTable").append("</tr></table>") + "<hr />";

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


var Submittedtype = ""; var TaskLookupId = "", workingHours = "", Commentss = "";

function ConfirmSendTimesheet(id) {
    
    var files = $("#flImage").get(0).files;
    var formData = new FormData();

    for (var i = 0; i < files.length; i++) {
        formData.append("fileInput", files[i]);
    }
    var files1 = $("#flImage1").get(0).files;


    for (var i = 0; i < files1.length; i++) {
        formData.append("fileInput", files1[i]);
    }

    if (files.length == 0) {
        formData.append("ID", 1);
    }
    else if (files1.length == 0) {
        formData.append("ID", 2);
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


    //if (Commentss == "") {
    //    alert("Please enter comments");
    //    $("#txtDescription").focus();
    //    return false;
    //}
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


        if (rowOn != 8) {
            if (TaskId != undefined && hours != undefined && Date != null) {
                rowData = { taskDate: Date, taskid: TaskId, hoursWorked: hours }
            }
        }
        else {
            continue;
        }
        listtimesheetdetails.push(rowData);
    }


    // alert("Do you want to fill holidaydate of " + satdates + " ?");
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
        TimesheetMode: '2',

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

function ManagerApprOrRejByWeekly(id) {

    TimesheetID = id.getAttribute("data-TimesheetID");
    MonthTimesheet = id.getAttribute("data-MonthYear");
    tsuserid = id.getAttribute("data-Usr_UserID");
    ApporRejprojectid = id.getAttribute("data-ProjectId");
    ByMonthlydates = id.getAttribute("data-ByMonthlyDates");
    var Timesheetmonarray7 = new Array();
    Timesheetmonarray7 = ByMonthlydates.split("-");
    TimesheetMonth = Timesheetmonarray7[0];
    empprojectname = id.getAttribute("data-ProjectName");
    empname = id.getAttribute("data-userName");
    empactualhrs = id.getAttribute("data-CompanyBillingHours");
    empworkedhrs = id.getAttribute("data-ResourceWorkingHours");
    var ApproveRejectstatus = id.getAttribute("data-Status");
    id = ApproveRejectstatus;

    if (id == 3) {
        $("#btnAdd").show();
        $("#btnRej").hide();
        $('#PopupAppRejId').html("Approve Timesheet");
        $("#ProjectNameid").html("Do you want to approve <b> " + empname + "'s </b>timesheets within <b>" + empworkedhrs + "</b> hours for duration <b>" + ByMonthlydates.replace('-', ' to ') + "</b> ");
    }
    if (id == 4) {
        $("#btnRej").show();
        $("#btnAdd").hide();
        $('#PopupAppRejId').html("Reject Timesheet");
        $("#ProjectNameid").html("Do you want to reject <b> " + empname + "'s </b>timesheets within <b>" + empworkedhrs + "</b> hours for duration <b>" + ByMonthlydates.replace('-', ' to ') + "</b> ");
    }
    $('#ContainerGridDetail').show();

    //$("#ProjectNameid").html("Do you want to approve<b>" + empname + " </b> of Project <b>" + projectname + "</b> and  Worked Hours is <b>" + Workedhrs + "</b>  for Month of <b>" + MonthTimesheet + "</b>");



}
function ByWeeklyManagerApprOrRejbFromWeb(id) {

    ByMonthlydates = $("#Timesheet_Duration").html();

    if (id == '3' || id == '4') {

        id = id;
        if (id == 3) {
            $("#btnAdd").show();
            $("#btnRej").hide();
            $('#PopupAppRejId').html("Approve Timesheet");
            $("#ProjectNameid").html("Do you want to approve <b> " + empname + "'s </b>timesheets within <b>" + empworkedhrs + "</b> hours for duration <b>" + ByMonthlydates.replace('-', ' to ') + "</b> ");
        }
        if (id == 4) {
            $("#btnRej").show();
            $("#btnAdd").hide();
            $('#PopupAppRejId').html("Reject Timesheet");
            $("#ProjectNameid").html("Do you want to reject <b> " + empname + "'s </b>timesheets within <b>" + empworkedhrs + "</b> hours for duration <b>" + ByMonthlydates.replace('-', ' to ') + "</b> ");
        }
        $('#ContainerGridDetail').show();
    }


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

            $('#ts_ClientNameid').html(value.ProjectName);
            $('#ts_ProjectNameid').html(value.ProjectClientName);
        });
    }
}

function LoadData() {

    if (resultDataArray && resultDataArray.length > 0) {
        $.each(resultDataArray, function (index, value) {
            $('#uc1_ddlTask' + index).val(value.Taskid).attr("selected", "true");
            $('#uc1_txtHours' + index).val(value.NoofHoursWorked).attr("selected", "true");
            $('#txtDescription').val(value.Comments).attr("disabled", "true");
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
            $('#btnSave').hide();
            $('#btnSend').hide();
            $('#ts_ClientNameid').html(value.ProjectName);
            $('#ts_ProjectNameid').html(value.ProjectClientName);
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


function PreviewByMonthlyHoursDataColours() {
    $.each(resultHoursWorkedColourArray, function (index, value) {
        //if (value.colour == 'black') {
        //    value.colour = 'white';
        //}
        $('#HoursColours' + index).attr('style', 'font-size:25px;line-height:25px;color:' + value.colour);
        $('#HoursColours' + index).attr('title', value.colour);
    });

}

function print_page() {
    
    $("#divEditTimesheetData").addClass("pdfHide");
    if (window.print()) {
        console.log("print");
    } else {
        $("#divEditTimesheetData").removeClass("pdfHide");
    }
}

$("body").unbind().on("click", "#btnpdf", function () {

    //$("#images").hide();
    $("#divEditTimesheetData").addClass("pdfHide");

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
    setTimeout(function () {
        $("#divEditTimesheetData").removeClass("pdfHide");
        // $("#images").show();
    }, 1000);
});


function CalculatePreview() {

    var sum = 0;
    $("td.colored[name='uc1txtHours'] > font").each(function () {

        //sum += Number($(this).val()) || 0;
        sum += parseInt($(this).text());


    });

    $("#TotalHoursCount").html(sum);
}


function PreviewByMonthlyLoadHoursWorkedAcctoDate(tsuserid, Timesheetstartdate, Timesheetenddate, accountid, projectid) {
    $.ajax({
        url: "/Timesheet/LoadColourBymonthly",
        type: "GET",
        data: { Timesheetstartdate: Timesheetstartdate, TimesheetEnddate: Timesheetenddate, TimesheetUserid: tsuserid, accountid: accountid, projectid: projectid },
        contentType: 'application/json; charset=utf-8',
        dataType: 'JSON',
        async: false,
        // cache: false,

        success: function (resultData) {
            resultHoursWorkedColourArray = resultData

        },

    });
}


function ShareByMonthyTimesheet() {
    //
    var tomail = $("#emailto").val();
    var Subj = $("#subject").val();
    var sectomail = $("#emailto2").val();
    var formData = new FormData();

    formData.append("mail", tomail);
    formData.append("mail", sectomail);
    formData.append("Subject", Subj);
    formData.append("TimeSheetId", TSID);

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



