(function () {
  app.modals.RemoveAuthenticatorModal = function () {
    let _profileService = abp.services.app.profile;

    let _modalManager;
    let _$form = null;

    this.init = function (modalManager) {
      _modalManager = modalManager;
      let $modal = _modalManager.getModal();

      _$form = $modal.find('form[name=RemoveAuthenticatorModalForm]');
      _$form.validate();

      let $verifyButton = $modal.find('#verifyButton');

      $verifyButton.click(function () {
        if (!_$form.valid()) {
          return;
        }

        let input = _$form.serializeFormToObject();
        _profileService.disableGoogleAuthenticator(input).done(function () {
          _modalManager.close();
          abp.event.trigger('app.profile.twoFactorAuthenticationDisabled');
          abp.message.success(app.localize('TwoFactorAuthenticationDisabled'));
        });
      });
    };
  };
})();
