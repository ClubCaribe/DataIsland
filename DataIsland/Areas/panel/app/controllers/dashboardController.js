/// <reference path="../../../../Scripts/angular.js" />

DiPanel.factory('dashboardDataFactory', ['$http', '$q', function ($http, $q) {
    return {
        GetNotifications: function (pageNum) {
            var apiUrl = "/api/panel/dashboard/getnotifications/"+pageNum;
            return $http.get(apiUrl);
        }
    }
}]);

DiPanel.filter('notificationFriendlyDate', function () {
    return function (then) {
        var now = new Date();
        var thenDate = new Date(then);
        var days = now.getUTCDate() - thenDate.getUTCDate();
        if (days == 0) {
            return "[tr]today[/tr]";
        } else if (days == 1) {
            return "[tr]yesterday[/tr]";
        } else if (days > 1 && days < 8) {
            return days + " [tr]days ago[/tr]";
        } else {
           return thenDate.toDateString();
        }
    };
});

DiPanel.filter('html', ['$sce', function ($sce) {
    return function (text) {
        return $sce.trustAsHtml(text);
    };
}])

DiPanel.controller('dashboardController', ['$scope', 'dashboardDataFactory', function ($scope, dashboardDataFactory) {
    var self = this;
    self.Notifications = {
        List: new Array(),
        PageNum: 1
    };

    self.GetNotifications = function (pageNum) {
        dashboardDataFactory.GetNotifications(pageNum).then(function (result) {
            for (var i = 0; i < result.data.length; i++) {
                self.AddNotificationIfDoesntExists(result.data[i]);
            }
        });
    }

    self.AddNotificationIfDoesntExists = function (notification) {
        if (self.Notifications.length > 0) {
            for (var i = 0; i < self.Notifications.List.length; i++) {
                if (self.Notifications.List[i].ID == notification.ID) {
                    return;
                }
            }
        }
        self.Notifications.List.push(notification);
    }

    $(window).on('newstreamailmessage', function (e) {
        var msg = e.Message;
        $scope.$apply(function () {
            if(msg.StreamailID == "notificator")
            {
                self.AddNotificationIfDoesntExists(msg);
            }
        });
    });


    angular.element(document).ready(function () {
        self.GetNotifications(self.Notifications.PageNum);
    });

}]);