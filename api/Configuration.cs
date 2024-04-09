using Microsoft.Extensions.Primitives;

namespace SO_tags;

public class Configuration : IConfiguration
{
  private Dictionary<string, string> configValues = new();

  public IEnumerable<IConfigurationSection> GetChildren()
  {
    throw new NotImplementedException();
  }

  public IChangeToken GetReloadToken()
  {
    throw new NotImplementedException();
  }

  public IConfigurationSection GetSection(string key)
  {
    throw new NotImplementedException();
  }

  public string? this[string key]
  {
    get => throw new NotImplementedException();
    set => throw new NotImplementedException();
  }
}