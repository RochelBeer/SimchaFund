using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hmwk50.data
{
    public class Simcha
    {
        public int Id { get; set; }
        public string SimchaName { get; set; }
        public DateTime Date { get; set; }
        public int ContributorCount { get; set; }
        public decimal TotalContribution { get; set; }
    }
    public class Contributor
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Cell { get; set; }
        public bool AlwaysInclude { get; set; }
        public DateTime CreatedDate { get; set; }
        public decimal Balance { get; set; }
        public int Index { get; set; }
        public decimal ContributionAmount { get; set; }
    }
    public class Deposit
    {
        public int Id { get; set; }
        public int ContributorId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string SimchaName { get; set; }

    }
    public class Contribution
    {
        public int ContributorId { get; set; }
        public int SimchaId { get; set; }
        public decimal Amount { get; set; }
        public bool Include { get; set; }
    }
    public class SimchosManager
    {
        public string _connectionString;
        public SimchosManager(string connectionString)
        {
            _connectionString = connectionString;
        }
        public List<Simcha> GetSimchos()
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = $@"SELECT * FROM SimchosTable ORDER BY Date DESC";
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            List<Simcha> _simchos = new();
            while (reader.Read())
            {
                _simchos.Add(new Simcha
                {
                    Id = (int)reader["Id"],
                    SimchaName = (string)reader["SimchaName"],
                    Date = (DateTime)reader["Date"]
                });
            }
            return _simchos;
        }
        public void Add(Simcha simcha)
        {

            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = $@"INSERT INTO SimchosTable(SimchaName, Date)
VALUES(@simchaName,@date)";
            command.Parameters.AddWithValue("@simchaName", simcha.SimchaName);
            command.Parameters.AddWithValue("@date", simcha.Date);
            connection.Open();
            command.ExecuteNonQuery();

        }

        public List<Contributor> GetContributors()
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = $@"SELECT * FROM ContributorsTable";
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            List<Contributor> _contributors = new();
            while (reader.Read())
            {
                _contributors.Add(new Contributor
                {
                    Id = (int)reader["Id"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    Cell = (string)reader["Cell"],
                    AlwaysInclude = (bool)reader["AlwaysInclude"],
                    CreatedDate = (DateTime)reader["CreatedDate"]
                });
            }
            return _contributors;
        }
        public int Add(Contributor contributor)
        {

            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = $@"INSERT INTO ContributorsTable(FirstName, LastName, Cell, AlwaysInclude, CreatedDate)
VALUES(@firstName,@lastName,@cell,@alwaysInclude, @createdDate); SELECT SCOPE_IDENTITY()";
            command.Parameters.AddWithValue("@firstName", contributor.FirstName);
            command.Parameters.AddWithValue("@lastName", contributor.LastName);
            command.Parameters.AddWithValue("@cell", contributor.Cell);
            command.Parameters.AddWithValue("@alwaysInclude", contributor.AlwaysInclude);
            command.Parameters.AddWithValue("@createdDate", contributor.CreatedDate);
            connection.Open();
            return (int)(decimal)command.ExecuteScalar();

        }
        public void Add(Deposit deposit)
        {

            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = $@"INSERT INTO Deposits(ContributorId, Amount, Date)
VALUES(@contributorId,@amount,@date)";
            command.Parameters.AddWithValue("@contributorId", deposit.ContributorId);
            command.Parameters.AddWithValue("@amount", deposit.Amount);
            command.Parameters.AddWithValue("@date", DateTime.Now);
            connection.Open();
            command.ExecuteNonQuery();
        }
        private void Add(Contribution contribution)
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = $@"INSERT INTO Contributions(ContributorId, SimchaId, Amount)
VALUES(@contributorId,@simchaId,@amount)";
            command.Parameters.AddWithValue("@contributorId", contribution.ContributorId);
            command.Parameters.AddWithValue("@simchaId", contribution.SimchaId);
            command.Parameters.AddWithValue("@amount", contribution.Amount);            
            connection.Open();
            command.ExecuteNonQuery();
        }
        public void AddManyContributions(List<Contribution> contributions)
        {
            foreach(Contribution contribution in contributions)
            {
                Add(contribution);
            }
        }
        public string GetContributorName(int id)
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = @$" SELECT FirstName, LastName FROM ContributorsTable
WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            string fullName = "";
            SqlDataReader reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }
            fullName = (string)reader["FirstName"];
            fullName += " ";
            fullName += (string)reader["LastName"];
            return fullName;
        }
        public int ContributorCount(int id)
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = @$"SELECT COUNT(*)FROM Contributions
WHERE SimchaId = @id";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            return (int)command.ExecuteScalar();
        }
        public int ContributorTotal()
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = @$"SELECT COUNT(*)FROM ContributorsTable";
            connection.Open();
            return (int)command.ExecuteScalar();
        }
        public decimal GetTotalContribution(int id)
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = @$"SELECT SUM(Amount) FROM Contributions WHERE SimchaId = @id
GROUP BY SimchaId";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();

            if (command.ExecuteScalar() == DBNull.Value || command.ExecuteScalar() == null)
            {
                return default;
            }

            decimal totalContributed = (int)(decimal)command.ExecuteScalar();
            return totalContributed;
        }

        private decimal GetSumDeposits(int id)
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = $@"SELECT SUM(Amount) FROM Deposits
WHERE ContributorId = @id";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            if (command.ExecuteScalar() == DBNull.Value)
            {
                return default;
            }                        
            return (int)(decimal)command.ExecuteScalar();
        }
        private decimal GetSumContributions(int id)
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = $@"SELECT SUM(Amount) FROM Contributions
WHERE ContributorId = @id";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            if (command.ExecuteScalar() == DBNull.Value)
            {
                return default;
            }
            return (int)(decimal)command.ExecuteScalar();
        }
        public decimal GetBalance(int id)
        {
            return GetSumDeposits(id) - GetSumContributions(id);
        }
        public List<Deposit> GetHistory(int id)
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = @$"SELECT s.SimchaName, s.Date, c.Amount FROM SimchosTable s
LEFT JOIN Contributions c
ON s.Id = c.SimchaId
WHERE c.ContributorId = @id";
            command.Parameters.AddWithValue("@id", id);
            List<Deposit> _history = new();
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                _history.Add(new Deposit
                {
                    SimchaName = (string)reader["SimchaName"],
                    Date = (DateTime)reader["Date"],
                    Amount = -(decimal)reader["Amount"]
                });
            }
            connection.Close();

            command.CommandText = @$"SELECT Date, Amount FROM Deposits 
