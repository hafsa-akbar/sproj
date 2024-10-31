namespace sproj.Services;

public interface ISMSService {
    public void SendCode(string to, int code);
}

public class SMSService : ISMSService {
    public void SendCode(string to, int code) {
        throw new NotImplementedException();
    }
}

public class DummySMSService : ISMSService {
    private readonly ILogger<DummySMSService> logger;

    public DummySMSService(ILogger<DummySMSService> logger) {
        this.logger = logger;
    }

    public void SendCode(string to, int code) {
        logger.LogInformation("Sending code {code} to {to}", code, to);
    }
}
