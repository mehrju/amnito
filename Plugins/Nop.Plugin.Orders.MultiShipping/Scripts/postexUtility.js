//////راهنمای استفاده
//عبارت های کنترلی روی ورودی ها
// نکته فعال کردن عبارت های کنترلی در فرم مقصد حتما باید تابع initform در ابتدای بارگذاری صفحه فراخوانی شود

//pricefield برای فیلد های ورودی متنی که فقط اجازه وارد کردن عدد را می دهد و از جدا کننده اعداد استفاده می کند
//acceptLink برای کادرهای متنی مشخص میکند در صورتی که کلید اینتر فشرده شود معادل زدن کدام دکمه یا لینک هست





//بعدا پیاده سازی شود
//ذخیره و بازیابی اطلاعات فیلد های مربوط به یک فرم ????


function postexForm(formId) {
    var RetObj = {
        AllValidationMessage: '',
        ValidationMessages: [],
        Values: {},
        formdata: FormData,
        isok: true
    };
    var obj = {};
    var formDat = new FormData();

    $(`#${formId} input, #${formId} select, #${formId} textarea`).each(function () {

        var elemId = $(this).attr('id');
        var elemName = $(this).attr('name');
        var nameOrId = elemName ? elemName : elemId;

        var elemTag = $(this).prop("tagName").toLowerCase();
        var elemType = $(this).attr('type');

        var elemPersianName = $(this).attr('persianName');
        var elemVal;
        if (elemType == 'radio' && $(this).prop('checked') == false) return;
        if (elemType == 'checkbox')
            elemVal = $(this).prop('checked');
        else
            elemVal = $(this).val();

        var required = $(this).attr('required');
        var maxlength = $(this).attr('maxlength');
        var minlength = $(this).attr('minlength');
        var pattern =  $(this).attr('pattern');

        var errorMessage = $(this).attr('errorMessage');


        var elemMustEqualElemId = $(this).attr('mustEqualElem');
        var EqualVarName = $(this).attr('mustEqualVar');

        var pricefield = $(this).attr('pricefield');

        if (pricefield != undefined) {
            elemVal = elemVal.replaceAll(',', '');
        }
        var digitType = $(this).attr('digitType');
        if (digitType != undefined) {
            if (digitType == 'en')
                elemVal = persianDigitToEnglish(elemVal);
        }

        formDat.append(nameOrId, elemVal);
        
        if (elemType == 'file') {
            //obj['_files'] = $(this).prop('files')[0];
            formDat.append('_files', $(this).prop('files')[0]);
        }
        else obj[nameOrId] = elemVal;

        var valid = true;
        for (var i = 0; i == 0; i++) {
            if ($(this).is(":visible")) {
                if (required && (!elemVal || (elemTag == 'select' && (elemVal == 0 || elemVal == -1)))) {
                    if (elemTag == 'select') {
                        errorMessage = `انتخاب فیلد ` + (elemPersianName ? elemPersianName : elemName) + ` الزامی است`;
                    }
                    else {
                        errorMessage = `وارد کردن فیلد ` + (elemPersianName ? elemPersianName : elemName) + ` الزامی است`;
                    }
                    valid = false;
                    break;
                }
                if (maxlength && elemVal.length > maxlength) {
                    if (!errorMessage) errorMessage = `طول رشته ی  ` + (elemPersianName ? elemPersianName : elemName) + ` نباید از ` + maxlength + ` بیشتر شود`;

                    valid = false;
                    break;
                }
                if (minlength && elemVal.length < minlength) {
                    if (!errorMessage) errorMessage = `طول رشته ی  ` + (elemPersianName ? elemPersianName : elemName) + ` نباید از ` + minlength + ` کمتر شود`;

                    valid = false;
                    break;
                }
                if (pattern && new RegExp(pattern).test(elemVal) == false) {
                    if (!errorMessage) errorMessage = `عبارت وارد شده برای  ` + (elemPersianName ? elemPersianName : elemName) + ` صحیح نمی باشد `;

                    valid = false;
                    break;
                }
                if (EqualVarName && window[EqualVarName] != elemVal) {
                    if (!errorMessage) errorMessage = 'خطا در فیلد ' + nameOrId;
                    valid = false;
                    break;
                }
            }
           
        }//End of for

        if (!valid) {
            RetObj.isok = false;
            RetObj.ValidationMessages.push(errorMessage);
            RetObj.AllValidationMessage += `${errorMessage}\n`;
        }
    });
    debugger;
    RetObj.Values = obj;
    RetObj.formdata = formDat;
    return RetObj;
}


