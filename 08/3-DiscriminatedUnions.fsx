// Listing 8.8
type ContactMethod =
    | Email of address: string
    | Telephone of country: string * number: string
    | Post of
        {|
            Line1: string
            Line2: string
            City: string
            Country: string
        |}

let isaacEmail = Email "isaac@myemailaddress.com"
let isaacPhone = Telephone("UK", "020-8123-4567")

let isaacPost =
    Post {|
        Line1 = "1 The Street"
        Line2 = "The Town"
        City = "The City"
        Country = "UK"
    |}

type Customer = {
    Name: string
    Age: int
    ContactMethod: ContactMethod
}

let customer = {
    Name = "Isaac"
    Age = 30
    ContactMethod = isaacEmail
}

// Listing 8.9
let message = "Discriminated Unions FTW!"

let description =
    match customer.ContactMethod with
    | Email address -> $"Emailing '{message}' to {address}."
    | Telephone(country, number) -> $"Calling {country}-{number} with the message '{message}'!"
    | Post postDetails -> $"Printing a letter with contents '{message}' to {postDetails.Line1} {postDetails.City}..."

// Listing 8.10
type TelephoneNumber =
    | Local of number: string
    | International of countryCode: string * number: string

type ContactMethodV2 =
    | Email of address: string
    | Telephone of country: string * number: string
    | Post of
        {|
            Line1: string
            Line2: string
            City: string
            Country: string
        |}
    | Sms of TelephoneNumber

let smsContact = Sms(Local "123-4567")

type CustomerV2 = {
    Name: string
    Age: int
    ContactMethod: ContactMethodV2
}

let sendTo customer message =
    match customer.ContactMethod with
    | Sms(Local number) -> $"Texting local number {number}"
    | Email _
    | Telephone _
    | Post _ -> "Other"

// Exercise 8.4
type YearsAsCustomer =
    | LessThanAYear
    | OneYear
    | TwoYears
    | MoreThanTwoYears

type OverdraftStatus =
    | InCredit
    | Overdrawn

type LoanDecision =
    | LoanRejected
    | LoanAccepted

let canTakeOutALoan customerDetails =
    match customerDetails with
    | LessThanAYear, InCredit -> LoanRejected
    | LessThanAYear, Overdrawn -> LoanRejected
    | OneYear, InCredit -> LoanRejected
    | OneYear, Overdrawn -> LoanAccepted
    | TwoYears, Overdrawn -> LoanAccepted
    | TwoYears, InCredit -> LoanAccepted
    | MoreThanTwoYears, InCredit -> LoanAccepted
    | MoreThanTwoYears, Overdrawn -> LoanAccepted

// Listing 8.11
type PhoneNumber = PhoneNumber of string
type CountryCode = CountryCode of string

type TelephoneNumberV2 =
    | Local of PhoneNumber
    | International of CountryCode * PhoneNumber

let localNumber = Local(PhoneNumber "123-456")

let internationalNumber =
    let countryCode = CountryCode "+44"
    let phoneNumber = PhoneNumber "208-123-4567"
    International(countryCode, phoneNumber)

// Unwrapping single-case DUs
let foo (PhoneNumber number) = ()
let phoneNumber = PhoneNumber "208-123-4567"
foo phoneNumber
let (PhoneNumber number) = phoneNumber

// Enforcing invariants
type Email = Email of address: string
type ValidatedEmail = ValidatedEmail of Email

let validateEmail (Email address) =
    if address.Contains "@" then
        ValidatedEmail(Email address)
    else
        failwith "Invalid email"

let sendEmail (ValidatedEmail(Email address)) = ()
