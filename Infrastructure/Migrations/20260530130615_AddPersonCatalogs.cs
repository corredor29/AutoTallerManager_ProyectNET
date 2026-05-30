using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonCatalogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_appointments_appointment_statuses_appointment_status_id",
                table: "appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_appointments_users_assigned_user_id",
                table: "appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_customers_persons_person_id",
                table: "customers");

            migrationBuilder.DropForeignKey(
                name: "FK_purchase_orders_users_user_id",
                table: "purchase_orders");

            migrationBuilder.DropForeignKey(
                name: "FK_quotations_users_created_by_user_id",
                table: "quotations");

            migrationBuilder.DropForeignKey(
                name: "FK_service_orders_order_statuses_order_status_id",
                table: "service_orders");

            migrationBuilder.DropForeignKey(
                name: "FK_service_orders_users_mechanic_id",
                table: "service_orders");

            migrationBuilder.DropForeignKey(
                name: "FK_user_roles_roles_role_id",
                table: "user_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_user_roles_users_user_id",
                table: "user_roles");

            migrationBuilder.DropForeignKey(
                name: "FK_users_persons_person_id",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "FK_vehicle_models_vehicle_brands_brand_id",
                table: "vehicle_models");

            migrationBuilder.DropForeignKey(
                name: "FK_vehicles_fuel_types_fuel_type_id",
                table: "vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_vehicles_transmission_types_transmission_type_id",
                table: "vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_vehicles_vehicle_colors_color_id",
                table: "vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_vehicles_vehicle_models_model_id",
                table: "vehicles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_users",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_roles",
                table: "roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_persons",
                table: "persons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_vehicle_models",
                table: "vehicle_models");

            migrationBuilder.DropPrimaryKey(
                name: "PK_vehicle_colors",
                table: "vehicle_colors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_vehicle_brands",
                table: "vehicle_brands");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_roles",
                table: "user_roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_transmission_types",
                table: "transmission_types");

            migrationBuilder.DropPrimaryKey(
                name: "PK_order_statuses",
                table: "order_statuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_fuel_types",
                table: "fuel_types");

            migrationBuilder.DropPrimaryKey(
                name: "PK_appointment_statuses",
                table: "appointment_statuses");

            migrationBuilder.RenameTable(
                name: "users",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "roles",
                newName: "Roles");

            migrationBuilder.RenameTable(
                name: "persons",
                newName: "Persons");

            migrationBuilder.RenameTable(
                name: "vehicle_models",
                newName: "VehicleModels");

            migrationBuilder.RenameTable(
                name: "vehicle_colors",
                newName: "VehicleColors");

            migrationBuilder.RenameTable(
                name: "vehicle_brands",
                newName: "VehicleBrands");

            migrationBuilder.RenameTable(
                name: "user_roles",
                newName: "UserRoles");

            migrationBuilder.RenameTable(
                name: "transmission_types",
                newName: "TransmissionTypes");

            migrationBuilder.RenameTable(
                name: "order_statuses",
                newName: "OrderStatuses");

            migrationBuilder.RenameTable(
                name: "fuel_types",
                newName: "FuelTypes");

            migrationBuilder.RenameTable(
                name: "appointment_statuses",
                newName: "AppointmentStatuses");

            migrationBuilder.RenameColumn(
                name: "person_id",
                table: "Users",
                newName: "PersonId");

            migrationBuilder.RenameColumn(
                name: "password_hash",
                table: "Users",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "is_active",
                table: "Users",
                newName: "IsActive");

            migrationBuilder.RenameIndex(
                name: "IX_users_person_id",
                table: "Users",
                newName: "IX_Users_PersonId");

            migrationBuilder.RenameColumn(
                name: "role_name",
                table: "Roles",
                newName: "RoleName");

            migrationBuilder.RenameIndex(
                name: "IX_roles_role_name",
                table: "Roles",
                newName: "IX_Roles_RoleName");

            migrationBuilder.RenameColumn(
                name: "registered_at",
                table: "Persons",
                newName: "RegisteredAt");

            migrationBuilder.RenameColumn(
                name: "last_name",
                table: "Persons",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "first_name",
                table: "Persons",
                newName: "FirstName");

            migrationBuilder.RenameColumn(
                name: "model_name",
                table: "VehicleModels",
                newName: "ModelName");

            migrationBuilder.RenameColumn(
                name: "brand_id",
                table: "VehicleModels",
                newName: "BrandId");

            migrationBuilder.RenameIndex(
                name: "IX_vehicle_models_brand_id_model_name",
                table: "VehicleModels",
                newName: "IX_VehicleModels_BrandId_ModelName");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "VehicleColors",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_vehicle_colors_name",
                table: "VehicleColors",
                newName: "IX_VehicleColors_Name");

            migrationBuilder.RenameColumn(
                name: "brand_name",
                table: "VehicleBrands",
                newName: "BrandName");

            migrationBuilder.RenameIndex(
                name: "IX_vehicle_brands_brand_name",
                table: "VehicleBrands",
                newName: "IX_VehicleBrands_BrandName");

            migrationBuilder.RenameColumn(
                name: "role_id",
                table: "UserRoles",
                newName: "RoleId");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "UserRoles",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_user_roles_role_id",
                table: "UserRoles",
                newName: "IX_UserRoles_RoleId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "TransmissionTypes",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_transmission_types_name",
                table: "TransmissionTypes",
                newName: "IX_TransmissionTypes_Name");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "OrderStatuses",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_order_statuses_name",
                table: "OrderStatuses",
                newName: "IX_OrderStatuses_Name");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "FuelTypes",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_fuel_types_name",
                table: "FuelTypes",
                newName: "IX_FuelTypes_Name");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "AppointmentStatuses",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_appointment_statuses_name",
                table: "AppointmentStatuses",
                newName: "IX_AppointmentStatuses_Name");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<string>(
                name: "ModelName",
                table: "VehicleModels",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "BrandName",
                table: "VehicleBrands",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Roles",
                table: "Roles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Persons",
                table: "Persons",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VehicleModels",
                table: "VehicleModels",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VehicleColors",
                table: "VehicleColors",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VehicleBrands",
                table: "VehicleBrands",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRoles",
                table: "UserRoles",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransmissionTypes",
                table: "TransmissionTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderStatuses",
                table: "OrderStatuses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FuelTypes",
                table: "FuelTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppointmentStatuses",
                table: "AppointmentStatuses",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AuditActionTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditActionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailDomains",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Domain = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailDomains", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MileageHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VehicleId = table.Column<int>(type: "integer", nullable: false),
                    Mileage = table.Column<int>(type: "integer", nullable: false),
                    RecordedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MileageHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MileageHistory_vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhoneCodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Country = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhoneCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VehicleOwnershipHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VehicleId = table.Column<int>(type: "integer", nullable: false),
                    CustomerId = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleOwnershipHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleOwnershipHistory_customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleOwnershipHistory_vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    AuditActionTypeId = table.Column<int>(type: "integer", nullable: false),
                    AffectedEntity = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AffectedRecordId = table.Column<int>(type: "integer", nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogs_AuditActionTypes_AuditActionTypeId",
                        column: x => x.AuditActionTypeId,
                        principalTable: "AuditActionTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuditLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PersonDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PersonId = table.Column<int>(type: "integer", nullable: false),
                    DocumentTypeId = table.Column<int>(type: "integer", nullable: false),
                    DocumentNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonDocuments_DocumentTypes_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DocumentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PersonDocuments_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonEmails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PersonId = table.Column<int>(type: "integer", nullable: false),
                    EmailDomainId = table.Column<int>(type: "integer", nullable: false),
                    EmailUser = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonEmails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonEmails_EmailDomains_EmailDomainId",
                        column: x => x.EmailDomainId,
                        principalTable: "EmailDomains",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PersonEmails_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonPhones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PersonId = table.Column<int>(type: "integer", nullable: false),
                    PhoneCodeId = table.Column<int>(type: "integer", nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonPhones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonPhones_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PersonPhones_PhoneCodes_PhoneCodeId",
                        column: x => x.PhoneCodeId,
                        principalTable: "PhoneCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditActionTypes_Name",
                table: "AuditActionTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_AuditActionTypeId",
                table: "AuditLogs",
                column: "AuditActionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_OccurredAt",
                table: "AuditLogs",
                column: "OccurredAt");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTypes_Code",
                table: "DocumentTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTypes_Name",
                table: "DocumentTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmailDomains_Domain",
                table: "EmailDomains",
                column: "Domain",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MileageHistory_VehicleId",
                table: "MileageHistory",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonDocuments_DocumentTypeId_DocumentNumber",
                table: "PersonDocuments",
                columns: new[] { "DocumentTypeId", "DocumentNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PersonDocuments_PersonId",
                table: "PersonDocuments",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonEmails_EmailDomainId",
                table: "PersonEmails",
                column: "EmailDomainId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonEmails_EmailUser_EmailDomainId",
                table: "PersonEmails",
                columns: new[] { "EmailUser", "EmailDomainId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PersonEmails_PersonId",
                table: "PersonEmails",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonPhones_PersonId",
                table: "PersonPhones",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonPhones_PhoneCodeId_PhoneNumber",
                table: "PersonPhones",
                columns: new[] { "PhoneCodeId", "PhoneNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhoneCodes_Code",
                table: "PhoneCodes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VehicleOwnershipHistory_CustomerId",
                table: "VehicleOwnershipHistory",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleOwnershipHistory_VehicleId",
                table: "VehicleOwnershipHistory",
                column: "VehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_appointments_AppointmentStatuses_appointment_status_id",
                table: "appointments",
                column: "appointment_status_id",
                principalTable: "AppointmentStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_appointments_Users_assigned_user_id",
                table: "appointments",
                column: "assigned_user_id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_customers_Persons_person_id",
                table: "customers",
                column: "person_id",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_purchase_orders_Users_user_id",
                table: "purchase_orders",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_quotations_Users_created_by_user_id",
                table: "quotations",
                column: "created_by_user_id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_service_orders_OrderStatuses_order_status_id",
                table: "service_orders",
                column: "order_status_id",
                principalTable: "OrderStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_service_orders_Users_mechanic_id",
                table: "service_orders",
                column: "mechanic_id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                table: "UserRoles",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Persons_PersonId",
                table: "Users",
                column: "PersonId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleModels_VehicleBrands_BrandId",
                table: "VehicleModels",
                column: "BrandId",
                principalTable: "VehicleBrands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_vehicles_FuelTypes_fuel_type_id",
                table: "vehicles",
                column: "fuel_type_id",
                principalTable: "FuelTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_vehicles_TransmissionTypes_transmission_type_id",
                table: "vehicles",
                column: "transmission_type_id",
                principalTable: "TransmissionTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_vehicles_VehicleColors_color_id",
                table: "vehicles",
                column: "color_id",
                principalTable: "VehicleColors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_vehicles_VehicleModels_model_id",
                table: "vehicles",
                column: "model_id",
                principalTable: "VehicleModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_appointments_AppointmentStatuses_appointment_status_id",
                table: "appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_appointments_Users_assigned_user_id",
                table: "appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_customers_Persons_person_id",
                table: "customers");

            migrationBuilder.DropForeignKey(
                name: "FK_purchase_orders_Users_user_id",
                table: "purchase_orders");

            migrationBuilder.DropForeignKey(
                name: "FK_quotations_Users_created_by_user_id",
                table: "quotations");

            migrationBuilder.DropForeignKey(
                name: "FK_service_orders_OrderStatuses_order_status_id",
                table: "service_orders");

            migrationBuilder.DropForeignKey(
                name: "FK_service_orders_Users_mechanic_id",
                table: "service_orders");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Roles_RoleId",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Users_UserId",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Persons_PersonId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleModels_VehicleBrands_BrandId",
                table: "VehicleModels");

            migrationBuilder.DropForeignKey(
                name: "FK_vehicles_FuelTypes_fuel_type_id",
                table: "vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_vehicles_TransmissionTypes_transmission_type_id",
                table: "vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_vehicles_VehicleColors_color_id",
                table: "vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_vehicles_VehicleModels_model_id",
                table: "vehicles");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "MileageHistory");

            migrationBuilder.DropTable(
                name: "PersonDocuments");

            migrationBuilder.DropTable(
                name: "PersonEmails");

            migrationBuilder.DropTable(
                name: "PersonPhones");

            migrationBuilder.DropTable(
                name: "VehicleOwnershipHistory");

            migrationBuilder.DropTable(
                name: "AuditActionTypes");

            migrationBuilder.DropTable(
                name: "DocumentTypes");

            migrationBuilder.DropTable(
                name: "EmailDomains");

            migrationBuilder.DropTable(
                name: "PhoneCodes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Roles",
                table: "Roles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Persons",
                table: "Persons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VehicleModels",
                table: "VehicleModels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VehicleColors",
                table: "VehicleColors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VehicleBrands",
                table: "VehicleBrands");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRoles",
                table: "UserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TransmissionTypes",
                table: "TransmissionTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderStatuses",
                table: "OrderStatuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FuelTypes",
                table: "FuelTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppointmentStatuses",
                table: "AppointmentStatuses");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "users");

            migrationBuilder.RenameTable(
                name: "Roles",
                newName: "roles");

            migrationBuilder.RenameTable(
                name: "Persons",
                newName: "persons");

            migrationBuilder.RenameTable(
                name: "VehicleModels",
                newName: "vehicle_models");

            migrationBuilder.RenameTable(
                name: "VehicleColors",
                newName: "vehicle_colors");

            migrationBuilder.RenameTable(
                name: "VehicleBrands",
                newName: "vehicle_brands");

            migrationBuilder.RenameTable(
                name: "UserRoles",
                newName: "user_roles");

            migrationBuilder.RenameTable(
                name: "TransmissionTypes",
                newName: "transmission_types");

            migrationBuilder.RenameTable(
                name: "OrderStatuses",
                newName: "order_statuses");

            migrationBuilder.RenameTable(
                name: "FuelTypes",
                newName: "fuel_types");

            migrationBuilder.RenameTable(
                name: "AppointmentStatuses",
                newName: "appointment_statuses");

            migrationBuilder.RenameColumn(
                name: "PersonId",
                table: "users",
                newName: "person_id");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "users",
                newName: "password_hash");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "users",
                newName: "is_active");

            migrationBuilder.RenameIndex(
                name: "IX_Users_PersonId",
                table: "users",
                newName: "IX_users_person_id");

            migrationBuilder.RenameColumn(
                name: "RoleName",
                table: "roles",
                newName: "role_name");

            migrationBuilder.RenameIndex(
                name: "IX_Roles_RoleName",
                table: "roles",
                newName: "IX_roles_role_name");

            migrationBuilder.RenameColumn(
                name: "RegisteredAt",
                table: "persons",
                newName: "registered_at");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "persons",
                newName: "last_name");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "persons",
                newName: "first_name");

            migrationBuilder.RenameColumn(
                name: "ModelName",
                table: "vehicle_models",
                newName: "model_name");

            migrationBuilder.RenameColumn(
                name: "BrandId",
                table: "vehicle_models",
                newName: "brand_id");

            migrationBuilder.RenameIndex(
                name: "IX_VehicleModels_BrandId_ModelName",
                table: "vehicle_models",
                newName: "IX_vehicle_models_brand_id_model_name");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "vehicle_colors",
                newName: "name");

            migrationBuilder.RenameIndex(
                name: "IX_VehicleColors_Name",
                table: "vehicle_colors",
                newName: "IX_vehicle_colors_name");

            migrationBuilder.RenameColumn(
                name: "BrandName",
                table: "vehicle_brands",
                newName: "brand_name");

            migrationBuilder.RenameIndex(
                name: "IX_VehicleBrands_BrandName",
                table: "vehicle_brands",
                newName: "IX_vehicle_brands_brand_name");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "user_roles",
                newName: "role_id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "user_roles",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "IX_UserRoles_RoleId",
                table: "user_roles",
                newName: "IX_user_roles_role_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "transmission_types",
                newName: "name");

            migrationBuilder.RenameIndex(
                name: "IX_TransmissionTypes_Name",
                table: "transmission_types",
                newName: "IX_transmission_types_name");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "order_statuses",
                newName: "name");

            migrationBuilder.RenameIndex(
                name: "IX_OrderStatuses_Name",
                table: "order_statuses",
                newName: "IX_order_statuses_name");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "fuel_types",
                newName: "name");

            migrationBuilder.RenameIndex(
                name: "IX_FuelTypes_Name",
                table: "fuel_types",
                newName: "IX_fuel_types_name");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "appointment_statuses",
                newName: "name");

            migrationBuilder.RenameIndex(
                name: "IX_AppointmentStatuses_Name",
                table: "appointment_statuses",
                newName: "IX_appointment_statuses_name");

            migrationBuilder.AlterColumn<bool>(
                name: "is_active",
                table: "users",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "model_name",
                table: "vehicle_models",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(80)",
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<string>(
                name: "brand_name",
                table: "vehicle_brands",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(80)",
                oldMaxLength: 80);

            migrationBuilder.AddPrimaryKey(
                name: "PK_users",
                table: "users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_roles",
                table: "roles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_persons",
                table: "persons",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_vehicle_models",
                table: "vehicle_models",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_vehicle_colors",
                table: "vehicle_colors",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_vehicle_brands",
                table: "vehicle_brands",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_roles",
                table: "user_roles",
                columns: new[] { "user_id", "role_id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_transmission_types",
                table: "transmission_types",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_order_statuses",
                table: "order_statuses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_fuel_types",
                table: "fuel_types",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_appointment_statuses",
                table: "appointment_statuses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_appointments_appointment_statuses_appointment_status_id",
                table: "appointments",
                column: "appointment_status_id",
                principalTable: "appointment_statuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_appointments_users_assigned_user_id",
                table: "appointments",
                column: "assigned_user_id",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_customers_persons_person_id",
                table: "customers",
                column: "person_id",
                principalTable: "persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_purchase_orders_users_user_id",
                table: "purchase_orders",
                column: "user_id",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_quotations_users_created_by_user_id",
                table: "quotations",
                column: "created_by_user_id",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_service_orders_order_statuses_order_status_id",
                table: "service_orders",
                column: "order_status_id",
                principalTable: "order_statuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_service_orders_users_mechanic_id",
                table: "service_orders",
                column: "mechanic_id",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_user_roles_roles_role_id",
                table: "user_roles",
                column: "role_id",
                principalTable: "roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_user_roles_users_user_id",
                table: "user_roles",
                column: "user_id",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_users_persons_person_id",
                table: "users",
                column: "person_id",
                principalTable: "persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_vehicle_models_vehicle_brands_brand_id",
                table: "vehicle_models",
                column: "brand_id",
                principalTable: "vehicle_brands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_vehicles_fuel_types_fuel_type_id",
                table: "vehicles",
                column: "fuel_type_id",
                principalTable: "fuel_types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_vehicles_transmission_types_transmission_type_id",
                table: "vehicles",
                column: "transmission_type_id",
                principalTable: "transmission_types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_vehicles_vehicle_colors_color_id",
                table: "vehicles",
                column: "color_id",
                principalTable: "vehicle_colors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_vehicles_vehicle_models_model_id",
                table: "vehicles",
                column: "model_id",
                principalTable: "vehicle_models",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
