// For more information see https://aka.ms/fsharp-console-apps
printfn "Hello from F#"

open CsCode
let c1 = Person()
printfn $"{c1.Name} is {c1.Age} years old!"
