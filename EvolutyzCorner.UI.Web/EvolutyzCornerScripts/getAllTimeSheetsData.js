

function BiWeeklyheetexits() {

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
        data: { timesheetstartdate: Timesheetmonarray1[0], timesheetenddate: Timesheetmonarray1[1] },
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
                LoadBymonthlyDefaultTimesheets();

            }
        },
    });
}





