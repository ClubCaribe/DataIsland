/// <reference path="../../../../Scripts/angular.js" />

DiPanel.factory("sidebarDataFactory", ['$http', '$q', function ($http, $q) {
    return {
        getMenus: function () {
            return $http.get("/api/panel/user/getmenu");
        }
    }
}]);

DiPanel.controller('mainHeaderController', ['$scope', '$http', function ($scope, $http) {

    $scope.NumberOfChatUnreadedMessages = 0;

	var getuserdetails = function () {
		$http.get("/api/panel/user/getuserdetails").success(function (data) {
			$scope.Username = data.Username;
		});
	}


	$scope.logout = function () {
		$http.get("/login/logout").success(function (data) {
		    window.location.href = "/login?ReturnUrl=" + window.location.href;
		});
	}

	$scope.getUserDetails = getuserdetails;

	getuserdetails();
	
	$(window).on('numberofchatunreadedmessagesupdated', function (e) {
	    var numberOfMessages = e.NumOfMessages;
	    $scope.NumberOfChatUnreadedMessages = numberOfMessages;
	});
	
}]);

DiPanel.controller('sidebarcontroller', ['$scope', '$location', "$timeout", 'sidebarDataFactory', function ($scope, $location, $timeout, sidebarDataFactory) {
    $scope.Menus = null;

    $scope.getMenus = function () {
        sidebarDataFactory.getMenus().success(function (data) {
            $scope.Menus = data;
            $timeout(function () {
                $("a").each(function () {
                    if ($(this).attr("href") == window.location.pathname) {
                        $(this).parent().addClass("active");
                        if ($(this).parent().parent().parent().prop("tagName").toLowerCase() == "li") {
                            $(this).parent().parent().parent().addClass("opened").addClass("active");
                            $(this).parent().parent().addClass("visible");
                        }
                    }
                });
            }, 500);
        });
    }

    $scope.getMenus();

}])