

var managerFlag;
var ManagerFName;
var ManagerLName;
var uid;
var perms;
var Rolid;
var Proj_AccountID = '@ViewBag.AccountId';
var projid;
var editprojectid;
editprojectid = $("#editpid").val();
var Proj_ProjectID = "";
var projectspecifictaskid;
var deluserid;
var delHoliday;
var delprotaskid;
var delproid;
var roleids;
var Pid;
Pid = $("#project option:selected").val();


$(document).ready(function () {
    $(window).keydown(function (event) {
        if (event.keyCode === 13) {
            event.preventDefault();
            return false;
        }
    });
    var appendRowClick_count = 1;
    $('#EmpForm').validate({
        rules: {
            Usr_TaskID: {
                required: true
            },
            Usr_UserTypeID: {
                required: true
            },
            RoleName: {
                required: true
            },
            project: {
                required: true
            },
            Usr_Titleid: {
                required: true

            },
            UsrP_FirstName: {
                required: {
                    depends: function () {
                        $(this).val($.trim($(this).val()));
                        return true;
                    }
                },
                regx: /[^-\s][a-zA-Z_\s-]+/

            },
            UsrP_LastName: {
                required: {
                    depends: function () {
                        $(this).val($.trim($(this).val()));
                        return true;
                    }
                },
                regx: /[^-\s][a-zA-Z_\s-]+/
            },

            Usrp_MobileNumber: {
                required: {
                    depends: function () {
                        $(this).val($.trim($(this).val()));
                        return true;
                    }
                },
                regex: /^[0-9]{10}$/

            },
            //Usr_Username: {
            //    required: {
            //        depends: function () {
            //            $(this).val($.trim($(this).val()));
            //            return true;
            //        }
            //    },
            //    regx: /^[a-zA-Z0-9]+$/
            //},
            Email: {
                required: {
                    depends: function () {
                        $(this).val($.trim($(this).val()));
                        return true;
                    }
                },
                email: true
            },
            Usr_Password: {
                required: true,
                reg: /^.*(?=.{8,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%&]).*$/
            },
            Usr_ConfirmPassword: {
                required: true,
                //reg: /^.*(?=.{8,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%&]).*$/,
                equalTo: "#Usr_Password"
            },
            Usr_GenderId: {
                required: true
            },
            ManagerName: {
                required: true
            },
            TimesheetMode_id: {
                required: true
            },
            Usr_isDeleted: {
                required: true
            },
            Usrp_CountryCode: {
                required: true
            },
            UProj_ParticipationPercentage: {
                perregex: /^(?:\d|[1-9]\d|100)$/
            }
        },
        submitHandler: function (form) {

            var man = $("#ManagerName").val();
            var id = "";
            id = $(this.submitButton).attr("id");
            if (man !== "" && id === "btnEmpAddModel") {
                AddEmployee();
            }
            else if ((man !== "" && id === "btnManAddModel") || (man === "" && id === "btnManAddModel")) {
                ManagerSaveData();
            }
            else {
                updategrid();
            }

            return false;


        }
    });


    $.validator.addMethod("regx", function (value, element, regexpr) {
        return regexpr.test(value);
    }, "Please enter  Only Alphabets.");
    $.validator.addMethod("perregex", function (value, element, regexpr) {
        return regexpr.test(value);
    }, "Please enter percentage below 100.");
    $.validator.addMethod("regex", function (value, element, regexpr) {
        return regexpr.test(value);
    }, "Please enter 10 Digit Number.");
    $.validator.addMethod("reg", function (value, element, regexpr) {
        return regexpr.test(value);
    }, "Password Must contain 1 Capital,1 small,1 number,1 Special Character and length must be 8 and above");

    $("#ManagerName").change(function () {
        
        var userid = $("#ManagerName").val();
        var CL_ProjectId = $("#project").val();
        ManagerOneChange(CL_ProjectId, userid);
    });


    //var template = jQuery.validator.format($.trim($("#appendTable > tbody > tr:not(:first-child)").html()));
    //console.log(template);
    $("#appendRow").click(function () {
        //appendRowClick_count++;
        var tplData = {
            i: appendRowClick_count
        };
        //console.log($(template(appendRowClick_count++)));
        Getfinancialyears();
        $("#appendTable").each(function (e) {
            var tds = '<tr>';
            jQuery.each($('tr:last td', this), function (i, v) {
                var k = jQuery.validator.format($.trim($(this).html()));
                tds += '<td class="control_' + Math.round((i + 1) * appendRowClick_count) + '">' + k(appendRowClick_count) + '</td>';
                //console.log($(this));
                //console.log($(this).children().attr("name"));
                //console.log($(this).children().attr("name", "name_" + appendRowClick_count + ""));
                $(this).children().attr("name", function (n, v) {
                    return v + appendRowClick_count;
                });
            });
            tds += '</tr>';

            if ($('tbody', this).length > 0) {
                $('tbody', this).append(tds)
                    .promise()
                    .done(function () {
                        $(this).find("input[name^='HolidayDate']").datepicker({
                            format: 'mm/dd/yyyy',
                            autoclose: true
                        });
                    });
                $('tbody', this).find(".form-control").prop("disabled", false);
            } else {
                $(this).append(tds)
                    .promise()
                    .done(function () {
                        $(this).find("input[name^='HolidayDate']").datepicker({
                            format: 'mm/dd/yyyy',
                            autoclose: true
                        });
                    });
                $(this).find(".form-control").prop("disabled", false);
            }
        });
        appendRowClick_count += 1;
        //$("input.HolidayName.form-control").each(function () {

        //    $(this).rules("add", {
        //        required: true,
        //        messages: {
        //            required: "Specify the Holiday Name"
        //        }
        //    });
        //});
    });
    $('form#holidayform').on('submit', function (event) {

        $("input.HolidayName.form-control").each(function () {
            $(this).rules("add", {
                required: true,
                regx: /[^-\s][a-zA-Z_\s-]+/,
                messages: {
                    required: "Specify the Holiday Name"
                }
            });
        });
        $("input.HolidayDate.form-control").each(function () {
            $(this).rules("add", {
                required: true,
                date: true,
                messages: {
                    required: "Enter the Holiday Date",

                }
            });
        });

        $("select.FinancialYearId.form-control").each(function () {
            $(this).rules("add", {
                required: true,
                messages: {
                    required: "Select Financial Year"
                }
            });
        });
        $("select.HolidayCalendarProjectId.form-control").each(function () {
            $(this).rules("add", {
                required: true,
                messages: {
                    required: "Select Project"
                }
            });
        });

        $("select.isOptionalHoliday.form-control").each(function () {
            $(this).rules("add", {
                required: true,
                messages: {
                    required: "Select Optional Holiday"
                }
            });
        });

        $("select.isDeleted.form-control").each(function () {
            $(this).rules("add", {
                required: true,
                messages: {
                    required: "Select Status"
                }
            });
        });




        event.preventDefault();

        if ($('form#holidayform').validate().form()) {
            console.log("validates");
        } else {
            console.log("does not validate");
        }
    });

    //$("#addInput").on('click', addInput);

    $('form#holidayform').validate({
        submitHandler: function (form) {

            var hcid = $("#HolidayCalendarID").val();
            if (hcid === "") {
                var holidays = [];
                var HolidayNames = new Array();
                $("input[name^='HolidayName']").each(function () {
                    HolidayNames.push($(this).val());
                });
                var HolidayDates = new Array();
                $("input[name^='HolidayDate']").each(function () {
                    HolidayDates.push($(this).val());
                });
                var FinancialYearIds = new Array();
                $("select[name^='FinancialYearId']").each(function () {
                    FinancialYearIds.push($(this).val());
                });
                var ClientCalProjIDs = new Array();
                $("select[name^='HolidayCalendarProjectId']").each(function () {
                    ClientCalProjIDs.push($(this).val());
                });
                var optionalholidays = new Array();
                $("select[name^='isOptionalHoliday']").each(function () {
                    optionalholidays.push($(this).val());
                });
                var status = new Array();
                $("select[name^='isDeleted']").each(function () {
                    status.push($(this).val());
                });

                for (var i = 0; i <= HolidayNames.length - 1; i++) {
                    holidays.push({
                        "HolidayName": HolidayNames[i],
                        "HolidayDate": HolidayDates[i],
                        "FinancialYearId": FinancialYearIds[i],
                        "HolidayCalendarProjectId": ClientCalProjIDs[i],
                        "isOptionalHoliday": optionalholidays[i],
                        "isDeleted": status[i], "ProjectID": projid,

                    });
                }

                holidays = JSON.stringify({ 'holidays': holidays });
                $.ajax({
                    contentType: 'application/json; charset=utf-8',
                    type: 'POST',
                    url: '/HolidayCalendar/CreateHolidayforclient',
                    data: holidays,

                    success: function (data) {

                        if (data === "HolidayName Already Exist In this Project") {
                            $("#HolidayName").addClass("validate_msg");
                            $("#HolidayDate").removeClass("validate_msg");
                        }
                        else if (data === "HolidayDate Already Exist In this Project") {
                            $("#HolidayName").removeClass("validate_msg");
                            $("#HolidayDate").addClass("validate_msg");
                            $("#HolidayDate").removeClass("validatemsg");
                        }
                        else if (data === "Please Select Correct financialyear") {


                            $("#HolidayName").removeClass("validate_msg");
                            $("#HolidayDate").removeClass("validate_msg");
                            $("#FinancialYearId").after("<label class='error'>" + data + "</label>");
                            $("#HolidayDate").addClass("validatemsg");
                        }
                        else {
                            alert(data);
                            $("#HolidayName").removeClass("validate_msg");
                            $("#HolidayDate").removeClass("validatemsg");
                            $("#HolidayDate").removeClass("validate_msg");
                            $('#Holidaycalendergrid').hide();
                            window.location.reload();
                            $(".modal-backdrop").hide();
                            $('#usertable').dataTable();
                            $('#holidaytable').dataTable();
                            $('#tasktable').dataTable();
                            $('#Projecttable').dataTable();

                            if (projid === "") {
                                usergriddata($("#editpid").val());
                                loadholidays($("#editpid").val());
                                loadtaks($("#editpid").val());
                                loadproject($("#editpid").val());
                            } else {
                                loadholidays(projid);
                                usergriddata(projid);
                                loadtaks(projid);
                                loadproject(projid);
                            }
                        }


                    },
                    complete: function () {
                        $('#loading-image').attr("style", "display: none;");
                    },
                    error: function (error) {
                        console.log(error);
                    }

                });
                return false;
            } else {

                var holiday = [];
                var Hcid = $("#HolidayCalendarID").val();
                var holidayname = $("#HolidayName").val();
                var holidatydate = $("#HolidayDate").val();
                var financialyear = $("#FinancialYearId").val();

                var optionalholiday = $("#isOptionalHoliday").val();
                var Status = $("#isDeleted").val();
                var ClientHoldyPid = $("#HolidayCalendarProjectId").val();
                holiday.push({
                    "HolidayName": holidayname,
                    "HolidayDate": holidatydate,
                    "FinancialYearId": financialyear,
                    "isOptionalHoliday": optionalholiday,
                    "isDeleted": Status,
                    "HolidayCalendarProjectId": ClientHoldyPid
                });

                holiday = JSON.stringify({ 'holiday': holiday });
                $.ajax({
                    //contentType: 'application/json; charset=utf-8',
                    type: 'POST',
                    url: '/HolidayCalendar/UpdateCalenderControl',
                    data: {
                        "HolidayCalendarID": Hcid,
                        "HolidayName": holidayname,
                        "HolidayDate": holidatydate,
                        "FinancialYearId": financialyear,
                        "isOptionalHoliday": optionalholiday,
                        "isDeleted": Status,
                        "HolidayCalendarProjectId": ClientHoldyPid
                    },
                    success: function (data) {
                        if (data === "HolidayName Already Exist") {
                            $("#HolidayName").addClass("validate_msg");
                            $("#HolidayDate").removeClass("validate_msg");
                        } else if (data === "HolidayDate Already Exist") {
                            $("#HolidayName").removeClass("validate_msg");
                            $("#HolidayDate").addClass("validate_msg");
                        } else if (data === "Please Select Correct financialyear") {
                            $("#HolidayName").removeClass("validate_msg");
                            $("#HolidayDate").removeClass("validate_msg");
                            $("#FinancialYearId").after("<label class='error'>" + data + "</label>");
                        } else {
                            alert(data);
                            $("#HolidayName").removeClass("validate_msg");
                            $("#HolidayDate").removeClass("validate_msg");
                            $('#Holidaycalendergrid').hide();
                            window.location.reload();
                            $(".modal-backdrop").hide();
                            $('#usertable').dataTable();
                            $('#holidaytable').dataTable();
                            $('#tasktable').dataTable();
                            $('#Projecttable').dataTable();
                            if (projid === "") {
                                usergriddata($("#editpid").val());
                                loadholidays($("#editpid").val());
                                loadtaks($("#editpid").val());
                                loadproject($("#editpid").val());
                            } else {
                                loadholidays(projid);
                                usergriddata(projid);
                                loadtaks(projid);
                                loadproject(projid);
                            }
                        }
                    },

                    error: function (error) {
                        console.log(error);
                    }

                });
                return false;
            }


        }
    });
    //$.validator.addMethod("regx", function (value, element, regexpr) {
    //        return regexpr.test(value);
    //    }, "Please enter  Only Alphabets .");
    $("#appendTable").find("input[name^='HolidayDate']").datepicker({

        format: 'mm/dd/yyyy',
        autoclose: true
    });
    loaddata();
    loadproject(editprojectid);
    GetUpdateClient(editprojectid);
    loadholidays(editprojectid);
    loadtaks(editprojectid);
    usergriddata($("#editpid").val());
    $("#btnSingleUpdate").hide();
    $("#Proj_ActiveStatus").val(1);

    $("#Proj_StartDate").datepicker({
        dateFormat: 'mm/dd/yyyy',
        autoclose: true
    });
    $("#Usrp_DOJ").datepicker({
        dateFormat: 'mm/dd/yyyy',
        autoclose: true
    });
    $("#Proj_StartDate").datepicker({
        dateFormat: 'mm/dd/yyyy',
        autoclose: true
    });

    $("#Proj_EndDate").datepicker({
        dateFormat: 'mm/dd/yyyy',
        autoclose: true
    });

    $("#UProj_StartDate").datepicker({
        dateFormat: 'mm/dd/yyyy',
        autoclose: true
    });

    $("#Usrp_DOJ").datepicker({
        dateFormat: 'mm/dd/yyyy',
        autoclose: true
    });
    $("#UProj_EndDate").datepicker({

        dateFormat: 'mm/dd/yyyy',
        autoclose: true
    });
    $("#Actual_StartDate").datepicker({
        dateFormat: 'mm/dd/yyyy',
        autoclose: true
    });
    $("#Actual_EndDate").datepicker({
        dateFormat: 'mm/dd/yyyy',
        autoclose: true
    });
    $("#Plan_StartDate").datepicker({
        dateFormat: 'mm/dd/yyyy',
        autoclose: true
    });
    $("#Plan_EndDate").datepicker({
        dateFormat: 'mm/dd/yyyy',
        autoclose: true

    });
    $("#btnAdd").show();
    $("#btnProject").hide();
    $("#btnSingleUpdate").hide();
    $("#btnManModel").hide();
    $("#btnEmpModel").hide();
    $("#btnHoliday").hide();
    $("#btnEdit").hide();
    $("#btnNewManModel").hide();
    $("#btnNewEmpModel").hide();

    //$("#image").click(function () {

    //    $("#fileUpload").trigger('click');

    //});
    $("#btnAddModel").click(function () {
        window.location.href = "/Client/Index?proid=" + 0;

        return false;

    });

    $("#btnAdd").on("click", function () {
        $('#clientform').validate({
            rules: {
                Proj_ProjectName: {
                    required: true
                },
                CountryId: {
                    required: true
                },
                StateId: {
                    required: true
                },
                WebUrl: {
                    required: true
                },
                Proj_isDeleted: {
                    required: true

                },

            },
            submitHandler: function (form) {
                add();

                return false;
            }
        });
        $.validator.addMethod("regx", function (value, element, regexpr) {
            return regexpr.test(value);
        }, "Please enter  Only Alphabets .");
        $.validator.addMethod("regex", function (value, element, regexpr) {
            return regexpr.test(value);
        }, "Please enter 10 Digit Number.");
        $.validator.addMethod("reg", function (value, element, regexpr) {
            return regexpr.test(value);
        }, "Password Must contain 1 Capital,1 small,1 number,1 Special Character and length must be 8 and above");




    });

    $("#CountryId").change(function () {
        countryChange();
    });
    $("#RoleName").change(function () {

        var roledata = $("#RoleName").val();


        GetAlltasknames(this.value);


    });
    $("#EmpRoleName").change(function () {

        var roledata = $("#EmpRoleName").val();


        GetAllEmptasknames(this.value);


    });
    $("#btnSingleUpdate").on("click", function () {
        $('#clientform').validate({
            rules: {
                Proj_ProjectName: {
                    required: true
                },
                CountryId: {
                    required: true
                },
                StateId: {
                    required: true
                },
                WebUrl: {
                    required: true
                },
                Proj_isDeleted: {
                    required: true

                },

            },
            submitHandler: function (form) {
                UpdateSingleRecord();

                return false;
            }
        });

    });

    $("#btnManModel").click(function () {

        $("#ContainerGridDetail").modal("show");
        var roledata = $("#RoleName").val();
        modelclick();
        $("#pwd").show();
        $("#cpwd").show();
        $("#ManagerName").empty();
        $("#manager1").hide();
        $("#Managername2").empty();
        $("#manager2").hide();
        $("#btnManAddModel").show();
        $("#btnEmpAddModel").hide();
        $("#btnUpdateModel").hide();
        $("#btnassociate").hide();
        $("#manager3").show();
        //bindNewManagers();
        bindProjectNames();
        bindRoleNames();
        bindUserNames();
        bindUserTypes();
        GetAlltasknames(roledata);
        $("#add").text('Add Manager');

        //Manger2();
        modelclick();
        getclientforproject();
        $("#UProj_ParticipationPercentage").val(0);
        // $("#btnassociateemp").hide();
        //$("#profile-image").prop('src', 'empty');
        $("#profile-image").prop('src', '/Content/images/local-disk.png');

        //$("#profile-image").removeAttr('src');


        $("#formanager").show();
        //$("#ISCilentHolidays").prop("checked", true);

        $("#IsDirectManager").prop("checked", true);
        // $("#TimesheetMode_id").val("1");
        // $("#TimesheetMode_id").removeAttr("disabled", "disabled")
    });

    $("#btnNewManModel").click(function () {

        $("#AddNewManager").modal("show");
        $("#AddSelectedManager").show();
        $("#btnEmpAddModel").hide();
        $("#btnManAddModel").hide();
        $("#ContainerGridDetail").modal("hide");
        $("#addtxt").text('Manager');

        //GetManagerDetails();  
    });

    $("#btnNewEmpModel").click(function () {


        $("#AddNewEmployee").modal("show");


        $("#ManagerforNewEmp").empty();


        //$("#SelectedMan2ForEmp").empty();
        $("#AddSelectedEmployee").show();
        $("#btnEmpAddModel").hide();
        $("#btnManAddModel").hide();
        $("#ContainerGridDetail").modal("hide");
        $("#addtxt1").text('Resource');
        var roledata = $("#EmpRoleName").val();

        //Manager5();

        bindProjectNames();
        GetRoleNamesbyemployee();
        GetAllEmptasknames(roledata);

    });

    $("#btnEmpModel").click(function () {
        $("#ContainerGridDetail").modal("show");
        $("#pwd").show();
        $("#cpwd").show();
        //$("#project").empty();
        $("#ManagerName").empty();
        $("#Managername2").empty();
        $("#project").not(':first').remove();
        $("#ManagerName").not(':first').remove();
        $("#Managername2").not(':first').remove();
        var roledata = $("#RoleName").val();
        modelclick();
        bindProjectNames();
        GetRoleNamesbyemp();
        bindUserNames();
        GetAlltasknames(roledata);
        bindUserTypes();
        //Manger2();
        getclientforproject();
        // $("#TimesheetMode_id").removeAttr("disabled", "disabled");
        $("#btnManAddModel").hide();
        $("#btnEmpAddModel").show();
        $("#btnUpdateModel").hide();
        $("#btnassociate").hide();
        $("#manager3").hide();
        $("#manager1").show();
        $("#manager2").show();

        $("#add").text('Add Resource');
        $("#formanager").hide();
        //$("#profile-image").prop('src', 'empty');
        //$("#profile-image").removeAttr('src');
        $("#profile-image").prop('src', '/Content/images/local-disk.png');
        // $("#ISCilentHolidays").prop("checked", true);
        $("#UProj_ParticipationPercentage").val(0);

        $("#ManagerName").append($("<option></option>").val("").html("Select Manager"));
        $("#Managername2").append($("<option></option>").val("").html("Select Manager"));
    });
    $("#btnclose").click(function () {
        $('#ContainerGridDetail').hide();
        $(".modal-backdrop").hide();
        window.location.reload();

    });
    $("#btnHoliday").click(function () {
        $("#Holidaycalendergrid").modal("show");
        Getfinancialyears();
        getclientforproject();
        $("#HolidayName").val("");
        $("#HolidayDate").val("");
        $("#HolidayCalendarProjectId").val("");
        $("#appendRow").show();
        $("#HolidaybtnAdd").show();
        $("#HolidaybtnUpdate").hide();


    });

    $("#hlbtnclose").click(function () {
        $('#holiwarningmsg').modal("show");
        return false;

    });
    $("#holicancel").click(function () {
        $('#Holidaycalendergrid').modal("hide");
        $('#holiwarningmsg').modal('hide');
        return false;
    });
    $("#closetask").click(function () {
        $('#Projectspecifictaskgrid').hide();
        window.location.reload();
        $(".modal-backdrop").hide();
    });
    $("#Closegrid").click(function () {
        $('#AssociateEmpGrid').hide();
        window.location.reload();
        $(".modal-backdrop").hide();
    });
    $("#Procancel").click(function () {
        //$('#ContainerGridProjectDetail').hide();
        $('#ContainerGridProjectDetail').modal("hide");
        //$('#prowarningmsg').hide();
        $('#prowarningmsg').modal('hide');
        //$(".modal-backdrop").hide();
        //$("body").removeClass('modal-open');
        return false;
    });
    $("#empcancel").click(function () {
        $('#ContainerGridDetail').modal("hide");
        $('#AddNewManager').modal("hide");
        $('#AddNewEmployee').modal("hide");
        $('#empwarningmsg').modal('hide');
        return false;
    });
    $("#btnwarningmsg").click(function () {
        $('#prowarningmsg').modal('show');
        return false;
    });
    $("#empclse").click(function () {
        
        //$("#project").empty();
        $("#project option").not(':first').remove();
        $('#empwarningmsg').modal('show');
        setTimeout(function () {


            window.location.reload();


        }, 500);
        return false;
        //

    });

    $("#empclsemdl").click(function () {
        $('#empwarningmsg').modal('show');
        return false;
    });
    $("#empclsemdl1").click(function () {
        $('#empwarningmsg').modal('show');
        return false;
    });
    $(".modal").on("shown.bs.modal", function () {
        if ($(".modal-backdrop").length > 1) {
            $(".modal-backdrop").not(':first').remove();
        }
    });
    $("#Back").click(function () {
        window.location.href = "/Project/Index";
        return false;
    });

    //$("#btnUpdateModel").on("click", function () {

    //                updategrid();


    //});

    $("input:checkbox").on('click', function () {
        // in the handler, 'this' refers to the box clicked on

        var $box = $(this);
        if ($box.is(":checked")) {
            // the name of the box is retrieved using the .attr() method
            // as it is assumed and expected to be immutable
            var group = "input:checkbox[name='" + $box.attr("name") + "']";
            // the checked state of the group/box on the other hand will change
            // and the current value is retrieved using .prop() method
            $(group).prop("checked", false);
            $box.prop("checked", true);
        } else {
            $box.prop("checked", false);
        }
    });

    $("#HolidaybtnUpdate").click(function () {


        $('#holidayform').validate({
            rules: {
                HolidayName: {
                    required: true,
                    regx: /[^-\s][a-zA-Z_\s-]+/
                },
                HolidayDate: {
                    required: true,
                    date: true
                },
                FinancialYearId: {
                    required: true
                },
                HolidayCalendarProjectId: {
                    required: true
                },
                isOptionalHoliday: {
                    required: true
                },
                isDeleted: {
                    required: true

                },

            },
            submitHandler: function (form) {

            }
        });
        $.validator.addMethod("regx", function (value, element, regexpr) {
            return regexpr.test(value);
        }, "Please enter  Only Alphabets .");
        $.validator.addMethod("regex", function (value, element, regexpr) {
            return regexpr.test(value);
        }, "Please enter 10 Digit Number.");
        $.validator.addMethod("reg", function (value, element, regexpr) {
            return regexpr.test(value);
        }, "Password Must contain 1 Capital,1 small,1 number,1 Special Character and length must be 8 and above");


    });



    //$("#ManagerName").change(function () {
    //    var ManagerID = $("#ManagerName").val();
    //    var Client_ProjId = $("#project").val();
    //    //alert(Client_ProjId);
    //    GetManagerOnChange(ManagerID, Client_ProjId);
    //    //GetManagerforProjects(ManagerID, Client_ProjId);
    //});

    $("#ManagerforNewEmp").change(function () {
        var ManagerID = $("#ManagerforNewEmp").val();
        GetManagerOnChange(ManagerID);
    });

    $("#project").change(function () {

        $("#Managername2 option").not(':first').remove();
        var CL_ProjectId = $("#project").val();
        var accid = $("#Prj_AccountId").val();
        GetManager1onProjectChange(CL_ProjectId);
        LeadforManager(accid, CL_ProjectId);
        //alert(accid);

    });







    $("#btntasks").click(function () {
        $("#Addprojecttask").show();
        $("#Updatetask").hide();
    });

    $("#Addprojecttask").click(function () {
        addtasks();
    });

    $("#Updatetask").click(function () {
        updatetasks();
    });
    var managertype;
    $('input[type=radio][name=optradio]').change(function () {
        if (this.value === 'rad1') {
            $("#managers").show();
            $("#employees").hide();
            $("#managertype").show();

            $('input[type=radio][name=optradio1]').change(function () {
                if (this.value === 'rad3') {
                    managertype = 1;
                } else {
                    managertype = 2;
                }

            });
            getmanagerlist();
        }
        else if (this.value === 'rad2') {
            $("#managers").hide();
            $("#employees").show();
            $("#managertype").hide();
            //$("#employeeslist").empty();
            getemployeelist();
        }
    });


    $("#empbtnassociate").click(function () {

        //var UProj_ActiveStatusVal = "";
        //if ($("#UProj_ActiveStatus").val() === "1") {

        //    UProj_ActiveStatusVal = true;
        //}
        //else {
        //    UProj_ActiveStatusVal = false;
        //}
        var file = document.getElementById("fileUpload").files[0];

        var formdata = new FormData();
        var projectid = projid;
        formdata.append("Usrp_ProfilePicture", file);
        formdata.append("Proj_ProjectID", projid);
        formdata.append("Email", $("#Email").val());
        formdata.append("Usr_UserID", Employeeid);
        formdata.append("Usr_Password", $("#Usr_Password").val());
        formdata.append("Usr_Username", $("#Usr_Username").val());
        formdata.append("Usr_ConfirmPassword", $("#Usr_ConfirmPassword").val());
        formdata.append("Usrp_MobileNumber", $("#Usrp_MobileNumber").val());
        formdata.append("Usr_Titleid", $("#Usr_Titleid").val());
        formdata.append("Usr_GenderId", $("#Usr_GenderId").val());
        formdata.append("UsrP_FirstName", $("#UsrP_FirstName").val());
        formdata.append("UsrP_LastName", $("#UsrP_LastName").val());
        formdata.append("UsrP_EmployeeID", $("#UsrP_EmployeeID").val());
        formdata.append("Usrp_DOJ", $("#Usrp_DOJ").val());
        formdata.append("Usr_UserTypeID", $("#Usr_UserTypeID").val());
        formdata.append("RoleName", $("#RoleName").val());
        formdata.append("ManagerName", $("#ManagerName").val());
        formdata.append("Managername2", $("#Managername2").val());
        formdata.append("Usr_TaskID", $("#Usr_TaskID").val());
        formdata.append("UProj_ParticipationPercentage", $("#UProj_ParticipationPercentage").val());
        formdata.append("UProj_StartDate", $("#UProj_StartDate").val());
        formdata.append("UProj_EndDate", $("#UProj_EndDate").val());
        //formdata.append("UProj_ActiveStatus", UProj_ActiveStatusVal);

        $.ajax({
            type: "POST",
            url: "/Client/AssociateEmployee",
            data: formdata,

            dataType: "json",
            complete: function (res) {
                // // 
                alert(res.responseText);
                // $('#ContainerGridDetail').modal('toggle');
                $('#ContainerGridDetail').hide();
                $(".modal-backdrop").hide();
                $('#usertable').dataTable();
                $('#holidaytable').dataTable();
                $('#tasktable').dataTable();
                $('#Projecttable').dataTable();
                if (projid === "") {
                    usergriddata($("#editpid").val());
                    loadholidays($("#editpid").val());
                    loadtaks($("#editpid").val());
                    loadproject($("#editpid").val());
                } else {
                    usergriddata(projid);
                    loadholidays(projid);
                    loadtaks(projid);
                    loadproject(projid);
                }
            },
            contentType: false,
            processData: false,
            error: function (Result) {

            }

        });
    });

    $("#manbtnassociate").click(function () {

        //var UProj_ActiveStatusVal = "";
        //if ($("#UProj_ActiveStatus").val() === "1") {

        //    UProj_ActiveStatusVal = true;
        //}
        //else {
        //    UProj_ActiveStatusVal = false;
        //}
        var file = document.getElementById("fileUpload").files[0];

        var formdata = new FormData();
        var projectid = projid;
        formdata.append("Usrp_ProfilePicture", file);
        formdata.append("Proj_ProjectID", projid);
        formdata.append("Email", $("#Email").val());
        formdata.append("Usr_UserID", Employeeid);
        formdata.append("Usr_Password", $("#Usr_Password").val());
        formdata.append("managertype", managertype);
        formdata.append("Usr_Username", $("#Usr_Username").val());
        formdata.append("Usr_ConfirmPassword", $("#Usr_ConfirmPassword").val());
        formdata.append("Usrp_MobileNumber", $("#Usrp_MobileNumber").val());
        formdata.append("Usr_Titleid", $("#Usr_Titleid").val());
        formdata.append("Usr_GenderId", $("#Usr_GenderId").val());
        formdata.append("UsrP_FirstName", $("#UsrP_FirstName").val());
        formdata.append("UsrP_LastName", $("#UsrP_LastName").val());
        formdata.append("UsrP_EmployeeID", $("#UsrP_EmployeeID").val());
        formdata.append("Usrp_DOJ", $("#Usrp_DOJ").val());
        formdata.append("Usr_UserTypeID", $("#Usr_UserTypeID").val());
        formdata.append("RoleName", $("#RoleName").val());
        formdata.append("ManagerName", $("#ManagerName").val());
        formdata.append("Managername2", $("#Managername2").val());
        formdata.append("Usr_TaskID", $("#Usr_TaskID").val());
        formdata.append("UProj_ParticipationPercentage", $("#UProj_ParticipationPercentage").val());
        formdata.append("UProj_StartDate", $("#UProj_StartDate").val());
        formdata.append("UProj_EndDate", $("#UProj_EndDate").val());
        //formdata.append("UProj_ActiveStatus", UProj_ActiveStatusVal);
        formdata.append("CL_ProjectID", $("#project").val());

        $.ajax({
            type: "POST",
            url: "/Client/AssociateManager",
            data: formdata,

            dataType: "json",
            complete: function (res) {
                // // 
                alert(res.responseText);
                // $('#ContainerGridDetail').modal('toggle');
                $('#ContainerGridDetail').hide();
                $(".modal-backdrop").hide();
                $('#usertable').dataTable();
                $('#holidaytable').dataTable();
                $('#tasktable').dataTable();
                $('#Projecttable').dataTable();
                if (projid === "") {
                    usergriddata($("#editpid").val());
                    loadholidays($("#editpid").val());
                    loadtaks($("#editpid").val());
                    loadproject($("#editpid").val());
                } else {
                    usergriddata(projid);
                    loadholidays(projid);
                    loadtaks(projid);
                    loadproject(projid);
                }
            },
            contentType: false,
            processData: false,
            error: function (Result) {

            }

        });
    });

    $("#btnProject").click(function () {

        $("#ContainerGridProjectDetail").modal("show");
        $("#btnupdateProj").hide();
        $("#btnAddProj").show();
        $("#editproj").hide();
        $("#addproj").show();
        $("#clientprojecttitle").val("");
        $("#clientprojectdescription").val("");
    });


    $('#btnAddProj').click(function () {


        //if (clientprojecttitle === "") {
        //    alert("Please Enter Project Name");
        //} else {
        $('#proForm').validate({
            rules: {
                clientprojecttitle: {
                    required: true
                }


            },
            submitHandler: function (form) {
                var clientprojecttitle = $("#clientprojecttitle").val();

                var clientprojdescription = $("#clientprojectdescription").val();
                $.ajax({

                    type: 'POST',
                    url: '/Client/CreateClientProject?client=' + projid,
                    data: {
                        "clientprojecttitle": clientprojecttitle, "clientprojdescription": clientprojdescription
                    },
                    success: function (data) {

                        if (data === "Project Name Already Existed") {
                            $("#clientprojecttitle").addClass("validate_msg");
                        } else {
                            alert(data);
                            // $('#ContainerGridDetail').modal('toggle');
                            //$('#ContainerGridProjectDetail').hide();
                            $('#ContainerGridProjectDetail').modal("hide");
                            //$(".modal-backdrop").hide();
                            //$("body").removeClass('modal-open');
                            $('#usertable').dataTable();
                            $('#holidaytable').dataTable();
                            $('#tasktable').dataTable();
                            $('#Projecttable').dataTable();
                            if (projid === "") {
                                usergriddata($("#editpid").val());
                                loadholidays($("#editpid").val());
                                loadtaks($("#editpid").val());
                                loadproject($("#editpid").val());
                            } else {
                                usergriddata(projid);
                                loadholidays(projid);
                                loadtaks(projid);
                                loadproject(projid);
                                window.location.href = "Index?proid=" + projid;

                            }
                        }




                    },
                    complete: function () {
                        //  $('#loading-image').attr("style", "display: none;");
                    },
                    error: function (error) {
                        console.log(error);
                    }

                });
                return false;
            }
        });

        //}
        // var client = editprojectid;



    });

    $("#Usr_Titleid").change(function () {

        var titleid = $(this).val();

        if (titleid === "1") {
            $("#Usr_GenderId").val(1);
        } else if (titleid === "2" || titleid === "3") {
            $("#Usr_GenderId").val(2);
        } else {
            $("#Usr_GenderId").val("");
        }

    });


    $("#btnupdateProj").click(function () {



        $('#proForm').validate({
            rules: {
                clientprojecttitle: {
                    required: true
                }


            },
            submitHandler: function (form) {
                var id = $("#Clientproid").val();

                var clientprojecttitle = $("#clientprojecttitle").val();

                var clientprojdescription = $("#clientprojectdescription").val();

                var accid = $("#accid").val();

                var projid = $("#projid").val();
                $.ajax({

                    type: 'POST',
                    url: '/Client/updatecp?client=' + id,
                    data: {
                        "clientprojecttitle": clientprojecttitle, "clientprojdescription": clientprojdescription, "projid": projid, "accid": accid
                    },
                    success: function (data) {

                        if (data === "Project Name Already Existed") {
                            $("#clientprojecttitle").addClass("validate_msg");
                        } else {
                            alert(data);
                            // $('#ContainerGridDetail').modal('toggle');
                            //$('#ContainerGridProjectDetail').hide();
                            $('#ContainerGridProjectDetail').modal('hide');
                            //$(".modal-backdrop").hide();
                            //$("body").removeClass('modal-open');
                            $('#usertable').dataTable();
                            $('#holidaytable').dataTable();
                            $('#tasktable').dataTable();
                            $('#Projecttable').dataTable();
                            if (projid === "") {
                                usergriddata($("#editpid").val());
                                loadholidays($("#editpid").val());
                                loadtaks($("#editpid").val());
                                loadproject($("#editpid").val());
                            } else {
                                usergriddata(projid);
                                loadholidays(projid);
                                loadtaks(projid);
                                loadproject(projid);
                                window.location.href = "Index?proid=" + projid;
                            }

                        }

                    },
                    complete: function () {
                        EditUserProject();//  $('#loading-image').attr("style", "display: none;");
                    },
                    error: function (error) {
                        console.log(error);
                    }

                });
                return false;
            }
        });

    });

    $("#btnEdit").click(function () {

        $("#Proj_ProjectName").removeAttr("disabled", "disabled");
        $("#CountryId").removeAttr("disabled", "disabled");
        $("#StateId").removeAttr("disabled", "disabled");
        $("#WebUrl").removeAttr("disabled", "disabled");
        $("#Proj_isDeleted").removeAttr("disabled", "disabled");
        $("#Proj_ProjectDescription").removeAttr("disabled", "disabled");
        $("#btnAdd").hide();
        $("#btnEdit").hide();
        $("#btnSingleUpdate").show();
    });
    $("#btnok").click(function () {
        $('#alertmodal').modal('hide');
        window.location.reload();

    });
});
function loaddata() {

    $.ajax({
        url: "/Project/GetProjectCollection",
        type: "Get",
        dataType: "json",
        success: function (res) {

            $('#table').DataTable({
                'data': res,
                'paginate': true,
                'sort': true,
                'Processing': true,
                'columns': [

                    { 'data': 'Proj_ProjectID', 'visible': false },
                    { 'data': 'Proj_ProjectCode' },
                    { 'data': 'Proj_ProjectName' },
                    {
                        'data': 'Proj_ProjectDescription', 'visible': false,
                        'render': function (data, type, row) {

                            if (data !== null) {
                                return data.substr(0, 15);
                            }
                            else {
                                return "";
                            }
                            //return data.substr(0, 15);
                        }
                    },



                    {
                        'data': 'Proj_isDeleted',
                        "render": function (Proj_isDeleted, type, full, meta) {
                            if (permissions === "Read/Write") {
                                if (Proj_isDeleted === true) {
                                    return '<div class="statuscheck"> <input type="checkbox" id="check_02"   onclick="CheckStatus(' + full.Proj_ProjectID + ')"> <label for="check_02"></label> </div>';

                                }
                                else {
                                    return '<div class="statuscheck"> <input type="checkbox" id="check_01" checked   onclick="UnCheckStatus(' + full.Proj_ProjectID + ')"> <label for="check_01"></label> </div>';
                                }
                            } else {
                                if (Proj_isDeleted === true) {
                                    return '<div class="statuscheck"> <input type="checkbox" id="check_02"   onclick="CheckStatus(' + full.Proj_ProjectID + ')" disabled> <label for="check_02"></label> </div>';

                                }
                                else {
                                    return '<div class="statuscheck"> <input type="checkbox" id="check_01" checked   onclick="UnCheckStatus(' + full.Proj_ProjectID + ')" disabled> <label for="check_01"></label> </div>';
                                }
                            }
                        }
                    },

                    {
                        "render": function (Proj_ProjectID, type, full, meta, data) {
                            // var permissions = '@ViewBag.a';
                            // perms = permissions;
                            if (permissions === "Read") {
                                // return '<a class="btn btn-icn" data-toggle="modal"  title="Edit"  id="edit"  onclick="EditUser(' + full.Proj_ProjectID + ')" ><i class="fa fa-edit"></i></a><a class="btn btn-icn" title="Delete" data-target="#containerDelete" data-toggle="modal" data-id="' + full.Proj_ProjectID + '"  onclick="DeleteSkill(' + full.Proj_ProjectID + ')" "><i class="fa fa-trash"></i></a>';
                                return '<a class="btn btn-icn btn-icn-hide " data-toggle="modal"  title="Edit"  id="edit"  onclick="Preview(' + full.Proj_ProjectID + ')" ><i class="fa fa-eye"></i></a>';

                            }
                            else if (permissions === "Read/Write") {
                                return '<a class="btn btn-icn btn-icn-hide  edit" data-toggle="modal"  title="Edit"  id="edit"  onclick="EditUser(' + full.Proj_ProjectID + ')" ><i class="fa fa-edit"></i></a><a class="btn btn-icn btn-icn-hide" title="Delete" style="display:none" data-target="#procontainerDelete" data-toggle="modal" data-id="' + full.Proj_ProjectID + '"  onclick="GetProId(' + full.Proj_ProjectID + ')" "><i class="fa fa-trash"></i></a>';

                            } else {
                                return '<a class="btn btn-icn  " data-toggle="modal"  title="Edit"  id="edit"  onclick="Preview(' + full.Proj_ProjectID + ')" ><i class="fa fa-eye"></i></a>';

                            }

                        },

                    },


                ]
            });

        },
        error: function (msg) {
            // alert(msg.responseText);
        }
    });
}

