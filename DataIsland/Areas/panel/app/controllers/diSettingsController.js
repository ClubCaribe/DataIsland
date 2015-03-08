/// <reference path="../../../../Scripts/angular.js" />

DiPanel.factory('diSettingsDataFactory', ['$http', '$q', function ($http, $q) {
    return {
        GetSettingsForm: function (section, formName) {
            var apiUrl = "/api/panel/dataisland/getmaindisettings";
            return $http.get(apiUrl);
        },
        SaveSettingsForm: function (form) {
            var apiUrl = "/api/panel/dataisland/savemaindisettings";
            return $http.post(apiUrl, form);
        }
    }
}]);

DiPanel.controller('diSettingsController', ['$scope', 'diSettingsDataFactory', function ($scope, diSettingsDataFactory) {
    $scope.mainSettingsForm = [];

    $scope.GetMainSettingsForm = function () {
        diSettingsDataFactory.GetSettingsForm("settingsforms", "dataislandmainsettings").success(function (data) {
            $scope.mainSettingsForm = data;
        });
    }

    $scope.saveMainSettings = function () {
        diSettingsDataFactory.SaveSettingsForm($scope.mainSettingsForm).success(function (data) {
            if (data.result) {
                toastr.success('[tr]Settings has been saved[/tr]', '[tr]Saving settings[/tr]');
            } else {
                toastr.error('[tr]Error occured. Try again later.[/tr]', '[tr]Saving settings[/tr]');
            }
        }).error(function (data, status, headers, config) {
            toastr.error('[tr]Error occured. Please try again later[/tr]', '[tr]Saving settings[/tr]');
        });
    }

    $scope.GetMainSettingsForm();
}]);