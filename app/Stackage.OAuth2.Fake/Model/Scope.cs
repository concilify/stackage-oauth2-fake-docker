namespace Stackage.OAuth2.Fake.Model;

using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

public record Scope : IEnumerable<string>
{
   public static readonly Scope Empty = new(ImmutableSortedSet<string>.Empty);

   private readonly ImmutableSortedSet<string> _tokens;

   private Scope(ImmutableSortedSet<string> tokens)
   {
      _tokens = tokens;
   }

   public bool IsEmpty => _tokens.Count == 0;

   public bool Contains(string scope) => _tokens.Contains(scope);

   public IEnumerator<string> GetEnumerator() => _tokens.GetEnumerator();

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

   public static implicit operator string(Scope scope) => scope.ToString();

   [return: NotNullIfNotNull(nameof(scope))]
   public static explicit operator Scope?(string? scope) => scope != null ? new Scope(ParseTokens(scope)) : null;

   [return: NotNullIfNotNull(nameof(scopes))]
   public static explicit operator Scope?(string[]? scopes) => scopes != null ? new Scope(TrimTokens(scopes)) : null;

   public override string ToString() => string.Join(" ", _tokens);

   private static ImmutableSortedSet<string> TrimTokens(IEnumerable<string> scopes)
   {
      return scopes.Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)).ToImmutableSortedSet();
   }

   private static ImmutableSortedSet<string> ParseTokens(string scope) => TrimTokens(scope.Split(" "));
}