function loadproject(id) {
    $.ajax({
        url: "/Client/getclientprojects?projid=" + id,
        type: "Get",
        dataType: "json",
        success: function (res) {
            
            $('#Projecttable').DataTable({
                'data': res,
                'paginate': true,
                'sort': true,
                'Processing': true,
                'destroy': true,
                'columns': [

                    { 'data': 'Proj_ProjectID', visible: false, },
                    { 'data': 'Accountid', visible: false, },
                    { 'data': 'ClientProjTitle' },
                    { 'data': 'ClientProjDesc' },
                    {
                        "render": function (CL_ProjectID, type, full, meta, data) {
                            
                            console.log(full);
                          
                            if (usrpermission === "Read" && full.hasUsers == false) {
                                return '<a class="btn btn-icn " data-toggle="modal" style="display:none"  data-target="#ContainerGridProjectDetail" id="edit" data-rolID='
                                    + full.Usr_RoleID + ' title="Edit" onclick="EditClientProj(' + full.CL_ProjectID + ')" ><i class="fa fa-edit" aria-hidden="true"></i></a> || <a class="btn btn-icn btn-icn-hide" data-toggle="modal" data-target="#containerDelete" data-id="CL_ProjectID" onclick ="DeleteTableProject(' + full.CL_ProjectID + ',' + full.ProjectID+')"><i class="fa fa-trash" aria-hidden="true"></i></a> ';

                            } else if (usrpermission === "Read/Write" && full.hasUsers == false) {
                                return '<a class="btn btn-icn btn-icn-hide" data-toggle="modal"  data-target="#ContainerGridProjectDetail" id="edit" data-rolID=' + full.Usr_RoleID + ' title="Edit" onclick="EditClientProj(' + full.CL_ProjectID + ')" ><i class="fa fa-edit" aria-hidden="true"></i></a> &nbsp;&nbsp;&nbsp; ||<a class="btn btn-icn btn-icn-hide" data-toggle="modal" data-target="#containerDelete" data-id="CL_ProjectID" onclick ="DeleteTableProject(' + full.CL_ProjectID + ',' + full.ProjectID +')"><i class="fa fa-trash" aria-hidden="true"></i></a>';

                            }
                            else if (usrpermission === "Read/Write" && full.hasUsers == true) {
                                return '<a class="btn btn-icn btn-icn-hide" data-toggle="modal"  data-target="#ContainerGridProjectDetail" id="edit" data-rolID=' + full.Usr_RoleID + ' title="Edit" onclick="EditClientProj(' + full.CL_ProjectID + ')" ><i class="fa fa-edit" aria-hidden="true"></i></a>';

                            }
                            else {
                                //return '<a class="btn btn-icn " data-toggle="modal" style="display:none"  data-target="#ContainerGridProjectDetail" id="edit" data-rolID=' + full.Usr_RoleID + ' title="Edit" onclick="EditClientProj(' + full.CL_ProjectID + ')" ><i class="fa fa-edit" aria-hidden="true"></i></a>';
                                return "";
                            }


                        }
                    }

                ]
            });

        },
        error: function (msg) {
            // alert(msg.responseText);
        }
    });



}

