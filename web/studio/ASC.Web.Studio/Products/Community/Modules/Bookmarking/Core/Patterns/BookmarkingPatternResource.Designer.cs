﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ASC.Web.Community.Bookmarking.Core.Patterns {
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
    public class BookmarkingPatternResource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal BookmarkingPatternResource() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ASC.Web.Community.Modules.Bookmarking.Core.Patterns.BookmarkingPatternResource", typeof(BookmarkingPatternResource).Assembly);
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to h1.New Comment to Bookmark: &quot;$BookmarkTitle&quot;:&quot;$BookmarkUrl&quot;
        ///
        ///$Date &quot;$__AuthorName&quot;:&quot;$__AuthorUrl&quot; has added a comment to the bookmark:&quot;$BookmarkTitle&quot;:&quot;$BookmarkUrl&quot;
        ///
        ///$CommentBody
        ///
        ///&quot;Read More&quot;:&quot;$BookmarkUrl&quot;.
        /// </summary>
        public static string pattern_BookmarkCommentCreatedID {
            get {
                return ResourceManager.GetString("pattern_BookmarkCommentCreatedID", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to h1.New Bookmark Added: &quot;$BookmarkTitle&quot;:&quot;$BookmarkUrl&quot;
        ///
        ///$Date &quot;$__AuthorName&quot;:&quot;$__AuthorUrl&quot; has added a new bookmark:&quot;$BookmarkTitle&quot;:&quot;$BookmarkUrl&quot;
        ///
        ///$BookmarkDescription.
        /// </summary>
        public static string pattern_BookmarkCreatedID {
            get {
                return ResourceManager.GetString("pattern_BookmarkCreatedID", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Community. New comment to bookmark: $BookmarkTitle.
        /// </summary>
        public static string subject_BookmarkCommentCreatedID {
            get {
                return ResourceManager.GetString("subject_BookmarkCommentCreatedID", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Community. New bookmark added: $BookmarkTitle.
        /// </summary>
        public static string subject_BookmarkCreatedID {
            get {
                return ResourceManager.GetString("subject_BookmarkCreatedID", resourceCulture);
            }
        }
    }
}