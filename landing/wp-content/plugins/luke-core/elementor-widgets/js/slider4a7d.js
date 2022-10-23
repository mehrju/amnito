;(function($){
  "use strict";
  jQuery(window).on("elementor/frontend/init", function() {
    elementorFrontend.hooks.addAction('frontend/element_ready/luke-slider.default', function($scope, $){
      $scope.find(".luke-slider").each(function(){
        function mainSlider() {
          var BasicSlider = $('.luke-slider');
          BasicSlider.on('init', function (e, slick) {
              var $firstAnimatingElements = $('.slide-item:first-child').find('[data-animation]');
              doAnimations($firstAnimatingElements);
          });
          BasicSlider.on('beforeChange', function (e, slick, currentSlide, nextSlide) {
              var $animatingElements = $('.slide-item[data-slick-index="' + nextSlide + '"]').find('[data-animation]');
              doAnimations($animatingElements);
          });
          BasicSlider.slick({
              infinite: true,
              autoplay: true,
              autoplaySpeed: 5000,
              speed: 1000,
              cssEase: "cubic-bezier(0.7, 0, 0.3, 1)",
              dots: true,
              fade: true,
              arrows: false,
              customPaging: function (slider, i) {
                  i++;
                  if (i < 10) {
                    return '<a class="slide-dot">0' + i + '</a>';
                  } else {
                    return '<a class="slide-dot>' + i + '</a>'
                  }
                },
              
          });
      
          function doAnimations(elements) {
              var animationEndEvents = 'animationend';
              elements.each(function () {
                  var $this = $(this);
                  var $animationDelay = $this.data('delay');
                  var $animationType = 'animated ' + $this.data('animation');
                  $this.css({
                      'animation-delay': $animationDelay,
                      '-webkit-animation-delay': $animationDelay
                  });
                  $this.addClass($animationType).one(animationEndEvents, function () {
                      $this.removeClass($animationType);
                  });
              });
          }
      }
      mainSlider();
      });
    });
  });

	$( document ).ready(function() {
/*

        $('.luke-slider').slick({
          dots: true,
          arrows: false,
          infinite: true,
          slidesToShow: 1,
          slidesToScroll: 1,
          fade: true,
          speed: 1000,
          autoplay: true,
          autoplaySpeed: 5000,
          customPaging: function (slider, i) {
            i++;
            if (i < 10) {
              return '<a class="slide-dot">0' + i + '</a>';
            } else {
              return '<a class="slide-dot>' + i + '</a>'
            }
          },
        });

    
        $('.luke-slider .slick-current').each(function () {
          $(this).addClass($(this).attr('data-animation-in'));
        });
    
        $('.luke-slider').on('beforeChange', function (event, slick, currentIndex, nextIndex) {
          var nextSlide = $(this).find("div[data-slick-index='" + nextIndex + "']");
          var next_slide = nextSlide.find('.animated');
          var current_slide = $(this).find('.slick-current .animated');
    
          current_slide.each(function () {
            var $this = $(this);
            $this.removeClass($this.attr('data-animation-in'));
          });
    
          next_slide.each(function () {
            var $this = $(this);
            if ($this.attr('data-animation-delay')) {
              setTimeout(function () {
                $this.removeClass($this.attr('data-animation-out')).addClass($this.attr('data-animation-in'));
              }, $this.attr('data-animation-delay'));
            } else {
              $this.removeClass($this.attr('data-animation-out')).addClass($this.attr('data-animation-in'));
            }
          });
    
        });

*/
        


        







       
    });

})(jQuery);