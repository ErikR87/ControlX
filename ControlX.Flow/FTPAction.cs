using ControlX.Flow.Contract;
using Renci.SshNet;
using System.Security;

namespace ControlX.Flow.Core
{
    public class FTPAction : IFTPAction
    {
        public string Name => "FTP File Transfer";
        public string Host {  get; set; }
        public int Port { get; set; }
        public string Path { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string SourceFile { get; set; }
        public IAutomate Automate { get; set; }

        public Task RunAsync()
        {
            var connectionInfo = new ConnectionInfo(
                Host,
                UserName,
                new PasswordAuthenticationMethod(
                    UserName,
                    Password
                )
            );

            using (var client = new SftpClient(connectionInfo))
            {
                client.HostKeyReceived += (sender, e) =>
                {
                    e.CanTrust = true;
                };

                using (var file = new FileStream(SourceFile, FileMode.Open))
                {
                    client.Connect();
                    client.UploadFile(file, Path + "test.txt", null);
                } 
            }

            return Task.CompletedTask;
        }
    }
}
