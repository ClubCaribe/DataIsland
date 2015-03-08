/// <reference path="../../../../Scripts/angular.js" />

DiPanel.factory("chatDataFactory", ['$http', '$q', function ($http, $q) {
    return {
        GetContacts: function () {
            return $http.get("/api/panel/usercontacts/getcontacts");
        },
        SendMessage: function (messageText, userId, username) {
            var data = {
                Receiver: userId,
                Message: messageText,
                Username: username
            }

            return $http.post("/api/panel/chat/sendmessage",data);
        },
        GetNumberOfUnreadMessagesFromAllUsers: function () {
            return $http.get("/api/panel/chat/getnumberofunreadmessagesalluser");
        },
        GetChatMessages: function (userId, pageNum, pageSize) {
            var data = {
                UserId: userId,
                PageNum: pageNum,
                PageSize: pageSize
            }
            return $http.post("/api/panel/chat/getchatmessages", data);
        }
    }
}]);

DiPanel.filter('filterChatByFavourite', function () {
    return function (items,include) {
        var filtered = [];
        if (items != null && items.length > 0) {
            for (var i = 0; i < items.length; i++) {
                var item = items[i];
                if (item.IsFavourite == include) {
                    filtered.push(item);
                }
            }
        }
        return filtered;
    };
});

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

