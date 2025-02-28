using System;
using System.Collections.Generic;
using System.Text;

namespace BBE.Helpers
{
    public interface IPrefab
    {
        void SetupAssets();
    }
    public interface INPCPrefab : IPrefab
    {

    }
    public interface IItemPrefab : IPrefab 
    { 
    }
}
