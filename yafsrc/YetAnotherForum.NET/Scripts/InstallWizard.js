(function ($) {
    $(document).ready(function () {
        $('.yafWizard').modal('show', {
            backdrop: 'static',
            keyboard: false
        });

        Ladda.bind('.buttonPrimary,.buttonInfo');

        $('.selectConnection, .selectTimeZone, .selectCulture').selectpicker();

    });

})(jQuery);