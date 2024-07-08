(function () {

  
  app.modals.MySettingsModal = function () {
    var _profileService = abp.services.app.profile;
    var _initialTimezone = null;

    var _modalManager;
    var _$form = null;
    var _currentEmail = null;

    this.init = function (modalManager) {
      _modalManager = modalManager;
      var $modal = _modalManager.getModal();
      _currentEmail = $modal.find('#EmailAddress').val();


      let twoFactorModal = new app.ModalManager({
        viewUrl: abp.appPath + 'AppAreaName/Profile/TwoFactorAuthenticationModal',
        scriptUrl: abp.appPath + 'view-resources/Areas/AppAreaName/Views/Profile/_TwoFactorAuthenticationModal.js',
        modalClass: 'TwoFactorAuthenticationModal',
      });

      $('#enableTwoFactorAuthenticationButton').on('click', (e) => {
        twoFactorModal.open();
      });

      abp.event.on('app.profile.twoFactorAuthenticationEnabled', () => {
        $('#two_factor_enabled_section').removeClass('d-none');
        $('#two_factor_disabled_section').addClass('d-none');
      });

      abp.event.on('app.profile.twoFactorAuthenticationDisabled', () => {
        $('#two_factor_enabled_section').addClass('d-none');
        $('#two_factor_disabled_section').removeClass('d-none');
      });
      
      var $viewRecoveryCodes = $modal.find('#btnViewRecoveryCodes');
      
      var viewRecoveryCodesModal = new app.ModalManager({
        viewUrl: abp.appPath + 'AppAreaName/Profile/ViewRecoveryCodesModal',
        scriptUrl: abp.appPath + 'view-resources/Areas/AppAreaName/Views/Profile/_ViewRecoveryCodesModal.js',
        modalClass: 'ViewRecoveryCodesModal',
      });

      $viewRecoveryCodes.click(function () {
        viewRecoveryCodesModal.open();
      });

      var $removeAuthenticator= $modal.find('#btnRemoveAuthenticator');

      var removeAuthenticatorModal = new app.ModalManager({
        viewUrl: abp.appPath + 'AppAreaName/Profile/RemoveAuthenticatorModal',
        scriptUrl: abp.appPath + 'view-resources/Areas/AppAreaName/Views/Profile/_RemoveAuthenticatorModal.js',
        modalClass: 'RemoveAuthenticatorModal',
      });

      $removeAuthenticator.click(function () {
        removeAuthenticatorModal.open();
      });
      
      _$form = $modal.find('form[name=MySettingsModalForm]');
      _$form.validate();

      _initialTimezone = _$form.find("[name='Timezone']").val();

      var $btnEnableGoogleAuthenticator = $modal.find('#btnEnableGoogleAuthenticator');
  
      $btnEnableGoogleAuthenticator.click(function () {
        _profileService
          .updateGoogleAuthenticatorKey()
          .done(function (result) {
            $modal.find('.google-authenticator-enable').show();
            $modal.find('.google-authenticator-disable').hide();
            $modal.find('img').attr('src', result.qrCodeSetupImageUrl);
          })
          .always(function () {
            _modalManager.setBusy(false);
          });
      });

      var $btnDisableGoogleAuthenticator = $modal.find('#btnDisableGoogleAuthenticator');

      $btnDisableGoogleAuthenticator.click(function () {
        
        let code = $modal.find('#GoogleAuthenticatorCode').val();
        
        _profileService
          .disableGoogleAuthenticator({"Code" : code})
          .done(function (result) {
            $modal.find('.google-authenticator-enable').hide();
            $modal.find('.google-authenticator-disable').show();
            $modal.find('img').attr('src', '');
          })
          .always(function () {
            _modalManager.setBusy(false);
          });
      });

      var $SmsVerification = $modal.find('#btnSmsVerification');
      var smsVerificationModal = new app.ModalManager({
        viewUrl: abp.appPath + 'AppAreaName/Profile/SmsVerificationModal',
        scriptUrl: abp.appPath + 'view-resources/Areas/AppAreaName/Views/Profile/_SmsVerificationModal.js',
        modalClass: 'SmsVerificationModal',
      });

      $SmsVerification.click(function () {
        _profileService.sendVerificationSms({ phoneNumber: $('#PhoneNumber').val() }).done(function () {
          smsVerificationModal.open({}, function () {
            $('#SpanSmsVerificationVerified').show();
            $('#btnSmsVerification').attr('disabled', true);
            _$form.find('.tooltips').tooltip();
          });
        });
      });

      _$form.find('.tooltips').tooltip();
      $('#PhoneNumber').keyup(function () {
        if ($('#savedPhoneNumber').val() != $(this).val() || $('#isPhoneNumberConfirmed').val() == false) {
          $('#SpanSmsVerificationVerified').hide();
          $('#btnSmsVerification').removeAttr('disabled');
        } else {
          $('#SpanSmsVerificationVerified').show();
          $('#btnSmsVerification').attr('disabled', true);
        }
      });
    };

    this.save = function () {
      if (!_$form.valid()) {
        return;
      }

      var profile = _$form.serializeFormToObject();

      _modalManager.setBusy(true);
      _profileService
        .updateCurrentUserProfile(profile)
        .done(function () {
          $('#HeaderCurrentUserName').text(profile.UserName);
          
          if (profile.EmailAddress !== _currentEmail) {
            abp.notify.info(app.localize('ChangeEmailRequestSentMessage'));
          } else {
          abp.notify.info(app.localize('SavedSuccessfully'));
          }
          
          _modalManager.close();

          var newTimezone = _$form.find("[name='Timezone']").val();

          if (abp.clock.provider.supportsMultipleTimezone && _initialTimezone !== newTimezone) {
            abp.message.info(app.localize('TimeZoneSettingChangedRefreshPageNotification')).done(function () {
              window.location.reload();
            });
          }
        })
        .always(function () {
          _modalManager.setBusy(false);
        });
    };
  };
})();
