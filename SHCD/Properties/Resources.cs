namespace SHCD.Properties
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.Resources;
    using System.Runtime.CompilerServices;

    [CompilerGenerated, DebuggerNonUserCode, GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    internal class Resources
    {
        private static CultureInfo resourceCulture;
        private static System.Resources.ResourceManager resourceMan;

        internal Resources()
        {
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture
        {
            get
            {
                return resourceCulture;
            }
            set
            {
                resourceCulture = value;
            }
        }

        internal static Bitmap doneL
        {
            get
            {
                return (Bitmap) ResourceManager.GetObject("doneL", resourceCulture);
            }
        }

        internal static Bitmap doneM
        {
            get
            {
                return (Bitmap) ResourceManager.GetObject("doneM", resourceCulture);
            }
        }

        internal static Icon Main
        {
            get
            {
                return (Icon) ResourceManager.GetObject("Main", resourceCulture);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    System.Resources.ResourceManager manager = new System.Resources.ResourceManager("SHCD.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = manager;
                }
                return resourceMan;
            }
        }

        internal static Bitmap security
        {
            get
            {
                return (Bitmap) ResourceManager.GetObject("security", resourceCulture);
            }
        }

        internal static Bitmap undoL
        {
            get
            {
                return (Bitmap) ResourceManager.GetObject("undoL", resourceCulture);
            }
        }

        internal static Bitmap undoM
        {
            get
            {
                return (Bitmap) ResourceManager.GetObject("undoM", resourceCulture);
            }
        }

        internal static Bitmap win
        {
            get
            {
                return (Bitmap) ResourceManager.GetObject("win", resourceCulture);
            }
        }
    }
}

