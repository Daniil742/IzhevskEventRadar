namespace IzhevskEventRadar.Contracts.Interfaces.Communicators;

public interface IGeminiApiCommunicator
{
    Task<string> GenerateTextFromPromptAsync(string prompt, IReadOnlyCollection<string>? imageUrls, CancellationToken cancellationToken = default);
}
