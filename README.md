# TK.MongoDB.Repository
[![Nuget](https://img.shields.io/nuget/v/TK.MongoDB.Repository)](https://www.nuget.org/packages/TK.MongoDB.Repository) 
[![Nuget](https://img.shields.io/nuget/dt/TK.MongoDB.Repository)](https://www.nuget.org/packages/TK.MongoDB.Repository)
![Azure DevOps builds](https://img.shields.io/azure-devops/build/tallalkazmi/79c589e2-20be-4ad6-9b5a-90be5ddc7916/2) 
![Azure DevOps tests](https://img.shields.io/azure-devops/tests/tallalkazmi/79c589e2-20be-4ad6-9b5a-90be5ddc7916/2) 
![Azure DevOps releases](https://img.shields.io/azure-devops/release/tallalkazmi/79c589e2-20be-4ad6-9b5a-90be5ddc7916/2/2)

Repository pattern implementation *(using linq)* of MongoDB in .NET Framework with optional Dependency Tracking implementation.

## Usage
#### Settings

1. Default `ConnectionStringSettingName` is set to "*MongoDocConnection*", but you can configure this by calling a static method as below:

    ```c#
    Settings.ConnectionStringSettingName = "MongoDocConnection";
    ```

2. You can configure `ExpiryAfterSeconds` index for a specific collection by calling a static method as below:

    ```c#
    Settings.Configure<Activity>(2592000);
    ```

3. If you intend to use **Dependency Tracking**, you can specify commands to <u>not</u> track by setting the *NotTrackedCommands* `IEnumerable<string>`:

   ```c#
   Settings.NotTrackedCommands = new List<string>() { "isMaster", "buildInfo", "getLastError", "saslStart", "saslContinue" };
   ```

4. Example:

    ```c#
    public class RepoUnitTest
    {
        public RepoUnitTest()
        {
            Settings.ConnectionStringSettingName = "MongoDocConnection";
            Settings.Configure<Activity>(2592000);
        }

        //.... other methods and properties
    }
    ```

#### Models

Create a document model implementing $BaseEntity$ to use in repository. The name of this model will be used as collection name in MongoDB.

```c#
public class Activity : BaseFile
{
    public string Name { get; set; }
}
```

#### Repository methods

1. Find *asynchronous* (using Linq Expression.)

   ```c#
   Activity result = ActivityRepository.FindAsync(x => x.Id == new ObjectId("5e36997898d2c15a400f8968")).Result;
   ```
   
2. Get *asynchronous* (by Id)

   ```c#
   Activity result = ActivityRepository.GetAsync(new ObjectId("5e36997898d2c15a400f8968")).Result;
   ```
   
3. Get *asynchronous* (using Linq Expression.)

   Has paged records in a `Tuple<IEnumerable<T>, long>` of records and total count.
   
   ```c#
   var result = ActivityRepository.GetAsync(1, 10, x => x.Name.Contains("abc") && x.Deleted == false).Result;
   Console.WriteLine($"Output:\nTotal: {result.Item2}\n{JToken.Parse(JsonConvert.SerializeObject(result.Item1)).ToString(Formatting.Indented)}");
   ```
   
4. Get *synchronous* (using Linq Expression.)

   Has nonpaged records.

   ```c#
   var result = ActivityRepository.Get(x => x.Name.Contains("abc") && x.Deleted == false);
   ```

5. In *synchronous*  (Get by `IN` filter. *Nonpaged records*)

   ```c#
   List<string> names = new List<string> { "abc", "def", "ghi" };
   var result = ActivityRepository.In(x => x.Name, names);
   ```

6. Insert *asynchronous* 

   ```c#
   Activity activity = new Activity()
   {
       Name = "abc"
   };
   
   Activity result = ActivityRepository.InsertAsync(activity).Result;
   ```

7. Update *asynchronous* 

   ```c#
   Activity activity = new Activity()
   {
   	Id = new ObjectId("5e36998998d2c1540ca23894"),
   	Name = "abc3"
   };
   
   bool result = ActivityRepository.UpdateAsync(activity).Result;
   ```

8. Delete *asynchronous* (by Id)

   ```c#
   bool result = ActivityRepository.DeleteAsync(new ObjectId("5e36998998d2c1540ca23894")).Result;
   ```

9. Count *asynchronous* 

   ```c#
   long result = ActivityRepository.CountAsync().Result;
   ```

10. Exists *asynchronous* (using Linq Expression)

   ```c#
   bool result = ActivityRepository.ExistsAsync(x => x.Name == "abc").Result;
   ```

#### Dependency Tracking

To use dependency tracking implement the `IDependencyTracker` interface as below:

```c#
public class DependencyTracker : IDependencyTracker
{
	public void Dependency(string name, string description, bool success, TimeSpan duration)
    {
    	Console.WriteLine($"{name}-{description}-{success}-{duration}");
    }
}
```

#### Tests

Refer to **TK.MongoDB.Test** project for all Unit Tests.
