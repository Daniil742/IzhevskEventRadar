namespace IzhevskEventRadar.WebBot.Interfaces;

public interface IReceiverService
{
    Task ReceiveAsync(CancellationToken stoppingToken);
}
