/// <reference path="../../../../Scripts/angular.js" />

angular.module("dataisland.DynamicForms", [])
    .directive("diDynamicSettingsForm", function () {
        return {
            restrict: "E",
            scope: {
                formModel: "=",
                submitForm: "&onSubmitForm"
            },
            templateUrl: '/Areas/panel/app/partials/diDynamicForm.tpl.html',
            link: function (scope, iElement, iAttrs, ctrl) {
            }
        }
    });