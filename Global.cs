using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using SBMS.Utilities;

/// <summary>
/// Summary description for Global
/// </summary>
public static class Global
{
    /// <summary>
    /// Static Class used for global functions
    /// </summary>
    public static class Customisation
    {
        /// <summary>
        /// Property to Get 'Connectionstring' from Web.Config
        /// </summary>
        public static string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings[SBMSConstants.ConnectionStringName].ToString(); }
        }

    }
}
