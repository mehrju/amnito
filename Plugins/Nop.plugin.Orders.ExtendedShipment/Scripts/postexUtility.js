


function postexForm(formId) {
    var RetObj = {
        AllValidationMessage: '',
        ValidationMessages: [],
        Values: {},
        isok: true
    };
    var obj = {};

    $(`#${formId} input, #${formId} select, #${formId} textarea`).each(function () {

        var elemId = $(this).attr('id');
        var elemName = $(this).attr('name');
        var nameOrId = elemName ? elemName : elemId;

        var elemTag = $(this).prop("tagName").toLowerCase();
        var elemType = $(this).attr('type');

        var elemPersianName = $(this).attr('persianName');
        var elemVal;
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


        obj[nameOrId] = elemVal;
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
            if (EqualVarName && window[EqualVarName] != elemVal) {
                if (!errorMessage) errorMessage = 'خطا در فیلد ' + nameOrId;
                valid = false;
                break;
            }
        }//End of for

        if (!valid) {
            RetObj.isok = false;
            RetObj.ValidationMessages.push(errorMessage);
            RetObj.AllValidationMessage += `${errorMessage}\n`;
        }
    });
    
    RetObj.Values = obj;
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


function initForm(formId) {

    $(`#${formId} input, #${formId} textarea`).each(function () {

        var elemId = $(this).attr('id');
        $(`#${elemId}`).keyup(function () {
            elementChange(this);
        });
    });

    //fill selects
    $(`#${formId} select`).each(function () {

        var elemId = $(this).attr('id');
        $(`#${elemId}`).change(function () {
            elementChange(this);
        });


        var elemName = $(`#${elemId}`).attr('name');
        var nameOrId = elemName ? elemName : elemId;
        var loadUrl = $(`#${elemId}`).attr('loadUrl');
        var parentSelectParams = $(`#${elemId}`).attr('parentSelectParams');
        var parentIds = [];
        if (parentSelectParams) {
            
            var spl = parentSelectParams.split(',');
            for (var i = 0; i < spl.length; i++) {
                var spl2 = spl[i].split('=');
                parentIds.push(spl2[1]);
            }

            for (var k = 0; k < parentIds.length; k++)
                $(`#${parentIds[k]}`).on('change',function () {
                $(`#${elemId}`).html('');
                $(`#${elemId}`).append(new Option('در حال بارگزاری....', '0', true, true));

                var queryParam = '?';
                for (var i = 0; i < spl.length; i++) {
                    var spl2 = spl[i].split('=');
                    queryParam += spl[i].replace(`=${spl2[1]}`, '=' + $(`#${spl2[1]}`).val()) + `&`;
                }
                if (queryParam.length > 0) queryParam = queryParam.substring(0, queryParam.length - 1);
                   
                $.ajax({
                    cache: true,
                    type: "GET",
                    url: loadUrl + queryParam,
                    data: {},
                    success: function (data) {
                        $(`#${elemId}`).html('');
                        $(`#${elemId}`).append(new Option('انتخاب کنید....', '0', true, true));
                        $.each(data, function (id, item) {
                            $(`#${elemId}`).append(new Option(item.Text, item.Value, false, false));
                        });
                        $(`#${elemId}`).select2();
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        console.log('Failed to retrieve ' + nameOrId);
                        $(`#${elemId}`).css('border', '1px solid red');
                    }
                });
            });
        }

            $.ajax({
                cache: true,
                type: "GET",
                url: loadUrl,
                data: {},
                success: function (data) {
                    $(`#${elemId}`).html('');
                    $(`#${elemId}`).append(new Option('انتخاب کنید....', '0', true, true));
                    $.each(data, function (id, item) {
                        $(`#${elemId}`).append(new Option(item.Text, item.Value, false, false));
                    });
                    $(`#${elemId}`).select2();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    console.log('Failed to retrieve ' + nameOrId);
                    $(`#${elemId}`).css('border', '1px solid red');
                }
            });

        

    });
}

