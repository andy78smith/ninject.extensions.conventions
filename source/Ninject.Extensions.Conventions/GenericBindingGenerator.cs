#region License

// 
// Author: Ian Davis <ian.f.davis@gmail.com>
// 
// Dual-licensed under the Apache License, Version 2.0, and the Microsoft Public License (Ms-PL).
// See the file LICENSE.txt for details.
// 

#endregion

#region Using Directives

using System;

#endregion

namespace Ninject.Extensions.Conventions
{
    public class GenericBindingGenerator : IBindingGenerator
    {
        private static readonly Type TypeOfObject = typeof (object);
        private readonly Type _contractType;

        public GenericBindingGenerator( Type contractType )
        {
            if ( !( contractType.IsGenericType || contractType.ContainsGenericParameters ) )
            {
                throw new ArgumentException( "The contract must be an open generic type.", "contractType" );
            }
            _contractType = contractType;
        }

        #region Implementation of IBindingGenerator

        /// <summary>
        /// Processes the specified type creating kernel bindings.
        /// </summary>
        /// <param name="type">The type to process.</param>
        /// <param name="kernel">The kernel to configure.</param>
        public void Process( Type type, IKernel kernel )
        {
            Type interfaceType = ResolveClosingInterface( type );
            if ( interfaceType != null )
            {
                kernel.Bind( interfaceType ).To( type );
            }
        }

        #endregion

        public Type ResolveClosingInterface( Type targetType )
        {
            if ( targetType.IsInterface || targetType.IsAbstract )
            {
                return null;
            }

            do
            {
                Type[] interfaces = targetType.GetInterfaces();
                foreach ( Type @interface in interfaces )
                {
                    if ( !@interface.IsGenericType )
                    {
                        continue;
                    }

                    if ( @interface.GetGenericTypeDefinition() == _contractType )
                    {
                        return @interface;
                    }
                }
                targetType = targetType.BaseType;
            } while ( targetType != TypeOfObject );

            return null;
        }
    }
}