DiPanel.controller('chatController', ['$scope','chatDataFactory', function ($scope,chatDataFactory) {
    var self = this;
    self.ChatList = null;
    self.ActiveChat = null;
    self.NewMessage = {
        Message: "",
        IsDisabled: false
    }
    self.TotalUnreadMessages = 0;
    self.MainChatID = null;

    self.LoadContacts = function () {
        chatDataFactory.GetContacts().then(function (response) {
            for (var i = 0; i < response.data.length; i++) {
                response.data[i].Messages = null;
                response.data[i].NumberOfUnreadMessages = 0;
            }
            self.ChatList = response.data;
            chatDataFactory.GetNumberOfUnreadMessagesFromAllUsers().then(function (response) {
                for (var key in response.data) {
                    self.SetNumberOfUnreadMessages(key, response.data[key]);
                }
                self.UpdateNumberOfTotalUnreadMessages();
            });
        });
    }

    self.SetNumberOfUnreadMessages = function (userId, numOfMessages) {
        for (var i = 0; i < self.ChatList.length; i++) {
            if (self.ChatList[i].UserId == userId) {
                self.ChatList[i].NumberOfUnreadMessages = numOfMessages;
                return;
            }
        }
    }

    self.UpdateNumberOfUnreadMessages = function (userId) {
        var chatObject = self.GetChatObject(userId);
        if (chatObject != null) {
            var numOfUnreadMessages = 0;
            for (var i = 0; i < chatObject.Messages.length; i++) {
                if (!chatObject.Messages[i].IsRead) {
                    numOfUnreadMessages = numOfUnreadMessages + 1;
                }
            }
            chatObject.NumberOfUnreadMessages = numOfUnreadMessages;
        }
    }

    self.UpdateNumberOfTotalUnreadMessages = function () {
        var total = 0;
        for (var i = 0; i < self.ChatList.length; i++) {
            total = total + self.ChatList[i].NumberOfUnreadMessages;
        }
        self.TotalUnreadMessages = total;

        var evt = $.Event('numberofchatunreadedmessagesupdated');
        evt.NumOfMessages = total;
        $(window).trigger(evt);
    }

    self.SetActiveChat = function (item) {
        self.MarkAllMessagesAsRead(item.UserId);
        item.NumberOfUnreadMessages = 0;
        self.UpdateNumberOfTotalUnreadMessages();
        self.ActiveChat = item;
        if (self.ActiveChat.Messages == null)
        {
            chatDataFactory.GetChatMessages(item.UserId, 1, 15).then(function (result) {
                self.ActiveChat.Messages = result.data;
                setTimeout(function () {
                    neonChat.updateScrollbars();
                }, 100);
            });
        }
        
    }

    self.CloseActiveChat = function () {
        self.ActiveChat = null;
    }

    self.MessageTextKeyPressed = function (keyEvent) {
        if (keyEvent.which === 13 && !keyEvent.shiftkey) {
            self.NewMessage.IsDisabled = true;
            chatDataFactory.SendMessage(self.NewMessage.Message, self.ActiveChat.UserId, self.ActiveChat.Username).then(function (result) {
                self.NewMessage.Message = "";
                self.NewMessage.IsDisabled = false;
            });
        }
        if (keyEvent.which === 27) {
            self.ActiveChat = null;
        }
    }

    self.GetMessageSenderName = function (msg) {
        if (msg.SenderID == self.ActiveChat.UserId) {
            return self.ActiveChat.Name;
        } else {
            return "[tr]Me[/tr]";
        }
    }

    self.IsUserMessageOwner = function (msg) {
        if (msg.SenderID == self.ActiveChat.UserId) {
            return false;
        } else {
            return true;
        }
    }

    self.GetChatObject = function (streamailID) {
        if (self.ChatList != null) {
            for (var i = 0; i < self.ChatList.length; i++) {
                if (self.ChatList[i].UserId == streamailID) {
                    return self.ChatList[i];
                }
            }
        }
        return null;
    }

    self.MarkMessageAsRead = function (messageID, streamailID) {
        var chatObject = self.GetChatObject(streamailID);
        if (chatObject != null) {
            if (chatObject.Messages != null) {
                for (var i = 0; i < chatObject.Messages.length; i++) {
                    if (chatObject.Messages[i].ID == messageID) {
                        chatObject.Messages[i].IsRead = true;
                    }
                }
                self.UpdateNumberOfUnreadMessages(streamailID);
                self.UpdateNumberOfTotalUnreadMessages();
            }
        }
    }

    self.MarkAllMessagesAsRead = function (userId) {
        var chatObject = self.GetChatObject(userId);
        if (chatObject != null) {
            if (chatObject.Messages != null) {
                for (var i = 0; i < chatObject.Messages.length; i++) {
                    chatObject.Messages[i].IsRead = false;
                }
                self.UpdateNumberOfUnreadMessages(userId);
            }
        }
    }

    self.AddOrUpdateContact = function(contact)
    {
        if (self.ActiveChat != null && self.ActiveChat.UserId == contact.userId && !contact.Accepted)
        {
            self.CloseActiveChat();
            neonChat.close();
        }
        
        for (var i = 0; i < self.ChatList.length; i++) {
            if (self.ChatList[i].UserId == contact.UserId) {
                self.ChatList[i] = contact;
                self.SetNumberOfUnreadMessages(contact.UserId, 0);
                self.UpdateNumberOfTotalUnreadMessages();
                return;
            }
        }
        self.ChatList.push(contact);
        self.SetNumberOfUnreadMessages(contact.UserId, 0);
        self.UpdateNumberOfTotalUnreadMessages();
        
    }

    self.DeleteContact = function(userId)
    {
        if (self.ActiveChat != null && self.ActiveChat.UserId == userId ) {
            self.CloseActiveChat();
            neonChat.close();
        }

        for (var i = 0; i < self.ChatList.length; i++) {
            if (self.ChatList[i].UserId == userId) {
                self.ChatList.splice(i, 1);
                self.SetNumberOfUnreadMessages(contact.UserId, 0);
                self.UpdateNumberOfTotalUnreadMessages();
                return;
            }
        }
    }

    angular.element(document).ready(function () {
        self.LoadContacts();
    });

    $(window).on('newstreamailmessage', function (e) {
        var msg = e.Message;
        $scope.$apply(function () {
            if (self.MainChatID != null && self.MainChatID == msg.StreamailID) {
                return;
            }
            if (self.ChatList != null) {
                if (self.ActiveChat != null && self.ActiveChat.UserId == msg.StreamailID)
                {
                    if (self.ActiveChat.Messages == null) {
                        self.ActiveChat.Messages = new Array();
                    }
                    self.ActiveChat.Messages.push(msg);
                    msg.IsRead = true;
                    srStreamails.server.markMessageAsRead(msg.ID);
                    setTimeout(function () {
                        neonChat.updateScrollbars();
                    }, 100);
                }
                else
                {
                    for (var i = 0; i < self.ChatList.length; i++) {
                        if (self.ChatList[i].UserId == msg.StreamailID) {
                            if (!msg.IsRead) {
                                self.ChatList[i].NumberOfUnreadMessages = self.ChatList[i].NumberOfUnreadMessages + 1;
                            }
                            if (self.ChatList[i].Messages == null) {
                                self.ChatList[i].Messages = new Array();
                            }
                            self.ChatList[i].Messages.push(msg);
                        }
                    }
                    self.UpdateNumberOfTotalUnreadMessages();
                }
                
            }
        });
    });

    $(window).on('contactfavouritestatechanged', function (e) {
        for (var i = 0; i < self.ChatList.length; i++) {
            if (self.ChatList[i].UserId == e.UserId) {
                self.ChatList[i].IsFavourite = e.IsFavourite;
            }
        }
    });

    $(window).on('markChatMessageAsRead', function (e) {
        self.MarkMessageAsRead(e.MessageID, e.StreamailID);
    });

    $(window).on('contactAccepted', function (e) {
        var contact = e.Contact;
        self.AddOrUpdateContact(contact);
    });

    $(window).on('contactDeleted', function (e) {
        self.DeleteContact(e.UserId);
    });

    $(window).on('addorupdatecontact', function (e) {
        var contact = e.Contact;
        $scope.$apply(function () {
            self.AddOrUpdateContact(contact);
        });
    });

    $(window).on('mainChatUpdated', function (e) {
        var mainchatid = e.MainChatID;
            self.MainChatID = mainchatid;
            self.MarkAllMessagesAsRead(mainchatid);
    });

}]);