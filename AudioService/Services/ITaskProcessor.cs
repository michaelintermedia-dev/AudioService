namespace AudioService.Services;

public interface ITaskProcessor
{
    Task ProcessAsync(CancellationToken cancellationToken);
}
