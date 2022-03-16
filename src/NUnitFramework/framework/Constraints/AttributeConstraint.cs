// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt
using System;
using NUnit.Compatibility;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// AttributeConstraint tests that a specified attribute is present
    /// on a Type or other provider and that the value of the attribute
    /// satisfies some other constraint.
    /// </summary>
    public class AttributeConstraint<TExpected> : PrefixConstraint where TExpected : Attribute
    {
        /// <summary>
        /// Constructs an AttributeConstraint for a specified attribute
        /// Type and base constraint.
        /// </summary>
        /// <param name="baseConstraint"></param>
        public AttributeConstraint(IConstraint baseConstraint)
            : base(baseConstraint)
        {
            this.DescriptionPrefix = "attribute " + typeof(TExpected).FullName;
        }

        /// <summary>
        /// Determines whether the Type or other provider has the 
        /// expected attribute and if its value matches the
        /// additional constraint specified.
        /// </summary>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            Guard.ArgumentNotNull(actual, nameof(actual));

            var attrs = AttributeHelper.GetCustomAttributes<TExpected>(actual);
            if (attrs.Length == 0)
                throw new ArgumentException($"Attribute {typeof(TExpected)} was not found", nameof(actual));

            return BaseConstraint.ApplyTo(attrs[0]);
        }

        /// <summary>
        /// Returns a string representation of the constraint.
        /// </summary>
        protected override string GetStringRepresentation()
        {
            return string.Format("<attribute {0} {1}>", typeof(TExpected), BaseConstraint);
        }
    }
}
