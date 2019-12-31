namespace Gu.Wpf.ToolTips
{
    using System;
    using System.Collections;

    internal class EmptyEnumerator : IEnumerator
    {
        internal static readonly IEnumerator Instance = new EmptyEnumerator();

        private EmptyEnumerator()
        {
        }

        object IEnumerator.Current => throw new InvalidOperationException();

        public void Reset()
        {
        }

        public bool MoveNext() => false;
    }
}
