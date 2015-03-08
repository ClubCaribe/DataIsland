/// <reference path="../../../../Scripts/angular.js" />

DiPanel.factory('diUsersDataFactory', ['$http', '$q', function ($http, $q) {
    return {
        getUsersCount: function (searchPhrase) {
            var data = {
                searchPhrase: searchPhrase
            }
            var apiUrl = "/api/panel/dataisland/getuserscount";
            return $http.post(apiUrl, data);
        },
        getUsers: function (pageIndex, elementsPerPage, searchPhrase) {
            var data = {
                pageIndex: pageIndex,
                numOfElements: elementsPerPage,
                searchPhrase: searchPhrase
            }
            var apiUrl = "/api/panel/dataisland/getusers";
            return $http.post(apiUrl, data);
        },
        SaveUserSettings: function (postData) {
            var apiUrl = "/api/panel/dataisland/updateusersettings";
            return $http.post(apiUrl, postData);
        },
        CheckUsernameExists: function (Username) {
            var postData = {
                username: Username
            }

            var apiUrl = "/api/panel/dataisland/checkusernameexists";
            return $http.post(apiUrl, postData);
        },
        AddUser: function (Username, Email, Password, Roles) {
            var apiUrl = "/api/panel/dataisland/registeruser";
            var postData = {
                username: Username,
                email: Email,
                password: Password,
                roles: Roles
            }
            return $http.post(apiUrl, postData);
        },
        DeleteUsers: function (users) {
            var apiUrl = "/api/panel/dataisland/deleteusers";
            var postData = {
                usernames:""
            }
            for (var i = 0; i < users.length; i++) {
                postData.usernames = postData.usernames + users[i] + ",";
            }
            return $http.post(apiUrl, postData);
        }

    }
}]);

DiPanel.directive('wsUsernameUnique', ['diUsersDataFactory', function (diUsersDataFactory) {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, ngModel) {
            element.bind("blur", function (e) {
                if (!ngModel || !element.val()) return;
                var currentValue = element.val();
                diUsersDataFactory.CheckUsernameExists(currentValue).success(function (data) {
                    ngModel.$setValidity('usernameunique', data);
                }).error(function (data, status, headers, config) {
                    ngModel.$setValidity('usernameunique', false);
                });
            });
        }
    }
}]);

