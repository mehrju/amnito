var JavaScriptCallbacks = {
  /**
   * this method is called each time webview starts a BACK navigation action
   * @param {current url} currentUrl
   */
  onWebViewNavigateBack: function (currentUrl) {
    console.log("onWebViewNavigateBack called from JS -- " + currentUrl);
  },
  /**
   * this method is called each time a page is starting to loading. note that this
   * callback might not be called if the javascript file containing this script is not loaded and available
   * @param {the url that is being loaded} url
   */
  onWebViewLoadingPage: function (url) {
    console.log("onWebViewLoadingPage called from JS -- " + url);
  },
  /**
   * this method is called each time a page is finished loading
   * @param {the url that had just finished loading} url
   */
  onWebViewFinishedLoading: function (url) {
    console.log("onWebViewFinishedLoading called from JS -- " + url);
  },
};