WHERE ContributorId = @Contributorid";
            command.Parameters.AddWithValue("@contributorid", id);
            connection.Open();
            SqlDataReader reader2 = command.ExecuteReader();


            while (reader2.Read())
            {
                _history.Add(new Deposit
                {
                    Date = (DateTime)reader2["Date"],
                    Amount = (decimal)reader2["Amount"]
                });
            }
            connection.Close();
            return _history;
        }
        private decimal GetGrandTotalDeposits()
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = $@"SELECT SUM(Amount) FROM Deposits";
            connection.Open();
            if (command.ExecuteScalar() == DBNull.Value)
            {
                return default;
            }
            return (int)(decimal)command.ExecuteScalar();
        }
        private decimal GetGrandTotalContributions()
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = $@"SELECT SUM(Amount) FROM Contributions";
            connection.Open();
            if (command.ExecuteScalar() == DBNull.Value)
            {
                return default;
            }
            return (int)(decimal)command.ExecuteScalar();
        }
        public decimal GetGrandTotal()
        {
            return GetGrandTotalDeposits() - GetGrandTotalContributions();
        }
        public string GetSimchaName(int id)
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = @$" SELECT SimchaName FROM SimchosTable
WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            if(command.ExecuteScalar() == DBNull.Value)
            {
                return null;
            }
            return (string)command.ExecuteScalar();            
        }
        public void ClearContributions(int simchaId)
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = $@"DELETE FROM Contributions
WHERE SimchaId = @simchaId";
            command.Parameters.AddWithValue("@simchaId", simchaId);
            connection.Open();
            command.ExecuteNonQuery();
        }
        public decimal GetContributionAmount(int contributorId, int simchaId)
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = $@"SELECT Amount FROM Contributions
WHERE ContributorId = @contributorId AND SimchaId = @simchaId";
            command.Parameters.AddWithValue("@contributorId", contributorId);
            command.Parameters.AddWithValue("@simchaId", simchaId);
            connection.Open();
            if (command.ExecuteScalar() == DBNull.Value || command.ExecuteScalar() == null)
            {
                return default;
            }            
            decimal amount = (int)(decimal)command.ExecuteScalar();
            return amount;
        }
        public void EditContributor(Contributor contributor)
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = $@" UPDATE ContributorsTable
SET FirstName = @firstName,
LastName = @lastName,
Cell = @cell,
AlwaysInclude = @alwaysInclude
WHERE Id = @id";
            command.Parameters.AddWithValue("@firstName", contributor.FirstName);
            command.Parameters.AddWithValue("@lastName", contributor.LastName);
            command.Parameters.AddWithValue("@cell", contributor.Cell);
            command.Parameters.AddWithValue("@alwaysInclude", contributor.AlwaysInclude);
            command.Parameters.AddWithValue("@id", contributor.Id);
            connection.Open();
            command.ExecuteNonQuery();
        }
        public List<Contributor> Search(string search)
        {
            using SqlConnection connection = new(_connectionString);
            using SqlCommand command = connection.CreateCommand();
            command.CommandText = $@"SELECT * FROM ContributorsTable
WHERE FirstName LIKE '@search' Or LastName LIKE '@search'";
            command.Parameters.AddWithValue("@search",$"%{search}%");
            List<Contributor> _searchedContributors = new();
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            while(reader.Read())
            {
                _searchedContributors.Add(new Contributor
                {
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    Cell = (string)reader["Cell"],
                    AlwaysInclude = (bool)reader["AlwaysInclude"]
                });
            }
            return _searchedContributors;
        }
    }
}
