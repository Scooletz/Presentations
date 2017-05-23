## Azure Service Fabric 

### Szymon Kulec

[@Scooletz](https://twitter.com/Scooletz)

[https://blog.scooletz.com](https://blog.scooletz.com)

NOTE: What's next technology? What's next for you, for your managers, for your company to build successful products?

---

### Foundation 2.0

NOTE: Service Fabric is Azure Foundation. Proven in battle. Delivering for many services. Current public version is v2.0

--

### Service related problems

- Where should I deploy my service?
- I need more RAM on server 5?
- Why do I need to run 2 VMs for each of my services?

--

### Service related solutions

- hosts services on some nodes
- scale up easily
- saturate your one cluster with all your apps

---

### Service Fabric topology

--

### Service Fabric topology

- Auth:
 - Service A
 - Service B

NOTE: application topology, what app is, what service is

--

### Service Fabric topology

- Auth [Version=1.1]:
 - Service A [Version=1.2]
 - Service B [Version=1.0]

NOTE: versions just strings, app, services may be different

--

### Image Service, Application Types

- Auth 1.0.0
- Auth 1.1.0
- Billing 2.0
- Billing 2.1
- Billing 3.0
- ...

NOTE: image service contains app packages

--

### Deployment

|Logical item|Uri|
|:---:|:---:|
|Auth 1.0.0|fabric:/AuthApp|
|Auth 1.0.0 / Service A|fabric:/AuthApp/A|
|Auth 1.0.0 / Service B|fabric:/AuthApp/B|

--

### Deployment

- AuthPL
 - Auth 1.1
  - Service A 1.2
  - Service B 1.1
- AuthES
 - Auth 1.0
  - Service A 1.2
  - Service B 1.0

NOTE: all versions of apps deployed

--

### Physical deployment

|Node1|Node2|Node3|
|:---:|:---:|:---:|
|AuthPL/A|AuthPL/B|AuthPL/B|
|AuthPL/B|AuthES/B|AuthES/B|
|AuthES/B|AuthES/A|   |

--

### Deployment

- many possibilities for scaling
- many possibilities for creating custom topologies
- app as an isolation unit
- when upgrading: versions checked

--

### Service Fabric topology summary

- application (versioned) -> service (versioned)
- same app type may be deployed many times 
- app as scaling unit

---

### Service Fabric services

--

### Service Fabric services

- guest (node, php, exec, docker)
- stateless
- stateful
- actors

--

### Guest services

- any kind of (.exe)
- n copies
- communication
- more love for Docker is coming!

--

### Stateless services

- WebAPI, 
- business logic
- no internal state 
- might use external services like storage
- every copy is the same

--

### Stateful services

- partitioned
- persistent
- reliable collections: 
 - a queue
 - a dictionary
- automatic replication
- good for storing metadata, sessions, hot data

--

### Stateful services - replicas

- Service
 - Partition 1
   - Replica 1.1
   - Replica 1.2
   - Replica 1.3
 - Partition 2
   - Replica 2.1
   - Replica 2.2
   - Replica 2.3
 - Partition 3
   - Replica 3.1
   - Replica 3.2
   - Replica 3.3

--

### Stateful services - replicas

|Node1|Node2|Node3|
|:---:|:---:|:---:|
|1.1|2.2|2.1|
|3.1|1.2|3.2|
|2.3|3.3|1.3|

NOTE: service partitions and replicas

--

### Stateful services - replicas

- by default same service replicas = same process
- replica Uri `fabric:/App/Service/Partition/Replica`
- replicas
 - primary
 - active secondaries
 - "chasing" secondaries

--

### Reliable actors

- persistent / volatile
- partitioned
- ActorId
- State[]
- Reminders
- no orchestration/hierarchy

--

### Reliable actors

```
var id = ActorId.CreateRandom();

var actor = ActorProxy.Create<IMyActor>(id, 
	new Uri("fabric:/MyApp/ActorService"));

await myActor.DoWorkAsync();
```

--

### Service Fabric services summary

App structure:

- Application
  - Service
    - Partition (actors here)
      - Replica

--

### Service Fabric services examples

- Amazon basket
- http session

---

### Service communication

--

### Service communication

```
public interface ICommunicationListener
{
    Task<string> OpenAsync(CancellationToken cancellationToken);

    Task CloseAsync(CancellationToken cancellationToken);

    void Abort();
}
```

--

### Service communication

```
Uri serviceUri = "fabric:/AuthPL";

var client = ServiceProxy.Create<IAuthService> (serviceUri, 
				partitionSelector);

client.Authorize(...);
```

---

### What's next

--

### What's next - docker

- persistent image (partition)
- replication and HA

--

### What's next - Azure API

`yourdomain.com/user/{id}`
`partition-{id}`

--

### What's next - Azure

domain and certificates management

---

### Summary

- simple & powerful conceptual model
- App/Service/Partition/Replica
- guest/stateless/statefull/actor
- Azure-ready, on-premises-ready

---

## Questions and partitioned answers
### Szymon Kulec

[@Scooletz](https://twitter.com/Scooletz)

[https://blog.scooletz.com](https://blog.scooletz.com)