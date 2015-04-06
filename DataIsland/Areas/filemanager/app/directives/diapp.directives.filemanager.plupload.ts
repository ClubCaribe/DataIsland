
module diapp.diresctives.filemanager {

    interface PlUploadScope extends ng.IScope {
        initUploader: any;
        uploader: any;
        fileUploadUrl: string;
        maxFileSize: string;
        chunkSize: string;
        flashSwfUrl: string;
        silverlightXapUrl: string;
        onFileUploaded: any;
        onUploadCompleted: any;
    }

    class PlUploadComponent implements ng.IDirective {
        static instance(): ng.IDirective {
            return new PlUploadComponent;
        }

        restrict = 'EA';
        scope = {
            fileUploadUrl: "=",
            maxFileSize: "=",
            chunkSize: "=",
            flashSwfUrl: "=",
            silverlightXapUrl: "=",
            onFileUploaded: "&",
            onUploadCompleted: "&"
        }
        replace = true;
        link(scope: PlUploadScope, element: ng.IAugmentedJQuery,attributes: ng.IAttributes): void {

            scope.uploader = element;

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

            scope.$watch("fileUploadUrl",(newValue, oldValue) => {
                if (scope.uploader != undefined) {
                    var up = scope.uploader.pluploadQueue();
                    if (up != undefined) {
                        if (up.settings != undefined) {
                            up.settings.url = newValue;
                        } 
                    }
                }
            }); 

            angular.element(document).ready(function () { 
                scope.initUploader(); 
            });
        }
    }

    angular
        .module('diapp')
        .directive('plUploadComponent', PlUploadComponent.instance);
}  