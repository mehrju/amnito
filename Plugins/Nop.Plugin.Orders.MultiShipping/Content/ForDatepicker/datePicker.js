var datepicker = function (options) {
    var defulte = {
        limitHour: false
    };
    $.extend({}, defulte, options);
    var shamsiDate;
    var shamsiDay;
    var shamsiMonth;
    var shamsiYear;

    var today = new Date();
    var hoursNow = today.getHours();
    var minNow = today.getMinutes();
    var dd = today.getDate();
    var mm = today.getMonth() + 1; //January is 0!
    var yyyy = today.getFullYear();
    if (dd < 10) {
        dd = '0' + dd
    }
    if (mm < 10) {
        mm = '0' + mm
    }
    shamsiDate = moment(today).locale('fa');
    shamsiYear = shamsiDate.format('YYYY');
    shamsiMonth = shamsiDate.format('M');
    shamsiDay = shamsiDate.format('D');

    var shamsiToday = shamsiYear + '/' + shamsiMonth + '/' + shamsiDay;
    var TimeNow = hoursNow + ':' + minNow;

    //$('.date-picker-display').html(shamsiToday + '  ' + TimeNow );

    $('.month').WSlot({
        items: ['فروردین', 'اردیبهشت', 'خرداد', 'تیر', 'مرداد', 'شهریور', 'مهر', 'آبان', 'آذر', 'دی', 'بهمن', 'اسفند'],
        center: shamsiDate.format('M') - 1, //2
        angle: 50,
        distance: 'auto',
        displayed_length: 1,
    }).on('WSlot.change', function (e, index) {
        // console.log(index);
        initDate(index + 1, parseInt($('.year').WSlot('getText')), $('.day').WSlot('get'));
        
        updateText($(this).parent().attr('data-target'));
    });

    initDate(parseInt(shamsiMonth), parseInt(shamsiYear), parseInt(shamsiDay) - 1);
    $('.month').bind('mousewheel', function (event) {
        event.stopPropagation();
        event.preventDefault();
        var index = $('.month').WSlot('get');
        if (event.deltaY > 0)
            index++;
        else
            index--;
        if (index == 0)
            index = -1;
        $('.month').WSlot('rollTo', index);
    });
    var years = [];
    for (var i = 1398; i <= 1500; i++) {
        years.push(i);
    };

    var index = years.findIndex(x => x == shamsiYear);
    $('.year').WSlot({
        items: years,
        center: index,
        angle: 50,
        distance: 'auto',
        displayed_length: 1,
    }).on('WSlot.change', function (e, index) {
        // console.log(index);
        initDate(parseInt($('.month').WSlot('get')) + 1, parseInt($('.year').WSlot('getText')), $('.day').WSlot('get'));
        updateText($(this).parent().attr('data-target'));
    });
    $('.year').bind('mousewheel', function (event) {
        event.stopPropagation();
        event.preventDefault();
        var index = $('.year').WSlot('get');
        if (event.deltaY > 0)
            index++;
        else
            index--;
        if (index == 0)
            index = -1;
        $('.year').WSlot('rollTo', index);
    });
    function updateText(id) {
        var dd = ('0' + ($('.day').WSlot('get') + 1)).slice(-2);
        var mm = ('0' + ($('.month').WSlot('get') + 1)).slice(-2);
        var yyyy = $('.year').WSlot('getText');

        var hours = ('0' + ($('.hours').WSlot('get'))).slice(-2);
        var min = ('0' + ($('.min').WSlot('get'))).slice(-2);

        var todaystring = yyyy + '/' + mm + '/' + dd;
        var Time = hours + ':' + min;

        //dispatch_date
        $('#'+id).val(todaystring + ' ' + Time);
    }


    function initDate(month, year, selected) {
        var totalDay = daysInMonth(month, year);
        var days = [];
        for (var i = 1; i <= totalDay; i++) {
            days.push(i);
        };
        $('.day').empty().WSlot({
            items: days,
            center: selected,
            angle: 50,
            distance: 'auto',
            displayed_length: 1,
            rotation: 0
        }).off('WSlot.change').on('WSlot.change', function (e, index) {
            // console.log(index);
            updateText($(this).parent().attr('data-target'));
        });
    }
    $('.day').bind('mousewheel', function (event) {
        event.stopPropagation();
        event.preventDefault();
        var index = $('.day').WSlot('get');
        if (event.deltaY > 0)
            index++;
        else
            index--;
        if (index == 0)
            index = -1;
        $('.day').WSlot('rollTo', index);
    });
    function daysInMonth(month, year) {
        return moment.jDaysInMonth(year, parseInt(month) - 1);
    }



    $('.hours').WSlot({
        items: options.limitHour==true ? ['09', '10',
            '11', '12'] : ['00', '01', '02', '03', '04', '05', '06', '07', '08', '09', '10',
                '11', '12', '13', '14', '15', '16', '17', '18', '19', '20', '21', '22', '23'],
        center: 'center',
        distance: 60,
        angle: 50,
        distance: 'auto',
        displayed_length: 1,
        rotation: 0,
    }).on('WSlot.change', function (e, index) {
        console.log(index);
        updateText($(this).parent().attr('data-target'));
    });
    $('.hours').bind('mousewheel', function (event) {
        event.stopPropagation();
        event.preventDefault();
        var index = $('.hours').WSlot('get');
        if (event.deltaY > 0)
            index++;
        else
            index--;
        if (index == 0)
            index = -1;
        $('.hours').WSlot('rollTo', index);
    });
    $('.min').WSlot({
        items: options.limitHour == true ?['00']: ['00', '01', '02', '03', '04', '05', '06', '07', '08', '09', '10',
            '11', '12', '13', '14', '15', '16', '17', '18', '19', '20', '21', '22', '23',
            '24', '25', '26', '27', '28', '29', '30', '31', '32', '33', '34', '35', '36',
            '37', '38', '39', '40', '41', '42', '43', '44', '45', '46', '47', '48', '49',
            '50', '51', '52', '53', '54', '55', '56', '57', '58', '59', '60'],
        center: 'center',
        distance: 60,
        angle: 50,
        distance: 'auto',
        displayed_length: 1,
        rotation: 0
    }).on('WSlot.change', function (e, index) {
        console.log(index);
        updateText($(this).parent().attr('data-target'));
    });
    $('.min').bind('mousewheel', function (event) {
        event.stopPropagation();
        event.preventDefault();
        var index = $('.min').WSlot('get');
        if (event.deltaY > 0)
            index++;
        else
            index--;
        if (index == 0)
            index = -1;
        $('.min').WSlot('rollTo', index);
    });
}

