// Consuming Tasks
open System.IO
open System.Threading.Tasks

File.WriteAllText("foo.txt", "Hello, world")
let text: string = File.ReadAllText "foo.txt"
let textAsync: Task<string> = File.ReadAllTextAsync "foo.txt"

let theText: string = textAsync.Result

// Exercise 12.1
let thisWontExecute = File.ReadAllText "doesNotExist.txt"
let thisReturnsAFaultedTask = File.ReadAllTextAsync "doesNotExist.txt"

let theTask = Task.Delay(10000)
theTask.Status // WaitingForActivation for the first ten seconds
theTask.Wait() // Blocks FSI until 10 seconds are up
theTask.Status // RanToCompletion

// The task block
let writeToFile fileName data =
    System.IO.File.AppendAllText(fileName, data)
    let data = System.IO.File.ReadAllText fileName
    data.Length

let total = writeToFile "sample.txt" "foo"

// Listing 12.1
let writeToFileAsync fileName data = task {
    do! System.IO.File.AppendAllTextAsync(fileName, data)
    let! data = System.IO.File.ReadAllTextAsync fileName
    return data.Length
}

let totalTask = writeToFileAsync "sample.txt" "foo"
// Listing 12.2

let filesComposed = task {
    let! file1 = System.IO.File.ReadAllTextAsync(__SOURCE_DIRECTORY__ + "/file1.txt")
    let! file2 = System.IO.File.ReadAllTextAsync(__SOURCE_DIRECTORY__ + "/file2.txt")
    let! file3 = System.IO.File.ReadAllTextAsync(__SOURCE_DIRECTORY__ + "/file3.txt")
    return $"{file1} {file2} {file3}"
}

filesComposed.Result

// Listing 12.3
let filesParallel = task {
    let! allFiles =
        [ "file1.txt"; "file2.txt"; "file3.txt" ]
        |> List.map (fun fileName -> __SOURCE_DIRECTORY__ + "/" + fileName)
        |> List.map File.ReadAllTextAsync
        |> Task.WhenAll

    return allFiles |> Array.reduce (sprintf "%s %s")
}

filesParallel.Result

// Listing 12.4
let writeToFileAsyncMix fileName data : Task<int> =
    printfn "1. This is happening synchronously!"
    Task.Delay(1000).Wait()
    printfn "2. Kicking off the background work!"

    let result = task {
        do! System.IO.File.AppendAllTextAsync(fileName, data)
        do! Task.Delay(1000)
        printfn "4. This is happening asychronously!"
        let! data = System.IO.File.ReadAllTextAsync fileName
        return data.Length
    }

    printfn "3. Doing something more, now let's return the task"
    result

let theResult = writeToFileAsyncMix "sample.txt" "foo"

// Listing 12.5
let writeToFileAsyncBlock fileName data = async {
    do! System.IO.File.AppendAllTextAsync(fileName, data) |> Async.AwaitTask
    let! data = System.IO.File.ReadAllTextAsync fileName |> Async.AwaitTask
    return data.Length
}

let asyncBlockResult = writeToFileAsyncBlock "sample.txt" "foo"
asyncBlockResult |> Async.RunSynchronously

// Listing 12.6
open System.Text.Json
let loadCustomerFromDb customerId = {| Name = "Isaac"; Balance = 0 |}

let tryGetCustomer customerId =
    let customer = loadCustomerFromDb customerId

    if customer.Balance <= 0 then
        Error "Customer is in debt!"
    else
        Ok customer

let handleRequest (json: string) =
    let request: {| CustomerId: int |} = JsonSerializer.Deserialize json
    let response = tryGetCustomer request.CustomerId

    match response with
    | Ok c -> {| CustomerName = c.Name.ToUpper() |}
    | Error msg -> failwith $"Bad request: {msg}"

handleRequest ({| CustomerId = 1 |} |> JsonSerializer.Serialize)

// Listing 12.7
let loadCustomerFromDbAsync customerId = task { return {| Name = "Isaac"; Balance = 0 |} }

let tryGetCustomerAsync customerId = task {
    let! customer = loadCustomerFromDbAsync customerId

    return
        if customer.Balance <= 0 then
            Error "Customer is in debt!"
        else
            Ok customer
}

let handleRequestAsync (json: string) = task {
    let request: {| CustomerId: int |} = JsonSerializer.Deserialize json
    let! response = tryGetCustomerAsync request.CustomerId

    return
        match response with
        | Ok c -> {| CustomerName = c.Name.ToUpper() |}
        | Error msg -> failwith $"Bad request: {msg}"
}
