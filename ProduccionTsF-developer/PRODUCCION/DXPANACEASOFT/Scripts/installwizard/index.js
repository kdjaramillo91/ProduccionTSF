
function initializaWizard() {
    $('#installWizard').wizard();
}

function UpdateCompanyInfo() {
    $("#logoImage").prop("src", $(logo.GetMainElement()).find("img").prop("src"));
    $("#txtBusinessName").html(businessName.GetText());
    $("#txtRUC").html(ruc.GetText());
    $("#txtTrademark").html(trademark.GetText());
    $("#txtEmail").html(email.GetText());
    $("#txAddress").html(address.GetText());
    $("#txtPhoneNumber").html(phoneNumber.GetText());
}

function UpdateEstructureInfo() {
   
    $("#txtDivisioName").html(divisionName.GetText());
    $("#txtBranchOfficeName").html(branchOfficeName.GetText());
    $("#txtBranchOfficeCode").html(branchOfficeCode.GetText());
    $("#txtEmissionPointName").html(emissionPointName.GetText());
    $("#txtEmissionPointCode").html(emissionPointCode.GetText());
}

function UpdateSecurityInfo() {
    $("#txtUserName").html(userAdmin.GetText());
}

function UpdateModel(wizard) {
    if (wizard.currentStep === 2) {
        UpdateCompanyInfo();
    }
    if (wizard.currentStep === 3) {
        UpdateEstructureInfo();
    }
    if (wizard.currentStep === 4) {
        UpdateSecurityInfo();
    }
    if (wizard.currentStep === 5) {
        Install();
    }
}

function Install() {
    var data = $("#formInstallWizard").serialize();

    $.ajax({
        url: 'InstallWizard/Install',
        type: 'post',
        data: data,
        async: true,
        cache: false,
        error: function (error) {
            console.log(error);
        },
        beforeSend: function () {
            showLoading();
        },
        success: function (result) {
            $("#confirmation").html(result);
        },
        complete: function () {
            hideLoading();
        }
    });
}

function IsValidStep(wizard) {
    var group = null;
    switch (wizard.currentStep) {
        case 1:
            {
                group = "welcome";
                break;
            }
        case 2:
            {
                group = "company";
                break;
            }
        case 3:
            {
                group = "structure";
                break;
            }
        case 4:
            {
                group = "security";
                break;
            }
        default:;
    }

    var valid = ASPxClientEdit.ValidateEditorsInContainer(null, group, null);
    return valid;
}

function BtnPrev_Click(s, e) {
    var wizard = $('#installWizard').data('fu.wizard');
    wizard.currentStep = wizard.currentStep - 1;
    wizard.setState();
    s.SetEnabled(wizard.currentStep !== 1);
    btnNext.SetEnabled(wizard.currentStep !== wizard.numSteps);
    btnFinish.SetEnabled(wizard.currentStep === wizard.numSteps);
}

function BtnNext_Click(s, e) {
    var wizard = $('#installWizard').data('fu.wizard');

    if (IsValidStep(wizard)) {

        UpdateModel(wizard);

        wizard.currentStep = wizard.currentStep + 1;
        wizard.setState();

        btnPrev.SetEnabled(true);
    }
    
    s.SetEnabled(wizard.currentStep !== wizard.numSteps);
    btnFinish.SetEnabled(wizard.currentStep === wizard.numSteps);
}

function BtnFinish_Click(s, e) {

    

    showLoading();
    window.location.href = "/";
}

function BtnCancel_Click(s, e) {
    showLoading();
    window.location.href = "/";
}

function init() {
    initializaWizard();
}

$(function() {
    init();
});
