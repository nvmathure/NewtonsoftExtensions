# CloudNDevOps.Newtonsoft.Extensions

CloudNDevOps.Newtonsoft.Extensions is a library that extends functionality provided by Newtonsoft needed by common applications. One of such functionalities is ability to de-serialize collection of object families.

## Installation

Use the package manager [NuGet](https://www.nuget.org/) to install CloudNDevOps.Newtonsoft.Extensions.

```Package Manager Console
Install-Package CloudNDevOps.Newtonsoft.Extensions -Version 1.0.1-beta1
```

## Usage

### Define Types
``` C#
public class Customer
{
    public string Identifier { get; set; }

    public string Classifier { get; set; }
}

public class Individual : Customer
{
    public string FirstName { get; set; }

    public string LastName { get; set; }
}

public class Organization : Customer
{
    public string Name { get; set; }
}
```

### Create Instance of Converter
First parameter is function, which need to return value of classifier based on instance of object. Second parameter is dictionary of value of classifier and type of derived class.

```C#
var typeFamilyConverter = new TypeFamilyConverter<Customer, string>(
    new Func<Customer, string>(e => e.Classifier),
    new Dictionary<string, Type>
    {
        { "Individual", typeof(Individual) },
        { "Organization", typeof(Organization) }
    });
```
### De-Serialize using Newtonsoft JsonConvert Class
```C#
var result = JsonConvert.DeserializeObject<List<Customer>>(
    inputString,
    new JsonSerializerSettings
    {
        Converters = { typeFamilyConverter }
    });
```

## License
[MIT](https://choosealicense.com/licenses/mit/)
