/// <reference path="../../../../Scripts/angular.js" />
/// <reference path="../../../../Scripts/jquery-2.1.3.intellisense.js" />

DiPanel.factory("foreignResourcesDataFactory", ['$http', '$q', function ($http, $q) {
    return {
        urlPrefix: "/",
        passportToken: "",
        ListDirectory: function (userID,resourceID,virtualPath) {
            var url = this.urlPrefix + "api/filemanager/foreignresources/listdirectory/" + userID.EscapeUserId() + "/" + resourceID.EscapeUserId() + "/" + virtualPath + "?diupt=" + this.passportToken;
            return $http.get(url);
        },
        GetPermissions: function (userID, resourceID) {
            var url = this.urlPrefix + "api/filemanager/foreignresources/getpermissions/" + userID.EscapeUserId() + "/" + resourceID.EscapeUserId() + "?diupt=" + this.passportToken;
            return $http.get(url);
        },
        GetPassportToken: function (dataislandid) {
            var url = "/api/panel/user/getpassporttoken";
            var data = {
                dataislandId: dataislandid
            }
            return $http.post(url, data);
        },
        GetUserDataIsland: function (userID) {
            var url = "/api/panel/user/getuserdataislanddata";
            var data = {
                userId: userID
            }
            return $http.post(url, data);
        }
    }
}]);

