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

            INSERT INTO Dealers (Name) VALUES ('City Cars');
            INSERT INTO Dealers (Name) VALUES ('Mountain Motors');

            INSERT INTO Cars (DealerId, Make, Model, Year, Stock) VALUES (1, 'Audi', 'A4', 2018, 10);
            INSERT INTO Cars (DealerId, Make, Model, Year, Stock) VALUES (1, 'BMW', 'X5', 2020, 5);
        ";
        command.ExecuteNonQuery();
    }
}