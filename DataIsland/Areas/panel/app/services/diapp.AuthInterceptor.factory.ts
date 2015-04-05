
//http://stackoverflow.com/questions/20798626/write-http-interceptor-as-class
declare var diApiToken;

module diapp.services {
    'use strict';

    export class AuthenticationInterceptor {

        public request = (config) => {
            this.progress.addRequest();
            config.headers = config.headers || {};
            if (diApiToken) {
                config.headers.Authorization = 'Bearer ' + diApiToken;
            }
            return config || this.$q.when(config);
        };

        public response = (response) => {
            this.progress.removeRequest();
            if (response.status === 401) {
                // TODO: Redirect user to login page.
            }
            return response || this.$q.when(response);
        }

        public requestError = (rejection) => {
            // do something on error
            this.progress.removeRequest();
            
            return this.$q.reject(rejection);
        }

        public responseError = (rejection) => {
            this.progress.removeRequest();
            console.log(rejection.status);
            if (rejection.status === 401) {
            }
            // Otherwise, default behavior
            return this.$q.reject(rejection); 
        } 

        constructor(public $q : ng.IQService, public $window, public progress:IDiProgress) {
        }
         
    }

    factory.$inject = ['$q', '$window','diapp.services.DiProgress'];
    function factory($q: ng.IQService, $window, progress: IDiProgress): AuthenticationInterceptor {
        return new AuthenticationInterceptor($q,$window,progress);
    }

    angular
        .module('diapp')
        .factory('diapp.services.AuthenticationInterceptor',
        factory);  
} 