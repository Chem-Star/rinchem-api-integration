using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RinchemApiIntegrationConsole
{
    public interface DataObject
    {
        Boolean validate();
        void setObjectName(String name);
        void setAction(String action);
        String getObjectName();
    }
}
