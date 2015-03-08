/// <reference path="../../../../Scripts/angular.js" />

DiPanel.directive('fileModel', ['$parse', function ($parse) {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            var model = $parse(attrs.fileModel);
            var modelSetter = model.assign;

            element.bind('change', function () {
                scope.$apply(function () {
                    modelSetter(scope, element[0].files[0]);
                });
            });
        }
    };
}]);

DiPanel.factory('userSettingsDataFactory', ['$http', '$q', function ($http, $q) {
    return {
        changeAvatar: function (file) {
            var fd = new FormData();
            fd.append('file', file);
            var uploadUrl = "/api/panel/user/uploadavatar";
            return $http.post(uploadUrl, fd, {
                transformRequest: angular.identity,
                headers: { 'Content-Type': undefined }
            });
        },
        updatePassword: function (newpassword) {
            var url = "/api/panel/user/updatepassword";

            var postData = {
                newPassword: newpassword
            };

            return $http.post(url, postData); 
        },
        getUserSettings: function () {
            return $http.get("/api/panel/user/getusersettings");
        },
        saveUserSettings: function (settings) {
            return $http.post("/api/panel/user/saveusersettings", settings);
        }
    }
}]);

DiPanel.controller('userSettingsController', ['$scope', '$http', 'userSettingsDataFactory', function ($scope, $http, userSettingsDataFactory) {

    $scope.UserDetails = {
        Username: "",
        Name: "",
        ProAccountExpirationDate: new Date(),
        Password: "",
        ConfirmPassword: "",
        Email: "",
        LastLoginTime: new Date(),
        Avatar: null
    };

    $scope.UserSettings = null;

    $scope.VisibleTab = "UserDetails";

    $scope.getUserDetails = function () {
        $http.get("/api/panel/user/getuserdetails").success(function (data) {
            $scope.UserDetails.Username = data.Username;
            $scope.UserDetails.Name = data.Name;
            $scope.UserDetails.ProAccountExpirationDate = data.ProAccountExpirationTime;
            $scope.UserDetails.Email = data.Email;
            $scope.UserDetails.LastLoginTime = data.LastLoginTime;
        });
    }

    $scope.getUserSettings = function () {
        userSettingsDataFactory.getUserSettings().success(function (data) {
            $scope.UserSettings = data;
            
        });
    }

    $scope.saveUserSettings = function () {

        $scope.UserSettings.Name = {
            Value: $scope.UserDetails.Name,
            Name: "Name"
        }
        $scope.UserSettings.Email = {
            Value: $scope.UserDetails.Email,
            Name: "Email"
        }
        userSettingsDataFactory.saveUserSettings($scope.UserSettings).success(function (data) {
            if (data == true) {
                toastr.success('[tr]User settings has been saved[/tr]', '[tr]User settings[/tr]');
            } else {
                toastr.error('[tr]Error occured. Please try again later[/tr]', '[tr]User settings[/tr]');
            }
        }).error(function (data, status, headers, config) {
        });
    }

    $scope.changeAvatar = function () {
        if ($scope.UserDetails.Avatar != null) {
            userSettingsDataFactory.changeAvatar($scope.UserDetails.Avatar).success(function (response) {
                if (response == true) {
                    toastr.success('[tr]Avatar has been changed[/tr]', '[tr]Avatar Change[/tr]');
                    refreshAvatars();
                }
                else {
                    toastr.error('[tr]Error occured. Please try again later[/tr]', '[tr]Avatar Change[/tr]');
                }

            }).error(function (data, status, headers, config) {
                toastr.error('[tr]Error occured. Please try again later[/tr]', '[tr]Avatar Change[/tr]');
            });

        }
    }

    $scope.changePassword = function () {
        if ($scope.UserDetails.Password != "" && $scope.UserDetails.Password == $scope.UserDetails.ConfirmPassword) {
            userSettingsDataFactory.updatePassword($scope.UserDetails.Password).then(function (response) {
                if (response.data == true) {
                    toastr.success('[tr]Password has been updated[/tr]', '[tr]Password Change[/tr]');
                } else {
                    toastr.error('[tr]Error occured. Please try again later[/tr]', '[tr]Password Change[/tr]');
                }
            });
        }
    }

    $scope.changeTab = function (tabName) {
        $scope.VisibleTab = tabName;
    }



    $scope.getUserDetails();
    $scope.getUserSettings();
}]);

