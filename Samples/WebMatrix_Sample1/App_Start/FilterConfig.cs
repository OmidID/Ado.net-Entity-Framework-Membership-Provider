using System.Web;
using System.Web.Mvc;

namespace WebMatrix_Sample1 {
    public class FilterConfig {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
            filters.Add(new HandleErrorAttribute());
        }
    }
}