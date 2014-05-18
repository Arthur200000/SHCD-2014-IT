namespace Qisi.Editor.Controls
{
    using Qisi.Editor.Expression;
    using System;
    using System.Runtime.CompilerServices;

    internal class ExpressionEventArgs : EventArgs
    {
        public ExpressionEventArgs(FType ft)
        {
            this.Type = ft;
        }

        public FType Type { get; set; }
    }
}

