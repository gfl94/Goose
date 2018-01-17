using System;
using System.Collections.Generic;
using System.Text;

namespace Goose.Scanner
{
    interface ISelector
    {
        void Populate(List<GooseTypePair> pairs);
    }
}
