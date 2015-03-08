/// <reference path="../../../../Scripts/angular.js" />

DiPanel.filter('filterByContactTab', function () {
    return function (items, selectedTab) {
        var filtered = [];
        if (selectedTab == "all") {
            return items;
        }
        for (var i = 0; i < items.length; i++) {
            var item = items[i];
            if (selectedTab == "favourites" && item.IsFavourite) {
                filtered.push(item);
            }
            else if (selectedTab == "requests" && item.RequestToAccept) {
                filtered.push(item);
            }
            else if (selectedTab == "notaccepted" && !item.RequestToAccept && !item.Accepted) {
                filtered.push(item);
            }
        }
        return filtered;
    };
});

DiPanel.factory('userContactsFactory', ['$http', '$q', function ($http, $q) {
    return {
        GetContacts: function () {
            var apiUrl = "/api/panel/usercontacts/getcontacts";
            return $http.get(apiUrl);
        },
        SetFavourite: function (userId, isFavourite) {
            var apiUrl = "/api/panel/usercontacts/setfavourite";
            var data = {
                UserId: userId,
                IsFavourite: isFavourite
            };
            return $http.post(apiUrl, data);
        },
        AcceptRequest: function (userId) {
            var apiUrl = "/api/panel/usercontacts/acceptrequest";
            var data = {
                UserId: userId
            };
            return $http.post(apiUrl, data);
        },
        DeleteContact: function (userId) {
            var apiUrl = "/api/panel/usercontacts/deletecontact";
            var data = {
                UserId: userId
            };
            return $http.post(apiUrl, data);
        },
        ResendContactRequest: function (userId) {
            var apiUrl = "/api/panel/usercontacts/resendrequest";
            var data = {
                UserId: userId
            };
            return $http.post(apiUrl, data);
        }
    }
}]);

DiPanel.controller('userContactsController', ['$scope', '$http', 'userContactsFactory', function ($scope, $http, userContactsFactory) {
    $scope.SelectedAction = "contacts";
    $scope.Contacts = {
        SearchPhrase: "",
        Contacts: new Array()
    };
    $scope.SelectedTab = "all";

    $scope.NumOfApprovals = function()
    {
        var count = 0;
        for (var i = 0; i < $scope.Contacts.Contacts.length; i++) {
            var itm = $scope.Contacts.Contacts[i];
            if (itm.RequestToAccept) {
                count++;
            }
        }
        return count;
    }

    $scope.ChangeAction = function (action) {
        $scope.SelectedAction = action; 
    }

    $scope.ChangeTab = function (tabName) {
        $scope.SelectedTab = tabName;
    }

    $scope.RefreshContacts = function () {
        userContactsFactory.GetContacts().success(function (data) {
            $scope.Contacts.Contacts = data;
        });
    }

    $scope.AddOrUpdateContact = function (item) {
        for (var i = 0; i < $scope.Contacts.Contacts.length; i++) {
            var it = $scope.Contacts.Contacts[i];
            if (it.UserId == item.UserId) {
                $scope.Contacts.Contacts[i] = item;
                return;
            }
        }
        $scope.Contacts.Contacts.push(item);
    }

    $scope.SetFavourite = function (item) {
        item.IsFavourite = !item.IsFavourite;
        userContactsFactory.SetFavourite(item.UserId, item.IsFavourite);

        var evt = $.Event('contactfavouritestatechanged');
        evt.UserId = item.UserId;
        evt.IsFavourite = item.IsFavourite;
        $(window).trigger(evt);
    }

    $scope.AcceptRequest = function (item) {
        if (confirm("[tr]Do You want to accept this contact request?[/tr]")) {
            item.Accepted = true;
            item.RequestToAccept = false;
            userContactsFactory.AcceptRequest(item.UserId);

            var evt = $.Event('contactAccepted');
            evt.Contact = item;
            $(window).trigger(evt);
        }
    }

    $scope.ResendContactRequest = function (item) {
        userContactsFactory.ResendContactRequest(item.UserId).success(function (result) {
            if (result) {
                toastr.success('[tr]Contact request has been sent[/tr]', '[tr]Contact request[/tr]');
            }
            else {
                toastr.error('[tr]Error during sending contact request. Try again later.[/tr]', '[tr]Contact request[/tr]');
            }
        });
    }

    $scope.DeleteContact = function (item) {
        if (confirm("[tr]Do You really want to delete this contact?[/tr]")) {
            for (var i = $scope.Contacts.Contacts.length - 1; i > -1; i--) {
                var it = $scope.Contacts.Contacts[i];
                if (it.UserId == item.UserId) {

                    $scope.Contacts.Contacts.splice(i, 1);
                    userContactsFactory.DeleteContact(item.UserId);

                    var evt = $.Event('contactDeleted');
                    evt.UserId = item.UserId;
                    $(window).trigger(evt);
                }
            }
        }
    }

    $scope.RefreshContacts();

    $(window).on('addorupdatecontact', function (e) {
        var contact = e.Contact;
        $scope.$apply(function () {
            $scope.AddOrUpdateContact(contact);
        });
    });

}]);



