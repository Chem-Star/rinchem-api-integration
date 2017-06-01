using asnIntegratorConsole.UiSpecific;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RinchemApiIntegrationConsole.UiSpecific
{
    static class ConsoleLogger
    {
        private static DebugConsole debugUi;
        private static ApiUserConsole consoleUi;
        private static String content;

        public static void initialize(DebugConsole debugUi)
        {
            ConsoleLogger.debugUi = debugUi;
        }
        public static void initialize(ApiUserConsole consoleUi)
        {
            ConsoleLogger.consoleUi = consoleUi;
        }

        public static void log(String message)
        {
            content += message + "\n";
            if (debugUi != null)
            {
                debugUi.updateLogBox(content);
            }
            if (consoleUi != null)
            {
                consoleUi.updateLogBox(content);
            }
        }
        public static String getContent()
        {
            return content;
        }
    }
}
