function btn_migrarProducto_click() {
	console.log('a');
	var data = { aMigrar: 'Productos' };
	btn_migrar_click(data);
}

function btn_migrarProveedor_click() {
	console.log('a');
	var data = { aMigrar: 'Proveedores' };
	btn_migrar_click(data);
}

function btn_migrarCliente_click() {
	console.log('a');
	var data = { aMigrar: 'Clientes' };
	btn_migrar_click(data);
}

function btn_migrar_click(data) {
	$('#loading').show();
	var message = "";
	$.ajax({
		url: 'Home/Migrar',
		type: 'post',
		async: false,
		cache: false,
		data: data,
		error: function (xhr, status, error) {
			var errorMessage = xhr.responseJSON ? xhr.responseJSON.Error : 'Error desconocido';
			alert('Error: ' + errorMessage);
		},
		success: function (result) {
			if (result && result.Data ) {
				resultado = JSON.parse(result.Data);
				
				if (resultado != null) {
					if (resultado.Data.Error || resultado.Data == null) {
						alert(resultado.Data.Error);
						return;
					}
					else {
						if ((parseInt(resultado.Data.numeroCor) + parseInt(resultado.Data.numeroInc)) > 0) {
							var url = "";


							if (parseInt(resultado.Data.numeroCor) > 0) {
								if (parseInt(resultado.Data.numeroCor) == 1) {
									message = "Se replicó " + parseInt(resultado.Data.numeroCor) + " dato correctamente. ";
								} else {
									message = "Se replicaron " + parseInt(resultado.Data.numeroCor) + " datos correctamente. ";
								}

								url = "Home/DownloadDocumentosImportadosImportacion";
							}
							if (parseInt(resultado.Data.numeroInc) > 0) {
								if (parseInt(resultado.Data.numeroInc) == 1) {
									message += "Hubo " + parseInt(resultado.Data.numeroInc) + " dato con error en la replicación. "
								} else {
									message += "Hubieron " + parseInt(resultado.Data.numeroInc) + " datos con errores en la replicación. ";
								}

								url = "Home/DownloadDocumentosFallidosImportacion";
							}

							$('#download-area').html("<iframe style='height:0;width:0;border:0;' src='" + url + "'></iframe>");
						}
						else {
							alert("No se ha replicado ningún dato.");
						}
						if (message != "") {
							alert(message);
						}
					}
				}
            }

			if (result && result.Error) {
				alert('Error: ' + (result.Error ? result.Error : (result.Data ? result.Data.Error : 'Error desconocido')));
            }
		},
		complete: function () {
			$('#loading').hide();
		}
	});
}