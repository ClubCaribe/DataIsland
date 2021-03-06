﻿/// <reference path="../../../../Scripts/angular.js" />
/// <reference path="../../../../Scripts/jquery-2.1.1.intellisense.js" />

DiPanel.factory("fileManagerDataFactory", ['$http', '$q', function ($http, $q) {
    return {
        ListDirectory: function (virtualPath) {
            var url = "/api/filemanager/directory/listdirectory" + virtualPath;
            return $http.get(url);
        },
        DeleteFiles: function (filenames) {
            var url = "/api/filemanager/file/delete";
            return $http.post(url, filenames);
        },
        DeleteDirectories: function (directoryNames) {
            var url = "/api/filemanager/directory/delete";
            return $http.post(url, directoryNames);
        },
        CreateDirectory: function (dirpath) {
            var url = "/api/filemanager/directory/create";
            var data = {
                path: dirpath
            }
            return $http.post(url, data);
        },
        PrepareFilesForDownload: function (filenames) {
            var url = "/api/filemanager/file/compress";
            return $http.post(url, filenames);
        },
        RenameCompressedFile: function (filename, newfilename) {
            var url = "/api/filemanager/file/renamecompressedfile/" + filename + "/" + newfilename;
            return $http.get(url);
        },
        CancelCompress: function (filename) {
            var url = "/api/filemanager/file/cancelcompressing/" + filename;
            return $http.get(url);
        },
        GetCompressedFiles: function () {
            var url = "/api/filemanager/file/getcompressedfiles";
            return $http.get(url);
        },
        DeleteCompressedFile: function (filename) {
            var url = "/api/filemanager/file/deletecompressedfile/" + filename;
            return $http.get(url);
        },
        MoveFile: function (oldFileName, newFileName) {
            var url = "/api/filemanager/file/move";
            var data = {
                OldFileName: oldFileName,
                NewFileName: newFileName
            };
            return $http.post(url,data);
        },
        CopyFile: function (oldFileName, newFileName) {
            var url = "/api/filemanager/file/copy";
            var data = {
                OldFileName: oldFileName,
                NewFileName: newFileName
            };
            return $http.post(url, data);
        },
        RotateFlipPreview: function (path, transformType) {
            var url = "/api/filemanager/file/rotateflip";
            var data = {
                Path: path,
                TransformType: transformType
            };
            return $http.post(url, data);
        },
        GetContacts: function () {
            var apiUrl = "/api/panel/usercontacts/getcontacts";
            return $http.get(apiUrl);
        },
        GetSharedOptions: function (filePaths) {
            var url = "/api/filemanager/shared/getresourcesoptions";
            return $http.post(url, filePaths);
        },
        SetSharedOptions: function (files) {
            var url = "/api/filemanager/shared/setresourcesoptions";
            return $http.post(url, files);
        },
        GetSharedResourcesSharedByMe: function () {
            var url = "/api/filemanager/shared/getresourcessharedbyme";
            return $http.get(url);
        },
        GetResourcesSharedByOthers: function () {
            var url = "/api/filemanager/shared/getresourcessharedbyothers";
            return $http.get(url);
        },
        DeleteForeignResource: function (id) {
            var url = "/api/filemanager/shared/deletesharedresource/" + id;
            return $http.get(url);
        }
    }
}]);

