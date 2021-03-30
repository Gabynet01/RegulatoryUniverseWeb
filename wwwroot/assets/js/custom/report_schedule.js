$(document).ready(function () {
    initializeEmailsBox();
    var scheduleDateVal = $("#reportScheduleDateRange").val();
    var scheduleFrequency = $("#ScheduleFrequency").val();
    var scheduleTimeVal = $("#reportScheduleTimeRange").val();

    if (scheduleDateVal == "" || scheduleDateVal == undefined || scheduleDateVal == null || scheduleDateVal == "not-specified") {
        $("#selectedScheduleDateInput").hide("fast");
    }
    else {
        if (scheduleFrequency.toUpperCase() == "DAILY") {
            //set the value
            $("#reportScheduleDateRange").val(scheduleDateVal);
            $("#reportScheduleDateRange").prop("disabled", true);
            //set the time
            onlyTimePicker("H:mm", true);
            $("#reportScheduleTimeRange").val(scheduleTimeVal);

        }
        if (scheduleFrequency.toUpperCase() == "WEEKLY") {
            $("#showPlaceHolderText").html("Select the day of the week e.g Friday");
            //set the value
            changeToWeeklyOption();
            $("#reportScheduleDateRange").val(scheduleDateVal);
            //set the time
            onlyTimePicker("H:mm", true);
            $("#reportScheduleTimeRange").val(scheduleTimeVal);

        }
        if (scheduleFrequency.toUpperCase() == "MONTHLY" || scheduleFrequency.toUpperCase() == "HALFYEAR") {
            $("#showPlaceHolderText").html("Select the day in the month");
            //set the value
            reportScheduleDatePicker("D", false);
            $("#reportScheduleDateRange").val(scheduleDateVal);
            //set the time
            onlyTimePicker("H:mm", true);
            $("#reportScheduleTimeRange").val(scheduleTimeVal);
        }
        if (scheduleFrequency.toUpperCase() == "QUARTERLY") {
            $("#showPlaceHolderText").html("Select the day below using these format e.g 15 or 15, after end of quarter");
            //set the value
            changeToQuarterlyOption();
            $("#reportScheduleDateRange").val(scheduleDateVal);
            //set the time
            onlyTimePicker("H:mm", true);
            $("#reportScheduleTimeRange").val(scheduleTimeVal);
        }
        if (scheduleFrequency.toUpperCase() == "ANNUALLY") {
            $("#showPlaceHolderText").html("Select the day and month");
            //set the value
            reportScheduleDatePicker("D/M", false);
            $("#reportScheduleDateRange").val(scheduleDateVal);
            //set the time
            onlyTimePicker("H:mm", true);
            $("#reportScheduleTimeRange").val(scheduleTimeVal);
        }
        $("#showPlaceHolderTimeText").html("Select the time (24hrs format) e.g 9:30 or 16:45");
        $("#selectedScheduleDateInput").show("fast");

    }

    //initiate datatables and order descending
    var table = $('#appDataTable').DataTable();

    table.order([8, 'desc']).draw();

   

});

