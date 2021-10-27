using ControlX.Flow.Contract;
using Renci.SshNet;
using Dahomey.Json.Attributes;
using Microsoft.Extensions.Logging;

namespace ControlX.Flow.Core;

[JsonDiscriminator(nameof(FTPAction))]
public class FTPAction : FlowAction<FTPAction>, IFTPAction
{
    public string Host {  get; set; }
    public int Port { get; set; }
    public string Path { get; set; }
    public string UserName { get; set; }
    public string? Password { get; set; }
    public string? PrivateKeyFile { get; set; }
    public string? PassPhrase { get; set; }
    public string? FingerPrint { get; set; }
    public string SourceFile { get; set; }
    public IAutomate Automate { get; set; }

    public async Task RunAsync()
    {
        await base.RunAsync();

        using (_logger.BeginScope(this))
        {
            var authentificationMethods = new List<AuthenticationMethod>();

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

            using (var client = new SftpClient(connectionInfo))
            {
                client.HostKeyReceived += (sender, e) =>
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
                };

                using (var file = new FileStream(SourceFile, FileMode.Open))
                {
                    var fileName = SourceFile.Split('\\').Last();
                    client.Connect();
                    client.UploadFile(file, Path + fileName, null);
                }
            }

            _logger.LogInformation($"FTP-Action: File {SourceFile} transferd to {Host} path {Path}");
        }   
    }
}
