// Listing 14.2

module Assert =
    let equal a b =
        if a <> b then
            failwith $"Value '{a}' does not equal '{b}'."

let myGuitarGentlyWeeps () =
    // Arrange
    let input = Guitar
    // Act
    let output = play input
    // Assert
    Assert.equal "Wah wah" output

let drumsGoBang () =
    // Arrange
    let input = Drums
    // Act
    let output = play input
    // Assert
    Assert.equal "Ba dam tss" output

// Exercise 14.1
let bassGoesBop () =
    // Arrange
    let input = Bass
    // Act
    let output = play input
    // Assert
    Assert.equal "Ba ba bom" output

let tests = [ myGuitarGentlyWeeps; drumsGoBang; bassGoesBop ]

printfn $"Running {tests.Length} tests..."

for test in tests do
    test ()

printfn "All tests passed!"