//handle select option field
function showScheduleDateField(selectedValue) {
    var value = selectedValue.value;
    $("#showPlaceHolderText").html("");

    resetInput();
    onlyTimePicker("H:mm", true);

    if (value.toUpperCase() == "") {
        $("#selectedScheduleDateInput").hide("fast");
        $("#showPlaceHolderText").html("");
    }
    if (value.toUpperCase() == "DAILY") {
        $("#selectedScheduleDateInput").show("fast");
        $("#showPlaceHolderTimeText").html("Select the time (24hrs format) e.g 9:30 or 16:45");
        $("#reportScheduleDateRange").replaceWith('<input name="ScheduleFrequencyDate" type="text" value="Everyday" disabled class="form-control" id="reportScheduleDateRange">');
        //dailyDatePicker("D", true);
    }
    if (value.toUpperCase() == "WEEKLY") {
        $("#selectedScheduleDateInput").show("fast");
        $("#showPlaceHolderText").html("Select the day of the week e.g Friday");
        $("#showPlaceHolderTimeText").html("Select the time (24hrs format) e.g 9:30 or 16:45");
        changeToWeeklyOption();
    }
    if (value.toUpperCase() == "MONTHLY") {
        $("#selectedScheduleDateInput").show("fast");
        $("#showPlaceHolderText").html("Select the day in the month");
        $("#showPlaceHolderTimeText").html("Select the time (24hrs format) e.g 9:30 or 16:45");
        reportScheduleDatePicker("D", false);
    }
    if (value.toUpperCase() == "QUARTERLY") {
        $("#selectedScheduleDateInput").show("fast");
        $("#showPlaceHolderText").html("Select the day below using these format e.g 15 or 15, after end of quarter");
        $("#showPlaceHolderTimeText").html("Select the time (24hrs format) e.g 9:30 or 16:45");
        changeToQuarterlyOption();
    }
    if (value.toUpperCase() == "HALFYEAR") {
        $("#selectedScheduleDateInput").show("fast");
        $("#showPlaceHolderText").html("Select the day in the month");
        $("#showPlaceHolderTimeText").html("Select the time (24hrs format) e.g 9:30 or 16:45");
        reportScheduleDatePicker("D", false);
    }
    if (value.toUpperCase() == "ANNUALLY") {
        $("#selectedScheduleDateInput").show("fast");
        $("#showPlaceHolderText").html("Select the day and month");
        $("#showPlaceHolderTimeText").html("Select the time (24hrs format) e.g 9:30 or 16:45");
        reportScheduleDatePicker("D/M", false);
    }
}

//update the placeholder
function placeholderUpdate(placeholderValue) {
    document.getElementById("scheduleFrequencyDateId").placeholder = placeholderValue;
}

//Handle delete button click
$(document).on('click', '[data-delete-id]', function (e) {

    var deleteId = $(this).attr('data-delete-id');

    swal({
        title: "Are you sure?",
        text: "",
        type: "warning",
        showCancelButton: true,
        closeOnConfirm: true,
        showLoaderOnConfirm: true,
        confirmButtonText: "Yes"
    },

        function () {
            window.location.href = "/ReportSchedules/Delete/" + deleteId;
        });

});

//Handle delete button click
$(document).on('click', '[data-update-report-status]', function (e) {

    var item = JSON.parse(JSON.parse(JSON.stringify($(this).attr('data-update-report-status'))));

    console.log(item)
    console.log(item.id)

    //save the id into storage 
    sessionStorage.setItem("reportUpdateId", item.id)
    window.location.href = "/ReportUpdate/Create";

});




// Daily Time Component
function dailyDatePicker(acceptedFormat, showTimePicker) {

    var start = moment().startOf('day');
    var end = moment().endOf('day');

    function cb(start, end) {
        $('#reportScheduleDateRange span').html(start.format(acceptedFormat) + ' - ' + end.format(acceptedFormat));
    }

    $('#reportScheduleDateRange').daterangepicker({
        parentEl: $('#reportScheduleDiv'),
        timePicker: showTimePicker,
        timePicker24Hour: true,
        startDate: start,
        endDate: end,
        locale: {
            format: acceptedFormat
        },
        singleDatePicker: true
    }, cb);

    cb(start, end);

}


// Only Time Component
function onlyTimePicker(acceptedFormat, showTimePicker) {

    var start = moment().startOf('hour');
    var end = moment().endOf('hour');

    function cb(start, end) {
        $('#reportScheduleTimeRange span').html(start.format(acceptedFormat) + ' - ' + end.format(acceptedFormat));
    }

    $('#reportScheduleTimeRange').daterangepicker({
        parentEl: $('#reportScheduleDiv'),
        timePicker: showTimePicker,
        timePicker24Hour: true,
        startDate: start,
        endDate: end,
        locale: {
            format: acceptedFormat
        },
        singleDatePicker: true
    }, cb);

    cb(start, end);


    //hide the calendar
    $('#reportScheduleTimeRange').on('show.daterangepicker', function (ev, picker) {

        picker.container.find(".calendar-table").hide();

    });

}