DiPanel.controller('diUsersController', ['$scope', 'diUsersDataFactory', function ($scope, diUsersDataFactory) {

    $scope.users = [];
    $scope.totalUsers = 0;
    $scope.usersPerPage = 10;
    $scope.searchPhrase = "";

    $scope.pagination = {
        current: 1
    };

    $scope.UserDetails = {
        username: "",
        Roles: {
            userRole: false,
            adminRole: false,
            allRole: false
        },
        email: ""
    }

    $scope.NewUser = {
        Username: "",
        Password: "",
        Email: "",
        Roles: {
            userRole: false,
            adminRole: false,
            allRole: false
        }
    }

    $scope.pageChanged = function (newPage) {
        $scope.getUsersPage(newPage);
    };

    $scope.getUsersPage = function (pageIndex) {
        diUsersDataFactory.getUsers(pageIndex, $scope.usersPerPage, $scope.searchPhrase).success(function (data) {
            $scope.users = data;
        })
    }

    $scope.refreshCurrentPage = function () {
        $scope.getUsersPage($scope.pagination.current);
    }

    $scope.FindUsers = function()
    {
        diUsersDataFactory.getUsersCount($scope.searchPhrase).success(function (data) {
            $scope.totalUsers = data;
            $scope.pagination.current = 1;
            $scope.refreshCurrentPage();
        });
    }

    $scope.getUsersCount = function () {
        diUsersDataFactory.getUsersCount($scope.searchPhrase).success(function (data) {
            $scope.totalUsers = data;
        });
    }

    $scope.init = function () {
        diUsersDataFactory.getUsersCount($scope.searchPhrase).success(function (data) {
            $scope.totalUsers = data;
            $scope.getUsersPage(1);
        });
    }

    $scope.ShowUserDetails = function (user) {
        var details = $scope.UserDetails;
        details.username = user.Username;
        details.Roles.userRole = false;
        details.Roles.adminRole = false;
        details.Roles.allRole = false;
        for (var i = 0; i < user.UserRoles.length; i++) {
            var role = user.UserRoles[i];
            if ((role.Role === "user"))
            {
                details.Roles.userRole = true;
            }
            if ((role.Role === "admin")) {
                details.Roles.adminRole = true;
            }
            if ((role.Role === "all")) {
                details.Roles.allRole = true;
            }
        }
        
        details.password = "";
        details.confirmPassword = "";
    }

    $scope.SaveUserSettings = function () {
        var details = $scope.UserDetails;
        var roles = ((details.Roles.userRole) ? "user," : "") + ((details.Roles.adminRole) ? "admin," : "") + ((details.Roles.allRole) ? "all" : "")

        var postData = {
            username: details.username,
            roles: roles,
            email: details.email
        }

        diUsersDataFactory.SaveUserSettings(postData).success(function (data) {
            toastr.success('[tr]Settings has been updated[/tr]', '[tr]User Settings[/tr]');
            var details = $scope.UserDetails;
            for (var i = 0; i < $scope.users.length; i++) {
                var user = $scope.users[i];
                if (user.Username == details.username) {
                    user.UserRoles = [];
                    
                    if (details.Roles.userRole) {
                        var role = {
                            Role: ""
                        }
                        role.Role = "user";
                        user.UserRoles.push(role)
                    }
                    if (details.Roles.adminRole) {
                        var role = {
                            Role: ""
                        }
                        role.Role = "admin";
                        user.UserRoles.push(role)
                    }
                    if (details.Roles.allRole) {
                        var role = {
                            Role: ""
                        }
                        role.Role = "all";
                        user.UserRoles.push(role)
                    }
                }
            }
        }).error(function (data, status, headers, config) {
            toastr.error('[tr]Error occured. Please try again later[/tr]', '[tr]User Settings[/tr]');
        });
    }

    $scope.InitNewUser = function () {
        $scope.NewUser.Username = "";
        $scope.NewUser.Password = "";
        $scope.NewUser.Email = "";
        $scope.NewUser.Roles.userRole = true;
        $scope.NewUser.Roles.adminRole = false;
        $scope.NewUser.Roles.allRole = false;
    }

    $scope.RegisterUser = function () {
        diUsersDataFactory.CheckUsernameExists($scope.NewUser.Username).success(function (data) {
            if (data == true) {
                var roles = (($scope.NewUser.Roles.userRole) ? "user," : "") + (($scope.NewUser.Roles.adminRole) ? "admin," : "") + (($scope.NewUser.Roles.allRole) ? "all" : "")
                diUsersDataFactory.AddUser($scope.NewUser.Username, $scope.NewUser.Email, $scope.NewUser.Password, roles).success(function (data) {
                    if (data.result == "ok") {
                        toastr.success('[tr]New user has been added[/tr]', '[tr]Register User[/tr]');
                        $scope.getUsersPage($scope.pagination.current);
                        $('#newuserdialog').modal('hide');
                    } else {
                        toastr.error(data.result, '[tr]Register User[/tr]');
                    }
                }).error(function (data, status, headers, config) {
                    toastr.error('[tr]Error occured. Please try again later[/tr]', '[tr]Register User[/tr]');
                });
            }
        })
    }

    $scope.DeleteUsers = function () {
        
        var usernames = [];
        for (var i = 0; i < $scope.users.length; i++) {
            var user = $scope.users[i];
            if (user.Selected) {
                usernames.push(user.Username);
            }
        }

        if (usernames.length > 0 && confirm("[tr]Are you sure you want to delete selected users?[/tr]")) {
            diUsersDataFactory.DeleteUsers(usernames).success(function (data) {
                if (data.result) {
                    toastr.success('[tr]Users has been deleted[/tr]', '[tr]Users Deletion[/tr]');
                } else {
                    toastr.warning('[tr]Some users has not been deleted[/tr]', '[tr]Users Deletion[/tr]');
                }
                $scope.refreshCurrentPage();
            }).error(function (data, status, headers, config) {
                toastr.error('[tr]Error occured. Please try again later[/tr]', '[tr]Users Deletion[/tr]');
            });
        }
    }

    $scope.init();

}]);