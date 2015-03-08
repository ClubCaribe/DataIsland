

angular.module("interceptors", []).factory("DiProgress", ["$timeout", "$log", function ($timeout, $log) {
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
            $timeout(console.log('0'));
            if (this.numOfRequests > 0) {
                $timeout(show_loading_bar(100 - (100 / this.numOfRequests)));
            }
            if (this.numOfRequests == 0) {
                $timeout(show_loading_bar(100));
            }
        }
    }
}]).factory('AuthInterceptor', ["$window", "$q", "DiProgress", function ($window, $q, DiProgress) {
    return {
        'request': function (config) {
            DiProgress.addRequest();
            config.headers = config.headers || {};
            if (diApiToken) {
                config.headers.Authorization = 'Bearer ' + diApiToken;
            }
            return config || $q.when(config);
        },
        'response': function (response) {
            DiProgress.removeRequest();
            if (response.status === 401) {
                location.href = "/login"
            }
            return response || $q.when(response);
        },
        'requestError': function (requestError) {
            DiProgress.removeRequest();
            return $q.reject(requestError);
        },
        'responseError': function (responseError) {
            DiProgress.removeRequest();
            return $q.reject(responseError);
        }
    };
    }]);

