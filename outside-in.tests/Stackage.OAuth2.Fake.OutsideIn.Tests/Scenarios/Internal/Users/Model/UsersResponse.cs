namespace Stackage.OAuth2.Fake.OutsideIn.Tests.Scenarios.Internal.Users.Model;

using System.Collections.Generic;
using System.Text.Json.Serialization;

public class UsersResponse : List<UsersResponse.User>
{
   public record User(
      [property: JsonPropertyName("subject")] string Subject,
      [property: JsonPropertyName("claims")] IDictionary<string, string> Claims);
}
