# WPF Chat Application TCP Socket

A lightweight, multi-user chat platform built with **WPF (C#)** using the **TCP/IP** protocol. This project demonstrates a classic Server-Client architecture capable of handling multiple concurrent connections.

## Features

* **Multi-Client Server:** Managed via `TcpListener`, supporting broadcasting logic to ensure all users receive messages in real-time.
* **Intuitive Client UI:** Simple nickname registration and IP-based connection.
* **Emoji Support:** Integrated emoji picker for enhanced user expression.
* **Asynchronous Processing:** Fully utilizes `async/await` patterns to keep the UI responsive during data transmission (No "Not Responding" windows).

## Tech Stack

* **Language:** C#
* **Framework:** .NET 8.0 (WPF)
* **Networking:** `System.Net.Sockets` (`TcpListener`, `TcpClient`)
* **Threading:** Task Parallel Library (TPL)


## Getting Started

### 1. Prerequisites

* Visual Studio 2022 or later.
* .NET SDK 6.0+ installed.

### 2. Running the Project

Follow these steps to get a local copy of the project up and running.

1. Clone the Repository
Open your terminal or Git Bash and run:

```
git clone https://github.com/huynphgtr/WPFChatApp-TCPSocket
cd WPFChatApp

```

2. **Open Solution:** Launch `WpfChatApp.sln`.
3. **Configure Startup:**
* Right-click the Solution -> **Set Startup Projects...**
* Select **Multiple startup projects**.
* Set the Action for both `ChatServer` and `ChatClient` to **Start**.
4. **Application Flow:**
* **Server:** Click **Start Server** first.
* **Client:** Enter a nickname and IP address (use `127.0.0.1` for local testing), then click **Connect**.

## System Architecture

The application operates on a **Star Topology**:

* **The Server** acts as the central hub. When a client sends a message, the server receives it and iterates through its list of active `TcpClient` objects to **broadcast** the message to everyone else.
* **Serialization:** Messages are typically sent as UTF-8 encoded strings, often prefixed with a length header or wrapped in a simple JSON packet for structured data.

## Network Notes

* **LAN Connection:** To chat across different computers, ensure Port **8888** (or your chosen port) is allowed through the Windows Firewall on the Server machine.
* **Loopback:** For testing on a single machine, use the loopback address `127.0.0.1`.

## Demo
<img width="1920" height="1080" alt="DemoImage" src="https://github.com/user-attachments/assets/0e84a3ee-f868-47b5-b813-c76484174122" />
