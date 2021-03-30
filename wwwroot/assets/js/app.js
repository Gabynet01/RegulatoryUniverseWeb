// GLOBAL VARIABLES DECLARATIONS

//Check for inactivity in application and logout the user
// Set timeout variables.
var timoutWarning = 240000; // Display warning in 4 Mins.
var timoutNow = 300000; // Timeout in 5 mins.
var logoutUrl = "/Login/LogoutUser"; // URL to logout page.

var warningTimer;
var timeoutTimer;

// Start timers.
function StartTimers() {
    warningTimer = setTimeout("IdleWarning()", timoutWarning);
    timeoutTimer = setTimeout("IdleTimeout()", timoutNow);
}

// Reset timers.
function ResetTimers() {
    clearTimeout(warningTimer);
    clearTimeout(timeoutTimer);
    StartTimers();
    $("#timeoutModal").modal("hide");
}

// Show idle timeout warning dialog.
function IdleWarning() {
    $("#timeoutModal").modal("show");
   
}

// Logout the user.
function IdleTimeout() {
    sessionStorage.clear();
    window.location.href = "/Login/LogoutUser";
}

//handle on browser close




// API DECLARATIONS
var loginApi = '/Login/ProcessLoginData';

// Page View Routes
var dashboardViewRoute = "/Home";

// execute this 
$(document).ready(function() {
    getStoredItem();
    $("#timeoutModal").modal("hide");
});

// On Login Check if enter key is pressed
$("#password").keyup(function(event) {
    if (event.keyCode === 13) {
        $("#loginBtn").click();
    }
});
/** RETRY BTN **/
$("#retryBtn").click(function(e) {
    location.reload();
});

/** Handle all click buttons and show loader **/
$(".showLoaderBtn").click(function (e) {
    show_loader();
    show_modal_loader();

    setTimeout(function () {
        hide_loader();
        hide_modal_loader();
    }, 30000);
});


/** Remove Item in storge when logout BTN CLICK**/
$("#logoutBtn").click(function(e) {

    swal({
            title: "Are you sure?",
            text: "",
            type: "warning",
            showCancelButton: true,
            closeOnConfirm: true,
            showLoaderOnConfirm: true,
            confirmButtonText: "Yes"
        },

        function() {
            sessionStorage.clear();
            window.location.href = "/Login/LogoutUser";
        });

});


// Logout Button on the dashboard when clicked
/** Remove Item in storge when logout BTN CLICK**/
$("#logoutDashBtn").click(function(e) {

    swal({
            title: "Are you sure?",
            text: "",
            type: "warning",
            showCancelButton: true,
            closeOnConfirm: true,
            showLoaderOnConfirm: true,
            confirmButtonText: "Yes"
        },

        function() {
            sessionStorage.clear();
            window.location.href = "/Login/LogoutUser";
        });

});


/** LOGIN API **/
$("#loginBtn").click(function(e) {
    e.preventDefault();
    show_loader();
    var username = $('#username').val();
    var password = $('#password').val();

    if ((username == "" || username == undefined) && (password == "" || password == undefined)) {
        displayErrorMsg("Username and password must be filled"); //display Error message
        return false;
    } else if (username == "" || username == undefined) {
        displayErrorMsg("Username must be filled"); //display Error message
        return false;
    } else if (password == "" || password == undefined) {
        displayErrorMsg("Password must be filled"); //display Error message
        return false;
    } else {

        $("#loginBtn").prop("disabled", true);
        var formData = {
            "username": username,
            "password": password
        };

        formData = JSON.stringify(formData);

        var request = $.ajax({
            url: loginApi,
            type: "POST",
            data: { formData: formData },
            datatype: "text"
        });

        request.done(function(data) {
            console.log(JSON.stringify(data));
            // console.log(JSON.stringify(data.RESPONSE_EXTRA["EMAIL"]));
            if (data.code == "200") {
                document.getElementById("loginForm").reset();
                $("#loginBtn").removeAttr('disabled');

                // Save data to sessionStorage
                sessionStorage.setItem('username', username);
                window.location.href = dashboardViewRoute;
                displaySuccessToast(toTitleCase(data.message), ""); //DISPLAY TOAST
            } else {
                hide_loader();
                $("#loginBtn").removeAttr('disabled');
                displayErrorMsg(toTitleCase(data.message)); //display Error message
            }
        });

        // Handle when it failed to connect
        request.fail(function(jqXHR, textStatus) {
            $("#loginBtn").removeAttr('disabled');
            displayErrorMsg("Sorry, connection to server failed. Please try again");
        });

    }

});

