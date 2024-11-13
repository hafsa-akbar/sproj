using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using sproj.Data;

namespace sproj.Services;

public interface ICnicVerificationService {
    Task<string?> VerifyCnicAsync(User user, Stream cnicImage);
}

public class CnicVerificationService : ICnicVerificationService {
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<DummyCnicVerificationService> _logger;


    public CnicVerificationService(IHttpClientFactory httpClientFactory, ILogger<DummyCnicVerificationService> logger) {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<string?> VerifyCnicAsync(User user, Stream cnicImage) {
        var httpClient = _httpClientFactory.CreateClient();

        try {
            using var content = new MultipartFormDataContent();
            using var imageContent = new StreamContent(cnicImage);
            imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            content.Add(imageContent, "image", "cnic.png");

            var response = await httpClient.PostAsync("http://127.0.0.1:1811", content);

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var responseData = JsonNode.Parse(jsonResponse)!;

            if (!response.IsSuccessStatusCode) {
                _logger.LogWarning("Request to microservice failed: {body}", jsonResponse);
                return null;
            }

            if (bool.Parse(responseData["id_proofing"]!.ToString())) return null;

            var birthdate = DateOnly.Parse(responseData["birthdate"]!.ToString());
            if (user.Birthdate != birthdate) return null;

            var expiry = DateOnly.Parse(responseData["expirydate"]!.ToString());
            if (expiry < DateOnly.FromDateTime(DateTime.Today)) return null;

            return responseData["cnic"]!.ToString();
        } catch (Exception exception) {
            _logger.LogError("Exception while verifying cnic: {exception}", exception);
            return null;
        }
    }
}

public class DummyCnicVerificationService : ICnicVerificationService {
    public Task<string?> VerifyCnicAsync(User user, Stream cnicImage) {
        return Task.FromResult("15609-0979259-9");
    }
}