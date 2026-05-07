# Review_Guard.Src

Review_Guard — Tamper-Resistant Review Platform
A production-ready .NET 8 + Clean Architecture + DDD (Lite) backend that prevents fake reviews by requiring proof of purchase and enforcing a trust-based approval matrix.

🏗 Architecture
VerifiedReviews/
├── Domain/ Entities, Enums, ValueObjects, Domain Services, Rules, Events
├── Application/ CQRS (MediatR), Validators (FluentValidation), Interfaces, Result Pattern
├── Infrastructure/ EF Core, Repositories, JWT, Email, FileStorage, RiskScore, WeightedRating
└── API/ Controllers, GlobalExceptionMiddleware, Swagger/JWT, ApiResponse<T>
Dependency rule: API → Application → Domain ← Infrastructure

🔑 Core Features
1️⃣ UserLevel System (TrustScore → Level)
Level TrustScore Proof Required Approval
LowTrust 0–39 ✅ Required Admin approval always
Normal 40–69 ✅ Required Auto-approve if RiskScore < 30
Trusted 70–100 Optional Auto-approve if RiskScore < 80
UserLevel is never stored — it's derived at runtime from TrustScoreValue.

2️⃣ Weighted Average Rating
WeightedAvg = SUM(Rating × TrustScore) / SUM(TrustScore)
Implemented as a pure domain service (WeightedAverageCalculator)
Recalculated on every approve/reject/moderate action
Exposed separately from the simple average so clients can display both
Used as the primary sort key for business discovery
3️⃣ Dashboard System (CQRS Read Side)
Three dedicated repositories — each returns a fully aggregated snapshot:

Dashboard Audience Key Data
UserDashboard End user TrustScore, Level, review counts, recent history
OwnerDashboard Business owner Both ratings, star breakdown, pending queue, reports
AdminDashboard Platform admin User trust distribution, moderation queue, suspicious activity
AdminDashboardRepository runs 6 sub-queries with Task.WhenAll for concurrent execution.

4️⃣ Proof System
Proof entity supports: Invoice file upload, Receipt file upload, Order ID string
File stored via IFileStorageService (local or swappable to Azure Blob / S3)
Max 10 MB, allowed types: PDF, JPEG, PNG, WebP
Must be verified by admin before it can be used to gate a review
5️⃣ RiskScore Engine
7 signals calculated per review submission:

Signal Weight
User's inverse TrustScore 30%
Account age < 7 days +20 pts
Reviews from same IP in 24h ≥ 5 +30 pts
Reviews submitted in last 1h ≥ 3 +25 pts
Content length < 50 chars +10 pts
Spam keywords matched +5 pts each
Email not verified +20 pts
Result: Low (<30) / Medium (30–59) / High (60–79) / Critical (≥80)

6️⃣ Email Service
Full HTML email templates with inline styles:

Template Trigger
Email Confirmation User registration
Welcome After email verified
Review Received Notifies business owner
Review Status Update Approve / Reject notifications
Account Suspended / Banned Admin actions
Password Reset Self-service reset
📡 API Endpoints
Auth
POST /api/users/register Register user
POST /api/users/login User login → JWT
GET /api/users/verify-email Verify email via token
GET /api/users/me Own profile (TrustScore + UserLevel)
POST /api/admin/auth/login Admin login → JWT (Admin/SuperAdmin role)
Reviews
POST /api/reviews Submit review (proof required for LowTrust/Normal)
GET /api/reviews/mine Own review history with statuses
GET /api/reviews/business/{id} Business reviews (public, Approved only)
GET /api/reviews/pending Pending queue (Admin)
GET /api/reviews/flagged Flagged reviews (Admin)
PATCH /api/reviews/{id}/approve Approve review (Admin)
PATCH /api/reviews/{id}/reject Reject review with reason (Admin)
Businesses
POST /api/businesses Register business
GET /api/businesses/mine Owner's businesses with ratings
GET /api/businesses/{id}/dashboard Full owner dashboard
Proofs
POST /api/proofs Upload invoice/receipt or order ID
PATCH /api/proofs/{id}/verify Verify or reject a proof (Admin)
Reports
POST /api/reports Report a review
GET /api/reports List all reports with filter (Admin)
Dashboard
GET /api/dashboard/user/me Personal dashboard (User)
GET /api/dashboard/user/{id} Any user's dashboard (Admin)
GET /api/dashboard/owner/{businessId} Owner dashboard with weighted rating
GET /api/dashboard/admin Platform-wide stats + suspicious activity
Admin
POST /api/admin/users/{id}/ban Ban user
POST /api/admin/users/{id}/suspend Suspend user (with optional until date)
POST /api/admin/reviews/{id}/moderate Approve / Reject / Flag
GET /api/admin/suspicious-activity Activity report (last N hours)
🚀 Getting Started

# 1. Clone and restore

dotnet restore

# 2. Configure appsettings.json

# - ConnectionStrings:DefaultConnection (SQL Server)

# - Jwt:Secret (min 32 chars)

# - Smtp.\* (your SMTP provider)

# 3. Run (auto-migrates on first start in Development)

cd src/VerifiedReviews.API
dotnet run

# 4. Open Swagger UI

# https://localhost:5001/swagger

# 5. Generate migrations manually (if you change entities)

dotnet ef migrations add YourMigrationName \
 --project src/VerifiedReviews.Infrastructure \
 --startup-project src/VerifiedReviews.API \
 --output-dir Persistence/Migrations
📦 NuGet Packages
Package Version Purpose
MediatR 12.2 CQRS + Pipeline Behaviors
FluentValidation 11.9 Input validation
EF Core + SqlServer 8.0.4 ORM
System.IdentityModel.Tokens.Jwt 7.5 JWT
Swashbuckle.AspNetCore 6.6 Swagger UI
🔒 Security
PBKDF2-SHA256 (100k iterations) password hashing
JWT HS256 with configurable expiry; separate tokens for Users and Admins
Role-based authorization: User, Admin, SuperAdmin
IP address tracking + UserActivity audit log
Anti-spam: daily review limit, IP-based pattern detection
Global exception middleware → no stack traces leak to clients
🧪 Testing the Proof-Gated Review Flow

1. Register user POST /api/users/register
2. Verify email GET /api/users/verify-email?token=...
3. Login POST /api/users/login
4. Create business POST /api/businesses
5. Upload proof POST /api/proofs (as a different user)
6. Admin login POST /api/admin/auth/login
7. Verify proof PATCH /api/proofs/{id}/verify { "approve": true }
8. Submit review POST /api/reviews { businessId, proofId, rating, title, content }
9. View owner dashboard GET /api/dashboard/owner/{businessId}
   → weightedAverageRating reflects the new approved review
