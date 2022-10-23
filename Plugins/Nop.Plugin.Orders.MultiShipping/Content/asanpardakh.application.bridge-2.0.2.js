var asanPardakhtSettings = {
	platform: ''
};

var asanPardakht = {
    eventSubscription: {
        trigger: function (eventName, data) {
            var cEvent = new CustomEvent(eventName, { detail: data });
            window.dispatchEvent(cEvent);
        },
        on: function (eventName, callback) {
            var callMe = function (e) {
                callback(e.detail);
            }
            this._events[eventName] = callMe;
            window.addEventListener(eventName, callMe);
        },
        off: function (eventName) {
            var registeredCallback = this._events[eventName];
            if (!registeredCallback) {
                return;
            }

            window.removeEventListener(eventName, registeredCallback);
            this._events[eventName] = undefined;
        },
        _events: {}
    },
    application: {
        private: {
            doAction: function (action, value) {
                console.log(this.getPlatform() + ' > ' + action + ': ' + JSON.stringify(value));
                try {
                    switch (this.getPlatform()) {
                        case 'android':
                            if (Android) {
                                Android.DoAction(action, JSON.stringify(value));
                            }
                            break;
                        case 'ios':
                            this.setupWebViewJavascriptBridge(function (bridge) {
                                bridge.callHandler('doAction',
                                    { action: action, value: value },
                                    function (response) {
                                        console.log('doAction');
                                    });
                            });
                            break;
                        case 'pwa':
	                        var params = action.split('.');
	                        if (params.length === 2) {
		                        window.parent[params[0]][params[1]](value);
	                        } else {
		                        window.parent[params[0]](value);
	                        }
	                        break;
                    }
                } catch (e) {
                    console.log(e);
                }
            },
            callMeOn: function (eventName, eventHandler) {
                asanPardakht.eventSubscription.on(eventName, eventHandler);
            },
            setupWebViewJavascriptBridge: function (callback) {
                if (window.WebViewJavascriptBridge) {
                    return callback(WebViewJavascriptBridge);
                }
                if (window.WVJBCallbacks) {
                    return window.WVJBCallbacks.push(callback);
                }
                window.WVJBCallbacks = [callback];
                var WVJBIframe = document.createElement('iframe');
                WVJBIframe.style.display = 'none';
                WVJBIframe.style.width = "0";
                WVJBIframe.style.border = "3px solid #0f0";
                WVJBIframe.src = 'wvjbscheme://__BRIDGE_LOADED__';
                document.documentElement.appendChild(WVJBIframe);
                setTimeout(function () { document.documentElement.removeChild(WVJBIframe) }, 0);
            },
            getPlatform: function () {
                return asanPardakhtSettings.platform;
            }
        },

        deviceMessageBoxTypes:
        {
            General: 0,
            Info: 1,
            Success: 2,
            Error: 3,
            FetalError: 4
        },

        beInformedWebAppIsReady: function (config) {
	        asanPardakhtSettings.platform = config;
            this.private.doAction('webApp.Ready');
        },

        setPageTitle: function (title) {
            this.private.doAction('pageTitle.Set', title);
        },

        goBack: function () {
            this.private.doAction('goBack', null);
        },

        showConfirmBox: function (title, message, okTitle, cancelTitle, onOk, onCancel) {
            this.private.doAction('confirmBox.Show',
                {
                    title: title,
                    message: message,
                    okButtonTitle: okTitle,
                    cancelButtonTitle: cancelTitle
                });

            asanPardakht.eventSubscription.off('confirmBox.Closed');
            this.private.callMeOn('confirmBox.Closed',
                function (e) {
                    if (e === 1) {
                        if (onOk)
                            onOk();
                    } else {
                        if (onCancel)
                            onCancel();
                    }
                });
        },

        showMessageBox: function (title, message, type) {
            var messages = message;

            if (!(message instanceof Array))
                messages = [message];

            this.private.doAction('messageBox.Show',
                {
                    title: title,
                    message: messages,
                    type: type
                });
        },

        showError: function (errorMessage) {
            this.showMessageBox('خطا', errorMessage, this.deviceMessageBoxTypes.Error);
        },

        showFetalError: function (errorMessage) {
            this.showMessageBox('خطا', errorMessage, this.deviceMessageBoxTypes.FetalError);
        },

        showLoading: function () {
            this.private.doAction('loading.Show', null);
        },

        hideLoading: function () {
            this.private.doAction('loading.Hide', null);
        },

        showPlusButton: function (pressEventHandler) {
            this.private.doAction('plusButton.Show', null);
            asanPardakht.eventSubscription.off('plusButton.Pressed');
            this.private.callMeOn('plusButton.Pressed', pressEventHandler);
        },

        hidePlusButton: function () {
	        asanPardakht.eventSubscription.off('plusButton.Pressed');
	        this.private.doAction('plusButton.Hide', null);
        },

        showRefreshButton: function (pressEventHandler) {
            this.private.doAction('refreshButton.Show', null);
            asanPardakht.eventSubscription.off('refreshButton.Pressed');
            this.private.callMeOn('refreshButton.Pressed', pressEventHandler);
        },

        hideRefreshButton: function () {
            asanPardakht.eventSubscription.off('refreshButton.Pressed');
            this.private.doAction('refreshButton.Hide', null);
        },

        callMeOnBackButtonPress: function (pressEventHandler) {
            asanPardakht.eventSubscription.off('backButton.Pressed');
            this.private.callMeOn('backButton.Pressed', pressEventHandler);
        },

        openExternalUrl: function (url, androidPreferredApp, iosPreferredApp) {
            var preferredApp = androidPreferredApp;
            if (this.private.getPlatform() == 'ios') {
                preferredApp = iosPreferredApp;
            }

            this.private.doAction('system.OpenUrl', { url: url, preferredApp: preferredApp });
        },

        startPayment: function (paymentRequest, paymentCompletedEventHandler) {
            var paymentId = paymentRequest.PaymentId;

            this.private.callMeOn('payment.Completed',
                function (e) {
                    asanPardakht.eventSubscription.off('payment.Completed');

                    var result = e;

                    if (result.indexOf('"host_response":""{') > -1) {
                        result = result.replace('"host_response":""{', '"host_response":"{');
                        result = result.replace('"}"","host_response_sign', '"}","host_response_sign');
                    } else {
                        var starting = '"host_response":"{';
                        var ending = '"}"';

                        if (result.indexOf(starting) > -1 && result.indexOf(ending) > -1) {
                            var innerJson = result.substring((result.indexOf(starting) + starting.length), result.indexOf(ending) + ending.length - 1);

                            var lastValue = '';
                            var prevCharacter = '';
                            for (var i = 0; i < innerJson.length; i++) {
	                            var character = innerJson[i];
	                            if (character == '"' && prevCharacter != '\\') {
		                            character = '\\' + character;
	                            }

	                            lastValue += character;
	                            prevCharacter = character;
                            }

                            result = result.substring(0, result.indexOf(starting)) +
                                starting +
                                lastValue +
                                result.substring(result.indexOf(ending) + ending.length - 1);
                        }
                    }

                    var trandIdRegex = /("|')unique_tran_id("|')\s*:\s*(\d+)/;
                    var tranIdRegexResult = trandIdRegex.exec(result); 
                    var trandIdField = '"unique_tran_id":"' + tranIdRegexResult[3] + '"';

                    result = result.replace(tranIdRegexResult[0], trandIdField);

                    var model = JSON.parse(result);
                    model.paymentId = paymentId;

                    paymentCompletedEventHandler(model);
                });

            this.private.doAction('payment.Start', paymentRequest);
        },

        fileManager: {
            types: { image: 'image', video: 'video', audio: 'audio', other: 'other' },
            isBusy: false,
            upload: function (destinationUrl, httpHeader, httpBody, title, desc, type, allowedExtensions, fileSize, duration, maxWidth, maxHeight, minWidth, minHeight, successCallback, errorCallback) {

                if (asanPardakht.application.private.getPlatform() == 'ios') {
                    asanPardakht.application.showMessageBox('خطا', 'این متد تنها مربوط به پلتفرم اندروید میباشد', asanPardakht.application.deviceMessageBoxTypes.Error);
                    return;
                };

                if (asanPardakht.application.fileManager.isBusy) {
                    return;
                }
                asanPardakht.application.fileManager.isBusy = true;

                var bodyValues = [];
                if (httpBody) {
                    for (var name in httpBody) {
                        bodyValues.push({ key: name, value: httpBody[name] });
                    }
                }

                var headerValues = [];
                if (httpHeader) {
                    for (var name in httpHeader) {
                        headerValues.push({ key: name, value: httpHeader[name] });
                    }
                }

                var model = {
                    Title: title,
                    Description: desc,
                    DestinationUrl: destinationUrl,
                    HeaderValues: headerValues,
                    BodyValues: bodyValues,
                    Constraints:
                    {
                        AllowedExtensions: allowedExtensions,
                        MaxFileSize: fileSize,
                        FileType: type,
                        MaxDuration: duration,
                        MaxWidth: maxWidth,
                        MaxHeight: maxHeight,
                        MinWidth: minWidth,
                        MinHeight: minHeight
                    }
                };

                asanPardakht.application.private.callMeOn('file.UploadCompleted', function (e) {
                    asanPardakht.eventSubscription.off('file.UploadCompleted');
                    asanPardakht.application.fileManager.uploadCompleteHandler(e, successCallback, errorCallback);
                });
                asanPardakht.application.private.doAction('file.Upload', model);
            },
            uploadCompleteHandler: function (e, successCallback, errorCallback) {
                asanPardakht.application.fileManager.isBusy = false;
                var result = JSON.parse(e);
                if (!isNaN(result.errorCode)) {
                    result.errorCode = Number(result.errorCode);
                } else {
                    result.errorCode = 0;
                }

                if (result.errorCode === 0) {
                    asanPardakht.application.showMessageBox('موفق', 'آپلود با موفقیت انجام شد.', asanPardakht.application.deviceMessageBoxTypes.Success);
                    if (successCallback) successCallback(result);
                } else {
                    if (result.errorCode != -5) {
                        asanPardakht.application.showMessageBox('خطا', asanPardakht.application.fileManager.getErrorMessage(result.errorCode, result.errorMessage), asanPardakht.application.deviceMessageBoxTypes.Error);
                    }
                    if (errorCallback) errorCallback(result);
                }
            },
            uploadImage: function (url, httpHeader, httpBody, title, desc, fileSize, maxWidth, maxHeight, minWidth, minHeight, successCallback, errorCallback) {
                asanPardakht.application.fileManager.upload(url, httpHeader, httpBody, title, desc, asanPardakht.application.fileManager.types.image, '.jpg,.jpeg,.png,.gif', fileSize, null, maxWidth, maxHeight, minWidth, minHeight, successCallback, errorCallback);
            },
            uploadVideo: function (url, httpHeader, httpBody, title, desc, fileSize, duration, maxWidth, maxHeight, minWidth, minHeight, successCallback, errorCallback) {
                asanPardakht.application.fileManager.upload(url, httpHeader, httpBody, title, desc, asanPardakht.application.fileManager.types.video, '.mp4,.avi,.3gp,.flv,.mkv,.wmv,.mov', fileSize, duration, maxWidth, maxHeight, minWidth, minHeight, successCallback, errorCallback);
            },
            uploadAudio: function (url, httpHeader, httpBody, title, desc, fileSize, duration, successCallback, errorCallback) {
                asanPardakht.application.fileManager.upload(url, httpHeader, httpBody, title, desc, asanPardakht.application.fileManager.types.audio, '.mp3,.wma,.aac,.wav,.wave,.mid,.ogg', fileSize, duration, 0, 0, 0, 0, successCallback, errorCallback);
            },
            simulateUploadCompleted: function (serverResponse, errorCode, errorMessage) {
                webApp.trigger('file.UploadCompleted', JSON.stringify({
                    serverResponse: serverResponse,
                    errorCode: errorCode,
                    errorMessage: errorMessage
                }));
            },
            getErrorMessage: function (code, msg) {
                if (msg && msg != '' && msg != ' ') return msg;
                var codeNumber = Number(code);
                switch (codeNumber) {
                    case -1: return 'لطفاً ارتباط خود را چک کنید.';
                    case -2: return 'خطای داخلی سیستم.';
                    case -3: return 'لطفاً ارتباط خود را چک کنید.';
                    case -4: return 'خطای نامشخص.';
                    case -5: return 'انصراف توسط کاربر.';
                    //case -6: return '';
                    //case -7: return '';
                    default:
                        return 'خطای نامشخص';
                }
            }
        }
    }
};

var webApp = {
    trigger: function (eventName, data) {
        console.log(eventName + ' > ' + JSON.stringify(data));
        asanPardakht.eventSubscription.trigger(eventName, data);
    }
};