// Listing 6.6
namespace Foo

type Order = { Name: string }

namespace Bar.Baz

type Customer = { Name: string; LastOrder: Foo.Order }

// Listing 6.7
open Foo
type CustomerTwo = { LastOrder: Order }
