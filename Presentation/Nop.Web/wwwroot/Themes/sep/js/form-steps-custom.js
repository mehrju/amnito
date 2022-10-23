/*--------step form------------*/
$(function(){
    //$("#wizard").steps({
    //    headerTag: "h2",
    //    bodyTag: "section",
    //    transitionEffect: "fade",
    //    enableAllSteps: true,
    //    transitionEffectSpeed: 500,
    //    labels: {
    //        finish: "<div style='padding-right: 20px;' class='fa fa-arrow-left'></div>تایید و مرحله بعد",
    //        next: "<div style='padding-right: 20px' class='fa fa-arrow-left'></div>تایید و مرحله بعد",
    //        previous: "<div style='' class='fa fa-arrow-right'></div> بازگشت"
    //    }
    //});
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



$('.actions li:last-child a').click(function() {
    window.location.href = 'showResults.html';
    return false;
});


$('#tableSelect tr').click(function() {
    $(this).find('td input:radio').prop('checked', true);
})

/*------tbl--form--------*/