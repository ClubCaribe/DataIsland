((): void => {
    'use strict';

    angular.module("diapp").config(["$httpProvider", ($httpProvider: ng.IHttpProvider) => {
        $httpProvider.interceptors.push('diapp.services.AuthenticationInterceptor');
    }]);

})();