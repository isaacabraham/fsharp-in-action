open Expecto

// Listing 14.3
let musicTests =
    testList "Music tests" [
        test "My guitar gently weeps" {
            let input = Guitar
            let output = play input
            Expect.equal output "Wah wah" "Guitar sound is incorrect"
        }
        test "Drums go bang" {
            let input = Drums
            let output = play input
            Expect.equal output "Ba dum tss" "Drums are wrong"
        }
    ]

// Listing 14.4
module MultipleTests =
    let add a b = a + b

    let all =
        testList "My Tests" [
            if System.Environment.GetEnvironmentVariable "CI_SERVER" <> null then
                test "CI Only Test" { Expect.equal (add 1 2) 3 "Something is very wrong!" }
            for line in System.IO.File.ReadAllLines "testData.txt" do
                test $"Addition test: {line}" {
                    match line.Split ',' with
                    | [| inputA; inputB; expected |] ->
                        let actual = add (int inputA) (int inputB)
                        Expect.equal actual (int expected) "Calculator is broken"
                    | _ -> failwith $"Invalid test data format: {line}"
                }
        ]

module HigherOrderFunctions =
    open FsToolkit.ErrorHandling

    // Some stub functions for illustration
    let validateCustomer _ = Ok()
    let saveToSql _ _ = () // goes to real database - not testable!
    let calculatePrice _ _ = 0.0

    let createInvoice customer order = result {
        do! validateCustomer customer
        saveToSql customer order
        return calculatePrice customer order
    }

    let createInvoiceTestable save customer order = result {
        do! validateCustomer customer
        save customer order
        return calculatePrice customer order
    }

    let invoiceTest = test "Create Invoice" {
        let customer = "Bob"
        let order = "Guitar"
        let fakeSave customer order = () // stub function
        let price = createInvoiceTestable fakeSave customer order
        Expect.equal (Ok 100M) (Ok 50M) "Invoice is wrong"
    }

[<EntryPoint>]
let main args =
    runTestsWithCLIArgs [] args MultipleTests.all
