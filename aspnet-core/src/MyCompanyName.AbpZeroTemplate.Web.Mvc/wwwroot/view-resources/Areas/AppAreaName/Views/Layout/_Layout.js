(function ($) {
    $(function () {
        function scrollToCurrentMenuElement() {
            if (!$('#kt_aside_menu').length) {
                return;
            }

            var path = location.pathname;
            var menuItem = $("a[href='" + path + "']");
            if (menuItem && menuItem.length) {
                menuItem[0].scrollIntoView({block: 'center'});
            }
        }

        scrollToCurrentMenuElement();

        // Hide tooltips when item is clicked
        $('[data-bs-toggle="tooltip"]').click(function () {
            var tooltip = bootstrap.Tooltip.getInstance(this);
            tooltip.hide();
        });
    });
})(jQuery);
