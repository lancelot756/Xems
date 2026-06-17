using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xems.Api;

namespace Xems.Tests.Api;

public class AuthTests : IClassFixture<WebApplicationFactory<Program>>
{
	private readonly HttpClient _client;

	public AuthTests(WebApplicationFactory<Program> factory)
	{
		_client = factory.CreateClient();
	}

	[Fact]
	public async Task GetElevators_WithoutToken_ShouldReturnUnauthorized()
	{
		var response = await _client.GetAsync("/api/v1/elevators");
		Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
	}

	[Fact]
	public async Task Maintenance_WithGuestToken_ShouldReturnForbidden()
	{
		var token = await GetToken("guest", "guest123");

		_client.DefaultRequestHeaders.Authorization =
				new AuthenticationHeaderValue("Bearer", token);

		var response = await _client.PostAsJsonAsync(
				"/api/v1/elevators/A1/maintenance",
				new { enabled = true });

		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}

	private async Task<string> GetToken(string username, string password)
	{
		var response = await _client.PostAsJsonAsync("/auth/token", new
		{
			username,
			password
		});

		response.EnsureSuccessStatusCode();

		var body = await response.Content.ReadFromJsonAsync<TokenResponse>();

		return body!.AccessToken;
	}

	private class TokenResponse
	{
		public string AccessToken { get; set; } = string.Empty;
	}
}