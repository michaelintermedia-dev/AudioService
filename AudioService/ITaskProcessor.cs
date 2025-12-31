namespace AudioService;

public interface ITaskProcessor
{
    Task ProcessAsync(CancellationToken cancellationToken);
}