function Getfinancialyears() {
    $("select[name^='FinancialYearId'][id*='FinancialYearId']").empty();
    //$("#FinancialYearId").empty();
    $.ajax({

        type: "GET",
        contentType: "application/json; charset=utf-8",
        url: "/Client/Getallfinancialyears",
        //data: "{}",
        dataType: "json",
        success: function (Result) {
            $(Result).each(function () {
                $("select[name^='FinancialYearId'][id*='FinancialYearId']").append($("<option></option>").val(this.FinancialYearId).html(this.StartDate));
            });
        },
        error: function (Result) {

        }



    });
}

function EditClientProj(id) {
    $("#btnupdateProj").show();
    $("#btnAddProj").hide();
    $("#editproj").show();
    $("#addproj").hide();

    $.ajax({

        url: '/Client/UpdateClientProject?id=' + id,
        type: 'Get',
        dataType: "Json",
        success: function (data) {
            //alert("OK");
            //   $("#ContainerGridProjectDetail").show();
            $("#accid").val(data.Accid);

            $("#projid").val(data.ProjectID)

            $("#Clientproid").val(data.Cl_projid);

            //   $("#accid").val(data.acc)
            $("#clientprojecttitle").val(data.clientprojecttitle);

            $("#clientprojectdescription").val(data.clientprojectdescription);

            //   $("#btnupdateProj").text('Update');


        },

        error: function () {
            //alert("No");
            alert(Response.text);
        }

    });

}


function UnCheckStatus(id) {
    $.ajax({
        url: "/Project/ChangeStatus",
        type: "POST",
        data: {
            'id': id,
            'status': true
        },

        //dataType: "json",
        success: function (data) {

            alert(data);
            window.location.reload();
        },
        complete: function () {
            $('#loading-image').attr("style", "display: none;");
        },
    });
}

function CheckStatus(id) {
    $.ajax({
        url: "/Project/ChangeStatus",
        type: "POST",
        data: {
            'id': id,
            'status': false
        },

        //dataType: "json",
        success: function (data) {

            alert(data);
            window.location.reload();
        },
        complete: function () {
            $('#loading-image').attr("style", "display: none;");
        },
    });
}

function GetProId(id) {
    delproid = id;
}

function SequenceClientCode() {
    var sequenceCode;
    $.ajax({
        type: "GET",
        url: "/Client/SequenceCode",
        dataType: "json",
        success: function (res) {
            $("#Proj_ProjectCode").val(res);
            $("#Proj_AccountID").val(Proj_AccountID);
        },

        error: function (Result) {

        }

    });
}




//function readURL(input) {

//    if (input.files && input.files[0]) {
//        var reader = new FileReader();
//        reader.onload = function (e) {
//            //$('#profile-image').css('background-image', 'url(' + e.target.result + ')');
//            $('#profile-image').prop('src', e.target.result);
//            //css('background-image', 'url(' + e.target.result + ')');
//            $('#profile-image').hide();
//            $('#profile-image').fadeIn(650);
//        }
//        reader.readAsDataURL(input.files[0]);
//    }
//}

function countryChange() {

    SelectedCountryId = $("#CountryId").val();
    $("#StateId").empty();
    $.ajax({

        type: "GET",
        contentType: "application/json; charset=utf-8",
        url: "/Client/GetStates?CountryId=" + SelectedCountryId,
        //data: "{}",
        dataType: "json",
        async: false,
        success: function (Result) {
            console.log(Result);
            $(Result).each(function () {

                $("#StateId").append($("<option></option>").val(this.StateId).html(this.StateName));

            });

        },

        error: function (Result) {

            // alert("Error");

        }

    });
}

function statebindfunction(countryid) {

    SelectedCountryId = $("#CountryId").val();
    $("#StateId").empty();
    $.ajax({

        type: "GET",
        contentType: "application/json; charset=utf-8",
        url: "/Client/GetStates?CountryId=" + countryid,
        //data: "{}",
        dataType: "json",
        async: false,
        success: function (Result) {
            console.log(Result);
            $(Result).each(function () {

                $("#StateId").append($("<option></option>").val(this.StateId).html(this.StateName));

            });

        },

        error: function (Result) {

            // alert("Error");

        }

    });
}

function dateConversion(value) {
    if (value === null) return "";
    var pattern = /Date\(([^)]+)\)/;
    var results = pattern.exec(value);
    var dt = new Date(parseFloat(results[1]));

    return dt.getDate() + "/" + (dt.getMonth() + 1) + "/" + dt.getFullYear();

}

function add() {

    //var Proj_ProjectCode = $("#Proj_ProjectCode").val();
    Proj_ProjectName = $("#Proj_ProjectName").val();
    var Proj_ProjectDescription = $("#Proj_ProjectDescription").val();
    // var Proj_StartDate = $("#Proj_StartDate").val();
    //  var Proj_EndDate = $("#Proj_EndDate").val();
    var Proj_isDeleted = $("#Proj_isDeleted").val();
    //var AccountName = $("#AccountName").val();
    var CountryId = $("#CountryId").val();
    var StateId = $("#StateId").val();
    var WebUrl = $("#WebUrl").val();
    //var Is_Timesheet_ProjectSpecific = $("#Is_Timesheet_ProjectSpecific").val();
    if ($('input[name="Is_Timesheet_ProjectSpecific"]').is(":checked")) {
        var Is_Timesheet_ProjectSpecific = true;
    }
    else if ($('input[name="Is_Timesheet_ProjectSpecific"]').is(":not(:checked)")) {
        Is_Timesheet_ProjectSpecific = false;
    }
    if (Proj_isDeleted === "1") {

        var Proj_ActiveStatusVal = true;
    }
    else {
        Proj_ActiveStatusVal = false;
    }
    $.ajax({
        type: 'POST',
        //contentType: "application/json; charset=utf-8",
        url: "/Client/CreateClient",
        data: {
            Proj_ProjectName: Proj_ProjectName, Proj_ProjectDescription: Proj_ProjectDescription,
            Proj_AccountID: Proj_AccountID, Proj_isDeleted: Proj_ActiveStatusVal,
            CountryId: CountryId, StateId: StateId, WebUrl: WebUrl, Is_Timesheet_ProjectSpecific: Is_Timesheet_ProjectSpecific
        },
        dataType: "json",

        success: function (res) {

            console.log(res.ProjectId);
            if (res.ProjectId === 0) {
                //  $("#Proj_ProjectName").addClass();
                $("#Proj_ProjectName").addClass("validate_msg");
                $("#btnAdd").show();
                $("#btnProject").hide();
                $("#btnSingleUpdate").hide();
                $("#btnManModel").hide();
                $("#btnEmpModel").hide();
                $("#btnHoliday").hide();
            } else {
                alert("Successfully Added");
                window.location.href = "Index?proid=" + res.ProjectId;
                //http://localhost:56305/Client/Index?proid=1724
                //projid = res.ProjectId;
                //usergriddata(projid);
                //loadholidays(projid);
                //loadtaks(projid);
                //loadproject(projid);
                //$("#btnAdd").hide();
                //$("#btnEdit").show();
                //$("#Proj_ProjectName").attr("disabled", "disabled");
                //$("#CountryId").attr("disabled", "disabled");
                //$("#StateId").attr("disabled", "disabled");
                //$("#WebUrl").attr("disabled", "disabled");
                //$("#Proj_isDeleted").attr("disabled", "disabled");
                //$("#Proj_ProjectDescription").attr("disabled", "disabled");

                //$("#UpdatebtnEdit").hide();
                //$("#btnSingleUpdate").hide();
                //$("#btnManModel").show();
                //$("#btnEmpModel").show();
                //$("#btnHoliday").show();
                //$("#btnProject").show();
                //$("#btnNewManModel").show();
                //$("#btnNewEmpModel").show();

            }

        },


        error: function (Result) {

            //alert("Error");

        }

    });

}

