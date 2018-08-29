using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Management.Automation;
using System.Threading.Tasks;

namespace Logic.Server
{
    public class IisServer
    {
        private readonly string _remoteServerComputerName;

        public IisServer(string remoteServerComputerName)
        {
            _remoteServerComputerName = remoteServerComputerName 
                                        ?? throw new ArgumentNullException(paramName: nameof(remoteServerComputerName));
        }

        public async Task StopAsync()
        {
            await RunStartOrStopCommandAsync("stop");
        }

        public async Task StartAsync()
        {
            await RunStartOrStopCommandAsync("start");
        }

        private async Task RunStartOrStopCommandAsync(string command)
        {
            // текст команды взят отсюда https://gallery.technet.microsoft.com/scriptcenter/Powershell-script-to-363dd543\
            // invoke-command -computername $serverName {cd C:\Windows\System32\; ./cmd.exe /c "iisreset /noforce /stop" }

            var script = @"invoke-command -computername " + _remoteServerComputerName + 
                         @" {cd C:\Windows\System32\; ./cmd.exe /c ""iisreset /noforce /" + command + @""" }";

            await RunPowerShellScriptAsync(script);
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
    }
}