var CurrentPage = (function () {
    
    function showSelectedPrice(playType) {
        $('[data-price-type]').hide();
        $('[data-price-type=' + playType + ']').show();
    }

    function updateBuyNowLink(plan) {
        $('.buy-now').each(function(){
            var href = $(this).attr('href');
            href = href.replace(/(paymentPeriodType=)[^\&]+/, '$1' + plan);
            $(this).attr('href', href)
        });
    }
    
    var handleSelectEdition = function () {
        $('input[name=plan]').change(function () {
            showSelectedPrice($(this).val().toLowerCase());
        });
        
        showSelectedPrice("monthly");
    };

    $('[data-kt-plan]').click(function(){
        var plan = $(this).attr('data-kt-plan');
        showSelectedPrice(plan);
        updateBuyNowLink(plan);
    });
    
    function init() {
        KTApp.init();
        handleSelectEdition();
    }

    return {
        init: init,
    };
})();
