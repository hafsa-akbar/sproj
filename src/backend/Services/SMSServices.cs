namespace sproj.Services;

public interface ISmsSender {
    public void SendCode(string to, int code);
}

public class SmsSender : ISmsSender {
    public void SendCode(string to, int code) {
        throw new NotImplementedException();
    }
}

public class DummySmsSender : ISmsSender {
    private readonly ILogger<DummySmsSender> _logger;

    public DummySmsSender(ILogger<DummySmsSender> logger) {
        _logger = logger;
    }

    public void SendCode(string to, int code) {
        _logger.LogInformation("Sending code {code} to {to}", code, to);
    }
}