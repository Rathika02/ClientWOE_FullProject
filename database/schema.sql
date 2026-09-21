CREATE DATABASE ClientWOE_DB;
GO

USE ClientWOE_DB;
GO

CREATE TABLE Client (
    ClientId INT IDENTITY(1,1) PRIMARY KEY,
    ClientCode NVARCHAR(30) NOT NULL UNIQUE,
    ClientName NVARCHAR(150) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1
);
GO

CREATE TABLE Patient (
    PatientId INT IDENTITY(1,1) PRIMARY KEY,
    PatientCode NVARCHAR(30) NOT NULL UNIQUE,
    ClientId INT NOT NULL,
    PatientName NVARCHAR(100) NOT NULL,
    Age INT NOT NULL,
    Gender INT NOT NULL,
    MobileNumber NVARCHAR(15) NOT NULL,
    CONSTRAINT FK_Patient_Client FOREIGN KEY (ClientId) REFERENCES Client(ClientId)
);
GO

CREATE TABLE TestMaster (
    TestId INT IDENTITY(1,1) PRIMARY KEY,
    TestCode NVARCHAR(30) NOT NULL UNIQUE,
    TestName NVARCHAR(150) NOT NULL,
    Rate DECIMAL(18,2) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1
);
GO

CREATE TABLE WorkOrder (
    WorkOrderId INT IDENTITY(1,1) PRIMARY KEY,
    WoeNumber NVARCHAR(30) NOT NULL UNIQUE,
    ClientId INT NOT NULL,
    PatientId INT NOT NULL,
    OrderDate DATETIME2 NOT NULL,
    TotalAmount DECIMAL(18,2) NOT NULL,
    Status NVARCHAR(30) NOT NULL,
    CONSTRAINT FK_WorkOrder_Client FOREIGN KEY (ClientId) REFERENCES Client(ClientId),
    CONSTRAINT FK_WorkOrder_Patient FOREIGN KEY (PatientId) REFERENCES Patient(PatientId)
);
GO

CREATE TABLE WorkOrderTestDetail (
    WorkOrderTestDetailId INT IDENTITY(1,1) PRIMARY KEY,
    WorkOrderId INT NOT NULL,
    TestId INT NOT NULL,
    Quantity INT NOT NULL,
    Rate DECIMAL(18,2) NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_WorkOrderTestDetail_WorkOrder
        FOREIGN KEY (WorkOrderId) REFERENCES WorkOrder(WorkOrderId) ON DELETE CASCADE,
    CONSTRAINT FK_WorkOrderTestDetail_TestMaster
        FOREIGN KEY (TestId) REFERENCES TestMaster(TestId)
);
GO

INSERT INTO Client (ClientCode, ClientName, IsActive) VALUES
('CLI001', 'Apollo Diagnostics', 1),
('CLI002', 'City Care Hospital', 1),
('CLI003', 'Walk-In Client', 1);
GO

INSERT INTO TestMaster (TestCode, TestName, Rate, IsActive) VALUES
('CBC', 'Complete Blood Count', 350, 1),
('FBS', 'Fasting Blood Sugar', 150, 1),
('LFT', 'Liver Function Test', 700, 1),
('KFT', 'Kidney Function Test', 650, 1),
('LIPID', 'Lipid Profile', 550, 1),
('TSH', 'TSH', 300, 1);
GO
