Have you ever written a service?
 'service' in a name
 WCF

Story 
 Of men
 Of domain
 Of software

Train transport (Turner train)
 Being on time
 Selling tickets
  compare to hotels and overbooking
  getting money from selling tickets, not actual users

A team (300 Sparta)
 Wants to replace a monolith
 DDD, CQRS, Microservices
 Bounded Contexts/Domain
  Who can distinguish ?
  Stick to a bubble
 Two bubbles: scheduling and selling
  infrequent scheduling 
  selling according to a schedule

First attempt (Adam & Eve)
 Separate into two services: Schedule, Ticketing
 Tickets gets information from Schedule, and sell
 Deployment & Outage (fail in schedule)
 What went good:
  A team can use microservice in CV now
 What went wrong:
  Hard dependency on the other system
  Services are not autonomous
 What could be fixed?
  Different interaction with modules
  Publish an event when Schedule changes
   By introducing a messaging framework
   By writing an event to a store in the same tx
   By using event sourcing
  React to the event in Ticketing
   Get data from Schedule
   Store it
   Use it
 Follow up
  Model autonomous services
  'Data on the Outside versus Data on the Inside'
  No atomicity across services. This is the boundary

Haven't I seen you here before? (Groundhog Day)
 A Team released the second version
 Fency HTTP Rest service calls to be accepted
 What went good:
  Providing an HTTP gateway
 What went wrong:
  Retries on failures ordering to many tickets
 What could be fixed?
  Pass identifier on a request
  In a tx store the id and apply the request
  No id - maybe it can be content based?

