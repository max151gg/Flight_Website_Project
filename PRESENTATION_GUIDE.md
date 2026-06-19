# SkyPath — Presentation Guide

A simple guide to help me present and defend the SkyPath project.

---

## 1. Short project overview
SkyPath is a flight-booking system. Visitors can browse flights, see flight details,
and (after signing up / logging in) buy tickets. Administrators use a separate desktop
app to manage flights, users, tickets, discounts and announcements.

The system is built as **separate parts that talk to each other through a Web API**, which
is a clean, real-world way to design software.

---

## 2. Main purpose of the system
- Let customers **search and book flights** through a website.
- Let an admin **manage everything** (flights, users, tickets, discounts, announcements)
  from a desktop application.
- Keep all the data in **one shared database** that both the website and the admin app use.

---

## 3. Technologies used
- **C#** as the main programming language.
- **ASP.NET Core MVC** for the website (Razor views, controllers, sessions).
- **ASP.NET Core Web API** for the server that holds all the logic and database access.
- **WPF (XAML)** for the admin desktop application.
- **Microsoft Access database** (`.accdb`), accessed with **ADO.NET / OLEDB**.
- **HTML, CSS, JavaScript** for the website pages.
- **JSON + HTTP** for communication between the parts.

---

## 4. Main parts of the system
The solution has several projects that each do one job:

- **Website / client side — `SkyPathWebApp`**
  The public site. Controllers (`Guest`, `User`) load data from the API and show Razor pages.
  JavaScript inside the pages handles small things (form scrolling, the profile pop-out, etc.).

- **API / server side — `SkyPathWS`**
  The "brain". It has the controllers (`Guest`, `User`, `Admin`, `City`) and the database layer.
  All the rules (login check, buying a ticket, reducing seats) live here.

- **Database**
  A Microsoft Access file (`SkyPathWS/App_Data/Database_SkyPath.accdb`) with tables for
  User, Flight, Ticket, Discount, Announcement and City. Email and username are **unique**.

- **WPF admin app — `AdminApp`**
  A desktop program for admins only. Login → dashboard → pages to manage each table.

- **Shared helpers**
  - `SkyPath_Models` — the data classes (Flight, User, Ticket…) and view models, shared by all projects.
  - `SkyPathWSClient` — a small `ApiClient` helper used by the website and admin app to call the API.

**How they connect:** Website / Admin app → `ApiClient` (HTTP + JSON) → `SkyPathWS` API → database.

---

## 5. Main user flow (website)
1. **Browse flights** — `Guest/Browse` (or `User/Browse`) calls the API `GetFlightCatalog`,
   which filters by city/date, sorts, and returns one page of flights.
2. **View flight details** — clicking a flight opens `FlightDetail` with the full information.
3. **Purchase ticket** — on `Checkout` the user picks a currency (display only) and an
   optional discount, then clicks **Confirm Purchase**. The API checks there is a free seat,
   creates the ticket, **reduces the seats by one**, and removes the used discount.
4. **View user tickets** — `User/Ticket` shows all tickets the logged-in user has bought.

---

## 6. Main admin flow (WPF app)
1. **Login** — `LoginPage`. Only a user with **Role_Id = "0" (admin)** can get in.
2. **Dashboard** — the home page after login; the sidebar buttons become enabled.
3. **Manage flights** — `FlightPage` lists flights; `AddNewFlight` adds or edits a flight.
4. **Manage users** — `UsersHomePage`: change a user's role, ban/unban, or delete a user.
5. **Manage tickets** — `TicketPage`: see all tickets and change their status / delete.
6. **Manage discounts** — `DiscountPage`: create or delete discounts.
7. **Manage announcements** — `AnnouncementPage`: create or delete announcements
   (announcements with user id "0" are public/for everyone).

---

## 7. Important functions I should know how to explain
- **`UserRepository.Create` / `Login`** — how passwords are stored safely: we make a random
  **salt**, then save the **SHA256 hash of (password + salt)**. Login hashes the typed password
  with the saved salt and compares.
- **`UserController.GetFlightCatalog` (API)** — filtering by city/date, sorting, and paging (12 per page).
- **`UserController.PurchaseTicket` (API)** — the booking steps: check seat → create ticket →
  reduce seats → remove the used discount.
- **`FlightRepository.ReduceSeats`** — one UPDATE that lowers the seat count after a purchase.
- **`DbHelperOleDb`** — opens the Access connection and runs SQL using **parameters**
  (`@Email`, `@Price`…) which protects against SQL injection.
- **`ApiClient<T>`** — the helper that sends/receives JSON between the apps and the API.
- **`LoginPage.PerformLoginAsync` (WPF)** — admin login + the role check.

---

## 8. Possible questions the teacher may ask (with simple answers)
- **"Why split it into an API, a website and a desktop app?"**
  So the logic and database access live in one place (the API). The website and the admin app
  both reuse it instead of each writing their own database code.

- **"How are passwords kept safe?"**
  They are never stored as plain text. Each user has a random salt, and we store the
  SHA256 hash of the password + salt.

- **"How do you stop SQL injection?"**
  All SQL uses parameters (`@Email`, `@Price`), so user input is never put directly into the query.

- **"How does the site know who is logged in?"**
  The website saves the user id in the **session** after login and reads it on each page.

- **"What happens to seats when someone books?"**
  After a successful purchase the API calls `ReduceSeats`, lowering `Seats_Available` by one.

- **"How does currency conversion work?"**
  The price is stored in USD. On checkout we call an exchange-rate API to show the price in the
  chosen currency, but the actual charge stays in USD. If the rate API fails, it falls back to USD.

- **"What is the difference between the two `GetAllFlights`?"**
  They do the same query but live in different controllers (Guest/User/Admin) so each area
  has its own endpoint.

---

## 9. Known limitations (normal for a school project)
- The database is **Microsoft Access**, so it must not be open in Access while the apps run
  (Access locks the tables).
- The API address `localhost:5125` is **hard-coded**, so everything runs on one machine.
- The admin **flight search box is not implemented yet** (it's a placeholder).
- There is no email sending / password recovery (the "Forgot password" button is a message only).
- The currency API key is stored in the code (fine for a school demo, not for production).

---

## 10. What to demonstrate first
**Start the API first, then the website, then the admin app.** Suggested order:

1. **Run `SkyPathWS` (the API)** so data is available. (Make sure **Microsoft Access is closed**.)
2. **Run the website** and show: Home → Browse flights → Flight details.
3. **Sign up / Log in**, then do a **ticket purchase** (Checkout → Confirm) and show the new
   ticket under "My Tickets" and that the seats went down.
4. **Run the AdminApp**, log in as admin, and show **adding a flight** and **managing users**.

### Quick "don't click these" list during the demo
- Don't type in the **admin flight search box** (not implemented).
- Don't open the database in **Microsoft Access** while demoing.
- Make sure the **API is running** before opening the website/admin app, or lists will be empty.