function elementChange(el) {
    var elemId = $(el).attr('id');
    var elemName = $(el).attr('name');
    var nameOrId = elemName ? elemName : elemId;
    var elemTag = $(el).prop("tagName");
    var elemType = $(el).attr('type');
    var elemPersianName = $(el).attr('persianName');
    var required = $(el).attr('required');
    var maxlength = $(el).attr('maxlength');
    var minlength = $(el).attr('minlength');
    var pattern = $(el).attr('pattern');
    var errorMessage = $(el).attr('errorMessage');
    var elemVal = $(el).val();
    var valid = true;

    for (var i = 0; i == 0; i++) {
        if (required && (!elemVal || (elemTag == 'select' && (elemVal == 0 || elemVal == -1)))) {
            if (elemTag == 'select') {
                errorMessage = `انتخاب فیلد ` + (elemPersianName ? elemPersianName : elemName) + ` الزامی است`;
            }
            else {
                errorMessage = `وارد کردن فیلد ` + (elemPersianName ? elemPersianName : elemName) + ` الزامی است`;
            }
            valid = false;
            break;
        }
        if (maxlength && elemVal.length > maxlength) {
            if (!errorMessage) errorMessage = `طول رشته ی  ` + (elemPersianName ? elemPersianName : elemName) + ` نباید از ` + maxlength + ` بیشتر شود`;
            valid = false;
            break;
        }
        if (minlength && elemVal.length < minlength) {
            if (!errorMessage) errorMessage = `طول رشته ی  ` + (elemPersianName ? elemPersianName : elemName) + ` نباید از ` + minlength + ` کمتر شود`;
            valid = false;
            break;
        }
        if (pattern && new RegExp(pattern).test(elemVal) == false) {
            if (!errorMessage) errorMessage = `عبارت وارد شده برای  ` + (elemPersianName ? elemPersianName : elemName) + ` صحیح نمی باشد `;
            valid = false;
            break;
        }
    }//End Of For
    if (!valid) {
        $(el).css('border', '1px solid red');  //$(el).parent().find("")
    } else {
        $(el).css('border', '1px solid green');
    }
}

