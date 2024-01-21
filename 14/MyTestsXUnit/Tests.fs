module Tests

open System
open Xunit

[<Fact>]
let ``My guitar gently weeps`` () =
    let input = Guitar
    let output = play input
    Assert.Equal ("Wah wah", output)

[<Fact>]
let ``Drums go bang`` () =
    let input = Drums
    let output = play input
    Assert.Equal ("Ba dum tss", output)