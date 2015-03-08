var srUserContacts;

$(function () {

    srUserContacts = $.connection.userContactsHub;
    // Create a function that the hub can call back to display messages.
    srUserContacts.client.AddOrUpdateContact = function (contact) {

        var evt = $.Event('addorupdatecontact');
        evt.Contact = contact;
        $(window).trigger(evt);

    };

});