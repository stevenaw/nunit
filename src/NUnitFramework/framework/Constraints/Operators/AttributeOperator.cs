// Copyright (c) Charlie Poole, Rob Prouse and Contributors. MIT License - see LICENSE.txt
using System;
using NUnit.Compatibility;

namespace NUnit.Framework.Constraints
{
    /// <summary>
    /// Operator that tests for the presence of a particular attribute
    /// on a type and optionally applies further tests to the attribute.
    /// </summary>
    public class AttributeOperator<TExpected> : SelfResolvingOperator where TExpected : Attribute
    {
        /// <summary>
        /// Construct an AttributeOperator for a particular Type
        /// </summary>
        public AttributeOperator()
        {
            // Attribute stacks on anything and allows only 
            // prefix operators to stack on it.
            this.left_precedence = this.right_precedence = 1;
        }

        /// <summary>
        /// Reduce produces a constraint from the operator and 
        /// any arguments. It takes the arguments from the constraint 
        /// stack and pushes the resulting constraint on it.
        /// </summary>
        public override void Reduce(ConstraintBuilder.ConstraintStack stack)
        {
            if (RightContext == null || RightContext is BinaryOperator)
                stack.Push(new AttributeExistsConstraint<TExpected>());
            else
                stack.Push(new AttributeConstraint<TExpected>(stack.Pop()));
        }
    }

    /// <summary>
    /// Operator that tests for the presence of a particular attribute
    /// on a type and optionally applies further tests to the attribute.
    /// </summary>
    public class AttributeOperator : SelfResolvingOperator
    {
        private readonly Type type;

        /// <summary>
        /// Construct an AttributeOperator for a particular Type
        /// </summary>
        /// <param name="type">The Type of attribute tested</param>
        public AttributeOperator(Type type)
        {
            this.type = type;

            // Attribute stacks on anything and allows only 
            // prefix operators to stack on it.
            this.left_precedence = this.right_precedence = 1;
        }

        /// <summary>
        /// Reduce produces a constraint from the operator and 
        /// any arguments. It takes the arguments from the constraint 
        /// stack and pushes the resulting constraint on it.
        /// </summary>
        public override void Reduce(ConstraintBuilder.ConstraintStack stack)
        {
            if (RightContext == null || RightContext is BinaryOperator)
                stack.Push(CreateGenericOperator(typeof(AttributeExistsConstraint<>), type));
            else
                stack.Push(CreateGenericOperator(typeof(AttributeConstraint<>), type, stack.Pop()));
        }

        private static IConstraint CreateGenericOperator(Type openGenericType, Type typeArgs, params object[] ctorArgs)
        {
            var closedGeneric = openGenericType.MakeGenericType(typeArgs);
            return Activator.CreateInstance(closedGeneric, ctorArgs) as IConstraint;
        }
    }
}
