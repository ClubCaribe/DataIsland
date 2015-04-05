module diapp.filemanager.services {

    export interface IForeignFileResourcesService {
        urlPrefix: string;
        ListDirectory(userID: string, resourceID: string, virtualPath: string): ng.IPromise<diapp.models.filemanager.IDiDirectoryListingEntry[]>;
        GetPermissions(userID: string, resourceID: string): ng.IPromise<diapp.models.filemanager.IDiForeignResourcePermissions>;
        GetUserDataIsland(userID:string): ng.IPromise<diapp.models.IClientDataIslandData>;
    }


    class ForeignFileResourcesService implements IForeignFileResourcesService {

        public urlPrefix: string = "/";

        constructor(private $http: ng.IHttpService) {
            
        }

        ListDirectory(userID: string, resourceID: string, virtualPath: string): ng.IPromise<diapp.models.filemanager.IDiDirectoryListingEntry[]> {
            var url = this.urlPrefix + "api/filemanager/foreignresources/listdirectory/" + userID.EscapeUserId() + "/" + resourceID.EscapeUserId() + "/" + virtualPath;
            return this.$http.get(url)
                .then((response: ng.IHttpPromiseCallbackArg<diapp.models.filemanager.IDiDirectoryListingEntry[]>): diapp.models.filemanager.IDiDirectoryListingEntry[] => {
                return <diapp.models.filemanager.IDiDirectoryListingEntry[]>response.data;
            });

        }

        GetPermissions(userID: string, resourceID: string): ng.IPromise<diapp.models.filemanager.IDiForeignResourcePermissions> {
            var url = this.urlPrefix + "api/filemanager/foreignresources/getpermissions/" + userID.EscapeUserId() + "/" + resourceID.EscapeUserId();
            return this.$http.get(url)
                .then((response: ng.IHttpPromiseCallbackArg<diapp.models.filemanager.IDiForeignResourcePermissions>): diapp.models.filemanager.IDiForeignResourcePermissions => {
                    return <diapp.models.filemanager.IDiForeignResourcePermissions>response.data;
            });
        }

        GetUserDataIsland(userID: string): ng.IPromise<diapp.models.IClientDataIslandData> {

            var url = "/api/panel/user/getuserdataislanddata";
            var data = {
                userId: userID
            }
            return this.$http.post(url, data)
                .then((response: ng.IHttpPromiseCallbackArg<diapp.models.IClientDataIslandData>): diapp.models.IClientDataIslandData => {
                    return <diapp.models.IClientDataIslandData>response.data;
            });
             
        }
    }

    factory.$inject = [
        '$http'
    ];
    function factory($http: ng.IHttpService): IForeignFileResourcesService {
        return new ForeignFileResourcesService($http);
    }

    angular
        .module('diapp')
        .factory('diapp.filemanager.services.ForeignFileResourcesService',
        factory);
}
