// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt
using System;
using NUnit.Compatibility;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// AttributeExistsConstraint tests for the presence of a
    /// specified attribute on a Type.
    /// </summary>
    public class AttributeExistsConstraint<TExpected> : Constraint where TExpected : Attribute
    {
        /// <summary>
        /// Constructs an AttributeExistsConstraint for a specific attribute Type
        /// </summary>
        public AttributeExistsConstraint() : base(typeof(TExpected)) { }

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description
        {
            get { return "type with attribute " + MsgUtils.FormatValue(typeof(TExpected)); }
        }

        /// <summary>
        /// Tests whether the object provides the expected attribute.
        /// </summary>
        /// <param name="actual">A Type, MethodInfo, or other ICustomAttributeProvider</param>
        /// <returns>True if the expected attribute is present, otherwise false</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            Guard.ArgumentNotNull(actual, nameof(actual));

            var attrs = AttributeHelper.GetCustomAttributes<TExpected>(actual);

            return new ConstraintResult(this, actual, attrs.Length > 0);
        }
    }

    /// <summary>
    /// AttributeExistsConstraint tests for the presence of a
    /// specified attribute on a Type.
    /// </summary>
    public class AttributeExistsConstraint : Constraint
    {
        private readonly Constraint _inner;

        /// <summary>
        /// Constructs an AttributeExistsConstraint for a specific attribute Type
        /// </summary>
        /// <param name="type"></param>
        public AttributeExistsConstraint(Type type)
        {
            _inner = AttributeHelper.MakeGenericForAttribute<Constraint>(type, typeof(AttributeExistsConstraint));
        }

        /// <summary>
        /// The Description of what this constraint tests, for
        /// use in messages and in the ConstraintResult.
        /// </summary>
        public override string Description => _inner.Description;

        /// <summary>
        /// Tests whether the object provides the expected attribute.
        /// </summary>
        /// <param name="actual">A Type, MethodInfo, or other ICustomAttributeProvider</param>
        /// <returns>True if the expected attribute is present, otherwise false</returns>
        public override ConstraintResult ApplyTo<TActual>(TActual actual)
            => _inner.ApplyTo(actual);
    }
}
