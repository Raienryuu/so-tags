using SO_tagsTests;



class FakeHttpClient(HttpMessageHandler messageHandler) : HttpClient(messageHandler) 
{
  public new async Task<HttpResponseMessage> GetAsync(string? s) {
    var expectedResponseContent = new StringContent(Stubs.StackExchangeResponse, System.Text.Encoding.UTF8, "application/json");
    return new HttpResponseMessage() { Content = expectedResponseContent };
  }
}

class FakeHttpMessageHandler() : HttpMessageHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var expectedResponseContent = new StringContent(Stubs.StackExchangeResponse, System.Text.Encoding.UTF8, "application/json");
        return new HttpResponseMessage() { Content = expectedResponseContent };
    }
}