//default datetime picker
function reportScheduleDatePicker(acceptedFormat, showTimePicker) {

    var start = moment().startOf('day');
    var end = moment().endOf('day');

    function cb(start, end) {
        $('#reportScheduleDateRange span').html(start.format(acceptedFormat) + ' - ' + end.format(acceptedFormat));
    }

    $('#reportScheduleDateRange').daterangepicker({
        parentEl: $('#reportScheduleDiv'),
        timePicker: showTimePicker,
        timePicker24Hour: true,
        startDate: start,
        endDate: end,
        ranges: {
            'Today': [moment().startOf('day'), moment()],
            'Tomorrow': [moment().startOf('day').add(1, 'days'), moment().add(1, 'days')],
            'Next Week': [moment().startOf('day').add(6, 'days'), moment()],
            'Next Month': [moment().startOf('day').add(29, 'days'), moment()],
            'This Month': [moment().startOf('day').startOf('month'), moment().endOf('month')]
        },
        locale: {
            format: acceptedFormat
        },
        singleDatePicker: true
    }, cb);

    cb(start, end);

    // on select 
    $('#reportScheduleDateRange').on('apply.daterangepicker', function (ev, picker) {

        allStartDate = picker.startDate.format(acceptedFormat);
        allEndDate = picker.endDate.format(acceptedFormat);

        //  do something, like logging an input
        console.log("all Start date");
        console.log(allStartDate);

        console.log('allEndDate');
        console.log(allEndDate);

    });

}


//set week dropdowns only when weekly schedule is selected
function changeToWeeklyOption() {

    //$("#reportScheduleDateRange").on("click", function () {
    $("#reportScheduleDateRange").replaceWith('<select class="form-control" name="ScheduleFrequencyDate" id="reportScheduleDateRange">' +
        '<option value="">Please select a day</option>' +
        '<option value="Monday">Monday</option>' +
        '<option value="Tuesday">Tuesday</option>' +
        '<option value="Wednesday">Wednesday</option>' +
        '<option value="Thursday">Thursday</option>' +
        '<option value="Friday">Friday</option>' +
        '<option value="Saturday">Saturday</option>' +
        '<option value="Sunday">Sunday</option>' +

        '</select> ');
    //});
}

//set quarterly dropdowns only when quarterly schedule is selected
function changeToQuarterlyOption() {

    var optionsFields = "";

    //auto generate the option fields
    for (var i = 1; i <= 31; i++) {
        optionsFields += "<option value=" + i + ">" + i + "</option> <option value=" + i + ",after>" + i + ", After End of Quarter</option>"
    }
    //$("#reportScheduleDateRange").on("click", function () {
    $("#reportScheduleDateRange").replaceWith('<select class="form-control" name="ScheduleFrequencyDate" id="reportScheduleDateRange">' +
        '<option value="">Please select below</option>' +
        optionsFields +

        '</select > ');
    //});
}

//reset the input field back to default
function resetInput() {
    $("#reportScheduleDateRange").replaceWith('<input name="ScheduleFrequencyDate" type="text" class="form-control" id="reportScheduleDateRange">');
}


//submit and assign the values
$("#submitReportBtn").click(function (e) {
    //enable disabled fields
    $("#reportScheduleDateRange").prop("disabled", false);
});

//handle assigned emails onclick
$("#assignedEmailsDiv").on("click", function () {
    //open the modal here
    $("#assignedEmailsModal").modal("show");
});

//handle all modal emails addall btn onclick
$("#addModalEmailsBtn").on("click", function () {
    //call the function below to fill in the emails entered into the text area
    getAllEmailsFromModal();
});


//initialize
function initializeEmailsBox() {
    var counter = 0;
    var addButton = $('.add_button'); //Add button selector
    var wrapper = $('.field_wrapper'); //Input field wrapper
   
    //Once add button is clicked
    $(addButton).click(function () {
        //increment the counter
        counter++;
        var fieldHTML = '<div class="form-group"><div class="input-group"><span class="input-group-addon"><i class="ti-email"></i></span><input type="email" id="sug_input_sup'+counter +'" class="form-control" placeholder="Enter email address here"/><a href="javascript:void(0);" class="remove_button">&nbsp;&nbsp;<span class="input-group-btn"><i class="ti-close"></i></span></a></div></div>'; //New input field html 
        $(wrapper).append(fieldHTML); //Add field html
        $('#sug_input_sup'+counter).keyup(function (e) {
            sug_email('sug_input_sup' + counter, 'result_sup');

        });
    });

    //Once remove button is clicked
    $(wrapper).on('click', '.remove_button', function (e) {
        e.preventDefault();
        $(this).parent('div').remove(); //Remove field html
       
    });



}


