module diapp.directives.filemanager {
    'use strict'

    
    interface IForeignResourcesScope extends ng.IScope {

        userId: string;
        resourceId: string;
        searchPhrase: string;
        resourceName: string;

        
    }

    class ForeignResourcesController {

        userId: string;
        resourceId: string;
        searchPhrase: string;
        resourceName: string;

        UserDataIsland: diapp.models.IClientDataIslandData = null;

        UserPermissions: diapp.models.filemanager.IDiForeignResourcePermissions = {
            Read: false,
            Write: false,
            All: false
        };

        uploader: diapp.models.filemanager.IPlUploaderArgs = {
            fileUploadUrl: "/filemanager/file/uploadfile",
            maxFileSize: "3000MB",
            chunkSize: "1MB",
            flashSwfUrl: "/Scripts/plupload/Moxie.swf",
            silverlightXapUrl: "/Scripts/plupload/Moxie.xap"
        };
        viewModeSelected: string = "listMode";
        directory: string = "/";
        directoryData: diapp.models.filemanager.IDiDirectoryListingEntry[] = null;
        directorySummary: diapp.models.filemanager.IDirectorySummary = {
            TotalSize: 0,
            TotalNumFiles: 0
        };
        sortField: string = "";
        sortIsDescending: boolean = false;
        newDirectoryName: string = "";
        viewMode: string = "list";
        detailsView: diapp.models.filemanager.IFileViewDetails = {
            isDetailsViewExpanded: false,
            imageContainerWidth: 100,
            imageContainerHeight: 100
        };
        isDetailsViewExpanded: boolean = true;
        SelectedFileIndex: number = 0;

        filteredDirectoryData: diapp.models.filemanager.IDiDirectoryListingEntry[] = null;

        static $inject = ['$scope','$filter','$timeout','diapp.filemanager.services.ForeignFileResourcesService'];
        constructor(private scope: IForeignResourcesScope, private $filter:ng.IFilterService, private $timeout:ng.ITimeoutService, private dataFactory: diapp.filemanager.services.IForeignFileResourcesService) {
            scope.$watch("fr.resourceId",(newValue, oldValue) => {
                this.onResourceIdChange(newValue);
            });

            scope.$watch('fr.directoryData',(newValue, oldValue) => {
                this.directoryData = newValue;
                this.updateFilteredData();
            });

            scope.$watch('fr.searchPhrase',(newValue, oldValue) => {
                this.searchPhrase = newValue;
                this.updateFilteredData();
            });

            scope.$watch('fr.sortField',(newValue, oldValue) => {
                this.sortField = newValue;
                this.updateFilteredData();
            });

            scope.$watch('fr.sortIsDescending',(newValue, oldValue) => {
                this.sortIsDescending = newValue;
                this.updateFilteredData();
            });
        }

        breadCrumbs():string[] {
            var breadCrubmsArray = this.directory.split("/");
            if (breadCrubmsArray[0] == "") {
                breadCrubmsArray.splice(0, 1);
            }
            if (breadCrubmsArray[breadCrubmsArray.length - 1] == "") {
                breadCrubmsArray.splice(breadCrubmsArray.length - 1, 1);
            }
            return breadCrubmsArray;
        }   

        onResourceIdChange(newresourceId:string): void {
            if (newresourceId === "") {
                return;
            }
            this.resourceId = newresourceId;
            this.dataFactory.GetUserDataIsland(this.userId).then((data) => {
                if (data != null) {

                    this.UserDataIsland = data;
                    this.dataFactory.urlPrefix = this.UserDataIsland.DataislandUrl;
                    this.dataFactory.GetPermissions(this.userId, this.resourceId).then((result) => {
                        this.UserPermissions.Read = result.Read;
                        this.UserPermissions.Write = result.Write;
                        this.UserPermissions.All = result.All;
                        this.refreshDirectory();
                    });
                } 
            }); 
        }
        
        updateFilteredData () {
            this.filteredDirectoryData = this.$filter('filter')(this.$filter('orderBy')(this.directoryData, this.sortField, this.sortIsDescending), this.searchPhrase);
        }

        changeMode(mode:string) {
            this.viewModeSelected = mode;

        }

        refreshDirectory() {
            this.viewMode = "list";
            this.dataFactory.ListDirectory(this.userId, this.resourceId, this.directory).then((data) => {
                this.directoryData = data;
                this.directorySummary.TotalNumFiles = 0;
                this.directorySummary.TotalSize = 0;
                for (var i = 0; i < data.length; i++) {
                    var item = data[i];
                    this.directorySummary.TotalNumFiles++;
                    this.directorySummary.TotalSize = this.directorySummary.TotalSize + item.Size;
                }

            });
        }
         
