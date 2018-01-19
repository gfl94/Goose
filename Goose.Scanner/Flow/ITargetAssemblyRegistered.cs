using System;
using System.Collections.Generic;
using System.Text;

namespace Goose.Scanner.Flow
{
    public interface ITargetAssemblyRegistered
    {
        /// <summary>
        /// The default convention makes pairs for:
        ///     1. public class Foo in source assembly and public interface IFoo in target assembly
        ///     2. public enum type E in source assembly and public enum type E in target assembly        
        /// </summary>
        /// <returns></returns>
        IScanOptionsBlock WithDefaultConvention();

        IScanOptionsBlock WithConvention(Func<Type, Type, bool> predicate);

        IScanOptionsBlock WithConvention<T>() where T : IConvention, new();

        IScanOptionsBlock WithConvention(IConvention convention);
    }
}
