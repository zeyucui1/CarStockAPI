using Microsoft.Data.Sqlite;

namespace CarStockAPI.Helpers;

// initialize database and add some sample data
public static class DatabaseInitializer
{
    public static void InitializeDatabase()
    {
        using var connection = new SqliteConnection("Data Source=CarStock.db");
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Dealers (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Username TEXT NOT NULL,
                Password TEXT NOT NULL,
                Name TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Cars (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                DealerId INTEGER NOT NULL,
                Make TEXT NOT NULL,
                Model TEXT NOT NULL,
                Year INTEGER NOT NULL,
                Stock INTEGER NOT NULL,
                FOREIGN KEY (DealerId) REFERENCES Dealers(Id)
            );

            -- Insert sample dealers
            INSERT INTO Dealers (Username, Password, Name) VALUES ('dealer1', 'password123', 'City Cars');
            INSERT INTO Dealers (Username, Password, Name) VALUES ('dealer2', 'password456', 'Mountain Motors');

            -- Insert sample cars
            INSERT INTO Cars (DealerId, Make, Model, Year, Stock) VALUES (1, 'Audi', 'A4', 2018, 10);
            INSERT INTO Cars (DealerId, Make, Model, Year, Stock) VALUES (1, 'BMW', 'X5', 2020, 5);
            INSERT INTO Cars (DealerId, Make, Model, Year, Stock) VALUES (2, 'Toyota', 'Camry', 2019, 8);
            INSERT INTO Cars (DealerId, Make, Model, Year, Stock) VALUES (2, 'Honda', 'Civic', 2022, 15);
        ";
        command.ExecuteNonQuery();
    }
}