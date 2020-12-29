using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TaskExcecutorProcess
{
    public class CertTool
    {
        public static string passwCert;
        public static string patchP12File;
        public static string patchPemFile;
        private static string KeytoolPath = Path.Combine(GetJavaInstallationPath(), "keytool");
        
        public static string GetJavaInstallationPath()
        {
            string environmentPath = Environment.GetEnvironmentVariable("JAVA_HOME");
            if (!string.IsNullOrEmpty(environmentPath))
            {
                return environmentPath;
            }

            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                string javaKey = "SOFTWARE\\JavaSoft\\Java Runtime Environment\\";
                using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(javaKey))
                {
                    string currentVersion = rk.GetValue("CurrentVersion").ToString();
                    using (Microsoft.Win32.RegistryKey key = rk.OpenSubKey(currentVersion))
                    {
                        return key.GetValue("JavaHome").ToString();
                    }
                }
            }
            else return string.Empty;
        }
        ///<summary> Получить название файла с форматом. </summary>
        public static string NameFile(string pathFile)
        {
            string filename = System.IO.Path.GetFileName(pathFile);
            return filename;
        }

        /// <summary> Сформировать csr файл </summary>
        public static void CreateCsr(string pathJksFile)
        {
            //получаем алиас из файла
            var aliasName = GetAliasKeyTool(pathJksFile);
            //генерируем csr из файла
            GenerateCsrFile(pathJksFile, aliasName);
        }
        public static void CreateNewKey(string pathJksFile, string crtPathFile, string csrPathFile = null)
        { 
            string compare = CertTool.GetCRC(csrPathFile,crtPathFile);
            Console.WriteLine(compare);
            if (compare.Substring(0,4) != "Ошибка сверки контрольных сумм.")
            {                
                //получаем алиас из файла
                var aliasName = GetAliasKeyTool(pathJksFile);
                //генерируем P12 и остальное
                convertJksInPkcs12(pathJksFile, aliasName);
                CertTool.newWorckKeyP12(crtPathFile);
            }
        }

        /// <summary> Получить альяс jks файла </summary>       
        private static string GetAliasKeyTool(string pathJksFile)
        {
            //выполняем скрипт powershell            
            var output = Execute.ExecuteExternal("pwsh", $"-c \"echo '{passwCert}' | {KeytoolPath} -list -v -keystore {pathJksFile}\"");
            //получаем по строкам
            var lines = output.Split(Environment.NewLine);            
            //находим строку алиаса
            var aliasline = lines.Where(s => s.Contains("Alias name")).FirstOrDefault();
            if (!string.IsNullOrEmpty(aliasline))
            {
                //получаем значение альяса
                var nameAlias = aliasline.Remove(aliasline.IndexOf("Alias name:"), "Alias name:".Length);
                return nameAlias.Trim();
            }
            return null;
        }

        /// <summary> Генерация csr файла </summary>      
        private static void GenerateCsrFile(string pathJksFile, string aliasName)
        {
            var csrFilePath = Path.ChangeExtension(pathJksFile, "csr");

            //выполняем скрипт powershell
            var output = Execute.ExecuteExternal("pwsh", $"-c \"echo '{passwCert}' | {KeytoolPath} -certreq -alias '{aliasName}' -keystore '{pathJksFile}' -file '{csrFilePath}'\"");

            if (output.Contains("error"))
                ConsoleHelper.ShowError(output);
            else
            {                              
                Console.WriteLine(GetCRC(csrFilePath));
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="patchCsr">Пусть до -.CSR</param>
        /// <param name="patchCrt">Пусть до -.CRT</param>
        /// <returns>Сверка контрольных сумм двух сертефикатов.Если перепутаешь порядок, контрольная сумма не сойдётся</returns>
        public static string GetCRC(string patchCsr, string patchCrt = null)
        {
            if (String.IsNullOrEmpty(patchCrt))
            {
                var output = Execute.ExecuteExternal("pwsh", $"-c \"openssl req -noout -modulus -in {patchCsr} | openssl md5\"");
                return output;
            }
            else
            {
                var output = Execute.ExecuteExternal("pwsh", $"-c \"openssl req -noout -modulus -in {patchCsr} | openssl md5\"");
                var output2 = Execute.ExecuteExternal("pwsh", $"-c \"openssl x509 -noout -modulus -in {patchCrt} | openssl md5\"");
                //По идее это УЖЕ не нужно, но бли-и-и-ин. 
                var stdin1 = Regex.Match(output,"\\(stdin\\)= (\\w{32})");
                /* 
                \(\w+\)= (\w{32}) Если, что-то помимо stdin, 
                если нужно понимать. что на месте stdim (напрмример тест), тогда \((\w+)\)= (\w{32}), 
                а ещё можно так, не все поддерживают парсеры \((?P<Name>\w+)\)= (?P<Value>\w{32})
                добавить начало(^) и конец($)  ^\((?P<Name>\w+)\)= (?P<Value>\w{32})$
                */
                var stdin2 = Regex.Match(output2,"\\(stdin\\)= (\\w{32})");            
                if (stdin1.Groups[1].Value != stdin2.Groups[1].Value) return "Бяда, Контрольная сумма не сходится \n\n Контрольная сумма " + NameFile(patchCsr) + " = " 
                                                                    + stdin1.Groups[1].Value +"\n Контрольная сумма " + NameFile(patchCrt) + " = " + stdin2.Groups[1].Value;                 
                else return "Контрольная сумма = ОК\n";
            }
            //if (output.Contains("error"))
            //    ConsoleHelper.ShowError(output);            
            
        }

/// <summary>
/// Конвертируем JKS в P12
/// </summary>
/// <param name="pathJksFile"></param>
/// <param name="aliasName"></param>
        public static void convertJksInPkcs12(string pathJksFile, string aliasName)
        {
            Console.WriteLine("Данные для формирования -.p12 получены = ОК\n");            
            var patchP12File = Path.ChangeExtension(pathJksFile, "p12");
            // Console.WriteLine($"echo {KeytoolPath} -importkeystore -srckeystore {pathJksFile} -destkeystore {patchP12File} -deststoretype PKCS12 -srcalias \"{aliasName}\" -srcstorepass {passwCert} -deststorepass {passwCert} -destkeypass {passwCert}");                                
            //выполняем скрипт powershell
            // var output = Execute.ExecuteExternal("pwsh", $"-c \"echo '{passwCert}' | {KeytoolPath} -list -v -keystore {NameFile(pathJksFile)}\"");
            var output = Execute.ExecuteExternal  ("pwsh", $"-c \"echo '{passwCert}' | {KeytoolPath} -importkeystore -srckeystore {pathJksFile} -destkeystore {patchP12File} -deststoretype PKCS12 -srcalias \"{aliasName}\" -srcstorepass {passwCert} -deststorepass {passwCert} -destkeypass {passwCert}");
            //Console.WriteLine("Тестовые данные. Файл должен быть тут: " + patchP12File + "\n");
            if (output.Contains("error")) ConsoleHelper.ShowError(output);
            else if (System.IO.File.Exists(patchP12File))
            {
                Console.WriteLine("Файл сертефиката -.p12 создан успешно = ОК\n");
                CertTool.patchP12File = patchP12File;
                CertTool.convertKeyPem();
            }
            else
            {
                Console.WriteLine("Неизвестная ошибка. Файл не создан. Начинай дебажить = FAIL\n");
            }            
        }
        //Знаю, что неправильно. похоже придётся снова всё переписывать. Пока сделаем просто реализацию.
        public static void convertKeyPem()
        {
            CertTool.patchPemFile = Path.ChangeExtension(CertTool.patchP12File, "pem");
            var output = Execute.ExecuteExternal("pwsh", $"-c \"openssl pkcs12 -passin pass:{passwCert} -in {CertTool.patchP12File} -nodes -nocerts -out {CertTool.patchPemFile}  \"");
                if (output.Contains("error")) ConsoleHelper.ShowError(output); 
                 else if (System.IO.File.Exists(CertTool.patchPemFile))
            {
                Console.WriteLine("Файл сертефиката -.pem создан успешно = ОК\n");
            }
            else
            {
                Console.WriteLine("Неизвестная ошибка. Файл -.pem не создан. Начинай дебажить = FAIL\n");
            }                           
        }
        public static void newWorckKeyP12(string patchCRT)
        {
            var newWorckKeyP12 = Path.ChangeExtension(CertTool.patchPemFile, "new.p12");
            var output = Execute.ExecuteExternal("pwsh", $"-c \"openssl pkcs12 -passout pass:{passwCert} -export -out {newWorckKeyP12} -inkey {CertTool.patchPemFile} -in {patchCRT}\"");
            if (output.Contains("error")) ConsoleHelper.ShowError(output); 
                 else if (System.IO.File.Exists(newWorckKeyP12))
            {
                Console.WriteLine("Файл сертефиката -.new.p12 создан успешно = ОК\n");
            }
            else
            {
                Console.WriteLine("Неизвестная ошибка. Файл -.new.p12 не создан. Начинай дебажить = FAIL\n");
            }                           
        }
    }
}
