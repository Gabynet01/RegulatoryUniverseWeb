
$(document).ready(function () {
    //initiate select picker component
    $('.schedule_frequency_select_picker').selectpicker();
    $('.report_status_select_picker').selectpicker();

    fromReportDateRangePicker();
    toReportDateRangePicker();

    getReportChartData();

    //show and hide some defaults
    $("#analyticsDefault").show("fast");
    $("#searchAnalyticsTable").hide("fast");
});


// Date Picker Component
function fromReportDateRangePicker() {

    var start = moment().startOf('day');
    var end = moment().endOf('day');

    function cb(start, end) {
        $('#fromReportDateRange').val("");
    }

    $('#fromReportDateRange').daterangepicker({
        parentEl: $('#searchAnalyticsModal'),
        timePicker: false,
        timePicker24Hour: false,
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
            format: 'YYYY-MM-DD'
        },
        singleDatePicker: true
    }, cb);

    cb(start, end);

    // on select 
    $('#fromReportDateRange').on('apply.daterangepicker', function (ev, picker) {

        allStartDate = picker.startDate.format('YYYY-MM-DD HH:mm:ss');
        allEndDate = picker.endDate.format('YYYY-MM-DD HH:mm:ss');

        //  do something, like logging an input
        console.log("all Start date");
        console.log(allStartDate);

        console.log('allEndDate');
        console.log(allEndDate);

    });

}

//to report date range
function toReportDateRangePicker() {

    var start = moment().startOf('day');
    var end = moment().endOf('day');

    function cb(start, end) {
        $('#toReportDateRange').val("");
    }

    $('#toReportDateRange').daterangepicker({
        parentEl: $('#searchAnalyticsModal'),
        timePicker: false,
        timePicker24Hour: false,
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
            format: 'YYYY-MM-DD'
        },
        singleDatePicker: true
    }, cb);

    cb(start, end);

    // on select 
    $('#toReportDateRange').on('apply.daterangepicker', function (ev, picker) {

        allStartDate = picker.startDate.format('YYYY-MM-DD HH:mm:ss');
        allEndDate = picker.endDate.format('YYYY-MM-DD HH:mm:ss');

        //  do something, like logging an input
        console.log("all Start date");
        console.log(allStartDate);

        console.log('allEndDate');
        console.log(allEndDate);

    });

}


//this is called onchange of the report status dropdown
function getReportScheduleByStatus(selectedValue) {

    var reportStatus = selectedValue.value;

    var formData = {
        "reportStatus": reportStatus
    };

    show_loader();

    $.ajax({
        url: '/AdminAnalytics/ReportScheduleByStatus',
        type: 'POST',
        data: formData,
        cache: false,
        success: function (responseData) {
            //console.log(JSON.stringify(responseData)) 

            if (responseData.code.toUpperCase() == "200") {

                var ctx = document.getElementById("report-schedule-chart").getContext('2d');

                var reportScheduleChart = new Chart(ctx, {
                    type: 'doughnut',
                    data: {
                        labels: ["Daily: " + responseData["reportDailyCount"], "Weekly: " + responseData["reportWeeklyCount"], "Monthly: " + responseData["reportMonthlyCount"], "Quarterly: " + responseData["reportQuarterlyCount"], "HalfYear: " + responseData["reportHalfYearCount"], "Annual: " + responseData["reportAnnualCount"]],
                        datasets: [{
                            label: "Label",
                            backgroundColor: ["#7d2d94", "#0863a2", "#0d7c43", "#f4b710", "#e73c44", "#f7931e"],
                            hoverBorderColor: ["#fff", "#fff", "#fff", "#fff", "#fff", "#fff"],
                            borderColor: ["#fff", "#fff", "#fff", "#fff", "#fff", "#fff"],
                            borderWidth: 10,
                            data: [responseData["reportDailyCount"], responseData["reportWeeklyCount"], responseData["reportMonthlyCount"], responseData["reportQuarterlyCount"], responseData["reportHalfYearCount"], responseData["reportAnnualCount"]]
                        }]
                    },
                    options: {
                        legend: {
                            display: true,
                            position: 'right',
                            labels: {
                                fontColor: "#2e3451",
                                usePointStyle: true,
                                fontSize: 14
                            }
                        },
                        tooltips: {
                            backgroundColor: 'rgba(47, 49, 66, 0.8)',
                            titleFontSize: 14,
                            titleFontColor: '#fff',
                            caretSize: 0,
                            cornerRadius: 4,
                            xPadding: 10,
                            displayColors: true,
                            yPadding: 10,
                            enabled: false
                        }
                    }
                });

                displaySuccessToast("Success", reportStatus.toUpperCase() + " report schedule data was pulled successfully")

            }
            else {
                displayErrorMsg("Failed to get report schedule data");
            }

        },
        error: function () {
            displayErrorMsg("Something went wrong from the server");
            console.log("Something went wrong when fetching data from the server");
        }
    });
}


