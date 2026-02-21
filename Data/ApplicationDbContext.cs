using EventTicketingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EventTicketingSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Users> Users { get; set; } = null!;
        public DbSet<Venue> Venues { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<EventCategory> EventCategories { get; set; } = null!;
        public DbSet<EventSession> EventSessions { get; set; } = null!;
        public DbSet<TicketType> TicketTypes { get; set; } = null!;
        public DbSet<MemberProfile> MemberProfiles { get; set; } = null!;
        public DbSet<MemberPreference> MemberPreferences { get; set; } = null!;
        public DbSet<Booking> Bookings { get; set; } = null!;
        public DbSet<BookedTickets> BookedTickets { get; set; } = null!;
        public DbSet<Payment> Payments { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;
        public DbSet<Inquiry> Inquiries { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Users>(e =>
            {
                e.ToTable("USER");
                e.HasKey(x => x.UserId);

                e.Property(x => x.UserId).HasColumnName("user_id");
                e.Property(x => x.FullName).HasColumnName("full_name").HasMaxLength(120).IsRequired();
                e.Property(x => x.Email).HasColumnName("email").HasMaxLength(120).IsRequired();
                e.Property(x => x.Phone).HasColumnName("phone").HasMaxLength(20).IsRequired();
                e.Property(x => x.Password).HasColumnName("password").HasMaxLength(255).IsRequired();
                e.Property(x => x.Role).HasColumnName("role").HasMaxLength(20).IsRequired();
                e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20).IsRequired();
                e.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();

                e.HasIndex(x => x.Email).IsUnique();

                e.ToTable(t => t.HasCheckConstraint("user_chk_role", "role IN ('ADMIN','STAFF','MEMBER')"));
                e.ToTable(t => t.HasCheckConstraint("user_chk_status", "status IN ('ACTIVE','INACTIVE')"));
            });

            modelBuilder.Entity<Venue>(e =>
            {
                e.ToTable("venue");
                e.HasKey(x => x.VenueId);

                e.Property(x => x.VenueId).HasColumnName("venue_id");
                e.Property(x => x.VenueName).HasColumnName("venue_name").HasMaxLength(120).IsRequired();
                e.Property(x => x.AddressLine1).HasColumnName("addressline_1").HasMaxLength(120);
                e.Property(x => x.AddressLine2).HasColumnName("addressline_2").HasMaxLength(120);
                e.Property(x => x.City).HasColumnName("city").HasMaxLength(60).IsRequired();
                e.Property(x => x.Capacity).HasColumnName("capacity");
                e.Property(x => x.Phone).HasColumnName("phone").HasMaxLength(20);
            });

            modelBuilder.Entity<Category>(e =>
            {
                e.ToTable("category");
                e.HasKey(x => x.CategoryId);

                e.Property(x => x.CategoryId).HasColumnName("category_id");
                e.Property(x => x.CategoryName).HasColumnName("category_name").HasMaxLength(50).IsRequired();
                e.Property(x => x.Description).HasColumnName("description").HasMaxLength(200);
            });

            modelBuilder.Entity<Event>(e =>
            {
                e.ToTable("event");
                e.HasKey(x => x.EventId);

                e.Property(x => x.EventId).HasColumnName("event_id");
                e.Property(x => x.VenueId).HasColumnName("venue_id").IsRequired();
                e.Property(x => x.CreatedBy).HasColumnName("created_by").IsRequired();
                e.Property(x => x.Title).HasColumnName("title").HasMaxLength(150).IsRequired();
                e.Property(x => x.Description).HasColumnName("description").HasMaxLength(2000);
                e.Property(x => x.PosterUrl).HasColumnName("poster_url").HasMaxLength(255);
                e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20).IsRequired();
                e.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();

                e.HasOne(x => x.Venue)
                    .WithMany(v => v.Events)
                    .HasForeignKey(x => x.VenueId)
                    .HasConstraintName("event_venue_fk")
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.CreatedByUser)
                    .WithMany(u => u.CreatedEvents)
                    .HasForeignKey(x => x.CreatedBy)
                    .HasConstraintName("event_user_fk")
                    .OnDelete(DeleteBehavior.Restrict);

                e.ToTable(t => t.HasCheckConstraint("event_chk_status", "status IN ('ACTIVE','FINISHED','CANCELLED')"));
            });

            modelBuilder.Entity<EventCategory>(e =>
            {
                e.ToTable("event_category");
                e.HasKey(x => new { x.EventId, x.CategoryId });

                e.Property(x => x.EventId).HasColumnName("event_id");
                e.Property(x => x.CategoryId).HasColumnName("category_id");

                e.HasOne(x => x.Event)
                    .WithMany(ev => ev.EventCategories)
                    .HasForeignKey(x => x.EventId)
                    .HasConstraintName("evcat_event_fk")
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Category)
                    .WithMany(c => c.EventCategories)
                    .HasForeignKey(x => x.CategoryId)
                    .HasConstraintName("evcat_cat_fk")
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<EventSession>(e =>
            {
                e.ToTable("event_session");
                e.HasKey(x => x.SessionId);

                e.Property(x => x.SessionId).HasColumnName("session_id");
                e.Property(x => x.EventId).HasColumnName("event_id").IsRequired();
                e.Property(x => x.SessionDate).HasColumnName("session_date").IsRequired();
                e.Property(x => x.StartDatetime).HasColumnName("start_datetime").IsRequired();
                e.Property(x => x.EndDatetime).HasColumnName("end_datetime");
                e.Property(x => x.SessionStatus).HasColumnName("session_status").HasMaxLength(20).IsRequired();
                e.Property(x => x.Notes).HasColumnName("notes").HasMaxLength(200);
                e.Property(x => x.TotalSeats).HasColumnName("total_seats").IsRequired();

                e.Property(x => x.ReservedSeats).HasColumnName("reserved_seats").IsRequired();
                e.Property(x => x.SoldSeats).HasColumnName("sold_seats").IsRequired();

                e.HasOne(x => x.Event)
                    .WithMany(ev => ev.Sessions)
                    .HasForeignKey(x => x.EventId)
                    .HasConstraintName("event_session_event_fk")
                    .OnDelete(DeleteBehavior.Restrict);

                e.ToTable(t => t.HasCheckConstraint("event_session_chk_status", "session_status IN ('ACTIVE','CANCELLED','FINISHED')"));

                e.ToTable(t => t.HasCheckConstraint("event_session_ck_2", "reserved_seats >= 0 AND sold_seats >= 0"));

                e.ToTable(t => t.HasCheckConstraint("event_session_ck_total", "total_seats > 0"));

                e.ToTable(t => t.HasCheckConstraint("event_session_ck_capacity", "reserved_seats + sold_seats <= total_seats"));
            });

            modelBuilder.Entity<TicketType>(e =>
            {
                e.ToTable("ticket_type");
                e.HasKey(x => x.TicketTypeId);

                e.Property(x => x.TicketTypeId).HasColumnName("tickettype_id");
                e.Property(x => x.SessionId).HasColumnName("session_id").IsRequired();
                e.Property(x => x.TypeName).HasColumnName("type_name").HasMaxLength(50).IsRequired();
                e.Property(x => x.Price).HasColumnName("price").HasPrecision(10, 2).IsRequired();
                e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20).IsRequired();

                e.HasIndex(x => new { x.SessionId, x.TypeName }).IsUnique();

                e.HasOne(x => x.Session)
                    .WithMany(s => s.TicketTypes)
                    .HasForeignKey(x => x.SessionId)
                    .HasConstraintName("ticket_type_event_session_fk")
                    .OnDelete(DeleteBehavior.Restrict);

                e.ToTable(t => t.HasCheckConstraint("ticket_type_chk_status", "status IN ('ACTIVE','CANCELLED','FINISHED')"));
            });

            modelBuilder.Entity<MemberProfile>(e =>
            {
                e.ToTable("member_profile");
                e.HasKey(x => x.MemberProfileId);

                e.Property(x => x.MemberProfileId).HasColumnName("memberprofile_id");
                e.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
                e.Property(x => x.FullName).HasColumnName("full_name").HasMaxLength(120).IsRequired();
                e.Property(x => x.ContactNumber).HasColumnName("contact_number").HasMaxLength(20).IsRequired();
                e.Property(x => x.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
                e.Property(x => x.DateOfBirth).HasColumnName("date_of_birth");
                e.Property(x => x.Address).HasColumnName("address").HasMaxLength(200);
                e.Property(x => x.City).HasColumnName("city").HasMaxLength(60).IsRequired();
                e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20).IsRequired();
                e.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();

                e.HasIndex(x => x.UserId).IsUnique();

                e.HasOne(x => x.User)
                    .WithOne(u => u.MemberProfile)
                    .HasForeignKey<MemberProfile>(x => x.UserId)
                    .HasConstraintName("member_profile_user_fk")
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<MemberPreference>(e =>
            {
                e.ToTable("member_preference");
                e.HasKey(x => new { x.MemberProfileId, x.CategoryId });

                e.Property(x => x.MemberProfileId).HasColumnName("memberprofile_id");
                e.Property(x => x.CategoryId).HasColumnName("category_id");
                e.Property(x => x.PreferenceLevel).HasColumnName("preference_level").HasMaxLength(20);
                e.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();

                e.HasOne(x => x.MemberProfile)
                    .WithMany(m => m.Preferences)
                    .HasForeignKey(x => x.MemberProfileId)
                    .HasConstraintName("mp_member_fk")
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Category)
                    .WithMany(c => c.MemberPreferences)
                    .HasForeignKey(x => x.CategoryId)
                    .HasConstraintName("mp_cat_fk")
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Booking>(e =>
            {
                e.ToTable("booking");
                e.HasKey(x => x.BookingId);

                e.Property(x => x.BookingId).HasColumnName("booking_id");
                e.Property(x => x.MemberProfileId).HasColumnName("memberprofile_id").IsRequired();
                e.Property(x => x.SessionId).HasColumnName("session_id").IsRequired();
                e.Property(x => x.BookingCode).HasColumnName("booking_code").HasMaxLength(30).IsRequired();
                e.Property(x => x.BookingDate).HasColumnName("booking_date").IsRequired();
                e.Property(x => x.BookingStatus).HasColumnName("booking_status").HasMaxLength(20).IsRequired();

                e.Property(x => x.TotalAmount).HasColumnName("total_amount").HasPrecision(10, 2).IsRequired();

                e.Property(x => x.IssuedAt).HasColumnName("issued_at").IsRequired();
                e.Property(x => x.IsUsed).HasColumnName("isused").HasMaxLength(1).IsRequired();
                e.Property(x => x.CheckedInBy).HasColumnName("checkedin_by");

                e.HasIndex(x => x.BookingCode).IsUnique();

                e.HasOne(x => x.MemberProfile)
                    .WithMany(m => m.Bookings)
                    .HasForeignKey(x => x.MemberProfileId)
                    .HasConstraintName("booking_member_profile_fk")
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Session)
                    .WithMany(s => s.Bookings)
                    .HasForeignKey(x => x.SessionId)
                    .HasConstraintName("booking_event_session_fk")
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.CheckedInByUser)
                    .WithMany(u => u.CheckedInBookings)
                    .HasForeignKey(x => x.CheckedInBy)
                    .HasConstraintName("booking_user_fk")
                    .OnDelete(DeleteBehavior.Restrict);

                e.ToTable(t => t.HasCheckConstraint("booking_chk_status", "booking_status IN ('PENDING','CONFIRMED')"));
            });

            modelBuilder.Entity<BookedTickets>(e =>
            {
                e.ToTable("booked_tickets");
                e.HasKey(x => x.BookedTicketId);

                e.Property(x => x.BookedTicketId).HasColumnName("bookedticket_id");
                e.Property(x => x.BookingId).HasColumnName("booking_id").IsRequired();
                e.Property(x => x.TicketTypeId).HasColumnName("tickettype_id").IsRequired();
                e.Property(x => x.Quantity).HasColumnName("quantity").IsRequired();
                e.Property(x => x.UnitPrice).HasColumnName("unit_price").HasPrecision(10, 2).IsRequired();
                e.Property(x => x.LineTotal).HasColumnName("line_total").HasPrecision(10, 2).IsRequired();

                e.HasOne(x => x.Booking)
                    .WithMany(b => b.BookedTickets)
                    .HasForeignKey(x => x.BookingId)
                    .HasConstraintName("booked_tickets_booking_fk")
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.TicketType)
                    .WithMany(t => t.BookedTickets)
                    .HasForeignKey(x => x.TicketTypeId)
                    .HasConstraintName("booked_tickets_ticket_type_fk")
                    .OnDelete(DeleteBehavior.Restrict);

                e.ToTable(t => t.HasCheckConstraint("booked_tickets_chk_quantity", "quantity > 0"));
            });

            modelBuilder.Entity<Payment>(e =>
            {
                e.ToTable("payment");
                e.HasKey(x => x.PaymentId);

                e.Property(x => x.PaymentId).HasColumnName("payment_id");
                e.Property(x => x.BookingId).HasColumnName("booking_id").IsRequired();
                e.Property(x => x.Amount).HasColumnName("amount").HasPrecision(10, 2).IsRequired();
                e.Property(x => x.Method).HasColumnName("method").HasMaxLength(30).IsRequired();
                e.Property(x => x.PaymentStatus).HasColumnName("payment_status").HasMaxLength(20).IsRequired();
                e.Property(x => x.PaidAt).HasColumnName("paid_at");
                e.Property(x => x.Notes).HasColumnName("notes").HasMaxLength(200);

                e.HasIndex(x => x.BookingId).IsUnique();

                e.HasOne(x => x.Booking)
                    .WithOne(b => b.Payment)
                    .HasForeignKey<Payment>(x => x.BookingId)
                    .HasConstraintName("payment_booking_fk")
                    .OnDelete(DeleteBehavior.Restrict);

                e.ToTable(t => t.HasCheckConstraint("payment_chk_method", "method IN ('CARD','BANK_TRANSFER','ONLINE_TRANSFER')"));
                e.ToTable(t => t.HasCheckConstraint("payment_chk_status", "payment_status IN ('PENDING','SUCCESS','FAILED')"));
            });

            modelBuilder.Entity<Inquiry>(e =>
            {
                e.ToTable("inquiry");
                e.HasKey(x => x.InquiryId);

                e.Property(x => x.InquiryId).HasColumnName("inquiry_id");
                e.Property(x => x.MemberProfileId).HasColumnName("memberprofile_id");
                e.Property(x => x.Email).HasColumnName("email").HasMaxLength(120).IsRequired();
                e.Property(x => x.Subject).HasColumnName("subject").HasMaxLength(150).IsRequired();
                e.Property(x => x.Message).HasColumnName("message").HasMaxLength(1000).IsRequired();
                e.Property(x => x.InquiryType).HasColumnName("inquiry_type").HasMaxLength(30).IsRequired();
                e.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
                e.Property(x => x.Status).HasColumnName("status").HasMaxLength(20).IsRequired();
                e.Property(x => x.Reply).HasColumnName("reply").HasMaxLength(1000);
                e.Property(x => x.RepliedAt).HasColumnName("replied_at");
                e.Property(x => x.RepliedBy).HasColumnName("replied_by").IsRequired();

                e.HasOne(x => x.MemberProfile)
                    .WithMany(m => m.Inquiries)
                    .HasForeignKey(x => x.MemberProfileId)
                    .HasConstraintName("inquiry_member_profile_fk")
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.RepliedByUser)
                    .WithMany(u => u.RepliedInquiries)
                    .HasForeignKey(x => x.RepliedBy)
                    .HasConstraintName("inquiry_user_fk")
                    .OnDelete(DeleteBehavior.Restrict);

                e.ToTable(t => t.HasCheckConstraint("inquiry_chk_status", "status IN ('OPEN','CLOSED')"));
               
                e.ToTable(t => t.HasCheckConstraint("inquiry_chk_type", "inquiry_type IN ('GENERAL','EVENT','MEMBERSHIP','TECHNICAL')"));
            });

            modelBuilder.Entity<Review>(e =>
            {
                e.ToTable("review");
                e.HasKey(x => x.ReviewId);

                e.Property(x => x.ReviewId).HasColumnName("review_id");
                e.Property(x => x.MemberProfileId).HasColumnName("memberprofile_id").IsRequired();
                e.Property(x => x.EventId).HasColumnName("event_id").IsRequired();
                e.Property(x => x.Rating).HasColumnName("rating").IsRequired();
                e.Property(x => x.Comment).HasColumnName("\"Comment\"").HasMaxLength(1000);
                e.Property(x => x.ReviewDate).HasColumnName("review_date").IsRequired();
                e.Property(x => x.IsApproved).HasColumnName("isapproved").HasMaxLength(1).IsRequired();
                e.Property(x => x.IsApprovedBy).HasColumnName("isapproved_by").IsRequired();

                e.HasIndex(x => new { x.MemberProfileId, x.EventId }).IsUnique();

                e.HasOne(x => x.MemberProfile)
                    .WithMany(m => m.Reviews)
                    .HasForeignKey(x => x.MemberProfileId)
                    .HasConstraintName("review_member_profile_fk")
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Event)
                    .WithMany(ev => ev.Reviews)
                    .HasForeignKey(x => x.EventId)
                    .HasConstraintName("review_event_fk")
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.ApprovedByUser)
                    .WithMany(u => u.ApprovedReviews)
                    .HasForeignKey(x => x.IsApprovedBy)
                    .HasConstraintName("review_user_fk")
                    .OnDelete(DeleteBehavior.Restrict);

                e.ToTable(t => t.HasCheckConstraint("review_chk_rating", "rating BETWEEN 1 AND 5"));
            });
        }
    }
}