function UpdateSingleRecord() {

    //  var Proj_ProjectCode = $("#Proj_ProjectCode").val();
    Proj_ProjectName = $("#Proj_ProjectName").val();
    var Proj_ProjectDescription = $("#Proj_ProjectDescription").val();
    //var Proj_StartDate = $("#Proj_StartDate").val();
    // var Proj_EndDate = $("#Proj_EndDate").val();
    var Proj_isDeleted = $("#Proj_isDeleted").val();

    var CountryId = $("#CountryId").val();
    var StateId = $("#StateId").val();
    var WebUrl = $("#WebUrl").val();
    if ($('input[name="Is_Timesheet_ProjectSpecific"]').is(":checked")) {
        var Is_Timesheet_ProjectSpecific = true;
    }
    else if ($('input[name="Is_Timesheet_ProjectSpecific"]').is(":not(:checked)")) {
        Is_Timesheet_ProjectSpecific = false;
    }
    if (Proj_isDeleted === "1") {
        var Proj_ActiveStatusVal = true;
    }
    else {
        Proj_ActiveStatusVal = false;
    }
    $.ajax({

        type: "POST",
        // contentType: "application/json; charset=utf-8",
        url: "/Client/UpdateClient",
        data: {
            Proj_ProjectID: projid, Proj_ProjectName: Proj_ProjectName, Proj_ProjectDescription: Proj_ProjectDescription,
            Proj_AccountID: Proj_AccountID, Proj_isDeleted: Proj_ActiveStatusVal,
            CountryId: CountryId, StateId: StateId, WebUrl: WebUrl, Is_Timesheet_ProjectSpecific: Is_Timesheet_ProjectSpecific
        },
        dataType: "json",
        // cache: false,
        complete: function (res) {
            alert(res.responseText);
            $("#btnAdd").hide();
            $("#btnEdit").show();
            $("#btnSingleUpdate").hide();
            $("#Proj_ProjectName").attr("disabled", "disabled");
            $("#CountryId").attr("disabled", "disabled");
            $("#StateId").attr("disabled", "disabled");
            $("#WebUrl").attr("disabled", "disabled");
            $("#Proj_isDeleted").attr("disabled", "disabled");
            $("#Proj_ProjectDescription").attr("disabled", "disabled");
            //  window.location.href = "/UserType/Index";
            //loaddata();
        },

        error: function (Result) {

        }

    });
}



function bindManagersForNewEmp() {

    var accid = $("#Prj_AccountId").val();

    $.ajax({

        type: "GET",
        contentType: "application/json; charset=utf-8",
        url: "/Client/bindManagersForNewEmp?accid=" + accid,
        //data: "{}",
        dataType: "json",
        success: function (data) {
            $.each(data, function (i, value) {

                $("#ManagerforNewEmp").append('<option value="' + value.Usr_UserID + '">' + value.UsrP_FirstName + '</option>');
            });

        },

        error: function (Result) {


        }

    });
}

function bindProjectNames() {

    $.ajax({

        type: "GET",
        contentType: "application/json; charset=utf-8",
        url: "/Project/GetProjectNames",
        //data: "{}",
        dataType: "json",
        success: function (Result) {

            $(Result).each(function () {

                console.log(Result);
                $("#ProjectName").append($("<option></option>").val(this.Proj_ProjectID).html(this.Proj_ProjectName));

            });
        },
        error: function (Result) {

            //alert("Error");

        }

    });
}




function bindUserNames() {

    $.ajax({

        type: "GET",
        contentType: "application/json; charset=utf-8",
        url: "/ProjectAllocation/GetUserNames",
        //data: "{}",
        dataType: "json",
        success: function (Result) {

            $(Result).each(function () {

                $("#Username").append($("<option></option>").val(this.UProj_UserID).html(this.Username));

            });
        },
        error: function (Result) {



        }

    });
}

function bindRoleNames() {

    $("#RoleName").empty();
    $.ajax({

        type: "GET",
        // contentType: "application/json; charset=utf-8",
        url: "/Client/GetRoles"/*+ '&RoleId=' + RoleId*/,
        dataType: "json",
        async: false,
        success: function (Result) {

            $("#RoleName").append($("<option></option>").val("").html("Select Role"));

            $(Result).each(function () {

                $("#RoleName").append($("<option></option>").val(this.GenericRoleID).html(this.Title));


            });

        },
        error: function (Result) {

            // alert("Error");

        }

    });

    GetAlltasknames($("#RoleName").val());
}

function bindAllRoleNames() {
    $("#RoleName").empty();
    $.ajax({

        type: "GET",
        // contentType: "application/json; charset=utf-8",
        url: "/Client/GetAllRoles"  /*+ '&RoleId=' + RoleId*/,
        dataType: "json",
        async: false,
        success: function (Result) {
            $("#RoleName").append($("<option></option>").val("").html("Select Role"));
            $(Result).each(function () {

                $("#RoleName").append($("<option></option>").val(this.GenericRoleID).html(this.Title));


            });

        },
        error: function (Result) {

            // alert("Error");

        }

    });
}


function bindAllNewEmpRoleNames() {
    $("#EmpRoleName").empty();
    $.ajax({

        type: "GET",
        // contentType: "application/json; charset=utf-8",
        url: "/Client/GetAllNewRoles"  /*+ '&RoleId=' + RoleId*/,
        dataType: "json",
        async: false,
        success: function (Result) {
            //  $("#EmpRoleName").append($("<option></option>").val("").html("Select RoleName"));
            $(Result).each(function () {

                $("#EmpRoleName").append($("<option></option>").val(this.GenericRoleID).html(this.Title));


            });

        },
        error: function (Result) {

            // alert("Error");

        }

    });
    bindAllNewEmpRoleNames($("#EmpRoleName").val());
}

function bindUserTypes() {

    $("#Usr_UserTypeID").empty();

    $.ajax({

        type: "GET",
        // contentType: "application/json; charset=utf-8",
        url: "/User/GetUserTypesByAccountid",
        dataType: "json",
        async: true,
        success: function (Result) {

            $(Result).each(function () {

                $("#Usr_UserTypeID").append($("<option></option>").val(this.Usr_UserTypeID).html(this.UserType));

            });

        },
        error: function (Result) {

            // alert("Error");

        }

    });
}

function GetManagerByRole(userid, CL_ProjectId) {

    uid = userid;

    $.ajax({

        type: "GET",
        contentType: "application/json; charset=utf-8",
        url: "/Client/GetManagerByRole?projid=" + CL_ProjectId + "&userid=" + userid,
        dataType: "json",
        success: function (Result) {
            $("#ManagerName").empty();
            $("#Managername2").empty();
            $("#ManagerName").append($("<option></option>").val("").html("Select Manager"));
            $("#Managername2").append($("<option></option>").val("").html("Select Manager"));
            $(Result).each(function () {

                $("#ManagerName").append($("<option></option>").val(this.Usr_UserID).html(this.UsrP_FirstName));
                $("#Managername2").append($("<option></option>").val(this.Usr_UserID).html(this.UsrP_FirstName));


            });

        },

        error: function (Result) {

            // alert("Error");

        }

    });



}

function GetManagerOnChange(ManagerID, Client_ProjId) {

    $("#Managername2").empty();

    $.ajax({

        type: "GET",
        contentType: "application/json; charset=utf-8",
        url: "/Client/GetManagerOnChange?projid=" + projid + "&ManagerID=" + ManagerID + "&Client_ProjId=" + Client_ProjId  /*+ "&userid=" + uid*/,
        //data: "{}",
        dataType: "json",
        success: function (Result) {

            $(Result).each(function () {

                // $("#ManagerName").append($("<option></option>").val(this.Usr_UserID).html(this.UsrP_FirstName));
                $("#Managername2").append($("<option></option>").val(this.Usr_UserID).html(this.UsrP_FirstName));


            });

        },

        error: function (Result) {

            // alert("Error");

        }

    });



}

function GetManagerforProjects(userid, CL_ProjectId) {
    //$("#ManagerName").empty();
    //$("#Managername2").empty();


    //$.ajax({

    //    type: "GET",
    //    contentType: "application/json; charset=utf-8",
    //    url: "/Client/GetManager1onProjectChange?CL_ProjectId=" + CL_ProjectId,
    //    //data: "{}",
    //    dataType: "json",
    //    success: function (Result) {

    //        console.log(Result);
    //        $(Result).each(function () {

    //            // $("#ManagerName").append("--Select Manager--");
    //            $("#ManagerName").append($("<option></option>").val(this.Usr_UserID).html(this.UsrP_FirstName));
    //            $("#Managername2").append($("<option></option>").val(this.Usr_UserID).html(this.UsrP_FirstName));


    //        });

    //    },

    //    error: function (Result) {

    //        // alert("Error");

    //    }

    //});
    uid = userid;

    $.ajax({

        type: "GET",
        contentType: "application/json; charset=utf-8",
        url: "/Client/BindManagerfroProj?CL_ProjectId=" + CL_ProjectId + "&userid=" + userid,
        dataType: "json",
        success: function (Result) {
            $("#ManagerName").empty();
            $("#Managername2").empty();
            $("#ManagerName").append($("<option></option>").val("").html("Select Manager"));
            $("#Managername2").append($("<option></option>").val("").html("Select Manager"));
            $(Result).each(function () {

                $("#ManagerName").append($("<option></option>").val(this.Usr_UserID).html(this.UsrP_FirstName));
                $("#Managername2").append($("<option></option>").val(this.Usr_UserID).html(this.UsrP_FirstName));


            });

        },

        error: function (Result) {

            // alert("Error");

        }

    });
}
function GetNewManagerOnChange(ManagerID) {


    // $("#SelectedMan2ForEmp").empty();

    $.ajax({

        type: "GET",
        contentType: "application/json; charset=utf-8",
        url: "/Client/GetManagerOnChange?projid=" + projid + "&ManagerID=" + ManagerID /*+ "&userid=" + uid*/,
        //data: "{}",
        dataType: "json",
        success: function (Result) {

            $(Result).each(function () {

                $("#SelectedMan2ForEmp").append($("<option></option>").val(this.Usr_UserID).html(this.UsrP_FirstName));


            });

        },

        error: function (Result) {

            // alert("Error");

        }

    });



}

//function Manger2() {
//    $("#Manager1").empty();
//    $("#Manager2").empty();
//    $.ajax({

//        type: "GET",
//        contentType: "application/json; charset=utf-8",
//        url: "/Client/GetL2Manager?projid=" + projid,
//        //data: "{}",
//        dataType: "json",
//        success: function (Result) {
//            $("#Manager1").append($("<option></option>").val("").html("Select Manager"));
//            $("#Manager2").append($("<option></option>").val("").html("Select Manager"));

//            $(Result).each(function () {

//                $("#Manager1").append($("<option></option>").val(this.Usr_UserID).html(this.UsrP_FirstName));
//                $("#Manager2").append($("<option></option>").val(this.Usr_UserID).html(this.UsrP_FirstName));


//            });

//        },

//        error: function (Result) {

//            // alert("Error");

//        }

//    });
////}

//function Manger2() {

//        
//    //var CL_Projid = $("#project option:selected").val();
//    $("#ManagerName").empty();
//    $("#Managername2").empty();
//    $.ajax({

//        type: "GET",
//        contentType: "application/json; charset=utf-8",
//        url: "/Client/GetL2Manager?projid=" + projid ,
//        //data: "{}",
//        dataType: "json",
//        success: function (Result) {
//            $("#ManagerName").append($("<option></option>").val("").html("Select Manager"));
//            $("#Managername2").append($("<option></option>").val("").html("Select Manager"));

//            $(Result).each(function () {

//                $("#ManagerName").append($("<option></option>").val(this.Usr_UserID).html(this.UsrP_FirstName));
//                $("#Managername2").append($("<option></option>").val(this.Usr_UserID).html(this.UsrP_FirstName));


//            });

//        },

//        error: function (Result) {

//            // alert("Error");

//        }

//    });


//    }

//function Manager5() {
//    
//    $("#projmanager1").empty();
//    $("#projmanager2").empty();
//    $.ajax({

//        type: "GET",
//        contentType: "application/json; charset=utf-8",
//        url: "/Client/GetL2ManagerforNewEmp?projid=" + projid ,
//        //data: "{}",
//        dataType: "json",
//        success: function (Result) {
//            $("#projmanager2").append($("<option></option>").val("").html("Select Manager"));

//            $(Result).each(function () {

//                $("#projmanager1").append($("<option></option>").val(this.Usr_UserID).html(this.UsrP_FirstName));
//                $("#projmanager2").append($("<option></option>").val(this.Usr_UserID).html(this.UsrP_FirstName));


//            });

//        },

//        error: function (Result) {

//            // alert("Error");

//        }

//    });
//}


function getclientforproject() {

    //$("#project").empty();
    $("#project").not(':first').remove();
    $("#HolidayCalendarProjectId").empty();
    $.ajax({

        type: "GET",
        contentType: "application/json; charset=utf-8",
        url: "/Client/getclientforproject?projid=" + projid,
        //data: "{}",
        dataType: "json",
        success: function (Result) {

            console.log(Result);

            $(Result).each(function () {

                $("#project").append($("<option></option>").val(this.CL_ProjectID).html(this.ClientProjTitle));
                $("#HolidayCalendarProjectId").append($("<option></option>").val(this.CL_ProjectID).html(this.ClientProjTitle));
                //   $("#Managername2").append($("<option></option>").val(this.Usr_UserID).html(this.UsrP_FirstName));


            });

        },

        error: function (Result) {

            // alert("Error");

        }

    });
}




//function Manger1() {
//    
//    var Pid = $("#project option:selected").val();
//    alert(Pid);
//    $.ajax({

//        type: "GET",
//        contentType: "application/json; charset=utf-8",
//        url: "/Client/GetL2Manager?projid=" + projid /*+ "&cl_projectid=" + Pid*/,
//        //data: "{}",
//        dataType: "json",
//        success: function (Result) {
//            $("#Manager2").append($("<option></option>").val("").html("Select Manager"));
//            $(Result).each(function () {

//                // $("#ManagerName").append($("<option></option>").val(this.Usr_UserID).html(this.UsrP_FirstName));
//                $("#Manager2").append($("<option></option>").val(this.Usr_UserID).html(this.UsrP_FirstName));


//            });

//        },

//        error: function (Result) {

//            // alert("Error");

//        }

//    });
//}



function GetAlltasknames(id) {

    $.ajax({

        type: "GET",
        contentType: "application/json; charset=utf-8",
        url: "/Client/GetAlltasknames?projid=" + projid + "&Roleid=" + id,
        //data: "{}",
        dataType: "json",
        success: function (Result) {
            $("#Usr_TaskID").empty();
            $("#Usr_TaskID").append($("<option></option>").val("").html("Select Task"));
            $(Result).each(function () {
                $("#Usr_TaskID").append($("<option></option>").val(this.Proj_SpecificTaskId).html(this.Proj_SpecificTaskName));


            });

        },
        error: function (Result) {

            // alert("Error");

        }

    });
}

function GetAllEmptasknames(id) {
    $.ajax({

        type: "GET",
        contentType: "application/json; charset=utf-8",
        url: "/Client/GetAllEmptasknames?projid=" + projid + "&Roleid=" + id,
        //data: "{}",
        dataType: "json",
        success: function (Result) {
            $("#EmpTaskName").empty();
            $("#EmpTaskName").append($("<option></option>").val("").html("Select Task"));
            $(Result).each(function () {

                $("#EmpTaskName").append($("<option></option>").val(this.Proj_SpecificTaskId).html(this.Proj_SpecificTaskName));


            });

        },
        error: function (Result) {

            // alert("Error");

        }

    });
}

function gettaskanames() {
    $.ajax({

        type: "GET",
        contentType: "application/json; charset=utf-8",
        url: "/Client/gettasknames?projid=" + projid,
        //data: "{}",
        dataType: "json",
        success: function (Result) {
            $("#Usr_TaskID").empty();
            $("#Usr_TaskID").append($("<option></option>").val("").html("Select Task"));
            $(Result).each(function () {

                $("#Usr_TaskID").append($("<option></option>").val(this.Proj_SpecificTaskId).html(this.Proj_SpecificTaskName));


            });

        },

        error: function (Result) {

            // alert("Error");

        }

    });
}

function GetRoleNamesbyemp() {

    $("#RoleName").empty();
    $.ajax({

        type: "GET",
        // contentType: "application/json; charset=utf-8",
        url: "/Client/GetRoleNamesbyemp"  /*+ '&RoleId=' + RoleId*/,
        dataType: "json",
        async: false,
        success: function (Result) {
            $("#RoleName").append($("<option></option>").val("").html("Select RoleName"));
            $(Result).each(function () {

                $("#RoleName").append($("<option></option>").val(this.GenericRoleID).html(this.Title));

                //roleids = $("#RoleName").val(this.GenericRoleID);
            });

        },
        error: function (Result) {

        }

    });

    // GetAlltasknames($("#RoleName").val());
}

