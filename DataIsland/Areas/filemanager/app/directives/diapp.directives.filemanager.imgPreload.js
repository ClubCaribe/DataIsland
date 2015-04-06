var diapp;
(function (diapp) {
    var diresctives;
    (function (diresctives) {
        var filemanager;
        (function (filemanager) {
            var ImgPreload = (function () {
                function ImgPreload() {
                    this.restrict = 'A';
                    this.scope = {
                        ngSrc: '@',
                        imgLoader: '@'
                    };
                }
                ImgPreload.instance = function () {
                    return new ImgPreload;
                };
                ImgPreload.prototype.link = function (scope, element, attributes) {
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
                };
                return ImgPreload;
            })();
            angular.module('diapp').directive('imgPreload', ImgPreload.instance);
        })(filemanager = diresctives.filemanager || (diresctives.filemanager = {}));
    })(diresctives = diapp.diresctives || (diapp.diresctives = {}));
})(diapp || (diapp = {}));
//# sourceMappingURL=diapp.directives.filemanager.imgPreload.js.map