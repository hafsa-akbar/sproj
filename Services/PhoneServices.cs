namespace sproj.Services;

public interface IPhoneService {
    public void SendCode(string to, string code);
}

public class PhoneService : IPhoneService {
    public void SendCode(string to, string code) {
        throw new NotImplementedException();
    }
}

public class DummyPhoneService : IPhoneService {
    private readonly ILogger<DummyPhoneService> logger;

    public DummyPhoneService(ILogger<DummyPhoneService> logger) {
        this.logger = logger;
    }

    public void SendCode(string to, string code) {
        logger.LogInformation("Sending code {code} to {to}", code, to);
    }
}
