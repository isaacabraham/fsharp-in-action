type RawCustomer = {
    CustomerId: string
    Name: string
    Street: string
    City: string
    Country: string
    AccountBalance: decimal
}

// Listing 9.1
type CustomerId = CustomerId of int
type Name = Name of string
type Street = Street of string
type City = City of string

type Country =
    | Domestic
    | Foreign of string

type AccountBalance = AccountBalance of decimal

type Customer = {
    Id: CustomerId
    Name: Name
    Address: {|
        Street: Street
        City: City
        Country: Country
    |}
    Balance: AccountBalance
}

// Listing 9.2
let validateCustomer (rawCustomer: RawCustomer) =
    let customerId =
        if rawCustomer.CustomerId.StartsWith "C" then
            Ok(CustomerId(int rawCustomer.CustomerId[1..]))
        else
            Error $"Invalid Customer Id '{rawCustomer.CustomerId}'."

    let country =
        match rawCustomer.Country with
        | "" -> Error "No country supplied"
        | "USA" -> Ok Domestic
        | other -> Ok(Foreign other)

    match customerId, country with
    | Ok customerId, Ok country ->
        Ok {
            Id = customerId
            Name = Name rawCustomer.Name
            Address = {|
                Street = Street rawCustomer.Street
                City = City rawCustomer.City
                Country = country
            |}
            Balance = AccountBalance rawCustomer.AccountBalance
        }
    | Error err, _
    | _, Error err -> Error err

let customerA = {
    CustomerId = "C123"
    Name = "Isaac"
    Street = "123 Main St"
    City = "Anytown"
    Country = "USA"
    AccountBalance = 123.45m
}

let validatedC1 = validateCustomer customerA

let customerB = {
    CustomerId = "123" // Bad customer id
    Name = "   "
    Street = "123 Main St"
    City = "Anytown"
    Country = "" // No country supplied
    AccountBalance = 123.45m
}

let validatedC2 = validateCustomer customerB

// Listing 9.3
let validateCustomerMultipleErrors (rawCustomer: RawCustomer) =
    let customerId =
        if rawCustomer.CustomerId.StartsWith "C" then
            Ok(CustomerId(int rawCustomer.CustomerId[1..]))
        else
            Error $"Invalid Customer Id '{rawCustomer.CustomerId}'."

    let country =
        match rawCustomer.Country with
        | "" -> Error "No country supplied"
        | "USA" -> Ok Domestic
        | other -> Ok(Foreign other)

    match customerId, country with
    | Ok customerId, Ok country ->
        Ok {
            Id = customerId
            Name = Name rawCustomer.Name
            Address = {|
                Street = Street rawCustomer.Street
                City = City rawCustomer.City
                Country = country
            |}
            Balance = AccountBalance rawCustomer.AccountBalance
        }
    | customerId, country ->
        Error [
            match customerId with
            | Ok _ -> ()
            | Error err -> err
            match country with
            | Ok _ -> ()
            | Error err -> err
        ]

let validatedC2MultiError = validateCustomerMultipleErrors customerB

// Strongly Typed Errors
type CustomerValidationError =
    | InvalidCustomerId of string
    | InvalidCountry of string

let validateCustomerStrongErrors (rawCustomer: RawCustomer) =
    let customerId =
        if rawCustomer.CustomerId.StartsWith "C" then
            Ok(CustomerId(int rawCustomer.CustomerId[1..]))
        else
            Error(InvalidCustomerId $"Invalid Customer Id '{rawCustomer.CustomerId}'.")

    let country =
        match rawCustomer.Country with
        | "" -> Error(InvalidCountry "No country supplied")
        | "USA" -> Ok Domestic
        | other -> Ok(Foreign other)

    match customerId, country with
    | Ok customerId, Ok country ->
        Ok {
            Id = customerId
            Name = Name rawCustomer.Name
            Address = {|
                Street = Street rawCustomer.Street
                City = City rawCustomer.City
                Country = country
            |}
            Balance = AccountBalance rawCustomer.AccountBalance
        }
    | customerId, country ->
        Error [
            match customerId with
            | Ok _ -> ()
            | Error err -> err
            match country with
            | Ok _ -> ()
            | Error err -> err
        ]

let validatedC2StrongError = validateCustomerStrongErrors customerB

// Listing 9.4
open System

