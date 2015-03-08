(function () {
    function initNgModules(element) {
        var elements = [element],
            moduleElements = [],
            modules = [],
            names = ["ng:dimodule", "ng-dimodule", "x-ng-dimodule", "data-ng-dimodule", "ng:dimodules", "ng-dimodules", "x-ng-dimodules", "data-ng-dimodules"],
            NG_MODULE_CLASS_REGEXP = /\sng[:\-]module[s](:\s*([\w\d_]+);?)?\s/;

        function append(element) {
            if (element != null) {
                element && elements.push(element);
            }
        }

        for (var i = 0; i < names.length; i++) {
            var name = names[i];
            //names[i] = true;
            append(document.getElementById(name));
            name = name.replace(':', '\\:');
            if (element.querySelectorAll) {
                var elements2;
                elements2 = element.querySelectorAll('.' + name);
                for (var j = 0; j < elements2.length; j++) append(elements2[j]);

                elements2 = element.querySelectorAll('.' + name + '\\:');
                for (var j = 0; j < elements2.length; j++) append(elements2[j]);

                elements2 = element.querySelectorAll('[' + name + ']');
                for (var j = 0; j < elements2.length; j++) append(elements2[j]);
            }
        }

        for (var i = 0; i < elements.length; i++) {
            var element = elements[i];

            var className = ' ' + element.className + ' ';
            var match = NG_MODULE_CLASS_REGEXP.exec(className);
            if (match) {
                moduleElements.push(element);
                modules.push((match[2] || '').replace(/\s+/g, ','));
            } else {
                if (element.attributes) {
                    for (var m = 0, attrs = element.attributes, l = element.attributes.length; m < l; m++) {
                        var attrName = attrs.item(m).nodeName;
                        if (attrName == "length") continue;
                        var attr = { name: attrName, value: element.attributes[attrName].nodeValue };

                        if (names.indexOf(attr.name)>-1) {
                            moduleElements.push(element);
                            modules.push(attr.value);
                        }
                    }
                }
            }
        }

        for (var i = 0; i < moduleElements.length; i++) {
            var moduleElement = moduleElements[i];
            var module = modules[i].replace(/ /g, '').split(",");
            angular.bootstrap(moduleElement, module);
        }
    }

    angular.element(document).ready(function () {
        initNgModules(document);
    });
})();