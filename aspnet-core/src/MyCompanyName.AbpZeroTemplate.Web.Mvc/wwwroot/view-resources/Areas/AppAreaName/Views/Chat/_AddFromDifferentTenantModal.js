(function () {
    const friendshipService = abp.services.app.friendship;
    const tenantToHostChatAllowed = abp.session.tenantId && abp.features.isEnabled('App.ChatFeature.TenantToHost')
    
    app.modals.AddFromDifferentTenantModal = function () {
        let _modalManager;
        let _$form = null;

        this.init = function (modalManager) {
            _modalManager = modalManager;

            const $modal = _modalManager.getModal();
            
            if (tenantToHostChatAllowed){
                this.tenantHostChanged($modal);
            }
            
            _$form = $modal.find('form');
            _$form.validate();
        };

        this.save = function () {
            if (!_$form.valid()) {
                return;
            }
            
            const addedFriend = _$form.serializeFormToObject();

            _modalManager.setBusy(true);

            friendshipService.createFriendshipWithDifferentTenant(addedFriend).done(function () {
                _modalManager.close();
                abp.notify.info(app.localize('FriendshipRequestAccepted'));
            }).always(function () {
                _modalManager.setBusy(false);
            });
        }
        
        this.tenantHostChanged = function ($modal) {

            const $tenancyNameInput = $modal.find('#TenancyName');
            const $switchToTenant = $modal.find('#SwitchToTenant');
            
            const tenancyNameGroup = $tenancyNameInput.closest('.tenancy-name-group');

            $switchToTenant.change(function () {
                if ($(this).is(':checked')) {
                    tenancyNameGroup.slideUp('fast');
                } else {
                    tenancyNameGroup.slideDown('fast');
                    $tenancyNameInput.val('');
                }
            });
        }
    };
})();
