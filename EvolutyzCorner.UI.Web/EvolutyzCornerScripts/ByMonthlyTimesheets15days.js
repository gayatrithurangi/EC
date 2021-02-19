
var dt = new Date();
var presentyear = dt.getFullYear();
var currMon = dt.getMonth(); var resultDataArray; var hidtimesheet;
var MonthYear = ''; var SessionUsrid; var sessionProjectName, Sessionclientname, SessionAccountid, SessionProjectid, SessionTaskid; var byweeklydates = '';
var resultHoursWorkedColourArray;
var TaskDetailsid = ''; var MonthTimesheet = '';
var tsuserid = '', AppRejectstatus = ''; var TimesheetID = ''; var resultDataArrayUploadimages = '';
var colourcode = ''; var xclientid;
$(document).ready(function (e) {




    $('a#images.kin').on('click', function () {
        $('#showimage').attr('src', $(this).find("img#previewFlImage").attr('src'));
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
    $("#flImage1").change(function () {
        readURL1(this);
    });
   

});


$(document).ready(function () {
    SessionUsrid = Userid; sessionProjectName = Projectname; SessionProjectid = Projectid;
    Sessionclientname = ClientName, SessionAccountid = accountid; SessionTaskid = Taskid;
    xclientid = clientid;

    ValidateUserforTimesheetMode();

    $("#btnChangeDate").on("click", function () {

        $("#divbyweeklydates").show();
        $("#body").show();

        $('#btnChangeDate').hide();
        $('#btnPreview').show();
        $('#btnPrint').hide();
        $('#btnpdf').hide();
        $('#submit').show();
        $('#TotHrs').hide();
        $("#MainTable").html("");
        $("#SubTable").html("");
        $("#tabCon").html("");
        $("#StardateToEnddate").html("");
        $("#ClientDetails").hide();
        $('#Cmtsave').hide();

    });

    $("#month").on("change", function () {

        ValidateUserforTimesheetMode();

    });

});

function ValidateUserforTimesheetMode() {

    $.ajax({
        type: "POST",

        url: "/Timesheet/LoadByMonthlyDates",
        datatype: "Json",
        success: function (data) {
            $(".Task").empty();
            $(data).each(function () {

                $(".Task").append($("<option></option>").val(this.Id).html(this.dateName));

            });
        }
    });

}

function LoadBymonthlyDefaultTimesheets(xclientid) {

    //   var SessionUsrid sessionProjectName, Sessionclientname, SessionAccountid, SessionProjectid;
    $("#imagelbl").show();
    $("#imagelbl1").show();
    var e = document.getElementById("byWeeklyDates");
    var Byweeklyid = e.options[e.selectedIndex].text;

    $('#ts_ClientNameid').html(sessionProjectName);
    $('#ts_ProjectNameid').html(Sessionclientname);
    $.ajax({
        type: "GET",
        url: "/Timesheet/GetByWeeklyHoursWorked",

        data: {
            tdate: Byweeklyid,
            Userid: SessionUsrid,
            cmpnyAccountid: SessionAccountid,
            UsrProjectId: SessionProjectid,
            ClientprojID: xclientid
        },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (resultData) {
            var datahourworked = resultData.timeSheetDetails;
            $('#ts_ManagerNamesid1').html(resultData.objEmpInfo.ManagerName1);
            $('#Timesheet_Duration').html(resultData.objEmpInfo.TotalMonthName);
            $("#StardateToEnddate").empty();
            $("#StardateToEnddate").html("<span id='StartToEnd'><b>Timesheet of " + resultData.objEmpInfo.UserName + " for the Period of " + resultData.objEmpInfo.TotalMonthName + "</b></span>");
            if (resultData.objEmpInfo.ManagerName2 != '0') {
                $('#ts_ManagerNamesid2').html(resultData.objEmpInfo.ManagerName2);
                $("b.ts_ManagerNamesid2").show();
                $("#ts_Manager2").show();
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

            FirstLoadByTimesheetsTable(MonthYear, datahourworked)
            LoadBymonthlyTasklookups();
        }
    });
}
//Load DropDown Tasks
function LoadBymonthlyTasklookups() {

    $.ajax({
        type: "GET",
        url: "/Admin/getLookUp",
        datatype: "Json",
        //data: { id: id },
        success: function (data) {
            $.each(data, function (index, value) {

                $('.lookup').append('<option value="' + value.tsk_TaskID + '">' + value.tsk_TaskName + '</option>');

                $(".lookup option").each(function () {

                    if ($(this).val() == SessionTaskid) {
                        $(this).attr("selected", "selected");
                        return;
                    }
                });

            });
        },
    });
}
function FirstLoadByTimesheetsTable(MonthYear, resultData) {

    $("#divbyweeklydates").hide();
    $('#divMonthYear').hide();
    $('#btnChangeDate').show();
    $('#btnPrint').hide();
    $('#btnpdf').hide();
    $('#TotHrs').show();
    $('#submit').hide();
    $('#btnPreview').hide();
    $("#tabCon").empty();
    $('#Cmtsave').show();
    $("#MainTable").empty();
    $("#SubTable").empty();
    var counter = -1;
    var counter15 = -1;
    $("#ClientDetails").show(true);

    $("#tabCon").append("<table name='maintbl' id='MainTable' width='100%'>");

    $.each(resultData, function (k, v) {

        if (k <= 6) {

            if (counter15 == -1) {

                $("#MainTable").append("<td id='td1'>" +
                    "<table id='SubTable' width='100%' class='evAdminTable1'><tr style='background-color:black;color:white'><th width='160' text-align='center' class='evAdminTable'>Date </th> <th class='evAdminTable'>Task </th> <th width='50' class='evAdminTable'>Hours </th></tr>");

            }
            //if (v.colour == 'black') {
            //    v.colour = 'white';
            //}
            $("#SubTable").append("<tr><td>" + v.TaskDate + "</td><td><select id='uc1_ddlTask' class='lookup form-control' selected></select></td><td><div class='flex'><input class='form-control uc1txtHours' name='hrs' type='number' title='Enter hours' maxlength='2' min='0' max='24' id='uc1_txtHours" + k + "' value=" + v.NoofHoursWorked + "  style='width: 56px;'><span id='ts_colourcode" + k + "' title=" + v.colour + " style='font-size: 25px;line-height: 25px;color:" + v.colour + "'>* </span></div></td><td >&nbsp</td></td>");

            counter15 = counter15 + 1;


        }
        else {

            if (counter == -1) {
                $("#SubTable").append("</table>")
                $("#SubTable").append("</td>")
                $("#MainTable").append("<td id='td2'><table id='SubTable2' width='100%' class='evAdminTable1'><tr style='background-color:black;color:white'><th width='160' text-align='center' class='evAdminTable'>Date </th> <th class='evAdminTable'>Task </th> <th width='50'class='evAdminTable'>Hours</th></tr>");
            }
            //if (v.colour == 'black') {
            //    v.colour = 'white';
            //}
            $("#SubTable2").append("<tr><td>" + v.TaskDate + "</td><td><select class='lookup form-control' selected  id='uc1_ddlTask'  selected ></select></td><td><div class='flex'><input class='form-control uc1txtHours' name='hrs' type='number' title='Enter hours' maxlength='2' min='0' max='24' id='uc1_txtHours" + k + "' value=" + v.NoofHoursWorked + "  style='width: 56px;'><span id='ts_colourcode" + k + "' title=" + v.colour + " style='font-size: 25px;line-height: 25px;color:" + v.colour + "'>* </span></div></td><td >&nbsp</td></td>");
            // $('#uc1_ddlTask' + k).append('<option value="' + v.Taskid + '">' + v.Taskname + '</option>');
            counter = counter + 1;

        }

    });

    ByMonthlycalculateSum();
    $(":input").bind('keyup mouseup', function () {
        ByMonthlycalculateSum();

    });
    "</table>" + + "<hr />"
    $("#MainTable").append("</tr></table>") + "<hr />";
}
function ByMonthlycalculateSum() {

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
function AddByMonthlyTimesheets(id) {
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
    } else if (files1.length == 0) {
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
    var Commentss = $.trim($("#txtDescription").val());
    var Projectid = SessionProjectid;
    var satdates = [];
    var rows = $("#SubTable,#SubTable2").find("tr");
    var listtimesheetdetails = []; var rowData = {}; var Date, TaskId, hours, workingHours;
    for (var rowOn = 1; rowOn < rows.length; rowOn++) {
        workingHours = $(rows[rowOn]).find('.uc1txtHours').attr('id');
        colourcode = $(rows[rowOn]).find('span').attr('title');
        Date = $(rows[rowOn]).find("td").eq(0).text();
        TaskId = $(rows[rowOn]).find("#uc1_ddlTask option:selected").val();
        // hours = $(rows[rowOn]).find("input").val();
        hours = $("#" + workingHours).val();
        if (hours == "") {
            if (!$.trim(this.value)) {
                alert("Please enter working hours");
                $("#" + workingHours).val('');
                $("#" + workingHours).focus('');
                calculateSum();
                return false;
            }
        }
        if (hours != "") {
            if (hours < 0) {
                alert("Working hours should be not be less than zero");
                $("#" + workingHours).val('');
                $("#" + workingHours).focus('');
                calculateSum();
                return false;
            }
        }
        if (hours != "") {
            if (hours > 24) {
                alert("Working hours should not be greater than 24 hours");
                $("#" + workingHours).val('');
                $("#" + workingHours).focus('');
                calculateSum();
                return false;
            }
        }
        if ((colourcode == "blue" && colourcode != undefined) || (colourcode == "black" && colourcode != undefined) || (colourcode == "Red" && colourcode != undefined)) {

            if (hours > 0) {
                satdates.push(Date);


                hours = $("#" + workingHours).val();
                ByMonthlycalculateSum();




            }
        }
        if (rowOn != 8) {
            if (TaskId != undefined && hours != undefined && Date != null) {
                rowData = { taskDate: Date, taskid: TaskId, hoursWorked: hours, ProjectID: Projectid }
            }
        }
        else {
            continue;
        }

        listtimesheetdetails.push(rowData);
    }


    var e = document.getElementById("byWeeklyDates");
    byweeklydates = e.options[e.selectedIndex].text;

    var Timesheetmonarray = new Array();
    Timesheetmonarray = byweeklydates.split("-");
    if (satdates.length != 0) {
        if (confirm("Do you want to enter hours for " + satdates + " ?")) {

            $("span[id^='HoursColours'][title='black']").closest('tr').removeClass("brown");

        } else {
            $("span[id^='HoursColours'][title='black']").closest('tr').addClass("brown");
            return false;
        }
    }

    var timesheets = {
        UserID: SessionUsrid,
        TimeSheetMonth: Timesheetmonarray[0],
        ByWeeklyStartDate: Timesheetmonarray[0],
        ByWeeklyEndDate: Timesheetmonarray[1],
        Comments: Commentss,
        SubmittedType: Submittedtype,
        TimesheetMode: '2',
    }

    var sheetObj =
    {
        timesheets: timesheets,
        listtimesheetdetails: listtimesheetdetails
    };

    $('#loading-image').attr("style", "display: block;");
    $.ajax({
        type: "POST",
        url: "/Timesheet/AddSubmitTimesheet",
        data: JSON.stringify(sheetObj),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            alert(data);
            window.location = '/Timesheet/PreviewBymonthlytimesheets15days';
        }
        ,
        complete: function () {
            $('#loading-image').attr("style", "display: none;");
        },
    });

}
function updateByMonthlyTimesheet(id) {
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
    } else if (files1.length == 0) {
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

    var e = document.getElementById("byWeeklyDates");
    byweeklydates = e.options[e.selectedIndex].text;
    var Timesheetmonarray3 = new Array();
    Timesheetmonarray3 = byweeklydates.split("-");


    var rows = $("#SubTable,#SubTable2").find("tr");

    var listtimesheetdetails = []; var rowData = {}; var Date, TaskId, hours;

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

        //if (colourcode == "black" && colourcode != undefined) {

        //    if (hours > 0) {
        //        var r = confirm("Do you want to fill satday and sunday hours of " + Date + " ?");
        //        if (r == true) {
        //            hours = $("#" + workingHours).val();
        //            ByMonthlycalculateSum();

        //        }
        //        else {
        //            return false;

        //        }

        //    }
        //}

        //if (colourcode == "blue" && colourcode != undefined) {

        //    if (hours > 0) {
        //        var r = confirm("Do you want to fill holidaydate of " + Date + " ?");
        //        if (r == true) {
        //            hours = $("#" + workingHours).val();
        //            ByMonthlycalculateSum();

        //        }
        //        else {
        //            return false;

        //        }

        //    }
        //}

        //if (colourcode == "Red" && colourcode != undefined) {

        //    if (hours > 0) {
        //        var r = confirm("Do you want to fill leavedate of " + Date + "  ?");
        //        if (r == true) {
        //            hours = $("#" + workingHours).val();
        //            ByMonthlycalculateSum();

        //        }
        //        else {
        //            return false;


        //        }

        //    }
        //}
        if ((colourcode == "blue" && colourcode != undefined) || (colourcode == "black" && colourcode != undefined) || (colourcode == "Red" && colourcode != undefined)) {

            if (hours > 0) {
                satdates.push(Date);


                hours = $("#" + workingHours).val();
                ByMonthlycalculateSum();




            }
        }
        if (rowOn != 8) {
            if (TaskId != undefined && hours != undefined && Date != null) {
                rowData = { taskDate: Date, taskid: TaskId, hoursWorked: hours, ProjectID: Projectid }
            }
        }
        else {
            continue;
        }
        listtimesheetdetails.push(rowData);
    }

    var e = document.getElementById("byWeeklyDates");
    byweeklydates = e.options[e.selectedIndex].text;

    var Timesheetmonarray = new Array();
    Timesheetmonarray = byweeklydates.split("-");
    if (satdates.length != 0) {
        if (confirm("Do you want to enter hours for " + satdates + " ?")) {

            $("span[id^='HoursColours'][title='black']").closest('tr').removeClass("brown");

        } else {
            $("span[id^='HoursColours'][title='black']").closest('tr').addClass("brown");
            return false;
        }
    }

    var timesheets = {
        TimesheetID: hidtimesheet,
        ByWeeklyStartDate: Timesheetmonarray[0],
        ByWeeklyEndDate: Timesheetmonarray[1],
        TimeSheetMonth: Timesheetmonarray3[2],
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
function CheckTimesheetexits() {

    var month = $("#month").val();
    var year = $("#year").val();
    if (month <= 9) {

        MonthYear = year + '-' + "0" + month + '-' + '01';
    }
    else {

        MonthYear = year + '-' + month + '-' + '01';
    }

    var e = document.getElementById("byWeeklyDates");
    byweeklydates = e.options[e.selectedIndex].text;
    var Timesheetmonarray1 = new Array();

    Timesheetmonarray1 = byweeklydates.split("-");
    var objDate = new Date(MonthYear),
        locale = "en-us",
        month = objDate.toLocaleString(locale, { month: "short" });


    $.ajax({
        type: "GET",
        url: "/Timesheet/CheckTimesheetByWeeklyExists",
        data: { timesheetstartdate: Timesheetmonarray1[0], timesheetenddate: Timesheetmonarray1[1], clientprojid: xclientid},
        datatype: "Json",
        async: false,
        success: function (data) {

            if (data == '101') {
                var r = confirm("TimeSheet  is already Submitted between '" + Timesheetmonarray1[0] + "' and '" + Timesheetmonarray1[1] + "',\nDo you want to Preview?");
                if (r == true) {
                    $("#btnSave").attr("style", "display: none;");
                    $("#btnSend").attr("style", "display: none;");
                    $("#btnSaveUpdate").attr("style", "display: none;");
                    $("#btnSubmitUpdate").attr("style", "display: none;");
                    $("#txtDescription").attr('readonly', true);
                    // LoadClientDetails();
                    LoadsubmittedBymonthlyTimesheets(Timesheetmonarray1[1]);
                }

            }
            if (data == '102') {

                var r = confirm("TimeSheet  is saved already between '" + Timesheetmonarray1[0] + "' and '" + Timesheetmonarray1[1] + "',\nDo you want to Preview?");
                if (r == true) {

                    $("#btnSave").attr("style", "display: none;");
                    $("#btnSend").attr("style", "display: none;");
                    $("#btnSaveUpdate").attr("style", "display: inline-block;");
                    $("#btnSubmitUpdate").attr("style", "display: inline-block;");
                    // LoadClientDetails();
                    displaysavedBymonthlyTimesheets(Timesheetmonarray1[1], SessionUsrid);

                }
            }
            if (data == "103") {
                LoadBymonthlyDefaultTimesheets(xclientid);

            }
        },
    });
}
//Saved  byMonthlyTimesheets
function displaysavedBymonthlyTimesheets(MonthTimesheet, tsuserid) {
    var taskid = '';
    $("#btnPrint").hide();
    $("#btnpdf").hide();
    // $("#flImage").show();
    $("#imagelbl").show();
    $("#images").show();
    $("#imagelbl1").show();
    $("#images1").show();
    $('#ts_ClientNameid').html(sessionProjectName);
    $('#ts_ProjectNameid').html(Sessionclientname);
    var e = document.getElementById("byWeeklyDates");
    byweeklydates = e.options[e.selectedIndex].text;
    var Timesheetmonarray2 = new Array();
    Timesheetmonarray2 = byweeklydates.split("-");

    $.ajax({
        url: "/Timesheet/ViewByWeeklySubmitedTimesheet",
        type: "GET",
        data: { TimesheetUserid: SessionUsrid, Timesheetstartdate: Timesheetmonarray2[0], TimesheetEnddate: Timesheetmonarray2[1], Accountid: SessionAccountid, Projectid: SessionProjectid },
        contentType: 'application/json; charset=utf-8',
        dataType: 'JSON',
        async: false,
        success: function (resultData) {
            resultDataArray = resultData.timeSheetDetails;
            resultDataArrayUploadimages = resultData.UploadTimesheetimage;
            ByMonthlyLoadHoursWorkedAcctoDate(SessionUsrid, Timesheetmonarray2[0], Timesheetmonarray2[1], SessionAccountid, SessionProjectid);
            savedBymonthlyTimesheets(MonthTimesheet, resultDataArray);
            LoadbymonthlySavedTasks();
        }
    });
    LoadmonthlyTaskData();

}

function savedBymonthlyTimesheets(MonthTimesheet, resultData) {
    $("#divbyweeklydates").hide();
    $('#divMonthYear').hide();
    $('#btnChangeDate').show();
    $('#btnPrint').hide();
    $('#btnpdf').hide();
    $('#TotHrs').show();
    $('#submit').hide();
    $('#btnPreview').hide();
    $("#StardateToEnddate").empty();
    $("#tabCon").empty();
    $('#Cmtsave').show();
    $("#MainTable").empty();
    $("#SubTable").empty();
    // $("#flImage").show();
    $("#imagelbl").show();
    $("#images").show();
    // $("#flImage1").show();
    $("#imagelbl1").show();
    $("#images1").show();
    var tbltask = ''; var objSelect = '';

    var counter = -1;
    var counter15 = -1;
    $("#ClientDetails").show(true);
    $("#tabCon").append("<table name='maintbl' id='MainTable' width='100%'>");

    $.each(resultData, function (k, v) {
        hidtimesheet = v.TimesheetID;
        $('#dvTimesheetids').html(hidtimesheet);
        $('#Timesheet_Duration').html(v.TotalMonthName);
        $("#ts_Approveid").hide();
        $("#ts_Submittedid").hide();
        $("#ts_approid").hide();
        $("#ts_submited").hide();
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
        $("#txtDescription").val(v.Comments);

        if (k <= 6) {
            tbltask = 'uc1_ddlTask' + k;

            if (counter15 == -1) {
                $("#MainTable").append("<td id='td1'>" +
                    "<table id='SubTable' width='100%' class='evAdminTable1'><tr style='background-color:black;color:white'><th width='160' text-align='center' class='evAdminTable'>Date </th> <th class='evAdminTable'>Task </th> <th width='50' class='evAdminTable'>Hours </th></tr>");
            }

            $("#SubTable").append("<tr><td>" + v.MonthYearName + "</td><td><select id='uc1_ddlTask" + k + "' class='lookup form-control' selected  ></select></td><td><div class='flex'><input class='form-control uc1txtHours' name='hrs' type='number' title='Enter hours' min='0' max='24' maxlength='2' id='uc1_txtHours" + k + "' value=" + v.NoofHoursWorked + "  style='width: 56px;'><span id='HoursColours" + k + "' title=" + v.colour + " style='font-size: 25px;line-height: 25px;color:" + v.colour + "'>* </span></div></td><td >&nbsp</td></td>");
            counter15 = counter15 + 1;
        }
        else {

            tbltask = 'uc1_ddlTask' + k;
            if (counter == -1) {
                $("#SubTable").append("</table>")
                $("#SubTable").append("</td>")
                $("#MainTable").append("<td id='td2'><table id='SubTable2' width='100%' class='evAdminTable1'><tr style='background-color:black;color:white'><th width='160' text-align='center' class='evAdminTable'>Date </th> <th class='evAdminTable'>Task </th> <th width='50'class='evAdminTable'>Hours</th></tr>");
            }
            //$("#SubTable2").append("<tr><td>" + v.MonthYearName + "</td><td><select id='uc1_ddlTask" + k + "' class='lookup form-control' selected  ></select></td><td><div class='flex'><input class='form-control uc1txtHours' name='hrs' type='number' title='Enter hours' min='0' max='24' maxlength='2' id='uc1_txtHours" + k + "' value=" + v.NoofHoursWorked + "  style='width: 56px;'><span id='HoursColours" + k + "' style='font-size: 25px;line-height: 25px;color:" + v.colour + "'>* </span></div></td><td >&nbsp</td></td>");
            $("#SubTable2").append("<tr><td>" + v.MonthYearName + "</td><td><select class='lookup form-control' selected  id='uc1_ddlTask" + k + "'  selected ></select></td><td><div class='flex'><input class='form-control uc1txtHours' title='Enter hours' name='hrs' type='number'   min='0' max='24'  maxlength='2' id='uc1_txtHours" + k + "'  value=" + v.NoofHoursWorked + " style='width: 56px;'><span id='HoursColours" + k + "' title=" + v.colour + " style='font-size: 25px;line-height: 25px;color:" + v.colour + "'>* </span></div></td><td>&nbsp</td></td>");

            counter = counter + 1;
        }

    });
    ByMonthlyHoursDataColours();
    ByMonthlycalculateSum();
    $(":input").bind('keyup mouseup', function () {
        ByMonthlycalculateSum();

    });


    "</table>" + + "<hr />"
    $("#MainTable").append("</tr></table>") + "<hr />";


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

function LoadbymonthlySavedTasks() {
    $('#loading-image').attr("style", "display: block;");
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


        },
        complete: function () {
            $('#loading-image').attr("style", "display: none;");
        }
    });

}

function LoadmonthlyTaskData() {


    if (resultDataArray && resultDataArray.length > 0) {

        $.each(resultDataArray, function (index, value) {
            $('#uc1_ddlTask' + index).val(value.Taskid).attr("selected", "selected");
        });
    }
}

function LoadsubmittedBymonthlyTimesheets(MonthTimesheet) {
    $("#btnPrint").show();
    $("#btnpdf").show();
    $('#ts_ClientNameid').html(sessionProjectName);
    $('#ts_ProjectNameid').html(Sessionclientname);
    $("#imagelbl").hide();
    $("#images").hide();
    //$("#flImage1").hide();

    $("#imagelbl1").hide();
    $("#images1").hide();
    var e = document.getElementById("byWeeklyDates");
    byweeklydates = e.options[e.selectedIndex].text;
    var Timesheetmonarray2 = new Array();
    Timesheetmonarray2 = byweeklydates.split("-");

    $.ajax({
        url: "/Timesheet/ViewByWeeklySubmitedTimesheet",
        type: "GET",
        data: { TimesheetUserid: SessionUsrid, Timesheetstartdate: Timesheetmonarray2[0], TimesheetEnddate: Timesheetmonarray2[1], Accountid: SessionAccountid, Projectid: SessionProjectid },
        contentType: 'application/json; charset=utf-8',
        dataType: 'JSON',
        async: false,
        // cache: false,

        success: function (resultData) {
            resultDataArray = resultData.timeSheetDetails;
            resultDataArrayUploadimages = resultData.UploadTimesheetimage;
            ByMonthlyLoadHoursWorkedAcctoDate(SessionUsrid, Timesheetmonarray2[0], Timesheetmonarray2[1], SessionAccountid, SessionProjectid);
            PreviewSubmittedByweeklyTimesheet(Timesheetmonarray2[1], resultDataArray);
        },
    });
    LoadBymonthlySubmittedTaskData();
}

function PreviewSubmittedByweeklyTimesheet(MonthTimesheet, resultData) {

    $("#divbyweeklydates").hide();
    $('#divMonthYear').hide();
    $('#btnChangeDate').show();
    $('#btnPrint').show();
    $('#btnpdf').show();
    $('#TotHrs').show();
    $('#submit').hide();
    $('#btnPreview').hide();
    $("#StardateToEnddate").empty();
    $("#tabCon").empty();
    $("#MainTable").empty();
    $("#SubTable").empty();
    var tbltask = ''; var objSelect = '';
    $('#Cmtsave').show();
    var counter = -1;
    var counter15 = -1;
    $("#ClientDetails").show(true);
    $("#tabCon").append("<table name='maintbl' id='MainTable' width='100%' /*style='position:relative;left:10%;'*/>");
    $.each(resultData, function (k, v) {

        $("#StardateToEnddate").html("<span id='StartToEnd'><b>Timesheet of " + v.UserName + " for the Period of " + v.TotalMonthName + "</b></span>");
        $('#Timesheet_Duration').html(v.TotalMonthName);
        $("#txtDescription").val(v.Comments);
        $('#ts_ManagerNamesid1').html(v.ManagerName1);
        $("#ts_Approveid").hide();
        $("#ts_Submittedid").hide();
        $("#ts_approid").hide();
        $("#ts_submited").hide();

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

        if (k <= 6) {
            tbltask = 'uc1_ddlTask' + k;
            if (counter15 == -1) {
                $("#MainTable").append("<td id='td1'>" +
                    "<table id='SubTable' width='100%' class='evAdminTable1'><tr style='background-color:black;color:white'><th width='160' text-align='center' class='evAdminTable'>Date </th> <th class='evAdminTable'>Task </th> <th width='50' class='evAdminTable'>Hours </th></tr>");
            }


            $("#SubTable").append("<tr><td class='colored'>" + v.MonthYearName + "</td><td class='colored'  id='uc1_ddlTask" + k + "'>" + v.Taskname + "</td><div class='flex'><td class='colored' class='form-control uc1txtHours' name='uc1txtHours' id='uc1_txtHours" + k + "'><font>" + v.NoofHoursWorked + "</font><span id='HoursColours" + k + "' title=" + v.colour + " style='font-size: 25px;line-height: 25px;color:" + v.colour + "'>* </span></div></td></td>");
            counter15 = counter15 + 1;


        }
        else if (k > 7) {

            tbltask = 'uc1_ddlTask' + k;
            if (counter == -1) {
                $("#SubTable").append("</table>")
                $("#SubTable").append("</td>")
                $("#MainTable").append("<td id='td2'><table id='SubTable2' /*style='position:relative;top:19px;'*/ width='100%' class='evAdminTable1'><tr style='background-color:black;color:white'><th width='160' text-align='center' class='evAdminTable'>Date </th> <th class='evAdminTable'>Task </th> <th width='50'class='evAdminTable'>Hours</th></tr>");
            }
            $("#SubTable2").append("<tr><td class='colored'>" + v.MonthYearName + "</td><td class='colored'  id='uc1_ddlTask" + k + "'>" + v.Taskname + "</td><div class='flex'><td class='colored' class='form-control uc1txtHours' name='uc1txtHours' id='uc1_txtHours" + k + "'><font>" + v.NoofHoursWorked + "</font><span id='HoursColours" + k + "' title=" + v.colour + " style='font-size: 25px;line-height: 25px;color:" + v.colour + "'>* </span></div></td></td>");
            //   $("#SubTable2").append("<tr><td class='colored'>" + v.MonthYearName + "</td><td id='uc1_ddlTask" + k + "'  class='colored lookup'>" + v.Taskname + "</td><td  class='colored uc1txtHours' name='uc1txtHours'  id='uc1_txtHours" + k + "'><font>" + v.NoofHoursWorked + "</font><span id='HoursColours" + k + "' style='font-size: 25px;line-height: 25px;color:" + v.colour + "'>* </span></div></td></td>");

            counter = counter + 1;


        }


        //  $('#txtDescription').val(v.Comments).attr("disabled", "true");


    });

    ByMonthlyHoursDataColours();
    ByMonthlyCalculateTtlLeaves();
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

function LoadSavedHoursDataColours() {
    $.each(resultHoursWorkedColourArray, function (index, value) {
        //    if (value.colour == 'black') {
        //        value.colour = 'white';
        //    }
        $('#HoursColours' + index).attr('style', 'font-size:25px;line-height:25px;color:' + value.colour);
        $('#HoursColours' + index).attr('title', value.colour);
    });

}

function LoadBymonthlySubmittedTaskData() {

    if (resultDataArray && resultDataArray.length > 0) {

        $.each(resultDataArray, function (index, value) {

            $('#uc1_ddlTask' + index).val(value.Taskid).attr("selected", "true");
            $('#uc1_txtHours' + index).val(value.NoofHoursWorked).attr("selected", "true");

        });
    }
}

function ByMonthlyHoursDataColours() {
    $.each(resultHoursWorkedColourArray, function (index, value) {
        ////if (value.colour == 'black') {
        ////    value.colour = 'white';
        ////}
        $('#HoursColours' + index).attr('style', 'font-size:25px;line-height:25px;color:' + value.colour);
        $('#HoursColours' + index).attr('title', value.colour);
    });

}


function ByMonthlyCalculateTtlLeaves() {
    var sum = 0;
    $("td.colored[name='uc1txtHours'] > font").each(function () {
        //sum += Number($(this).val()) || 0;
        sum += parseInt($(this).text());

    });
    $("#TotalHoursCount").html(sum);
}


function ByMonthlyLoadHoursWorkedAcctoDate(tsuserid, Timesheetstartdate, Timesheetenddate, accountid, projectid) {
    $.ajax({
        url: "/Timesheet/LoadColourBymonthly",
        type: "GET",
        data: { Timesheetstartdate: Timesheetstartdate, TimesheetEnddate: Timesheetenddate, TimesheetUserid: tsuserid, accountid: accountid, projectid: projectid },
        contentType: 'application/json; charset=utf-8',
        dataType: 'JSON',
        async: false,
        // cache: false,

        success: function (resultData) {
            resultHoursWorkedColourArray = resultData;

        }

    });
}




