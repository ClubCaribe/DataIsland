/// <reference path="../../../../Scripts/angular.js" />
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
        }
    }
}]);

DiPanel.directive('imgPreload', ['$rootScope', function ($rootScope) {
    return {
        restrict: 'A',
        scope: {
            ngSrc: '@',
            imgLoader: '@'
        },
        link: function (scope, element, attrs) {
            element.on('load', function () {
                element.addClass('in');
                if (document.getElementById(scope.imgLoader) != undefined) {
                    $(document.getElementById(scope.imgLoader)).removeClass('in');
                }
            }).on('error', function () {
                //
            });

            scope.$watch('ngSrc', function (newVal) {
                element.removeClass('in');
                if (document.getElementById(scope.imgLoader) != undefined) {
                    $(document.getElementById(scope.imgLoader)).addClass('in');
                }
                
            });
        }
    };
}]);

DiPanel.directive("plUploadComponent", function () {
    return {
        scope: {
            fileUploadUrl: "=",
            maxFileSize: "=",
            chunkSize: "=",
            flashSwfUrl: "=",
            silverlightXapUrl: "=",
            onFileUploaded: "&",
            onUploadCompleted: "&"
        },
        restrict: "EA",
        replace: 'true',
        link: function (scope, elem, attrs) {
            scope.uploader = elem;

            scope.initUploader = function () {
                scope.uploader.pluploadQueue({
                    runtimes: 'html5,html4,gears,browserplus,flash,silverlight',
                    url: scope.fileUploadUrl,
                    max_file_size: scope.maxFileSize,
                    chunk_size: scope.chunkSize,
                    flash_swf_url: scope.flashSwfUrl,
                    silverlight_xap_url: scope.silverlightXapUrl,
                    init: {
                        FileUploaded: function (up, files) {
                            if (scope.onFileUploaded != undefined) {
                                scope.onFileUploaded({
                                    file: files
                                });
                            }
                        },
                        UploadComplete: function (up, uploadedFiles) {
                            if (scope.onUploadCompleted != undefined) {
                                scope.onUploadCompleted({
                                    files: uploadedFiles
                                });
                            }
                            // destroy the uploader and init a new one
                            up.destroy();
                            scope.initUploader();
                        }
                    }
                });
            }

            angular.element(document).ready(function () {
                scope.initUploader();
            });
        }
    }
});

DiPanel.filter('bytes', function () {
    return function (bytes, precision) {
        if (bytes == 0) return "-";
        if (isNaN(parseFloat(bytes)) || !isFinite(bytes)) return '-';
        if (typeof precision === 'undefined') precision = 1;
        var units = ['bytes', 'kB', 'MB', 'GB', 'TB', 'PB'],
        number = Math.floor(Math.log(bytes) / Math.log(1024));
        return (bytes / Math.pow(1024, Math.floor(number))).toFixed(precision) + ' ' + units[number];
    }
});

DiPanel.controller('fileManagerController', ['$scope', 'fileManagerDataFactory', '$filter', '$timeout', function ($scope, fileManagerDataFactory, $filter, $timeout) {
    $scope.uploader = {
        fileUploadUrl: "/filemanager/file/uploadfile",
        maxFileSize: "3000MB",
        chunkSize: "1MB",
        flashSwfUrl: "/Scripts/plupload/Moxie.swf",
        silverlightXapUrl: "/Scripts/plupload/Moxie.xap"
    }

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

    $scope.refreshDirectory();
    $scope.GetCompressedFiles();

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

