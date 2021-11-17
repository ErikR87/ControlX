using ControlX.Flow.Contract;
using Renci.SshNet;
using Dahomey.Json.Attributes;
using Microsoft.Extensions.Logging;
using Renci.SshNet.Common;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace ControlX.Flow.Core;

[JsonDiscriminator(nameof(FTPAction))]
public class FTPAction : FlowAction<FTPAction>, IFTPAction
{

    #region Properties
    public string Host {  get; set; }
    public int Port { get; set; }
    public string Path { get; set; }
    public string UserName { get; set; }
    public string? Password { get; set; }
    public string? PasswordFile { get; set; }
    public string? PrivateKeyFile { get; set; }
    public string? PassPhrase { get; set; }
    public string? FingerPrint { get; set; }
    public string? SourceFile { get; set; }
    public string? DownloadDestination { get; set; }
    public IAutomate Automate { get; set; }
    public FTPMethod FTPMethod { get; set; } = FTPMethod.Upload;
    #endregion

    public async Task RunAsync()
    {
        using (_logger.BeginScope(this))
        {
            await base.RunAsync();

            #region authentification
            var authentificationMethods = new List<AuthenticationMethod>();

            if(PasswordFile != null)
                Password = File.ReadAllText(PasswordFile);

            if (Password != null)
                authentificationMethods.Add(
                    new PasswordAuthenticationMethod(
                    UserName,
                    Password
                ));

            if (PrivateKeyFile != null)
                authentificationMethods.Add(
                    new PrivateKeyAuthenticationMethod(
                        UserName,
                        new PrivateKeyFile(PrivateKeyFile, PassPhrase)
                ));


            var connectionInfo = new ConnectionInfo(
                Host,
                UserName,
                authentificationMethods.ToArray()
            );
            #endregion

            using (var client = new SftpClient(connectionInfo))
            {
                // check fingerprint
                client.HostKeyReceived += HandleHostKeyReceived;

                switch(FTPMethod)
                {
                    case FTPMethod.Upload:
                        UploadFile(client);
                        break;
                    case FTPMethod.Download:
                        DownloadFile(client);
                        break;
                }
            }

            
        }   
    }

    private void HandleHostKeyReceived(object? sender, HostKeyEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(FingerPrint))
        {
            if (BitConverter.ToString(e.FingerPrint).Replace('-', ':') == FingerPrint)
                e.CanTrust = true;
            else
                e.CanTrust = false;
        }
        else
            e.CanTrust = true;
    }

    private void UploadFile(SftpClient client)
    {
        using (var file = new FileStream(SourceFile, FileMode.Open))
        {
            string fileName = null;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                fileName = SourceFile.Split('/').Last();
            }
            else
            {
                fileName = SourceFile.Split('\\').Last();
            }

            client.Connect();
            client.UploadFile(file, Path + fileName, null);
        }

        _logger.LogInformation($"FTP-Action: File {SourceFile} uploaded to {Host} path {Path}");
    }

    private void DownloadFile(SftpClient client)
    {
        var fileName = Path.Split('/').Last();
        var destinationPath = System.IO.Path.Combine(DownloadDestination, fileName);

        using (var file = new FileStream(destinationPath, FileMode.CreateNew))
        {
            
            client.Connect();
            client.DownloadFile(Path, file);
        }

        _logger.LogInformation($"FTP-Action: File {Path} downloaded from {Host} to local path {DownloadDestination}");

    }
}
