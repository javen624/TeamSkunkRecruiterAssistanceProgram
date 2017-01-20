using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamSkunk.Services
{
    public static class Constants
    {
        public const string DEFAULT_ERROR_MESSAGE = "An error has occured.  Please contact Technical Support.";

        /// <summary>Regular Expression to check for valid emails.  Dervied from RFC5322 (but omiting more stuff).</summary>
		public const string EmailRegex = @"^[a-z0-9!#$%&+/=?^_'{|}~-]+(?:\.[a-z0-9!#$%&+/=?^_'{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$";
        public const string PASSWORD_REGEX = @"^((?=(.*[\d]){2,})(?=(.*[a-z]){2,})(?=(.*[A-Z]){2,})(?=(.*[^\w\d\s]){2,})).{15,}$";

        /// <summary>Regex for alphanumeric and punctuation.</summary>
		public const string REGEX_ALPHANUMERIC_PUNC = "^[-a-zA-Z0-9,.';:\"_)(\\[\\]!@?#$%&/\\s]+$";

        #region Files
        public const int MAXFILESIZEBYTES = 104857600;

        public const string UNKNOWN_FILE = "Unknown";

        #endregion
    }
}
