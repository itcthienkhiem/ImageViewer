﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.269
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClearCanvas.Ris.Client {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
    internal sealed partial class WebServicesSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static WebServicesSettings defaultInstance = ((WebServicesSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new WebServicesSettings())));
        
        public static WebServicesSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        /// <summary>
        /// Specifies the URL on which the application services are hosted.
        /// </summary>
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Configuration.SettingsDescriptionAttribute("Specifies the URL on which the application services are hosted.")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("net.tcp://localhost:8000/")]
        public string ApplicationServicesBaseUrl {
            get {
                return ((string)(this["ApplicationServicesBaseUrl"]));
            }
        }
        
        /// <summary>
        /// Specifies the name of the service configuration class.
        /// </summary>
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Configuration.SettingsDescriptionAttribute("Specifies the name of the service configuration class.")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ClearCanvas.Enterprise.Common.ServiceConfiguration.Client.NetTcpConfiguration, Cl" +
            "earCanvas.Enterprise.Common")]
        public string ConfigurationClass {
            get {
                return ((string)(this["ConfigurationClass"]));
            }
        }
        
        /// <summary>
        /// Specifies the maximum size of received messages in bytes.
        /// </summary>
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Configuration.SettingsDescriptionAttribute("Specifies the maximum size of received messages in bytes.")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("2000000")]
        public int MaxReceivedMessageSize {
            get {
                return ((int)(this["MaxReceivedMessageSize"]));
            }
        }
        
        /// <summary>
        /// Specifies X509 certification validation mode
        /// </summary>
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Configuration.SettingsDescriptionAttribute("Specifies X509 certification validation mode")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("PeerOrChainTrust")]
        public global::System.ServiceModel.Security.X509CertificateValidationMode CertificateValidationMode {
            get {
                return ((global::System.ServiceModel.Security.X509CertificateValidationMode)(this["CertificateValidationMode"]));
            }
        }
        
        /// <summary>
        /// Specifies X509 certificate revocation mode
        /// </summary>
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Configuration.SettingsDescriptionAttribute("Specifies X509 certificate revocation mode")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("NoCheck")]
        public global::System.Security.Cryptography.X509Certificates.X509RevocationMode RevocationMode {
            get {
                return ((global::System.Security.Cryptography.X509Certificates.X509RevocationMode)(this["RevocationMode"]));
            }
        }
        
        /// <summary>
        /// Specify true to use JSML shim service
        /// </summary>
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Configuration.SettingsDescriptionAttribute("Specify true to use JSML shim service")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool UseJsmlShimService {
            get {
                return ((bool)(this["UseJsmlShimService"]));
            }
        }
    }
}
