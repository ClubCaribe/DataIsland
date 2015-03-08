/// <reference path="../../../Scripts/angular.js" />

var DiPanel = angular.module("DiPanel", []);

DiPanel.factory('AuthInterceptor', function ($window, $q, DiProgress) {
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
		}
	};
}).factory("DiProgress", ["$log", function ($log) {
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
                try{
                    show_loading_bar(100 / (this.numOfRequests+1));
                } catch (e) {

                }
            }
            if (this.numOfRequests == 0) {
                try{
                    show_loading_bar(100);
                } catch (e) {

                }
            }
        }
    }
}]);

// Register the previously created AuthInterceptor.
DiPanel.config(function ($httpProvider) {
	$httpProvider.interceptors.push('AuthInterceptor');
});

