namespace Qisi.Editor.Controls
{
    using Qisi.Editor.Expression;
    using System;
    using System.Runtime.CompilerServices;
	/// <summary>
	/// Expression event arguments.
	/// </summary>
    internal class ExpressionEventArgs : EventArgs
    {
        public ExpressionEventArgs(FunctionType ft)
        {
            this.Type = ft;
        }

        public FunctionType Type { get; set; }
    }
}

