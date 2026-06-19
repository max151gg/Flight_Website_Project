# SkyPath — Code Explanation Summary

A simple list of the most important files and what each one does.
The solution has 6 projects: **SkyPath_Models** (shared data), **SkyPathWS** (API + database),
**SkyPathWSClient** (HTTP helper), **SkyPathWebApp** (website), **AdminApp** (WPF admin app),
and **Testing** (a small scratch project, not part of the real system).

---

## API / Server side (`SkyPathWS`)
- `Program.cs` — starts the Web API server.
- `Controllers/GuestController.cs` — public actions: **Register** (sign up, with duplicate
  email/username checks) and **Login**.
- `Controllers/UserController.cs` — the main logged-in actions: **GetFlightCatalog**
  (filter/sort/page the flights), **PurchaseTicket** (buy a ticket and reduce seats),
  tickets, discounts, announcements, and profile image upload.
- `Controllers/AdminController.cs` — everything the admin app needs: create/update/delete
  flights, manage users (role, ban, delete), tickets, discounts and announcements.
- `Controllers/CityController.cs` — returns the list of cities (used to turn city ids into names).

### Database layer (`SkyPathWS/ORM`)
- `DbHelperOleDb.cs` — opens the Access database and runs SQL (Insert/Select/Update/Delete)
  using **parameters** to stay safe from SQL injection.
- `Repositories/RepositoryUOW.cs` — one place that creates and hands out all the repositories.
- `Repositories/UserRepository.cs` — user database code: **Create** (saves a salted SHA256
  password), **Login** (checks the password), **EmailExists/UserNameExists**, ban, delete, update.
- `Repositories/FlightRepository.cs` — flight database code: get all, get by id, create, update,
  delete, filter by city, and **ReduceSeats** (lower seats after a booking).
- `Repositories/TicketRepository.cs` — ticket database code (create, get by user, status, delete).
- `Repositories/DiscountRepository.cs` / `AnnouncementRepository.cs` / `CityRepository.cs` —
  database code for discounts, announcements and cities.
- `ORM/CreatorsModels/*Creator.cs` — turn a database row into a C# object (e.g. `FlightCreator`).

---

## Shared HTTP helper (`SkyPathWSClient`)
- `ApiClient.cs` — the helper used by the website and the admin app to call the API.
  `GetAsync` reads JSON, `PostAsync` sends JSON (and stores the error in `LastError` if it fails),
  `PostAsyncReturn` sends data and reads an object back.
- `SkyPathHttpClient.cs` — keeps one shared `HttpClient` instance.

---

## Website / Client side (`SkyPathWebApp`)
- `Program.cs` — starts the website and turns on **sessions** (to remember the logged-in user).
- `Controllers/GuestController.cs` — pages for visitors: home, browse, login and **Register**
  (sends the new user to the API and shows the real error if it fails).
- `Controllers/UserController.cs` — pages for logged-in users: **Browse**, **Checkout**,
  **Purchase**, tickets, discounts, account, and profile image upload.

### Important views (Razor pages, `SkyPathWebApp/Views`)
- `Guest/HomePage.cshtml` — home page with the **sign-up form**.
- `Guest/Browse.cshtml` / `User/Browse.cshtml` — the flight search results.
- `User/FlightDetail.cshtml` — full details of one flight.
- `User/Checkout.cshtml` — booking summary, currency + discount pickers, and **Confirm Purchase**.
- `User/Ticket.cshtml` — the user's purchased tickets.
- `User/Account.cshtml` — profile info, profile image upload, and change password (with JavaScript).
- `Shared/MasterPage*.cshtml` — the shared layout/menu for the pages.

---

## WPF Admin app (`AdminApp`)
- `MainWindow.xaml(.cs)` — the main window: the sidebar menu and **page navigation**.
- `UserControls/LoginPage.xaml.cs` — admin **login** and the check that only admins can enter.
- `UserControls/DashboardPage.xaml.cs` — the home screen after login.
- `UserControls/FlightPage.xaml.cs` — lists flights and has **add / edit / delete** buttons.
- `UserControls/AddNewFlight.xaml.cs` — the form to **add or edit a flight** (with validation).
- `UserControls/UsersHomePage.xaml.cs` — manage users (role, ban, delete).
- `UserControls/TicketPage.xaml.cs` — manage tickets.
- `UserControls/DiscountPage.xaml.cs` — manage discounts.
- `UserControls/AnnouncementPage.xaml.cs` — manage announcements.
- `Converters/*.cs` — small helpers that turn ids into names for display
  (e.g. city id → city name, user id → full name).

