if (!window.dustedcodes) {
    dustedcodes = {};
}

(function ($) {
    dustedcodes = {
        window: {
            open: function (url, width, height) {
                width = (typeof width === "undefined") ? 700 : width;
                height = (typeof height === "undefined") ? 300 : height;
                window.open(url, "_blank", "width=" + width + ", height=" + height);
            }
        }
    }
})(jQuery);