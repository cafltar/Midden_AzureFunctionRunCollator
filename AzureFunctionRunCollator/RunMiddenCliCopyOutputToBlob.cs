using System;
using System.Diagnostics;
using System.IO;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace AzureFunctionRunCollator
{
    public class RunMiddenCliCopyOutputToBlob
    {
        [FunctionName("RunMiddenCliCopyOutputToBlob")]
        public void Run(
            [TimerTrigger("0 0 0 * * *")]TimerInfo myTimer, 
            ILogger log,
            ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            string exePath = Path.Combine(context.FunctionDirectory, "..", "MiddenCli.exe");
            if (!File.Exists(exePath))
                throw new Exception("Cannot find MiddenCli.exe");
            else
            {
                log.LogInformation("Found MiddenCli.exe at: " + exePath);
            }

            string configPath = Path.Combine(context.FunctionDirectory, "..", "configuration.json");
            if (!File.Exists(configPath))
                throw new Exception("Cannot find configuration.json");
            else
            {
                log.LogInformation("Found configuration.json at: " + configPath);
            }

            var catalogPath = Path.Combine(context.FunctionDirectory, "..", "catalog.json");

            // Run MiddenCli.exe
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.OutputDataReceived += (sender, args) => log.LogInformation(args.Data);
            p.StartInfo.FileName = exePath;
            p.StartInfo.Arguments = $"collate -s -o \"{catalogPath}\"";
            p.Start();
            p.BeginOutputReadLine();
            p.WaitForExit();

            log.LogInformation("ExitCode: " + p.ExitCode.ToString());

            // Upload output
            if (File.Exists(catalogPath))
            {
                log.LogInformation("Found catalog.json");

                string connection = Environment.GetEnvironmentVariable("CafPublicConnString");
                string container = Environment.GetEnvironmentVariable("ContainerName");
                string writeBlobName = Environment.GetEnvironmentVariable("BlobPath");

                using(Stream blob = File.OpenRead(catalogPath))
                {
                    BlobContainerClient blobContainerClient = new BlobContainerClient(connection, container);
                    BlobClient blobClient = blobContainerClient.GetBlobClient(writeBlobName);
                    blobClient.Upload(
                        blob,
                        new BlobUploadOptions() { AccessTier = AccessTier.Hot });
                }    
                
                try
                {
                    File.Delete(catalogPath);
                    log.LogInformation("Deleted catalog.json");
                }      
                catch(Exception e)
                {
                    log.LogInformation("Failed to delete catalog.json with error: " + e.ToString());
                }
            }
            else
            {
                throw new Exception("catalog.json not found");
            }

            log.LogInformation("Seems like a success");
        }
    }
}