function GetRoleNamesbyemployee() {
    $("#EmpRoleName").empty();
    $.ajax({

        type: "GET",
        // contentType: "application/json; charset=utf-8",
        url: "/Client/GetRoleNamesbyemployee"  /*+ '&RoleId=' + RoleId*/,
        dataType: "json",
        async: false,
        success: function (Result) {

            $("#EmpRoleName").append($("<option></option>").val("").html("Select RoleName"));
            $(Result).each(function () {

                $("#EmpRoleName").append($("<option></option>").val(this.GenericRoleID).html(this.Title));

                //roleids = $("#RoleName").val(this.GenericRoleID);
            });

            //$.each(Result, function (data, value) {

            //    $("#EmpRoleName").append($("<option></option>").val(value.GenericRoleID).html(value.Title));
            //})  

        },
        error: function (Result) {

        }

    });


}



function modelclick() {
    $("#add").show();
    $("#UProj_L1_ManagerId").val(0);
    // $('input[type=checkbox]').prop('checked', false);
    // $("#Username option[value='option1']").remove();
    $("#Usr_Username").val(null);
    $("#Email").val(null);
    $("#Usr_Password").val(null);
    $("#Usr_ConfirmPassword").val(null);
    $("#Usr_Titleid").val(null);
    $("#UsrP_FirstName").val(null);

    $("#UsrP_LastName").val(null);
    $("#Usrp_MobileNumber").val(null);
    $("#Usrp_DOJ").val(null);
    $("#Usr_GenderId").val(null);
    $("#UsrP_EmployeeID").val(null);
    $("#UProj_ParticipationPercentage").val(null);
    $("#UProj_StartDate").val(null);
    $("#Proj_StartDate").val(null);
    $("#UProj_EndDate").val(null);
    $("#Usr_TaskID").val(null);
    $("#ProjectName").val(Proj_ProjectName);
    $("#ContainerGridDetail").show();
    $("#ContainerGridDetail").addClass('in');
    $("body").addClass('modal-open');
    //$(UProj_ActiveStatus).val(1);



    var L1Manager = $("#UProj_L1_ManagerId").val();
    var L2Manager = $("#UProj_L2_ManagerId").val();

}

function ManagerSaveData() {

    $("#AddNewManager").hide();
    var formdata = new FormData();
    var projectid = projid;
    var isdirectmanager = "";
    if ($("#IsDirectManager").is(":checked")) {
        isdirectmanager = true;
    }
    else if ($("#IsDirectManager2").is(":checked")) {
        isdirectmanager = false;
    }
    var clientcal = "";
    if ($("#ISCilentHolidays").is(":checked")) {
        clientcal = true;
    }
    else {
        clientcal = false;
    }

    var UProj_ActiveStatusVal = "";
    if ($("#Usr_isDeleted").val() === "1") {

        UProj_ActiveStatusVal = true;
    }
    else {
        UProj_ActiveStatusVal = false;
    }


    var file = document.getElementById("fileUpload").files[0];
    if (file == undefined) {
        formdata.append("Usrp_ProfilePicture", file);
        //formdata.append("imgCropped", '');
    }
    else {
        var filePath = file.name;
        var allowedExtensions = /(\.jpg|\.jpeg|\.png)$/i;
        if (!allowedExtensions.exec(filePath)) {

            $("#lblchvideoexits").show();
            return false;
        }
        else {

            $("#lblchvideoexits").hide();
            var imgSrc = $("#image_output").attr("src");

            formdata.append("Usrp_ProfilePicture", file);
            formdata.append("imgCropped", imgSrc);
        }

    }

    formdata.append("Proj_ProjectID", projid);
    formdata.append("Email", $("#Email").val());
    formdata.append("Usr_Password", $("#Usr_Password").val());
    formdata.append("Usr_Username", $("#Usr_Username").val());
    formdata.append("Usr_ConfirmPassword", $("#Usr_ConfirmPassword").val());
    formdata.append("Usrp_MobileNumber", $("#Usrp_MobileNumber").val());
    formdata.append("Usr_Titleid", $("#Usr_Titleid").val());
    formdata.append("Usr_GenderId", $("#Usr_GenderId").val());
    formdata.append("UsrP_FirstName", $("#UsrP_FirstName").val());
    formdata.append("UsrP_LastName", $("#UsrP_LastName").val());
    formdata.append("UsrP_EmployeeID", $("#UsrP_EmployeeID").val());
    formdata.append("Usrp_DOJ", $("#Usrp_DOJ").val());
    formdata.append("Usr_UserTypeID", $("#Usr_UserTypeID").val());
    formdata.append("RoleName", $("#RoleName").val());
    formdata.append("ManagerName", $("#ManagerName").val());
    formdata.append("Managername2", $("#Managername2").val());
    formdata.append("LeadforManager", $("#LeadforManager option:selected").val());
    formdata.append("Usr_TaskID", $("#Usr_TaskID").val());
    formdata.append("UProj_ParticipationPercentage", $("#UProj_ParticipationPercentage").val());
    formdata.append("UProj_StartDate", $("#UProj_StartDate").val());
    formdata.append("UProj_EndDate", $("#UProj_EndDate").val());
    formdata.append("Usr_isDeleted", UProj_ActiveStatusVal);
    formdata.append("CL_ProjectID", $("#project").val());
    formdata.append("TimesheetMode_id", $("#TimesheetMode_id").val());
    formdata.append("Usrp_CountryCode", $("#Usrp_CountryCode").val());
    formdata.append("IsDirectManager", isdirectmanager);
    formdata.append("isclientholidays", clientcal);

    $.ajax({
        type: "POST",
        url: "/Client/ManagerSave",
        data: formdata,

        dataType: "json",
        complete: function (res) {

            if (res.responseText === "UserName already Exists") {
                $("#Usr_Username").addClass("validate_msg");
                $("#Email").removeClass("validate_msg");

            } else if (res.responseText === "Loginid already Exists") {
                $("#Usr_Username").removeClass("validate_msg");
                $("#Email").addClass("validate_msg");
            }
            else {
                $("#Usr_Username").removeClass("validate_msg");
                $("#Email").removeClass("validate_msg");

                function explode() {
                    alert(res.responseText);
                }
                setTimeout(explode, 15);

                $('#ContainerGridDetail').hide();
                $(".modal-backdrop").hide();
                $("body").removeClass('modal-open');
                $('#usertable').dataTable();
                $('#holidaytable').dataTable();
                $('#tasktable').dataTable();
                $('#Projecttable').dataTable();
                if (projid === "") {
                    usergriddata($("#editpid").val());
                    loadholidays($("#editpid").val());
                    loadtaks($("#editpid").val());
                    loadproject($("#editpid").val());
                } else {
                    usergriddata(projid);
                    loadholidays(projid);
                    loadtaks(projid);
                    loadproject(projid);
                    window.location.href = "Index?proid=" + projid;
                }
                //  
            }


        },
        contentType: false,
        processData: false,
        error: function (Result) {
            console.log(Result);
        }

    });


}


function AddEmployee() {

    $("#AddNewEmployee").show();
    var formdata = new FormData();
    var projectid = projid;
    var UProj_ActiveStatusVal = "";
    if ($("#Usr_isDeleted").val() === "1") {

        UProj_ActiveStatusVal = true;
    }
    else {
        UProj_ActiveStatusVal = false;
    }
    var clientcal1 = "";
    if ($("#ISCilentHolidays").is(":checked")) {
        clientcal1 = true;
    }
    else {
        clientcal1 = false;
    }

    var file = document.getElementById("fileUpload").files[0];
    if (file == undefined) {
        formdata.append("Usrp_ProfilePicture", file);
        //formdata.append("imgCropped", '');
    }
    else {

        var filePath = file.name;
        var allowedExtensions = /(\.jpg|\.jpeg|\.png)$/i;
        if (!allowedExtensions.exec(filePath)) {

            $("#lblchvideoexits").show();
            return false;
        }
        else {

            $("#lblchvideoexits").hide();
            var imgSrc = $("#image_output").attr("src");

            formdata.append("Usrp_ProfilePicture", file);
            formdata.append("imgCropped", imgSrc);
        }

    }


    //formdata.append("Usrp_ProfilePicture", file);
    formdata.append("Proj_ProjectID", projid);

    formdata.append("Email", $("#Email").val());
    formdata.append("Usr_Password", $("#Usr_Password").val());
    formdata.append("Usr_Username", $("#Usr_Username").val());
    formdata.append("Usr_ConfirmPassword", $("#Usr_ConfirmPassword").val());
    formdata.append("Usrp_MobileNumber", $("#Usrp_MobileNumber").val());
    formdata.append("Usr_Titleid", $("#Usr_Titleid").val());
    formdata.append("Usr_GenderId", $("#Usr_GenderId").val());
    formdata.append("UsrP_FirstName", $("#UsrP_FirstName").val());
    formdata.append("UsrP_LastName", $("#UsrP_LastName").val());
    formdata.append("UsrP_EmployeeID", $("#UsrP_EmployeeID").val());
    formdata.append("Usrp_DOJ", $("#Usrp_DOJ").val());
    formdata.append("Usr_UserTypeID", $("#Usr_UserTypeID").val());
    formdata.append("RoleName", $("#RoleName").val());
    formdata.append("ManagerName", $("#ManagerName option:selected").val());
    formdata.append("Managername2", $("#Managername2 option:selected").val());
    formdata.append("Usr_TaskID", $("#Usr_TaskID").val());
    formdata.append("UProj_ParticipationPercentage", $("#UProj_ParticipationPercentage").val());
    formdata.append("UProj_StartDate", $("#UProj_StartDate").val());
    formdata.append("UProj_EndDate", $("#UProj_EndDate").val());
    formdata.append("Usr_isDeleted", UProj_ActiveStatusVal);
    formdata.append("CL_ProjectID", $("#project").val());
    formdata.append("TimesheetMode_id", $("#TimesheetMode_id").val());
    formdata.append("Usrp_CountryCode", $("#Usrp_CountryCode").val());
    formdata.append("isclientholidays", clientcal1);

    $.ajax({
        type: "POST",
        url: "/Client/SaveEmployee",
        data: formdata,

        dataType: "json",
        complete: function (res) {
            if (res.responseText === "UserName already Exists") {
                $("#Usr_Username").addClass("validate_msg");
                $("#Email").removeClass("validate_msg");

            } else if (res.responseText === "Loginid already Exists") {
                $("#Usr_Username").removeClass("validate_msg");
                $("#Email").addClass("validate_msg");
            } else {
                $("#Usr_Username").removeClass("validate_msg");
                $("#Email").removeClass("validate_msg");

                function explode() {

                    alert(res.responseText);
                    window.location.reload();
                }
                setTimeout(explode, 15);
                // window.location.reload();m
                // $('#ContainerGridDetail').modal('toggle');
                $('#ContainerGridDetail').hide();
                $(".modal-backdrop").hide();
                $("body").removeClass('modal-open');
                $('#usertable').dataTable();
                $('#holidaytable').dataTable();
                $('#tasktable').dataTable();
                $('#Projecttable').dataTable();
                if (projid === "") {
                    usergriddata($("#editpid").val());
                    loadholidays($("#editpid").val());
                    loadtaks($("#editpid").val());
                    loadproject($("#editpid").val());
                } else {
                    usergriddata(projid);
                    loadholidays(projid);
                    loadtaks(projid);
                    loadproject(projid);
                }

            }

        },
        contentType: false,
        processData: false,
        error: function (Result) {

        }

    });

}


function usergriddata(pid) {
    // // 
    // // 

    $.ajax({
        url: "/Client/GetProjectAllocationCollection?id=" + pid,
        type: "Get",
        dataType: "json",

        success: function (res) {


            $('#usertable').DataTable({
                'data': res,
                'paginate': true,
                'sort': true,
                'Processing': true,
                'destroy': true,
                'columns': [


                    { 'data': 'Usr_UserID', visible: false },
                    { 'data': 'Usr_RoleID', visible: false },
                    { 'data': 'UsrP_EmployeeID', visible: false },
                    { 'data': 'UsrP_FirstName' },

                    { 'data': 'project' },
                    { 'data': 'CL_ProjectId', visible: false },
                    { 'data': 'RoleName' },
                    { 'data': 'UProj_ParticipationPercentage' },
                    { 'data': 'Email', visible: false },
                    {
                        'data': 'UProj_StartDate', visible: false,
                        "type": "date ",
                        "render": function (value) {
                            if (value === null) return "";

                            //var pattern = /Date\(([^)]+)\)/;
                            //var results = pattern.exec(value);
                            // dt = new Date(parseFloat(results[1]));

                            //return (dt.getMonth() + 1) + "/" + dt.getDate() + "/" + dt.getFullYear();
                            return dateConversion(value);
                        }
                    },

                    {
                        'data': 'UProj_EndDate', visible: false,
                        "type": "date ",
                        "render": function (value) {
                            if (value === null) return "";

                            //var pattern = /Date\(([^)]+)\)/;
                            //var results = pattern.exec(value);
                            // dt = new Date(parseFloat(results[1]));

                            //return (dt.getMonth() + 1) + "/" + dt.getDate() + "/" + dt.getFullYear();
                            return dateConversion(value);
                        }
                    },
                    {
                        'data': 'Usr_isDeleted',
                        "render": function (Usr_isDeleted, type, full, meta) {

                            if (usrpermission === "Read/Write") {
                                if (Usr_isDeleted === true) {
                                    return '<div class="statuscheck"> <input type="checkbox" id="check_02" onclick="UsrCheckStatus(' + full.Usr_UserID + ')"> <label for="check_02"></label> </div>';
                                }
                                else {
                                    return '<div class="statuscheck"> <input type="checkbox" id="check_01" checked onclick="UsrUnCheckStatus(' + full.Usr_UserID + ')"> <label for="check_01"></label> </div>';

                                }
                            } else {
                                if (Usr_isDeleted === true) {
                                    return '<div class="statuscheck" > <input type="checkbox" class="read_only" id="check_02" onclick="UsrCheckStatus(' + full.Usr_UserID + ')" disabled> <label for="check_02"></label> </div>';
                                }
                                else {
                                    return '<div class="statuscheck" > <input type="checkbox" class="read_only" id="check_01" checked onclick="UsrUnCheckStatus(' + full.Usr_UserID + ')" disabled> <label for="check_01"></label> </div>';

                                }
                            }

                        }
                    },
                    {
                        "render": function (Usr_UserID, type, full, meta, data) {
                            if (usrpermission === "Read") {
                                return '<a class="btn btn-icn btn-icn-hide" data-toggle="modal" style="display:none"  data-target="#ContainerGridDetail" id="edit" data-rolID=' + full.Usr_RoleID + ' title="Edit" onclick="Userpreview(' + full.Usr_UserID + ',' + full.Usr_RoleID + ',\'' + full.RoleName + '\',' + full.CL_ProjectId + ')" ><i class="fa fa-eye" aria-hidden="true"></i></a>';
                                // return '<a class="btn btn-icn btn-icn-hide " data-toggle="modal"  title="Edit"  id="edit"  onclick="Preview(' + full.Proj_ProjectID + ')" ><i class="fa fa-eye"></i></a>';

                            }
                            else if (usrpermission === "Read/Write") {
                                return '<a class="btn btn-icn btn-icn-hide" data-toggle="modal"  data-target="#ContainerGridDetail" id="edit" data-rolID=' + full.Usr_RoleID + ' title="Edit" onclick="EditUserProject(' + full.Usr_UserID + ',' + full.Usr_RoleID + ',\'' + full.RoleName + '\', ' + full.CL_ProjectId + ')" ><i class="fa fa-edit" aria-hidden="true"></i></a><a class="btn btn-icn btn-icn-hide" data-target="#usercontainerDelete" style="display:none" data-toggle="modal"  data-id="' + full.Usr_UserID + '" title="Delete" onclick="GetUserid(' + full.Usr_UserID + ')" "><i class="fa fa-trash" aria-hidden="true"></i></a>';



                                // return '<a class="btn btn-icn btn-icn-hide  edit" data-toggle="modal"  title="Edit"  id="edit"  onclick="EditUser(' + full.Proj_ProjectID + ')" ><i class="fa fa-edit"></i></a><a class="btn btn-icn btn-icn-hide" title="Delete" data-target="#containerDelete" data-toggle="modal" data-id="' + full.Proj_ProjectID + '"  onclick="DeleteSkill(' + full.Proj_ProjectID + ')" "><i class="fa fa-trash"></i></a>';



                            }
                            else {
                                return '<a class="btn btn-icn " data-toggle="modal" style="display:none"  data-target="#ContainerGridDetail" id="edit" data-rolID=' + full.Usr_RoleID + ' title="Edit" onclick="Userpreview(' + full.Usr_UserID + ',' + full.Usr_RoleID + ',\'' + full.RoleName + '\',' + full.CL_ProjectId + ')" ><i class="fa fa-eye" aria-hidden="true"></i></a>';

                            }

                        },

                    },




                ]
            });
            function dateConversion(value) {
                if (value === null) return "";
                var pattern = /Date\(([^)]+)\)/;
                var results = pattern.exec(value);
                var dt = new Date(parseFloat(results[1]));

                return (dt.getMonth() + 1) + "/" + dt.getDate() + "/" + dt.getFullYear();

            }
        }


    });
}

function GetUserid(userid) {
    deluserid = userid;
}


