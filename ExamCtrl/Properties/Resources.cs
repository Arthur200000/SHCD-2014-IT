namespace ExamClientControlsLibrary.Properties
{
    using System;
    using System.CodeDom.Compiler;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Globalization;
    using System.Resources;
    using System.Runtime.CompilerServices;

    [DebuggerNonUserCode, GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0"), CompilerGenerated]
    internal class Resources
    {
        private static CultureInfo resourceCulture;
        private static System.Resources.ResourceManager resourceMan;

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
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

        internal static Bitmap done
        {
            get
            {
                return (Bitmap) ResourceManager.GetObject("done", resourceCulture);
            }
        }

        internal static Bitmap logo
        {
            get
            {
                return (Bitmap) ResourceManager.GetObject("logo", resourceCulture);
            }
        }

        internal static Bitmap noPic
        {
            get
            {
                return (Bitmap) ResourceManager.GetObject("noPic", resourceCulture);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals(resourceMan, null))
                {
                    System.Resources.ResourceManager manager = new System.Resources.ResourceManager("ExamClientControlsLibrary.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = manager;
                }
                return resourceMan;
            }
        }

        internal static Bitmap undo
        {
            get
            {
                return (Bitmap) ResourceManager.GetObject("undo", resourceCulture);
            }
        }
    }
}