//populate the text area with all the emails that were entered in the modal
function getAllEmailsFromModal() {
    var inputs = document.querySelectorAll('input[type="email"]');
    var txtArea = document.querySelector('[name="AssignedEmails"]');

    //collate all errors here
    var errors = [];

    //lets clear the text area field
    txtArea.value = "";

    //loop the text inputs
    inputs.forEach(function (elem, index) {
        var valueOf = $.trim(elem.value);

        if (!validateEmail(valueOf) && (valueOf == "" || valueOf == undefined)) {
            displayErrorMsgModal("email address cannot be empty");
            errors.push("Email address cannot be empty");
            return false;
        }

        else if (validateEmail(valueOf) && (valueOf !== "" || valueOf != undefined)) {
            //only append a comma if it is not the last item
            if (index < (inputs.length - 1)) {
                txtArea.value += valueOf + ";";  //concat the value
            }
            else {
                txtArea.value += valueOf;
            }

        }

        else {
            displayErrorMsgModal(valueOf + " is a not a valid email address");
            errors.push(valueOf + " is a not a valid email address");
            return false;
        }
    });

    //lets check if the errors array is empty or not 
    if (errors.length == 0) {
        //close the modal
        $("#assignedEmailsModal").modal("hide");
    }
    else {
        //display all the errors
        displayErrorMsgModal(errors.toString());
        return false;
    }

}


//this will handle onclick of the assigned emails field in edit mode
$("#editAssignedEmailsDiv").on("click", function () {
    //lets get all the data inside the text area
    var allEmails = $("#editAssignedEmailsDiv").val();
    //convert the string to array
    var stringToArray = allEmails.split(";");

    var counter = 0;
    // initiate the dynamic form fields here
    var addButton = $('.add_button_edit'); //Add button selector
    var wrapper = $('.field_wrapper_edit'); //Input field wrapper
  
    //Once add button is clicked
    $(addButton).click(function () {
        //increment the counter
        counter++;
        var fieldHTML = '<div class="form-group"><div class="input-group"><span class="input-group-addon"><i class="ti-email"></i></span><input type="email" id="sug_input_sup' + counter + '" class="form-control" placeholder="Enter email address here"/><a href="javascript:void(0);" class="remove_button_edit">&nbsp;&nbsp;<span class="input-group-btn"><i class="ti-close"></i></span></a></div></div>'; //New input field html 

        $(wrapper).append(fieldHTML); //Add field html
        $('#sug_input_sup' + counter).keyup(function (e) {
            sug_email('sug_input_sup' + counter, 'result_sup');

        });
    });

    //Once remove button is clicked
    $(wrapper).on('click', '.remove_button_edit', function (e) {
        e.preventDefault();
        $(this).parent('div').remove(); //Remove field html
    });



    var htmlString = "";

    for (i = 0; i < stringToArray.length; i++) {
        htmlString += '<div class="form-group"><div class="input-group"><span class="input-group-addon"><i class="ti-email"></i></span><input type="email" id="sug_input_sup" class="form-control" value="' + stringToArray[i] + '" /><a href="javascript:void(0);" class="remove_button_edit">&nbsp;&nbsp;<span class="input-group-btn"><i class="ti-close"></i></span></a></div></div>';

    }

    $("#editAssignedEmailsInput").html(htmlString);


    //open the modal here
    $("#assignedEmailsModal").modal("show");
});


//lets handle clicks of each cards
var table = $('#appDataTable').DataTable();

$('#dailyCard').on('click', function () {
    table.columns(2).search("daily").draw();
});

//weekly card
$('#weeklyCard').on('click', function () {
    table.columns(2).search("weekly").draw();
});

//monthly card
$('#monthlyCard').on('click', function () {
    table.columns(2).search("monthly").draw();
});

//quarterly card
$('#quarterlyCard').on('click', function () {
    table.columns(2).search("quarterly").draw();
});

//half year card
$('#halfYearCard').on('click', function () {
    table.columns(2).search("halfyear").draw();
});

//annual card
$('#annualCard').on('click', function () {
    table.columns(2).search("annually").draw();
});