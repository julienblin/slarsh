﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18051
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Slarsh.NHibernate {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Slarsh.NHibernate.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Building {0}....
        /// </summary>
        internal static string Building {
            get {
                return ResourceManager.GetString("Building", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} built in {1} ms..
        /// </summary>
        internal static string BuiltIn {
            get {
                return ResourceManager.GetString("BuiltIn", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Can only create NHEntities. Actual type: {0}..
        /// </summary>
        internal static string CanOnlyCreateNHEntities {
            get {
                return ResourceManager.GetString("CanOnlyCreateNHEntities", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error while creating type {0}. It&apos;s probably because it does not have a public constructor with a single IContext argument..
        /// </summary>
        internal static string ErrorWhileCreatingNHEntity {
            get {
                return ResourceManager.GetString("ErrorWhileCreatingNHEntity", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to NHibernate configuration.
        /// </summary>
        internal static string NHConfiguration {
            get {
                return ResourceManager.GetString("NHConfiguration", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Session {0} opened..
        /// </summary>
        internal static string SessionOpened {
            get {
                return ResourceManager.GetString("SessionOpened", resourceCulture);
            }
        }
    }
}
