## Keep Its Storage Simple Stupid

### Szymon Kulec

[@Scooletz](https://twitter.com/Scooletz)

[https://blog.scooletz.com](https://blog.scooletz.com)

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

### Structure

HTTP APIs

- Azure storage service
- snapshot
- how to do sth
- snapshot

---

### Azure Storage

- foundation of Azure
- exists from the very beginning
- cheap, scalable
- partitioned

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
- Single partition can handle up to 2000 entities hits/s
- Optimistic concurrency (ETags)

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

### Tables (3)

#### Partitioning

Partition Key | Row Key | Name
--- | --- | ---
001 | Powers | Austin
002 | Borewicz | Sławomir
007 | Connery | Sean
008 | Brosnan | Pierce

Note: partitioning

--

### Tables (4)

#### Partitioning

Partition Key | Row Key | Name
--- | --- | ---
001 | Powers | Austin
002 | Borewicz | Sławomir
--- | --- | ---
007 | Connery | Sean
008 | Brosnan | Pierce

--

### Tables (5)

#### Partitioning

Partition Key | Row Key | Name
--- | --- | ---
001 | Powers | Austin
002 | Borewicz | Sławomir
--- | --- | ---
007 | Connery | Sean
008 | Brosnan | Pierce
009 | Craig | Daniel

--

### Querying Tables

1. PK = 'a' and RK = 'b'
1. PK = 'a' and RK > 'b'
1. PK > 'a' and RK > 'b'
1. Name = 'b'

--

### Tables - snapshot

- Subscription > Account > Table > Partition
- Partition throughput limit
- Batches for the same PartitionKey
- PartitionKey generation matters
- Query pattern matters

---

### Event Sourcing with Tables

- Aggregate is a transaction boundary
- Aggregate has state
- Events are appended
- Aggregate sometimes is called a stream

--

### Event Sourcing with Tables (2)

- Partition by aggregate
- Events as rows
- Aggregate as a row
- One table for all aggregates
- One storage account per Saas account

--

### Event Sourcing with Tables (3)

Partition Key | Row Key | Version | Type | Payload
--- | --- | --- | --- | ---
payment-713b49be | stream | 0 | |

--

### Event Sourcing with Tables (4)

Partition Key | Row Key | Version | Type | Payload
--- | ---: | --- | --- | ---
payment-713b49be | stream | 1 | |
payment-713b49be | 00000001 | 1 | Issued | 0x24234b2

--

### Event Sourcing with Tables (5)

Partition Key | Row Key | Version | Type | Payload
--- | ---: | --- | --- | ---
payment-713b49be | stream | 2 | |
payment-713b49be | 00000001 | 1 | Issued | 0x24234b23
payment-713b49be | 00000002 | 2 | Accepted | 0x452334

--

### Event Sourcing with Tables (6)

Partition Key | Row Key | Version | Type | Payload
--- | ---: | --- | --- | ---
payment-713b49be | stream | 2 | |
payment-713b49be | 00000001 | 1 | Issued | 0x24234b23
payment-713b49be | 00000002 | 2 | Accepted | 0x452334
payment-713b49be | ba890723 | 2 | Idempotent marker | 0x452334

Note: idempotent marker for Accepted

--

### Event Sourcing with Tables (7)

Initializing a stream
```
INSERT ('p', 'stream') {Version: '0'}
```

Appending an event
```
UPDATE ('p', 'stream') 
	SET Version = '1' WHERE ETag = 'sdfhsdfsd'

INSERT ('p', '00000001') 
	{Version: '1', Type: 'Issued', Payload: '0x24234b23'}
```

--

### Event Sourcing with Tables - snapshot

- Aggregate/stream = partition
- Events and aggregate version hold together
- Scalable up to the account limits

---

### Blobs

- page & block & append-only blobs
- custom metadata
- leases
- conditional operations

--

### Blobs (2)

- Get Blob - gets blob data (Range: might be applied)
- Lease Blob - start a lease
- Set/Get Metadata

--

### Block blobs

- Put Block - puts a block to a specified blob
- Put Block List - puts and commits the content of the blob, clearing uncommitted
- Get Block List - gets list of (un-) committed blocks
- Append Block - APPEND-ONLY BLOB, puts + put block list in one call

Note: Many small blocks = a lot of overhead

--

### Block blobs (2)

Example: upload files in 3 chunks

1. PUT BLOCK chunk1
1. PUT BLOCK chunk2
1. FAILURE
1. GET BLOCK LIST uncommitted -> {chunk1, chunk2}
1. PUT BLOCK chunk3
1. PUT BLOCK LIST chunk1, chunk2, chunk3
1. GET BLOCK LIST uncommitted -> {}
1. GET BLOCK LIST committed -> {chunk1, chunk2, chunk3}

--

### Page blobs

- sparse files for random IO
- page = 512 bytes
- pay as you go (only stored pages)
- used for VM disks
- now in Premium Storage as well

--

### Page blobs (2)

- Put Page - puts from 1 up to 8192 pages (4MB)
- Get Page Ranges - gets written page ranges in within a specific range

--

### Blobs - snapshot

- Page & Block - metadata, leases
- Page - for disks, random IO, sparse, offset based
- Block - for data, chunked in blocks

---

### Projecting with Blobs

- not only aggregates
- views combining data
- processing & reacting to events

--

### Projecting with Blobs (2)

Aggregate all payments by customer

```
public class PaymentsByCustomer : 
	ProjectionBase<PaymentsByCustomer.State>
{
    public class State
    {
        public decimal TotalPayments;
    }

    protected override void Map(IEventMapping<State> m)
    {
        m.For((PaymentIssued e) => e.Event.UserId, 
        	(e, s) => s.TotalPayments += e.Event.Amount);
    }
}
```

--

### Projecting with Blobs (3)

Top paying customer per day

```
public class TopCustomerPerDay : 
	ProjectionBase<TopCustomerPerDay.State>
{
    public class State
    {
        public decimal MaximumAmountSoFar;
        public UserId UserId;
    }

    protected override void Map(IEventMapping<State> m)
    {
        m.For(e => e.Metadata.DateTime.Date, Apply);
    }

    void Apply (PaymentIssued e, State s)
    {
    	if (e.Event.Amount > s.MaximumAmountSoFar)
		{
			s.MaximumAmoundSoFar = e.Event.Amount;
			s.UserId = e.Event.UserId;
		}
    }
}
```

--

### Projecting with Blobs (4)

- making this projections/processes easy requires a log
- the log contains ordered pointers to all events in an account
- an index is required
- if we had an index entry that fits in 512 bytes (page)...
- we could also use smaller (64 bytes) and store multiple per page

--

### Projecting with Blobs (5)

|0-63|64-127|128-191|192-255|256-319|320-383|384-447|448-511|
|---|---|---|---|---|---|---|---|
|entry0|entry1|entry2|entry3|entry4|entry5|entry6|entry7|

Note: mention Event Store

--

### Projecting with Blobs (6)

```
[StructLayout(LayoutKind.Explicit, Size = 64)]
struct IndexEntry
{
    public const int EntriesPerPage = 8;

    [FieldOffset(0) ] public Sha1 Stream;
    [FieldOffset(20)] public int Version;
    [FieldOffset(24)] public Guid EventTypeId;
    [FieldOffset(40)] public Guid MetadataId;
}
```

--

### Projecting with Blobs (7)

- a single writer per account, scanning changes (*advanced)
- using a lease to lock
- index file = page blob
- scale up - multiple blobs with ranges: 1-10000, 10001- 20001
- readers Get Blob content

---

### Queues

- 2000 msg/s (queue = partition)
- no exchanges, simple Queue/Dequeue operations
- VisibilityTimeout
- explicit delete
- at-least-once, at-most-once

---

### Summary

- Azure Storage provides powerful, scalable, simple services
- you need to know how to use them well
- think of boundaries and guarantees you need to provide
- remember: partitioning is the key

---

## Questions and (possibly) answers
### Szymon Kulec

[@Scooletz](https://twitter.com/Scooletz)

[https://blog.scooletz.com](https://blog.scooletz.com)
