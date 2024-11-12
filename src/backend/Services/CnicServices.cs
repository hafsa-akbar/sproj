using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using sproj.Data;

namespace sproj.Services;

public interface ICnicVerificationService {
    Task<bool> VerifyCnicAsync(User user, Stream cnicImage);
}

public class CnicVerificationService : ICnicVerificationService {
    private IHttpClientFactory _httpClientFactory;
    private ILogger<DummyCnicVerificationService> _logger;


    public CnicVerificationService(IHttpClientFactory httpClientFactory, ILogger<DummyCnicVerificationService> logger) {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<bool> VerifyCnicAsync(User user, Stream cnicImage) {
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
                return false;
            }

            var cnic = responseData["cnic"]!.ToString();
            var idProofing = bool.Parse(responseData["id_proofing"]!.ToString());

            return cnic == user.CnicNumber && idProofing;
        } catch (Exception exception){
            _logger.LogError("Exception while verifying cnic: {exception}", exception);
            return false;
        }
    }
}

public class DummyCnicVerificationService : ICnicVerificationService {
    public Task<bool> VerifyCnicAsync(User user, Stream cnicImage) {
        return Task.FromResult(true);
    }
}