DiPanel.directive('diForeignResourcesBrowser', ['$filter', '$timeout', 'foreignResourcesDataFactory', function ($filter,$timeout, foreignResourcesDataFactory) {
    var dataFactory = foreignResourcesDataFactory;
    return {
        restrict: 'E',
        scope: {
            userId: '=',
            resourceId: "=",
            searchPhrase: "=",
            resourceName: "="
        },
        templateUrl: '/Areas/filemanager/app/partials/diForeignResources.tpl.html',
        link: function ($scope, element, attrs) {
            window.addEventListener("resize", function () {
                $scope.calculateDetailsViewImageSize();
            });
        },
        controller: function ($scope) {

            $scope.UserPassportToken = null;
            $scope.UserDataIsland = null;

            $scope.UserPermissions = {
                Read: false,
                Write: false,
                All: false
            }

            $scope.uploader = {
                fileUploadUrl: "/filemanager/file/uploadfile",
                maxFileSize: "3000MB",
                chunkSize: "1MB",
                flashSwfUrl: "/Scripts/plupload/Moxie.swf",
                silverlightXapUrl: "/Scripts/plupload/Moxie.xap"
            }
            $scope.sectionSelected = "files";
            $scope.viewModeSelected = "listMode";
            $scope.directory = "/";
            $scope.breadCrumbs = function () {
                var breadCrubmsArray = $scope.directory.split("/");
                if (breadCrubmsArray[0] == "") {
                    breadCrubmsArray.splice(0, 1);
                }
                if (breadCrubmsArray[breadCrubmsArray.length - 1] == "") {
                    breadCrubmsArray.splice(breadCrubmsArray.length - 1, 1);
                }
                return breadCrubmsArray;
            }
            $scope.directoryData = null;
            $scope.directorySummary = {
                TotalSize: 0,
                TotalNumFiles: 0
            };
            $scope.sortField = "";
            $scope.sortIsDescending = false;
            $scope.newDirectoryName = "";
            $scope.compressingFiles = [];
            $scope.viewMode = "list";
            $scope.detailsView = {
                isDetailsViewExpanded: false,
                imageContainerWidth: 100,
                imageContainerHeight: 100
            };
            $scope.isDetailsViewExpanded = true;
            $scope.SelectedFileIndex = 0;

            $scope.filteredDirectoryData = null;

            $scope.$watch('resourceId', function () {
                if($scope.resourceId === ""){
                    $scope.UserPassportToken = null;
                    return;
                }
                dataFactory.GetUserDataIsland($scope.userId).then(function (result) {
                    if (result.data != null) {

                        $scope.UserDataIsland = result.data;
                        dataFactory.urlPrefix = $scope.UserDataIsland.DataislandUrl;

                        dataFactory.GetPassportToken($scope.UserDataIsland.DataislandID).then(function (result) {

                            if (result.data != null) {
                                $scope.UserPassportToken = result.data;

                                dataFactory.passportToken = $scope.UserPassportToken.TokenID;

                                $scope.refreshClientToken();
                                dataFactory.GetPermissions($scope.userId, $scope.resourceId).then(function (result) {
                                    $scope.UserPermissions.Read = result.data.Read;
                                    $scope.UserPermissions.Write = result.data.Write;
                                    $scope.UserPermissions.All = result.data.All;
                                    $scope.refreshDirectory();
                                });
                            }
                        });
                    }
                });
            });

            $scope.$watch('directoryData', function () {
                $scope.updateFilteredData();
            });

            $scope.$watch('searchPhrase', function () {
                $scope.updateFilteredData();
            });

            $scope.$watch('sortField', function () {
                $scope.updateFilteredData();
            });

            $scope.$watch('sortIsDescending', function () {
                $scope.updateFilteredData();
            });

            $scope.updateFilteredData = function () {
                $scope.filteredDirectoryData = $filter('filter')($filter('orderBy')($scope.directoryData, $scope.sortField, $scope.sortIsDescending), $scope.searchPhrase);
            }

            $scope.refreshClientToken = function(){
                if($scope.UserPassportToken!=null){
                    var currentTime = new Date().getTime();
                    var tokendate = new Date($scope.UserPassportToken.ExpirationTime);
                    var timespan = tokendate - currentTime;
                    timespan = timespan - 2000;
                    $timeout(function(){
                        dataFactory.GetPassportToken($scope.UserDataIsland.DataislandID).then(function (result) {

                            if (result.data != null) {
                                $scope.UserPassportToken = result.data;
                                dataFactory.passportToken = $scope.UserPassportToken.TokenID;
                                $scope.refreshClientToken();
                            }
                        });
                    },timespan);
                }
            }

            $scope.changeMode = function (mode) {
                $scope.viewModeSelected = mode;

            }

            $scope.refreshDirectory = function () {
                $scope.viewMode = "list";
                dataFactory.ListDirectory($scope.userId,$scope.resourceId, $scope.directory).success(function (data) {
                    $scope.directoryData = data;
                    $scope.directorySummary.TotalNumFiles = 0;
                    $scope.directorySummary.TotalSize = 0;
                    for (var i = 0; i < data.length; i++) {
                        var item = data[i];
                        $scope.directorySummary.TotalNumFiles++;
                        $scope.directorySummary.TotalSize = $scope.directorySummary.TotalSize + item.Size;
                    }

                });
            }

            $scope.upDirectory = function () {
                var newindex = $scope.breadCrumbs().length - 1;
                if (newindex < 0) {
                    newindex = 0;
                }
                $scope.changeDirectory(newindex);
            }

            $scope.changeDirectory = function (breadcrumbsIndex) {
                $scope.directoryData = new Array();
                var breadCrubmsArray = $scope.directory.split("/");

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
                $scope.directoryData = new Array();
                $scope.directory = newDirectory;
                $scope.refreshDirectory();
            }

            $scope.itemClicked = function (item) {
                if (item != null) {
                    if (item.IsDirectory) {
                        $scope.directoryData = new Array();
                        $scope.directory = item.FileSystemObject.FullName;
                        $scope.refreshDirectory();
                    }
                }
            }

            $scope.ToggleSelection = function () {
                if ($scope.sectionSelected == "files") {
                    if ($scope.directoryData != undefined && $scope.directoryData.length > 0) {

                        if ($scope.isSomethingSelected()) {
                            for (var i = 0; i < $scope.directoryData.length; i++) {
                                $scope.directoryData[i].Checked = false;
                            }
                        }
                        else {
                            for (var i = 0; i < $scope.directoryData.length; i++) {
                                $scope.directoryData[i].Checked = true;
                            }
                        }
                    }
                }
                else if ($scope.sectionSelected == "shared") {
                    if ($scope.IsSomethingSelectedInSharedResourcesByMe()) {
                        for (var i = 0; i < $scope.ResourcesSharedByMe.Files.length; i++) {
                            $scope.ResourcesSharedByMe.Files[i].Checked = false;
                        }
                    }
                    else {
                        for (var i = 0; i < $scope.ResourcesSharedByMe.Files.length; i++) {
                            $scope.ResourcesSharedByMe.Files[i].Checked = true;
                        }
                    }
                }
            }

            $scope.isSomethingSelected = function () {
                var isSomethingSelected = false;
                for (var i = 0; i < $scope.directoryData.length; i++) {
                    if ($scope.directoryData[i].Checked) {
                        isSomethingSelected = true;
                    }
                }
                return isSomethingSelected;
            }

            $scope.SwitchToDetailsView = function (index) {
                $scope.SelectedFileIndex = index;
                $scope.viewMode = "details";
                $scope.isDetailsViewExpanded = false;
            }

            $scope.SwitchToListView = function () {
                $scope.viewMode = "list";
                $scope.isDetailsViewExpanded = false;
            }

            $scope.MoveNextItemIndex = function () {
                if ($scope.viewMode == "details" && $scope.SelectedFileIndex < $scope.filteredDirectoryData.length - 1) {
                    $scope.SelectedFileIndex++;
                }
            }

            $scope.toggleDetailsViewExpandedMode = function () {
                $scope.detailsView.isDetailsViewExpanded = !$scope.detailsView.isDetailsViewExpanded;
                $timeout(function () {
                    $scope.calculateDetailsViewImageSize();
                }, 50);
            }

            $scope.calculateDetailsViewImageSize = function () {
                if ($scope.detailsView.isDetailsViewExpanded) {
                    var containerWidth, containerHeight, occupiedHeight;
                    var detailsContainer = $("#diForeignResourcesBrowserdetailsViewContainer");
                    containerWidth = $(detailsContainer).width();
                    containerHeight = $(detailsContainer).height();
                    occupiedHeight = $("#diForeignResourcesBrowserdetailsViewMenu").outerHeight() + $("#diForeignResourcesBrowserdetailsViewFileName").outerHeight() + $("#diForeignResourcesBrowserdetailsViewFileDescription").outerHeight() + 50;
                    $scope.detailsView.imageContainerHeight = containerHeight - occupiedHeight;
                    $scope.detailsView.imageContainerWidth = containerWidth - 50;
                }
            }

            $scope.MovePreviousItemIndex = function () {
                if ($scope.viewMode == "details" && $scope.SelectedFileIndex > 0) {
                    $scope.SelectedFileIndex--;
                }
            }

        }
    };
}]);