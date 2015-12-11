using System.Web.Optimization;

namespace Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new ScriptBundle("~/scripts/all").Include(
                   "~/js/plugins/jQuery/jQuery-2.1.4.min.js",
                   "~/css/bootstrap/js/bootstrap.js",
                   "~/js/plugins/fastclick/fastclick.js",
                   "~/js/app.js",
                   "~/js/plugins/slimScroll/jquery.slimscroll.js"));


            bundles.Add(new StyleBundle("~/css/all").Include(
                      "~/css/bootstrap/css/bootstrap.css",
                      "~/css/font-awesome.css",
                      "~/css/ionicons.css",
                      "~/css/AdminLTE.css",
                      "~/css/skins/skin-green.css", 
                      "~/css/site.css"));


            bundles.Add(new ScriptBundle("~/scripts/angular")
             .Include("~/scripts/angular.js",
                      "~/scripts/angular-ui/ui-bootstrap-tpls.js",
                      "~/scripts/codemwnci/Markdown.Converter.js",
                      "~/scripts/codemwnci/Markdown.Sanitizer.js",
                      "~/scripts/codemwnci/bootstrap-markdown.js",
                      "~/scripts/codemwnci/markdown-editpreview-ng.js"));


            bundles.Add(new ScriptBundle("~/scripts/taxtemplate")
                   .IncludeDirectory("~/js/apps/taxtemplate/", "*.js", true));

            bundles.Add(new ScriptBundle("~/scripts/taxdatamanager")
                   .Include("~/scripts/ng-file-input.js")
                   .IncludeDirectory("~/js/apps/taxdatamanager/", "*.js", true));

            bundles.Add(new ScriptBundle("~/scripts/taxdataupload")
                   .Include("~/scripts/ng-file-input.js")
                   .IncludeDirectory("~/js/apps/taxdataupload/", "*.js", true));

            bundles.Add(new StyleBundle("~/css/all.css")
                   .Include("~/content/font-awesome.css")
                   .Include("~/content/bootstrap.css")
                   .Include("~/content/sb-admin.css")
                   .Include("~/content/layout.css"));

            bundles.Add(new StyleBundle("~/css/splcenter")
                   .Include("~/content/bootstrap.css")
                   .Include("~/content/font-awesome.css")
                   .Include("~/content/splcenter-base.css")
                   .Include("~/content/splcenter.css"));

            bundles.Add(new ScriptBundle("~/scripts/splcenter")
                   .Include("~/scripts/jquery-{version}.js")
                   .Include("~/scripts/bootstrap.js")
                   .Include("~/scripts/splc.js"));


            bundles.Add(new ScriptBundle("~/scripts/all.js")
                .Include("~/scripts/jquery-{version}.js")
                .Include("~/scripts/bootstrap.js"));





            //                .Include("~/scripts/bootstrap-toggle.js")
            //                .Include("~/scripts/toastr.js")
            //                .Include("~/scripts/moment.js")
            //                .Include("~/scripts/lodash.js")
            //                .Include("~/scripts/respond.js")
            //                .Include("~/scripts/angular.js")
            //                .Include("~/scripts/angular-local-storage.js")
            //                .Include("~/scripts/ng-tags-input.js")
            //                .Include("~/scripts/angular-ui/ui-bootstrap-tpls.js")
            //                .Include("~/app/app.js")
            //                .IncludeDirectory("~/app/", "*.js", true)
            //                );



            //            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            //                        "~/Scripts/jquery-{version}.js"));
            //
            //            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //                        "~/Scripts/jquery.validate*"));
            //
            //            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            //            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            //            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //                        "~/Scripts/modernizr-*"));
            //
            //            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
            //                      "~/Scripts/bootstrap.js",
            //                      "~/Scripts/respond.js"));
            //
            //            bundles.Add(new StyleBundle("~/Content/css").Include(
            //                      "~/Content/bootstrap.css",
            //                      "~/Content/site.css"));
        }
    }
}
