
var DiPanel = angular.module("DiPanel", ["interceptors", "formValidators", "angularUtils.directives.dirPagination", "dataisland.DynamicForms", "ngSanitize"]);

DiPanel.factory('AuthInterceptor', ["$window", "$q", "DiProgress", function ($window, $q, DiProgress) {
    return {
        request: function (config) {
            DiProgress.addRequest();
            config.headers = config.headers || {};
            if (diApiToken) {
                config.headers.Authorization = 'Bearer ' + diApiToken;
            }
            return config || $q.when(config);
        },
        response: function (response) {
            DiProgress.removeRequest();
            if (response.status === 401) {
                // TODO: Redirect user to login page.
            }
            return response || $q.when(response);
        },
        requestError: function (rejection) {
            // do something on error
            DiProgress.removeRequest();
            if (canRecover(rejection)) {
                return responseOrNewPromise
            }
            return $q.reject(rejection);
        },
        responseError: function (rejection) {
            // do something on error
            DiProgress.removeRequest();
            if (canRecover(rejection)) {
                return responseOrNewPromise
            }
            return $q.reject(rejection);
        }
    };
}]).factory("DiProgress", ["$log", function ($log) {
    return {
        numOfRequests: 0,
        addRequest: function () {
            this.numOfRequests++;
            this.showLoadingBar();
        },
        removeRequest: function () {
            this.numOfRequests--;
            if (this.numOfRequests < 0) {
                this.numOfRequests = 0;
            }
            this.showLoadingBar();
        },
        showLoadingBar: function () {
            if (this.numOfRequests > 0) {
                try
                {
                    show_loading_bar(100 / (this.numOfRequests+1));
                }
                catch(e)
                {

                }
            }
            if (this.numOfRequests == 0) {
                try{
                    show_loading_bar(100);
                }
                catch(e)
                {

                }
            }
        }
    }
}]);

// Register the previously created AuthInterceptor.
DiPanel.config(["$httpProvider", function ($httpProvider) {
    $httpProvider.interceptors.push('AuthInterceptor');
}]);