var diapp;
(function (diapp) {
    var diresctives;
    (function (diresctives) {
        var filemanager;
        (function (filemanager) {
            var PlUploadComponent = (function () {
                function PlUploadComponent() {
                    this.restrict = 'EA';
                    this.scope = {
                        fileUploadUrl: "=",
                        maxFileSize: "=",
                        chunkSize: "=",
                        flashSwfUrl: "=",
                        silverlightXapUrl: "=",
                        onFileUploaded: "&",
                        onUploadCompleted: "&"
                    };
                    this.replace = true;
                }
                PlUploadComponent.instance = function () {
                    return new PlUploadComponent;
                };
                PlUploadComponent.prototype.link = function (scope, element, attributes) {
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
                    };
                    scope.$watch("fileUploadUrl", function (newValue, oldValue) {
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
                };
                return PlUploadComponent;
            })();
            angular.module('diapp').directive('plUploadComponent', PlUploadComponent.instance);
        })(filemanager = diresctives.filemanager || (diresctives.filemanager = {}));
    })(diresctives = diapp.diresctives || (diapp.diresctives = {}));
})(diapp || (diapp = {}));
//# sourceMappingURL=diapp.directives.filemanager.plupload.js.map