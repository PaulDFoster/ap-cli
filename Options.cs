using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using CommandLine;

namespace ap_cli
{
    internal partial class Program
    {
        public class Options
        {
            [Option('j', "appsettings", Required = false, HelpText = "Use the appsettings.json values to configure (true) or use commandline args (false)")]
            public bool appsettings { get; set; } = false;

            [Option('i', "instancename", Required = false, HelpText = "Azure cloud login")]
            public string InstanceName { get; set; } = "https://login.microsoftonline.com/{0}";

            [Option('a', "apiurl", Required = false, HelpText = "Microsoft Graph API URL")]
            public string ApiUrl { get; set; } = "https://graph.microsoft.com/";

            [Option('t', "tenantid", Required = false, HelpText = "Azure tenant id (guid). REQUIRED")]
            public string TenantId { get; set; }

            [Option('c', "clientid", Required = false, HelpText = "Azure application client id REQUIRED")]
            public string ClientId { get; set; }

            [Option('s',"clientsecret", Required = false, HelpText = "Azure application client secret")]
            public string ClientSecret { get; set; }

            [Option('n',"certificatename", Required = false, HelpText = "The name of a certificate (from the user cert store) as registered with your application")]
            public string CetificateName { get; set; }

            [Option('g', "groupid", Required = true, HelpText = "The object id (guid) of the Azure AD Security Group to add to the Access Package")]
            public string GroupId { get; set; }

            [Option('l', "catalogname", Required = true, HelpText = "The Entitlement Management Catalog name to use/create")]
            public string CatalogDisplayName { get; set; }

            [Option('p', "accesspackagename", Required = true, HelpText = "The Entitlement Management Access Package name to use/create")]
            public string AccessPackageName { get; set; }
        }
    }
}
