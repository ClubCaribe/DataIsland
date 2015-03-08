/// <reference path="../../../../Scripts/angular.js" />

DiPanel.factory('userAddContactsFactory', ['$http', '$q', function ($http, $q) {
    return {
        FindUsers: function (searchPhrase) {
            var apiUrl = "/api/panel/usercontacts/findexternalusers/"+searchPhrase;
            return $http.get(apiUrl);
        },
        AddContact: function (contactToAdd) {
            var apiUrl = "/api/panel/usercontacts/addcontact";
            return $http.post(apiUrl,contactToAdd);
        }
    }
}]);


DiPanel.controller('userAddContactsController', ['$scope', '$http', 'userAddContactsFactory', function ($scope, $http, userAddContactsFactory) {
    $scope.SearchPhrase = "";
    $scope.ExternalUsers = null;

    $scope.FindUsers = function()
    {
        if($scope.SearchPhrase.length>2)
        {
            userAddContactsFactory.FindUsers($scope.SearchPhrase).success(function (data) {
                $scope.ExternalUsers = data;
            });
        }
    }

    $scope.SearchKeyPressed = function (keyEvent) {
        if (keyEvent.which === 13) {
            $scope.FindUsers();
        }
    }

    $scope.AddContact = function (item) {
        userAddContactsFactory.AddContact(item).success(function (result) {
            if (result) {
                toastr.success('[tr]New contact has been added[/tr]', '[tr]Add Contact[/tr]');
            }
            else {
                toastr.error('[tr]Cannot add this contact[/tr]', '[tr]Add Contact[/tr]');
            }
            $scope.RemoveItemFromExternalUsers(item);
        });
    }

    $scope.RemoveItemFromExternalUsers = function(item){
        for (var i = $scope.ExternalUsers.length - 1; i > -1; i--){
            var itm = $scope.ExternalUsers[i];
            if (itm.Username == item.Username) {
                $scope.ExternalUsers.splice(i, 1);
            }
        }
    }

}]);