function EditUserProject(userid, roleid, rolename, CL_ProjectId) {
    
    GetAlltasknames(roleid);
    bindProjectNames();
    //GetManagerforProjects(userid,CL_ProjectId);
    GetManager1onProjectChange(CL_ProjectId);
    ManagerOneChange(CL_ProjectId, userid);

    //bindProjectNamesById(CL_ProjectId);
    // bindAllRoleNames();
    if (rolename === "Manager") {
        bindRoleNames();

    } else {
        GetRoleNamesbyemp();
    }
    $("#pwd").hide();
    $("#cpwd").hide();
    //GetManagerByRole(userid, CL_ProjectId);

    bindUserNames();
    bindUserTypes();

    $("#add").text('Edit User');
    getclientforproject();

    $("#btnUpdateModel").show();
    //$("#ContainerGridDetail").show();
    $("#btnManAddModel").hide();
    $("#btnEmpAddModel").hide();
    $("#manbtnassociate").hide();
    $("#empbtnassociate").hide();

    var accid = $("#Prj_AccountId").val();
    LeadforManager(accid, CL_ProjectId);


    $.ajax({

        url: '/Client/GetUserDetailById?id=' + userid,
        type: 'Get',
        data: {
            "proid": projid,
            "CL_ProjID": CL_ProjectId
        },
        success: function (data) {

            var Usr_UserID = data.Usr_UserID;
            var Email = data.Email;
            var Usr_RoleID = data.Usr_RoleID;
            var man1 = data.ManagerName;
            var CL_ProjectId = data.CL_ProjectId;
            $.ajax({

                type: "GET",
                contentType: "application/json; charset=utf-8",
                url: "/Client/GetAlltasknames?projid=" + projid + "&Roleid=" + Usr_RoleID,
                //data: "{}",
                dataType: "json",
                success: function (Result) {


                    $("#Usr_TaskID").empty();
                    $("#Usr_TaskID").append($("<option></option>").val("").html("Select Task"));
                    $(Result).each(function () {

                        $("#Usr_TaskID").append($("<option></option>").val(this.Proj_SpecificTaskId).html(this.Proj_SpecificTaskName));


                    });
                    //$("#Usr_TaskID").val(Usr_TaskID).attr("disabled", "disabled");

                },
                error: function (Result) {

                    alert("Error");

                }

            });



            var UsrP_EmployeeID = data.UsrP_EmployeeID;
            Proj_ProjectID = data.Proj_ProjectID;
            var Usr_TaskID = data.Usr_TaskID;
            var Proj_ProjectName = data.Proj_ProjectName;
            var UProj_ParticipationPercentage = data.UProj_ParticipationPercentage;
            var UsrP_FirstName = data.UsrP_FirstName;
            var UsrP_LastName = data.UsrP_LastName;
            // var Usr_Password = data.Usr_Password;
            var Usr_Titleid = data.Usr_Titleid;
            var Usrp_MobileNumber = data.Usrp_MobileNumber;
            var Usr_GenderId = data.Usr_GenderId;
            var Usrp_ProfilePicture = data.Usrp_ProfilePicture;
            var Usr_UserTypeID = data.Usr_UserTypeID;

            var ManagerName = data.ManagerName;
            var Managername2 = data.Managername2;
            var LeadforManager = data.LeadforManager;
            var Usr_Username = data.Usr_Username;
            var TimesheetMode_id = data.TimesheetMode_id;
            var Usr_isDeleted = data.Usr_isDeleted;
            var isdirectmanager = data.isdirectmanager;
            var isclientHoliday = data.isclientholiday;

            var ClientProjDesc = data.ClientProjDesc;


            if (data.roleid === 1007 || data.roleid === 1006 || data.roleid === 1055) {
                $("#manager1").hide();
                $("#manager2").hide();
                $("#manager3").show();
                $("#formanager").show();
            } else {
                $("#manager1").show();
                $("#manager2").show();
                $("#formanager").hide();
                $("#formanager1").hide();
                $("#manager3").hide();
            }

            $("label[for='fileUpload']").children("span").html(Usrp_ProfilePicture);
            $("#Usr_UserID").val(Usr_UserID);
            $("#Usr_Username").val(Usr_Username);
            $("#Email").val(Email);
            $("#Usr_Password").val();
            $("#Usr_ConfirmPassword").val();
            $("#Usrp_MobileNumber").val(Usrp_MobileNumber);
            $("#Usr_Titleid").val(Usr_Titleid);
            $("#Usr_GenderId").val(Usr_GenderId);
            $("#UsrP_FirstName").val(UsrP_FirstName);
            $("#UsrP_LastName").val(UsrP_LastName);
            $("#UsrP_EmployeeID").val(UsrP_EmployeeID);
            $("#Usr_TaskID").val(Usr_TaskID);
            $("#TimesheetMode_id").val(TimesheetMode_id);
            $("#Usr_UserTypeID").val(Usr_UserTypeID);

            $("#RoleName").val(Usr_RoleID);
            $("#project").val(data.Cl_projid);


            $("#LeadforManager").val(LeadforManager);
            $('#Managername2 option[value=' + Managername2 + ']').attr('selected', 'selected');

            $('#LeadforManager option[value=' + LeadforManager + ']').attr('selected', 'selected');
            $('#project option[value=' + data.Cl_projid + ']').attr('selected', 'selected');

            $("#Usrp_CountryCode").val(data.Usrp_CountryCode);
            $("#UProj_ParticipationPercentage").val(UProj_ParticipationPercentage);
            $("#Usr_isDeleted").val(Usr_isDeleted);
            $("#IsDirectManager").val(isdirectmanager);

            var folderName = "/uploadimages/images/" + Usrp_ProfilePicture;

            $("#profile-image").prop('src', folderName);
            $("#profile-image").addClass('img-responsive');


            //bindNewManagers();

            if (Usr_isDeleted === true) {
                $("#Usr_isDeleted").val("1");
            } else {
                $("#Usr_isDeleted").val("0");
            }
            if (isdirectmanager === true) {
                $("#IsDirectManager").prop('checked', true);
                $("#IsDirectManager2").prop('checked', false);
            } else {
                $("#IsDirectManager").prop('checked', false);
                $("#IsDirectManager2").prop('checked', true);
            }
            if (isclientHoliday === true) {
                $("#ISCilentHolidays").prop('checked', true);
            } else {
                $("#ISCilentHolidays").prop('checked', false);
            }

            setTimeout(function () {

                //$("#ContainerGridDetail").show();
                $("#ContainerGridDetail").modal("show");

                $("#project").children("option[value = '" + CL_ProjectId + "']").attr("selected", "selected");
                $("#Usr_TaskID").children("option[value = '" + Usr_TaskID + "']").attr("selected", "selected");
                $("#ManagerName").children("option[value = '" + ManagerName + "']").attr("selected", "selected");
                $("#Managername2").children("option[value = '" + Managername2 + "']").attr("selected", "selected");
                $("#LeadforManager").children("option[value = '" + LeadforManager + "']").attr("selected", "selected");



            }, 1000);
        },
        error: function () {
            alert(Response.text);
        }
    });
}


function ManagerOneChange(CL_ProjectId, userid) {

    uid = userid;

    $.ajax({

        type: "GET",
        contentType: "application/json; charset=utf-8",
        url: "/Client/BindManagerforProj?CL_ProjectId=" + CL_ProjectId + "&userid=" + userid,
        dataType: "json",
        success: function (Result) {
            $("#Managername2").empty();

            $("#Managername2").append($("<option></option>").val("").html("Select Manager"));
            $(Result).each(function () {


                $("#Managername2").append($("<option></option>").val(this.Usr_UserID).html(this.UsrP_FirstName));


            });

        },

        error: function (Result) {

            // alert("Error");

        }

    });


}
function LeadforManager(accid, CL_ProjectId) {


    $("#LeadforManager option").not(':first').remove();

    $.ajax({

        type: "GET",
        contentType: "application/json; charset=utf-8",
        url: "/Client/GetallNewManagersList?accid=" + accid + "&clientprjid=" + CL_ProjectId,
        //data: "{}",
        dataType: "json",
        success: function (data) {
            $(data).each(function () {


                $("#LeadforManager").append($("<option></option>").val(this.Usr_UserID).html(this.UsrP_FirstName));


            });


        },

        error: function (Result) {

        }

    });


}
function GetManager1onProjectChange(CL_ProjectId) {


    $("#ManagerName").empty();

    $.ajax({

        type: "GET",
        contentType: "application/json; charset=utf-8",
        url: "/Client/GetManager1onProjectChange?CL_ProjectId=" + CL_ProjectId,
        //data: "{}",
        dataType: "json",
        success: function (Result) {

            console.log(Result);
            $(Result).each(function () {

                // $("#ManagerName").append("--Select Manager--");
                $("#ManagerName").append($("<option></option>").val(this.Usr_UserID).html(this.UsrP_FirstName));


            });

        },

        error: function (Result) {

            // alert("Error");

        }

    });



}
function Userpreview(userid, roleid, rolename, CL_ProjectId) {

    // 
    //$("#ManagerName").empty();
    //$("#Managername2").empty();
    var roledata = $("#RoleName").val();
    GetAlltasknames(roleid);
    bindProjectNames();
    //  bindAllRoleNames();
    if (rolename === "Manager") {
        bindRoleNames();

    } else {
        GetRoleNamesbyemp();
    }
    GetManager1onProjectChange(CL_ProjectId);
    // gettaskanames();
    bindUserNames();
    bindUserTypes();
    //Manger2();
    $("#add").text('Edit User');
    getclientforproject();
    //GetManagerByRole(userid);
    // $("#btnUpdateModel").show();
    $("#btnManAddModel").hide();
    $("#btnEmpAddModel").hide();




    // $("#btnassociateemp").hide();
    $.ajax({

        url: '/Client/GetUserDetailById?id=' + userid,
        type: 'Get',
        data: {
            "proid": projid
        },
        success: function (data) {
            //$("#profile-image").attr("src", "/Content/images/local-disk.png");
            //$("#profile-image").removeClass("img-responsive");
            //$("label[for='fileUpload']").next("span").text("Choose file from local disk");
            //$("#profile-image").next("div.cropper-container").remove();
            //$("#lblchvideoexits").hide();
            //$('#image_input > img.file-upload-image').attr('src', '/Content/images/local-disk.png');
            //$('#image_input > img.file-upload-image').removeAttr('style');
            //$("#image_input > .jcrop-holder").remove();
            //$('#image_input > img.file-upload-image').show();
            //$('#image_input > img.file-upload-image').css("visibility", "visible");
            var Usr_UserID = data.Usr_UserID;
            var Email = data.Email;
            var Usr_RoleID = data.Usr_RoleID;
            // alert(Usr_RoleID);
            var UsrP_EmployeeID = data.UsrP_EmployeeID;
            Proj_ProjectID = data.Proj_ProjectID;
            var Proj_ProjectName = data.Proj_ProjectName;
            var UProj_ParticipationPercentage = data.UProj_ParticipationPercentage;
            var UsrP_FirstName = data.UsrP_FirstName;
            var UsrP_LastName = data.UsrP_LastName;
            var Usr_Password = data.Usr_Password;
            var Usr_Titleid = data.Usr_Titleid;
            var Usrp_MobileNumber = data.Usrp_MobileNumber;
            var Usr_GenderId = data.Usr_GenderId;
            var Usrp_ProfilePicture = data.Usrp_ProfilePicture;
            var Usr_UserTypeID = data.Usr_UserTypeID;
            var Usr_TaskID = data.Usr_TaskID;
            var ManagerName = data.ManagerName;
            var Managername2 = data.Managername2;
            var Usr_Username = data.Usr_Username;
            var isdirectmanager = data.isdirectmanager;
            var TimesheetMode_id = data.TimesheetMode_id;
            //var UProj_ActiveStatus = data.UProj_ActiveStatus;
            if (isdirectmanager === true) {
                $("#IsDirectManager").prop('checked', true);
            } else {
                $("#IsDirectManager2").prop('checked', false);
            }

            $("#project").val(data.Cl_projid);
            $("#Usr_UserID").val(Usr_UserID);
            $("#Usr_Username").val(Usr_Username);
            $("#Email").val(Email);
            $("#Usr_Password").val(Usr_Password);
            $("#Usr_ConfirmPassword").val(Usr_Password);
            $("#Usrp_MobileNumber").val(Usrp_MobileNumber);
            $("#Usr_Titleid").val(Usr_Titleid);
            $("#Usr_GenderId").val(Usr_GenderId);
            $("#UsrP_FirstName").val(UsrP_FirstName);
            $("#UsrP_LastName").val(UsrP_LastName);
            $("#UsrP_EmployeeID").val(UsrP_EmployeeID);
            $("#Usrp_CountryCode").val(data.Usrp_CountryCode);
            $("#TimesheetMode_id").val(TimesheetMode_id);
            $("#Usr_UserTypeID").val(Usr_UserTypeID);
            $("#RoleName").val(Usr_RoleID);
            $("#ManagerName").val(ManagerName);
            $("#Managername2").val(Managername2);
            $("#Usr_TaskID").val(Usr_TaskID);
            $("#UProj_ParticipationPercentage").val(UProj_ParticipationPercentage);
            $("#IsDirectManager").val(isdirectmanager);

            //bindNewManagers();
            $('#ManagerName option[value=' + ManagerName + ']').prop('selected', true);
            var folderName = "/uploadimages/images/" + Usrp_ProfilePicture;

            $("#profile-image").prop('src', folderName);

            $("#profile-image").addClass('img-responsive');
        },
        error: function () {
            alert(Response.text);
        }
    });
}

function updategrid() {

    var formdata = new FormData();
    var isdirectmanager = "";
    if ($("#IsDirectManager").is(":checked")) {
        isdirectmanager = true;
    }
    else if ($("#IsDirectManager").is(":not(:checked)")) {
        isdirectmanager = false;
    }
    var UProj_ActiveStatusVal = "";
    if ($("#Usr_isDeleted").val() === "1") {

        UProj_ActiveStatusVal = true;
    }
    else {
        UProj_ActiveStatusVal = false;
    }
    var isClientCalendar = "";
    if ($("#ISCilentHolidays").is(":checked")) {
        isClientCalendar = true;
    }
    else if ($("#ISCilentHolidays").is(":not(:checked)")) {
        isClientCalendar = false;
    }


    var imagename = $("label[for='fileUpload']").children("span").html();
    var imgSrc = $("#image_output").attr("src");
    var file = document.getElementById("fileUpload").files[0];
    if (file == undefined) {
        //formdata.append("Usrp_ProfilePicture", file);
        //formdata.append("imgCropped", '');
        formdata.append("Usrp_ProfilePicture", imagename);
        formdata.append("imgCropped", imgSrc);
    }
    else {

        var filePath = file.name;
        var allowedExtensions = /(\.jpg|\.jpeg|\.png)$/i;
        if (!allowedExtensions.exec(filePath)) {

            $("#lblchvideoexits").show();
            return false;
        }
        else {

            $("#lblchvideoexits").hide();


            formdata.append("Usrp_ProfilePicture", file);
            formdata.append("imgCropped", imgSrc);
        }

    }




    var enddate = $("#UProj_EndDate").val();

    formdata.append("Usr_UserID", $("#Usr_UserID").val());

    formdata.append("Proj_ProjectID", Proj_ProjectID);
    formdata.append("Email", $("#Email").val());
    formdata.append("Usr_Password", $("#Usr_Password").val());
    formdata.append("Usr_Username", $("#Usr_Username").val());
    formdata.append("Usr_ConfirmPassword", $("#Usr_ConfirmPassword").val());
    formdata.append("Usrp_MobileNumber", $("#Usrp_MobileNumber").val());
    formdata.append("Usr_Titleid", $("#Usr_Titleid").val());
    formdata.append("Usr_GenderId", $("#Usr_GenderId").val());
    formdata.append("UsrP_FirstName", $("#UsrP_FirstName").val());
    formdata.append("UsrP_LastName", $("#UsrP_LastName").val());
    formdata.append("UsrP_EmployeeID", $("#UsrP_EmployeeID").val());
    formdata.append("Usrp_DOJ", $("#Usrp_DOJ").val());
    formdata.append("Usr_UserTypeID", $("#Usr_UserTypeID").val());
    formdata.append("RoleName", $("#RoleName").val());
    formdata.append("ManagerName", $("#ManagerName option:selected").val());
    formdata.append("LeadforManager", $("#LeadforManager option:selected").val());


    formdata.append("Managername2", $("#Managername2 option:selected").val());
    formdata.append("Usr_TaskID", $("#Usr_TaskID").val());
    formdata.append("UProj_ParticipationPercentage", $("#UProj_ParticipationPercentage").val());
    formdata.append("UProj_StartDate", $("#UProj_StartDate").val());
    formdata.append("UProj_EndDate", enddate);
    formdata.append("Usr_isDeleted", UProj_ActiveStatusVal);
    formdata.append("CL_ProjectID", $("#project").val());
    formdata.append("TimesheetMode_id", $("#TimesheetMode_id").val());
    formdata.append("Usrp_CountryCode", $("#Usrp_CountryCode").val());
    formdata.append("IsDirectManager", isdirectmanager);
    formdata.append("isclientholidays", isClientCalendar);
    $.ajax({
        type: "POST",
        url: "/Client/updateuserprojectbyid",
        data: formdata,

        dataType: "json",
        complete: function (res) {
            if (res.responseText === "UserName already Exists") {
                $("#Usr_Username").addClass("validate_msg");
                $("#Email").removeClass("validate_msg");

            } else if (res.responseText === "Loginid already Exists") {
                $("#Usr_Username").removeClass("validate_msg");
                $("#Email").addClass("validate_msg");
            } else {
                $("#Usr_Username").removeClass("validate_msg");
                $("#Email").removeClass("validate_msg");
                alert(res.responseText);
                //$("#project").not(':first').remove();
                $("#project").empty();
                $('#ContainerGridDetail').hide();
                //  window.location.reload();
                //$('#ContainerGridDetail').modal("toggle");
                $(".modal-backdrop").hide();
                $("body").removeClass('modal-open');
                $('#usertable').dataTable();
                usergriddata(projid);
                loadholidays(projid);
                loadtaks(projid);
                loadproject(projid);
            }
        },
        contentType: false,
        processData: false,
        error: function (Result) {

        }

    });

}

