namespace Stackage.OAuth2.Fake.OutsideIn.Tests.Scenarios.Internal.Users;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;
using Stackage.OAuth2.Fake.OutsideIn.Tests.Scenarios.Internal.Users.Model;

// ReSharper disable once InconsistentNaming
public class get_all_after_create
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
            claim_b = "claim-b",
         },
      };

      var content = JsonContent.Create(body);

      await httpClient.PostAsync(".internal/users", content);

      _httpResponse = await httpClient.GetAsync(".internal/users");
   }

   [Test]
   public void response_status_should_be_okay()
   {
      Assert.That(_httpResponse?.StatusCode, Is.EqualTo(HttpStatusCode.OK));
   }

   [Test]
   public async Task response_content_should_contain_users_from_configuration()
   {
      var usersResponse = await _httpResponse!.ParseAsync<UsersResponse>();

      var expectedUserA = new UsersResponse.User(
         Subject: "user-a-subject",
         Claims: new Dictionary<string, string>
         {
            ["nickname"] = "User A Nickname",
            ["picture"] = "user-a-picture",
         });

      var expectedUserB = new UsersResponse.User(
         Subject: "user-b-subject",
         Claims: new Dictionary<string, string>
         {
            ["nickname"] = "User B Nickname",
            ["picture"] = "user-b-picture",
         });

      var expectedNewUser = new UsersResponse.User(
         Subject: _subject!,
         Claims: new Dictionary<string, string>
         {
            ["claim_a"] = "claim-a",
            ["claim_b"] = "claim-b",
         });

      // Can't use ShouldContain as it doesn't check for equivalence
      usersResponse.Single(u => u.Subject == "user-a-subject").ShouldBeEquivalentTo(expectedUserA);
      usersResponse.Single(u => u.Subject == "user-b-subject").ShouldBeEquivalentTo(expectedUserB);
      usersResponse.Single(u => u.Subject == _subject).ShouldBeEquivalentTo(expectedNewUser);
   }
}
