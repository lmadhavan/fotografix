using NUnit.Framework.Constraints;
using System;

namespace Fotografix.Core.Tests
{
    /// <summary>
    /// Provides a custom NUnit constraint for asserting that a ContentChanged event was raised.
    /// </summary>
    public static class Raises
    {
        public static ContentChangedConstraintBuilder ContentChanged => new ContentChangedConstraintBuilder();

        public sealed class ContentChangedConstraintBuilder
        {
            public ContentChangedConstraint When(Action action)
            {
                return new ContentChangedConstraint(action);
            }
        }

        public sealed class ContentChangedConstraint : Constraint
        {
            private readonly Action action;

            public ContentChangedConstraint(Action action)
            {
                this.action = action;
                this.Description = "a ContentChanged event";
            }

            public override ConstraintResult ApplyTo<TActual>(TActual actual)
            {
                ContentChangedEventArgs contentChangedEvent = null;

                if (actual is INotifyContentChanged notifyContentChanged)
                {
                    notifyContentChanged.ContentChanged += (s, e) => contentChangedEvent = e;
                    action();
                }

                return new ConstraintResult(this, contentChangedEvent, contentChangedEvent != null);
            }
        }
    }
}
