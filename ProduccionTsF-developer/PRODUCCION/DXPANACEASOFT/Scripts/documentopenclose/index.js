function OnBtnCancelPopupOpeningCloseDocument_Click(s, e) {
    $.fancybox.close();
    if (gvOpeningCloseDocument != undefined) {
        gvOpeningCloseDocument.PerformCallback({ id_docRq: $('#id_doc').val() });
    }
}