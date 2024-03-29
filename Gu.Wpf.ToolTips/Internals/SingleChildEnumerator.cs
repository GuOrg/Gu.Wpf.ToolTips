﻿namespace Gu.Wpf.ToolTips
{
    using System.Collections;

    internal sealed class SingleChildEnumerator : IEnumerator
    {
        private readonly object? child;
        private int index = -1;

        internal SingleChildEnumerator(object? child)
        {
            this.child = child;
        }

        object? IEnumerator.Current => this.index == 0 ? this.child : null;

        bool IEnumerator.MoveNext()
        {
            this.index++;
            return this.index < (this.child is null ? 0 : 1);
        }

        void IEnumerator.Reset() => this.index = -1;
    }
}
