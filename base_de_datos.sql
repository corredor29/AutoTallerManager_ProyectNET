-- ------------------------------------------------------------
-- 1. BASE CATALOGS
-- ------------------------------------------------------------

CREATE TABLE DocumentTypes (
    DocumentTypeId SERIAL PRIMARY KEY,
    Code           VARCHAR(10) NOT NULL UNIQUE,
    Name           VARCHAR(80) NOT NULL UNIQUE
);

CREATE TABLE EmailDomains (
    EmailDomainId SERIAL PRIMARY KEY,
    Domain        VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE PhoneCodes (
    PhoneCodeId SERIAL PRIMARY KEY,
    Code        VARCHAR(10) NOT NULL UNIQUE,
    Country     VARCHAR(80) NOT NULL
);

-- ------------------------------------------------------------
-- 2. PERSONS (base for Customers and Users)
-- ------------------------------------------------------------

CREATE TABLE Persons (
    PersonId      SERIAL PRIMARY KEY,
    FirstName     VARCHAR(100) NOT NULL,
    LastName      VARCHAR(100) NOT NULL,
    RegisteredAt  TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE PersonDocuments (
    PersonDocumentId SERIAL PRIMARY KEY,
    PersonId         INT         NOT NULL REFERENCES Persons(PersonId),
    DocumentTypeId   INT         NOT NULL REFERENCES DocumentTypes(DocumentTypeId),
    DocumentNumber   VARCHAR(50) NOT NULL,
    IsPrimary        BOOLEAN     NOT NULL DEFAULT FALSE,
    UNIQUE (DocumentTypeId, DocumentNumber)
);

CREATE TABLE PersonEmails (
    PersonEmailId SERIAL PRIMARY KEY,
    PersonId      INT          NOT NULL REFERENCES Persons(PersonId),
    EmailDomainId INT          NOT NULL REFERENCES EmailDomains(EmailDomainId),
    EmailUser     VARCHAR(100) NOT NULL,
    IsPrimary     BOOLEAN      NOT NULL DEFAULT FALSE,
    UNIQUE (EmailUser, EmailDomainId)
);

CREATE TABLE PersonPhones (
    PersonPhoneId SERIAL PRIMARY KEY,
    PersonId      INT         NOT NULL REFERENCES Persons(PersonId),
    PhoneCodeId   INT         NOT NULL REFERENCES PhoneCodes(PhoneCodeId),
    PhoneNumber   VARCHAR(30) NOT NULL,
    IsPrimary     BOOLEAN     NOT NULL DEFAULT FALSE,
    UNIQUE (PhoneCodeId, PhoneNumber)
);

-- ------------------------------------------------------------
-- 3. CUSTOMERS
-- ------------------------------------------------------------

CREATE TABLE Customers (
    CustomerId SERIAL PRIMARY KEY,
    PersonId   INT     NOT NULL UNIQUE REFERENCES Persons(PersonId),
    IsActive   BOOLEAN NOT NULL DEFAULT TRUE
);

-- ------------------------------------------------------------
-- 4. USERS AND ROLES
-- ------------------------------------------------------------

CREATE TABLE Roles (
    RoleId   SERIAL PRIMARY KEY,
    RoleName VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE Users (
    UserId       SERIAL PRIMARY KEY,
    PersonId     INT          NOT NULL UNIQUE REFERENCES Persons(PersonId),
    PasswordHash VARCHAR(255) NOT NULL,
    IsActive     BOOLEAN      NOT NULL DEFAULT TRUE
);

CREATE TABLE UserRoles (
    UserId INT NOT NULL REFERENCES Users(UserId),
    RoleId INT NOT NULL REFERENCES Roles(RoleId),
    PRIMARY KEY (UserId, RoleId)
);

-- ------------------------------------------------------------
-- 5. VEHICLES
-- ------------------------------------------------------------

CREATE TABLE VehicleBrands (
    BrandId   SERIAL PRIMARY KEY,
    BrandName VARCHAR(80) NOT NULL UNIQUE
);

CREATE TABLE VehicleModels (
    ModelId   SERIAL PRIMARY KEY,
    BrandId   INT         NOT NULL REFERENCES VehicleBrands(BrandId),
    ModelName VARCHAR(80) NOT NULL,
    UNIQUE (BrandId, ModelName)
);

CREATE TABLE VehicleColors (
    ColorId SERIAL PRIMARY KEY,
    Name    VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE FuelTypes (
    FuelTypeId SERIAL PRIMARY KEY,
    Name       VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE TransmissionTypes (
    TransmissionTypeId SERIAL PRIMARY KEY,
    Name               VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE Vehicles (
    VehicleId          SERIAL PRIMARY KEY,
    ModelId            INT         NOT NULL REFERENCES VehicleModels(ModelId),
    ColorId            INT         NULL     REFERENCES VehicleColors(ColorId),
    FuelTypeId         INT         NULL     REFERENCES FuelTypes(FuelTypeId),
    TransmissionTypeId INT         NULL     REFERENCES TransmissionTypes(TransmissionTypeId),
    VIN                VARCHAR(17) NOT NULL UNIQUE,
    Year               SMALLINT    NOT NULL,
    Mileage            INT         NOT NULL DEFAULT 0,
    LicensePlate       VARCHAR(20) NULL UNIQUE
);

-- Vehicle ownership history: EndDate NULL = current owner
CREATE TABLE VehicleOwnershipHistory (
    OwnershipHistoryId SERIAL PRIMARY KEY,
    VehicleId          INT  NOT NULL REFERENCES Vehicles(VehicleId),
    CustomerId         INT  NOT NULL REFERENCES Customers(CustomerId),
    StartDate          DATE NOT NULL,
    EndDate            DATE NULL
);

-- Mileage history per service
CREATE TABLE MileageHistory (
    MileageHistoryId SERIAL PRIMARY KEY,
    VehicleId        INT       NOT NULL REFERENCES Vehicles(VehicleId),
    Mileage          INT       NOT NULL,
    RecordedAt       TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Notes            TEXT      NULL
);

-- ------------------------------------------------------------
-- 6. APPOINTMENTS
-- ------------------------------------------------------------

CREATE TABLE AppointmentStatuses (
    AppointmentStatusId SERIAL PRIMARY KEY,
    Name                VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE Appointments (
    AppointmentId      SERIAL PRIMARY KEY,
    CustomerId         INT       NOT NULL REFERENCES Customers(CustomerId),
    VehicleId          INT       NOT NULL REFERENCES Vehicles(VehicleId),
    ServiceTypeId      INT       NOT NULL,  -- FK added after ServiceTypes
    AppointmentStatusId INT      NOT NULL REFERENCES AppointmentStatuses(AppointmentStatusId),
    AssignedUserId     INT       NULL     REFERENCES Users(UserId),
    AppointmentDate    TIMESTAMP NOT NULL,
    Notes              TEXT      NULL
);

-- ------------------------------------------------------------
-- 7. SERVICE ORDERS
-- ------------------------------------------------------------

CREATE TABLE ServiceTypes (
    ServiceTypeId     SERIAL PRIMARY KEY,
    Name              VARCHAR(80) NOT NULL UNIQUE,
    EstimatedDuration INT         NULL  -- in hours
);

-- FK from Appointments to ServiceTypes (added after to avoid circular dependency)
ALTER TABLE Appointments ADD CONSTRAINT fk_appointments_servicetype
    FOREIGN KEY (ServiceTypeId) REFERENCES ServiceTypes(ServiceTypeId);

CREATE TABLE OrderStatuses (
    OrderStatusId SERIAL PRIMARY KEY,
    Name          VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE ServiceOrders (
    ServiceOrderId       SERIAL PRIMARY KEY,
    VehicleId            INT       NOT NULL REFERENCES Vehicles(VehicleId),
    ServiceTypeId        INT       NOT NULL REFERENCES ServiceTypes(ServiceTypeId),
    MechanicId           INT       NOT NULL REFERENCES Users(UserId),
    OrderStatusId        INT       NOT NULL REFERENCES OrderStatuses(OrderStatusId),
    AppointmentId        INT       NULL     REFERENCES Appointments(AppointmentId),
    CreatedAt            TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    EstimatedDeliveryAt  TIMESTAMP NULL,
    ClosedAt             TIMESTAMP NULL,
    WorkPerformed        TEXT      NULL,
    Notes                TEXT      NULL
);

-- ------------------------------------------------------------
-- 8. QUOTATIONS
--    Flow: ServiceOrder → Quotation → Customer accepts/rejects → Invoice
-- ------------------------------------------------------------

CREATE TABLE QuotationStatuses (
    QuotationStatusId SERIAL PRIMARY KEY,
    Name              VARCHAR(50) NOT NULL UNIQUE
    -- Values: 'Pending', 'Accepted', 'Rejected'
);

CREATE TABLE Quotations (
    QuotationId       SERIAL PRIMARY KEY,
    ServiceOrderId    INT           NOT NULL REFERENCES ServiceOrders(ServiceOrderId),
    CreatedByUserId   INT           NOT NULL REFERENCES Users(UserId),
    QuotationStatusId INT           NOT NULL REFERENCES QuotationStatuses(QuotationStatusId),
    CreatedAt         TIMESTAMP     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    RespondedAt       TIMESTAMP     NULL,  -- when customer accepted or rejected
    LaborCost         DECIMAL(10,2) NOT NULL DEFAULT 0,
    Subtotal          DECIMAL(10,2) NOT NULL DEFAULT 0,
    Total             DECIMAL(10,2) NOT NULL DEFAULT 0,
    RejectionReason   TEXT          NULL,  -- if customer rejected, why
    Notes             TEXT          NULL
);

-- Parts quoted (may differ from parts actually used)
CREATE TABLE QuotationDetails (
    QuotationDetailId SERIAL PRIMARY KEY,
    QuotationId       INT           NOT NULL REFERENCES Quotations(QuotationId),
    PartId            INT           NOT NULL,  -- FK added after Parts
    Quantity          INT           NOT NULL,
    UnitPrice         DECIMAL(10,2) NOT NULL,
    UNIQUE (QuotationId, PartId)
);

-- ------------------------------------------------------------
-- 9. PARTS AND INVENTORY
-- ------------------------------------------------------------

CREATE TABLE PartCategories (
    PartCategoryId SERIAL PRIMARY KEY,
    Name           VARCHAR(80) NOT NULL UNIQUE
);

CREATE TABLE MeasurementUnits (
    MeasurementUnitId SERIAL PRIMARY KEY,
    Name              VARCHAR(50) NOT NULL UNIQUE,
    Abbreviation      VARCHAR(10) NOT NULL UNIQUE
);

CREATE TABLE Parts (
    PartId         SERIAL PRIMARY KEY,
    PartCategoryId INT           NOT NULL REFERENCES PartCategories(PartCategoryId),
    UnitId         INT           NULL     REFERENCES MeasurementUnits(MeasurementUnitId),
    Code           VARCHAR(50)   NOT NULL UNIQUE,
    Description    VARCHAR(255)  NOT NULL,
    Stock          INT           NOT NULL DEFAULT 0,
    MinStock       INT           NOT NULL DEFAULT 0,
    UnitPrice      DECIMAL(10,2) NOT NULL,
    IsActive       BOOLEAN       NOT NULL DEFAULT TRUE
);

-- FK from QuotationDetails to Parts
ALTER TABLE QuotationDetails ADD CONSTRAINT fk_quotationdetails_part
    FOREIGN KEY (PartId) REFERENCES Parts(PartId);

-- Parts actually used in the order (post-approval)
CREATE TABLE ServiceOrderParts (
    ServiceOrderPartId SERIAL PRIMARY KEY,
    ServiceOrderId     INT           NOT NULL REFERENCES ServiceOrders(ServiceOrderId),
    PartId             INT           NOT NULL REFERENCES Parts(PartId),
    Quantity           INT           NOT NULL,
    AppliedUnitPrice   DECIMAL(10,2) NOT NULL,
    UNIQUE (ServiceOrderId, PartId)
);

-- ------------------------------------------------------------
-- 10. SUPPLIERS AND PURCHASE ORDERS
-- ------------------------------------------------------------

CREATE TABLE Suppliers (
    SupplierId    SERIAL PRIMARY KEY,
    CompanyName   VARCHAR(150) NOT NULL,
    TaxId         VARCHAR(50)  NULL UNIQUE,
    ContactName   VARCHAR(100) NULL,
    Phone         VARCHAR(30)  NULL,
    Email         VARCHAR(150) NULL,
    Address       TEXT         NULL,
    IsActive      BOOLEAN      NOT NULL DEFAULT TRUE
);

CREATE TABLE PartSuppliers (
    PartSupplierId SERIAL PRIMARY KEY,
    PartId         INT           NOT NULL REFERENCES Parts(PartId),
    SupplierId     INT           NOT NULL REFERENCES Suppliers(SupplierId),
    PurchasePrice  DECIMAL(10,2) NOT NULL,
    IsPrimary      BOOLEAN       NOT NULL DEFAULT FALSE,
    UNIQUE (PartId, SupplierId)
);

CREATE TABLE PurchaseOrderStatuses (
    PurchaseOrderStatusId SERIAL PRIMARY KEY,
    Name                  VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE PurchaseOrders (
    PurchaseOrderId       SERIAL PRIMARY KEY,
    SupplierId            INT           NOT NULL REFERENCES Suppliers(SupplierId),
    UserId                INT           NOT NULL REFERENCES Users(UserId),
    PurchaseOrderStatusId INT           NOT NULL REFERENCES PurchaseOrderStatuses(PurchaseOrderStatusId),
    OrderedAt             TIMESTAMP     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    ReceivedAt            TIMESTAMP     NULL,
    Total                 DECIMAL(10,2) NOT NULL DEFAULT 0,
    Notes                 TEXT          NULL
);

CREATE TABLE PurchaseOrderDetails (
    PurchaseOrderDetailId SERIAL PRIMARY KEY,
    PurchaseOrderId       INT           NOT NULL REFERENCES PurchaseOrders(PurchaseOrderId),
    PartId                INT           NOT NULL REFERENCES Parts(PartId),
    Quantity              INT           NOT NULL,
    UnitPrice             DECIMAL(10,2) NOT NULL,
    UNIQUE (PurchaseOrderId, PartId)
);

-- ------------------------------------------------------------
-- 11. INVOICES AND PAYMENTS
-- ------------------------------------------------------------

CREATE TABLE Invoices (
    InvoiceId              SERIAL PRIMARY KEY,
    ServiceOrderId         INT           NOT NULL UNIQUE REFERENCES ServiceOrders(ServiceOrderId),
    QuotationId            INT           NULL     REFERENCES Quotations(QuotationId),
    IssuedAt               TIMESTAMP     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    LaborCost              DECIMAL(10,2) NOT NULL DEFAULT 0,
    Subtotal               DECIMAL(10,2) NOT NULL DEFAULT 0,
    Tax                    DECIMAL(10,2) NOT NULL DEFAULT 0,
    Total                  DECIMAL(10,2) NOT NULL DEFAULT 0,
    -- If customer rejected quotation, only diagnosis/inspection is charged
    DiagnosisOnlyCharged   BOOLEAN       NOT NULL DEFAULT FALSE
);

CREATE TABLE InvoiceDetails (
    InvoiceDetailId SERIAL PRIMARY KEY,
    InvoiceId       INT           NOT NULL REFERENCES Invoices(InvoiceId),
    Description     VARCHAR(150)  NOT NULL,
    Quantity        INT           NOT NULL DEFAULT 1,
    UnitPrice       DECIMAL(10,2) NOT NULL
);

CREATE TABLE PaymentMethods (
    PaymentMethodId SERIAL PRIMARY KEY,
    Name            VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE Payments (
    PaymentId       SERIAL PRIMARY KEY,
    InvoiceId       INT           NOT NULL REFERENCES Invoices(InvoiceId),
    PaymentMethodId INT           NOT NULL REFERENCES PaymentMethods(PaymentMethodId),
    Amount          DECIMAL(10,2) NOT NULL,
    PaidAt          TIMESTAMP     NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Reference       VARCHAR(100)  NULL  -- transaction number, check number, etc.
);

-- ------------------------------------------------------------
-- 12. AUDIT
-- ------------------------------------------------------------

CREATE TABLE AuditActionTypes (
    AuditActionTypeId SERIAL PRIMARY KEY,
    Name              VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE AuditLogs (
    AuditLogId        SERIAL PRIMARY KEY,
    UserId            INT          NOT NULL REFERENCES Users(UserId),
    AuditActionTypeId INT          NOT NULL REFERENCES AuditActionTypes(AuditActionTypeId),
    AffectedEntity    VARCHAR(100) NOT NULL,
    AffectedRecordId  INT          NOT NULL,
    OccurredAt        TIMESTAMP    NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Description       TEXT         NULL
);

