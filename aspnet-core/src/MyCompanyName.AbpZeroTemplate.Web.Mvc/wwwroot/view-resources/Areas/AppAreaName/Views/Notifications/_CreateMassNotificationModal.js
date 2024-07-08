(function ($) {
    app.modals.CreateMassNotificationModal = function () {
        var _notificationService = abp.services.app.notification;

        var _modalManager;
        var _$massNotificationForm = null;
        var _selectedUsersId = [];
        var _selectedOrganizationUnits = [];

        var _userLookupTableModal = new app.ModalManager({
            viewUrl: abp.appPath + 'AppAreaName/Notifications/UserLookupTableModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/AppAreaName/Views/Notifications/_UserLookupTableModal.js',
            modalClass: 'UserLookupTableModal',
        });

        var _organizationUnitLookupTableModal = new app.ModalManager({
            viewUrl: abp.appPath + 'AppAreaName/Notifications/OrganizationUnitLookupTableModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/AppAreaName/Views/Notifications/_OrganizationUnitLookupTableModal.js',
            modalClass: 'OrganizationUnitLookupTableModal',
        });

        this.init = function (modalManager) {
            _modalManager = modalManager;

            _$massNotificationForm = _modalManager
                .getModal()
                .find('form[name=MassNotificationForm]');
            _$massNotificationForm.validate();
        };

        $('#OpenUserLookupTableButton').click(function () {
            var massNotificationInfo = _$massNotificationForm.serializeFormToObject();

            _userLookupTableModal.open(
                {
                    id: massNotificationInfo.userIds,
                    displayName: massNotificationInfo.userName
                },
                function (data) {
                    var selectedNames = '';
                    if (data) {
                        selectedNames = '[' + data.length + ' ' + app.localize('ItemsSelected') + '] ' +
                            data.map(function (item) {
                                return item.displayName;
                            }).join(', ');
                    }
                    _$massNotificationForm.find('input[name=userName]').val(selectedNames);
                    _selectedUsersId = data.map(function (item) {
                        return item.id;
                    });
                }
            );
        });

        $('#ClearUserNameButton').click(function () {
            _$massNotificationForm.find('input[name=userName]').val('');
            _selectedUsersId = [];
        });

        $('#OpenOrganizationUnitLookupTableButton').click(function () {
            var massNotificationInfo = _$massNotificationForm.serializeFormToObject();

            _organizationUnitLookupTableModal.open(
                {
                    id: massNotificationInfo.organizationUnitId,
                    displayName: massNotificationInfo.organizationUnitDisplayName
                },
                function (data) {
                    var selectedNames = '';
                    if (data) {
                        selectedNames = '[' + data.length + ' ' + app.localize('ItemsSelected') + '] ' +
                            data.map(function (item) {
                                return item.displayName;
                            }).join(', ');
                    }
                    _$massNotificationForm.find('input[name=organizationUnitDisplayName]').val(selectedNames);
                    _selectedOrganizationUnits = data.map(function (item) {
                        return item.id;
                    });
                }
            );
        });

        $('#ClearOrganizationUnitDisplayNameButton').click(function () {
            _$massNotificationForm.find('input[name=organizationUnitDisplayName]').val('');
            _selectedOrganizationUnits = [];
        });

        $('#CreateMassNotificationModal_TargetNotifiers').change(function () {
            var selectedTargetNotifiers = getSelectedTargetNotifiers();
            var isSMSSelected = selectedTargetNotifiers.indexOf("MyCompanyName.AbpZeroTemplate.Notifications.SmsRealTimeNotifier") >= 0;
            $("#targetNotifierSmsLengthInfo").toggle(isSMSSelected);
        });

        this.save = function () {
            if (!_$massNotificationForm.valid()) {
                return;
            }

            if ($('#MassNotification_UserIds').prop('required') && $('#MassNotification_UserIds').val() == '') {
                abp.message.error(app.localize('{0}IsRequired', app.localize('User')));
                return;
            }

            if (
                $('#MassNotification_OrganizationUnitId').prop('required') &&
                $('#MassNotification_OrganizationUnitId').val() == ''
            ) {
                abp.message.error(app.localize('{0}IsRequired', app.localize('OrganizationUnit')));
                return;
            }

            var targetNotifiers = getSelectedTargetNotifiers();

            var massNotification = _$massNotificationForm.serializeFormToObject();
            massNotification.userIds = _selectedUsersId;
            massNotification.organizationUnitIds = _selectedOrganizationUnits;
            massNotification.targetNotifiers = targetNotifiers;

            if (massNotification.message === '') {
                abp.message.error(app.localize('MassNotificationMessageFieldIsRequiredMessage'));
                return;
            }

            if (massNotification.targetNotifiers.length === 0) {
                abp.message.error(app.localize('MassNotificationTargetNotifiersFieldIsRequiredMessage'));
                return;
            }

            if (massNotification.userIds.length === 0 && massNotification.organizationUnitIds.length === 0) {
                abp.message.error(app.localize('MassNotificationUserOrOrganizationUnitFieldIsRequiredMessage'));
                return;
            }

            abp.message.confirm(
                app.localize('SendMassNotificationWarningMessage'),
                app.localize('AreYouSure'),
                function (isConfirmed) {
                    if (isConfirmed) {
                        _modalManager.setBusy(true);
                        _notificationService
                            .createMassNotification(massNotification)
                            .done(function () {
                                abp.notify.info(app.localize('SavedSuccessfully'));
                                _modalManager.close();
                                abp.event.trigger('app.createMassNotificationModalSaved');
                            })
                            .always(function () {
                                _modalManager.setBusy(false);
                            });
                    }
                }
            );
        };

        function getSelectedTargetNotifiers() {
            var checked = document.querySelectorAll('#CreateMassNotificationModal_TargetNotifiers :checked');
            var targetNotifiers = [...checked].map(option => option.value);
            return targetNotifiers;
        }
    };
})(jQuery);
