module diapp.controllers.filemanager {
    'use strict';

    interface ISelectedResource {
        resourceId: string;
        userId: string;
        resourceName: string;
    }

    class UserSharedResourcesController {
        userId: string = "";
        searchPhrase: string = "";
        selectedResource: ISelectedResource = {
            resourceId: "",
            userId: "",
            resourceName: ""
        };
        isDetailsDisplayed: boolean = false;

        foreignResources: diapp.models.filemanager.IForeignSharedResource[] = null;

        static $inject = [
            '$scope',
            'diapp.services.filemanager.SharedResourcesService'
        ];
        constructor(private $scope:ng.IScope, private dataFactory: diapp.services.filemanager.ISharedResourcesService) {
            $(window).on('initsharedresources', (e:any) => {
                var userId = e.UserID;
                
                $scope.$apply(() =>{
                    this.InitResources(userId);
                });
            });
        }

        InitResources(userid: string):void {
            this.userId = userid;
            this.dataFactory.GetUserForeignSharedResources(this.userId).then((data) => {
                this.foreignResources = data; 
            });
        }

        ItemClicked(item: diapp.models.filemanager.IForeignSharedResource):void {
            this.selectedResource.resourceId = item.ID;
            this.selectedResource.resourceName = item.Name;
            this.selectedResource.userId = item.OwnerID;
            this.isDetailsDisplayed = true;
        }

        DeleteSharedResource(item: diapp.models.filemanager.IForeignSharedResource):void {
            if (confirm("[tr]Do you want to delete this resource?[/tr]")) {
                this.dataFactory.DeleteForeignResource(item.ID).then((result) => {
                    for (var i = 0; i < this.foreignResources.length; i++) {
                        if (this.foreignResources[i].ID == item.ID) {
                            this.foreignResources.splice(i, 1);
                            return;
                        }
                    }
                });
            }
        } 

        ReturnToList():void {
            this.isDetailsDisplayed = false;
        }
        
    }

    angular
        .module('diapp')
        .controller('diapp.filemanager.UserSharedResourcesController',
        UserSharedResourcesController);
} 