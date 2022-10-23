/**
 * shows a native android toast to the user
 *
 * @param {string} toast - not optional must be provided
 */
function showAndroidToast(toast) {
    Android.showToast(toast);
}

/**
 * shows a native android snackbar as a warning message to the user
 *
 * @param {string} message - not optional must be provided
 */
function showAndroidWarningSnackBar(message) {
    Android.showWarningSnackBar(message);
}

/**
 * shows an android native dialog to the user that prompts user with the given message
 *
 * @param {string} message - not optional must be provided
 */
function showAndroidDialog(message) {
    Android.showDialog(message);
}

/**
 * shows an android native dialog that shows a loading indicator and the given message
 *
 * @param {string} message - not optional must be provided
 */
function showAndroidDialogLoading(message) {
    Android.showDialogLoading(message);
}

/**
 * shows an android native dialog  with the the 2 buttons which have the values of the given options
 * when user clicks a button the given callback will be called with the clicked button value as argument
 *
 * @param {string} message - not optional must be provided
 * @param {string} option1 - not optional must be provided
 * @param {string} option2 - not optional must be provided
 * @param {string} callback - optional - enter name of the callback function as string like -> 'myCallbackFunction' - callback function must
 *  be available in public global scope
 */
function showAndroidDialogWithOptions(message, option1, option2, callback) {
    Android.showDialogWithOptions(message, option1, option2, callback);
}

/**
 * checks if the client is an android app
 *
 * basically android app is injecting a javascript object called Android to the loaded page to
 * expose android native functionalities to the web page . this method only checks if that object is available
 */
function isAndroid() {
    return typeof Android != "undefined";
}

/**
 * shows an android native page (fullscreen) that shows a loading indicator
 *
 * note: parameter message is not optional and must not be null
 */
function showAndroidLoading() {
    Android.showLoading();
}

/**
 * hide android native loading pages
 */
function hideAndroidLoading() {
    Android.hideLoading();
}

/**
 * shows android login page
 */
function showAndroidLoginPage() {
    Android.showLoginPage();
}

/**
 * shows android register page
 */
function showAndroidRegisterPage() {
    Android.showRegisterPage();
}

/**
 * hide android native loading pages or any other native pages like login_page, register_page etc.
 */
function hideAndroidNativePage() {
    Android.hideLoading();
}

/**
 * logs out user natively.
 * after user is logged out. login page will be opened and user needs to login again
 */
function logoutAndroid() {
    Android.logout();
}

/**
 * exits android app
 */
function exitAndroidApp(){
  Android.exitApp();
}

/**
 * launches phone's Dialer with the given number. allowing user to call the number
 *
 * @param {string} number - not optional must be provided
 */
function callNumberAndroid(number) {
    Android.callNumber(number);
}

/**
 * it will check if the android device has connection
 *
 * @returns {boolean} true if user is connected to internet
 */
function hasConnectionAndroid() {
    return Android.hasConnection();
}

/**
 * opens app page on the market for user to rate the app
 */
function rateAndroidApp() {
    Android.rateApp();
}

/**
 * subscribes client to a topic on pushe service
 *
 * @param {string} topic - not optional must be provided
 * @param {string} callback - optional - enter function name as string like 'myCallbackFunction' - function must be visible in page scope so that it can be called from native side
 */
function subscribeToPusheTopicAndroid(topic, callback) {
    Android.subscribeToPusheTopic(topic, callback);
}

/**
 * unsubscribes client from a topic on pushe service
 *
 * @param {string} topic - not optional must be provided
 * @param {string} callback - optional - enter function name as string like 'myCallbackFunction' - function must be visible
 * in page scope so that it can be called from native side
 */
function unSubscribeFromPusheTopicAndroid(topic, callback) {
    Android.unSubscribeFromPusheTopic(topic, callback);
}

/**
 * sends a pushe event to pushe service from this client
 *
 * @param {string} event - not optional must be provided
 */
function sendPusheEventAndroid(event) {
    Android.sendPusheEvent(event);
}

/**
 * sets user's credentials data on pushe service.
 *
 * @param {string} email - optional
 * @param {string} mobile - optional
 * @param {string} id - optional
 */
function setUserDataOnPusheAndroid(email, mobile, id) {
    Android.setUserDataOnPushe(email, mobile, id);
}
function login(_token, userName) {

    var postData = { UName: userName };
    addAntiForgeryToken(postData);
    $.ajax({
        beforeSend: function (xhr) {
            xhr.setRequestHeader('Token', _token);
        },
        cache: false,
        url: '/App/Login',
        data: postData,
        type: "GET",
        success: function (data) {
            if (data.message)
                alert(data.message);
            if (data.success)
                window.location = 'https://postex.ir'
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert('خطای در زمان اعتبار سنجی رخ داد لطفا ارتباط اینترنت خود را بررسی کنید');
        }
    });
}
function addAntiForgeryToken(data) {
    //if the object is undefined, create a new one.
    if (!data) {
        data = {};
    }
    //add token
    var tokenInput = $('input[name=__RequestVerificationToken]');
    if (tokenInput.length) {
        data.__RequestVerificationToken = tokenInput.val();
    }
    return data;
};

/**
  this function is for testing purposes only do not use this function in production
*/
function logNative(str) {
    Android.logNative(str);
}

/**
 * opens a Browser app that is installed on user's Device and launches the given url
 * @param {url} url
 */
function openAndroidBrowser(url) {
    Android.openBrowser(url);
}