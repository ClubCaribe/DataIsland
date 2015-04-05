var diapp;
(function (diapp) {
    var filemanager;
    (function (filemanager) {
        var services;
        (function (services) {
            var ForeignFileResourcesService = (function () {
                function ForeignFileResourcesService($http) {
                    this.$http = $http;
                    this.urlPrefix = "/";
                }
                ForeignFileResourcesService.prototype.ListDirectory = function (userID, resourceID, virtualPath) {
                    var url = this.urlPrefix + "api/filemanager/foreignresources/listdirectory/" + userID.EscapeUserId() + "/" + resourceID.EscapeUserId() + "/" + virtualPath;
                    return this.$http.get(url).then(function (response) {
                        return response.data;
                    });
                };
                ForeignFileResourcesService.prototype.GetPermissions = function (userID, resourceID) {
                    var url = this.urlPrefix + "api/filemanager/foreignresources/getpermissions/" + userID.EscapeUserId() + "/" + resourceID.EscapeUserId();
                    return this.$http.get(url).then(function (response) {
                        return response.data;
                    });
                };
                ForeignFileResourcesService.prototype.GetUserDataIsland = function (userID) {
                    var url = "/api/panel/user/getuserdataislanddata";
                    var data = {
                        userId: userID
                    };
                    return this.$http.post(url, data).then(function (response) {
                        return response.data;
                    });
                };
                return ForeignFileResourcesService;
            })();
            factory.$inject = [
                '$http'
            ];
            function factory($http) {
                return new ForeignFileResourcesService($http);
            }
            angular.module('diapp').factory('diapp.filemanager.services.ForeignFileResourcesService', factory);
        })(services = filemanager.services || (filemanager.services = {}));
    })(filemanager = diapp.filemanager || (diapp.filemanager = {}));
})(diapp || (diapp = {}));
//# sourceMappingURL=diapp.filemanager.services.foreignresources.js.map