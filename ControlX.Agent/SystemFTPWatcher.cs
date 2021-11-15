using Renci.SshNet;
using Renci.SshNet.Common;
using Renci.SshNet.Sftp;

namespace ControlX.Agent;

public class SystemFTPWatcher : IDisposable
{
    #region properties
    public string Host { get; set; }
    public int Port { get; set; }
    public string Path { get; set; }
    public string UserName { get; set; }
    public string? Password { get; set; }
    public string? PasswordFile { get; set; }
    public string? PrivateKeyFile { get; set; }
    public string? PassPhrase { get; set; }
    public string? FingerPrint { get; set; }
    public FTPWatcherEvents WatcherEvents { get; set; }
    #endregion
    #region events
    public delegate void FileAdded(SftpFile file);
    public delegate void FileRemoved(string fullName);
    public delegate void FileUpdated(SftpFile file);

    public event FileAdded OnFileAdded;
    public event FileRemoved OnFileRemoved;
    public event FileUpdated OnFileUpdated;
    #endregion
    #region fields
    private FTPWatcherWorkerConfig _config;
    private SftpClient _client;
    private Dictionary<string, int> _fileHashes = new Dictionary<string, int>();
    #endregion

    public SystemFTPWatcher(FTPWatcherWorkerConfig config, FTPWatcherEvents events)
    {
        _config = config;
        Host = _config.Host;
        Port = _config.Port;
        Path = _config.Path;
        UserName = _config.UserName;
        Password = _config.Password;
        PasswordFile = _config.PasswordFile;
        PrivateKeyFile = _config.PrivateKeyFile;
        PassPhrase = _config.PassPhrase;
        FingerPrint = _config.FingerPrint;
        WatcherEvents = events;
        Connect();
        InitFileHashes();
    }

    public void Connect()
    {
        #region authentification
        var authentificationMethods = new List<AuthenticationMethod>();

        if (PasswordFile != null)
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

        _client = new SftpClient(connectionInfo);

        // check fingerprint
        _client.HostKeyReceived += HandleHostKeyReceived;

        _client.Connect();
    }

    public void InitFileHashes()
    {
        var files = _client.ListDirectory(Path);

        foreach(var file in files)
        {
            _fileHashes.Add(file.FullName, file.GetHashCode());
        }
    }

    public void Watch()
    {
        var files = _client.ListDirectory(Path);

        //files deleted or added?
        if ((
                WatcherEvents.HasFlag(FTPWatcherEvents.FileAddEvent) ||
                WatcherEvents.HasFlag(FTPWatcherEvents.FileDeleteEvent)
        ) && files.Count() != _fileHashes.Count())
        {
            if(WatcherEvents.HasFlag(FTPWatcherEvents.FileAddEvent) && files.Count() > _fileHashes.Count())
            {
                //files added

                foreach(var file in files)
                {
                    if (!_fileHashes.ContainsKey(file.FullName))
                    {
                        _fileHashes.Add(file.FullName, file.GetHashCode());
                        OnFileAdded.Invoke(file);
                    }
                }
            }
            else if(WatcherEvents.HasFlag(FTPWatcherEvents.FileDeleteEvent))
            {
                //files deleted

                var fileNames = files.Select(x => x.FullName);
                foreach(var keyValue in _fileHashes)
                {
                    if (!fileNames.Contains(keyValue.Key))
                    {
                        _fileHashes.Remove(keyValue.Key);
                        OnFileRemoved.Invoke(keyValue.Key);
                    }
                }
            }
        }

        if (WatcherEvents.HasFlag(FTPWatcherEvents.FileUpdateEvent))
            foreach(var file in files)
            {
                if (_fileHashes[file.FullName] != file.GetHashCode())
                {
                    _fileHashes[file.FullName] = file.GetHashCode();
                    OnFileUpdated.Invoke(file);
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

    public void Dispose()
    {
        _client.Disconnect();
        _client.Dispose();
    }
}

[Flags]
public enum FTPWatcherEvents
{
    FileAddEvent = 1,
    FileDeleteEvent = 2,
    FileUpdateEvent = 4
}