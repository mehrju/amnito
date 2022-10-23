(function ($) {
    "use strict";
    $(document).ready(function () {
        $('nav.main-nav').meanmenu({
            meanMenuOpen: '<i class="ti-menu"></i>',
            meanMenuClose: '<i class="ti-close"></i>',
            meanMenuCloseSize: '28px',
            meanScreenWidth: '991',
            meanExpandableChildren: true,
            meanMenuContainer: '.mobile-menu',
            onePage: false
        });
        $('.scroll-top').on('click', function () {
            $('html,body').animate({
                scrollTop: 0
            }, 900);
        });
        $(window).scroll(function () {
            //Scroll to top Hide > Show
            if ($(window).scrollTop() >= 500) {
                $('.scroll-top').slideDown(450);
            } else {
                $('.scroll-top').slideUp(450);
            }
        });
        $('select').niceSelect();
        $('.woo_pro_img_popup').magnificPopup({
            type: 'image',
            gallery: {
                enabled: true
            }
        });
        $('.header-searchtrigger').on('click', function () {
            $(this).find('.ti-search').toggleClass('ti-close');
            $(this).siblings('.header-searchbox').toggleClass('is-visible');
        });
        if ($(window).width() > 992) {
            $('.mega_menu>.luke-dropdown-menu').wrap('<div class="mega_menu_inner"></div>');
        }
        $(window).resize(function () {
            if ($(window).width() > 992) {
                $('.mega_menu>.luke-dropdown-menu').wrap('<div class="mega_menu_inner"></div>');
            }
        });
    });
    //preloader
    $(window).on('load', function () {
        $(".loadding-box").delay(1000).fadeOut(200);
        $(".loading-img").on('click', function () {
            $(".loading-img").fadeOut(200);
        });
    })
    $(window).on('scroll', function () {
        let navbar = $(".luke-fixed-header");
        var scroll = $(window).scrollTop();
        if (scroll < 500) {
            navbar.removeClass('fixed-header fadeInDown');
        } else {
            if (window.innerWidth >= 168) {
                navbar.addClass('fixed-header animated fadeInDown');
            }
        }
    });
})(jQuery);