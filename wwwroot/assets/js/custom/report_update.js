$(document).ready(function () {
   
    //initiate datatables and order descending
    $('#appDataTable').dataTable({
        "bDestroy": true
    }).fnDestroy();

    //initiate datatables and order descending
    var table = $('#appDataTable').DataTable({
        dom: 'Bfrtip',
        buttons: [
            {
                extend: 'pdfHtml5',
                orientation: 'landscape',
                pageSize: 'LEGAL'
            },
            'copy', 'csv', 'excel', 'print'
        ]
    });

    table.order([7, 'desc']).draw();

    //get the form ID that was stored
    var formId = sessionStorage.getItem("reportUpdateId");
    var formName = sessionStorage.getItem("reportUpdateName");

    //set the formId into the input field
    $("#reportUpdateId").val(formId);

    //set the form name and display it
    $("#reportUpdateName").html(formName);

    var reportSentDateVal = $("#reportSentDateRange").val();

    //check if the value of the date field exists and set it with js
    if (reportSentDateVal == "" || reportSentDateVal == undefined || reportSentDateVal == null || reportSentDateVal == "not-specified") {
        reportSentDateRangePicker();
    }
    else {
        reportSentDateRangePicker();
        $("#reportSentDateRange").val(reportSentDateVal);
    }       
});


//default datetime picker
function reportSentDateRangePicker() {

    var start = moment().startOf('day');
    var end = moment().endOf('day');

    function cb(start, end) {
        $('#reportSentDateRange span').html(start.format('YYYY-MM-DD H:mm') + ' - ' + end.format('YYYY-MM-DD H:mm'));
    }

    $('#reportSentDateRange').daterangepicker({
        
        timePicker: true,
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
            format: 'YYYY-MM-DD H:mm'
        },
        singleDatePicker: true
    }, cb);

    cb(start, end);

    // on select 
    $('#reportSentDateRange').on('apply.daterangepicker', function (ev, picker) {

        allStartDate = picker.startDate.format('YYYY-MM-DD H:mm:s');
        allEndDate = picker.endDate.format('YYYY-MM-DD H:mm:s');

        //  do something, like logging an input
        console.log("all Start date");
        console.log(allStartDate);

        console.log('allEndDate');
        console.log(allEndDate);

    });


}

//Handle the report update edit button click
$(document).on('click', '[data-update-report-edit]', function (e) {

    var item = JSON.parse(JSON.parse(JSON.stringify($(this).attr('data-update-report-edit'))));
    //save the id into storage 
    sessionStorage.setItem("reportUpdateId", item.reportId)
    window.location.href = "/ReportUpdate/Edit/"+item.id;

});

//Handle the admin report update edit button click
$(document).on('click', '[data-admin-update-report-edit]', function (e) {

    var item = JSON.parse(JSON.parse(JSON.stringify($(this).attr('data-admin-update-report-edit'))));

    if (item.createdBy.toUpperCase() == item.aknowledger.toUpperCase()) {
        displayErrorMsg("You cannot acknowledge your own request, kindly prompt another admin to acknowledge.")
        return false;
    }
    else {
        //save the id into storage 
        sessionStorage.setItem("reportUpdateId", item.reportId);
        sessionStorage.setItem("reportUpdateName", item.reportName);
        window.location.href = "/AdminReportUpdate/Edit/" + item.id;
    }

});


//lets handle clicks of each cards --- all report, pending, sent, acknowledged
var table = $('#appDataTable').DataTable();

$('#allReportCard').on('click', function () {
    //table.columns(8).search(" ").draw();
    table.columns(1).search(" ").draw();
});

$('#pendingCard').on('click', function () {
    //table.columns(8).search(" ").draw();
    table.columns(1).search("pending").draw();
});

$('#sentCard').on('click', function () {
    //table.columns(8).search(" ").draw();
    table.columns(1).search("sent").draw();
});

$('#acknowledgeCard').on('click', function () {
    table.columns(1).search("acknowledged").draw();
    //table.columns(8).search("yes").draw();
});