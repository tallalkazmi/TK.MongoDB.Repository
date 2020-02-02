# TK.MongoDB.Repository
Repository pattern implementation *(using linq)* of MongoDB in .NET Framework

## Usage
#### Settings

1. Default `expireAfterSeconds` is set to 2592000 seconds or 30 days, but you can configure this by calling a static method as below:

   ```c#
   Settings.Configure(2592000);
   ```

2. Default `ConnectionStringSettingName` is set to "*MongoDocConnection*", but you can configure this by calling a static method as below:

   ```c#
   Settings.Configure(connectionStringSettingName: "MongoDocConnection");
   ```

3. You can also set both of the settings as below:

   ```c#
   Settings.Configure(2592000, "MongoDocConnection");
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

1. Find (by Linq Expression)

   ```c#
   Repository<Activity> repository = new Repository<Activity>();
   Activity result = repository.FindAsync(x => x.Id == new ObjectId("5e36997898d2c15a400f8968")).Result;
   ```

2. Get (by Id)

   ```c#
   Repository<Activity> repository = new Repository<Activity>();
   Activity result = repository.GetAsync(new ObjectId("5e36997898d2c15a400f8968")).Result;
   ```

3. Get (by Linq Expression)

   ```c#
   Repository<Activity> repository = new Repository<Activity>();
   var result = repository.GetAsync(1, 10, x => x.Name.Contains("abc") && x.Deleted == false).Result;
   ```
   
4. Insert

   ```c#
   Activity activity = new Activity()
   {
       Name = "abc"
   };
   
   Repository<Activity> repository = new Repository<Activity>();
   Activity result = repository.InsertAsync(activity).Result;
   ```

5. Update

   ```c#
   Activity activity = new Activity()
   {
   	Id = new ObjectId("5e36998998d2c1540ca23894"),
   	Name = "abc3"
   };
   
   Repository<Activity> repository = new Repository<Activity>();
   bool result = repository.UpdateAsync(activity).Result;
   ```

6. Delete (by Id)

   ```c#
   Repository<Activity> repository = new Repository<Activity>();
   bool result = repository.DeleteAsync(new ObjectId("5e36998998d2c1540ca23894")).Result;
   ```

7. Count

   ```c#
   Repository<Activity> repository = new Repository<Activity>();
   long result = repository.CountAsync().Result;
   ```

8. Exists (by Linq Expression)

   ```c#
   Repository<Activity> repository = new Repository<Activity>();
   bool result = repository.ExistsAsync(x => x.Name == "abc").Result;
   ```

#### Tests

Refer to **TK.MongoDB.Test** project for all Unit Tests.