function EditUser(projectid) {
    editprojectid = projectid;
    window.location.href = "/Client/Index?proid=" + projectid;
    //  window.location.href = "/Client/EditIndex?proid=" + projectid;
    return false;
}
function Preview(projectid) {
    //  // 
    editprojectid = projectid;
    window.location.href = "/Client/Index?proid=" + projectid;
    $("#btnProject").hide();
    $("#btnManModel").hide();
    $("#btnEmpModel").hide();
    $("#btnHoliday").hide();
    // $("btnNewEmpModel").hide();
    //  window.location.href = "/Client/EditIndex?proid=" + projectid;

    return false;
}



function GetUpdateClient(editprojectid) {


    var currentprojectid = $("#editpid").val();
    projid = editprojectid;
    $.ajax({

        type: "POST",

        url: '/Client/GetClientByID?catID=' + currentprojectid,
        dataType: "json",
        success: function (data) {

            statebindfunction(data.CountryId);
            if (perms === "Read") {
                $("#btnSingleUpdate").hide();
            } else if (perms === "Read/Write") {
                $("#btnSingleUpdate").hide();
                $("#btnProject").show();

                $("#btnManModel").show();
                $("#btnEmpModel").show();
                $("#btnHoliday").show();
                $("#btnNewManModel").show();
                $("#btnNewEmpModel").show();

            } else {
                $("#btnSingleUpdate").hide();
                $("#btnProject").hide();

                $("#btnManModel").hide();
                $("#btnEmpModel").hide();
                $("#btnHoliday").hide();
            }
            $("#Proj_ProjectName").attr("disabled", "disabled");
            $("#CountryId").attr("disabled", "disabled");
            $("#StateId").attr("disabled", "disabled");
            $("#WebUrl").attr("disabled", "disabled");
            $("#Proj_isDeleted").attr("disabled", "disabled");
            $("#Proj_ProjectDescription").attr("disabled", "disabled");
            $("#btnAdd").hide();
            if (usrpermission !== "Read") {
                $("#btnEdit").show();
            }

            $("#UpdatebtnEdit").hide();
            $("#Proj_StartDate").val("");

            //

            //
            //
            //
            $("#Proj_EndDate").val("");

            var Proj_AccountID = data.Proj_AccountID;

            var AccountName = data.AccountName;

            var Proj_ProjectID = data.Proj_ProjectID;

            // var Proj_ProjectCode = data.Proj_ProjectCode;

            var Proj_ProjectName = data.Proj_ProjectName;

            var Proj_ProjectDescription = data.Proj_ProjectDescription;

            var CountryId = data.CountryId;

            var StateId = data.StateId;

            var WebUrl = data.WebUrl;
            var Is_Timesheet_ProjectSpecific = data.Is_Timesheet_ProjectSpecific;
            if (Is_Timesheet_ProjectSpecific === true) {
                $("#Is_Timesheet_ProjectSpecific").prop('checked', true);
            } else {
                $("#Is_Timesheet_ProjectSpecific").prop('checked', false);
            }
            if (data.Proj_StartDate !== null) {
                var StartDate = (data.Proj_StartDate).substr(6);
            }

            if (data.Proj_EndDate !== null) {
                var EndDate = (data.Proj_EndDate).substr(6);
            }




            var Proj_StartDate = new Date(parseInt(StartDate));

            var Proj_EndDate = new Date(parseInt(EndDate));


            var Proj_isDeleted = data.Proj_isDeleted;


            if (Proj_isDeleted === true) {
                $("#Proj_isDeleted").val("1");
            } else {
                $("#Proj_isDeleted").val("0");
            }


            $("#AccountName").val(AccountName);

            $("#Proj_AccountID").val(Proj_AccountID);

            $("#Proj_ProjectID").val(Proj_ProjectID);

            // $("#Proj_ProjectCode").val(Proj_ProjectCode);

            $("#Proj_ProjectName").val(Proj_ProjectName).attr("disabled", "disabled");

            $("#Proj_ProjectDescription").val(Proj_ProjectDescription);

            $("#Proj_StartDate").val(Proj_StartDate.format('mm/dd/yyyy'));

            $("#Proj_EndDate").val(Proj_EndDate.format('mm/dd/yyyy'));

            $("#CountryId").val(CountryId);

            $("#StateId").val(StateId);

            $("#WebUrl").val(WebUrl);

        }
    });

}

function DeleteSkill(Proj_ProjectID) {

    $.ajax({

        type: "POST",
        url: '/Project/DeleteProject?ProjectID=' + Proj_ProjectID,
        dataType: "json",

        complete: function (res) {
            alert(res.responseText);
            window.location.reload();

        },

        error: function (Result) {

        }

    });

}
function Deleteuser(Usr_UserID) {

    $.ajax({

        type: "POST",
        url: '/Client/DeleteUser?userid=' + Usr_UserID,
        dataType: "json",
        //   cache: false,
        complete: function (res) {

            alert(res.responseText);
            //loaddata();
            //window.location.href = "/Project/Index";
            window.location.href = "/Client/Index?proid=" + editprojectid;

        },

        error: function (Result) {

            //alert("Error");

        }

    })

}




function loadholidays(id) {

    $.ajax({
        url: "/Client/GetHolidays?projectid=" + id,
        type: "Get",
        dataType: "json",

        success: function (res) {

            if (res.length === 0) {
                $("#clientholidays").hide();
                $("#accountholidays").show();
            } else if (res[0].ProjectID === null) {
                $("#clientholidays").hide();
                $("#accountholidays").show();
            } else {
                $("#clientholidays").show();
                $("#accountholidays").hide();
            }


            $("#optionalholidays").val(res[0].optionalholidays);
            $("#useroptionalholidays").val(res[0].useroptionalholidays);
            $('#holidaytable').DataTable({
                'data': res,
                'paginate': true,
                'sort': true,
                'Processing': true,
                'destroy': true,
                'columns': [


                    { 'data': 'HolidayCalendarID', visible: false },
                    { 'data': 'AccountID', visible: false },
                    { 'data': 'HolidayName' },

                    {
                        'data': 'HolidayDate',
                        "type": "date ",
                        "render": function (value) {
                            if (value === null) return "";

                            var pattern = /Date\(([^)]+)\)/;
                            var results = pattern.exec(value);
                            var dt = new Date(parseFloat(results[1]));

                            return (dt.getMonth() + 1) + "/" + dt.getDate() + "/" + dt.getFullYear();
                        }
                    },

                    {
                        'data': 'financialyear',
                    },

                    { 'data': 'ProjectName' },

                    {
                        'data': 'isOptionalHoliday',
                        "render": function (isOptionalHoliday, type, full, meta) {
                            if (isOptionalHoliday === true) {
                                return "<span >Yes</span>";
                            }
                            else {
                                return "<span >No</span>";
                            }
                        }
                    },



                    {
                        'data': 'isDeleted',
                        "render": function (isDeleted, type, full, meta) {
                            if (hlpermission === "Read/Write") {
                                if (isDeleted === true) {
                                    return '<div class="statuscheck"> <input type="checkbox" id="check_02" onclick="HolidayCheckStatus(' + full.HolidayCalendarID + ')"> <label for="check_02"></label> </div>';

                                }
                                else {
                                    return '<div class="statuscheck"> <input type="checkbox" id="check_01" checked onclick="HolidayUnCheckStatus(' + full.HolidayCalendarID + ')"> <label for="check_01"></label> </div>';
                                }
                            } else {
                                if (isDeleted === true) {
                                    return '<div class="statuscheck"> <input type="checkbox" id="check_02" onclick="HolidayCheckStatus(' + full.HolidayCalendarID + ')" disabled> <label for="check_02"></label> </div>';

                                }
                                else {
                                    return '<div class="statuscheck"> <input type="checkbox" id="check_01" checked onclick="HolidayUnCheckStatus(' + full.HolidayCalendarID + ')" disabled> <label for="check_01"></label> </div>';
                                }
                            }

                        }
                    },


                    {
                        "render": function (Usr_UserID, type, full, meta, data) {
                            if (hlpermission === "Read/Write") {
                                return '<a class="btn btn-icn btn-icn-hide" data-toggle="modal"  data-target="#Holidaycalendergrid" id="edit" title="Edit" onclick="EditHoliday(' + full.HolidayCalendarID + ')" ><i class="fa fa-edit" aria-hidden="true"></i></a><a class="btn btn-icn btn-icn-hide" style="display:none"  data-target="#holidaycontainerDelete" data-toggle="modal"  onclick="GetHc(' + full.HolidayCalendarID + ')" "><i class="fa fa-trash" aria-hidden="true"></i></a>';


                            } else {
                                return '<a class="btn btn-icn" data-toggle="modal" style="display:none"  data-target="#Holidaycalendergrid" id="edit" title="Edit" onclick="PreviewHoliday(' + full.HolidayCalendarID + ')" ><i class="fa fa-eye" aria-hidden="true"></i></a>';

                            }

                        }

                    }




                ]
            });




        }


    });
}

function HolidayUnCheckStatus(id) {
    $.ajax({
        url: "/HolidayCalendar/ChangeStatus",
        type: "POST",
        data: {
            'id': id,
            'status': true
        },

        //dataType: "json",
        success: function (data) {

            alert(data);
            window.location.reload();
        },
        complete: function () {
            $('#loading-image').attr("style", "display: none;");
        },
    });
}
function HolidayCheckStatus(id) {
    $.ajax({
        url: "/HolidayCalendar/ChangeStatus",
        type: "POST",
        data: {
            'id': id,
            'status': false
        },

        //dataType: "json",
        success: function (data) {

            alert(data);
            window.location.reload();
        },
        complete: function () {
            $('#loading-image').attr("style", "display: none;");
        },
    });
}

function UsrUnCheckStatus(id) {
    $.ajax({
        url: "/User/ChangeStatus",
        type: "POST",
        data: {
            'id': id,
            'status': true
        },

        //dataType: "json",
        success: function (data) {

            alert(data);
            window.location.reload();
        },
        complete: function () {
            $('#loading-image').attr("style", "display: none;");
        },
    });
}
function UsrCheckStatus(id) {
    $.ajax({
        url: "/User/ChangeStatus",
        type: "POST",
        data: {
            'id': id,
            'status': false
        },

        //dataType: "json",
        success: function (data) {

            alert(data);
            window.location.reload();
        },
        complete: function () {
            $('#loading-image').attr("style", "display: none;");
        },
    });
}


function GetHc(holidayid) {
    delHoliday = holidayid;
}
function EditHoliday(hcid) {

    $("#HolidaybtnAdd").hide();
    $("#HolidaybtnUpdate").show();
    $("#appendRow").hide();


    $.ajax({
        url: "/HolidayCalendar/GetCalenderDetailByID",
        type: "POST",
        data: {
            'id': hcid
        },

        dataType: "json",
        success: function (data) {

            Getfinancialyears();
            getclientforproject();
            $("#HolidayCalendarID").val(data.HolidayCalendarID);
            $("#HolidayName").val(data.HolidayName);
            $("#HolidayCalendarProjectId").val(data.HolidayCalendarProjectId);
            var holiday = data.HolidayDate;
            var exnowdate = new Date(parseInt(holiday.substr(6)));
            $("input[name^='HolidayDate']").val(exnowdate.format('mm/dd/yyyy'));

            $("#FinancialYearId").val(data.FinancialYearId);
            // var date = data.HolidayDate;
            var status = data.isDeleted;

            if (status === true) {
                $("#isDeleted").val("True");
            } else {
                $("#isDeleted").val("False");
            }
            var optional = data.isOptionalHoliday;
            if (optional === true) {
                $("#isOptionalHoliday").val("True");
            } else {
                $("#isOptionalHoliday").val("False");
            }
        }


    });
}

function PreviewHoliday(hcid) {
    $("#HolidaybtnAdd").hide();
    //$("#appendTable").find("input[name='HolidayDate']").datepicker({
    //    format: 'yyyy/mm/dd'

    //});

    $.ajax({
        url: "/HolidayCalendar/GetCalenderDetailByID",
        type: "POST",
        data: {
            'id': hcid
        },

        dataType: "json",
        success: function (data) {
            $("#HolidayCalendarID").val(data.HolidayCalendarID);
            $("#HolidayName").val(data.HolidayName);

            var holiday = data.HolidayDate;
            var exnowdate = new Date(parseInt(holiday.substr(6)));
            $("#HolidayDate").val(exnowdate.format('mm/dd/yyyy'));
            $("#HolidayDate").val();
            $("#FinancialYearId").val(data.FinancialYearId);
            var date = data.HolidayDate;
            var status = data.isActive;

            if (status === true) {
                $("#isActive").val("True");
            } else {
                $("#isActive").val("False");
            }
            var optional = data.isOptionalHoliday;
            if (optional === true) {
                $("#isOptionalHoliday").val("True");
            } else {
                $("#isOptionalHoliday").val("False");
            }



        }


    });
}

function Deletehc(hcid) {

    $('#loading-image').attr("style", "display: block;");

    $.ajax({
        url: "/HolidayCalendar/DeleteHoliday",
        type: "POST",
        data: {
            'id': hcid
        },

        success: function (data) {


            $('#holidaycontainerDelete').hide();
            $(".modal-backdrop").hide();
            $("body").removeClass('modal-open');
            $('#usertable').dataTable();
            $('#holidaytable').dataTable();
            $('#tasktable').dataTable();
            $('#Projecttable').dataTable();
            if (projid === "") {
                usergriddata($("#editpid").val());
                loadholidays($("#editpid").val());
                loadproject($("#editpid").val());
                loadtaks($("#editpid").val());
            } else {
                usergriddata(projid);
                loadholidays(projid);
                loadtaks(projid);
                loadproject(projid);
            }
        },
        complete: function () {
            $('#loading-image').attr("style", "display: none;");
        }
    });
}

function DeleteTask(protaskid) {

    $('#loading-image').attr("style", "display: block;");

    $.ajax({
        url: "/Client/DeleteProjecttask",
        type: "POST",
        data: {
            'id': protaskid
        },

        success: function (data) {

            alert(data);
            $('#taskcontainerDelete').hide();
            $(".modal-backdrop").hide();
            $("body").removeClass('modal-open');
            $('#usertable').dataTable();
            $('#holidaytable').dataTable();
            $('#tasktable').dataTable();
            $('#Projecttable').dataTable();
            if (projid === "") {
                usergriddata($("#editpid").val());
                loadholidays($("#editpid").val());
                loadtaks($("#editpid").val());
                loadproject($("#editpid").val());
            } else {
                usergriddata(projid);
                loadholidays(projid);
                loadtaks(projid);
                loadproject(projid);
            }
        },
        complete: function () {
            $('#loading-image').attr("style", "display: none;");
        }
    });
}


function loadtaks(id) {
    $.ajax({
        url: "/Client/Getprotasks?projectid=" + id,
        type: "Get",
        dataType: "json",

        success: function (res) {

            $('#tasktable').DataTable({
                'data': res,
                'paginate': true,
                'sort': true,
                'Processing': true,
                'destroy': true,
                'columns': [


                    { 'data': 'Proj_SpecificTaskId', visible: false },
                    { 'data': 'Acc_SpecificTaskName' },
                    { 'data': 'Proj_SpecificTaskName' },
                    { 'data': 'RTMId' },

                    // { 'data': 'ProjectId', visible: false },


                    {
                        'data': 'Actual_StartDate',
                        "type": "date ",
                        "render": function (value) {
                            if (value === null) return "";

                            var pattern = /Date\(([^)]+)\)/;
                            var results = pattern.exec(value);
                            var dt = new Date(parseFloat(results[1]));

                            return (dt.getMonth() + 1) + "/" + dt.getDate() + "/" + dt.getFullYear();
                        }
                    },
                    {
                        'data': 'Actual_EndDate',
                        "type": "date ",
                        "render": function (value) {
                            if (value === null) return "";

                            var pattern = /Date\(([^)]+)\)/;
                            var results = pattern.exec(value);
                            var dt = new Date(parseFloat(results[1]));

                            return (dt.getMonth() + 1) + "/" + dt.getDate() + "/" + dt.getFullYear();
                        }
                    },
                    {
                        'data': 'Plan_StartDate',
                        "type": "date ",
                        "render": function (value) {
                            if (value === null) return "";

                            var pattern = /Date\(([^)]+)\)/;
                            var results = pattern.exec(value);
                            var dt = new Date(parseFloat(results[1]));

                            return (dt.getMonth() + 1) + "/" + dt.getDate() + "/" + dt.getFullYear();
                        }
                    },

                    {
                        'data': 'Plan_EndDate',
                        "type": "date ",
                        "render": function (value) {
                            if (value === null) return "";

                            var pattern = /Date\(([^)]+)\)/;
                            var results = pattern.exec(value);
                            var dt = new Date(parseFloat(results[1]));

                            return (dt.getMonth() + 1) + "/" + dt.getDate() + "/" + dt.getFullYear();
                        }
                    },


                    {
                        'data': 'StatusId',
                        "render": function (StatusId, type, full, meta) {
                            if (StatusId === true) {

                                return '<div class="statuscheck"> <input type="checkbox" id="check_01" checked onclick="TaskUnCheckStatus(' + full.Proj_SpecificTaskId + ')"> <label for="check_01"></label> </div>';
                            }
                            else {
                                return '<div class="statuscheck"> <input type="checkbox" id="check_02" onclick="TaskCheckStatus(' + full.Proj_SpecificTaskId + ')"> <label for="check_02"></label> </div>';
                            }
                        }
                    },


                    {
                        "render": function (Proj_SpecificTaskId, type, full, meta, data) {
                            if (taskpermission === "Read") {
                                return '<a class="btn btn-icn btn-icn-hide" data-toggle="modal"  data-target="#Projectspecifictaskgrid" id="edit" title="Edit" onclick="PreviewTasks(' + full.Proj_SpecificTaskId + ')" ><i class="fa fa-eye" aria-hidden="true"></i></a>';

                            }
                            else if (taskpermission === "Read/Write") {
                                return '<a class="btn btn-icn btn-icn-hide" data-toggle="modal"  data-target="#Projectspecifictaskgrid" id="" title="Edit" onclick="EditProjecttask(' + full.Proj_SpecificTaskId + ')" ><i class="fa fa-edit" aria-hidden="true"></i></a><a class="btn btn-icn btn-icn-hide" data-target="#taskcontainerDelete" data-toggle="modal"   onclick="GetProid(' + full.Proj_SpecificTaskId + ')" "><i class="fa fa-trash" aria-hidden="true"></i></a>';


                            } else {
                                return '<a class="btn btn-icn" data-toggle="modal"  data-target="#Projectspecifictaskgrid" id="edit" title="Edit" onclick="PreviewTasks(' + full.Proj_SpecificTaskId + ')" ><i class="fa fa-eye" aria-hidden="true"></i></a>';

                            }

                        },

                    },




                ]
            });

        }


    });
}

