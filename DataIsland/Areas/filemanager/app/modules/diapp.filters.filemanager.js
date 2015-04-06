var diapp;
(function (diapp) {
    var filters;
    (function (filters) {
        var filemanager;
        (function (filemanager) {
            function propsFilter() {
                return function (items, props) {
                    var out = [];
                    if (angular.isArray(items)) {
                        items.forEach(function (item) {
                            var itemMatches = false;
                            var keys = Object.keys(props);
                            for (var i = 0; i < keys.length; i++) {
                                var prop = keys[i];
                                var text = props[prop].toLowerCase();
                                if (item[prop].toString().toLowerCase().indexOf(text) !== -1) {
                                    itemMatches = true;
                                    break;
                                }
                            }
                            if (itemMatches) {
                                out.push(item);
                            }
                        });
                    }
                    else {
                        // Let the output be the input untouched
                        out = items;
                    }
                    return out;
                };
            }
            filemanager.propsFilter = propsFilter;
            function userDisplayNameFilter() {
                return function (userId, contacts) {
                    for (var i = 0; i < contacts.length; i++) {
                        if (contacts[i].UserId == userId) {
                            if (contacts[i] != null && contacts[i].Name != "") {
                                return contacts[i].Name + " (" + contacts[i].Username + ")";
                            }
                            else {
                                return contacts[i].Username;
                            }
                        }
                    }
                    return "";
                };
            }
            filemanager.userDisplayNameFilter = userDisplayNameFilter;
            function bytesFilter() {
                return function (bytes, precision) {
                    if (bytes == 0)
                        return "-";
                    if (isNaN(parseFloat(bytes)) || !isFinite(bytes))
                        return '-';
                    if (typeof precision === 'undefined')
                        precision = 1;
                    var units = ['bytes', 'kB', 'MB', 'GB', 'TB', 'PB'], number = Math.floor(Math.log(bytes) / Math.log(1024));
                    return (bytes / Math.pow(1024, Math.floor(number))).toFixed(precision) + ' ' + units[number];
                };
            }
            filemanager.bytesFilter = bytesFilter;
            function filterAcceptedContactsFilter() {
                return function (items) {
                    var filtered = [];
                    if (items != undefined && items != null) {
                        for (var i = 0; i < items.length; i++) {
                            var item = items[i];
                            if (!item.RequestToAccept && item.Accepted) {
                                filtered.push(item);
                            }
                        }
                    }
                    return filtered;
                };
            }
            filemanager.filterAcceptedContactsFilter = filterAcceptedContactsFilter;
            angular.module('diapp').filter("propsFilter", diapp.filters.filemanager.propsFilter).filter("bytes", diapp.filters.filemanager.bytesFilter).filter("userDisplayName", diapp.filters.filemanager.userDisplayNameFilter).filter("filterAcceptedContacts", diapp.filters.filemanager.filterAcceptedContactsFilter);
        })(filemanager = filters.filemanager || (filters.filemanager = {}));
    })(filters = diapp.filters || (diapp.filters = {}));
})(diapp || (diapp = {}));
//# sourceMappingURL=diapp.filters.filemanager.js.map