(function () {
  app.modals.AddMemberModal = function () {
    var _modalManager;

    var _options = {
      serviceMethod: null, //Required
      title: app.localize('SelectAnItem'),
      loadOnStartup: true,
      showFilter: true,
      filterText: '',
      pageSize: app.consts.grid.defaultPageSize,
    };

    var _$table;
    var _$filterInput;
    var dataTable;

    function refreshTable() {
      dataTable.ajax.reload();
    }

    function updateSaveButtonState() {
      var rowData = dataTable.rows({ selected: true }).data().toArray();
      var $saveButton = _modalManager.getModal().find('#btnAddUsersToOrganization');
      if (rowData.length > 0) {
        $saveButton.removeAttr('disabled');
      } else {
        $saveButton.attr('disabled', 'disabled');
      }
    }

    function handleSelectAllCheckbox() {
      const selectedRowCount = dataTable.rows({"selected":true}).count();
      const totalRowCount = dataTable.rows().count();
      
      $('#select-all-members').prop('checked', selectedRowCount === totalRowCount);
    }

    this.init = function (modalManager) {
      _modalManager = modalManager;
      _options = $.extend(_options, _modalManager.getOptions().addMemberOptions);

      _$table = _modalManager.getModal().find('#addMemberModalTable');

      _$filterInput = _modalManager.getModal().find('.add-member-filter-text');
      _$filterInput.val(_options.filterText);

      dataTable = _$table.DataTable({
        paging: true,
        serverSide: true,
        processing: true,
        deferLoading: 0,
        listAction: {
          ajaxFunction: _options.serviceMethod,
          inputFilter: function () {
            return {
              filter: _$filterInput.val(),
              organizationUnitId: _modalManager.getArgs().organizationUnitId,
            };
          },
        },
        columnDefs: [
          {
            'targets': 0,
            'searchable': false,
            'orderable': false,
            'className': 'dt-body-center',
            'render': function (data){
              return '<input type="checkbox" name="id[]" value="' + $('<div/>').text(data).html() + '">';
            }
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
          },
          {
            targets: 4,
            visible: false,
            data: 'id',
          },
        ],
        select: {
          style: 'multi',
          info: false,
          selector: 'td:first-child input[type="checkbox"]',
        },
      });

      dataTable
        .on('select', function (e, dt, type, indexes) {
          updateSaveButtonState();
          handleSelectAllCheckbox();
        })
        .on('deselect', function (e, dt, type, indexes) {
          updateSaveButtonState();
          handleSelectAllCheckbox();
        });

      // Handle click on "Select all" control
      $('#select-all-members').on('click', function(){      
        const selectedRowCount = dataTable.rows({"selected":true}).count();
        const totalRowCount = dataTable.rows().count();
        const rows = dataTable.rows({ 'search': 'applied' }).nodes();

        if( selectedRowCount !== totalRowCount )
        {
          // Select all rows
          dataTable.rows().select();
          $('input[type="checkbox"]', rows).prop('checked', true);
        } else {
          // Deselect all rows
          dataTable.rows().deselect();
          $('input[type="checkbox"]', rows).prop('checked', false);
        }
        
      });
      
      
      _modalManager
        .getModal()
        .find('.add-member-filter-button')
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

      if (_options.loadOnStartup) {
        refreshTable();
      }

      _modalManager
        .getModal()
        .find('#btnAddUsersToOrganization')
        .click(function () {
          _modalManager.setResult(dataTable.rows({ selected: true }).data().toArray());
          _modalManager.close();
        });
    };
  };
})();
