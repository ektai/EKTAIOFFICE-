﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ASC.Web.Community.News.Resources {
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
    public class NewsPatternResource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal NewsPatternResource() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ASC.Web.Community.Modules.News.Resources.NewsPatternResource", typeof(NewsPatternResource).Assembly);
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
        ///   Looks up a localized string similar to h1.#if($FEED_TYPE == &quot;feed&quot;)New Event Added: &quot;$Caption&quot;:&quot;$URL&quot;
        ///
        ///$Date &quot;$UserName&quot;:&quot;$UserURL&quot; has added a new event: &quot;$Caption&quot;:&quot;$URL&quot;
        ///
        ///$Text
        ///
        ///&quot;Read more&quot;:&quot;$URL&quot;
        ///
        ///#end#if($FEED_TYPE == &quot;poll&quot;)New Poll Added: &quot;$Caption&quot;:&quot;$URL&quot;
        ///
        ///$Date &quot;$UserName&quot;:&quot;$UserURL&quot; has added a new poll: &quot;$Caption&quot;:&quot;$URL&quot;
        ///
        ///#foreach($Answer in $Answers)
        ///
        ///#each
        ///
        ///* $Answer
        ///
        ///#end
        ///
        ///&quot;Vote&quot;:&quot;$URL&quot;
        ///
        ///#end.
        /// </summary>
        public static string pattern_new_text {
            get {
                return ResourceManager.GetString("pattern_new_text", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to h1.#if($FEED_TYPE == &quot;poll&quot;)New Comment to Poll#end#if($FEED_TYPE == &quot;feed&quot;)New Comment to Event#end: &quot;$Caption&quot;:&quot;$URL&quot;
        ///
        ///$Date &quot;$UserName&quot;:&quot;$UserURL&quot; has added a new comment to the &quot;$Caption&quot;:&quot;$URL&quot; #if($FEED_TYPE == &quot;poll&quot;)poll#end#if($FEED_TYPE == &quot;feed&quot;)event#end:
        ///
        ///$CommentBody
        ///
        ///&quot;Read More&quot;:&quot;$CommentURL&quot;.
        /// </summary>
        public static string pattern_new_text_comment {
            get {
                return ResourceManager.GetString("pattern_new_text_comment", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Community. #if($FEED_TYPE == &quot;poll&quot;)New poll added#end#if($FEED_TYPE == &quot;feed&quot;)New event added#end: $Caption.
        /// </summary>
        public static string subject_new_text {
            get {
                return ResourceManager.GetString("subject_new_text", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Community. #if($FEED_TYPE == &quot;poll&quot;)New comment to poll#end#if($FEED_TYPE == &quot;feed&quot;)New comment to event#end: $Caption.
        /// </summary>
        public static string subject_new_text_comment {
            get {
                return ResourceManager.GetString("subject_new_text_comment", resourceCulture);
            }
        }
    }
}