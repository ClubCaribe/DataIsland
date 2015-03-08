/// <reference path="../../Scripts/jquery-2.1.3.intellisense.js" />

var srStreamails;

$(function () {

    srStreamails = $.connection.streamailCommandsHub;
    // Create a function that the hub can call back to display messages.
    srStreamails.client.NewStreamailMessage = function (message) {
        var evt = $.Event('newstreamailmessage');
        evt.Message = message;
        $(window).trigger(evt);
    };

    srStreamails.client.MessageRead = function (messageId, participantId) {
        var evt = $.Event('streamailmessageread');
        evt.MessageID = messageId;
        evt.ParticipantID = participantId;
        $(window).trigger(evt);
    }

});