using PamelloClientCLI.Enumerators;
using PamelloV7.Wrapper;

namespace PamelloClientCLI.Commands;

public abstract class Command
{
    protected readonly PamelloClient _pamello;
    
    public ECommandName Name { get; }
    public readonly List<string> Args;
    
    protected Command(ECommandName name, PamelloClient pamello) {
        Name = name;
        Args = new List<string>();
        
        _pamello = pamello;
    }
    
    public abstract void InitFromArgs();
    public abstract Task Execute();
    
    public abstract string Help();
    public abstract string Examples();
}