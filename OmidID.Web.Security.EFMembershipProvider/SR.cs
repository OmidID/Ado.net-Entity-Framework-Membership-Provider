using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OmidID.Web.Security {
    public static class SR {

        public static string Resource(this string Key) {
            var resource = OmidID.Web.Security.Properties.Resources.ResourceManager.GetString(Key, System.Threading.Thread.CurrentThread.CurrentCulture);
            if (resource == null)
                return Key + "\r\nMore info: http://omid.mafakher.name/Projects/EFMembership/" + Key;
            else
                return resource + "\r\nMore info: http://omid.mafakher.name/Projects/EFMembership/" + Key;
        }

        public static string Resource(this string Key, object arg1) {
            return string.Format(Resource(Key), arg1);
        }

        public static string Resource(this string Key, object arg1, object arg2) {
            return string.Format(Resource(Key), arg1, arg2);
        }

        public static string Resource(this string Key, object arg1, object arg2, object arg3) {
            return string.Format(Resource(Key), arg1, arg2, arg3);
        }

        public static string Resource(this string Key, object[] args) {
            return string.Format(Resource(Key), args);
        }

    }
}
