

/*--------step form------------*/
$(function(){
	//$("#wizard").steps({
 //       headerTag: "h2",
 //       bodyTag: "section",
 //       transitionEffect: "fade",
 //       enableAllSteps: true,
 //       transitionEffectSpeed: 500,
 //       labels: {
 //           finish: "<div style='padding-right: 20px;' class='fa fa-arrow-left'></div>تایید و مرحله بعد",
 //           next: "<div style='padding-right: 20px' class='fa fa-arrow-left'></div>تایید و مرحله بعد",
 //           previous: "<div style='' class='fa fa-arrow-right'></div> بازگشت"
 //       }
 //   });
    $('.wizard > .steps li a').click(function(){
    	$(this).parent().addClass('checked');
		$(this).parent().prevAll().addClass('checked');
		$(this).parent().nextAll().removeClass('checked');
    });
    // Custome Jquery Step Button
    $('.forward').click(function(){
    	$("#wizard").steps('next');
    })
    $('.backward').click(function(){
        $("#wizard").steps('previous');
    })
    
    // Select Dropdown
    $('html').click(function() {
        $('.select .dropdown').hide();
    });
    $('.select').click(function(event){
        event.stopPropagation();
    });
    $('.select .select-control').click(function(){
        $(this).parent().next().toggle();
    })
    $('.select .dropdown li').click(function(){
        $(this).parent().toggle();
        var text = $(this).attr('rel');
        $(this).parent().prev().find('div').text(text);
    })
});



sendEvent = function(sel, step) {
    var sel_event = new CustomEvent('next.m.' + step, {detail: {step: step}});
    window.dispatchEvent(sel_event);
};

/*--------menu-----------*/
$( ' .main-ul-menu a' ).on( 'click', function () {
    $( ' .main-ul-menu' ).find( 'li.active' ).removeClass( 'active' );
    $( this ).parent( 'li' ).addClass( 'active' );
});
$( () => {

    //On Scroll Functionality
    /*   $(window).scroll( () => {
           var windowTop = $(window).scrollTop();
           windowTop > 30 ? $('nav').addClass('navShadow') : $('nav').removeClass('navShadow');
           windowTop > 30 ? $('ul').css('top','100px') : $('ul').css('top','100px');
       });*/


    //Toggle Menu
    $('#menu-toggle').on('click', () => {
        $('#menu-toggle').toggleClass('closeMenu');
        $('ul').toggleClass('showMenu');
        $('.cc').show(500);

        $('li').on('click', () => {
            $('ul').removeClass('showMenu');
            $('#menu-toggle').removeClass('closeMenu');
        });
    });

});


$("document").ready(function () {
    var flag = false;
    $('input.checkbox_check').on('click', function () {
        if (flag === true) {
            flag = false;
            $('body').css('overflow','inherit');
        } else {
            flag = true;
            $('body').css('overflow','hidden');
        }
        console.log(flag)
    });
})


