using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RinchemApiIntegrationConsole
{
    public interface DataObject
    {
        //API Relevant
        String getCustomApiSuffix();            //Suffix of the api

        //Data Relevant
        Boolean validate();
        void setObjectName(String name);
        void setAction(String action);
        String getObjectName();

        void initializeRequest();
        void initializeResponse();
        String serializeRequest();
        void deserializeResponse(String response);

        Window getResponseView();
    }
}
