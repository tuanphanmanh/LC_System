(function () {
    app.modals.TwoFactorAuthenticationModal = function () {

        let _modalManager;
        this.twoFactorEnabled = false;

        // Stepper element
        const $btnDone = $('#btnDone');
        const element = document.querySelector("#two_factor_stepper");
        const _profileService = abp.services.app.profile;

        // Initialize Stepper
        const stepper = new KTStepper(element);

        stepper.on("kt.stepper.next", function (stepper) {
            stepper.goNext(); // go next step
        });

        stepper.on("kt.stepper.previous", function (stepper) {
            stepper.goPrevious(); // go previous step
        });

        this.getRecoveryCodesHtml = function (recoveryCodes) {
            let html = `<div class="row">`;

            recoveryCodes.forEach((code) => {
                html += `<div class="text-dark fs-6 fw-bold col-6">${code}</div>`;
            });

            html += `</div>`;

            return html;
        }

        this.init = function (modalManager) {
            let _self = this;
            _modalManager = modalManager;

            let $modal = _modalManager.getModal();

            $btnDone.on('click', (e) => {
                _modalManager.close();
                if (_self.twoFactorEnabled) {
                    abp.message.success(app.localize('TwoFactorAuthenticationEnabled'));
                }
            });

            $("#authenticationCode").on('input', () => {
                const code = $("#authenticationCode").val();

                if (code.length !== 6) {
                    return;
                }

                const authenticatorKey = $("#GoogleAuthenticatorKey").val();

                const input = {
                    AuthenticatorCode: code,
                    GoogleAuthenticatorKey: authenticatorKey
                }

                $("#btnContinue").attr('data-kt-indicator', 'on');

                _profileService.updateGoogleAuthenticatorKey(input).done((result) => {
                    $("#authenticationCode").val("");
                    _self.twoFactorEnabled = true;
                    abp.event.trigger('app.profile.twoFactorAuthenticationEnabled');

                    let html = _self.getRecoveryCodesHtml(result.recoveryCodes);

                    $("#recoveryCodes").html(html);

                    const recoveryCodesText = result.recoveryCodes.join("\r\n");

                    $("#btnDownload").on('click', () => {
                        // Create txt and download
                        const element = document.createElement('a');
                        element.setAttribute('href', 'data:text/plain;charset=utf-8,' + encodeURIComponent(recoveryCodesText));
                        element.setAttribute('download', "recovery-codes.txt");
                        element.click();

                        URL.revokeObjectURL(element.href);

                        // User can continue
                        $("#btnContinue").removeAttr('disabled');
                    });

                    $("#btnCopy").on('click', () => {
                        // Copy to clipboard
                        navigator.clipboard.writeText(recoveryCodesText);

                        // User can continue
                        $("#btnContinue").removeAttr('disabled');
                    });

                    stepper.goNext();
                }).always(() => {
                    $("#btnContinue").removeAttr('data-kt-indicator');
                    $modal.find('.btn-close').remove();
                });

            });
        };
    };

})(jQuery);
