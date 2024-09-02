
function showLoading() {
    LoadingPanel.Show();
}

function hideLoading(parameters) {
    LoadingPanel.Hide();
}

function btn_login_click() {
     
    var userData = {
        username: $("#username").val(),
        password: md5($("#password").val()),
        rememberMe: $("#rememberMe").prop("checked")
    };

    $.ajax({
        url: 'Login/ValidateLogin',
        type: 'post',
        data: userData,
        async: true,
        cache: false,
        error: function (e) {
            console.log(e);
        },
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
         //   console.log(result);
            if (result.valid) {
                //console.log("../../");
                window.location.href = "/";
                //$(document).fullScreen(true);
            }
            else {
                hideLoading();
                $("#message #text").html(result.message);
                $("#message").css("display", "");
                //showNotification(result.Message, 'error');
            }
        },
        complete: function () {
            
        }
    });
}

function btn_setup_click() {
    $.ajax({
        url: "InstallWizard/Index",
        type: "post",
        data: null,
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            $("body").removeClass("login-layout light-login");
            $("body").html(result);
            //$("body").addClass("login-layout light-login");
        },
        complete: function () {
            hideLoading();
        }
    });
}

jQuery(function ($) {
    $('#btn-login-dark').on('click', function (e) {
        $('body').attr('class', 'login-layout');
        $('#id-text2').attr('class', 'white');
        $('#id-company-text').attr('class', 'blue');

        e.preventDefault();
    });
    $('#btn-login-light').on('click', function (e) {
        $('body').attr('class', 'login-layout light-login');
        $('#id-text2').attr('class', 'grey');
        $('#id-company-text').attr('class', 'blue');

        e.preventDefault();
    });
    $('#btn-login-blur').on('click', function (e) {
        $('body').attr('class', 'login-layout blur-login');
        $('#id-text2').attr('class', 'white');
        $('#id-company-text').attr('class', 'light-blue');

        e.preventDefault();
    });

    $("#username").on("focus", function () {
        $("#message").css("display", "none");
    });

    $("#username").on("blur", function() {
        $("#message").css("display", "none");
    });

    $("#password").on("focus", function () {
        $("#message").css("display", "none");
    });

    $("#password").on("blur", function () {
        $("#message").css("display", "none");
    });

    $('#btn_login').click(btn_login_click);

    $("#btnSetupWizard").click(btn_setup_click);

    $(window).keypress(function (event) {
        if(event.keyCode === 13) {
            btn_login_click();}
    });
});
