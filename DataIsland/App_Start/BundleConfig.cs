using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace DataIsland.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/bundles/mainstyles").Include(
                "~/Scripts/jquery-ui/css/no-theme/jquery-ui-1.10.3.custom.min.css",
                "~/Content/panel/css/bootstrap.css",
                "~/Content/panel/css/neon-core.css",
                "~/Content/panel/css/neon-theme.css",
                "~/Content/panel/css/neon-forms.css",
                "~/Scripts/icheck/skins/all.css",
                "~/Content/panel/css/font-icons/entypo/css/entypo.css",
                "~/Content/panel/css/font-icons/entypo/css/animation.css",
                "~/Content/panel/css/font-icons/font-awesome/css/font-awesome.css", 
                "~/Content/css/mainstyle.css"
                ));

            bundles.Add(new ScriptBundle("~/bundles/mainstcipts").Include(
                        "~/Scripts/jquery-2.1.3.min.js",
                        "~/Scripts/json2.js",
                        "~/Scripts/jquery.signalR-2.2.0.js",
                        "~/Scripts/angular.min.js",
                        "~/Scripts/angular-animate.min.js",
                        "~/Scripts/angular-cookies.min.js",
                        "~/Scripts/angular-loader.min.js",
                        "~/Scripts/angular-mocks.js",
                        "~/Scripts/angular-resource.min.js",
                        "~/Scripts/angular-route.min.js",
                        "~/Scripts/angular-sanitize.min.js",
                        "~/Scripts/angular-scenario.js",
                        "~/Scripts/angular-touch.min.js",
                        "~/Areas/panel/app/directives/dirPagination.js",
                        "~/Areas/panel/app/directives/diDynamicForm.js",
                        "~/Scripts/select2/ui-select.js"));

            bundles.Add(new ScriptBundle("~/bundles/diappbase").Include(
                "~/Areas/panel/app/services/diapp.DiProgress.factory.js",
                "~/Areas/panel/app/services/diapp.AuthInterceptor.factory.js",
                "~/Areas/panel/app/controllers/mainHeaderController.js",
                "~/Areas/panel/app/controllers/chatController.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/libraries").Include(
                "~/Scripts/gsap/main-gsap.js",
                "~/Scripts/jquery-ui/js/jquery-ui-1.10.3.minimal.min.js",
                "~/Scripts/bootstrap.js",
                "~/Scripts/joinable.js",
                "~/Scripts/resizeable.js",
                "~/Scripts/neon-api.js",
                "~/Scripts/jquery.validate.min.js",
                "~/Scripts/jquery.validate.unobtrusive.min.js",
                "~/Scripts/jquery.inputmask.bundle.min.js",
                "~/Scripts/toastr.js",
                "~/Scripts/icheck/icheck.min.js",
                "~/Scripts/neon-chat.js",
                "~/Scripts/neon-custom.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/diappcommons").Include(
                "~/Scripts/pages/panel/panel-main.js",
                "~/Scripts/dataisland/diapp.utilities.js",
                "~/Scripts/dataisland/common.js",
                "~/Areas/panel/app/commonmodules/formValidators.js",
                "~/Areas/panel/app/scripts/streamails.js",
                "~/Areas/panel/app/scripts/srcontacts.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/filemanagerscripts").Include(
               "~/Scripts/plupload/plupload.full.min.js",
               "~/Scripts/plupload/jquery.plupload.queue/jquery.plupload.queue.min.js",
               "~/Areas/filemanager/app/directives/diapp.directives.filemanager.imgPreload.js",
               "~/Areas/filemanager/app/directives/diapp.directives.filemanager.plupload.js",
               "~/Areas/filemanager/app/modules/diapp.filters.filemanager.js",
               "~/Areas/filemanager/app/services/diapp.filemanager.services.foreignresources.js",
               "~/Areas/filemanager/app/directives/diapp.directives.filemanager.foreignresourcesbrowser.js",
               "~/Areas/filemanager/app/controllers/fileManagerController.js"
                ));
        }
    }
}