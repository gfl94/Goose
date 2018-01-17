using System;
using System.Collections.Generic;
using System.Text;

namespace Goose.Scanner.Flow
{
    public interface ITargetAssemblyRegistered
    {
        /// <summary>
        /// The default convention makes Foo/IFoo pairs for public class Foo and public interface IFoo
        /// </summary>
        /// <returns></returns>
        IScanOptionsBlock WithDefaultConvention();

        IScanOptionsBlock WithConvention(Func<Type, Type, bool> predicate);

        IScanOptionsBlock WithConvention<T>() where T : IConvention, new();

        IScanOptionsBlock WithConvention(IConvention convention);
    }
}