DiPanel.controller('fileManagerController', ['$scope', 'fileManagerDataFactory', '$filter', '$timeout', function ($scope, fileManagerDataFactory, $filter, $timeout) {
    $scope.uploader = {
        fileUploadUrl: "/filemanager/file/uploadfile",
        maxFileSize: "3000MB",
        chunkSize: "1MB",
        flashSwfUrl: "/Scripts/plupload/Moxie.swf",
        silverlightXapUrl: "/Scripts/plupload/Moxie.xap"
    }

    $scope.sectionSelected = "files";
    $scope.viewModeSelected = "listMode";
    $scope.sharedSectionModeSelected = "list";
    $scope.foreignResourcesConfig = {
        userId: "",
        resourceId: "",
        resourceName: ""
    }
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
    $scope.searchPhrase = "";
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

    //$scope.filteredDirectoryData = $filter('orderBy')($scope.filteredDirectoryData, {$scope.sortField$scope.sortIsDescending});
    //var testDownload = {
    //    Name: "a1",
    //    DisplayName: "File to download",
    //    Progress: 30
    //}

    //$scope.compressingFiles.push(testDownload);

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
        $scope.filteredDirectoryData = $filter('filter')($filter('orderBy')($scope.directoryData, $scope.sortField,$scope.sortIsDescending), $scope.searchPhrase);
    }

    $scope.changeMode = function (mode) {
        $scope.viewModeSelected = mode;
        
    }

    $scope.changeSection = function (mode) {
        $scope.sectionSelected = mode;
        if (mode === "shared") {
            $scope.InitResourcesSharedByMe();
            $scope.GetResourcesSharedByOthers();
        }
    }

    $scope.GetCurrentUserId = function () {
        return diCurrentUserID;
    }

    $scope.openUploadContainer = function () {
        var $this = $("#uploadContainer"),
				$panel = $this.closest('.panel'),
				$body = $panel.children('.panel-body, .table'),
				do_collapse = !$panel.hasClass('panel-collapse');

        if ($panel.is('[data-collapsed="0"]')) {
            do_collapse = false;
        }

        if (!do_collapse) {
            $body.slideDown('normal', fit_main_content_height);
            $panel.removeClass('panel-collapse');
        }
    }

    $scope.refreshDirectory = function () {
        $scope.viewMode = "list";
        fileManagerDataFactory.ListDirectory($scope.directory).success(function (data) {
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

    $scope.changeSortOrder = function (column) {
        if (column != $scope.sortField) {
            $scope.sortIsDescending = false;
            $scope.sortField = column;
        } else {
            $scope.sortIsDescending = !$scope.sortIsDescending;
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

    $scope.fileUploaded = function(file){

    }

    $scope.fileUploadCompleted = function (files) {
        $scope.refreshDirectory();
    }

    $scope.deleteSelectedItems = function () {
        if ($scope.isSomethingSelected()) {
            if (confirm("[tr]Do you want to delete selected items?[/tr]")) {
                var selectedFiles = [];
                var selectedDirectories = [];
                for (var i = 0; i < $scope.directoryData.length; i++) {
                    var item = $scope.directoryData[i];
                    if (item.Checked && !item.IsDirectory) {
                        selectedFiles.push(item.FullName);
                    }
                    if (item.Checked && item.IsDirectory) {
                        selectedDirectories.push(item.FullName);
                    }
                };
                if (selectedFiles.length > 0) {
                    fileManagerDataFactory.DeleteFiles(selectedFiles).success(function (data) {
                        $scope.refreshDirectory();
                    });
                }
                if (selectedDirectories.length > 0) {
                    fileManagerDataFactory.DeleteDirectories(selectedDirectories).success(function (data) {
                        $scope.refreshDirectory();
                    });
                }
            }
        }
    }

    $scope.AreItemsSelected = function()
    {
        if ($scope.directoryData != undefined && $scope.directoryData != null) {
            for (var i = 0; i < $scope.directoryData.length; i++) {
                var item = $scope.directoryData[i];
                if (item.Checked) {
                    return true;
                }
            }
        }
        return false;
    }

    $scope.GetSelectedItems = function () {
        var selectedItems = new Array();
        if ($scope.directoryData != undefined && $scope.directoryData != null) {
            for (var i = 0; i < $scope.directoryData.length; i++) {
                var item = $scope.directoryData[i];
                if (item.Checked) {
                    selectedItems.push(item);
                }
            }
        }
        return selectedItems;
    }

    $scope.initNewDirectoryDialog = function () {
        $scope.newDirectoryName = "";
    }

    $scope.createNewDirectory = function () {
        if ($scope.newDirectoryName.length > 0) {
            var fullName = $scope.directory + "/" + $scope.newDirectoryName;
            fileManagerDataFactory.CreateDirectory(fullName).success(function (data) {
                if (data.result) {
                    $scope.refreshDirectory();
                } else {
                    toastr.error("[tr]Directory creation failed.[/tr]", "[tr]Create Directory[/tr]");
                }
            }).error(function () {
                toastr.error("[tr]Directory creation failed.[/tr]", "[tr]Create Directory[/tr]");
            });
        }
    }

    $scope.downloadSelectedFiles = function () {
        if ($scope.isSomethingSelected()) {
            var selectedFiles = [];
            for (var i = 0; i < $scope.directoryData.length; i++) {
                var item = $scope.directoryData[i];
                if (item.Checked) {
                    selectedFiles.push(item.FullName);
                }
            }
            if (selectedFiles.length > 0) {

                fileManagerDataFactory.PrepareFilesForDownload(selectedFiles).success(function (data) {
                    if (data.name != "error") {
                        var progressFile = {
                            Name: data.name,
                            DisplayName: data.name,
                            Progress: 0,
                            Size: 0
                        }
                        $scope.compressingFiles.push(progressFile);
                    } else {
                        toastr.error("[tr]Error occured. Please try again.[/tr]", "[tr]Download files[/tr]");
                    }
                });
            }
        }
    }

    $scope.SetFilesCompressionProgress = function (ZipFileName, percent) {
        if ($scope.compressingFiles.length > 0) {
            for (var i = 0; i < $scope.compressingFiles.length; i++) {
                var item = $scope.compressingFiles[i];
                if (item.Name == ZipFileName) {
                    item.Progress = percent;
                    if (item.Progress == 100 ) {
                        if (item.DisplayName.length > 0 && item.Name != item.DisplayName) {
                            item.Name = item.DisplayName;
                        }
                    }
                }
            }
        }
    }

    $scope.RemoveFileFromCompressionFiles = function (name) {
        if ($scope.compressingFiles.length > 0) {
            for (var i = 0; i < $scope.compressingFiles.length; i++) {
                var item = $scope.compressingFiles[i];
                if (item.Name == name) {
                    $scope.compressingFiles.splice(i, 1);
                }
            }
        }
    }

    $scope.CancelCompress = function (name) {
        fileManagerDataFactory.CancelCompress(name).success(function (data) {

        });
    }

    $scope.RenameCompressedFile = function (item) {
        if (item.DisplayName.length == 0) {
            item.DisplayName = item.Name;
            return;
        }
        if (item.DisplayName.lastIndexOf(".zip") < 0) {
            item.DisplayName = item.DisplayName + ".zip";
        }
        fileManagerDataFactory.RenameCompressedFile(item.Name, item.DisplayName).success(function (data) {
            if (data.result == true) {
                if (item.Progress == 100) {
                    item.Name = item.DisplayName;
                }
            } else {
                item.DisplayName = item.Name;
            }
        });
    }

    $scope.SetCompressedFileSize = function (name,size) {
        if ($scope.compressingFiles.length > 0) {
            for (var i = 0; i < $scope.compressingFiles.length; i++) {
                var item = $scope.compressingFiles[i];
                if (item.Name == name) {
                    item.Size = size;
                }
            }
        }
    }

    $scope.DeleteCompressedFile = function (filename) {
        if (confirm('[tr]Delete selected file?[/tr]')) {
            fileManagerDataFactory.DeleteCompressedFile(filename).success(function (data) {
                if (data.result == true) {
                    $scope.RemoveFileFromCompressionFiles(filename);
                }
            });
        }
    }

    $scope.GetCompressedFiles = function () {
        fileManagerDataFactory.GetCompressedFiles().success(function (data) {
            $scope.compressingFiles = data;
        });
    }

    $scope.hover = function (item,state) {
        // Shows/hides the delete button on hover
        return item.isMouseOver = state;
    };

    $scope.SwitchToRenameMode = function (item) {
        item.isInRenameMode = true;
        item.NewName = item.Name;
    }

    $scope.CancelRename = function (item) {
        item.isInRenameMode = false;
    }

    $scope.RenameFile = function (item) {
        
        if (item.NewName != item.Name) {
            fileManagerDataFactory.MoveFile($scope.directory + "/" + item.Name, $scope.directory + "/" + item.NewName).success(function (data) {
                if (data.result == true) {
                    item.Name = item.NewName;
                    item.FileSystemObject.FullName = $scope.directory + "/" + item.NewName;
                }
                item.isInRenameMode = false;
            })
        }
    };

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

    $scope.calculateDetailsViewImageSize = function(){
        if ($scope.detailsView.isDetailsViewExpanded) {
            var containerWidth, containerHeight,occupiedHeight;
            var detailsContainer = $("#detailsViewContainer");
            containerWidth = $(detailsContainer).width();
            containerHeight = $(detailsContainer).height();
            occupiedHeight = $("#detailsViewMenu").outerHeight() + $("#detailsViewFileName").outerHeight() + $("#detailsViewFileDescription").outerHeight()+50;
            $scope.detailsView.imageContainerHeight = containerHeight - occupiedHeight;
            $scope.detailsView.imageContainerWidth = containerWidth-50;
        }
    }

    $scope.MovePreviousItemIndex = function () {
        if ($scope.viewMode == "details" && $scope.SelectedFileIndex > 0) {
            $scope.SelectedFileIndex--;
        }
    }

    $scope.RotateFlipPreview = function (transform) {
        var detailsItem = $scope.filteredDirectoryData[$scope.SelectedFileIndex];
        fileManagerDataFactory.RotateFlipPreview($scope.directory + "/" + detailsItem.Name, transform).success(function (data) {
            if (data.result == true) {
                refreshImage("filemanager/file/preview/1000" + $scope.directory + "/" + detailsItem.Name, replaceImage("detailsImagePreview"));
                refreshImage("filemanager/file/previewcustomsize/" + $scope.detailsView.imageContainerWidth + "/" + $scope.detailsView.imageContainerHeight + $scope.directory + "/" + detailsItem.Name, replaceImage("detailsImagePreviewExpanded"));
                refreshImage("filemanager/file/preview/400" + $scope.directory + "/" + detailsItem.Name, replaceImage("iconFilePreview" + $scope.SelectedFileIndex));
                refreshImage("filemanager/file/preview/30" + $scope.directory + "/" + detailsItem.Name, replaceImage("listFilePreview" + $scope.SelectedFileIndex));
            }
        });
    }

    //#region sharing options dialog

    $scope.ShareOptions = {
        SelectedFiles: new Array(),
        SelectedContacts: new Array(),
        Contacts: new Array(),
        Options: {
            Read: false,
            Write: false,
            All: false,
            IsPublic: false
        }
    }

    $scope.GetContacts = function () {
        fileManagerDataFactory.GetContacts().then(function (result) {
            $scope.ShareOptions.Contacts = result.data;
        });
    }

    $scope.InitShareOptionsDialog = function () {
        if ($scope.sectionSelected == "files" && !$scope.AreItemsSelected())
        {
            alert("[tr]One or more items must be selected[/tr]");
            return;
        }
        var filesList = new Array();
        $scope.ShareOptions.SelectedFiles = new Array();
        $scope.ShareOptions.SelectedContacts = new Array();
        var selectedFiles = null;
        if ($scope.sectionSelected == "files") {
            selectedFiles = $scope.GetSelectedItems();
            if ($scope.directory.length == 0) {
                $scope.directory = "/";
            }
            var directoryToSend = ($scope.directory.length > 1) ? $scope.directory + "/" : $scope.directory;
            for (var i = 0; i < selectedFiles.length; i++) {
                filesList.push(directoryToSend + selectedFiles[i].Name);
                $scope.ShareOptions.SelectedFiles.push({
                    ID: "",
                    Name: selectedFiles[i].Name,
                    FullPath: directoryToSend + selectedFiles[i].Name,
                    IsDirectory: selectedFiles[i].IsDirectory,
                    IsPublic: false,
                    IsRead: false,
                    IsWrite: false,
                    IsAll: false,
                    Recipients: new Array()
                });
            }
        } else if ($scope.sectionSelected == "shared") {
            selectedFiles = $scope.GetSelectedSharedResourcesByMe();
            if (selectedFiles.length == 0) {
                alert("[tr]One or more items must be selected[/tr]");
                return;
            }
            for (var i = 0; i < selectedFiles.length; i++) {
                filesList.push(selectedFiles[i].FullPath);
                $scope.ShareOptions.SelectedFiles.push({
                    ID: "",
                    Name: selectedFiles[i].FullPath.substring(selectedFiles[i].FullPath.lastIndexOf("/")+1),
                    FullPath: selectedFiles[i].FullPath,
                    IsDirectory: selectedFiles[i].IsDirectory,
                    IsPublic: false,
                    IsRead: false,
                    IsWrite: false,
                    IsAll: false,
                    Recipients: new Array()
                });
            }
        }
        fileManagerDataFactory.GetSharedOptions(filesList).then(function (result) {
            
            for (var i = 0; i < result.data.length; i++) {
                $scope.PopulateSharedFileOptions(result.data[i]);
            }

            if ($scope.ShareOptions.Contacts.length == 0) {
                fileManagerDataFactory.GetContacts().then(function (result) {

                    $scope.ShareOptions.Contacts = result.data;
                    $scope.PopulateSharedOptions();
                    jQuery('#shareOptionsDialog').modal('show');
                });
            }
            else
            {
                $scope.PopulateSharedOptions();
                jQuery('#shareOptionsDialog').modal('show');
            }
        })
        
    }

    $scope.SetShareOptions = function () {
        var recipients = new Array();
        for (var i = 0; i < $scope.ShareOptions.SelectedContacts.length; i++) {
            var recipient = {
                ID: "",
                RecipientID: $scope.ShareOptions.SelectedContacts[i].UserId,
                ResourceID: "",
                Deleted: false
            }
            recipients.push(recipient);
        }

        for (var i = 0; i < $scope.ShareOptions.SelectedFiles.length; i++) {
            $scope.ShareOptions.SelectedFiles[i].Recipients = recipients;
            $scope.ShareOptions.SelectedFiles[i].IsPublic = $scope.ShareOptions.Options.IsPublic;
            $scope.ShareOptions.SelectedFiles[i].IsRead = $scope.ShareOptions.Options.Read;
            $scope.ShareOptions.SelectedFiles[i].IsWrite = $scope.ShareOptions.Options.Write;
            $scope.ShareOptions.SelectedFiles[i].IsAll = $scope.ShareOptions.Options.All;
        }

        if ($scope.ShareOptions.SelectedFiles.length > 0) {
            for (var i = 0; i < $scope.ShareOptions.SelectedFiles.length; i++) {
                if ($scope.sectionSelected == "files") {
                    for (var k = 0; k < $scope.directoryData.length; k++) {
                        if ((($scope.directoryData[k].IsDirectory) ? "" : "/") + $scope.directoryData[k].FullName == $scope.ShareOptions.SelectedFiles[i].FullPath) {
                            $scope.directoryData[k].IsShared = (($scope.ShareOptions.SelectedContacts.length > 0) && ($scope.ShareOptions.Options.IsPublic || $scope.ShareOptions.Options.Read || $scope.ShareOptions.Options.Write || $scope.ShareOptions.Options.All));
                        }
                    }
                }
                else if ($scope.sectionSelected == "shared") {
                    for (var k = 0; k < $scope.ResourcesSharedByMe.Files.length; k++) {
                        if ($scope.ResourcesSharedByMe.Files[k].FullPath == $scope.ShareOptions.SelectedFiles[i].FullPath) {
                            $scope.ResourcesSharedByMe.Files[k].IsPublic = $scope.ShareOptions.Options.IsPublic;
                            $scope.ResourcesSharedByMe.Files[k].IsRead = $scope.ShareOptions.Options.Read;
                            $scope.ResourcesSharedByMe.Files[k].IsWrite = $scope.ShareOptions.Options.Write;
                            $scope.ResourcesSharedByMe.Files[k].IsAll = $scope.ShareOptions.Options.All;
                            $scope.ResourcesSharedByMe.Files[k].Recipients = recipients;
                        }
                    }
                }
            }
        }

        fileManagerDataFactory.SetSharedOptions($scope.ShareOptions.SelectedFiles).then(function (result) {
            $scope.ToggleSelection();
            jQuery('#shareOptionsDialog').modal('hide');
            if (result) {
                toastr.success("[tr]Items sharing options has been successfully set[/tr]", "[tr]Sharing items[/tr]");
            } else {
                toastr.error("[tr]Error ocurred. Try again.[/tr]", "[tr]Sharing items[/tr]");
            }
        })
    }

    $scope.PopulateSharedFileOptions = function(item)
    {
        for (var i = 0; i < $scope.ShareOptions.SelectedFiles.length; i++) {
            if ($scope.ShareOptions.SelectedFiles[i].FullPath == item.FullPath) {
                $scope.ShareOptions.SelectedFiles[i].ID = item.ID;
                $scope.ShareOptions.SelectedFiles[i].IsPublic = item.IsPublic;
                $scope.ShareOptions.SelectedFiles[i].IsRead = item.IsRead;
                $scope.ShareOptions.SelectedFiles[i].IsWrite = item.IsWrite;
                $scope.ShareOptions.SelectedFiles[i].IsAll = item.IsAll;
                $scope.ShareOptions.SelectedFiles[i].Recipients = item.Recipients;
            }
        }
    }

    $scope.PopulateSharedOptions = function () {
        var recipientsId = new Array();

        $scope.ShareOptions.Options.Read = ($scope.ShareOptions.SelectedFiles.length > 0);
        $scope.ShareOptions.Options.Write = ($scope.ShareOptions.SelectedFiles.length > 0);
        $scope.ShareOptions.Options.All = ($scope.ShareOptions.SelectedFiles.length > 0);
        $scope.ShareOptions.Options.IsPublic = ($scope.ShareOptions.SelectedFiles.length > 0);

        for (var i = 0; i < $scope.ShareOptions.SelectedFiles.length; i++) {

            $scope.ShareOptions.Options.Read = (($scope.ShareOptions.Options.Read & $scope.ShareOptions.SelectedFiles[i].IsRead)==1);
            $scope.ShareOptions.Options.Write = (($scope.ShareOptions.Options.Write & $scope.ShareOptions.SelectedFiles[i].IsWrite)==1);
            $scope.ShareOptions.Options.All = (($scope.ShareOptions.Options.All & $scope.ShareOptions.SelectedFiles[i].IsAll)==1);
            $scope.ShareOptions.Options.IsPublic = (($scope.ShareOptions.Options.IsPublic & $scope.ShareOptions.SelectedFiles[i].IsPublic) == 1);

            for (var ii = 0; ii < $scope.ShareOptions.SelectedFiles[i].Recipients.length; ii++) {
                if ($scope.CheckRecipientExistsInAllSharedRecipients($scope.ShareOptions.SelectedFiles[i].Recipients[ii].RecipientID, i)) {
                    var recId = $scope.ShareOptions.SelectedFiles[i].Recipients[ii].RecipientID;
                    if (recId != undefined && recId != null) {
                        var exists = false;
                        for (var k = 0; k < recipientsId.length; k++) {
                            if (recipientsId[k] == recId) {
                                exists = true;
                            }
                        }
                        if (!exists) {
                            recipientsId.push(recId);
                        }
                    }
                }
            }
        }

        if (recipientsId.length > 0) {
            for (var i = 0; i < recipientsId.length; i++) {
                for (var ii = 0; ii < $scope.ShareOptions.Contacts.length; ii++) {
                    if ($scope.ShareOptions.Contacts[ii].UserId == recipientsId[i]) {
                        $scope.ShareOptions.SelectedContacts.push($scope.ShareOptions.Contacts[ii]);
                    }
                }
            }
        }
        
    }

    $scope.CheckRecipientExistsInAllSharedRecipients = function (recipientId, indexToSkip) {
        for (var i = 0; i < $scope.ShareOptions.SelectedFiles.length; i++) {
            if (i != indexToSkip) {
                var exists = false;
                for (var ii = 0; ii < $scope.ShareOptions.SelectedFiles[i].Recipients.length; ii++) {
                    if ($scope.ShareOptions.SelectedFiles[i].Recipients[ii].RecipientID == recipientId) {
                        exists = true;
                    }
                }
                if (!exists) {
                    return false;
                }
            }
        }
        return true;
    }
    //#endregion

    //#region resources shared by me
    $scope.ResourcesSharedByMe = {
        Files: new Array()
    };

    $scope.ResourcesSharedByOthers = {
        Files: new Array()
    }

    $scope.InitResourcesSharedByMe = function () {
        fileManagerDataFactory.GetSharedResourcesSharedByMe().then(function (result) {
            $scope.ResourcesSharedByMe.Files = result.data;
        });
    }

    $scope.GetSelectedSharedResourcesByMe = function () {
        var selected = new Array();
        for (var i = 0; i < $scope.ResourcesSharedByMe.Files.length; i++) {
            if ($scope.ResourcesSharedByMe.Files[i].Checked == true) {
                selected.push($scope.ResourcesSharedByMe.Files[i]);
            }
        }
        return selected;
    }

    $scope.IsSomethingSelectedInSharedResourcesByMe = function () {
        for (var i = 0; i < $scope.ResourcesSharedByMe.Files.length; i++) {
            if ($scope.ResourcesSharedByMe.Files[i].Checked == true) {
                return true;
            }
        }
        return false;
    }

    $scope.GoToFolderFromSharedResource = function (item) {
        if (item != null) {
            if (item.IsDirectory) {
                $scope.directory = item.FullPath;
            }
            else {
                $scope.directory = item.FullPath.substring(0, item.FullPath.lastIndexOf("/"));
            }

            $scope.refreshDirectory();

            $scope.changeSection("files");
        }
    }

    $scope.GetResourcesSharedByOthers = function () {
        fileManagerDataFactory.GetResourcesSharedByOthers().then(function (result) {
            $scope.ResourcesSharedByOthers.Files = result.data;
        });
    }

    $scope.GetCurrentHostname = function () {
        return location.protocol + '//' + location.hostname + (location.port ? ':' + location.port : '');
    }
    //#endregion

    //#region resources shared by others
    $scope.GoToForeignResource = function (item) {
        $scope.searchPhrase = "";
        $scope.foreignResourcesConfig.userId = item.OwnerID;
        $scope.foreignResourcesConfig.resourceId = item.ID;
        $scope.foreignResourcesConfig.resourceName = item.Name;
        $scope.sharedSectionModeSelected = "files";
    }

    $scope.ChangeSharedResourcesViewMode = function (mode) {
        $scope.sharedSectionModeSelected = mode;
    }

    $scope.DeleteSharedResource = function (item) {
        if (confirm("[tr]Do you want to delete this resource?[/tr]")) {
            fileManagerDataFactory.DeleteForeignResource(item.ID).then(function (result) {
                for (var i = 0; i < $scope.ResourcesSharedByOthers.Files.length; i++) {
                    if ($scope.ResourcesSharedByOthers.Files[i].ID == item.ID) {
                        $scope.ResourcesSharedByOthers.Files.splice(i, 1);
                        return;
                    }
                }
            });
        }
    }
    //#endregion

    $scope.refreshDirectory();
    $scope.GetCompressedFiles();
    $scope.GetContacts();

}]);

var srFileManager;

$(function () {

    document.ondragenter = function () {

        var $this = $("#uploadContainer"),
				$panel = $this.closest('.panel'),
				$body = $panel.children('.panel-body, .table'),
				do_collapse = !$panel.hasClass('panel-collapse');

        if ($panel.is('[data-collapsed="0"]')) {
            do_collapse = false;
        }

        if (!do_collapse) {
            $body.slideDown('normal', fit_main_content_height);
            $panel.removeClass('panel-collapse');
        }
        
    }

    //navigate between files with keys
    $("body").keyup(function (event) {
        if (event.which == 37) {
            var scope = angular.element($("body")).scope();
            scope.$apply(function () {
                scope.MovePreviousItemIndex();
            })
        }
        if (event.which == 39) {
            var scope = angular.element($("body")).scope();
            scope.$apply(function () {
                scope.MoveNextItemIndex();
            })
        }
    });

    srFileManager = $.connection.fileManagerHub;
    // Create a function that the hub can call back to display messages.
   srFileManager.client.FileZipIsReadyForDownload = function (zipName,size) {
       var scope = angular.element($("body")).scope();
       scope.$apply(function () {
           scope.SetCompressedFileSize(zipName, size);
       })
    };

    srFileManager.client.SetFilesCompressionProgress = function (ZipFileName, percent) {
        var scope = angular.element($("body")).scope();
        scope.$apply(function () {
            scope.SetFilesCompressionProgress(ZipFileName, percent);
        })
    };

    srFileManager.client.RemoveFileFromCompressionFiles = function (filename) {
        var scope = angular.element($("body")).scope();
        scope.$apply(function () {
            scope.RemoveFileFromCompressionFiles(filename);
        })
    }

});

window.addEventListener("resize", function () {
    var scope = angular.element($("body")).scope();
    scope.$apply(function () {
        scope.calculateDetailsViewImageSize();
    })
});

