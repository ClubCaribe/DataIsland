var diapp;
(function (diapp) {
    var directives;
    (function (directives) {
        var filemanager;
        (function (filemanager) {
            'use strict';
            var ForeignResourcesController = (function () {
                function ForeignResourcesController(scope, $filter, $timeout, dataFactory) {
                    var _this = this;
                    this.scope = scope;
                    this.$filter = $filter;
                    this.$timeout = $timeout;
                    this.dataFactory = dataFactory;
                    this.UserDataIsland = null;
                    this.UserPermissions = {
                        Read: false,
                        Write: false,
                        All: false
                    };
                    this.uploader = {
                        fileUploadUrl: "/filemanager/file/uploadfile",
                        maxFileSize: "3000MB",
                        chunkSize: "1MB",
                        flashSwfUrl: "/Scripts/plupload/Moxie.swf",
                        silverlightXapUrl: "/Scripts/plupload/Moxie.xap"
                    };
                    this.viewModeSelected = "listMode";
                    this.directory = "/";
                    this.directoryData = null;
                    this.directorySummary = {
                        TotalSize: 0,
                        TotalNumFiles: 0
                    };
                    this.sortField = "";
                    this.sortIsDescending = false;
                    this.newDirectoryName = "";
                    this.viewMode = "list";
                    this.detailsView = {
                        isDetailsViewExpanded: false,
                        imageContainerWidth: 100,
                        imageContainerHeight: 100
                    };
                    this.isDetailsViewExpanded = true;
                    this.SelectedFileIndex = 0;
                    this.filteredDirectoryData = null;
                    scope.$watch("fr.resourceId", function (newValue, oldValue) {
                        _this.onResourceIdChange(newValue);
                    });
                    scope.$watch('fr.directoryData', function (newValue, oldValue) {
                        _this.directoryData = newValue;
                        _this.updateFilteredData();
                    });
                    scope.$watch('fr.searchPhrase', function (newValue, oldValue) {
                        _this.searchPhrase = newValue;
                        _this.updateFilteredData();
                    });
                    scope.$watch('fr.sortField', function (newValue, oldValue) {
                        _this.sortField = newValue;
                        _this.updateFilteredData();
                    });
                    scope.$watch('fr.sortIsDescending', function (newValue, oldValue) {
                        _this.sortIsDescending = newValue;
                        _this.updateFilteredData();
                    });
                }
                ForeignResourcesController.prototype.breadCrumbs = function () {
                    var breadCrubmsArray = this.directory.split("/");
                    if (breadCrubmsArray[0] == "") {
                        breadCrubmsArray.splice(0, 1);
                    }
                    if (breadCrubmsArray[breadCrubmsArray.length - 1] == "") {
                        breadCrubmsArray.splice(breadCrubmsArray.length - 1, 1);
                    }
                    return breadCrubmsArray;
                };
                ForeignResourcesController.prototype.onResourceIdChange = function (newresourceId) {
                    var _this = this;
                    if (newresourceId === "") {
                        return;
                    }
                    this.resourceId = newresourceId;
                    this.dataFactory.GetUserDataIsland(this.userId).then(function (data) {
                        if (data != null) {
                            _this.UserDataIsland = data;
                            _this.dataFactory.urlPrefix = _this.UserDataIsland.DataislandUrl;
                            _this.dataFactory.GetPermissions(_this.userId, _this.resourceId).then(function (result) {
                                _this.UserPermissions.Read = result.Read;
                                _this.UserPermissions.Write = result.Write;
                                _this.UserPermissions.All = result.All;
                                _this.refreshDirectory();
                            });
                        }
                    });
                };
                ForeignResourcesController.prototype.updateFilteredData = function () {
                    this.filteredDirectoryData = this.$filter('filter')(this.$filter('orderBy')(this.directoryData, this.sortField, this.sortIsDescending), this.searchPhrase);
                };
                ForeignResourcesController.prototype.changeMode = function (mode) {
                    this.viewModeSelected = mode;
                };
                ForeignResourcesController.prototype.refreshDirectory = function () {
                    var _this = this;
                    this.viewMode = "list";
                    this.dataFactory.ListDirectory(this.userId, this.resourceId, this.directory).then(function (data) {
                        _this.directoryData = data;
                        _this.directorySummary.TotalNumFiles = 0;
                        _this.directorySummary.TotalSize = 0;
                        for (var i = 0; i < data.length; i++) {
                            var item = data[i];
                            _this.directorySummary.TotalNumFiles++;
                            _this.directorySummary.TotalSize = _this.directorySummary.TotalSize + item.Size;
                        }
                    });
                };
                ForeignResourcesController.prototype.upDirectory = function () {
                    var newindex = this.breadCrumbs().length - 1;
                    if (newindex < 0) {
                        newindex = 0;
                    }
                    this.changeDirectory(newindex);
                };
                ForeignResourcesController.prototype.changeDirectory = function (breadcrumbsIndex) {
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
                };
                ForeignResourcesController.prototype.itemClicked = function (item) {
                    if (item != null) {
                        if (item.IsDirectory) {
                            this.directoryData = new Array();
                            this.directory = item.FileSystemObject.FullName;
                            this.refreshDirectory();
                        }
                    }
                };
                ForeignResourcesController.prototype.ToggleSelection = function () {
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
                };
                ForeignResourcesController.prototype.isSomethingSelected = function () {
                    var isSomethingSelected = false;
                    for (var i = 0; i < this.directoryData.length; i++) {
                        if (this.directoryData[i].Checked) {
                            isSomethingSelected = true;
                        }
                    }
                    return isSomethingSelected;
                };
                ForeignResourcesController.prototype.SwitchToDetailsView = function (index) {
                    this.SelectedFileIndex = index;
                    this.viewMode = "details";
                    this.isDetailsViewExpanded = false;
                };
                ForeignResourcesController.prototype.SwitchToListView = function () {
                    this.viewMode = "list";
                    this.isDetailsViewExpanded = false;
                };
                ForeignResourcesController.prototype.MoveNextItemIndex = function () {
                    if (this.viewMode == "details" && this.SelectedFileIndex < this.filteredDirectoryData.length - 1) {
                        this.SelectedFileIndex++;
                    }
                };
                ForeignResourcesController.prototype.toggleDetailsViewExpandedMode = function () {
                    var _this = this;
                    this.detailsView.isDetailsViewExpanded = !this.detailsView.isDetailsViewExpanded;
                    this.$timeout(function () {
                        _this.calculateDetailsViewImageSize();
                    }, 50);
                };
                ForeignResourcesController.prototype.calculateDetailsViewImageSize = function () {
                    if (this.detailsView.isDetailsViewExpanded) {
                        var containerWidth, containerHeight, occupiedHeight;
                        var detailsContainer = $("#diForeignResourcesBrowserdetailsViewContainer");
                        containerWidth = $(detailsContainer).width();
                        containerHeight = $(detailsContainer).height();
                        occupiedHeight = $("#diForeignResourcesBrowserdetailsViewMenu").outerHeight() + $("#diForeignResourcesBrowserdetailsViewFileName").outerHeight() + $("#diForeignResourcesBrowserdetailsViewFileDescription").outerHeight() + 50;
                        this.detailsView.imageContainerHeight = containerHeight - occupiedHeight;
                        this.detailsView.imageContainerWidth = containerWidth - 50;
                    }
                };
                ForeignResourcesController.prototype.MovePreviousItemIndex = function () {
                    if (this.viewMode == "details" && this.SelectedFileIndex > 0) {
                        this.SelectedFileIndex--;
                    }
                };
                ForeignResourcesController.$inject = ['$scope', '$filter', '$timeout', 'diapp.filemanager.services.ForeignFileResourcesService'];
                return ForeignResourcesController;
            })();
            var ForeignResourcesDirective = (function () {
                function ForeignResourcesDirective() {
                    this.restrict = 'E';
                    this.scope = {
                        userId: "=",
                        resourceId: "=",
                        searchPhrase: "=",
                        resourceName: "=",
                    };
                    this.controller = ForeignResourcesController;
                    this.controllerAs = "fr";
                    this.bindToController = true;
                    this.templateUrl = '/Areas/filemanager/app/partials/diForeignResources.tpl.html';
                }
                ForeignResourcesDirective.instance = function () {
                    return new ForeignResourcesDirective;
                };
                ForeignResourcesDirective.prototype.link = function (scope, element, attributes, controller) {
                };
                return ForeignResourcesDirective;
            })();
            angular.module('diapp').directive('diForeignResourcesBrowser', ForeignResourcesDirective.instance);
        })(filemanager = directives.filemanager || (directives.filemanager = {}));
    })(directives = diapp.directives || (diapp.directives = {}));
})(diapp || (diapp = {}));
//# sourceMappingURL=diapp.directives.filemanager.foreignresourcesbrowser.js.map