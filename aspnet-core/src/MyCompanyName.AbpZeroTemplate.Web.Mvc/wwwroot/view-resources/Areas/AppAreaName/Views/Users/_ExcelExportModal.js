(function ($) {
    app.modals.ExcelExportModal = function () {
        var _modalManager;
        var _userService = abp.services.app.user;

        this.init = function (modalManager) {
            _modalManager = modalManager;
        };

        this.save = function () {
            const exportExcelInput = _modalManager.getArgs();
            exportExcelInput.selectedColumns = _modalManager.getModal().find('input[type="checkbox"]:checked').map(function () {
                return $(this).val();
            }).get();

            _userService
                .getUsersToExcel(exportExcelInput)
                .done(function (result) {
                    app.downloadTempFile(result);
                    _modalManager.close();
                });
        };

        $('#btnToggleAll').click(function (e) {
            
            e.preventDefault();
            
            // if all checkboxes are checked, uncheck them
            const checkBoxes = _modalManager.getModal().find('input[type="checkbox"]');

            if (checkBoxes.filter(':checked').length === checkBoxes.length) {
                $(this).text(app.localize('SelectAll'));
                checkBoxes.prop('checked', false);
            } else {
                $(this).text(app.localize('UnselectAll'));
                checkBoxes.prop('checked', true);
            }

        });

        $('input[type="checkbox"]').change(function (e) {
            const checkBoxes = _modalManager.getModal().find('input[type="checkbox"]');
            const btnToggleAll = $('#btnToggleAll');

            if (checkBoxes.filter(':checked').length === checkBoxes.length) {
                btnToggleAll.text(app.localize('UnselectAll'));
            } else {
                btnToggleAll.text(app.localize('SelectAll'));
            }
        });

    };
})(jQuery);
