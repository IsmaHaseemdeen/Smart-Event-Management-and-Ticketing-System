using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventTicketingSystem.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "category",
                columns: table => new
                {
                    category_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    category_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_category", x => x.category_id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    full_name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    email = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.user_id);
                    table.CheckConstraint("user_chk_role", "role IN ('ADMIN','STAFF','MEMBER')");
                    table.CheckConstraint("user_chk_status", "status IN ('ACTIVE','INACTIVE')");
                });

            migrationBuilder.CreateTable(
                name: "venue",
                columns: table => new
                {
                    venue_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    venue_name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    addressline_1 = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    addressline_2 = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    city = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    capacity = table.Column<int>(type: "int", nullable: true),
                    phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_venue", x => x.venue_id);
                });

            migrationBuilder.CreateTable(
                name: "inquiry",
                columns: table => new
                {
                    inquiry_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: true),
                    email = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    subject = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    message = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    inquiry_type = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    reply = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    replied_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    replied_by = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inquiry", x => x.inquiry_id);
                    table.CheckConstraint("inquiry_chk_status", "status IN ('OPEN','CLOSED')");
                    table.CheckConstraint("inquiry_chk_type", "inquiry_type IN ('GENERAL','EVENT','MEMBERSHIP','TECHNICAL')");
                    table.ForeignKey(
                        name: "inquiry_user_fk",
                        column: x => x.replied_by,
                        principalTable: "User",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "inquiry_userid_fk",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "event",
                columns: table => new
                {
                    event_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    venue_id = table.Column<int>(type: "int", nullable: false),
                    created_by = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    poster_url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event", x => x.event_id);
                    table.CheckConstraint("event_chk_status", "status IN ('ACTIVE','FINISHED','CANCELLED')");
                    table.ForeignKey(
                        name: "event_user_fk",
                        column: x => x.created_by,
                        principalTable: "User",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "event_venue_fk",
                        column: x => x.venue_id,
                        principalTable: "venue",
                        principalColumn: "venue_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "event_category",
                columns: table => new
                {
                    event_id = table.Column<int>(type: "int", nullable: false),
                    category_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_category", x => new { x.event_id, x.category_id });
                    table.ForeignKey(
                        name: "evcat_cat_fk",
                        column: x => x.category_id,
                        principalTable: "category",
                        principalColumn: "category_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "evcat_event_fk",
                        column: x => x.event_id,
                        principalTable: "event",
                        principalColumn: "event_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "event_session",
                columns: table => new
                {
                    session_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    event_id = table.Column<int>(type: "int", nullable: false),
                    session_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    start_datetime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    end_datetime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    session_status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    notes = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    total_seats = table.Column<int>(type: "int", nullable: false),
                    reserved_seats = table.Column<int>(type: "int", nullable: false),
                    sold_seats = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_session", x => x.session_id);
                    table.CheckConstraint("event_session_chk_status", "session_status IN ('ACTIVE','CANCELLED','FINISHED')");
                    table.CheckConstraint("event_session_ck_2", "reserved_seats >= 0 AND sold_seats >= 0");
                    table.CheckConstraint("event_session_ck_capacity", "reserved_seats + sold_seats <= total_seats");
                    table.CheckConstraint("event_session_ck_total", "total_seats > 0");
                    table.ForeignKey(
                        name: "event_session_event_fk",
                        column: x => x.event_id,
                        principalTable: "event",
                        principalColumn: "event_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "review",
                columns: table => new
                {
                    review_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    event_id = table.Column<int>(type: "int", nullable: false),
                    rating = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(name: "\"Comment\"", type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    review_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    isapproved = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    isapproved_by = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_review", x => x.review_id);
                    table.CheckConstraint("review_chk_rating", "rating BETWEEN 1 AND 5");
                    table.ForeignKey(
                        name: "review_event_fk",
                        column: x => x.event_id,
                        principalTable: "event",
                        principalColumn: "event_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "review_user_fk",
                        column: x => x.isapproved_by,
                        principalTable: "User",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "review_userid_fk",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "booking",
                columns: table => new
                {
                    booking_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    session_id = table.Column<int>(type: "int", nullable: false),
                    booking_code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    booking_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    booking_status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    total_amount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    issued_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    isused = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: false),
                    checkedin_by = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_booking", x => x.booking_id);
                    table.CheckConstraint("booking_chk_status", "booking_status IN ('PENDING','CONFIRMED')");
                    table.ForeignKey(
                        name: "booking_event_session_fk",
                        column: x => x.session_id,
                        principalTable: "event_session",
                        principalColumn: "session_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "booking_user_fk",
                        column: x => x.checkedin_by,
                        principalTable: "User",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "booking_userid_fk",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ticket_type",
                columns: table => new
                {
                    tickettype_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    session_id = table.Column<int>(type: "int", nullable: false),
                    type_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    price = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ticket_type", x => x.tickettype_id);
                    table.CheckConstraint("ticket_type_chk_status", "status IN ('ACTIVE','CANCELLED','FINISHED')");
                    table.ForeignKey(
                        name: "ticket_type_event_session_fk",
                        column: x => x.session_id,
                        principalTable: "event_session",
                        principalColumn: "session_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "payment",
                columns: table => new
                {
                    payment_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    booking_id = table.Column<int>(type: "int", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    method = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    payment_status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    paid_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    notes = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payment", x => x.payment_id);
                    table.CheckConstraint("payment_chk_method", "method IN ('CARD','BANK_TRANSFER','ONLINE_TRANSFER')");
                    table.CheckConstraint("payment_chk_status", "payment_status IN ('PENDING','SUCCESS','FAILED')");
                    table.ForeignKey(
                        name: "payment_booking_fk",
                        column: x => x.booking_id,
                        principalTable: "booking",
                        principalColumn: "booking_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "booked_tickets",
                columns: table => new
                {
                    bookedticket_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    booking_id = table.Column<int>(type: "int", nullable: false),
                    tickettype_id = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    unit_price = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    line_total = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_booked_tickets", x => x.bookedticket_id);
                    table.CheckConstraint("booked_tickets_chk_quantity", "quantity > 0");
                    table.ForeignKey(
                        name: "booked_tickets_booking_fk",
                        column: x => x.booking_id,
                        principalTable: "booking",
                        principalColumn: "booking_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "booked_tickets_ticket_type_fk",
                        column: x => x.tickettype_id,
                        principalTable: "ticket_type",
                        principalColumn: "tickettype_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_booked_tickets_booking_id",
                table: "booked_tickets",
                column: "booking_id");

            migrationBuilder.CreateIndex(
                name: "IX_booked_tickets_tickettype_id",
                table: "booked_tickets",
                column: "tickettype_id");

            migrationBuilder.CreateIndex(
                name: "IX_booking_booking_code",
                table: "booking",
                column: "booking_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_booking_checkedin_by",
                table: "booking",
                column: "checkedin_by");

            migrationBuilder.CreateIndex(
                name: "IX_booking_session_id",
                table: "booking",
                column: "session_id");

            migrationBuilder.CreateIndex(
                name: "IX_booking_user_id",
                table: "booking",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_event_created_by",
                table: "event",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_event_venue_id",
                table: "event",
                column: "venue_id");

            migrationBuilder.CreateIndex(
                name: "IX_event_category_category_id",
                table: "event_category",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_event_session_event_id",
                table: "event_session",
                column: "event_id");

            migrationBuilder.CreateIndex(
                name: "IX_inquiry_replied_by",
                table: "inquiry",
                column: "replied_by");

            migrationBuilder.CreateIndex(
                name: "IX_inquiry_user_id",
                table: "inquiry",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_payment_booking_id",
                table: "payment",
                column: "booking_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_review_event_id",
                table: "review",
                column: "event_id");

            migrationBuilder.CreateIndex(
                name: "IX_review_isapproved_by",
                table: "review",
                column: "isapproved_by");

            migrationBuilder.CreateIndex(
                name: "IX_review_user_id_event_id",
                table: "review",
                columns: new[] { "user_id", "event_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ticket_type_session_id_type_name",
                table: "ticket_type",
                columns: new[] { "session_id", "type_name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_email",
                table: "User",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "booked_tickets");

            migrationBuilder.DropTable(
                name: "event_category");

            migrationBuilder.DropTable(
                name: "inquiry");

            migrationBuilder.DropTable(
                name: "payment");

            migrationBuilder.DropTable(
                name: "review");

            migrationBuilder.DropTable(
                name: "ticket_type");

            migrationBuilder.DropTable(
                name: "category");

            migrationBuilder.DropTable(
                name: "booking");

            migrationBuilder.DropTable(
                name: "event_session");

            migrationBuilder.DropTable(
                name: "event");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "venue");
        }
    }
}
