module diapp.diresctives.filemanager {
    interface IimgPreloadScope extends ng.IScope {
        ngSrc: string;
        imgLoader: string;
    } 

    class ImgPreload implements ng.IDirective {

        static instance(): ng.IDirective {
            return new ImgPreload;
        }

        restrict = 'A';
        scope = {
            ngSrc: '@',
            imgLoader: '@'
        };

        link(scope: IimgPreloadScope, element: ng.IAugmentedJQuery, attributes: ng.IAttributes): void {

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
    }

    angular
        .module('diapp')
        .directive('imgPreload', ImgPreload.instance);
} 