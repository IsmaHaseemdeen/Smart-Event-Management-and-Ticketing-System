using System;
using System.Collections.Generic;
using System.Linq;
using EventTicketingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingSystem.Data
{
    public static class DbSeeder
    {
        public static void Seed(ApplicationDbContext db)
        {
            if (db.Users.AsNoTracking().Any())
                return;

            var now = DateTime.Now;

            //Users
            var admin = new Users
            {
                FullName = "Amelia Grant",
                Email = "admin@council.com",
                Phone = "07700 900321",
                Password = "Admin#2026!",
                Role = "ADMIN",
                Status = "ACTIVE",
                CreatedAt = now.AddMonths(-10)
            };

            var staff1 = new Users
            {
                FullName = "Daniel Brooks",
                Email = "daniel.brooks@council.com",
                Phone = "07700 900117",
                Password = "Staff1#2026!",
                Role = "STAFF",
                Status = "ACTIVE",
                CreatedAt = now.AddMonths(-8)
            };

            var staff2 = new Users
            {
                FullName = "Priya Shah",
                Email = "priya.shah@council.com",
                Phone = "07700 900204",
                Password = "Staff2#2026!",
                Role = "STAFF",
                Status = "ACTIVE",
                CreatedAt = now.AddMonths(-8)
            };

            var m1 = new Users
            {
                FullName = "Hannah Collins",
                Email = "hannah.collins@gmail.com",
                Phone = "07911 284903",
                Password = "Hannah@2026",
                Role = "MEMBER",
                Status = "ACTIVE",
                CreatedAt = now.AddMonths(-6)
            };

            var m2 = new Users
            {
                FullName = "Ayaan Khan",
                Email = "ayaan.khan@outlook.com",
                Phone = "07855 601334",
                Password = "Ayaan@2026",
                Role = "MEMBER",
                Status = "ACTIVE",
                CreatedAt = now.AddMonths(-6)
            };

            var m3 = new Users
            {
                FullName = "Sofia Martins",
                Email = "sofia.martins@yahoo.com",
                Phone = "07712 448920",
                Password = "Sofia@2026",
                Role = "MEMBER",
                Status = "ACTIVE",
                CreatedAt = now.AddMonths(-5)
            };

            var m4 = new Users
            {
                FullName = "Oliver Bennett",
                Email = "oliver.bennett@gmail.com",
                Phone = "07988 115673",
                Password = "Oliver@2026",
                Role = "MEMBER",
                Status = "ACTIVE",
                CreatedAt = now.AddMonths(-5)
            };

            db.Users.AddRange(admin, staff1, staff2, m1, m2, m3, m4);
            db.SaveChanges();

            // Venues
            var v1 = new Venue
            {
                VenueName = "Barbican Centre - Main Hall",
                AddressLine1 = "Silk Street",
                AddressLine2 = "Barbican",
                City = "Oxford",
                Capacity = 1900,
                Phone = "020 7638 8891"
            };

            var v2 = new Venue
            {
                VenueName = "Royal Festival Hall",
                AddressLine1 = "Southbank Centre",
                AddressLine2 = "Belvedere Road",
                City = "London",
                Capacity = 2500,
                Phone = "020 3879 9555"
            };

            var v3 = new Venue
            {
                VenueName = "Hackney Empire",
                AddressLine1 = "291 Mare Street",
                AddressLine2 = "Hackney",
                City = "Cambridge",
                Capacity = 1200,
                Phone = "020 8985 2424"
            };

            db.Venues.AddRange(v1, v2, v3);
            db.SaveChanges();

            // Categories
            var cMusic = new Category { CategoryName = "Music", Description = "Live music, orchestral, contemporary and acoustic sets." };
            var cDrama = new Category { CategoryName = "Drama", Description = "Theatre productions, stage drama, spoken performances." };
            var cDance = new Category { CategoryName = "Dance", Description = "Contemporary dance, cultural dance, showcases and recitals." };
            var cArt = new Category { CategoryName = "Art Exhibition", Description = "Exhibitions, installations, galleries and curated collections." };
            var cWorkshop = new Category { CategoryName = "Workshop", Description = "Learning sessions, masterclasses and community workshops." };
            var cCulture = new Category { CategoryName = "Cultural Festival", Description = "Seasonal festivals, city culture nights, heritage events." };

            db.Categories.AddRange(cMusic, cDrama, cDance, cArt, cWorkshop, cCulture);
            db.SaveChanges();

            //Events
            var finishedEvent = new Event
            {
                Title = "London Winter Culture Gala 2025",
                Description =
                    "A city-wide celebration of music and dance featuring a compact orchestra, guest vocalists, " +
                    "and a closing cultural showcase. The evening is known for tight timekeeping, smooth entry flow, " +
                    "and a well-managed audience experience with clear signage and friendly stewards.",
                PosterUrl = "/Images/Posters/london-winter-gala.png",
                VenueId = v2.VenueId,
                CreatedBy = admin.UserId,
                Status = "FINISHED",
                CreatedAt = now.AddMonths(-9)
            };

            var finishedEvent2 = new Event
            {
                Title = "Oxford Spring Theatre Night 2025",
                Description =
                    "A theatre evening featuring two compact stage dramas and a short spoken segment. " +
                    "Audience guidance, timing, and seating assistance were managed carefully to reduce disruption.",
                PosterUrl = "/Images/Posters/oxford-theatre-night.png",
                VenueId = v1.VenueId,
                CreatedBy = admin.UserId,
                Status = "FINISHED",
                CreatedAt = now.AddMonths(-8)
            };

            var finishedEvent3 = new Event
            {
                Title = "Cambridge Cultural Dance Festival 2025",
                Description =
                    "A multi-act dance programme with cultural segments and contemporary choreography. " +
                    "Entry flow, intermission crowd control, and stage transitions were praised for being smooth.",
                PosterUrl = "/Images/Posters/cambridge-dance-festival.png",
                VenueId = v3.VenueId,
                CreatedBy = admin.UserId,
                Status = "FINISHED",
                CreatedAt = now.AddMonths(-7)
            };

            var upcomingEvent = new Event
            {
                Title = "Southbank Youth Music Night 2026",
                Description =
                    "An energetic night of emerging London talent with curated sets across indie, acoustic, and modern jazz. " +
                    "Expect clean sound checks, timed stage changeovers, and seating guidance on arrival. " +
                    "Doors open early for pre-show ambience and refreshments.",
                PosterUrl = "/Images/Posters/southbank-youth-night.png",
                VenueId = v2.VenueId,
                CreatedBy = admin.UserId,
                Status = "ACTIVE",
                CreatedAt = now.AddDays(-18)
            };

            var hybridEvent = new Event
            {
                Title = "Hackney Stage & Studio Week",
                Description =
                    "A mixed programme combining a short theatre showcase with creative workshops. " +
                    "Last month’s show received strong feedback for stage visibility and steward coordination. " +
                    "The upcoming session focuses on performance craft and audience engagement.",
                PosterUrl = "/Images/Posters/hackney-stage-week.png",
                VenueId = v3.VenueId,
                CreatedBy = admin.UserId,
                Status = "ACTIVE",
                CreatedAt = now.AddMonths(-3)
            };

            var artExpo = new Event
            {
                Title = "Barbican Contemporary Art Evening 2026",
                Description =
                    "A curated exhibition night featuring emerging artists and short guided walk-throughs. " +
                    "Lighting and room flow are designed for easy viewing, with quiet zones and scheduled talk segments.",
                PosterUrl = "/Images/Posters/barbican-art-evening.png",
                VenueId = v1.VenueId,
                CreatedBy = admin.UserId,
                Status = "ACTIVE",
                CreatedAt = now.AddDays(-30)
            };

            var danceShowcase = new Event
            {
                Title = "Cambridge Dance Showcase: Contemporary & Culture",
                Description =
                    "A varied dance programme blending contemporary pieces with cultural segments. " +
                    "The show is designed for clear sightlines and balanced pacing, with a short intermission " +
                    "and managed re-entry to reduce disruption.",
                PosterUrl = "/Images/Posters/cambridge-dance-showcase.png",
                VenueId = v3.VenueId,
                CreatedBy = admin.UserId,
                Status = "ACTIVE",
                CreatedAt = now.AddDays(-22)
            };

            var workshopDay = new Event
            {
                Title = "Weekend Creative Workshops (Oxford)",
                Description =
                    "A full-day learning programme covering basic stagecraft, script reading, and performance confidence. " +
                    "All materials are provided and sessions are structured for beginners with time for Q&A.",
                PosterUrl = "/Images/Posters/weekend-workshops.png",
                VenueId = v1.VenueId,
                CreatedBy = admin.UserId,
                Status = "ACTIVE",
                CreatedAt = now.AddDays(-16)
            };

            db.Events.AddRange(
                finishedEvent, finishedEvent2, finishedEvent3,
                upcomingEvent, hybridEvent, artExpo, danceShowcase, workshopDay
            );
            db.SaveChanges();

            // Event Categories
            db.EventCategories.AddRange(

                new EventCategory { EventId = finishedEvent.EventId, CategoryId = cCulture.CategoryId },
                new EventCategory { EventId = finishedEvent.EventId, CategoryId = cDance.CategoryId },
                new EventCategory { EventId = finishedEvent.EventId, CategoryId = cMusic.CategoryId },

                new EventCategory { EventId = finishedEvent2.EventId, CategoryId = cDrama.CategoryId },
                new EventCategory { EventId = finishedEvent2.EventId, CategoryId = cCulture.CategoryId },

                new EventCategory { EventId = finishedEvent3.EventId, CategoryId = cDance.CategoryId },
                new EventCategory { EventId = finishedEvent3.EventId, CategoryId = cCulture.CategoryId },

                new EventCategory { EventId = upcomingEvent.EventId, CategoryId = cMusic.CategoryId },
                new EventCategory { EventId = upcomingEvent.EventId, CategoryId = cCulture.CategoryId },

                new EventCategory { EventId = hybridEvent.EventId, CategoryId = cDrama.CategoryId },
                new EventCategory { EventId = hybridEvent.EventId, CategoryId = cWorkshop.CategoryId },
                new EventCategory { EventId = hybridEvent.EventId, CategoryId = cArt.CategoryId },

                new EventCategory { EventId = artExpo.EventId, CategoryId = cArt.CategoryId },
                new EventCategory { EventId = artExpo.EventId, CategoryId = cCulture.CategoryId },

                new EventCategory { EventId = danceShowcase.EventId, CategoryId = cDance.CategoryId },
                new EventCategory { EventId = danceShowcase.EventId, CategoryId = cCulture.CategoryId },

                new EventCategory { EventId = workshopDay.EventId, CategoryId = cWorkshop.CategoryId },
                new EventCategory { EventId = workshopDay.EventId, CategoryId = cDrama.CategoryId }
            );
            db.SaveChanges();

            // Event Sessions
            var fe_s1 = new EventSession
            {
                EventId = finishedEvent.EventId,
                SessionDate = now.AddMonths(-7).Date,
                StartDatetime = now.AddMonths(-7).Date.AddHours(19),
                EndDatetime = now.AddMonths(-7).Date.AddHours(22),
                SessionStatus = "FINISHED",
                Notes = "Strong steward coordination; quick entry checks and clear hall directions.",
                TotalSeats = 2500,
                ReservedSeats = 0,
                SoldSeats = 0
            };

            var fe_s2 = new EventSession
            {
                EventId = finishedEvent.EventId,
                SessionDate = now.AddMonths(-7).Date.AddDays(2),
                StartDatetime = now.AddMonths(-7).Date.AddDays(2).AddHours(19),
                EndDatetime = now.AddMonths(-7).Date.AddDays(2).AddHours(22),
                SessionStatus = "FINISHED",
                Notes = "Clean sound mix; intermission was well timed with minimal crowding.",
                TotalSeats = 2500,
                ReservedSeats = 0,
                SoldSeats = 0
            };

            var f2_s1 = new EventSession
            {
                EventId = finishedEvent2.EventId,
                SessionDate = now.AddMonths(-6).Date,
                StartDatetime = now.AddMonths(-6).Date.AddHours(18),
                EndDatetime = now.AddMonths(-6).Date.AddHours(21),
                SessionStatus = "FINISHED",
                Notes = "Good pacing, clear announcements, and minimal late seating disruptions.",
                TotalSeats = 1900,
                ReservedSeats = 0,
                SoldSeats = 0
            };

            var f3_s1 = new EventSession
            {
                EventId = finishedEvent3.EventId,
                SessionDate = now.AddMonths(-5).Date,
                StartDatetime = now.AddMonths(-5).Date.AddHours(19),
                EndDatetime = now.AddMonths(-5).Date.AddHours(21),
                SessionStatus = "FINISHED",
                Notes = "Smooth intermission crowd control and clean stage transitions.",
                TotalSeats = 1200,
                ReservedSeats = 0,
                SoldSeats = 0
            };

            var ue_s1 = new EventSession
            {
                EventId = upcomingEvent.EventId,
                SessionDate = now.AddDays(28).Date,
                StartDatetime = now.AddDays(28).Date.AddHours(18),
                EndDatetime = now.AddDays(28).Date.AddHours(22),
                SessionStatus = "ACTIVE",
                Notes = "Doors 17:15. Bag checks active. Seating assistance available.",
                TotalSeats = 2500,
                ReservedSeats = 2,
                SoldSeats = 0
            };

            var he_past = new EventSession
            {
                EventId = hybridEvent.EventId,
                SessionDate = now.AddMonths(-1).Date,
                StartDatetime = now.AddMonths(-1).Date.AddHours(19),
                EndDatetime = now.AddMonths(-1).Date.AddHours(21),
                SessionStatus = "FINISHED",
                Notes = "Stage visibility excellent; show started on time and ran smoothly.",
                TotalSeats = 1200,
                ReservedSeats = 0,
                SoldSeats = 0
            };

            var he_future = new EventSession
            {
                EventId = hybridEvent.EventId,
                SessionDate = now.AddDays(42).Date,
                StartDatetime = now.AddDays(42).Date.AddHours(18),
                EndDatetime = now.AddDays(42).Date.AddHours(20),
                SessionStatus = "ACTIVE",
                Notes = "Workshop + performance segment. Registration opens 17:30.",
                TotalSeats = 1200,
                ReservedSeats = 0,
                SoldSeats = 0
            };

            var ax_s1 = new EventSession
            {
                EventId = artExpo.EventId,
                SessionDate = now.AddDays(16).Date,
                StartDatetime = now.AddDays(16).Date.AddHours(17),
                EndDatetime = now.AddDays(16).Date.AddHours(21),
                SessionStatus = "ACTIVE",
                Notes = "Guided walk-through at 18:00. Quiet viewing zones available.",
                TotalSeats = 1900,
                ReservedSeats = 0,
                SoldSeats = 0
            };

            var ds_s1 = new EventSession
            {
                EventId = danceShowcase.EventId,
                SessionDate = now.AddDays(64).Date,
                StartDatetime = now.AddDays(64).Date.AddHours(19),
                EndDatetime = now.AddDays(64).Date.AddHours(21),
                SessionStatus = "ACTIVE",
                Notes = "Short intermission at 20:00. Please be seated by 18:50.",
                TotalSeats = 1200,
                ReservedSeats = 0,
                SoldSeats = 0
            };

            var wd_s1 = new EventSession
            {
                EventId = workshopDay.EventId,
                SessionDate = now.AddDays(22).Date,
                StartDatetime = now.AddDays(22).Date.AddHours(10),
                EndDatetime = now.AddDays(22).Date.AddHours(16),
                SessionStatus = "ACTIVE",
                Notes = "Bring a notebook. Materials provided. Refreshments available during breaks.",
                TotalSeats = 1900,
                ReservedSeats = 0,
                SoldSeats = 0
            };

            db.EventSessions.AddRange(
                fe_s1, fe_s2,
                f2_s1, f3_s1,
                ue_s1, he_past, he_future, ax_s1, ds_s1, wd_s1
            );
            db.SaveChanges();

            // Ticket Types
            var allSessions = new[] { fe_s1, fe_s2, f2_s1, f3_s1, ue_s1, he_past, he_future, ax_s1, ds_s1, wd_s1 };

            const string STD = "STANDARD";
            const string VIP = "VIP";
            const string BAL = "BALCONY";

            decimal pStd = 18.00m, pVip = 45.00m, pBal = 30.00m;

            var tt = new Dictionary<int, Dictionary<string, TicketType>>();

            foreach (var s in allSessions)
            {
                var list = new List<TicketType>
                {
                    new TicketType { SessionId = s.SessionId, TypeName = STD, Price = pStd, Status = "ACTIVE" },
                    new TicketType { SessionId = s.SessionId, TypeName = VIP, Price = pVip, Status = "ACTIVE" },
                    new TicketType { SessionId = s.SessionId, TypeName = BAL, Price = pBal, Status = "ACTIVE" }
                };

                db.TicketTypes.AddRange(list);
                db.SaveChanges();

                tt[s.SessionId] = list.ToDictionary(x => x.TypeName, x => x);
            }

            //Bookings
            var att1 = new Booking
            {
                UserId = m1.UserId,
                SessionId = fe_s1.SessionId,
                BookingCode = "BK-LON-FE-3001",
                BookingDate = fe_s1.StartDatetime.AddDays(-5),
                BookingStatus = "CONFIRMED",
                TotalAmount = (pStd * 2),
                IssuedAt = fe_s1.StartDatetime.AddDays(-5).AddMinutes(20),
                IsUsed = "Y",
                CheckedInBy = staff1.UserId
            };

            var att2 = new Booking
            {
                UserId = m2.UserId,
                SessionId = fe_s2.SessionId,
                BookingCode = "BK-LON-FE-3002",
                BookingDate = fe_s2.StartDatetime.AddDays(-6),
                BookingStatus = "CONFIRMED",
                TotalAmount = (pBal * 1) + (pStd * 1),
                IssuedAt = fe_s2.StartDatetime.AddDays(-6).AddMinutes(15),
                IsUsed = "Y",
                CheckedInBy = staff2.UserId
            };

            var att3 = new Booking
            {
                UserId = m3.UserId,
                SessionId = fe_s2.SessionId,
                BookingCode = "BK-LON-FE-3003",
                BookingDate = fe_s2.StartDatetime.AddDays(-3),
                BookingStatus = "CONFIRMED",
                TotalAmount = (pVip * 1),
                IssuedAt = fe_s2.StartDatetime.AddDays(-3).AddMinutes(10),
                IsUsed = "Y",
                CheckedInBy = staff1.UserId
            };

            var f2_b1 = new Booking
            {
                UserId = m4.UserId,
                SessionId = f2_s1.SessionId,
                BookingCode = "BK-OXF-F2-5001",
                BookingDate = f2_s1.StartDatetime.AddDays(-4),
                BookingStatus = "CONFIRMED",
                TotalAmount = (pStd * 2),
                IssuedAt = f2_s1.StartDatetime.AddDays(-4).AddMinutes(14),
                IsUsed = "Y",
                CheckedInBy = staff2.UserId
            };

            var f2_b2 = new Booking
            {
                UserId = m1.UserId,
                SessionId = f2_s1.SessionId,
                BookingCode = "BK-OXF-F2-5002",
                BookingDate = f2_s1.StartDatetime.AddDays(-2),
                BookingStatus = "CONFIRMED",
                TotalAmount = (pBal * 1),
                IssuedAt = f2_s1.StartDatetime.AddDays(-2).AddMinutes(9),
                IsUsed = "Y",
                CheckedInBy = staff1.UserId
            };

            var f3_b1 = new Booking
            {
                UserId = m2.UserId,
                SessionId = f3_s1.SessionId,
                BookingCode = "BK-CAM-F3-6001",
                BookingDate = f3_s1.StartDatetime.AddDays(-5),
                BookingStatus = "CONFIRMED",
                TotalAmount = (pStd * 2) + (pVip * 1),
                IssuedAt = f3_s1.StartDatetime.AddDays(-5).AddMinutes(11),
                IsUsed = "Y",
                CheckedInBy = staff1.UserId
            };

            var f3_b2 = new Booking
            {
                UserId = m3.UserId,
                SessionId = f3_s1.SessionId,
                BookingCode = "BK-CAM-F3-6002",
                BookingDate = f3_s1.StartDatetime.AddDays(-3),
                BookingStatus = "CONFIRMED",
                TotalAmount = (pBal * 2),
                IssuedAt = f3_s1.StartDatetime.AddDays(-3).AddMinutes(16),
                IsUsed = "Y",
                CheckedInBy = staff2.UserId
            };

            var att4 = new Booking
            {
                UserId = m4.UserId,
                SessionId = he_past.SessionId,
                BookingCode = "BK-LON-HE-4001",
                BookingDate = he_past.StartDatetime.AddDays(-4),
                BookingStatus = "CONFIRMED",
                TotalAmount = (pStd * 2),
                IssuedAt = he_past.StartDatetime.AddDays(-4).AddMinutes(12),
                IsUsed = "Y",
                CheckedInBy = staff2.UserId
            };

            db.Bookings.AddRange(att1, att2, att3, f2_b1, f2_b2, f3_b1, f3_b2, att4);
            db.SaveChanges();

            // BookedTickets for past bookings
            db.BookedTickets.AddRange(

                // F1
                new BookedTickets
                {
                    BookingId = att1.BookingId,
                    TicketTypeId = tt[fe_s1.SessionId][STD].TicketTypeId,
                    Quantity = 2,
                    UnitPrice = pStd,
                    LineTotal = pStd * 2
                },
                new BookedTickets
                {
                    BookingId = att2.BookingId,
                    TicketTypeId = tt[fe_s2.SessionId][BAL].TicketTypeId,
                    Quantity = 1,
                    UnitPrice = pBal,
                    LineTotal = pBal
                },
                new BookedTickets
                {
                    BookingId = att2.BookingId,
                    TicketTypeId = tt[fe_s2.SessionId][STD].TicketTypeId,
                    Quantity = 1,
                    UnitPrice = pStd,
                    LineTotal = pStd
                },
                new BookedTickets
                {
                    BookingId = att3.BookingId,
                    TicketTypeId = tt[fe_s2.SessionId][VIP].TicketTypeId,
                    Quantity = 1,
                    UnitPrice = pVip,
                    LineTotal = pVip
                },

                // F2
                new BookedTickets
                {
                    BookingId = f2_b1.BookingId,
                    TicketTypeId = tt[f2_s1.SessionId][STD].TicketTypeId,
                    Quantity = 2,
                    UnitPrice = pStd,
                    LineTotal = pStd * 2
                },
                new BookedTickets
                {
                    BookingId = f2_b2.BookingId,
                    TicketTypeId = tt[f2_s1.SessionId][BAL].TicketTypeId,
                    Quantity = 1,
                    UnitPrice = pBal,
                    LineTotal = pBal
                },

                // F3
                new BookedTickets
                {
                    BookingId = f3_b1.BookingId,
                    TicketTypeId = tt[f3_s1.SessionId][STD].TicketTypeId,
                    Quantity = 2,
                    UnitPrice = pStd,
                    LineTotal = pStd * 2
                },
                new BookedTickets
                {
                    BookingId = f3_b1.BookingId,
                    TicketTypeId = tt[f3_s1.SessionId][VIP].TicketTypeId,
                    Quantity = 1,
                    UnitPrice = pVip,
                    LineTotal = pVip
                },
                new BookedTickets
                {
                    BookingId = f3_b2.BookingId,
                    TicketTypeId = tt[f3_s1.SessionId][BAL].TicketTypeId,
                    Quantity = 2,
                    UnitPrice = pBal,
                    LineTotal = pBal * 2
                },

                // Hybrid past
                new BookedTickets
                {
                    BookingId = att4.BookingId,
                    TicketTypeId = tt[he_past.SessionId][STD].TicketTypeId,
                    Quantity = 2,
                    UnitPrice = pStd,
                    LineTotal = pStd * 2
                }
            );
            db.SaveChanges();

            // Payments for past bookings (SUCCESS)
            db.Payments.AddRange(
                new Payment { BookingId = att1.BookingId, Amount = att1.TotalAmount, Method = "CARD", PaymentStatus = "SUCCESS", PaidAt = att1.IssuedAt.AddMinutes(6), Notes = "Card payment confirmed." },
                new Payment { BookingId = att2.BookingId, Amount = att2.TotalAmount, Method = "BANK_TRANSFER", PaymentStatus = "SUCCESS", PaidAt = att2.IssuedAt.AddMinutes(25), Notes = "Bank transfer verified." },
                new Payment { BookingId = att3.BookingId, Amount = att3.TotalAmount, Method = "ONLINE_TRANSFER", PaymentStatus = "SUCCESS", PaidAt = att3.IssuedAt.AddMinutes(18), Notes = "Online transfer received." },

                new Payment { BookingId = f2_b1.BookingId, Amount = f2_b1.TotalAmount, Method = "CARD", PaymentStatus = "SUCCESS", PaidAt = f2_b1.IssuedAt.AddMinutes(8), Notes = "Card payment confirmed." },
                new Payment { BookingId = f2_b2.BookingId, Amount = f2_b2.TotalAmount, Method = "ONLINE_TRANSFER", PaymentStatus = "SUCCESS", PaidAt = f2_b2.IssuedAt.AddMinutes(20), Notes = "Online transfer received." },

                new Payment { BookingId = f3_b1.BookingId, Amount = f3_b1.TotalAmount, Method = "CARD", PaymentStatus = "SUCCESS", PaidAt = f3_b1.IssuedAt.AddMinutes(10), Notes = "Card payment confirmed." },
                new Payment { BookingId = f3_b2.BookingId, Amount = f3_b2.TotalAmount, Method = "BANK_TRANSFER", PaymentStatus = "SUCCESS", PaidAt = f3_b2.IssuedAt.AddMinutes(22), Notes = "Bank transfer verified." },

                new Payment { BookingId = att4.BookingId, Amount = att4.TotalAmount, Method = "CARD", PaymentStatus = "SUCCESS", PaidAt = att4.IssuedAt.AddMinutes(9), Notes = "Card payment confirmed." }
            );
            db.SaveChanges();

            // Update sold seats for finished sessions
            ApplySessionSoldSeats(db, fe_s1.SessionId);
            ApplySessionSoldSeats(db, fe_s2.SessionId);
            ApplySessionSoldSeats(db, f2_s1.SessionId);
            ApplySessionSoldSeats(db, f3_s1.SessionId);
            ApplySessionSoldSeats(db, he_past.SessionId);

            // Reviews
            db.Reviews.AddRange(

                // F1
                new Review
                {
                    UserId = m1.UserId,
                    EventId = finishedEvent.EventId,
                    Rating = 5,
                    Comment = "Really well run. Entry was quick, staff were helpful, and the programme flowed smoothly. Sound balance was excellent from our seats.",
                    ReviewDate = fe_s1.StartDatetime.AddDays(1),
                    IsApproved = "Y",
                    IsApprovedBy = admin.UserId
                },
                new Review
                {
                    UserId = m2.UserId,
                    EventId = finishedEvent.EventId,
                    Rating = 4,
                    Comment = "Great atmosphere and strong performances. The venue signage and ushering made it easy to find seats. Intermission queueing was managed well.",
                    ReviewDate = fe_s2.StartDatetime.AddDays(1),
                    IsApproved = "Y",
                    IsApprovedBy = admin.UserId
                },

                // F2
                new Review
                {
                    UserId = m4.UserId,
                    EventId = finishedEvent2.EventId,
                    Rating = 5,
                    Comment = "Fantastic theatre night. Clear announcements, great pacing, and seating was managed without interruptions. Staff support was excellent.",
                    ReviewDate = f2_s1.StartDatetime.AddDays(1),
                    IsApproved = "Y",
                    IsApprovedBy = admin.UserId
                },
                new Review
                {
                    UserId = m1.UserId,
                    EventId = finishedEvent2.EventId,
                    Rating = 4,
                    Comment = "Very organised event with smooth entry checks. The show timing was accurate and the audience flow felt controlled.",
                    ReviewDate = f2_s1.StartDatetime.AddDays(2),
                    IsApproved = "Y",
                    IsApprovedBy = admin.UserId
                },

                // F3
                new Review
                {
                    UserId = m2.UserId,
                    EventId = finishedEvent3.EventId,
                    Rating = 5,
                    Comment = "Beautiful dance programme and very professional organisation. Intermission and re-entry were handled smoothly with clear guidance.",
                    ReviewDate = f3_s1.StartDatetime.AddDays(1),
                    IsApproved = "Y",
                    IsApprovedBy = admin.UserId
                },
                new Review
                {
                    UserId = m3.UserId,
                    EventId = finishedEvent3.EventId,
                    Rating = 4,
                    Comment = "Great performances and good crowd control. Seating directions were clear and stage transitions were impressive.",
                    ReviewDate = f3_s1.StartDatetime.AddDays(2),
                    IsApproved = "Y",
                    IsApprovedBy = admin.UserId
                },

                // Hybrid past (existing)
                new Review
                {
                    UserId = m4.UserId,
                    EventId = hybridEvent.EventId,
                    Rating = 5,
                    Comment = "The stage setup was clear from most angles and the stewards kept everything moving. Looking forward to the upcoming session too.",
                    ReviewDate = he_past.StartDatetime.AddDays(2),
                    IsApproved = "Y",
                    IsApprovedBy = admin.UserId
                }
            );
            db.SaveChanges();

            // Bookings Upcoming
            var b1 = new Booking
            {
                UserId = m1.UserId,
                SessionId = ue_s1.SessionId,
                BookingCode = "BK-LON-UE-1001",
                BookingDate = now.AddDays(-4),
                BookingStatus = "CONFIRMED",
                TotalAmount = (pStd * 2) + (pVip * 1),
                IssuedAt = now.AddDays(-4).AddMinutes(12),
                IsUsed = "N",
                CheckedInBy = null
            };

            var b2 = new Booking
            {
                UserId = m2.UserId,
                SessionId = ue_s1.SessionId,
                BookingCode = "BK-LON-UE-1002",
                BookingDate = now.AddDays(-2),
                BookingStatus = "PENDING",
                TotalAmount = (pBal * 2),
                IssuedAt = now.AddDays(-2).AddMinutes(7),
                IsUsed = "N",
                CheckedInBy = null
            };

            var b3 = new Booking
            {
                UserId = m3.UserId,
                SessionId = he_future.SessionId,
                BookingCode = "BK-LON-HE-2001",
                BookingDate = now.AddDays(-6),
                BookingStatus = "CONFIRMED",
                TotalAmount = (pStd * 2),
                IssuedAt = now.AddDays(-6).AddMinutes(9),
                IsUsed = "N",
                CheckedInBy = null
            };

            db.Bookings.AddRange(b1, b2, b3);
            db.SaveChanges();

            db.BookedTickets.AddRange(
                new BookedTickets
                {
                    BookingId = b1.BookingId,
                    TicketTypeId = tt[ue_s1.SessionId][STD].TicketTypeId,
                    Quantity = 2,
                    UnitPrice = pStd,
                    LineTotal = pStd * 2
                },
                new BookedTickets
                {
                    BookingId = b1.BookingId,
                    TicketTypeId = tt[ue_s1.SessionId][VIP].TicketTypeId,
                    Quantity = 1,
                    UnitPrice = pVip,
                    LineTotal = pVip
                },
                new BookedTickets
                {
                    BookingId = b2.BookingId,
                    TicketTypeId = tt[ue_s1.SessionId][BAL].TicketTypeId,
                    Quantity = 2,
                    UnitPrice = pBal,
                    LineTotal = pBal * 2
                },
                new BookedTickets
                {
                    BookingId = b3.BookingId,
                    TicketTypeId = tt[he_future.SessionId][STD].TicketTypeId,
                    Quantity = 2,
                    UnitPrice = pStd,
                    LineTotal = pStd * 2
                }
            );
            db.SaveChanges();

            db.Payments.AddRange(
                new Payment { BookingId = b1.BookingId, Amount = b1.TotalAmount, Method = "CARD", PaymentStatus = "SUCCESS", PaidAt = now.AddDays(-4).AddMinutes(25), Notes = "Card payment confirmed." },
                new Payment { BookingId = b3.BookingId, Amount = b3.TotalAmount, Method = "ONLINE_TRANSFER", PaymentStatus = "SUCCESS", PaidAt = now.AddDays(-6).AddMinutes(22), Notes = "Online transfer received." },
                new Payment { BookingId = b2.BookingId, Amount = b2.TotalAmount, Method = "BANK_TRANSFER", PaymentStatus = "PENDING", PaidAt = null, Notes = "Awaiting payment confirmation." }
            );
            db.SaveChanges();

            ApplySessionSoldSeats(db, ue_s1.SessionId);
            ApplySessionSoldSeats(db, he_future.SessionId);

            // Inquiries
            db.Inquiries.AddRange(
                new Inquiry
                {
                    UserId = null, // guest
                    Email = "laura.mitchell@gmail.com",
                    Subject = "Seating view for balcony tickets",
                    Message = "Hi, do balcony seats at Royal Festival Hall have any restricted view sections for this event?",
                    InquiryType = "EVENT",
                    CreatedAt = now.AddDays(-3),
                    Status = "OPEN",
                    Reply = null,
                    RepliedAt = null,
                    RepliedBy = admin.UserId
                },
                new Inquiry
                {
                    UserId = null, // guest
                    Email = "tom.edwards@outlook.com",
                    Subject = "Accessibility and step-free access",
                    Message = "Do you have step-free access and accessible seating options at Barbican Centre?",
                    InquiryType = "GENERAL",
                    CreatedAt = now.AddDays(-2),
                    Status = "CLOSED",
                    Reply = "Yes. Step-free access is available. Please mention accessibility needs when booking so staff can assist you on arrival.",
                    RepliedAt = now.AddDays(-2).AddHours(4),
                    RepliedBy = admin.UserId
                },
                new Inquiry
                {
                    UserId = m2.UserId, // member
                    Email = m2.Email,
                    Subject = "Updating my contact number",
                    Message = "Hi team, my phone number has changed. Where can I update it in my profile?",
                    InquiryType = "MEMBERSHIP",
                    CreatedAt = now.AddDays(-5),
                    Status = "CLOSED",
                    Reply = "Go to My Profile, update your contact number, and click Save. If you face any issue, reply here and we’ll help.",
                    RepliedAt = now.AddDays(-4),
                    RepliedBy = admin.UserId
                }
            );
            db.SaveChanges();
        }

        private static void ApplySessionSoldSeats(ApplicationDbContext db, int sessionId)
        {
            var session = db.EventSessions.FirstOrDefault(s => s.SessionId == sessionId);
            if (session == null) return;

            var sold = db.BookedTickets
                .Join(db.TicketTypes,
                    bt => bt.TicketTypeId,
                    tt => tt.TicketTypeId,
                    (bt, ttype) => new { bt, ttype })
                .Where(x => x.ttype.SessionId == sessionId)
                .Join(db.Bookings,
                    x => x.bt.BookingId,
                    b => b.BookingId,
                    (x, b) => new { x.bt, b })
                .Where(x => x.b.BookingStatus == "CONFIRMED")
                .Sum(x => x.bt.Quantity);

            session.SoldSeats = sold;
            if (session.SoldSeats < 0) session.SoldSeats = 0;

            db.SaveChanges();
        }
    }
}