var initedList = [];
function showLoadding(elem) {
    $(elem).find(`[name="loadingIndicator"]`).show();
    $(elem).attr('disabled', true);
}
function hideLoadding(elem) {
    $(elem).find(`[name="loadingIndicator"]`).hide();
    $(elem).attr('disabled', false);
}
function initForm(formId) {
    if (initedList.indexOf(formId) != -1) return;
    initedList.push(formId);
    $(`#${formId} button`).each(function () {
        var el = this;
        var loadingIndicator = $(this).attr('loadingIndicator');
        if (loadingIndicator != undefined) {
            if (loadingIndicator == null || loadingIndicator == '') loadingIndicator = 'fa fa-spinner fa-spin fa-1x fa-fw';
            $(this).append(`<i name="loadingIndicator" class="${loadingIndicator}" style="display:none;"></i>`);
            
        }
    });
    $(`#${formId} input, #${formId} textarea, #${formId} div.needEvent`).each(function () {
        var el = this;
        var elemId = $(this).attr('id');
        $(el).keyup(function () {
            elementChange(this);
        });
        var pricefield = $(this).attr('pricefield');
        if (pricefield != undefined) {
            $(el).keyup(function (inp) {

                Number = inp.target.value;
                Number += '';
                Number = Number.replaceAll(',', '');

                var matches = Number.match(/^\d+$/);

                if (matches == null) { inp.target.value = ""; return; }

                x = Number.split('.');
                y = x[0];
                z = x.length > 1 ? '.' + x[1] : '';
                var rgx = /(\d+)(\d{3})/;
                while (rgx.test(y))
                    y = y.replace(rgx, '$1' + ',' + '$2');
                inp.target.value = y + z;
            });
        }
        var acceptLink = $(this).attr('acceptLink');
        
        if (acceptLink != undefined) {
            $(this).keyup(function (event) {
                if (event.keyCode === 13) {
                    $(`#${acceptLink}`).click();
                }
            });
        }


        var evt_onVisibilityChanged = $(this).attr('evt_onVisibilityChanged');
        if (evt_onVisibilityChanged != undefined) {
            
            var timer1 = setInterval(function () {
                if (isOnScreen(el)) {
                    clearInterval(timer1);
                    window[evt_onVisibilityChanged]();
                }
            }, 500);
        }
    });

    //fill selects
    $(`#${formId} select`).each(function () {
        var elem = this;
        var elemId = $(this).attr('id');
        $(elem).change(function () {
            elementChange(this);
        });


        var elemName = $(elem).attr('name');
        var nameOrId = elemName ? elemName : elemId;
        var loadUrl = $(elem).attr('loadUrl');
        if(!loadUrl)
            return;
        var defaultVal = $(elem).attr('defaultVal');
        var parentSelectParams = $(elem).attr('parentSelectParams');
        var parentIds = [];
        if (parentSelectParams) {
            
            var spl = parentSelectParams.split(',');
            for (var i = 0; i < spl.length; i++) {
                var spl2 = spl[i].split('=');
                parentIds.push(spl2[1]);
            }

            for (var k = 0; k < parentIds.length; k++)
                $(`#${formId}`).find(`[name="${parentIds[k]}"]`).on('change',function () {
                    $(elem).html('');
                    $(elem).append(new Option('در حال بارگزاری....', '0', true, true));

                var queryParam = '?';
                for (var i = 0; i < spl.length; i++) {
                    var spl2 = spl[i].split('=');
                    queryParam += spl[i].replace(`=${spl2[1]}`, '=' + $(`#${formId}`).find(`[name="${spl2[1]}"]`).val() ) + `&`;
                }
                if (queryParam.length > 0) queryParam = queryParam.substring(0, queryParam.length - 1);
                   
                $.ajax({
                    cache: true,
                    type: "GET",
                    url: loadUrl + queryParam,
                    data: {},
                    success: function (data) {
                        $(elem).html('');
                        if (!defaultVal) {
                            $(elem).append(new Option('انتخاب کنید....', '0', true, true));
                        }
                        else {
                            $(elem).append(new Option(defaultVal, '0', true, true));
                        }
                        $.each(data, function (id, item) {
                            $(elem).append(new Option((item.Text?item.Text:item.Name), (item.Value?item.Value:item.Id), false, false));
                        });
                        $(elem).select2();
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        console.log('Failed to retrieve ' + nameOrId);
                        $(elem).css('border', '1px solid red');
                        $(elem).prop('title', thrownError);
                    }
                });
            });
        }


        //var timer2 = setInterval(function () {
            //if (isOnScreen($(`#${elemId}`))) {
             //   clearInterval(timer2);
                $.ajax({
                    cache: true,
                    type: "GET",
                    url: loadUrl,
                    data: {},
                    success: function (data) {
                        $(elem).html('');
                        if (!defaultVal) {
                            $(elem).append(new Option('انتخاب کنید....', '0', true, true));
                        }
                        else {
                            $(elem).append(new Option(defaultVal, '0', true, true));
                        }
                        $.each(data, function (id, item) {
                            $(elem).append(new Option((item.Text?item.Text:item.Name), (item.Value?item.Value:item.Id), false, false));
                        });
                        $(elem).select2();
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        console.log('Failed to retrieve ' + nameOrId);
                        $(elem).css('border', '1px solid red');
                        $(elem).prop('title', thrownError);
                    }
                });
          //  }
        //}, 500);

            

        

    });
}


function persianDigitToEnglish(input) {
    var inputstring = input;
    var persian = ["۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹"]
    var english = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9"]
    for (var i = 0; i < 10; i++) {
        var regex = new RegExp(persian[i], 'g');
        inputstring = inputstring.toString().replace(regex, english[i]);
    }
    return inputstring;
}

function isOnScreen(element) {
    var curPos = $(element).offset();
    var curTop = curPos.top;
    if (curTop == 0) return false;
    var screenHeight = $(window).height();
    return (curPos > screenHeight) ? false : true;
}