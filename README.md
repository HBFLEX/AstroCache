# AstroCACHE/DB

AstroCache is a **light-weight in-memory data structure** built on top of **TCP**. It is designed to be fast, efficient, and scalable, making it ideal for a variety of applications.

## Overview

- **AstroCache Components**  
  AstroCache comprises two key components:
    - **Server**: Runs on a TCP connection, serving and communicating with client requests.
    - **Client**: Interacts with the server over the network.

- **Data Transfer**
    - Uses a light-weight RESP (Redis Serialization Protocol) for transferring data between the client and server.
    - Implements its own protocol called **AstroParser**, ensuring efficient data transfer and seamless communication between machines with different architectures.

- **Data Storage**
    - Data is stored in RAM, ensuring efficiency and fast caching.
    - Also supports a small-scale database infrastructure for persistent data storage.
    - Persistent data is managed through a binary extension format (`.astrodb`), enabling effective read and write operations.

## Features

1. Handles **multiple clients concurrently**.
2. Supports **data caching**, offering faster response times for server applications.
3. Provides **persistent data** support (currently in beta testing).
4. Operates over **TCP communication**, ensuring reliable and robust networking.

## Future Features (Roadmap)

- **Data replication support**: Enhance reliability and scalability.
- **Data stream support**: Enable continuous and real-time data handling.
- **Transactions**: Provide atomicity and ensure consistency in data operations.
- **More data structures**: Extend functionality with additional data types.

---

## Inspiration

AstroCache/DB is **inspired by Redis** and built by **HBFL3Xx**.

AstroCache/DB represents a blend of simplicity and power, ideal for developers looking to integrate efficient caching and database solutions into their applications.
