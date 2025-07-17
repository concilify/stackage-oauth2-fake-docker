namespace Stackage.OAuth2.Fake.OutsideIn.Tests;

using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

public static class Support
{
   public static async Task<JsonNode> ParseContentAsJson(this HttpResponseMessage httpResponseMessage)
   {
      var jsonNode = JsonNode.Parse(await httpResponseMessage.Content.ReadAsStringAsync());

      if (jsonNode == null)
      {
         throw new JsonException("Failed to deserialize the response.");
      }

      return jsonNode;
   }
}
