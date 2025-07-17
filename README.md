# Thotron Backend

This repository contains the backend API for **Thotron – AI Narrated Video Compilation**, a graduation project developed by students at Cairo University's Faculty of Computers and Artificial Intelligence.

The backend is built with **ASP.NET Core (.NET 8)** and uses **MongoDB** for data persistence. It serves as the central hub for handling user authentication, project management, video metadata, and communication with the AI modules.

---

## ⚙️ Tech Stack

- ASP.NET Core 8
- MongoDB (via MongoDB.Driver)
- JWT Authentication
- RESTful API Design
- CORS Policy Support
- Swagger API Documentation
- Integration with Flask-based AI services
- Project sharing via public links

---

## 📌 Features

- User registration, login, and authentication (JWT-based)
- Create, edit, and delete video projects
- Support for manual and AI-generated video compilations
- Store user preferences and selected video timestamps
- PDF summary download endpoints
- Link-based project sharing (read-only access)
- Integration-ready endpoints for AI summarization & narration

---

## 🚀 Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/ThotronOrg/backend.git
cd backend
