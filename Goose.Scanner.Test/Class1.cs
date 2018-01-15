using System;
using System.Collections.Generic;
using System.Text;

namespace Goose.Scanner.Test
{
    class Class1
    {
        public void M()
        {
            var pairs = GooseTypePairs.Build(options =>
            {
                //options.FromAssembly(sourceAssembly)
                //    .ToAssembly(targetAssembly)
                //    .WithDefaultConvention()     //make all Foo -> IFoo pattern pair
                //    .WithConvention((sourceType, targetType) => targetType.Name == "IStandard" + sourceType.Name);  // Foo -> IStandardFoo convention

                //other similiar registers
            });

            //use the pairs (probably cache it)
        }
    }
}
