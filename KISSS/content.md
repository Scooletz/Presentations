## Keep Its Storage Simple Stupid

### Szymon Kulec

[@Scooletz](https://twitter.com/Scooletz)

[https://blog.scooletz.com](https://blog.scooletz.com)

Note: a new page about... (trainings)

---

### Project C
- SaaS
- Scalable
- Secure (high isolation)
- A complex domain 
- A lot of interacting components
- Rich in processes

Note: Based on pre-alpha of the project

---

### Goals

- how to NoSQL
- how to nottransact
- how to azurestore
- how to eventsource

---

### Azure Storage

- foundation of Azure
- exists from the very beginning
- cheap, scalable 

--

### Azure Storage structure

- Azure subscription
- an account
- services

Note: account=database, partition=table on a different disk

--

### Azure Storage services

- Tables - NoSQL database
- Blobs - page & block blobs
- Queues - simple queues

--

### Tables

- (PartitionKey, RowKey) -> data
- Partition Key - partition
- Partition supports batches/transactions
- Single partition can handle up to 500 req/s
- Optimistic concurrency

--

### Tables (2)

This can be batched

```
INSERT ('Partition1', 'Row1') {name: 'test'}
INSERT ('Partition1', 'Row2') {name: 'test2'}
```

This can't

```
INSERT ('Partition1', 'Row1') {name: 'test'}
INSERT ('Partition2', 'Row2') {name: 'test2'}
```

--

### Event Sourcing with Tables

## External 2

Content 2.1

---

## External 3.1

Content 3.1

---

## External 3.2

Content 3.2