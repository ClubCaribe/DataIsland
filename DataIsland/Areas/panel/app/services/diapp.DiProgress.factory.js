var diapp;
(function (diapp) {
    var services;
    (function (services) {
        'use strict';
        var DiProgress = (function () {
            function DiProgress() {
                this.numOfRequests = 0;
            }
            DiProgress.prototype.addRequest = function () {
                this.numOfRequests++;
                this.showLoadingBar();
            };
            DiProgress.prototype.removeRequest = function () {
                this.numOfRequests--;
                if (this.numOfRequests < 0) {
                    this.numOfRequests = 0;
                }
                this.showLoadingBar();
            };
            DiProgress.prototype.showLoadingBar = function () {
                if (this.numOfRequests > 0) {
                    try {
                        show_loading_bar(100 / (this.numOfRequests + 1));
                    }
                    catch (e) {
                    }
                }
                if (this.numOfRequests == 0) {
                    try {
                        show_loading_bar(100);
                    }
                    catch (e) {
                    }
                }
            };
            return DiProgress;
        })();
        function factory() {
            return new DiProgress();
        }
        angular.module('diapp').factory('diapp.services.DiProgress', factory);
    })(services = diapp.services || (diapp.services = {}));
})(diapp || (diapp = {}));
//# sourceMappingURL=diapp.DiProgress.factory.js.map