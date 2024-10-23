using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;
using System.Web;

public class OAuthController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public OAuthController(IConfiguration configuration, HttpClient httpClient)
    {
        _configuration = configuration;
        _httpClient = httpClient;
    }

    [HttpGet("authorize")]
    public IActionResult Authorize()
    {
        string authorizationEndpoint = _configuration["OAuth:AuthorizationEndpoint"];
        string clientId = _configuration["OAuth:ClientId"];
        string redirectUri = _configuration["OAuth:RedirectUri"];
        string scope = _configuration["OAuth:Scope"];

        // Generate PKCE code verifier and challenge
        string codeVerifier = PkceHelper.GenerateCodeVerifier();
        string codeChallenge = PkceHelper.GenerateCodeChallenge(codeVerifier);
        string state = Guid.NewGuid().ToString(); // State to prevent CSRF attacks

        // Save the code verifier in session for later use
        HttpContext.Session.SetString("code_verifier", codeVerifier);
        HttpContext.Session.SetString("state", state);

        // Construct the authorization URL
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["client_id"] = clientId;
        query["response_type"] = "code";
        query["redirect_uri"] = redirectUri;
        query["scope"] = scope;
        query["code_challenge"] = codeChallenge;
        query["code_challenge_method"] = "S256";
        query["state"] = state;

        string authorizationUrl = $"{authorizationEndpoint}?{query.ToString()}";

        // Redirect the user to the authorization server
        return Redirect(authorizationUrl);
    }

    [HttpGet("oauth/callback")]
    public async Task<IActionResult> Callback(string code, string state)
    {
        // Validate state to prevent CSRF attacks
        var storedState = HttpContext.Session.GetString("state");
        if (state != storedState)
        {
            return BadRequest("Invalid state parameter");
        }

        // Retrieve the code verifier from session
        var codeVerifier = HttpContext.Session.GetString("code_verifier");

        // Exchange authorization code for access token
        var tokenEndpoint = _configuration["OAuth:TokenEndpoint"];
        var clientId = _configuration["OAuth:ClientId"];
        var redirectUri = _configuration["OAuth:RedirectUri"];

        var requestBody = new StringContent(
            $"grant_type=authorization_code&code={code}&redirect_uri={redirectUri}&client_id={clientId}&code_verifier={codeVerifier}",
            Encoding.UTF8, "application/x-www-form-urlencoded"
        );

        var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint)
        {
            Content = requestBody
        };

        var response = await _httpClient.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseContent);
            // Store the tokens (access_token, id_token, refresh_token)
            HttpContext.Session.SetString("access_token", tokenResponse.AccessToken);
            return Ok(tokenResponse);
        }

        return BadRequest("Token exchange failed");
    }

    public class TokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("id_token")]
        public string IdToken { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
    }
}