        upDirectory() {
            var newindex = this.breadCrumbs().length - 1;
            if (newindex < 0) {
                newindex = 0;
            }
            this.changeDirectory(newindex);
        }

        changeDirectory(breadcrumbsIndex) {
            this.directoryData = new Array();
            var breadCrubmsArray = this.directory.split("/");

            if (breadCrubmsArray[0] == "") {
                breadCrubmsArray.splice(0, 1);
            }
            if (breadCrubmsArray[breadCrubmsArray.length - 1] == "") {
                breadCrubmsArray.splice(breadCrubmsArray.length - 1, 1);
            }

            var newDirectory = "";
            for (var i = 0; i < breadcrumbsIndex; i++) {
                newDirectory = newDirectory + "/" + breadCrubmsArray[i];
            }
            this.directoryData = new Array();
            this.directory = newDirectory;
            this.refreshDirectory();
        }

        itemClicked(item) {
            if (item != null) {
                if (item.IsDirectory) {
                    this.directoryData = new Array();
                    this.directory = item.FileSystemObject.FullName;
                    this.refreshDirectory();
                }
            }
        }
         
        ToggleSelection() {
            if (this.directoryData != undefined && this.directoryData.length > 0) {

                    if (this.isSomethingSelected()) {
                        for (var i = 0; i < this.directoryData.length; i++) {
                            this.directoryData[i].Checked = false;
                        }
                    }
                    else {
                        for (var i = 0; i < this.directoryData.length; i++) {
                            this.directoryData[i].Checked = true;
                        }
                    }
                }
            
        }

        isSomethingSelected() {
            var isSomethingSelected = false;
            for (var i = 0; i < this.directoryData.length; i++) {
                if (this.directoryData[i].Checked) {
                    isSomethingSelected = true;
                }
            }
            return isSomethingSelected;
        }

        SwitchToDetailsView(index) {
            this.SelectedFileIndex = index;
            this.viewMode = "details";
            this.isDetailsViewExpanded = false;
        }

        SwitchToListView() {
            this.viewMode = "list";
            this.isDetailsViewExpanded = false;
        }

        MoveNextItemIndex() {
            if (this.viewMode == "details" && this.SelectedFileIndex < this.filteredDirectoryData.length - 1) {
                this.SelectedFileIndex++;
            }
        }

        toggleDetailsViewExpandedMode() {
            this.detailsView.isDetailsViewExpanded = !this.detailsView.isDetailsViewExpanded;
            this.$timeout(() => {
                this.calculateDetailsViewImageSize();
            }, 50);
        }
         
        calculateDetailsViewImageSize() {
            if (this.detailsView.isDetailsViewExpanded) {
                var containerWidth, containerHeight, occupiedHeight;
                var detailsContainer = $("#diForeignResourcesBrowserdetailsViewContainer");
                containerWidth = $(detailsContainer).width();
                containerHeight = $(detailsContainer).height();
                occupiedHeight = $("#diForeignResourcesBrowserdetailsViewMenu").outerHeight() + $("#diForeignResourcesBrowserdetailsViewFileName").outerHeight() + $("#diForeignResourcesBrowserdetailsViewFileDescription").outerHeight() + 50;
                this.detailsView.imageContainerHeight = containerHeight - occupiedHeight;
                this.detailsView.imageContainerWidth = containerWidth - 50;
            }
        }

        MovePreviousItemIndex() {
            if (this.viewMode == "details" && this.SelectedFileIndex > 0) {
                this.SelectedFileIndex--;
            }
        }
    }

    class ForeignResourcesDirective implements ng.IDirective {
        static instance(): ng.IDirective {
            return new ForeignResourcesDirective;
        }

        restrict = 'E';
        scope = {
            userId: "=",
            resourceId: "=",
            searchPhrase: "=",
            resourceName: "=",
        }
        controller = ForeignResourcesController;
        controllerAs = "fr";
        bindToController = true;
        templateUrl = '/Areas/filemanager/app/partials/diForeignResources.tpl.html';
        link(scope: IForeignResourcesScope, element: ng.IAugmentedJQuery,
            attributes: ng.IAttributes, controller: ForeignResourcesController): void {
        }
    }

    angular
        .module('diapp')
        .directive('diForeignResourcesBrowser', ForeignResourcesDirective.instance);
}