﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18051
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Slarsh {
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Slarsh.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Cannot create context if the context factory is not started..
        /// </summary>
        internal static string CannotCreateContextIfFactoryNotStarted {
            get {
                return ResourceManager.GetString("CannotCreateContextIfFactoryNotStarted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You cannot start the context factory when it has been started..
        /// </summary>
        internal static string CannotStartContextFactoryIfStarted {
            get {
                return ResourceManager.GetString("CannotStartContextFactoryIfStarted", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Committing {0}....
        /// </summary>
        internal static string Committing {
            get {
                return ResourceManager.GetString("Committing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The context is not ready. It is either not started or disposed..
        /// </summary>
        internal static string ContextIsNotReady {
            get {
                return ResourceManager.GetString("ContextIsNotReady", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Discarding current IContextFactory..
        /// </summary>
        internal static string DiscardingCurrentContextFactory {
            get {
                return ResourceManager.GetString("DiscardingCurrentContextFactory", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Disposing {0}....
        /// </summary>
        internal static string Disposing {
            get {
                return ResourceManager.GetString("Disposing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error while disposing {0}..
        /// </summary>
        internal static string ErrorWhileDisposing {
            get {
                return ResourceManager.GetString("ErrorWhileDisposing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error while Starting provider factories..
        /// </summary>
        internal static string ErrorWhileStartingProviderFactories {
            get {
                return ResourceManager.GetString("ErrorWhileStartingProviderFactories", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No IContextFactory has been defined. Please call Start() first..
        /// </summary>
        internal static string NoCurrentContextFactory {
            get {
                return ResourceManager.GetString("NoCurrentContextFactory", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No ICurrentContextHolder has been defined..
        /// </summary>
        internal static string NoCurrentContextHolder {
            get {
                return ResourceManager.GetString("NoCurrentContextHolder", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} started in {1} ms..
        /// </summary>
        internal static string StartedIn {
            get {
                return ResourceManager.GetString("StartedIn", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Starting {0}....
        /// </summary>
        internal static string Starting {
            get {
                return ResourceManager.GetString("Starting", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Transaction options cannot be modified when there is a dependant transaction..
        /// </summary>
        internal static string TransactionOptionsCannotBeModified {
            get {
                return ResourceManager.GetString("TransactionOptionsCannotBeModified", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to find a suitable context provider for entity type {0}. Available context providers : {1}..
        /// </summary>
        internal static string UnableToFindASuitableContextProviderFor {
            get {
                return ResourceManager.GetString("UnableToFindASuitableContextProviderFor", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to set a new current context because there is already a current context that is ready. You must first commit / dispose the current context..
        /// </summary>
        internal static string UnableToSetCurrentContextBecauseThereIsAlreadyOne {
            get {
                return ResourceManager.GetString("UnableToSetCurrentContextBecauseThereIsAlreadyOne", resourceCulture);
            }
        }
    }
}
