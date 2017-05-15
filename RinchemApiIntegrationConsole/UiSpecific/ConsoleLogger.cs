using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RinchemApiIntegrationConsole.UiSpecific
{
    static class ConsoleLogger
    {
        private static asnIntegratorUI integratorUi;
        private static String content;

        public static void initialize(asnIntegratorUI integratorUi)
        {
            ConsoleLogger.integratorUi = integratorUi;
        }

        public static void log(String message)
        {
            content += message + "\n";
            integratorUi.updateLogBox(content);
        }
    }
}
