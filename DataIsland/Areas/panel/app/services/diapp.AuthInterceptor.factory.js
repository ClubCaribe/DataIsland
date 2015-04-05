var diapp;
(function (diapp) {
    var services;
    (function (services) {
        'use strict';
        var AuthenticationInterceptor = (function () {
            function AuthenticationInterceptor($q, $window, progress) {
                var _this = this;
                this.$q = $q;
                this.$window = $window;
                this.progress = progress;
                this.request = function (config) {
                    _this.progress.addRequest();
                    config.headers = config.headers || {};
                    if (diApiToken) {
                        config.headers.Authorization = 'Bearer ' + diApiToken;
                    }
                    return config || _this.$q.when(config);
                };
                this.response = function (response) {
                    _this.progress.removeRequest();
                    if (response.status === 401) {
                    }
                    return response || _this.$q.when(response);
                };
                this.requestError = function (rejection) {
                    // do something on error
                    _this.progress.removeRequest();
                    return _this.$q.reject(rejection);
                };
                this.responseError = function (rejection) {
                    _this.progress.removeRequest();
                    console.log(rejection.status);
                    if (rejection.status === 401) {
                    }
                    // Otherwise, default behavior
                    return _this.$q.reject(rejection);
                };
            }
            return AuthenticationInterceptor;
        })();
        services.AuthenticationInterceptor = AuthenticationInterceptor;
        factory.$inject = ['$q', '$window', 'diapp.services.DiProgress'];
        function factory($q, $window, progress) {
            return new AuthenticationInterceptor($q, $window, progress);
        }
        angular.module('diapp').factory('diapp.services.AuthenticationInterceptor', factory);
    })(services = diapp.services || (diapp.services = {}));
})(diapp || (diapp = {}));
//# sourceMappingURL=diapp.AuthInterceptor.factory.js.map