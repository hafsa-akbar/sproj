namespace sproj.Endpoints;

public static class UserEndpoints {
    // public static IResult SendVerificationSmsEndpoint(ClaimsPrincipal claimsPrincipal, AppDbContext dbContext,
    //     CodeVerifier codeVerifier, ISmsSender smsSender) {
    //     var username = claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Sub)!.Value;
    //
    //     var user = dbContext.Users.First(u => u.Username == username);
    //     var code = codeVerifier.CreateCode(username);
    //     smsSender.SendCode(user.PhoneNumber, code);
    //
    //     return Results.Ok(new {
    //         message = "Verification code sent!"
    //     });
    // }
    //
    // public static async Task<IResult> VerifySmsEndpoint(int code, AppDbContext dbContext,
    //     ClaimsPrincipal claimsPrincipal, CodeVerifier codeVerifier) {
    //     var username = claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Sub)!.Value;
    //
    //     var result = codeVerifier.VerifyCode(username, code);
    //     if (!result) return Results.Unauthorized();
    //
    //     var user = dbContext.Users.First(u => u.Username == username);
    //     user.IsPhoneVerified = true;
    //     await dbContext.SaveChangesAsync();
    //
    //     return Results.Ok(new {
    //         message = "Verification successful!"
    //     });
    // }
}
