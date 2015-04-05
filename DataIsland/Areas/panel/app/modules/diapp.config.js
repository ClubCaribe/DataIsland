(function () {
    'use strict';
    angular.module("diapp").config(["$httpProvider", function ($httpProvider) {
        $httpProvider.interceptors.push('diapp.services.AuthenticationInterceptor');
    }]);
})();
//# sourceMappingURL=diapp.config.js.map