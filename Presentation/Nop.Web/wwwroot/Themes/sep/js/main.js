

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


