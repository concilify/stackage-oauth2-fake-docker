namespace Stackage.OAuth2.Fake.OutsideIn.Tests.Scenarios.Internal.Users;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;
using Stackage.OAuth2.Fake.OutsideIn.Tests.Scenarios.Internal.Users.Model;

// ReSharper disable once InconsistentNaming
public class get_happy_path
{
   private string? _subject;
   private HttpResponseMessage? _httpResponse;

   [OneTimeSetUp]
   public async Task setup_before_all_tests()
   {
      using var httpClient = new HttpClient();
      httpClient.BaseAddress = new Uri(Configuration.AppUrl);

      _subject = Guid.NewGuid().ToString();

      var body = new
      {
         subject = _subject,
         claims = new
         {
            claim_a = "claim-a",
            claim_b = "claim-b"
         }
      };

      var content = JsonContent.Create(body);

      await httpClient.PostAsync(".internal/users", content);

      _httpResponse = await httpClient.GetAsync($".internal/users?subject={_subject}");
   }

   [Test]
   public void response_status_should_be_okay()
   {
      Assert.That(_httpResponse?.StatusCode, Is.EqualTo(HttpStatusCode.OK));
   }

   [Test]
   public async Task response_content_should_contain_single_user()
   {
      var usersResponse = await _httpResponse!.ParseAsync<UsersResponse>();

      Assert.That(usersResponse.Count, Is.EqualTo(1));
   }

   [Test]
   public async Task response_content_should_contain_subject()
   {
      var usersResponse = await _httpResponse!.ParseAsync<UsersResponse>();

      Assert.That(usersResponse[0].Subject, Is.EqualTo(_subject));
   }

   [Test]
   public async Task response_content_should_contain_claims()
   {
      var usersResponse = await _httpResponse!.ParseAsync<UsersResponse>();

      var expectedClaims = new Dictionary<string, string>
      {
         ["claim_a"] = "claim-a",
         ["claim_b"] = "claim-b"
      };

      usersResponse[0].Claims.ShouldBeEquivalentTo(expectedClaims);
   }
}
