﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Maestro.LiveMapEditor {
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
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Maestro.LiveMapEditor.Strings", typeof(Strings).Assembly);
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
        ///   Looks up a localized string similar to New Map.
        /// </summary>
        internal static string CaptionNewMap {
            get {
                return ResourceManager.GetString("CaptionNewMap", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You have an existing open map. Save that map first?.
        /// </summary>
        internal static string ConfirmNewMap {
            get {
                return ResourceManager.GetString("ConfirmNewMap", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Coordinate System Required.
        /// </summary>
        internal static string ErrCoordSysRequired {
            get {
                return ResourceManager.GetString("ErrCoordSysRequired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Extents are invalid or empty.
        /// </summary>
        internal static string ErrInvalidExtents {
            get {
                return ResourceManager.GetString("ErrInvalidExtents", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to New Map.
        /// </summary>
        internal static string NewMap {
            get {
                return ResourceManager.GetString("NewMap", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Because this map has tiled layers, the RenderMap API will be used instead of the normal RenderDynamicOverlay API for map images.
        /// </summary>
        internal static string TiledMapNote {
            get {
                return ResourceManager.GetString("TiledMapNote", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Tiled Map Support.
        /// </summary>
        internal static string TitleTiledMap {
            get {
                return ResourceManager.GetString("TitleTiledMap", resourceCulture);
            }
        }
    }
}
