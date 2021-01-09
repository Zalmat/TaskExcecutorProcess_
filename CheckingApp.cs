using System;
using System.Linq;

namespace TaskExcecutorProcess
{
    public class CheckingApp
    {
        public static string OpenSSL()
        {   
            string environmentPath = Environment.GetEnvironmentVariable("openssl");
            if (!string.IsNullOrEmpty(environmentPath))
            {
                return environmentPath;
            }
            //return "OPENSSL Не установлен. \nУстановите FULL Version(Запоминаем папку установки): https://slproweb.com/products/Win32OpenSSL.html\nДалее: Мой компьютер ->  Изменить параметры -> Дополнительно -> Переменные среды. -> \n Системные среды -> Path -> Изменить -> В поле значение переменной ДОПИСАТЬ путь до установленного openssl\n Пример: ;C:\\OpenSSL\\OpenSSL-Win64\\bin\\\n Нажимаем ОК->OK->OK и для проверки вбиваем в командной строке (CMD) openssl\n";
            return "OPENSSL - Не установлен";
        }
        /// <summary>
        /// Проверка на наличие JAVA_HOME в переменных окружения WINDOWS
        /// </summary>
        /// <returns></returns>
        public static string Keytool()
        {   
            if (OSWindows())
            {
                string environmentPath = Environment.GetEnvironmentVariable("JAVA_HOME");
                if (!string.IsNullOrEmpty(environmentPath))
                {
                    return "";
                }
                return "JAVA_HOME - Не установлен.";
            }
            return "";
        }
        public static string GetCheckOpenSslCommandLine()
        {
            var output = Execute.ExecuteExternal(namePowerShellForOS, $"-c \"openssl help\"");
            var lines = output.Split(Environment.NewLine);
            var сheckreponse = lines.Where(s => s.Contains("Standard commands")).FirstOrDefault();
            if (!string.IsNullOrEmpty(сheckreponse))
            {
                return "";
            }
            return "OpenSSL - Не настроен, либо не установлен";
        }

/// <summary>
/// Проверка. ОС Windows?
/// </summary>
/// <returns></returns>
        public static bool OSWindows()
        {
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                return true;
            }
            else return false;
        } 
        public static bool oSWindows = OSWindows();
//По доке в W10 всегда для вызова используется pwsh == Ложь.
//Если win == powershell, если *nix то == pwsh и баста.
        public static string NamePowerShellForOS(bool oSWindowsTrue)
        {
            if (oSWindowsTrue == true)
            {
                return "powershell";
            }
            else
            {
                return "pwsh";
            }
        }
        public static string namePowerShellForOS = NamePowerShellForOS(OSWindows());
    }
}