(function ($) {
    app.modals.UserLookupTableModal = function () {
        var _modalManager;

        var _notificationService = abp.services.app.notification;
        var _$userTable = $('#UserTable');

        this.init = function (modalManager) {
            _modalManager = modalManager;

            _modalManager.getModal().find('.save-button').click(function () {
                var selectedItems = dataTable.rows({selected: true}).data().toArray();
                _modalManager.setResult(selectedItems);
                _modalManager.close();
            });
        };

        var dataTable = _$userTable.DataTable({
            paging: true,
            serverSide: true,
            processing: true,
            listAction: {
                ajaxFunction: _notificationService.getAllUserForLookupTable,
                inputFilter: function () {
                    return {
                        filter: $('#UserTableFilter').val(),
                    };
                },
            },
            columnDefs: [
                {
                    targets: 0,
                    data: null,
                    orderable: false,
                    defaultContent: '',
                    render: function (data) {
                        return (
                            '<label for="checkbox_' + data.value +'" class="checkbox form-check ms-5" style="width:50px">' +
                            '<input type="checkbox" id="checkbox_' +data.value +'" class="form-check-input" />&nbsp;' +
                            '<span class="form-check-label"></span>' +
                            '</label>'
                        );
                    },
                },
                {
                    autoWidth: false,
                    orderable: false,
                    targets: 1,
                    data: 'displayName',
                },
            ],
            select: {
                style: 'multi',
                info: false,
                selector: 'td:first-child label.checkbox input',
            }
        });
        
        function getUser() {
            dataTable.ajax.reload();
        }

        $('#GetUserButton').click(function (e) {
            e.preventDefault();
            getUser();
        });

        $(document).keypress(function (e) {
            if (e.which === 13) {
                getUser();
            }
        });
    };
})(jQuery);
