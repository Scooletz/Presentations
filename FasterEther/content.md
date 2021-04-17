background-image: url(img/Ethereum.png)
background-size: cover

# Faster Ether

## a.k.a War Stories from optimizing Ethereum

## Szymon Kulec @Scooletz

---

background-image: url(img/flight-to-mars.jpg)
background-size: cover

## Discovering New Worlds

1. You select a new planet...

--

1. You pay for the fuel...

--

1. You ship starts...

--

1. Ethereum node suffers a memory leak 💥

--

This is what happened with **Dark Forest** game, which is

> an MMO space-conquest game where players discover and conquer planets in an infinite, procedurally-generated, cryptographically-specified universe!

---

background-image: url(img/flight-to-mars.jpg)
background-size: cover

## Discovering New Worlds - more

To solve the mystery, a usual setup was required:

1. having a **reproduction scenario**:

    - loading the game against a locally setup node
--

1. using a **measurement tool**

    - dotMemory/dotTrace from JetBrains
--

1. finding a **point** that makes a difference

    - a.k.a. WHAT causes it

--

???

We need to go deeper!

---

background-image: url(img/flight-to-mars.jpg)
background-size: cover

## Discovering New Worlds - EVM

- Ethereum has an internal Virtual Machine (EVM)

--

- EVM is stack based (as .NET Runtime) and uses `OpCodes` (.NET IL)

--

- **32 bytes** - a native size word

--

- accessing top 16 elements in the stack is cheap and easy:
  - `DUP_N` - duplicates specific item on the top
  - `SWAP_N` - swaps specific item with the top
  - `PUSH_N` - pushes an item on the stack

--

- when random memory is needed:
  - `MLOAD` - pops an `offset` and loads a slice of memory from `offset` to `offset + 32`
  - `MSTORE offset` - pops an `offset` and a `value` and stores it under `offset`

---

background-image: url(img/flight-to-mars.jpg)
background-size: cover

## Discovering New Worlds - more EVM

- stack is cheap

--

- high offsets of memory are pricey
  - the cost of memory is correlated with `O(n*n)`
  - where `n` is a number of words allocated

--

- apps usually tend to optimize for the memory usage - they're cheaper to run

---

background-image: url(img/Ethereum.png)
background-size: cover

# Faster Ether

## Szymon Kulec @Scooletz

### Thank you!