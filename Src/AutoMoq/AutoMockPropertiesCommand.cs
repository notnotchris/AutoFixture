﻿using System;
using System.Reflection;
using AutoFixture.Kernel;
using Moq;

namespace AutoFixture.AutoMoq
{
    /// <summary>
    /// A command that populates all public writable properties/fields of a mock object with anonymous values.
    /// </summary>
    public class AutoMockPropertiesCommand : ISpecimenCommand
    {
        private readonly ISpecimenCommand autoPropertiesCommand =
            new AutoPropertiesCommand(new IgnoreProxyMembersSpecification());

        /// <summary>
        /// Populates all public writable properties/fields of a mock object with anonymous values.
        /// </summary>
        /// <param name="specimen">The mock object whose properties/fields will be populated.</param>
        /// <param name="context">The context that is used to create anonymous values.</param>
        public void Execute(object specimen, ISpecimenContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var mock = specimen as Mock;
            if (mock == null)
                return;

            this.autoPropertiesCommand.Execute(mock.Object, context);
        }

        /// <summary>
        /// Evaluates whether a request to populate a member is valid.
        /// The request is valid if the member is a property or a field,
        /// except for fields generated by Castle's DynamicProxy.
        /// </summary>
        private class IgnoreProxyMembersSpecification : IRequestSpecification
        {
            public bool IsSatisfiedBy(object request)
            {
                switch (request)
                {
                    case FieldInfo fi:
                        return !IsProxyMember(fi);

                    case PropertyInfo _:
                        return true;

                    default:
                        return false;
                }
            }

            private static bool IsProxyMember(FieldInfo fi)
            {
                return string.Equals(fi.Name, "__interceptors", StringComparison.Ordinal) ||
                       string.Equals(fi.Name, "__target", StringComparison.Ordinal);
            }
        }
    }
}
