﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DICOMReceiver {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.13.0.0")]
    internal sealed partial class GeneralSettings : global::System.Configuration.ApplicationSettingsBase {
        
        private static GeneralSettings defaultInstance = ((GeneralSettings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new GeneralSettings())));
        
        public static GeneralSettings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string BaseDirectory {
            get {
                return ((string)(this["BaseDirectory"]));
            }
            set {
                this["BaseDirectory"] = value;
            }
        }
        
        [global::System.Configuration.ApplicationScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("DicomData.db")]
        public string DatabaseName {
            get {
                return ((string)(this["DatabaseName"]));
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("104")]
        public int LocalPort {
            get {
                return ((int)(this["LocalPort"]));
            }
            set {
                this["LocalPort"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("DCMReceiver")]
        public string LocalAETitle {
            get {
                return ((string)(this["LocalAETitle"]));
            }
            set {
                this["LocalAETitle"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("localhost")]
        public string LocalIP {
            get {
                return ((string)(this["LocalIP"]));
            }
            set {
                this["LocalIP"] = value;
            }
        }
    }
}
