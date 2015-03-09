/// <reference path="../../../../Scripts/jquery-2.1.3.intellisense.js" />
/// <reference path="../../../../Scripts/angular.js" />

DiPanel.factory('dirSelectContactsFactory', ['$http', '$q', function ($http, $q) {
    return {
        GetContacts: function () {
            var apiUrl = "/api/panel/usercontacts/getcontacts";
            return $http.get(apiUrl);
        }
    }
}]);

DiPanel.filter('filterSelectContacts', function () {
    return function (items, selectedTab) {
        var filtered = [];
        for (var i = 0; i < items.length; i++) {
            var item = items[i];
            if (!item.RequestToAccept && item.Accepted) {
                filtered.push(item);
            }
        }
        return filtered;
    };
});

DiPanel.directive('dirSelectContacts', ['$compile', 'dirSelectContactsFactory', function ($compile, dirSelectContactsFactory) {
    var _dataFactory = dirSelectContactsFactory;
    return {
        restrict: 'E',
        scope: {
            selectedContacts: '='
        },
        link: function (scope, element, attrs) {
            scope.Contacts = {
                SearchPhrase: "",
                Contacts: new Array()
            }
            
            angular.element(document).ready(function () {
                _dataFactory.GetContacts().then(function (results) {
                    scope.Contacts.Contacts = results.data;
                });
            });

        },
        templateUrl: '/Areas/panel/app/partials/di-select-contacts.html'
    }
}]);