open Bogus
let faker = Faker()

let customers = [
    for i in 1..100 do
        {|
            EmployeeId = faker.Random.Guid()
            Name = faker.Name.FullName()
            Role = faker.PickRandom [ "Manager"; "Team Lead"; "Member" ]
            Department = faker.Commerce.Department()
            Address = {|
                Street = faker.Address.StreetAddress()
                State = faker.Address.State()
            |}
        |}
]

for customer in customers do
    printfn "%A" customer
