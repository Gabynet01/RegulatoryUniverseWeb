$(document).ready(function () {
    //initiate datatables and order descending
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
});

//lets handle clicks of each cards
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
