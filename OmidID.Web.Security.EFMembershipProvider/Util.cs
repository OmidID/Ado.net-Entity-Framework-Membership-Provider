using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Globalization;

namespace OmidID.Web.Security {
    internal static class Util {

        public static string GetStringValue(this NameValueCollection config, string Key, string Default) {
            string sValue = config[Key];
            if (sValue == null)
                return Default;

            return sValue;
        }

        public static bool GetBooleanValue(this NameValueCollection config, string Key, bool Default) {
            string sValue = config[Key];
            if (sValue == null)
                return Default;

            bool result;
            if (bool.TryParse(sValue, out result))
                return result;
            else
                throw new ProviderException("Value_must_be_boolean".Resource(Key));

        }

        public static int GetIntValue(this NameValueCollection config, string valueName, int defaultValue, bool zeroAllowed, int maxValueAllowed) {
            string sValue = config[valueName];

            if (sValue == null)
                return defaultValue;

            int iValue;
            if (!Int32.TryParse(sValue, out iValue)) {
                if (zeroAllowed)
                    throw new ProviderException("Value_must_be_non_negative_integer".Resource(valueName));

                throw new ProviderException("Value_must_be_positive_integer".Resource(valueName));
            }

            if (zeroAllowed && iValue < 0)
                throw new ProviderException("Value_must_be_non_negative_integer".Resource(valueName));

            if (!zeroAllowed && iValue <= 0) 
                throw new ProviderException("Value_must_be_positive_integer".Resource(valueName));

            if (maxValueAllowed > 0 && iValue > maxValueAllowed) 
                throw new ProviderException("Value_too_big".Resource(valueName, maxValueAllowed.ToString(CultureInfo.InvariantCulture)));

            return iValue;
        }

    }
}
