using System.Text.Json.Serialization;

public class Token
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
    
    [JsonPropertyName("expired_at")]
    public DateTime ExpiredAt { get; set; }
}