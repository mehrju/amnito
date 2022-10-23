var newCheckout = function (options) {
    var defulte = {
        document: null,
        wizard: "",
        WeightItem: "",
        InsuranceItem: "",
        SenderCountryItem: "",
        SenderStateItem: "",
        receiverCountryItem: "",
        receiverStateItem: "",
        kartonSizeItem: "",
        IsAgent: false,
        IsInCodRole:false
    }

    this.construct = function (options) {
        $('#checkOutModelItemId').val(Math.floor((Math.random() * 500) + 1));
        $.extend({}, defulte, options);
        var root = this;
        $('#btn_SaveNext').click(function () {
            
            if (checkOutModelList.length == 0) {
                alert('اطلاعاتی جهت ثبت موجود نمی باشد. مجداد اقدام به ثبت اطلاعات مرسوله کنید')
                return;
            }
            debugger;
            if ($('#discountCouponCode').val())
            {
                checkOutModelList[0].discountCouponCode = $('#discountCouponCode').val();
            }
            var sendData = JSON.stringify(checkOutModelList);
            $.ajax({
                beforeSend: function () {
                    $('.ajax-loading-block-window').show();
                },
                complete: function () {
                    $('.ajax-loading-block-window').hide();
                },
                type: "POST",
                url: "/NewCheckout/SaveNewCheckOutOrder",
                data: { "JsonCheckoutModel": sendData },
                success: function (result) {
                    if (result.success == true) {
                        
                        alert('اطلاعات شما جهت بررسی و ثبت به سامانه ارسال شد. به صفحه مشاهده صورت حساب و پرداخت هدایت خواهید شد');
                        window.location = '/order/billpayment?orderIds[0]=' + result.orderIds;
                    }
                    else {
                        alert(result.message);
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert('کاربر گرامی در زمان ثبت سفارش شما اشکالی به وجود آمده، لطفا ارتباط اینترنتی دستگاه خود را بررسی کنید');
                }
            });
        });
        // load weights
        //load WeightItems
        //if ($($(options.WeightItem).find('option')).length < 2) {
        //    $.ajax({
        //        cache: true,
        //        type: "GET",
        //        url: "/NewCheckout/getWeightItems",
        //        data: {},
        //        success: function (data) {
        //            $(options.WeightItem).append(new Option('انتخاب کنید....', '0', true, true));
        //            $.each(data, function (id, item) {
        //                $(options.WeightItem).append(new Option(item.Text, item.value, false, false));
        //            });
        //            $(options.WeightItem).select2();
        //        },
        //        error: function (xhr, ajaxOptions, thrownError) {
        //            console.log('Failed to retrieve tWeightItems.');
        //        }
        //    });
        //}
        // load Insurances
        if ($($(options.InsuranceItem).find('option')).length < 2) {
            $.ajax({
                cache: true,
                type: "GET",
                url: "/NewCheckout/getInsuranceItems",
                data: {},
                success: function (data) {
                    $.each(data, function (id, item) {
                        $(options.InsuranceItem).append(new Option(item.Text, item.Value, false, false));
                    });
                    $(options.InsuranceItem).select2();
                    $(".select2-container").tooltip({
                        title: function () {
                            return $(this).prev().attr("title");
                        },
                        placement: "auto"
                    });
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    console.log('Failed to retrieve InsuranceItems.');
                }
            });
        }
        //load kartonSizeItem
        if ($($(options.kartonSizeItem).find('option')).length < 2) {
            $.ajax({
                cache: true,
                type: "GET",
                url: "/NewCheckout/getKartonItems",
                data: {},
                success: function (data) {
                    
                    $.each(data, function (id, item) {
                        $(options.kartonSizeItem).append(new Option(item.Text, item.Value, false, false));
                    });
                    $(options.kartonSizeItem).select2();
                    $(".select2-container").tooltip({
                        title: function () {
                            return $(this).prev().attr("title");
                        },
                        placement: "auto"
                    });
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    console.log('Failed to retrieve tWeightItems.');
                }
            });
        }
        //====================================Load Country & State==========================================================
        $.ajax({
            cache: true,
            type: "GET",
            url: "/NewCheckout/getCountryList",
            data: {},
            success: function (data) {
                $(options.SenderCountryItem).append(new Option('انتخاب کنید....', '0', true, true));
                $(options.receiverCountryItem).append(new Option('انتخاب کنید....', '0', true, true));
                $.each(data, function (id, item) {
                    $(options.SenderCountryItem).append(new Option(item.Text, item.Value, false, false));
                    $(options.receiverCountryItem).append(new Option(item.Text, item.Value, false, false));
                });

                $(options.SenderCountryItem).select2();
                $(options.receiverCountryItem).select2();

                $(options.SenderCountryItem).change(function () {

                    var selectedItem = $(this).val();
                    var ddlStates = $(options.SenderStateItem);
                    ddlStates.html('');
                    ddlStates.append(new Option('درحال بارگذاری....', '-1', true, true));
                    $.ajax({
                        cache: false,
                        type: "GET",
                        url: "/NewCheckout/GetStatesByCountryId",
                        data: { "countryId": selectedItem },
                        success: function (reuslt) {
                            ddlStates.html('');

                            ddlStates.append(new Option('انتخاب کنید....', '0', true, true));
                            $.each(reuslt, function (id, item) {
                                ddlStates.append(new Option(item.Text, item.Value, false, false));
                            });
                            ddlStates.select2();
                        },
                        error: function (xhr, ajaxOptions, thrownError) {
                            console.log('Failed to retrieve states.');
                        }
                    });
                });
                $(options.receiverCountryItem).change(function () {
                    var selectedItem = $(this).val();
                    var ddlStates = $(options.receiverStateItem);
                    ddlStates.html('');
                    ddlStates.append(new Option('درحال بارگذاری....', '-1', true, true));
                    $.ajax({
                        cache: false,
                        type: "GET",
                        url: "/NewCheckout/GetStatesByCountryId",
                        data: { "countryId": selectedItem },
                        success: function (result) {

                            ddlStates.html('');
                            ddlStates.append(new Option('انتخاب کنید....', '0', true, true));
                            $.each(result,
                                function (id, item) {
                                    ddlStates.append(new Option(item.Text, item.Value, false, false));
                                });
                            ddlStates.select2();
                        },
                        error: function (xhr, ajaxOptions, thrownError) {
                            console.log('Failed to retrieve states.');
                        }
                    });
                });
            },
            error: function (xhr, ajaxOptions, thrownError) {
                console.log('Failed to retrieve Countries.');
            }
        });
        //====================================Load Country & State==========================================================

        //====================================Wizard========================================================================
        var wizard = $('#wizard').steps({
            headerTag: "h2",
            bodyTag: "section",
            transitionEffect: "fade",
            enableAllSteps: true,
            transitionEffectSpeed: 500,
            labels: {
                finish: "<div style='padding-right: 20px;' class='fa fa-arrow-left faa-horizontal' id='stepFinish'></div>تایید و بازبینی اطلاعات",
                next: "<div style='padding-right: 20px' class='fa fa-arrow-left faa-passing-reverse'></div>تایید و مرحله بعد",
                previous: "<div class='fa fa-arrow-right faa-horizontal'></div> بازگشت"
            },
            onStepChanging: function (vent, currentIndex, newIndex) {
                $('.ajax-loading-block-window').show();
                if (currentIndex < newIndex) {
                    var IsValid = validateStepItem(currentIndex, newIndex);
                    if (IsValid == false)
                        return false;
                }
                if (currentIndex > newIndex) {
                    return true;
                }
                else if (currentIndex == 0 && newIndex == 1) {
                    // load ServicesInfo
                    $('#SenderStateTown').text('استان ' + $('#Sender_Country option:selected').text() + ' شهرستان ' + $('#Sender_State option:selected').text());
                    $('#ReceiverStateTown').text('استان ' + $('#receiver_Country option:selected').text() + ' شهرستان ' + $('#receiver_State option:selected').text());
                    if ($('#Sender_Country').val() == '1')
                        $('#pickUpNow').show();
                    else
                        $('#pickUpNow').hide();
                    if (HasServiceInfoChange()) {
                        var canNext = true;
                        $.ajax({
                            beforeSend: function () {
                                //$('.ajax-loading-block-window').show();
                            },
                            complete: function () {
                                // $('.ajax-loading-block-window').hide();
                            },
                            type: "GET",
                            async: false,
                            url: "/NewCheckout/getServicesInfo",
                            data: {
                                'senderCountry': $('#Sender_Country').val()
                                , 'senderState': $('#Sender_State').val()
                                , 'receiverCountry': $('#receiver_Country').val()
                                , 'receiverState': $('#receiver_State').val()
                                , 'weightItem': ($('#Weight_g').val()).replace(/,/g, '')
                                , 'AproximateValue': ($('#ApproximateValue').val()).replace(/,/g, '')
                            },
                            success: function (data) {

                                if ($(data).length == 0) {
                                    alert('با توجه به مقادیر ورودی شما در قسمت وزن،استان و شهرستان فرستنده و گیرنده سرویسی یافت نشد');
                                    $('.ajax-loading-block-window').hide();
                                    canNext = false;
                                    return false;
                                }
                                canNext = true;
                                var tbody = $('#tblServiceInfo').find('tbody');
                                tbody.html('');
                                $.each(data, function (id, item) {
                                    tbody.append(`<tr>
                                            <td><input type="radio" data-val="${item.ServiceId}" data-IsCod="${item.IsCod}" name="radioGroup"></td>
                                            <td>
                                               ${item.ServiceName}
                                            </td>
                                            <td>${item.SLA} روز کاری</td>
                                            <td>${item._Price} ريال</td>
                                        </tr>`);
                                });
                            },
                            error: function (xhr, ajaxOptions, thrownError) {
                                alert('به دلیل قطع ارتباط دستگاه شما با سامانه، لیست سرویس ها دریافت نشد. مجددا سعی کنید');
                                $('.ajax-loading-block-window').hide();
                                canNext = false;
                                return false;
                            }
                        });
                        return canNext;
                    }
                    return true;
                }
                else if (currentIndex == 1 && newIndex == 2) {
                    if ($('#tblServiceInfo').find('input[type=radio]:checked').attr('data-IsCod') == 'true') {
                        if (confirm('آیا مبلغ کالای شما باید از گیرنده گرفته شود؟')) {
                            $('#CodGoodsPrice').val($('#ApproximateValue').val());
                            
                        }
                        else {
                            $('#CodGoodsPrice').val('');
                        }
                    }
                }
                return true;
            },
            onStepChanged: function (event, currentIndex, priorIndex) {
                $('.ajax-loading-block-window').hide();
            },
            onFinishing: function (event, currentIndex) {
                var IsValid = validateStepItem(2, 3);
                if (IsValid == false)
                    return false;
                var currentItem = validate_SaveLocal($('#checkOutModelItemId').val());
                if (!currentItem) {
                    alert('اطلاعات وارد شده دارای نقص می باشد');
                    return false;
                }
                fillReViewData(currentItem);
                return true;
            },
            onFinished: function (event, currentIndex) {
                $('#orderSteps').hide(250);
                $('body').css('background', '#FFFFFF');
                $('#ReViewData').show(250);
            }
        });
        $('#bnt_gotoOrdersStep').click(function () {
            $('#ReViewData').hide(250);
            $('body').css('background', '#bae1fe');
            $('#orderSteps').show(250);
            return false;
        });
        $('.editStep').click(function () {
            var step = $(this).attr('data-val');
            wizard.steps("setStep", 1);
            $('#ReViewData').hide(250);
            $('body').css('background', '#bae1fe');
            $('#orderSteps').show(250);
        });
        //====================================Wizard========================================================================

        //====================================Addresses=====================================================================
        senderAddress = {};
        receiverAddress = {};
        _senderAddressArr = [];
        _reciverAddressArr = [];
        $('#senderAddress').select2({
            placeholder: "جستجو آدرس های موجود یا ثبت آدرس ->",
            minimumInputLength: 3,
            ajax:
            {
                url: "/NewCheckout/FetchAddress",
                dataType: 'json',
                quietMillis: 250,
                data: function (term, page) {
                    return { 'countryId': $(options.SenderCountryItem).val(), 'stateId': $(options.SenderStateItem).val(), 'searchtext': term.term };
                },
                processResults: function (data) {
                    _senderAddressArr = data.results;
                    return {
                        results: $.map(data.results, function (e) { return { id: e.id, text: e.text } })
                    };
                },
                cache: false
            }
        });
        
        $('#senderAddress').on('change', function (e) {

            var id = $(this).val();
            var selectedAddressitem = _senderAddressArr.filter(x => x.id === id);
            senderAddress = {};
            $.extend(senderAddress, selectedAddressitem[0]);
            fillAddress(selectedAddressitem[0], 'Sender');
        });

        $('#receiverAddress').select2({
        placeholder: "جستجو در آدرس های قبلی یا ثبت آدرس ->",
            minimumInputLength: 3,
            ajax:
            {
                url: "/NewCheckout/FetchAddress",
                dataType: 'json',
                quietMillis: 250,
                data: function (term, page) {

                    return { 'countryId': $(options.receiverCountryItem).val(), 'stateId': $(options.receiverStateItem).val(), 'searchtext': term.term };
                },
                processResults: function (data) {

                    _reciverAddressArr = data.results;
                    return {
                        results: $.map(data.results, function (e) { return { id: e.id, text: e.text } })
                    };
                },
                cache: false
            }
        });
        $(".select2-container").tooltip({
            title: function () {
                return $(this).prev().attr("title");
            },
            placement: "auto"
        });
        $('#receiverAddress').on('change', function (e) {

            var id = $(this).val();
            var selectedAddressitem = _reciverAddressArr.filter(x => x.id === id);
            receiverAddress = {};
            $.extend(receiverAddress, selectedAddressitem[0]);
            fillAddress(receiverAddress[0], 'Reciver');
        });
        function fillAddress(selectitem, addressType) {
            ClearAddress();
            if (selectitem) {
                $('#FirstName').val(selectitem.FirstName);
                $('#LastName').val(selectitem.LastName);
                $('#Company').val(selectitem.Company);
                $('#Email').val(selectitem.Email);
                $('#PhoneNumber').val(selectitem.PhoneNumber);
                $('#ZipPostalCode').val(selectitem.ZipPostalCode);
                $('#Address1').val(selectitem.Address1);
                $('#Lat').val(selectitem.Lat);
                $('#Lon').val(selectitem.Lon);
                $('#addressType').val(addressType);
            }
        }
        function ClearAddress() {
            $('#FirstName').val('');
            $('#LastName').val('');
            $('#Company').val('');
            $('#Email').val('');
            $('#PhoneNumber').val('');
            $('#ZipPostalCode').val('');
            $('#Address1').val('');
            $('#Lat').val('');
            $('#Lon').val('');
            $('#addressType').val('');
        }
        $('.confirmAddress').click(function () {

            var $firstName = $('#FirstName');
            var $lastName = $('#LastName');
            var $company = $('#Company');
            var $email = $('#Email');
            var $phoneNumber = $('#PhoneNumber');
            var $zipPostalCode = $('#ZipPostalCode');
            var $address1 = $('#Address1');
            var $Lat = $('#Lat');
            var $Lon = $('#Lon');
            var msg = '';
            if ($firstName.val() == '')
                msg = 'وارد کردن نام الزامی می باشد' + '\r\n';
            if ($lastName.val() == '')
                msg += 'وارد کردن نام خانوادگی الزامی می باشد' + '\r\n';
            if ($phoneNumber.val() == '')
                msg += 'وارد کردن شماره موبایل الزامی می باشد' + '\r\n';
            if ($address1.val() == '')
                msg += 'وارد کردن آدرس الزامی می باشد' + '\r\n';
            if (msg != '') {
                alert(msg);
                return;
            }
            
            if ($('#addressType').val() == 'Sender') {

                senderAddress = {
                    FirstName: $firstName.val(),
                    LastName: $lastName.val(),
                    Company: $company.val(),
                    Email: $email.val(),
                    PhoneNumber: $phoneNumber.val(),
                    ZipPostalCode: $zipPostalCode.val(),
                    Address1: $address1.val(),
                    Lat: $Lat.val(),
                    Lon: $Lon.val()

                };
                senderAddress.text = (senderAddress.Address1 + ' ' + senderAddress.ZipPostalCode + ' ' + (senderAddress.Company) + ' '
                        + senderAddress.FirstName + ' ' + senderAddress.LastName + ' ' + (senderAddress.PhoneNumber));
                //if (!$('#senderAddress').val()) {
                $('#senderAddress').append(new Option(senderAddress.text, '-2', true, true));
                //}
            }
            else {

                receiverAddress = {
                    FirstName: $firstName.val(),
                    LastName: $lastName.val(),
                    Company: $company.val(),
                    Email: $email.val(),
                    PhoneNumber: $phoneNumber.val(),
                    ZipPostalCode: $zipPostalCode.val(),
                    Address1: $address1.val(),
                    Lat: $Lat.val(),
                    Lon: $Lon.val()

                };
                receiverAddress.text = (receiverAddress.Address1 + ' ' + receiverAddress.ZipPostalCode + ' ' + (receiverAddress.Company) + ' ' + receiverAddress.FirstName + ' '
                        + receiverAddress.LastName + ' ' + (receiverAddress.PhoneNumber))
                //if (!$('#receiverAddress').val()) {
                $('#receiverAddress').append(new Option(receiverAddress.text, '-2', true, true));
                //}
            }
            $('#mapModal').modal('hide');
        });

        //====================================Addresses=====================================================================

        $('.get-btn-map').click(function () {
            var _addressType = $(this).attr('data-val');
            fillAddress((_addressType == 'Sender' ? senderAddress : receiverAddress), _addressType);
            $('#mapModal').appendTo("body");
            $('#mapModal').modal('show', { backdrop: 'static' });
        });
    };
    this.construct(options);
    var checkOutModelList = [];
    //ذخیره سفارش جاری در سیستم کاربر
    function validate_SaveLocal(checkOutModelItemId) {
        var checkOutModelItem = {};
        var msg = '';
        if (!senderAddress) {
            msg = 'آدرس فرستنده به درستی وارد نشده' + ' \r\n ';
        }
        if (!receiverAddress) {
            msg = 'آدرس گیرنده به درستی وارد نشده' + ' \r\n ';
        }
        if (msg) {
            alert(msg);
            return;
        }
        
        var checkOutModelItem = checkOutModelList.find(x => x.Id == checkOutModelItemId);
        if (checkOutModelItem) {
            $.each(checkOutModelList, function () {
                debugger;
                if (this.Id == checkOutModelItemId) {
                    
                    this.Id = checkOutModelItemId;
                    this.ServiceId = $('#tblServiceInfo').find('input[type=radio]:checked').attr('data-val');

                    var SetviceType = $('#tblServiceInfo').find('input[type=radio]:checked').parent().next()
                    var slaName = SetviceType.next();
                    var servicePrice = slaName.next();
                    this._ServiceId = SetviceType.text().trim() + ' ، ' + slaName.text().trim() + ' , ' + servicePrice.text().trim();

                    this.GoodsType = $('#GoodsType').val();
                    this._GoodsType = $('#GoodsType').val();
                    this.ApproximateValue = ($('#ApproximateValue').val()).replace(/,/g, '');
                    this._ApproximateValue = $('#ApproximateValue').val() + " ريال "
                    this.CodGoodsPrice = ($('#CodGoodsPrice').val()).replace(/,/g, '');
                    this.CodGoodsPrice = (this.CodGoodsPrice == '' ? null : this.CodGoodsPrice);
                    if ($('#AgentSaleAmount').length > 0) {
                        this.AgentSaleAmount = $('#AgentSaleAmount').val().replace(/,/g, '');
                        this.AgentSaleAmount = (this.AgentSaleAmount == '' ? 0 : this.AgentSaleAmount);
                    }
                    this.Weight = ($('#Weight_g').val()).replace(/,/g, '');
                    this._Weight = $('#Weight_g').val() + " گرم ";
                    this.Count = $('#Count').val(); 
                    this._Count = $('#Count').val(); 
                    this.InsuranceName = $('#Insurance').val();
                    this._InsuranceName = $('#Insurance option:selected').text();
                    this.CartonSizeName = $('#KartonLafaf').val();
                    this._CartonSizeName = $('#KartonLafaf option:selected').text();

                    senderAddress.CountryId = $('#Sender_Country').val();
                    senderAddress.StateProvinceId = $('#Sender_State').val();
                    this.billingAddressModel = senderAddress;

                    this._billingAddressModel = $('#Sender_Country option:selected').text() + ' ' + $('#Sender_State  option:selected').text() + ' ' + senderAddress.text;

                    receiverAddress.CountryId = $('#receiver_Country').val();
                    receiverAddress.StateProvinceId = $('#receiver_State').val();
                    this.shippingAddressModel = receiverAddress;
                    this._shippingAddressModel = $('#receiver_Country option:selected').text() + ' ' + $('#receiver_State  option:selected').text() + ' ' + receiverAddress.text;


                    this.HasAccessToPrinter = $('#HasAccessToPrinter').val();
                    this._HasAccessToPrinter = $('#HasAccessToPrinter').val();
                    this.hasNotifRequest = $('#hasNotifRequest').is(':checked');
                    this._hasNotifRequest = $('#hasNotifRequest').is(':checked');
                    this.getItNow = $('#getItNow').is(':checked');
                    this._getItNow = $('#getItNow').is(':checked');
                    //this.AgentSaleAmount = $('#AgentSaleAmount').val();
                    checkOutModelItem = this;
                    return;
                }
            });
            return checkOutModelItem;
        }
        else {
            checkOutModelItem = {};
            checkOutModelItem.Id = $('#checkOutModelItemId').val();
            checkOutModelItem.ServiceId = $('#tblServiceInfo').find('input[type=radio]:checked').attr('data-val');

            var SetviceType = $('#tblServiceInfo').find('input[type=radio]:checked').parent().next()
            var slaName = SetviceType.next();
            var servicePrice = slaName.next();
            checkOutModelItem._ServiceId = SetviceType.text().trim() + ' ، ' + slaName.text().trim() + ' , ' + servicePrice.text().trim()

            checkOutModelItem.GoodsType = $('#GoodsType').val();
            checkOutModelItem._GoodsType = $('#GoodsType').val();
            checkOutModelItem.ApproximateValue = ($('#ApproximateValue').val()).replace(/,/g, '');
            checkOutModelItem._ApproximateValue = $('#ApproximateValue').val() + " ريال "

            checkOutModelItem.CodGoodsPrice = ($('#CodGoodsPrice').val()).replace(/,/g, '');
            checkOutModelItem.CodGoodsPrice = (checkOutModelItem.CodGoodsPrice == '' ? null : checkOutModelItem.CodGoodsPrice);
            if ($('#AgentSaleAmount').length > 0) {
                checkOutModelItem.AgentSaleAmount = $('#AgentSaleAmount').val().replace(/,/g, '');
                checkOutModelItem.AgentSaleAmount = (checkOutModelItem.AgentSaleAmount == '' ? 0 : checkOutModelItem.AgentSaleAmount);
            }
            checkOutModelItem.Weight = ($('#Weight_g').val()).replace(/,/g, '');
            checkOutModelItem._Weight = $('#Weight_g').val() + " گرم ";
            checkOutModelItem.Count = $('#Count').val();
            checkOutModelItem._Count = $('#Count').val();
            checkOutModelItem.InsuranceName = $('#Insurance').val();
            checkOutModelItem._InsuranceName = $('#Insurance option:selected').text();
            checkOutModelItem.CartonSizeName = $('#KartonLafaf').val();
            checkOutModelItem._CartonSizeName = $('#KartonLafaf option:selected').text();

            senderAddress.CountryId = $('#Sender_Country').val();
            senderAddress.StateProvinceId = $('#Sender_State').val();
            checkOutModelItem.billingAddressModel = senderAddress;
            checkOutModelItem._billingAddressModel = $('#Sender_Country option:selected').text() + ' ' + $('#Sender_State  option:selected').text() + ' ' + senderAddress.text;

            receiverAddress.CountryId = $('#receiver_Country').val();
            receiverAddress.StateProvinceId = $('#receiver_State').val();
            checkOutModelItem.shippingAddressModel = receiverAddress;
            checkOutModelItem._shippingAddressModel = $('#receiver_Country option:selected').text() + ' ' + $('#receiver_State  option:selected').text() + ' ' + receiverAddress.text;

            checkOutModelItem.HasAccessToPrinter = $('#HasAccessToPrinter').val();
            checkOutModelItem._HasAccessToPrinter = $('#HasAccessToPrinter').val();
            checkOutModelItem.hasNotifRequest = $('#hasNotifRequest').is(':checked');
            checkOutModelItem._hasNotifRequest = $('#hasNotifRequest').is(':checked');
            checkOutModelItem.getItNow = $('#getItNow').is(':checked');
            checkOutModelItem._getItNow = $('#getItNow').is(':checked');
            //checkOutModelItem.AgentSaleAmount = $('#AgentSaleAmount').val();
            checkOutModelList.push(checkOutModelItem);
            return checkOutModelItem;
        }
    }
    // پر کردن اطلاعات بازبینی سفارش
    function fillReViewData(checkOutModelItem) {
        ClearViewData();
        $('#_GoodsType').text(checkOutModelItem._GoodsType);
        $('#_ApproximateValue').text(checkOutModelItem._ApproximateValue);
        $('#_Weight').text(checkOutModelItem._Weight);
        $('#_billingAddressModel').text(checkOutModelItem._billingAddressModel);
        $('#_shippingAddressModel').text(checkOutModelItem._shippingAddressModel);
        $('#_ServiceId').text(checkOutModelItem._ServiceId);
        $('#_InsuranceName').text(checkOutModelItem._InsuranceName);
        $('#_CartonSizeName').text(checkOutModelItem._CartonSizeName);
        $('#_HasAccessToPrinter').attr('checked', checkOutModelItem._HasAccessToPrinter);
        $('#_hasNotifRequest').attr('checked', checkOutModelItem._hasNotifRequest);
        $('#_getItNow').attr('checked', checkOutModelItem._getItNow);
        $('#_Count').text(checkOutModelItem._Count);
    }
    // پاک کردن اطلاعات بازبینی سفارش
    function ClearViewData() {
        $('#_GoodsType').text('');
        $('#_ApproximateValue').text('');
        $('#_Weight').text('');
        $('#_billingAddressModel').text('');
        $('#_shippingAddressModel').text('');
        $('#_ServiceId').text('');
        $('#_InsuranceName').text('');
        $('#_CartonSizeName').text('');
        $('#_HasAccessToPrinter').removeAttr('checked');
        $('#_hasNotifRequest').removeAttr('checked');
        $('#_getItNow').removeAttr('checked');
        $('#_Count').text('');
    }
    // بررسی تغییر اطلاعات مورد نیاز جهت دریافت نوع سرویس
    function HasServiceInfoChange() {
        debugger;
        var checkOutModelItemId = $('#checkOutModelItemId').val();
        var checkOutModelItem = checkOutModelList.find(x => x.Id == checkOutModelItemId);
        if (!checkOutModelItem)
            return true;
        if (checkOutModelItem.shippingAddressModel.CountryId != $('#receiver_Country').val()
            || checkOutModelItem.shippingAddressModel.StateProvinceId != $('#receiver_State').val()
            || checkOutModelItem.billingAddressModel.CountryId != $('#Sender_Country').val()
            || checkOutModelItem.billingAddressModel.StateProvinceId != $('#Sender_State').val()
            || checkOutModelItem.Weight != $('#Weight_g').val())
            return true;
        return false;
    }
    // اعتبار سنجی هر مرحله از ویزارد
    function validateStepItem(currentIndex, newIndex) {
        var msg = '';
        if (currentIndex == 0 && newIndex == 1) {
            if ($('#GoodsType').val() == '')
                msg += '* ' + 'محتویات مرسوله را مشخص نمایید' + '\r\n'
            if ($('#Weight_g').val() == '' || parseInt(($('#Weight_g').val()).replace(/,/g, '')) == 0)
                msg += '* ' + 'وزن مرسوله را مشخص نمایید' + '\r\n'
            if ($('#ApproximateValue').val() == '' || parseInt(($('#ApproximateValue').val()).replace(/,/g, '')) == 0)
                msg += '* ' + 'ارزش ریالی مرسوله را مشخص نمایید' + '\r\n'
            if (!$('#Count').val() || parseInt($('#Count').val()) <= 0)
                msg += '* ' + 'تعداد را به درستی مشخص نمایید نمایید' + '\r\n'

            if (!$('#receiver_Country').val() || $('#Sender_Country').val() == '0')
                msg += '* ' + 'استان مبدا را مشخص نمایید' + '\r\n'
            if (!$('#receiver_State').val() || $('#Sender_State').val() == '0' || $('#Sender_State').val() == '-1')
                msg += '* ' + 'شهرستان مبدا را مشخص نمایید' + '\r\n'
            if (!$('#receiver_Country').val() || $('#receiver_Country').val() == '0')
                msg += '* ' + 'استان مقصد را مشخص نمایید' + '\r\n'
            if (!$('#receiver_State').val() || $('#receiver_State').val() == '0' || $('#receiver_State').val() == '-1')
                msg += '* ' + 'شهرستان مقصد را مشخص نمایید' + '\r\n'

            if (msg != '') {
                alert(msg);
                $('.ajax-loading-block-window').hide();
                return false;
            }
            return true;
        }
        if (currentIndex == 1 && newIndex == 2) {
            if ($('#tblServiceInfo').find('input[type=radio]:checked').length == 0)
                msg += '* ' + 'نوع سرویس را مشخص نمایید' + '\r\n';
            if (!$('#acceptRole').is(':checked'))
                msg += '* ' + 'لطفا برای ادامه تایید کنید که قوانین راخوانده و قبول دارید' + '\r\n';

            if (!isValidService($('#tblServiceInfo').find('input[type=radio]:checked').attr('data-val'))) {
                msg += '* ' + 'امکان ثبت سفارش در سرویس انتخابی برای شما فعال نیست، جهت استفاده از این سرویس با پشتیبانی سامانه {02191300250,02191090926} تماس بگیرید' + '\r\n';
            }
            if (msg != '') {
                alert(msg);
                $('.ajax-loading-block-window').hide();
                return false;
            }
        }
        if (currentIndex == 2 && newIndex == 3) {

            if (!options.IsAgent) {
                if ($('#Insurance').val() == '* انتخاب بیمه ضروری است *') {
                    msg += '* ' + 'انتخاب بیمه ضروری می باشد' + ' \r\n ';
                }
            }
            if (!senderAddress || !$('#senderAddress option:selected').text()) {
                msg += '* ' + 'آدرس فرستنده به درستی وارد نشده' + ' \r\n ';
            }
            if (!receiverAddress || !$('#receiverAddress option:selected').text()) {
                msg += '* ' + 'آدرس گیرنده به درستی وارد نشده' + ' \r\n ';
            }
            if (msg) {
                alert(msg);
                $('.ajax-loading-block-window').hide();
                return false;
            }
        }
    }
    function isValidService(serviceid) {
        _canCheckoutAsCod = false;
        $.ajax({
            beforeSend: function () {
               // $('.ajax-loading-block-window').show();
            },
            complete: function () {
               // $('.ajax-loading-block-window').hide();
            },
            type: "POST",
            async: false,
            url: "/NewCheckout/IsvalidService",
            data: { 'serviceId': serviceid },
            success: function (data) {
                _canCheckoutAsCod = data.success;
            },
            error: function (xhr, ajaxOptions, thrownError) {
                alert('به دلیل قطع ارتباط دستگاه شما با سامانه،اعتبار سنجی امکان پذیر نیست مجددا سعی کنید');
                _canCheckoutAsCod = false;
            }
        });
        return _canCheckoutAsCod;
    }
};
