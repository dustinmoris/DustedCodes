if (!window.dustedcodes) {
    dustedcodes = {};
}

(function ($) {
    window.dustedcodes = {
        window: {
            open: function (url, width, height) {
                width = (typeof width === "undefined") ? 520 : width;
                height = (typeof height === "undefined") ? 250 : height;
                window.open(url, "_blank", "width=" + width + ", height=" + height);
            }
        },
        menu: {
            toggle: function () {
                $("nav").slideToggle("fast", function () {
                    if ($(this).css("display") === "none") {
                        $(this).removeClass("nav-expanded").addClass("nav-collapsed").removeAttr("style");
                    } else {
                        $(this).removeClass("nav-collapsed").addClass("nav-expanded").removeAttr("style");
                    }
                });
            }
        }
    };
    $(document).on("click", "#nav-toggle-button", function () {
        dustedcodes.menu.toggle();
    });
    $(document).on("click", ".share-links > a", function (event) {
        event.defaultPrevented = true;
        dustedcodes.window.open($(this).attr("href"));
        return false;
    });
})(jQuery);