using System.Text;
using sproj.Data;

namespace sproj.Services;

public interface ICnicVerificationService {
    Task<bool> VerifyCnicAsync(User user, Stream cnicImage);
}

public class CnicVerificationService : ICnicVerificationService {
    public Task<bool> VerifyCnicAsync(User user, Stream cnicImage) {
        throw new NotImplementedException();
    }
}

public class DummyCnicVerificationService : ICnicVerificationService {
    private ILogger<DummyCnicVerificationService> _logger;

    public DummyCnicVerificationService(ILogger<DummyCnicVerificationService> logger) {
        _logger = logger;
    }

    public Task<bool> VerifyCnicAsync(User user, Stream cnicImage) {
        return Task.FromResult(true);
    }
}
