/// <reference path="../../../../Scripts/angular.js" />

DiPanel.factory("userChatDataFactory", ['$http', '$q', function ($http, $q) {
    return {
        GetChatMessages: function (userId, pageNum, pageSize) {
            var data = {
                UserId: userId,
                PageNum: pageNum,
                PageSize: pageSize
            }
            return $http.post("/api/panel/chat/getchatmessages", data);
        },
        GetContact: function (userId) {
            var data = {
                userId: userId
            }

            return $http.post("/api/panel/usercontacts/getcontact", data);
        },
        SendMessage: function (messageText, userId, username) {
            var data = {
                Receiver: userId,
                Message: messageText,
                Username: username
            }

            return $http.post("/api/panel/chat/sendmessage", data);
        }
    }
}]);

DiPanel.filter('filterChatExcludeUnaccepted', function () {
    return function (items) {
        var filtered = [];
        if (items != null && items.length > 0) {
            for (var i = 0; i < items.length; i++) {
                var item = items[i];
                if (item.Accepted) {
                    filtered.push(item);
                }
            }
        }
        return filtered;
    };
});

DiPanel.filter('FriendlyDate', function () {
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
            thenDate.toDateString();
        }
    };
});

DiPanel.controller('userChatController', ['$scope', 'userChatDataFactory','$q','$timeout', function ($scope, userChatDataFactory,$q,$timeout) {
    var self = this;
    self.Messages = new Array();
    self.CurrentPageNum = 1;
    self.UserID = "";
    self.ChatUser = null;
    self.AllMessagesLoaded = false;
    self.SearchPhrase = "";

    self.NewMessage = {
        Message: "",
        IsDisabled: false
    }

    self.InitChat = function (userId) {
        self.Messages = new Array();
        self.UserID = userId;
        self.CurrentPageNum = 1;
        self.LoadChat()

        var evt = $.Event('mainChatUpdated');
        evt.MainChatID = userId;
        $(window).trigger(evt);
    }

    self.LoadChat = function () {
        
        userChatDataFactory.GetContact(self.UserID).then(function (result) {
            self.ChatUser = result.data;
            self.LoadMessages().then(function (result) {
                $timeout(function () {
                    scrollToBottom("#conversationArea");
                }, 500);
            });
        });

    }

    self.GetEscapedUseId = function (userId)
    {
        if (userId != null) {
            return userId.replaceAll("/", "(2F)");
        }
        return "";
    }

    self.IsMessageSenderMe = function (senderId) {
        if (senderId == self.UserID) {
            return false;
        }
        return true;
    }

    self.LoadPreviousMessages = function () {
        if (!self.AllMessagesLoaded) {
            var chatArea = $("#chatMessagesArea");
            var prevHeight = $(chatArea).height();
            var numOfMessages = self.Messages.length;
            self.LoadMessages().then(function (result) {
                if (numOfMessages == self.Messages.length) {
                    self.AllMessagesLoaded = true;
                    return;
                }

                setTimeout(function () {
                    var container = $("#chatMessagesArea");
                    var currHeight = $(container).height();
                    var currscrollTop = $("#conversationArea").scrollTop();
                    $("#conversationArea").scrollTop(currscrollTop+(currHeight - prevHeight));
                }, 0);

            });
        }
    }

    self.LoadMessages = function () {
        var deferred = $q.defer();

        $timeout(function () {
            userChatDataFactory.GetChatMessages(self.UserID, self.CurrentPageNum, 15).then(function (results) {
                self.CurrentPageNum = self.CurrentPageNum + 1;
                for (var i = 0; i < results.data.length; i++) {
                    self.AddMessage(results.data[i]);
                }
                deferred.resolve(true);
            });
        }, 0);
        return deferred.promise;
    }

    self.CheckMessageExists = function (messageID) {
        for (var i = 0; i < self.Messages.length; i++) {
            if (self.Messages[i].ID == messageID) {
                return true;
            }
        }
        return false;
    }

    self.AddMessage = function (message) {
        if (!self.CheckMessageExists(message.ID)) {
            self.Messages.push(message);

            setTimeout(function () {

                var evt = $.Event('markChatMessageAsRead');
                evt.MessageID = message.ID;
                evt.StreamailID = message.StreamailID;
                $(window).trigger(evt);

            }, 0);
            
        }
    }

    self.MessageTextKeyPressed = function (keyEvent) {
        if (keyEvent.which === 13 && !keyEvent.shiftKey) {
            if (self.NewMessage.Message != "" && self.NewMessage.Message.length > 0) {
                self.NewMessage.IsDisabled = true;
                var newMessage = self.NewMessage.Message.replace(new RegExp('\r?\n', 'g'), '<br />');
                self.SendMessage(newMessage).then(function (result) {
                    self.NewMessage.IsDisabled = false;
                    self.NewMessage.Message = "";
                });
            }
        }
    }

    self.SendTextMessage = function () {
        if (self.NewMessage.Message != "" && self.NewMessage.Message.length > 0) {
            self.NewMessage.IsDisabled = true;
            var newMessage = self.NewMessage.Message.replace(new RegExp('\r?\n', 'g'), '<br />');
            self.SendMessage(newMessage).then(function (result) {
                self.NewMessage.IsDisabled = false;
                self.NewMessage.Message = "";
            });
        }
    }

    self.SendMessage = function (message) {
        var deferred = $q.defer();

        $timeout(function () {
            userChatDataFactory.SendMessage(message, self.ChatUser.UserId, self.ChatUser.Username).then(function (result) {
                deferred.resolve(true);
            });
        }, 0);

        return deferred.promise;
    }

    $(window).on('newstreamailmessage', function (e) {
        var msg = e.Message;
        $scope.$apply(function () {
            if (msg.StreamailID == self.UserID) {
                self.AddMessage(msg);

                srStreamails.server.markMessageAsRead(msg.ID);

                var chatArea = $("#chatMessagesArea");
                var chatContainerArea = $("#conversationArea");
                
                if(($(chatArea).height() - $(chatContainerArea).height()) + $(chatArea).position().top < 0)
                {
                    setTimeout(function () {
                        scrollToBottom("#conversationArea");
                    }, 600);
                }
            }
        });
    });

    $(window).on('initChat', function (e) {
        var userId = e.UserID;
        $scope.$apply(function () {
            self.InitChat(userId);
        });
    });
    
    $(window).on('chatAreaScrolledToTop', function (e) {
        $scope.$apply(function () {
            if (self.SearchPhrase == "" && self.SearchPhrase.length == 0) {
                self.LoadPreviousMessages();
            }
        });
    });

}]);

$(function () {

    LayoutChatContainer();

    $(window).resize(function () {
        LayoutChatContainer();
    });

    $("#chatUserProfileImage").load(function () {
        LayoutChatContainer();
    });

    $("#conversationArea").scroll(function () {
        var chatArea = $("#chatMessagesArea");
        
        if ($(chatArea).position().top >= 30) {

            var evt = $.Event('chatAreaScrolledToTop');
            $(window).trigger(evt);

        }
       
    });

});


function LayoutChatContainer()
{
    var profileTabs = $("#profileInfoTabs");
    var chatContainer = $("#chatMainContainer");
    var profileTabPoisition = $(profileTabs).position();
    var profileTabHeight = $(profileTabs).outerHeight();
    $(chatContainer).css("top", (profileTabPoisition.top + profileTabHeight + 5) + "px");
}

