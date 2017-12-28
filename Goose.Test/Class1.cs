using System;

namespace Goose.Test
{
    interface IToString
    {
        void NotExist();
        string ToString();
    }

    public class Class1
    {
        public void M()
        {
            IToString x = 1.Goose<IToString>();
            x.ToString();
            x.NotExist();
        }
    }
}
