# 🏨 Hotel Management System

[![Build Status](https://img.shields.io/badge/build-passing-brightgreen)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![Platform](https://img.shields.io/badge/platform-.NET%206-blue)](https://dotnet.microsoft.com/)
[![Status](https://img.shields.io/badge/status-active-success)](#)

A web-based Hotel Management System developed using **ASP.NET 9.0 MVC**. This system allows hotel staff or admins to manage **room bookings**, **handle special requests**, and communicate with users through an integrated **chatbot**.

---

## 📚 Table of Contents

- [Features](#-features)
- [Technologies Used](#-technologies-used)
- [Architecture](#-architecture)

---

## ✨ Features

- ✅ Add and manage rooms
- ✅ Create and update bookings
- ✅ Add special requests to bookings
- ✅ ChatBot interaction for user assistance
- ✅ Reports generation
- ✅ Clean and modular MVC design
- ✅ JSON-driven chatbot intent logic

---

## 🧰 Technologies Used

| Technology     | Description                          |
|----------------|--------------------------------------|
| ASP.NET Core   | Backend framework                    |
| C#             | Programming language                 |
| Razor Pages    | Frontend templating engine           |
| JSON           | For chatbot intent definitions       |
| JetBrain Rider  | Development environment              |

---

## 🏗 Architecture

This project follows the **Model-View-Controller (MVC)** pattern:

- **Models** define the structure of data (e.g., Booking, Room).
- **Views** render the data to users using Razor syntax.
- **Controllers** manage user requests and return views or data.

---

1. **Clone the repository**:
   ```bash
   git clone https://github.com/your-username/hotel-management-system.git
   cd hotel-management-system
