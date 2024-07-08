(function () {
  app.modals.ViewRecoveryCodesModal = function () {
    var _profileService = abp.services.app.profile;

    var _modalManager;
    var _$form = null;

    this.init = function (modalManager) {
      _modalManager = modalManager;
      var $modal = _modalManager.getModal();

      _$form = $modal.find('form[name=AuthenticatorVerificationModalForm]');
      _$form.validate();

      var $verifyButton = $modal.find('#verifyButton');

      $verifyButton.click(function () {
        if (!_$form.valid()) {
          return;
        }

        var input = _$form.serializeFormToObject();
        input.code = $('#authenticatorCode').val();
        _profileService.viewRecoveryCodes(input).done(function (result) {
          _$form.hide();
          $('#recoveryCodesCard').show();

          let html = `<div class="row">`;

          result.recoveryCodes.forEach((code) => {
            html += `<div class="text-dark fs-6 fw-bold col-6">${code}</div>`;
          });

          html += `</div>
            </div>`;

          $("#recoveryCodes").html(html);

          const recoveryCodesText = result.recoveryCodes.join("\r\n");

          $("#btnDownload").on('click', () => {
            // Create txt and download
            const element = document.createElement('a');
            element.setAttribute('href', 'data:text/plain;charset=utf-8,' + encodeURIComponent(recoveryCodesText));
            element.setAttribute('download', "recovery-codes.txt");
            element.click();

            URL.revokeObjectURL(element.href);
          });

          $("#btnCopy").on('click', () => {
            // Copy to clipboard
            navigator.clipboard.writeText(recoveryCodesText);
          });
        }).always(function (error) {
            $('#authenticatorCode').val('');
          });
      });
    };
  };
})();
