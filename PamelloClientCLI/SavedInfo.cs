namespace PamelloClientCLI;

public class SavedInfo
{
    public string? ServerAddress;
    public Guid? Token;
    
    public Dictionary<string, string> Aliases;
    
    private string filePath => Environment.CurrentDirectory + "\\SavedInfo";
    
    public void Load() {
        if (!File.Exists(filePath)) {
            Aliases = new Dictionary<string, string>();
            return;
        }
        
        using var fs = File.Open(filePath, FileMode.Open);
        using var br = new BinaryReader(fs);

        Aliases = new Dictionary<string, string>();
        if (fs.Length == 0) return;
        
        if (br.ReadBoolean()) {
            ServerAddress = br.ReadString();
        }
        if (br.ReadBoolean()) {
            Token = Guid.Parse(br.ReadString());
        }
        
        var aliasesCount = br.ReadInt32();
        Aliases = new Dictionary<string, string>(aliasesCount);
        for (int i = 0; i < aliasesCount; i++) {
            Aliases.Add(br.ReadString(), br.ReadString());
        }
    }
    
    public void Save() {
        using var fs = File.Open(filePath, FileMode.OpenOrCreate);
        using var bw = new BinaryWriter(fs);
        
        fs.SetLength(0);
        
        bw.Write(ServerAddress is not null);
        if (ServerAddress is not null) {
            bw.Write(ServerAddress);
        }
        bw.Write(Token is not null);
        if (Token is not null) {
            bw.Write(Token.ToString()!);
        }
        
        bw.Write(Aliases.Count);
        foreach (var (alias, command) in Aliases) {
            bw.Write(alias);
            bw.Write(command);
        }
        
        bw.Flush();
        fs.Flush();
    }
}