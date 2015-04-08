module diapp.services.filemanager {
    'use strict';

    export interface ISharedResourcesService {
        GetUserForeignSharedResources(userID: string): ng.IPromise<diapp.models.filemanager.IForeignSharedResource[]>;
        DeleteForeignResource(resourceId: string): ng.IPromise<boolean>;
    }

    class SharedResourcesService implements ISharedResourcesService {

        constructor(private $http: ng.IHttpService) {
        }

        GetUserForeignSharedResources(userID: string) {
            var url = "/api/filemanager/shared/getforeignresourcesbyuserid";
            var data = {
                userId: userID
            }
            return this.$http.post(url,data)
                .then((response: ng.IHttpPromiseCallbackArg<diapp.models.filemanager.IForeignSharedResource[]>): diapp.models.filemanager.IForeignSharedResource[]=> {
                    return <diapp.models.filemanager.IForeignSharedResource[]>response.data;
            });
        }

        DeleteForeignResource(resourceId: string): ng.IPromise<boolean> {
            var url = "/api/filemanager/shared/deletesharedresource/" + resourceId;
            return this.$http.get(url)
                .then((response:ng.IHttpPromiseCallbackArg<boolean>):boolean => {
                    return <boolean>response;
            });
        }
    }

    factory.$inject = [
        '$http'
    ];
    function factory($http: ng.IHttpService): ISharedResourcesService {
        return new SharedResourcesService($http);
    }

    angular
        .module('diapp')
        .factory('diapp.services.filemanager.SharedResourcesService',
        factory);
}