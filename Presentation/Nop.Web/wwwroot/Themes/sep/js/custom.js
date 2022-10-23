$(function() {

 //services
     $('.carousel-normal').owlCarousel({
        rtl:true,
        loop:true,
        stagePadding: 0,
        margin:5,
        dots:true,
        nav:true,
        autoplay:true,
    autoplayTimeout:4000,
    autoplayHoverPause:true,
       
        responsive:{
            0:{
                items:1
            },
            360:{
                items:1
            },
            760:{
                items:3
            },
            
            1024:
            {
                items:4
            }
        }
    });

    
 //services
 $('.carousel-blog').owlCarousel({
    rtl:true,
    loop:true,
    stagePadding: 0,
    margin:5,
    dots:true,
    nav:false,
    autoplay:true,
autoplayTimeout:4000,
autoplayHoverPause:true,
   
    responsive:{
        0:{
            items:0
        },
        360:{
            items:0
        },
        760:{
            items:4
        },
        
        1024:
        {
            items:4
        }
    }
});
  


    //enamad
    $('.enamad-carousel').owlCarousel({
        rtl:true,
        loop:false,
        stagePadding: 0,
        margin:0,
        dots:true,
        nav:false,
        autoplay:false,
    autoplayTimeout:6000,
    autoplayHoverPause:true,
       
        responsive:{
            0:{
                items:1
            },
            360:{
                items:1
            },
            760:{
                items:1
            },
            
            1024:
            {
                items:3
            }
        }
    });




});

$(document).ready(function(){
    $(".owl-carousel").owlCarousel();
  });
//$('.loop').owlCarousel({
//    center: true,
//    items:1,
//    loop:true,
//    margin:10,
//    responsive:{
//        600:{
//            items:1
//        }
//    }
//});





/*----go to top button------*/


$(function() {
    var btn = $('#back-to-top');
    $(window).scroll(function() {
        if ($(window).scrollTop() > 300) {
          btn.addClass('show');
        } else {
          btn.removeClass('show');
        }
      });
      
      btn.on('click', function (e) {
        e.preventDefault();
        $('html,body').animate({
            scrollTop: 0
        }, 700);
    });
    
    });
      //Slider
    //$(document).ready() 
    //    $('.flexslider').flexslider({
    //      animation: "fade",
    //      reverse: false,
    //      animationLoop: true,
    //      slideshow: true,
    //      slideshowSpeed: 5000, 
         
    //    });
      
         
    /*--detect browser size and different size images for slider---*/
             
    var getBrowserWidth = function () {
        if (window.innerWidth < 700) {
            // Extra Small Device
            return "xs";
        } else if (window.innerWidth < 769) {
            // Small Device
            return "sm"
        } else if (window.innerWidth < 1199) {
            // Medium Device
            return "md"
        } else {
            // Large Device
            return "lg"
        }
    };
    
    var idsParam = ["1", "2","3","4"];
    $(document).ready(function () {
        
        // run test on initial page load
        checkSize();

        // run test on resize of the window
        $(window).resize(checkSize);
    });

    //Function to the css rule
    function checkSize() { 
        var device = getBrowserWidth();
        $.each(idsParam, function (index, id) {  
             
            var smallImgName = "small-img" + id+".jpg";
            var mediumImgName = "medium-img" + id + ".jpg";
            var largeImgName = "large-img" + id + ".jpg";

            var imgId = "img-size" + id;
           
            if (device === "xs") {
             
                $("#" + imgId).attr("src", "images/" + smallImgName);
            }
          else  if (device === "sm") {
                $("#" + imgId).attr("src", "images/" + mediumImgName);
            }
            else  if (device === "md") {
                $("#" + imgId).attr("src", "images/" + largeImgName);
            }
            else  if (device === "lg") {
                $("#" + imgId).attr("src", "images/" + largeImgName);
            }
        }); 
    }

    /*login steps */
    $( document ).ready(function() {
        $("#phoneNum").on("keyup",function() {
            var maxLength = $(this).attr("maxlength");
            if(maxLength == $(this).val().length) {
                $( '#continue' ).removeClass("bg-light-gray").addClass("bg-orange");
                $( '#continue' ).click(function(){
                    $('.step1-content').css('display','none');
                    $('.step3-content').css('display','block');
                }); 
            }
          })
          $("#vertifyCode").on("keyup",function() {
            var minlength = $(this).attr("minlength");
            if(minlength == $(this).val().length) {
               
                $( '#verify-continue-btn' ).click(function(){
                    $('.step1-content').css('display','none');
                    $('.step3-content').css('display','none');
                    $('.step4-content').css('display','block');
                }); 
            }
          })
          $( '.change-number' ).click(function(){
            $('.step1-content').css('display','block');
            $('.step3-content').css('display','none');
            $( '#continue' ).removeClass("bg-orange").addClass("bg-light-gray");
        }); 
      });
    


$(document).ready(function(){

    /*----open a tab from external link------*/
 $( ".favorite-link" ).click(function() {
    $('#pills-tab li:nth-child(2) a').tab('show')
});
$( ".profile-link" ).click(function() {
    $('#pills-tab li:nth-child(1) a').tab('show')
});
$( ".address-link" ).click(function() {
    $('#pills-tab li:nth-child(3) a').tab('show')
});
$( ".order-link" ).click(function() {
    $('#pills-tab li:nth-child(4) a').tab('show')
});
$( ".change-pass-link" ).click(function() {
    $('#pills-tab li:nth-child(5) a').tab('show')
});
  
  });
$("document").ready(function () {
    var flag = false;
    $('input.checkbox_check').on('click', function () {
        if (flag === true) {
            flag = false;
            $('body').css('overflow', 'inherit');
        } else {
            flag = true;
            $('body').css('overflow', 'hidden');
        }
        console.log(flag)
    });
});