function TaskUnCheckStatus(id) {

    $.ajax({
        url: "/Client/ChangeStatus",
        type: "POST",
        data: {
            'id': id,
            'status': false
        },

        success: function (data) {

            alert(data);
            window.location.reload();
        },
        complete: function () {
            $('#loading-image').attr("style", "display: none;");
        }
    });
}
function TaskCheckStatus(id) {

    $.ajax({
        url: "/Client/ChangeStatus",
        type: "POST",
        data: {
            'id': id,
            'status': true
        },

        //dataType: "json",
        success: function (data) {

            alert(data);
            window.location.reload();
        },
        complete: function () {
            $('#loading-image').attr("style", "display: none;");
        },
    });
}



function GetProid(protaskid) {
    delprotaskid = protaskid;
}

function addtasks() {

    var Acc_SpecificTaskName = $("#Acc_SpecificTaskName").val();
    var Proj_SpecificTaskName = $("#Proj_SpecificTaskName").val();
    // Proj_ProjectName = $("#Proj_ProjectName").val();
    var RTMId = $("#RTMId").val();
    var Actual_StartDate = $("#Actual_StartDate").val();
    var Actual_EndDate = $("#Actual_EndDate").val();
    var Plan_StartDate = $("#Plan_StartDate").val();
    var Plan_EndDate = $("#Plan_EndDate").val();
    var StatusId = $("#StatusId").val();
    var projectid = projid;
    var Proj_ActiveStatusVal;

    if ($("#StatusId").val() === "1") {

        Proj_ActiveStatusVal = true;
    }
    else {
        Proj_ActiveStatusVal = false;
    }
    $.ajax({
        type: 'POST',
        //contentType: "application/json; charset=utf-8",
        url: "/Client/Addprotasks",
        data: {

            "Acc_SpecificTaskName": Acc_SpecificTaskName, "ProjectID": projectid, "Proj_SpecificTaskName": Proj_SpecificTaskName,
            "RTMId": RTMId, "Actual_StartDate": Actual_StartDate, "Actual_EndDate": Actual_EndDate, "Plan_StartDate": Plan_StartDate,
            "Plan_EndDate": Plan_EndDate, "StatusId": Proj_ActiveStatusVal
        },
        //dataType: "json",

        success: function (data) {

            alert(data);
            // window.location.reload();
            $('#Projectspecifictaskgrid').hide();
            $(".modal-backdrop").hide();
            $("body").removeClass('modal-open');
            $('#usertable').dataTable();
            $('#holidaytable').dataTable();
            $('#tasktable').dataTable();
            $('#Projecttable').dataTable();

            if (projid === "") {
                usergriddata($("#editpid").val());
                loadholidays($("#editpid").val());
                loadtaks($("#editpid").val());
                loadproject($("#editpid").val());
            } else {
                usergriddata(projid);
                loadholidays(projid);
                loadtaks(projid);
                loadproject(projid);
            }

        },
        complete: function () {
            $('#loading-image').attr("style", "display: none;");
        },
        error: function (error) {
            console.log(error);
        }


    });

}

function EditProjecttask(hcid) {
    $("#Addprojecttask").hide();
    $("#Updatetask").show();

    $.ajax({
        url: "/Client/getprojecttaskbyid?id=" + hcid,
        type: "Get",

        dataType: "json",
        success: function (data) {

            $("#Acc_SpecificTaskName").val(data[0].Acc_SpecificTaskId);
            $("#Proj_SpecificTaskName").val(data[0].Proj_SpecificTaskName);
            $("#RTMId").val(data[0].RTMId);
            $("#Proj_SpecificTaskName").val(data[0].Proj_SpecificTaskName);

            var actualstartdate = data[0].Actual_StartDate;
            var actualenddate = data[0].Actual_EndDate;
            var planstartdate = data[0].Plan_StartDate;
            var planenddate = data[0].Plan_EndDate;
            actualstartdate = new Date(parseInt(actualstartdate.substr(6)));
            actualenddate = new Date(parseInt(actualenddate.substr(6)));
            planstartdate = new Date(parseInt(planstartdate.substr(6)));
            planenddate = new Date(parseInt(planenddate.substr(6)));
            $("#Actual_StartDate").val(actualstartdate.format('mm/dd/yyyy'));
            $("#Actual_EndDate").val(actualenddate.format('mm/dd/yyyy'));
            $("#Plan_StartDate").val(planstartdate.format('mm/dd/yyyy'));
            $("#Plan_EndDate").val(planenddate.format('mm/dd/yyyy'));

            // projid = data[0].ProjectId;
            projectspecifictaskid = data[0].Proj_SpecificTaskId;
            var status = data[0].StatusId;

            if (status === true) {
                $("#StatusId").val(1);
            } else {
                $("#StatusId").val(0);
            }





        },


    });
}

function PreviewTasks(id) {
    EditProjecttask(id);
    $("#Addprojecttask").hide();
    $("#Updatetask").hide();
}


function updatetasks() {
    var Acc_SpecificTaskName = $("#Acc_SpecificTaskName").val();
    var Proj_SpecificTaskName = $("#Proj_SpecificTaskName").val();
    // Proj_ProjectName = $("#Proj_ProjectName").val();
    var RTMId = $("#RTMId").val();
    var Actual_StartDate = $("#Actual_StartDate").val();
    var Actual_EndDate = $("#Actual_EndDate").val();
    var Plan_StartDate = $("#Plan_StartDate").val();
    var Plan_EndDate = $("#Plan_EndDate").val();
    var StatusId = $("#StatusId").val();




    $.ajax({
        type: 'POST',
        //contentType: "application/json; charset=utf-8",
        url: "/Client/updatetasks?id=" + projectspecifictaskid,
        data: {

            "Acc_SpecificTaskName": Acc_SpecificTaskName, "ProjectID": projid, "Proj_SpecificTaskName": Proj_SpecificTaskName,
            "RTMId": RTMId, "Actual_StartDate": Actual_StartDate, "Actual_EndDate": Actual_EndDate, "Plan_StartDate": Plan_StartDate,
            "Plan_EndDate": Plan_EndDate, "StatusId": StatusId
        },
        //dataType: "json",

        success: function (data) {
            // 
            alert(data);
            // window.location.reload();
            $('#Projectspecifictaskgrid').hide();
            $(".modal-backdrop").hide();
            $('#usertable').dataTable();
            $('#holidaytable').dataTable();
            $('#tasktable').dataTable();
            $('#Projecttable').dataTable();

            if (projid === "") {
                usergriddata($("#editpid").val());
                loadholidays($("#editpid").val());
                loadtaks($("#editpid").val());
                loadproject($("#editpid").val());
            } else {
                usergriddata(projid);
                loadholidays(projid);
                loadtaks(projid);
                loadproject(projid);
            }

        },
        complete: function () {
            $('#loading-image').attr("style", "display: none;");
        },
        error: function (error) {
            console.log(error);
        }


    });

}


function getmanagerlist() {
    $.ajax({

        type: "GET",
        contentType: "application/json; charset=utf-8",
        url: "/Client/Associatemanagers?projectid=" + projid,

        dataType: "json",
        success: function (Result) {

            $("#managerslist").empty();
            $("#managerslist").append($("<option></option>").val("").html("Select Manager"));
            $(Result).each(function () {

                //// $("#ManagerName").append($("<option></option>").val(this.Usr_UserID).html(this.UsrP_FirstName));
                $("#managerslist").append($("<option></option>").val(this.Usr_UserID).html(this.Usr_Username));


            });

        },

        error: function (Result) {

            // alert("Error");

        }

    });
}

function getemployeelist() {
    $.ajax({

        type: "GET",
        contentType: "application/json; charset=utf-8",
        url: "/Client/AssociateEmployees?projectid=" + projid,

        dataType: "json",
        success: function (Result) {

            $("#employeeslist").empty();
            $("#employeeslist").append($("<option></option>").val(0).html("Select Resource"));
            $(Result).each(function () {

                //// $("#ManagerName").append($("<option></option>").val(this.Usr_UserID).html(this.UsrP_FirstName));
                $("#employeeslist").append($("<option></option>").val(this.Usr_UserID).html(this.Usr_Username));


            });

        },

        error: function (Result) {

            // alert("Error");

        }

    });
}
var Employeeid;
function Selectmanager(value) {


    var projectid = projid;
    $("#ContainerGridDetail").show();
    $("#manbtnassociate").show();
    $("#empbtnassociate").hide();
    Associateuser(value);

}

function SelectEmployee(empvalue) {

    var projectid = projid;
    $("#ContainerGridDetail").show();
    $("#manbtnassociate").hide();
    $("#empbtnassociate").show();
    Associateuser(empvalue);
    //$("#btnUpdateModel").hide();
    //$("#btnassociateemp").show();
}

function Associateuser(userid) {

    var roledata = $("#RoleName").val();
    $("#ManagerName").empty();
    $("#Managername2").empty();
    bindProjectNames();
    bindAllRoleNames();
    GetAlltasknames(roledata);
    bindUserNames();
    bindUserTypes();
    GetManagerByRole(userid);
    $("#btnManAddModel").hide();
    $("#btnEmpAddModel").hide();
    $("#btnUpdateModel").hide();
    Employeeid = userid;


    $.ajax({

        url: '/Client/GetassUserDetailById?id=' + userid,
        type: 'Get',

        success: function (data) {

            //$("#profile-image").attr("src", "/Content/images/local-disk.png");
            //$("#profile-image").removeClass("img-responsive");
            //$("label[for='fileUpload']").next("span").text("Choose file from local disk");
            //$("#profile-image").next("div.cropper-container").remove();
            //$("#lblchvideoexits").hide();
            //$('#image_input > img.file-upload-image').attr('src', '/Content/images/local-disk.png');
            //$('#image_input > img.file-upload-image').removeAttr('style');
            //$("#image_input > .jcrop-holder").remove();
            //$('#image_input > img.file-upload-image').show();
            //$('#image_input > img.file-upload-image').css("visibility", "visible");
            Employeeid = data.Usr_UserID;
            var Email = data.Email;
            var Usr_RoleID = data.Usr_RoleID;
            var UsrP_EmployeeID = data.UsrP_EmployeeID;
            Proj_ProjectID = data.Proj_ProjectID;
            var Proj_ProjectName = data.Proj_ProjectName;
            var UProj_ParticipationPercentage = data.UProj_ParticipationPercentage;
            var UsrP_FirstName = data.UsrP_FirstName;
            var UsrP_LastName = data.UsrP_LastName;
            var Usr_Password = data.Usr_Password;
            var Usr_Titleid = data.Usr_Titleid;
            var Usrp_MobileNumber = data.Usrp_MobileNumber;
            var Usr_GenderId = data.Usr_GenderId;
            var Usrp_ProfilePicture = data.Usrp_ProfilePicture;
            var Usr_UserTypeID = data.Usr_UserTypeID;
            var Usr_TaskID = data.Usr_TaskID;
            var ManagerName = data.ManagerName;
            var Managername2 = data.Managername2;
            var Usr_Username = data.Usr_Username;

            //var UProj_ActiveStatus = data.UProj_ActiveStatus;
            //if (UProj_ActiveStatus === true) {
            //    $("#UProj_ActiveStatus").val("1");
            //} else {
            //    $("#UProj_ActiveStatus").val("0");
            //}
            if (data.Usrp_DOJ !== null) {
                var DOJ = (data.Usrp_DOJ).substr(8);
            }

            if (data.UProj_StartDate !== null) {
                var StartDate = (data.UProj_StartDate).substr(8);
            }
            if (data.UProj_EndDate !== null) {
                var EndDate = (data.UProj_EndDate).substr(8);
            }
            var Proj_StartDate = new Date(parseInt(StartDate));

            var Proj_EndDate = new Date(parseInt(EndDate));

            var doj = new Date(parseInt(DOJ));

            $("#Usr_UserID").val(Usr_UserID);
            $("#Usr_Username").val(Usr_Username);
            $("#Email").val(Email);
            $("#Usr_Password").val();
            $("#Usr_ConfirmPassword").val();
            $("#Usrp_MobileNumber").val(Usrp_MobileNumber);
            $("#Usr_Titleid").val(Usr_Titleid);
            $("#Usr_GenderId").val(Usr_GenderId);
            $("#UsrP_FirstName").val(UsrP_FirstName);
            $("#UsrP_LastName").val(UsrP_LastName);
            $("#UsrP_EmployeeID").val(UsrP_EmployeeID);
            $("#Usr_UserTypeID").val(Usr_UserTypeID);
            $("#RoleName").val(Usr_RoleID);
            $("#ManagerName").val(ManagerName);
            $("#Managername2").val(Managername2);
            $("#Usr_TaskID").val(Usr_TaskID);
            $("#UProj_ParticipationPercentage").val(UProj_ParticipationPercentage);

            var folderName = "/uploadimages/images/" + Usrp_ProfilePicture;

            $("#profile-image").prop('src', folderName);
            $("#profile-image").addClass('img-responsive');
            $("#Usrp_DOJ").val(doj.format('mm/dd/yyyy'));


            $("#UProj_StartDate").val(Proj_StartDate.format('mm/dd/yyyy'));
            $("#UProj_EndDate").val(Proj_EndDate.format('mm/dd/yyyy'));

        },
        error: function () {
            alert(Response.text);
        }
    });
}


var deletecid = 0;
var deleprojid = 0;
function DeleteTableProject(CL_ProjectID, ProjectID) {
    
    deletecid = CL_ProjectID;
    deleprojid = ProjectID;
    $("#deletecid").val(deletecid, deleprojid);
}
function DeleteRow() {
    
    $.ajax({
        url: "/Client/DeleteProjectnotAssigned?CL_ProjectID=" + deletecid + "&ProjectId=" + deleprojid,
        type: "GET",
        contentType: 'application/json; charset=utf-8',
        async: false,
        success: function (res) {
            $("#alertmodal").modal('show');
            //alert("Successfully Deleted");
            //window.location.href = "/Admin/TableSubjects";
        },

        error: function (msg) {

        }

    });

}


//function AddSelectedEmployee() {

//       ;
//        var Empclientcal = "";
//        if ($("#isclientholidays").is(":checked")) {
//            Empclientcal = true;
//        }
//        else {
//            Empclientcal = false;
//        }

//        var formdata = new FormData();
//        formdata.append("Proj_ProjectID", $("#proid").val());
//        formdata.append("Usr_UserID", $("#SelectedEmployeeName option:selected").val());
//        formdata.append("ClientprojID", $("#Userprojects option:selected").val());
//        formdata.append("ManagerName", $("#ManagerforNewEmp option:selected").val());
//        formdata.append("Managername2", $("#SelectedMan2ForEmp option:selected").val());
//        formdata.append("TimesheetMode_id", $("#TimesheetMode_id2 option:selected").val());
//        formdata.append("RoleName", $("#EmpRoleName option:selected").val());
//        formdata.append("Usr_TaskID", $("#EmpTaskName option:selected").val());
//        formdata.append("isclientholidays", Empclientcal);

//        $.ajax({
//            url: "/Client/AddSelectedEmployee",
//            type: 'POST',
//            contentType: false,
//            data: formdata,
//            async: true,
//            processData: false,
//            cache: false,
//            success: function (data) {
//                if (data == "1") {

//                    window.location.href = "/Client/Index?proid=" + $("#proid").val();
//                }
//                else {
//                    alert("Error");
//                }
//            },
//            error: function (Result) {

//            }



//        });

//}