//query from AD to fetch all user emails based on user inputs
function sug_email(inputdiv, outputdiv) {

    var sug_inputdiv = $('#' + inputdiv + '').val();
    var sug_outputdiv = $('#' + outputdiv + '').val();
    //console.log('sugmail:');
    //console.log('inputdiv:' + inputdiv);
    //console.log('outputdiv:' + outputdiv);

    var sug_input = $('#' + inputdiv + '').val();
    if (sug_input == 0) {
        $('#' + outputdiv + '').empty();
    }
    if (sug_input.length >= 3) {
        //console.dir(sug_input);
        //showloader
        show_modal_loader();
        var settings = {
            "url": document.location.origin+"/aduser/get_adusers",
            "method": "POST",
            "timeout": 0,
            "headers": {
                "Content-Type": "application/x-www-form-urlencoded"
            },
            "data": {
                "search": sug_input
            }
        };

        $.ajax(settings).done(function (response) {
            console.log(response);
            var data = response;
            if (data.length == 0) {
                //console.log('length = 0');
                hide_modal_loader();
                $('#' + outputdiv + '').empty();
                $('#' + outputdiv + '').append('<li class=\"list-group-item\" >Account Not Found</li>');
            } else {
                hide_modal_loader();
                $('#' + outputdiv + '').empty();
                var i;
                for (i = 0; i < data.length; i++) {
                    //console.log(data[0].email);
                
                    $('#' + outputdiv + '').append('<li class=\"list-group-item\" data-id= "' + data[i].email + '" >' + data[i].email + '</li >');
                }

                $('#' + outputdiv + ' li').click(function () {
                    $('#' + inputdiv + '').val($(this).text());
                    $('#' + outputdiv + '').empty();

                });
            }

        });
    }

}


//Get stored item from storage with this function
function getStoredItem() {
    // Get saved data from sessionStorage
    //var username = sessionStorage.getItem('username');

    //if (username == null) {
    //    sessionStorage.clear();
    //    window.location.href = "/Login/LogoutUser"; 
    //}

    //var isLoggedIn = '@HttpContext.Session.GetString("isLoggedIn")';

    //console.log("isLoggedIn", isLoggedIn)

    //$("#showUsername").html(username);
}


//AJAX SETUP
$.ajaxSetup({
    timeout: 60000,
    cache: false,
    headers: {
        'Cache-Control': 'no-cache, no-store, must-revalidate',
        'Pragma': 'no-cache',
        'Expires': '0'
    }
});


/** UTILITIES FUNCTIONS STARTS HERE **/

function randomString() {
    var length = 15;
    var chars = '0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ';
    var result = '';
    for (var i = length; i > 0; --i) result += chars[Math.round(Math.random() * (chars.length - 1))];
    return result;
}

function generateInvoiceNumber() {
    var invNo = "P2/E/" + "" + Math.floor((Math.random() * 10000) + 1)
    return invNo;
}

function userRandomString() {
    var length = 10;
    var chars = '0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ';
    var result = '';
    for (var i = length; i > 0; --i) result += chars[Math.round(Math.random() * (chars.length - 1))];
    return result;
}

function itemRandomString() {
    var length = 5;
    var chars = '0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ';
    var result = '';
    for (var i = length; i > 0; --i) result += chars[Math.round(Math.random() * (chars.length - 1))];
    return "P2-" + result;
}

function categoryRandomString() {
    var length = 8;
    var chars = '0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ';
    var result = '';
    for (var i = length; i > 0; --i) result += chars[Math.round(Math.random() * (chars.length - 1))];
    return "P2-" + result;
}

//TO SENTENCE CASE
function toTitleCase(str) {
    if (str == "" || str == undefined) {
        return str
    } else {
        return str.replace(
            /\w\S*/g,
            function(txt) {
                return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase();
            }
        );
    }
}


//RETURN BOOLEAN VALUES
function getBoolean(value) {
    switch (value) {
        case true:
        case "true":
        case 1:
        case "1":
        case "on":
        case "yes":
            return true;
        default:
            return false;
    }
}

// thousand seperators
function numberWithCommas(x) {
    return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}

//round to 2 decimal place
function roundToTwo(num) {
    return +(Math.round(num + "e+2") + "e-2");
}


/**
 *Show Loader Functions
 */
