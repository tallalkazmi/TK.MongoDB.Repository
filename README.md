# TK.MongoDB.Repository
Repository pattern implementation *(using linq)* of MongoDB in .NET Framework

## Usage
#### Settings

2. Default `ConnectionStringSettingName` is set to "*MongoDocConnection*", but you can configure this by calling a static method as below:

   ```c#
   Settings.Configure("MongoDocConnection");
   ```

3. you can configure `ExpiryAfterSeconds` index for a specific collection by calling a static method as below:

   ```c#
   Settings.Configure<Activity>(2592000);
   ```

4. Example:

   ```c#
   public class RepoUnitTest
   {
       Repository<Activity> ActivityRepository;
       public RepoUnitTest()
       {
           Settings.Configure("MongoDocConnection");
           Settings.Configure<Activity>(2592000);
           ActivityRepository = new Repository<Activity>();
       }
   
   	//.... other methods and properties
   }
   ```

#### Models

Create a document model implementing $BaseEntity$ to use in repository. The name of this model will be used as collection name in MongoDB.

```c#
public class Activity : BaseFile<ObjectId>
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

#### Tests

Refer to **TK.MongoDB.Test** project for all Unit Tests.