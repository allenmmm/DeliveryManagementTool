using System.Web.Optimization;

namespace DMT.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/datatables").Include(
                        "~/Scripts/datatables.js"));

            bundles.Add(new ScriptBundle("~/bundles/moment").Include(
                        "~/Scripts/moment.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/brand-dark.css",
                        "~/Content/datatables-collins.css",
                        "~/Content/selectric.css"));
        }
    }
}