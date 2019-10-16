using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Core.Logging
{
    public class LoggingEvents
    {
        #region Web Layer issues
        public const int Controller = 1000;
        public const int Authentication = 1100;
        #endregion

        #region Business Layer issues
        public const int Business = 2000;
        #endregion

        #region Infrastructure Layer issues
        public const int Database = 3000;

        public const int ApiClient = 3100;

        #endregion

        #region Uncategorized issues
        public const int Uncategorized = 0;
        #endregion

    }
}