---

## Shared models (`SkyPath_Models`)
- `Models/*.cs` — the data classes: `Flight`, `User`, `Ticket`, `Discount`, `Announcement`, `City`.
- `ViewModel/*.cs` — classes that carry data to the pages, e.g. `BrowseViewModel`,
  `CheckoutViewModel`, `LoginViewModel`, `TicketViewModel`.
- `Attribute/*.cs` — custom validation rules (e.g. only digits, first letter capital).
- `Models/Model.cs` — the base class that **runs the validation rules** and remembers any
  errors in an `errors` dictionary (key = property name, value = list of error messages).
  `Validate()` checks the whole object; `IsValid` is true when there are no errors.

---

## Model creators (turn a database row into an object)
- Files: `SkyPathWS/ORM/CreatorsModels/*Creator.cs` (e.g. `FlightCreator`, `UserCreator`, `TicketCreator`).
- **What they do:** each creator receives one database row (an `IDataReader`) and builds one
  C# object from it (for example a `Flight`). This is the step that converts raw table data
  into objects the rest of the program can use.
- **Why needed:** it keeps all the "read column → set property" code in one place, so the
  repositories stay short.
- **Who uses them:** the repositories (e.g. `FlightRepository.GetALL` calls `FlightCreator`).
- `ModelCreators.cs` holds one shared instance of every creator (built only when first used).

---

## Key dictionaries (lookup tables)
A dictionary lets the code find a value instantly by its key, instead of searching a whole list.
The important ones in this project:

- **`ViewBag.CityDict`** (website) — Key = city id, Value = city name. Flights store only city
  ids, so the pages use this to show the city **name**. Built in the controllers, used in the views.
- **`CityIdToNameConverter.CityNamesById`** (WPF) — same idea on the admin side:
  Key = city id, Value = city name. Used in XAML to display city names on flight cards.
- **`UserIdToFullNameConverter.UserNamesById`** (WPF) — Key = user id, Value = full name.
  Used on the admin users/tickets screens.
- **`TicketFlightRouteConverter.FlightsById`** (WPF) — Key = flight id, Value = the whole `Flight`.
  Used to show a ticket's route ("Tel Aviv → Paris") without another database call.
- **`Model.errors`** (shared) — Key = property name, Value = list of error messages for that field.

---

## What to study before the presentation
Make sure you can explain, in your own words:
1. **The 3-part design** — website + API + admin app, all sharing one API and one database.
2. **`ApiClient`** — how the website/admin app call the API with GET/POST and JSON.
3. **Ticket purchase** — `UserController.PurchaseTicket`: check seat → create ticket →
   reduce seats → remove used discount.
4. **Database access** — `DbHelperOleDb` (parameters stop SQL injection) and how a repository
   uses a **model creator** to turn rows into objects.
5. **Passwords** — salt + SHA256 hash in `UserRepository`.
6. **Dictionaries** — what the CityDict / converter dictionaries are for (see above).

---

## Recommended Study Order Before Presentation
Read the files in this order — each step builds on the previous one:

1. **Website flow first** — `SkyPathWebApp/Controllers/GuestController.cs` and
   `UserController.cs`, plus the views `Views/User/Browse.cshtml` and `Views/User/Checkout.cshtml`.
   This is what the teacher sees on screen.
2. **The bridge** — `SkyPathWSClient/ApiClient.cs` (and `SkyPathHttpClient.cs`): how the pages
   talk to the API.
3. **Ticket purchase** — `SkyPathWS/Controllers/UserController.cs` → `PurchaseTicket`,
   then `TicketRepository.cs` and `FlightRepository.ReduceSeats`.
4. **Database access** — `SkyPathWS/ORM/DbHelperOleDb.cs`, then a repository
   (`UserRepository.cs` for login + password hashing), then a creator (`FlightCreator.cs`).
5. **WPF admin** — `AdminApp/UserControls/LoginPage.xaml.cs`, `FlightPage.xaml(.cs)` and
   `AddNewFlight.xaml.cs`.
6. **Helpers / converters / model creators last** — `AdminApp/Converters/*.cs`,
   `ModelCreators.cs`, and `Models/Model.cs` (validation). These support everything above.

**Read tonight first:** steps 1–3 (the website flow, `ApiClient`, and ticket purchase) — those
are the parts a teacher is most likely to ask you to walk through.