function show_loader() {
    //    if (msg == '' || msg == undefined){
    //        msg="Loading...";
    //    }
    $(".loader").html('<div align="center" style="margin:0 auto; margin-top:30px;" class="text-center">' +
        '<div class="-spinner-ring -error-"></div>' +
        '</div>')
    $(".loader").show("fast");
}

/**
 *Hide Loader Functions
 */
function hide_loader() {
    $(".loader").html("")
    $(".loader").hide("fast");
}

function displaySuccessMsg(msg) {
    hide_loader();

    $(".msgAlertPlaceHolder").html("<div class='alert alert-success alert-dismissable fadeIn'><p class='text-center'>" +
        msg + "</p></div>");
    setTimeout(function() {
        $(".msgAlertPlaceHolder").html('');
    }, 7000);
}

function displayErrorMsg(msg) {
    hide_loader();
    $(".msgAlertPlaceHolder").html("<div class='alert alert-danger alert-dismissable fadeIn'><p class='text-center'>" +
        msg + "</p></div>");
    $(".loader").show('fast');
    setTimeout(function() {
        $(".msgAlertPlaceHolder").html('');
    }, 5000);
}

function displaySuccessToast(head, msg) {
    hide_loader();

    $.toast({
        heading: head,
        text: msg,
        position: 'top-right',
        loaderBg: '#060606',
        icon: 'success',
        hideAfter: 3500,
        stack: 6
    });
}


/**
 *Show Modal Loader Functions
 */
function show_modal_loader() {
    //    if (msg == '' || msg == undefined){
    //        msg="Loading...";
    //    }
    $(".modal_loader").html('<div align="center" style="margin:0 auto; margin-top:30px;" class="text-center">' +
        '<div class="-spinner-ring -error-"></div>' +
        '</div>')
    $(".modal_loader").show("fast");
}

/**
 *Hide modal Loader Functions
 */
function hide_modal_loader() {
    $(".modal_loader").html("")
    $(".modal_loader").hide("fast");
}

function displayErrorMsgModal(msg) {
    //hide loader
    hide_modal_loader();

    $(".modalAlertPlaceHolder").html("<div class='alert alert-danger alert-dismissable fadeIn'><p class='text-left'>" +
        msg + "</p></div>");
    setTimeout(function() {
        $(".modalAlertPlaceHolder").html('');
    }, 5000);
}

function displaySuccessMsgModal(msg) {
    hide_modal_loader();

    $(".modalAlertPlaceHolder").html("<div class='alert alert-success alert-dismissable fadeIn'><p class='text-center'>" +
        msg + "</p></div>");
    setTimeout(function() {
        $(".modalAlertPlaceHolder").html('');
    }, 7000);
}

function displaySuccessToastModal(head, msg) {
    hide_modal_loader();

    $.toast({
        heading: head,
        text: msg,
        position: 'top-right',
        loaderBg: '#060606',
        icon: 'success',
        hideAfter: 3500,
        stack: 6
    });
}

function addslashes(str) {
    return (str + '').replace(/[\\"']/g, '\\$&').replace(/\u0000/g, '\\0');
}

/**
 * ASCII to Unicode (decode Base64 to original data)
 * @param {string} b64
 * @return {string}
 */
function atou(b64) {
    return decodeURIComponent(escape(atob(b64)));
}
/**
 * Unicode to ASCII (encode data to Base64)
 * @param {string} data
 * @return {string}
 */
function utoa(data) {
    return btoa(unescape(encodeURIComponent(data)));
}

function htmlEntities(str) {
    return String(str).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;');
}

//validate an email if its correct

function validateEmail(email) {
    const re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(email);
}

// encrypt the data
function encryptData(dt) {
    let keyHex = CryptoJS.enc.Base64.parse(NBSSI_KEY);
    let encrypted = CryptoJS.DES.encrypt(dt, keyHex, {
        mode: CryptoJS.mode.ECB,
        padding: CryptoJS.pad.Pkcs7
    });
    return encrypted.toString();
}

// decrypt the data
function decryptData(dt) {
    let keyHex = CryptoJS.enc.Base64.parse(NBSSI_KEY);
    let decrypted = CryptoJS.DES.decrypt({
        ciphertext: CryptoJS.enc.Base64.parse(dt)
    }, keyHex, {
        mode: CryptoJS.mode.ECB,
        padding: CryptoJS.pad.Pkcs7
    });

    return JSON.parse(decrypted.toString(CryptoJS.enc.Utf8));
}