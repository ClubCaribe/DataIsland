
function refreshUri(username:string, size:string, type:string, callback) {
    var reload = function () {
        var finishReload = function () {

            // Remove `load` event listener and remove iframe
            this.removeEventListener('load', finishReload, false);

            this.parentElement.removeChild(this);

            // Run the callback if it is provided
            if (typeof callback === 'function') {
                callback();
            }
        }
        // Force a reload of the iframe
        this.contentWindow.location.reload(true);

        // Remove `load` event listener and remove iframe
        this.removeEventListener('load', reload, false);
        this.addEventListener('load', finishReload, false);

    };

    var iframe = document.createElement('iframe');
    iframe.style.display = 'none';

    // Reload iframe once it has loaded
    iframe.addEventListener('load', reload, false);

    // Only call callback if error occured while loading
    iframe.addEventListener('error', callback, false);
    iframe.src = "/utilities/RefreshAvatar/" + username + "/?size=" + size + "&type=" + type;
    document.body.appendChild(iframe);
}

function refreshImage(path, callback) {
    var reload = function () {
        var finishReload = function () {

            // Remove `load` event listener and remove iframe
            this.removeEventListener('load', finishReload, false);

            this.parentElement.removeChild(this);

            // Run the callback if it is provided
            if (typeof callback === 'function') {
                callback();
            }
        }
        // Force a reload of the iframe
        this.contentWindow.location.reload(true);

        // Remove `load` event listener and remove iframe
        this.removeEventListener('load', reload, false);
        this.addEventListener('load', finishReload, false);

    };

    var iframe = document.createElement('iframe');
    iframe.style.display = 'none';

    // Reload iframe once it has loaded
    iframe.addEventListener('load', reload, false);

    // Only call callback if error occured while loading
    iframe.addEventListener('error', callback, false);
    iframe.src = "/utilities/RefreshImage/" + path;
    document.body.appendChild(iframe);
}

function replaceImage(id) {
    return function () {
        var oldImage = <HTMLImageElement>document.getElementById(id);
        if (oldImage != null && oldImage != undefined) {
            var oldSrc = oldImage.src;
            oldImage.src = "";
            oldImage.src = oldSrc;
        }
    }
}

function refreshAvatars() {
    $(".diuseravatar").each(function () {
        var oldUrl = $(this).attr("src");
        var urlElements = oldUrl.split("/");
        var username = urlElements[urlElements.length - 3];
        var size = urlElements[urlElements.length - 2];
        var type = urlElements[urlElements.length - 1];
        var id = $(this).attr("id");
        refreshUri(username, size, type, replaceImage(id));

    });
}

interface String {
    replaceAll(str1: string, str2: string, ignore: boolean): string;
    EscapeUserId(): string;
    UnescapeUserId(): string;
}

String.prototype.replaceAll = function (str1, str2, ignore) {
    return this.replace(new RegExp(str1.replace(/([\/\,\!\\\^\$\{\}\[\]\(\)\.\*\+\?\|\<\>\-\&])/g, "\\$&"),(ignore ? "gi" : "g")),(typeof (str2) == "string") ? str2.replace(/\$/g, "$$$$") : str2);
}

String.prototype.EscapeUserId = function () {
    return this.replaceAll("/", "(2F)").replaceAll("+", "(plus)");
}

String.prototype.UnescapeUserId = function () {
    return this.replaceAll("(2F)", "/").replaceAll("(plus)", "+");
} 
 