using System.Diagnostics;
using System.Text;
using System.Threading;

namespace TaskExcecutorProcess
{
    public class Execute
    {
        public static string ExecuteExternal(string fileName, string args)
        {          
            StringBuilder log = new StringBuilder();
            string outputText = string.Empty;
            string outputError = string.Empty;

            using (Process process = new Process())
            {
                process.StartInfo.FileName = fileName;
                process.StartInfo.Arguments = args;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();

                outputText = process.StandardOutput.ReadToEnd();
                log.AppendLine(outputText);

                outputError = process.StandardError.ReadToEnd();
                log.AppendLine(outputError);
              
                process.WaitForExit();

            }

            return log.ToString();
        }

        public static string ExecuteExternal(string fileName, string args, int timeout)
        {
            StringBuilder log = new StringBuilder();

            using (Process process = new Process())
            {
                process.StartInfo.FileName = fileName;
                process.StartInfo.Arguments = args;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;

                using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
                using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
                {
                    process.OutputDataReceived += (sender, e) => {
                        if (e.Data == null)
                        {
                            outputWaitHandle.Set();
                        }
                        else
                        {
                            log.AppendLine(e.Data);
                        }
                    };
                    process.ErrorDataReceived += (sender, e) => {
                        if (e.Data == null)
                        {
                            errorWaitHandle.Set();
                        }
                        else
                        {
                            log.AppendLine(e.Data);
                        }
                    };

                    process.Start();

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    if (timeout > 0)
                    {
                        if (process.WaitForExit(timeout) &&
                            outputWaitHandle.WaitOne(timeout) &&
                            errorWaitHandle.WaitOne(timeout))
                        {

                        }
                    }
                    else
                    {
                        process.WaitForExit();
                        outputWaitHandle.WaitOne();
                        errorWaitHandle.WaitOne();

                    }
                }
            }

            return log.ToString();
        }
        
    }
}
