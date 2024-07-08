(function () {
    let _addFromDifferentTenantModal = new app.ModalManager({
        viewUrl: abp.appPath + 'AppAreaName/Chat/AddFromDifferentTenantModal',
        scriptUrl: abp.appPath + 'view-resources/Areas/AppAreaName/Views/Chat/_AddFromDifferentTenantModal.js',
        modalClass: 'AddFromDifferentTenantModal',
    });
    
    app.modals.AddFriendModal = function () {
        var _modalManager;
        var _options = {
            serviceMethod: null, //Required
            title: app.localize('SelectAnItem'),
            loadOnStartup: true,
            showFilter: true,
            filterText: '',
            excludeCurrentUser: false,
            pageSize: app.consts.grid.defaultPageSize,
            canSelect: function (item) {
                /* This method can return boolean or a promise which returns boolean.
                 * A false value is used to prevent selection.
                 */
                return true;
            },
        };
        var _dataTable;
        var _$form = null;
        
        this.init = function (modalManager) {
            _modalManager = modalManager;
           const $modal = _modalManager.getModal();
            
            $($modal).on("click", "#AddFriendFromDifferentTenant", function (e) {
                e.preventDefault();

                _addFromDifferentTenantModal.open();
            });

            _$form = $modal.find('form');
            _$form.validate();

            if (abp.auth.isGranted("Pages.Administration.Users")){
                inializeDatatable();
            }

        };

        this.save = function () {
            if (!_$form.valid()) {
                return;
            }
            
            const addedFriend = _$form.serializeFormToObject();

            _modalManager.setBusy(true);

            friendshipService.createFriendshipForCurrentTenant(addedFriend).done(function () {
                _modalManager.close();
                abp.notify.info(app.localize('FriendshipRequestAccepted'));
            }).always(function () {
                _modalManager.setBusy(false);
            });
        }

        function refreshTable() {
            _dataTable.ajax.reload();
        }

        function selectItem(item) {
            var boolOrPromise = _options.canSelect(item);
            if (!boolOrPromise) {
                return;
            }

            if (boolOrPromise === true) {
                _modalManager.setResult(item);
                _modalManager.close();
                return;
            }

            //assume as promise
            boolOrPromise.then(function (result) {
                if (result) {
                    _modalManager.setResult(item);
                    _modalManager.close();
                }
            });
        }
        
        function inializeDatatable(){
            var _$table;
            var _$filterInput;
            
            _options = $.extend(_options, _modalManager.getOptions().lookupOptions);
            _$table = _modalManager.getModal().find('.lookup-modal-table');

            _dataTable = _$table.DataTable({
                paging: true,
                serverSide: true,
                processing: true,
                lengthChange: false,
                pageLength: _options.pageSize,
                deferLoading: _options.loadOnStartup ? null : 0,
                listAction: {
                    ajaxFunction: _options.serviceMethod,
                    inputFilter: function () {
                        return $.extend(
                            {
                                filter: _$filterInput.val(),
                                excludeCurrentUser: _options.excludeCurrentUser,
                            },
                            _modalManager.getArgs().extraFilters
                        );
                    },
                },
                columnDefs: [
                    {
                        targets: 0,
                        data: null,
                        orderable: false,
                        defaultContent: '',
                        className: 'text-center',
                        rowAction: {
                            element: $('<button/>')
                                .addClass('btn btn-icon btn-bg-light btn-active-color-primary btn-sm')
                                .attr('title', app.localize('Select'))
                                .append($('<i/>').addClass('la la-chevron-circle-right'))
                                .click(function () {
                                    var record = $(this).data();
                                    selectItem(record);
                                }),
                        },
                    },
                    {
                        targets: 1,
                        data: 'name',
                    },
                    {
                        targets: 2,
                        data: 'surname',
                    },
                    {
                        targets: 3,
                        data: 'emailAddress'
                    }
                ],
            });

            _modalManager
                .getModal()
                .find('.lookup-filter-button')
                .click(function (e) {
                    e.preventDefault();
                    refreshTable();
                });

            _modalManager
                .getModal()
                .find('.modal-body')
                .keydown(function (e) {
                    if (e.which === 13) {
                        e.preventDefault();
                        refreshTable();
                    }
                });

            _$filterInput = _modalManager.getModal().find('.lookup-filter-text');
            _$filterInput.val(_options.filterText);
        }

    };
    
    app.modals.AddFriendModal.create = function (lookupOptions) {
        return new app.ModalManager({
            viewUrl: abp.appPath + 'AppAreaName/Chat/AddFriendModal',
            scriptUrl: abp.appPath + 'view-resources/Areas/AppAreaName/Views/Chat/_AddFriendModal.js',
            modalClass: 'AddFriendModal',
            lookupOptions: lookupOptions,
        });
    };
})();
