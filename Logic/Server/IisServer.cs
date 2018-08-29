using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Threading.Tasks;
using Logic.Arhivator;

namespace Logic.Server
{
    internal class IisServer
    {
        private readonly string _remoteServerComputerName;

        public IisServer(string remoteServerComputerName)
        {
            _remoteServerComputerName = remoteServerComputerName 
                                        ?? throw new ArgumentNullException(paramName: nameof(remoteServerComputerName));
        }

        internal async Task StopAsync(Action<AsyncActionResult> asyncActionCallback)
        {
            try
            {
                string result = await RunStartOrStopCommandAsync("stop");

                AsyncActionResult actionResult = IsSuccessfull(result)
                    ? AsyncActionResult.Success()
                    : AsyncActionResult.Fail(new IisServerException($"Не удалось остановить сервер: {result}"));

                asyncActionCallback(actionResult);
            }
            catch (Exception exception)
            {
                AsyncActionResult.Fail(new IisServerException($"Остановка сервера закончилась ошибкой: {exception.Message}", exception));
            }
            
        }

        internal async Task StartAsync(Action<AsyncActionResult> asyncActionCallback)
        {
            try
            {
                string result = await RunStartOrStopCommandAsync("start");

                AsyncActionResult actionResult = IsSuccessfull(result)
                    ? AsyncActionResult.Success()
                    : AsyncActionResult.Fail(new IisServerException($"Не удалось запустить сервер: {result}"));

                asyncActionCallback(actionResult);
            }
            catch (Exception exception)
            {
                AsyncActionResult.Fail(new IisServerException($"Запуск сервера закончился ошибкой: {exception.Message}", exception));
            }
        }

        private bool IsSuccessfull(string outputText)
        {
            const string successfulTextPart = @"successfully";

            return outputText.ToLowerInvariant().Contains(successfulTextPart);
        }

        private async Task<string> RunStartOrStopCommandAsync(string command)
        {
            // текст команды взят отсюда https://gallery.technet.microsoft.com/scriptcenter/Powershell-script-to-363dd543\
            // invoke-command -computername $serverName {cd C:\Windows\System32\; ./cmd.exe /c "iisreset /noforce /stop" }

            var script = @"invoke-command -computername " + _remoteServerComputerName + 
                         @" {cd C:\Windows\System32\; ./cmd.exe /c ""iisreset /noforce /" + command + @""" }";

            return await RunPowerShellScriptAsync(script);
        }

        // Код взят отсюда https://stackoverflow.com/a/41963553
        // более подробно о powershell в c# 
        // https://docs.microsoft.com/en-us/dotnet/api/system.management.automation.powershell?redirectedfrom=MSDN&view=powershellsdk-1.1.0
        private async Task<string> RunPowerShellScriptAsync(string script)
        {
            return await Task.Run(() =>
            {
                //The first step is to create a new instance of the PowerShell class
                //PowerShell.Create() creates an empty PowerShell pipeline for us to use for execution.
                using (var powerInstance = PowerShell.Create())
                {
                    // use "AddScript" to add the contents of a script file to the end of the execution pipeline.
                    // use "AddCommand" to add individual commands/cmdlets to the end of the execution pipeline.

                    powerInstance.AddScript(script);

                    //Result of the script with Invoke()
                    Collection<PSObject> result = powerInstance.Invoke();

                    string text = string.Empty;
                    foreach (PSObject psObject in result)
                    {
                        text += psObject.BaseObject.ToString();
                    }

                    return text;
                }
            });
        }

        public override string ToString()
        {
            return _remoteServerComputerName;
        }
    }
}