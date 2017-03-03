## Rabbit ZooKeeper Extensions

The project uses the [Apache ZooKeeper .NET async Client](https://www.nuget.org/packages/ZooKeeperNetEx/) component, in addition to providing the basic zk operation, but also additional encapsulation of the commonly used functions in order to allow. Net developers to better use zookeeper.
## Features

1. session expired
2. Permanent watcher
3. Recursively delete nodes
4. Recursively create nodes
5. Cross-platform (support. Net core)

## Instructions for use
### Create connection

    IZookeeperClient client = new ZookeeperClient(new ZookeeperClientOptions("172.18.20.132:2181")
            {
                BasePath = "/", //default value
                ConnectionTimeout = TimeSpan.FromSeconds(10), //default value
                SessionTimeout = TimeSpan.FromSeconds(20), //default value
                OperatingTimeout = TimeSpan.FromSeconds(60), //default value
                ReadOnly = false, //default value
                SessionId = 0, //default value
                SessionPasswd = null //default value
                EnableEphemeralNodeRestore = true //default value
            });
### Create node
    var data = Encoding.UTF8.GetBytes("2016");
    
    //Fast create temporary nodes
    await client.CreateEphemeralAsync("/year", data);
    await client.CreateEphemeralAsync("/year", data, ZooDefs.Ids.OPEN_ACL_UNSAFE);
    
    //Fast create permanent nodes
    await client.CreatePersistentAsync("/year", data);
    await client.CreatePersistentAsync("/year", data, ZooDefs.Ids.OPEN_ACL_UNSAFE);
    
    //Full call
    await client.CreateAsync("/year", data, ZooDefs.Ids.OPEN_ACL_UNSAFE, CreateMode.PERSISTENT_SEQUENTIAL);
    
    //Recursively created
    await client.CreateRecursiveAsync("/microsoft/netcore/aspnet", data);
### Get node data
    IEnumerable<byte> data = await client.GetDataAsync("/year");
    Encoding.UTF8.GetString(data.ToArray());
### Get the child node
    IEnumerable<string> children= await client.GetChildrenAsync("/microsoft");
### Determine whether the node exists
    bool exists = await client.ExistsAsync("/year");
### Delete the node
    await client.DeleteAsync("/year");

    //Recursively deleted
    bool success = await client.DeleteRecursiveAsync("/microsoft");
### update data
    Stat stat = await client.SetDataAsync("/year", Encoding.UTF8.GetBytes("2017"));
### Subscription data changes
    await client.SubscribeDataChange("/year", (ct, args) =>
    {
        IEnumerable<byte> currentData = args.CurrentData;
        string path = args.Path;
        Watcher.Event.EventType eventType = args.Type;
        return Task.CompletedTask;
    });
### Subscription node changes
    await client.SubscribeChildrenChange("/microsoft", (ct, args) =>
    {
        IEnumerable<string> currentChildrens = args.CurrentChildrens;
        string path = args.Path;
        Watcher.Event.EventType eventType = args.Type;
        return Task.CompletedTask;
    });
## FAQ
### When will the "SubscribeDataChange" event be triggered?
The event subscribed by the "SubscribeDataChange" method is triggered in the following cases:

1. The node is created
2. The node is deleted
3. Node data changes
4. zk connection re-successful

### When will the "SubscribeChildrenChange" event be triggered?
The event subscribed by the "SubscribeChildrenChange" method is triggered in the following cases:

1. The node is created
2. The node is deleted
3. Node node changes
4. zk connection re-successful

### How do I distinguish the status of a node in the "xxxxChange" event?
In the event trigger parameter will have a type "EventType" attribute "Type", through this attribute can clearly distinguish the reasons for node changes.

### Why write this program, it and "ZooKeeperEx" What is the difference?
Officially provided components, only provide the basic api, in the normal use of the zk needs to do very complicated things, breed a lot of additional code and can not guarantee the correctness of its implementation.

In the java language also has the official zk package package ZKClient, the current component is also a reference to this project. What are the specific packages that are available? Please refer to the section "Features Provided".
