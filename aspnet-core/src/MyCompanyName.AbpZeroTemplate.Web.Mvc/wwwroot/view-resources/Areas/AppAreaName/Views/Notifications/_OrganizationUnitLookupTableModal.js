(function ($) {
    app.modals.OrganizationUnitLookupTableModal = function () {
        var _modalManager;
        var _organizationTree;

        this.init = function (modalManager) {
            _modalManager = modalManager;

            _organizationTree = new OrganizationTree();
            _organizationTree.init(_modalManager.getModal().find('.organization-tree'), {
                cascadeSelectEnabled: false
            });
            
            _modalManager.getModal().find('.save-button').click(function () {
                var selectedItems = _organizationTree.getSelectedOrganizations();
                _modalManager.setResult(selectedItems);
                _modalManager.close();
            });
        };
    };
})(jQuery);
