using PamelloClientCLI.Enumerators;

namespace PamelloClientCLI.Commands;

public abstract class Command
{
    public ECommand Name { get; }
    public string[] Args { get; protected set; }
    
    protected Command(ECommand name) {
        Name = name;
    }
    
    public abstract Task Execute();
    
    public abstract string Help();
    public abstract string Examples();
}