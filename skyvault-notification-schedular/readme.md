# ✉️ SkyVault – Cloud Messaging Functions

⚠️ This project is published for portfolio purposes only.
Unauthorized use or redistribution is not permitted.

Welcome to the **SkyVault Cloud Messaging Functions** repository 🚀  
This project is a core part of the **SkyVault** ecosystem and is responsible for **automated, reliable, and scalable customer communications**.

For the main application, see the  
👉 [SkyVault API Repository](https://github.com/sanuja-ariyapperuma/SkyVault-Backend)
---

## 🌤️ Project Overview

This Azure Cloud Function handles **scheduled and on-demand messaging workflows** for SkyVault.  
Its primary goal is to ensure customers receive **timely, relevant, and business-critical notifications** with minimal manual effort.

The solution is designed with **scalability, clarity, and extensibility** in mind.

---

## 🎯 Key Responsibilities

### 📅 Scheduled Email Notifications
The cloud function automatically sends emails based on predefined business rules, including:

- 🛂 **Passport expiration reminders**
- 🛃 **Visa expiration alerts**
- 🎂 **Birthday greetings**

These scheduled jobs help businesses stay proactive and maintain strong customer relationships.

---

### 📢 Bulk Promotion Messaging (HTTP Trigger)
In addition to scheduled jobs, the function exposes an **HTTP endpoint** that can be triggered by the main SkyVault backend.

**How it works:**

1. Promotions are defined in the main SkyVault application
2. Once ready, the backend calls the cloud function’s HTTP endpoint
3. The cloud function processes the request
4. Bulk messages are sent automatically to the target audience

This makes large-scale promotional campaigns fast, flexible, and easy to manage.

---

## 📬 Messaging Provider

All emails are sent using **Brevo** (formerly Sendinblue):

- ✅ Modern and developer-friendly
- 📤 Reliable bulk email delivery
- 📱 Supports multiple channels (Email, WhatsApp, etc.)

> **Note:** WhatsApp messaging is planned and architecturally supported, but **not yet implemented** at this stage.

---

## 🧱 Tech Stack

This project is built using a modern and performance-focused stack:

- **Azure Functions** – Serverless execution
- **.NET 8** – Latest LTS runtime
- **C#** – Clean, maintainable code
- **Dapper** – Lightweight and fast data access
- **Brevo API** – Email delivery service

---

## 🧠 Design Philosophy

- ⚡ **Serverless & scalable**
- 🔍 **Clear separation of responsibilities**
- 🔄 **Easy to extend with new message channels**
- 🛠️ **Optimized for maintainability and performance**

---

## 🚧 Roadmap

Planned enhancements include:

- 📲 WhatsApp message delivery via Brevo
- 📊 Improved monitoring and delivery tracking
- 🔐 Enhanced security for HTTP-triggered endpoints
- 🧩 Additional notification types as SkyVault evolves

---

## 🏁 Final Notes

This cloud function plays a vital role in SkyVault’s communication layer, ensuring **right messages reach the right users at the right time**.

If you’re exploring or extending SkyVault—this is where customer engagement begins ✨

---

⚠️ This project is published for portfolio purposes only.
Unauthorized use or redistribution is not permitted.