module Validation =
    type CustomerValidationErrorRich =
        | EmptyCustomerId
        | InvalidCustomerIdFormat of string
        | NoNameSupplied
        | TooManyNameParts
        | NoCountrySupplied

    let validateCustomerId customerId =
        if String.IsNullOrWhiteSpace customerId then
            Error EmptyCustomerId
        elif customerId.StartsWith "C" then
            Ok(CustomerId(int customerId[1..]))
        else
            Error(InvalidCustomerIdFormat customerId)

    let validateCountry country =
        match country with
        | "" -> Error NoCountrySupplied
        | "USA" -> Ok Domestic
        | other -> Ok(Foreign other)

    let validateName name =
        if String.IsNullOrWhiteSpace name then
            Error NoNameSupplied
        elif name.Split ' ' |> Array.length > 2 then
            Error TooManyNameParts
        else
            Ok(Name name)

let validateCustomerFull (rawCustomer: RawCustomer) =
    let customerId =
        if String.IsNullOrWhiteSpace rawCustomer.CustomerId then
            Error Validation.EmptyCustomerId
        elif rawCustomer.CustomerId.StartsWith "C" then
            Ok(CustomerId(int rawCustomer.CustomerId[1..]))
        else
            Error(Validation.InvalidCustomerIdFormat rawCustomer.CustomerId)

    let country =
        match rawCustomer.Country with
        | "" -> Error Validation.NoCountrySupplied
        | "USA" -> Ok Domestic
        | other -> Ok(Foreign other)

    let name =
        if String.IsNullOrWhiteSpace rawCustomer.Name then
            Error Validation.NoNameSupplied
        elif rawCustomer.Name.Split ' ' |> Array.length > 2 then
            Error Validation.TooManyNameParts
        else
            Ok(Name rawCustomer.Name)

    match customerId, country, name with
    | Ok customerId, Ok country, Ok name ->
        Ok {
            Id = customerId
            Name = name
            Address = {|
                Street = Street rawCustomer.Street
                City = City rawCustomer.City
                Country = country
            |}
            Balance = AccountBalance rawCustomer.AccountBalance
        }
    | customerId, country, name ->
        Error [
            match customerId with
            | Ok _ -> ()
            | Error err -> err
            match country with
            | Ok _ -> ()
            | Error err -> err
            match name with
            | Ok _ -> ()
            | Error err -> err
        ]

let validatedC2Full = validateCustomerFull customerB

// Listing 9.5
module ValidationNested =
    type CustomerIdError =
        | EmptyId
        | InvalidIdFormat of string

    type NameError =
        | NoNameSupplied
        | TooManyParts

    type CountryError = | NoCountrySupplied

    let validateCustomerId customerId =
        if String.IsNullOrWhiteSpace customerId then
            Error EmptyId
        elif customerId.StartsWith "C" then
            Ok(CustomerId(int customerId[1..]))
        else
            Error(InvalidIdFormat customerId)

    let validateCountry country =
        match country with
        | "" -> Error NoCountrySupplied
        | "USA" -> Ok Domestic
        | other -> Ok(Foreign other)

    let validateName name =
        if String.IsNullOrWhiteSpace name then
            Error NoNameSupplied
        elif name.Split ' ' |> Array.length > 2 then
            Error TooManyParts
        else
            Ok(Name name)

type CustomerValidationErrorRoot =
    | CustomerIdError of ValidationNested.CustomerIdError
    | NameError of ValidationNested.NameError
    | CountryError of ValidationNested.CountryError

let validateCustomerNested rawCustomer =
    let customerId = ValidationNested.validateCustomerId rawCustomer.CustomerId
    let country = ValidationNested.validateCountry rawCustomer.Country
    let name = ValidationNested.validateName rawCustomer.Name

    match customerId, country, name with
    | Ok customerId, Ok country, Ok name ->
        Ok {
            Id = customerId
            Name = name
            Address = {|
                Street = Street rawCustomer.Street
                City = City rawCustomer.City
                Country = country
            |}
            Balance = AccountBalance rawCustomer.AccountBalance
        }
    | customerId, country, name ->
        Error [
            match customerId with
            | Ok _ -> ()
            | Error err -> CustomerIdError err
            match country with
            | Ok _ -> ()
            | Error err -> CountryError err
            match name with
            | Ok _ -> ()
            | Error err -> NameError err
        ]

let validatedC2Nested = validateCustomerNested customerB

// Exceptions
try
    Some(1 / 0)
with ex ->
    printfn $"Error: {ex.Message}"
    None

let handleException func arg =
    try
        func arg |> Ok
    with ex ->
        Error ex

let divide (a, b) = a / b
let divideSafe = handleException divide
let divideResult = divideSafe (2, 0)
