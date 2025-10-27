using IzhevskEventRadar.Infrastructure.Models.GeminiApiRequests;

namespace IzhevskEventRadar.Infrastructure.Models.GeminiApiResponses;

public record GeminiApiResponse(IReadOnlyCollection<Candidate> Candidates);

public record Candidate(Content Content);