//search analytics button 
$("#searchAnalyticsBtn").click(function (e) {

    var selectScheduleFrequency = $.trim($('#selectScheduleFrequency').val());
    var selectReportStatus = $.trim($('#selectReportStatus').val());
    var fromReportDateRange = $.trim($('#fromReportDateRange').val());
    var toReportDateRange = $.trim($('#toReportDateRange').val());

    if (selectScheduleFrequency == "" || selectScheduleFrequency.length == 0 || selectScheduleFrequency == undefined) {
        displayErrorMsgModal("Please select at least one schedule");
        return false;
    }

    if (selectReportStatus == "" || selectReportStatus.length == 0 || selectReportStatus == undefined) {
        displayErrorMsgModal("Please select at least one status");
        return false;
    }

    if (fromReportDateRange == "" || fromReportDateRange == undefined) {
        displayErrorMsgModal("Please select a from date");
        return false;
    }

    if (toReportDateRange == "" || toReportDateRange == undefined) {
        displayErrorMsgModal("Please select a to date");
        return false;
    }

    var formData = {
        "selectScheduleFrequency": selectScheduleFrequency.toString(),
        "selectReportStatus": selectReportStatus.toString(),
        "fromReportDateRange": fromReportDateRange,
        "toReportDateRange": toReportDateRange
    };

    //console.log("formData--> ", formData);

    show_modal_loader();

    $("#searchAnalyticsBtn").prop("disabled", true);

    $.ajax({
        url: '/AdminAnalytics/ReportAnalyticsBySearch',
        type: 'POST',
        data: formData,
        cache: false,
        success: function (responseData) {
            console.log(JSON.stringify(responseData))
            $("#searchAnalyticsBtn").removeAttr('disabled');

            //lets destroy the datatable first
            $('#appDataTable').dataTable({
                "bDestroy": true
            }).fnDestroy();

            if (responseData.code.toUpperCase() == "200") {
                //hide the modal
                $("#searchAnalyticsModal").modal("hide");
                displaySuccessToastModal(responseData.reportCount + " records pulled successfully", "")

                var allData = responseData.reportData;

                //first lets populate the table
                var table_list = "";
                var updatedReportSearch = responseData.reportCount;
                var pendingReportSearch = 0;
                var sentReportSearch = 0;
                var acknowledgedReportSearch = 0;

                for (i = 0; i < allData.length; i++) {
                    mainData = allData[i];

                    //lets get a count of all pending
                    if (mainData.status.toUpperCase() == "PENDING") {
                        pendingReportSearch++
                    }

                    if (mainData.status.toUpperCase() == "SENT") {
                        sentReportSearch++
                    }

                    if (mainData.status.toUpperCase() == "ACKNOWLEDGED") {
                        acknowledgedReportSearch++
                    }


                    table_list +=
                        "<tr width='100%'>" +
                        "<td>" + parseInt(i + 1) + "</td>" +
                        "<td>" + mainData.reportName + "</td>" +
                        "<td>" + mainData.status.toUpperCase() + "</td>" +
                        "<td>" + mainData.scheduleFrequency + "</td>" +
                        "<td>" + mainData.scheduleFrequencyDate + " / <b>" + mainData.scheduleFrequencyTime + "</b> </td>" +
                        "<td>" + mainData.receivingInstitution + "</td>" +
                        "<td>" + mainData.responsibleDepartment + "</td>" +
                        "<td>" + new Date(mainData.reportSentDate).toLocaleString() + "</td>" +
                        "<td>" + new Date(mainData.createdDate).toLocaleString() + "</td>" +
                        "<td>" + mainData.acknowledgeReceipt.toUpperCase() + "</td>"

                }

                //populate the data into the table
                $('#populateSearchAnalyticsData').html(table_list);

                //initiate the datatable
                $('#appDataTable').DataTable({
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

                //lets add all the data counts to the cards
                $("#updatedReportSearch").html(updatedReportSearch);
                $("#pendingReportSearch").html(pendingReportSearch);
                $("#sentReportSearch").html(sentReportSearch);
                $("#acknowledgedReportSearch").html(acknowledgedReportSearch);


                //lets work on the data charts

                //show or hide
                $("#analyticsDefault").hide("fast");
                $("#searchAnalyticsTable").show("fast");


            }
            else {
                displayErrorMsgModal("Failed to get report schedule data");
            }

        },
        error: function () {
            $("#searchAnalyticsBtn").removeAttr('disabled');

            displayErrorMsgModal("Something went wrong from the server");
            console.log("Something went wrong when fetching data from the server");
        }
    });
});
