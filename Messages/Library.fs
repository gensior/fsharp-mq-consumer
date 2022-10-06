namespace Messages

type OnboardNewCustomer =
    { Name: string
      Email: string }
    static member For name email = { Name = name; Email = email }

type CreateCustomerAccount =
    { Name: string
      Email: string }
    static member For name email = { Name = name; Email = email }

type CustomerAccountCreated =
    { Email: string
      CustomerId: int }
    static member For email customerId =
        { Email = email
          CustomerId = customerId }

type SendWelcomeEmail =
    { CustomerId: int }
    static member For customerId = { CustomerId = customerId }

type WelcomeEmailSent =
    { CustomerId: int }
    static member For customerId = { CustomerId = customerId }

type ScheduleSalesCall =
    { CustomerId: int }
    static member For customerId = { CustomerId = customerId }

type SalesCallScheduled =
    { CustomerId: int }
    static member For customerId = { CustomerId = customerId }
