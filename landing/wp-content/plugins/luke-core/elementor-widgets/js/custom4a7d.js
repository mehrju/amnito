(function($){
	"use strict";
	$(window).load(function() {
		$('.overlay-icon, .portfolio-material-img').magnificPopup({
			delegate: 'a.luke-zoom-btn',
			removalDelay: 300,
			type: 'image',
			gallery: {
				enabled: true
			},
			callbacks: {
				beforeOpen: function() {
					this.st.image.markup = this.st.image.markup.replace('mfp-figure', 'mfp-figure animated ' + this.st.el.attr('data-effect'));
				}
			},
		});	

	});
	$(document).ready(function () {
		$('.overlay-video-icon').magnificPopup({
			delegate: 'a',
			removalDelay: 300,
			type: 'iframe',
		});
		
	});

	var reviews_slider_handler = function ($scope, $) {

        var luke_reviews_wrapper = $scope.find(".reviews-slider");
      
        if( luke_reviews_wrapper.length === 0 )
          return;
      
        var settings = luke_reviews_wrapper.data('settings'); 


    $( document ).ready(function() {
     var lukeRtl = luke_php_object.enable_rtl;
     var rtlYes = (lukeRtl == 1);
     
            $(".reviews-slider").owlCarousel({
                loop:true,
                center: settings['center'],
                margin:15,
                nav:true,
                dots:false,
                nav: true,
                rtl: rtlYes,
                autoplay: settings['autoplay'],
                autoplayTimeout: settings['testimonial_autoplay_time'],
                navText: ["<i class='ti-arrow-left'></i>", "<i class='ti-arrow-right'></i>"],
                responsive:{
                0:{
                    items: settings['luke_xs_item'],
                },
                600:{
                    items: settings['luke_md_item'],
                },
                1000:{
                    items: settings['luke_lg_item'],
                }
                }
            });
        });
    }               

    jQuery(window).on("elementor/frontend/init", function() {
        elementorFrontend.hooks.addAction('frontend/element_ready/luke-testimonials.default', reviews_slider_handler);
    });

    var client_logo_carousel_handler = function ($scope, $) {

      var luke_logo_carousel_wrapper = $scope.find(".client-logo-carousel");
    
      if( luke_logo_carousel_wrapper.length === 0 )
        return;
    
      var settings = luke_logo_carousel_wrapper.data('settings'); 


  $( document ).ready(function() {
    var lukeRtl = luke_php_object.enable_rtl;
     var rtlYes = (lukeRtl == 1);
          $(".client-logo-carousel").owlCarousel({
            loop:true,
            margin:35,
            nav:true,
            dots:false,
            nav: false,
            rtl: rtlYes,
            autoplay: true,
            autoplayTimeout: settings['client_logo_autoplay_time'],
            navText: ["<i class='ti-arrow-left'></i>", "<i class='ti-arrow-right'></i>"],
            responsive:{
            0:{
              items: settings['luke_xs_item']
            },
            600:{
              items: settings['luke_md_item']							
            },
            1000:{
              items: settings['luke_lg_item']							
            }
            }
          });
      });
  }
  jQuery(window).on("elementor/frontend/init", function() {
      elementorFrontend.hooks.addAction('frontend/element_ready/luke-clients-logo.default', client_logo_carousel_handler);
  });


    $('.luke-post-thumbnails-slider').owlCarousel({
      loop:true,
      margin:10,
      nav:true,
      items: 1,
      navText: ["<i class='ti-arrow-left'></i>", "<i class='ti-arrow-right'></i>"],
    });

})(jQuery);