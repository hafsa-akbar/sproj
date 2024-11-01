namespace sproj.Services;

public interface ISmsService {
    public void SendCode(string to, int code);
}

public class SmsService : ISmsService {
    public void SendCode(string to, int code) {
        throw new NotImplementedException();
    }
}

public class DummySmsService : ISmsService {
    private readonly ILogger<DummySmsService> _logger;

    public DummySmsService(ILogger<DummySmsService> logger) {
        _logger = logger;
    }

    public void SendCode(string to, int code) {
        _logger.LogInformation("Sending code {code} to {to}", code, to);
    }
}
