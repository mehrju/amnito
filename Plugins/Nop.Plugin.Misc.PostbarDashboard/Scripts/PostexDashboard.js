function loadData(url, pageIndedx1, pageSize1, elementToInsert) {
    var firsttime1 = false;
    if (elementToInsert == '#GvwWallet') firsttime1 = true;
    $.ajax({
        beforeSend: function () {

            $('#loader').show();
        },
        complete: function () {
            lazyScrollBusy = false;
            $('#loader').hide();
        },
        cache: false,
        type: "GET",
        url: url,
        data: { 'pageSize': pageSize1, 'pageIndex': pageIndedx1, firstTime: firsttime1 },
        success: function (data) {
            if (data != null) {
                $(elementToInsert).html(data);
            }
            if ($('#PageIndex').length > 0) {
                $('#PageIndex').val(pageIndedx1);
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert('Failed to retrive Code.');
            $('#loading').hide();
        }
    });
}
var dashboard = function (options) {

    var defulte = {
    }
    this.construct = function (options) {
        $.extend({}, defulte, options);



        createYourAffiliateId = function () {
            $.ajax({
                cache: true,
                type: "POST",
                url: options.createYourAffiliateIdUrl,
                data: {
                    customerId: options.customerId
                },
                success: function (response) {
                    alert(response.message);
                    if (response.success) {
                        window.location = '/dashboard#list-score';
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) {

                }
            });
        }

        openLink = function (url) {
            window.open(url, "_blank");
        }
        wallet_VisibilityChanged = function () {
            loadData('PostbarDashboard/WalletPaged', 0, 20, '#GvwWallet');
        }
        factor_VisibilityChanged = function () {
            $("#GvwFactors").kendoGrid({
                dataBound: function (e) { $('.k-grid-header').css('padding-right', '0 !important'); },
                dataSource: {
                    type: "json",
                    transport: {
                        read: {
                            url: options.GridFactorsUrl,
                            type: "POST",
                            dataType: "json",
                            data: additionalDataFactors
                        }
                    },
                    schema: {
                        data: "Data",
                        total: "Total",
                        errors: "Errors"
                    },
                    requestEnd: function (e) {
                        $("tbody").css("display", "contents");
                        if (e.type == "update") {
                            this.read();
                        }
                        $('.k-grid-header').css('padding-right', '0 !important')
                    },
                    error: function () {
                        this.cancelChanges();
                    },
                    pageSize: 10,
                    serverPaging: true,
                    serverFiltering: true,
                    serverSorting: true
                },
                pageable: {
                    refresh: true,
                    pageSizes: [10, 50, 100, 1000],

                },
                filterable: false,
                sortable: false,
                scrollable: false,
                columns: [
                    {
                        field: "Id",
                        title: " ",
                        width: 10
                    },
                    //{
                    //    field: "ShamsiCreatedDate",
                    //    title: "تاریخ ایجاد درخواست",
                    //    width: 400
                    //},
                    //{
                    //    field: "ShamsiDateFrom",
                    //    title: "تاریخ درخواست از",
                    //    width: 100
                    //},
                    //{
                    //    field: "ShamsiDateTo",
                    //    title: "تاریخ درخواست تا",
                    //    width: 100
                    //},
                    {
                        field: "ConfirmState",
                        title: "وضعیت تایید",
                        width: 100
                    },
                    {
                        headerAttributes: { style: "text-align:center" },
                        attributes: { style: "text-align:center" },
                        template: `<a data-value="#=Id#" class="btn btn-success" onclick="funDownloadFactor(this,'${options.DownloadFactorUrl}','#=SafeFileName#')"  title="دانلود فاکتور"><i class="fa fa-eye"></i></a>`,
                        title: "دانلود",
                        width: 80
                    }
                ]
            });
        }
        orders_VisibilityChanged = function () {
            setTimeout(function () {
                $(`#btnsearchOrder`).click();
            }, 1000);
        }
        txtSearchMessages_VisibilityChanged = function () {
            $("#GvwMessages").kendoGrid({
                dataBound: function (e) { $('.k-grid-header').css('padding-right', '0 !important'); },
                dataSource: {
                    type: "json",
                    transport: {
                        read: {
                            url: options.GridCustommerMessagesUrl,
                            type: "POST",
                            dataType: "json",
                            data: additionalDataMessages
                        }
                    },
                    schema: {
                        data: "Data",
                        total: "Total",
                        errors: "Errors"
                    },
                    requestEnd: function (e) {
                        $("tbody").css("display", "contents");
                        if (e.type == "update") {
                            this.read();
                        }
                        $('.k-grid-header').css('padding-right', '0 !important')
                    },
                    error: function () {
                        this.cancelChanges();
                    },
                    pageSize: 10,
                    serverPaging: true,
                    serverFiltering: true,
                    serverSorting: true
                },
                pageable: {
                    refresh: true,
                    pageSizes: [10, 50, 100, 1000],
                },
                filterable: false,
                sortable: false,
                scrollable: false,
                columns: [
                    {
                        field: "MessageId",
                        title: "شماره ",
                        width: 80,
                        template: `<span #if(!IsRead) {# style="color:red" '#}#'>#=MessageId#</span>`
                    },
                    {
                        field: "ShamsiCreatedDate",
                        title: "تاریخ",
                        width: 100,
                        template: `<span #if(!IsRead) {# style="color:red" '#}#'>#=ShamsiCreatedDate#</span>`
                    },
                    {
                        field: "Subject",
                        title: "عنوان",
                        width: 400,
                        template: `<span #if(!IsRead) {# style="color:red" '#}#'>#=Subject#</span>`
                    },
                    {
                        headerAttributes: { style: "text-align:center" },
                        attributes: { style: "text-align:center" },
                        template: `<a data-value="#=MessageId#" class="btn btn-success" onclick="funReadMessage(this,'${options.ReadCustomerMessageUrl}')"  title="مشاهده متن پیام"><i class="fa fa-eye"></i></a>`,//IsRead
                        title: "مشاهده",
                        width: 80
                    }
                ]
            });
        }
        btnSearchMessagesClick = function () {
            var grid = $('#GvwMessages').data('kendoGrid');
            grid.dataSource.page(1); //new search. Set page size to 1
            $("tbody").css("display", "contents");
        }
        txtSearchAddress_VisibilityChanged = function () {
            $("#GvwAddresses").kendoGrid({
                dataBound: function (e) { $('.k-grid-header').css('padding-right', '0 !important'); },
                dataSource: {
                    type: "json",
                    transport: {
                        read: {
                            url: options.GridCustommerAddressesUrl,
                            type: "POST",
                            dataType: "json",
                            data: additionalDataAddress
                        }
                    },
                    schema: {
                        data: "Data",
                        total: "Total",
                        errors: "Errors"
                    },
                    requestEnd: function (e) {
                        $("tbody").css("display", "contents");
                        if (e.type == "update") {
                            this.read();
                        }
                        $('.k-grid-header').css('padding-right', '0 !important')
                    },
                    error: function () {

                        this.cancelChanges();
                    },
                    pageSize: 10,
                    serverPaging: true,
                    serverFiltering: true,
                    serverSorting: true
                },
                pageable: {
                    refresh: true,
                    pageSizes: [10, 50, 100, 1000],

                },
                filterable: false,
                sortable: false,
                scrollable: false,
                //dataBound: onDataBound,
                columns: [
                    {
                        field: "AddressId",
                        title: "شماره ",
                        width: 80
                    },
                    {
                        field: "Address1",
                        title: "آدرس",
                        width: 400
                    }
                ]
            });
        }

        btnSearchAddressClick = function () {
            var grid = $('#GvwAddresses').data('kendoGrid');
            grid.dataSource.page(1); //new search. Set page size to 1
            $("tbody").css("display", "contents");
        }
        btnSearchFactorRequestsClick = function () {
            var grid = $('#GvwFactors').data('kendoGrid');
            grid.dataSource.page(1); //new search. Set page size to 1
            $("tbody").css("display", "contents");
        }

        btnSaveFactorRequestsClick = function () {

            var valids = postexForm("divFactorRequestModal");
            if (!valids.isok) {
                alert(valids.AllValidationMessage);
                return;
            }
            showLoadding(this);

            $.ajax({
                cache: true,
                type: "POST",
                url: options.SaveFactorsUrl,
                data: valids.Values,
                success: (response) => {
                    $('#divFactorRequestModal').modal('hide');
                    alert(response.message);
                    hideLoadding(this);
                },
                error: (xhr, ajaxOptions, thrownError) => {

                    hideLoadding(this);
                }
            });

        }

        var pageSize = 10;

        var pageIndex = 0;
        function loadOrderData(pageIndedx, pageSize, elementToInsert, searchOptions, withClean) {

            $.ajax({
                beforeSend: function () {

                    $('#loader').show();
                },
                complete: function () {
                    lazyScrollBusy = false;
                    $('#loader').hide();
                },
                cache: false,
                type: "GET",
                url: options.OrdersPagedUrl,
                data: { 'pageSize': pageSize, 'pageIndex': pageIndedx, 'searchConditions': searchOptions },
                success: function (data) {
                    if ($(elementToInsert).html()) {
                        if (($(elementToInsert).html()).includes('سفارشی یافت نشد') && data == 'سفارشی یافت نشد')
                            return;
                    }
                    if (withClean) {
                        $(elementToInsert).html('');
                    }
                    if (data != null) {
                        $(elementToInsert).append(data);
                    }
                    else {
                        $(elementToInsert).html('');
                    }

                },
                error: function (xhr, ajaxOptions, thrownError) {
                }
            });
        }
        onSave = function () {
            $.ajax({
                beforeSend: function () {

                    $('#loader').show();
                },
                complete: function () {
                    lazyScrollBusy = false;
                    $('#loader').hide();
                },
                async: false,
                type: "POST",
                url: options.AddPostalAddressUrl,
                data: senderAddress,
                success: function (response) {
                    if (!response.success) {
                        alert(response.error);
                        return;
                    }
                    alert('آدرس با موفقیت ثبت شد');
                    var grid = $('#GvwAddresses').data('kendoGrid');
                    grid.dataSource.read();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    $(`#divOrders`).html(xhr.error);

                }
            });
        }
        $(document).ready(function () {
            var msg = options.msg;
            if (msg) {
                alert(msg);
            }
            var activeTab = location.href;
            var activeTabId = activeTab.substring(activeTab.lastIndexOf('#'));
            if (activeTabId) {
                $(`button[href="` + activeTabId + `"]`).click();
            }
            var addresOptions = {
                document: document,
                CountryItem: '#Country',
                StateItem: '#State',
                kartonSizeItem: null,
                IsAgent: false,
                IsInCodRole: false,
                IsForegin: false,
                IsHeavy: false,
                IsCod: false
            };
            var _address = new Address(addresOptions);
            $(`#ReciveSmsPersuit`).prop('checked', options.ReciveSmsPersuit);
            $(`#ReciveOrderEmail`).prop('checked', options.ReciveOrderEmail);
            $(`#AccessToPrinter`).prop('checked', options.AccessToPrinter);
            $(`#AccessToPackage`).prop('checked', AccessToPackage);
            $(`#UseLogo`).prop('checked', options.UseLogo);



            $('#_DateFrom').MdPersianDateTimePicker({
                targetTextSelector: '#orderDateFrom',
            });

            $('#_DateTo').MdPersianDateTimePicker({
                targetTextSelector: '#orderDateTo',
            });

            $('#_frDateFrom').MdPersianDateTimePicker({
                targetTextSelector: '#frDateFrom',
            });

            $('#_frDateTo').MdPersianDateTimePicker({
                targetTextSelector: '#frDateTo',
            });

            $('#_DateFrom2').MdPersianDateTimePicker({
                targetTextSelector: '#DateFrom2',
            });

            $('#_DateTo2').MdPersianDateTimePicker({
                targetTextSelector: '#DateTo2',
            });

            initForm("divPersonelInfo");
            initForm("divaddresses");
            initForm("list-settings");
            initForm("list-arch");
            initForm("list-repo");
            initForm("list-flatbed");
            initForm("list-message");
            initForm("list-factor");
            initForm("divChargePrice");
            initForm("list-trans");
            initForm("list-medkitAdd");
            initForm("list-squareAdd");


            $(`#btnRegisterCodRequest`).click(function () {
                $('#isFromApp').val(isAndroid());
                var valids = postexForm("list-flatbed");
                if (!valids.isok) {
                    alert(valids.AllValidationMessage);
                    return false;
                }
                return true;
            });

            $(`#btnInsertTicketClick`).click(function () {
                debugger;
                var valids = postexForm("list-squareAdd");
                if (!valids.isok) {
                    alert(valids.AllValidationMessage);
                    return;
                }

                //showLoadding(this);
                //hideshowLoadding(this);

            });

            $(`#btnDownloadOrderReport`).click(function () {
                debugger;
                var ty = $(`#reportDownloadType`).val();
                if (ty == 1) downloadOrdersPagedExcel();
                else if (ty == 2) downloadOrderBillDetailExcel();
                else if (ty == 3) downloadBarcodeReport();
                else if (ty == 4) downloadBarcodeReportExcel();
                else if (ty == 5) downloadWalletIncomeDetailsMiniAdmin();
                else if (ty == 6) downloadWalletIncomeForServicesMiniAdmin();
                else if (ty == 7) downloadReportWithTrackingCodeMiniAdmin();

                else alert("لطفا نوع گزارش را انتخاب کنید");
            });
            currentSearchObj = {};
            function downloadOrderBillDetailExcel() {
                var valids = postexForm("list-arch");
                if (!valids.isok) {
                    alert(valids.AllValidationMessage);
                    return;
                }
                currentSearchObj = {
                    orderSerialFrom: $('#OrderIdFrom').val(),
                    orderSerialTo: $('#OrderIdTo').val(),
                    payStatus: $('#paymentStatus1').val(),
                    recieverName: $('#reciverName').val(),
                    orderStatus: $('#OrderStatus').val(),
                    recieverProvinceId: valids.Values.reciverProvinceId,
                    recieverCityId: valids.Values.reciverStateId,
                    senderProvinceId: valids.Values.senderProvinceId,
                    senderCityId: valids.Values.senderStateId,
                    fromDate: valids.Values.orderDateFrom,
                    toDate: valids.Values.orderDateTo,
                    serviceTypes: $('#serviceType2').val()
                };

                var fromDate = $('#_DateFrom').MdPersianDateTimePicker('getDate');
                var toDate = $('#_DateTo').MdPersianDateTimePicker('getDate');

                const diffTime = Math.abs(toDate - fromDate);
                const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
                if (diffDays > 8) {
                    alert('لطفا برای دریافت خروجی بازه تاریخی حداکثر یک هفته ای مشخص کنید')
                    return;
                }

                location.href = options.OrderBillDetailExcelUrl + "?searchConditions=" + JSON.stringify(currentSearchObj)
            }
            function downloadOrdersPagedExcel() {
                var valids = postexForm("list-arch");
                if (!valids.isok) {
                    alert(valids.AllValidationMessage);
                    return;
                }
                currentSearchObj = {
                    orderSerialFrom: $('#OrderIdFrom').val(),
                    orderSerialTo: $('#OrderIdTo').val(),
                    payStatus: $('#paymentStatus1').val(),
                    recieverName: $('#reciverName').val(),
                    orderStatus: $('#OrderStatus').val(),
                    recieverProvinceId: valids.Values.reciverProvinceId,
                    recieverCityId: valids.Values.reciverStateId,
                    senderProvinceId: valids.Values.senderProvinceId,
                    senderCityId: valids.Values.senderStateId,
                    fromDate: valids.Values.orderDateFrom,
                    toDate: valids.Values.orderDateTo,
                    serviceTypes: $('#serviceType2').val()
                };

                var fromDate = $('#_DateFrom').MdPersianDateTimePicker('getDate');
                var toDate = $('#_DateTo').MdPersianDateTimePicker('getDate');

                const diffTime = Math.abs(toDate - fromDate);
                const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
                if (diffDays > 8) {
                    alert('لطفا برای دریافت خروجی بازه تاریخی حداکثر یک هفته ای مشخص کنید')
                    return;
                }

                location.href = options.OrdersPagedExcelUrl + "?searchConditions=" + JSON.stringify(currentSearchObj)
            }
            $('#btnDownloadWalletRecord').click(function () {
                location.href = options.WalletRecordDownlaod;
            });
            $(`#btnsearchOrder`).click(function () {

                var valids = postexForm("list-arch");
                if (!valids.isok) {
                    alert(valids.AllValidationMessage);
                    return;
                }
                currentSearchObj = {
                    orderSerialFrom: $('#OrderIdFrom').val(),
                    orderSerialTo: $('#OrderIdTo').val(),
                    payStatus: $('#paymentStatus1').val(),
                    recieverName: $('#reciverName').val(),
                    orderStatus: $('#OrderStatus').val(),
                    recieverProvinceId: valids.Values.reciverProvinceId,
                    recieverCityId: valids.Values.reciverStateId,
                    senderProvinceId: valids.Values.senderProvinceId,
                    senderCityId: valids.Values.senderStateId,
                    fromDate: valids.Values.orderDateFrom,
                    toDate: valids.Values.orderDateTo,
                    serviceTypes: $('#serviceType2').val()
                };

                $(`#divOrders`).html("در حال بارگذاری...");
                $.ajax({
                    cache: true,
                    type: "POST",
                    url: options.GridCustommerOrdersUrl,
                    data: {},
                    success: function (response) {
                        $(`#divOrders`).html(response);
                        pageIndex = 0;
                        loadOrderData(pageIndex, pageSize, '#div_table', JSON.stringify(currentSearchObj));
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        $(`#divOrders`).html(xhr.error);

                    }
                });
            });

            var win = $(window);
            win.scroll(function () { onScroll(); });

            var lazyScrollBusy = false;
            document.addEventListener('touchend', function (e) {
                //e.preventDefault();
                //if ((window.innerHeight + window.scrollY) >= document.body.offsetHeight) {
                //    alert('end of page');
                //}
                onScroll(true);

            }, false);

            function onScroll(isMobile) {

                var addition = 0;
                if (isMobile)
                    addition = 200;
                var $btn = $(`button[href="#list-arch"]`);
                if (!($btn.hasClass('active') && $btn.hasClass('show')))
                    return;
                if (parseInt(window.innerHeight + window.scrollY + 1) >= parseInt(document.body.offsetHeight)) {
                    console.log('try load data');
                    pageIndex++;
                    loadOrderData(pageIndex, pageSize, '#div_table', JSON.stringify(currentSearchObj));
                }
                //if (parseInt($(document).height() - win.height() - 1) == parseInt(win.scrollTop() + addition) || parseInt($(document).height() - win.height()) == parseInt(win.scrollTop() + addition)) {

                //}
            };
            //$(`#divOrders`).scroll(function () {
            //    if (lazyScrollBusy) return;
            //    if (parseInt($(`#divOrders`).height()) < parseInt($(`#divOrders`).scrollTop())) {
            //        var valids = postexForm("list-arch");
            //        if (!valids.isok) {
            //            alert(valids.AllValidationMessage);
            //            return;
            //        }
            //        currentSearchObj = {
            //            orderSerialFrom: $('#OrderIdFrom').val(),
            //            orderSerialTo: $('#OrderIdTo').val(),
            //            payStatus: $('#paymentStatus1').val(),
            //            recieverName: $('#reciverName').val(),
            //            orderStatus: $('#OrderStatus').val(),
            //            recieverProvinceId: valids.Values.reciverProvinceId,
            //            recieverCityId: valids.Values.reciverStateId,
            //            senderProvinceId: valids.Values.senderProvinceId,
            //            senderCityId: valids.Values.senderStateId,
            //            fromDate: valids.Values.orderDateFrom,
            //            toDate: valids.Values.orderDateTo,
            //            serviceTypes: $('#serviceType2').val()
            //        };
            //        lazyScrollBusy = true;
            //        pageIndex++;
            //        loadOrderData(pageIndex, pageSize, '#div_table', JSON.stringify(currentSearchObj));
            //    }
            //});

            $('#btnSavePersonalInfo').click(function () {

                var valids = postexForm("divPersonelInfo");
                if (!valids.isok) {
                    alert(valids.AllValidationMessage);
                    return;
                }
                showLoadding(this);

                $.ajax({
                    cache: true,
                    type: "POST",
                    url: options.SavePersonalInfoUrl,
                    data: valids.Values,
                    success: (response) => {
                        alert(response.message);
                        hideLoadding(this);
                    },
                    error: (xhr, ajaxOptions, thrownError) => {

                        hideLoadding(this);
                    }
                });
            });

            $('#btnChargePrice').click(function () {

                if (!$('input[name=paymentmethod]:checked').val()) {
                    alert('روش پرداخت مورد نظر خود را انتخاب نمایید');
                    return;
                }
                var amount = ($('#priceToCharge').val()).replace(/,/g, '');
                if (!amount || !parseInt(amount) || parseInt(amount) < 50000) {
                    if ($('#priceChargeIndex').val() > 0) {
                        amount = parseInt($('#priceChargeIndex option:selected').text().replace(/,/g, ''));
                    }
                    else {
                        alert("مبلغ وارد شده نامعتبر می باشد");
                        return;
                    }
                }

                $('#isFromApp').val(isAndroid());
                $('#amount').val(amount);
                $(".container ,nav").css({ "filter": "blur(5px)" });
                $("body").css({ "overflowY": "hidden", });

                setTimeout(function () {
                    $(".container ,nav").css({ "opacity": "1", "filter": "blur(0px)" });
                    $("body").css({ "overflowY": "auto" });
                }, 8000);
                $('#PayForCharge').submit();


            });

            $('#btnTransferPrice').click(function () {
                var valids = postexForm("list-trans");
                if (!valids.isok) {
                    alert(valids.AllValidationMessage);
                    return;
                }
                var amount = ($('#priceToTransfer').val()).replace(/,/g, '');
                if (!amount || !parseInt(amount) || parseInt(amount) < 50000) {
                    if ($('#priceIndex').val() > 0) {
                        amount = parseInt($('#priceIndex option:selected').text().replace(/,/g, ''));
                    }
                    else {
                        alert("مبلغ وارد شده نامعتبر می باشد");
                        return;
                    }
                }
                var data = { "priceToTransfer": amount, "mobile": $('#mobile').val() };

                showLoadding(this);
                $.ajax({
                    cache: true,
                    type: "POST",
                    url: options.TransferPriceUrl,
                    data: data,
                    success: function (response) {
                        alert(response.message);
                        hideLoadding(this);
                    },
                    error: function (xhr, ajaxOptions, thrownError) {

                        hideLoadding(this);
                    }
                });
            });

            $('#btnSaveSettings').click(function () {
                var valids = postexForm("list-settings");
                if (!valids.isok) {
                    alert(valids.AllValidationMessage);
                    return;
                }

                showLoadding(this);
                $.ajax({
                    cache: true,
                    type: "POST",
                    url: options.SaveDashboardSettingsUrl,
                    data: { model: valids.Values },
                    success: (response) => {
                        alert(response.message);
                        hideLoadding(this);
                    },
                    error: (xhr, ajaxOptions, thrownError) => {

                        hideLoadding(this);
                    }
                });
            });
        });


        $('.dropdown').click(function () {
            $(this).attr('tabindex', 1).focus();
            $(this).toggleClass('active');
            $(this).find('.dropdown-menu').slideToggle(300);
        });
        $('.dropdown').focusout(function () {
            $(this).removeClass('active');
            $(this).find('.dropdown-menu').slideUp(300);
        });
        $('.dropdown .dropdown-menu li').click(function () {
            $(this).parents('.dropdown').find('span').text($(this).text());
            $(this).parents('.dropdown').find('input').attr('value', $(this).attr('id'));
        });

        $('.dropdown-menu li').click(function () {
            var input = '<strong>' + $(this).parents('.dropdown').find('input').val() + '</strong>',
                msg = '<span class="msg">Hidden input value: ';
            $('.msg').html(msg + input + '</span>');
        });
        function onDataBound(e) {

        }
        function additionalDataAddress() {
            var data = {
                address: $(`#txtSearchAddress`).val()
            };
            firstRead = true;
            addAntiForgeryToken(data);
            return data;
        }
        function additionalDataFactors() {

            var data = {
                datefrom: persianDigitToEnglish($('#frDateFrom').val()),
                dateto: persianDigitToEnglish($('#frDateTo').val())
            };
            firstRead = true;
            addAntiForgeryToken(data);
            return data;
        }

        function additionalDataMessages() {
            var data = {
                message: $(`#txtSearchMessages`).val()
            };
            firstRead = true;
            addAntiForgeryToken(data);
            return data;
        }

        function downloadBarcodeReport() {
            var valids = postexForm("list-arch");
            if (!valids.isok) {
                alert(valids.AllValidationMessage);
                return;
            }
            currentSearchObj = {
                orderSerialFrom: $('#OrderIdFrom').val(),
                orderSerialTo: $('#OrderIdTo').val(),
                payStatus: $('#paymentStatus1').val(),
                recieverName: $('#reciverName').val(),
                orderStatus: $('#OrderStatus').val(),
                recieverProvinceId: valids.Values.reciverProvinceId,
                recieverCityId: valids.Values.reciverStateId,
                senderProvinceId: valids.Values.senderProvinceId,
                senderCityId: valids.Values.senderStateId,
                fromDate: valids.Values.orderDateFrom,
                toDate: valids.Values.orderDateTo,
                serviceTypes: $('#serviceType2').val()
            };

            var fromDate = $('#_DateFrom').MdPersianDateTimePicker('getDate');
            var toDate = $('#_DateTo').MdPersianDateTimePicker('getDate');

            const diffTime = Math.abs(toDate - fromDate);
            const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
            if (diffDays > 8) {
                alert('لطفا برای دریافت خروجی بازه تاریخی حداکثر یک هفته ای مشخص کنید')
                return;
            }

            location.href = options.OrdersBarcodeReportUrl + "?searchConditions=" + JSON.stringify(currentSearchObj)
        }

        function downloadWalletIncomeDetailsMiniAdmin() {
            var valids = postexForm("list-arch");
            if (!valids.isok) {
                alert(valids.AllValidationMessage);
                return;
            }
            currentSearchObj = {
                fromDate: valids.Values.orderDateFrom,
                toDate: valids.Values.orderDateTo,
                serviceTypes: $('#serviceType2').val()
            };

            var fromDate = $('#_DateFrom').MdPersianDateTimePicker('getDate');
            var toDate = $('#_DateTo').MdPersianDateTimePicker('getDate');

            const diffTime = Math.abs(toDate - fromDate);
            const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
            if (diffDays > 8) {
                alert('لطفا برای دریافت خروجی بازه تاریخی حداکثر یک هفته ای مشخص کنید')
                return;
            }

            location.href = options.WalletIncomeForServicesMiniAdminUrl + "?searchConditions=" + JSON.stringify(currentSearchObj)
        }

        function downloadWalletIncomeForServicesMiniAdmin() {
            var valids = postexForm("list-arch");
            if (!valids.isok) {
                alert(valids.AllValidationMessage);
                return;
            }
            currentSearchObj = {
                fromDate: valids.Values.orderDateFrom,
                toDate: valids.Values.orderDateTo,
                serviceTypes: $('#serviceType2').val()
            };

            var fromDate = $('#_DateFrom').MdPersianDateTimePicker('getDate');
            var toDate = $('#_DateTo').MdPersianDateTimePicker('getDate');

            const diffTime = Math.abs(toDate - fromDate);
            const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
            if (diffDays > 8) {
                alert('لطفا برای دریافت خروجی بازه تاریخی حداکثر یک هفته ای مشخص کنید')
                return;
            }

            location.href = options.WalletIncomeForServicesMiniAdminUrl + "?searchConditions=" + JSON.stringify(currentSearchObj)
        }

        function downloadWalletIncomeDetailsMiniAdmin() {
            var valids = postexForm("list-arch");
            if (!valids.isok) {
                alert(valids.AllValidationMessage);
                return;
            }
            currentSearchObj = {
                fromDate: valids.Values.orderDateFrom,
                toDate: valids.Values.orderDateTo,
                serviceTypes: $('#serviceType2').val()
            };

            var fromDate = $('#_DateFrom').MdPersianDateTimePicker('getDate');
            var toDate = $('#_DateTo').MdPersianDateTimePicker('getDate');

            const diffTime = Math.abs(toDate - fromDate);
            const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
            if (diffDays > 8) {
                alert('لطفا برای دریافت خروجی بازه تاریخی حداکثر یک هفته ای مشخص کنید')
                return;
            }

            location.href = options.WalletIncomeForServicesMiniAdminUrl + "?searchConditions=" + JSON.stringify(currentSearchObj)
        }

        function downloadBarcodeReportExcel() {
            var valids = postexForm("list-arch");
            if (!valids.isok) {
                alert(valids.AllValidationMessage);
                return;
            }
            currentSearchObj = {
                orderSerialFrom: $('#OrderIdFrom').val(),
                orderSerialTo: $('#OrderIdTo').val(),
                payStatus: $('#paymentStatus1').val(),
                recieverName: $('#reciverName').val(),
                orderStatus: $('#OrderStatus').val(),
                recieverProvinceId: valids.Values.reciverProvinceId,
                recieverCityId: valids.Values.reciverStateId,
                senderProvinceId: valids.Values.senderProvinceId,
                senderCityId: valids.Values.senderStateId,
                fromDate: valids.Values.orderDateFrom,
                toDate: valids.Values.orderDateTo,
                serviceTypes: $('#serviceType2').val()
            };

            var fromDate = $('#_DateFrom').MdPersianDateTimePicker('getDate');
            var toDate = $('#_DateTo').MdPersianDateTimePicker('getDate');

            const diffTime = Math.abs(toDate - fromDate);
            const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
            if (diffDays > 8) {
                alert('لطفا برای دریافت خروجی بازه تاریخی حداکثر یک هفته ای مشخص کنید')
                return;
            }

            location.href = options.OrdersBarcodeReportExcelUrl + "?searchConditions=" + JSON.stringify(currentSearchObj)
        }

    };
    this.construct(options);
}



function funDownloadFactor(btn, url, SafeFileName) {
    debugger;
    if (SafeFileName == "1") {
        Id = $(btn).attr('data-value');
        location.href = url + "?requestFactorId=" + Id
    }
    else alert("فاکتور تایید نشده است");

}
function funReadMessage(btn, url) {
    msgId = $(btn).attr('data-value');
    $(`#rtxtMessageBody`).html("در حال بارگذاری");
    $(`#msgTime`).html("");
    $(`#msgSubject`).html("");

    $('#divMessageModal').modal('show');
    $.ajax({
        cache: true,
        type: "POST",
        url: url,
        data: { messageId: msgId, state: 1 },
        success: function (response) {
            //$(`#btnSearchMessages`).click();
            $(`#rtxtMessageBody`).html(response.MessageText);
            $(`#msgTime`).html(response.CreatedDate);
            $(`#msgSubject`).html(response.Subject);
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert(xhr.error);
        }
    });
}

function btnAddFactorRequestsClick() {
    $('#divFactorRequestModal').modal('show');
    initForm("divFactorRequestModal");
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