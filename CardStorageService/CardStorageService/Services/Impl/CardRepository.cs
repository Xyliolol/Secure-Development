using CardStorageService.Data;
using CardStorageService.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace CardStorageService.Services.Impl
{
    public class CardRepository : ICardRepositoryService
    {

        private readonly CardStorageServiceDbContext _context;
        private readonly ILogger<ClientRepository> _logger;
        private readonly IOptions<DatabaseOptions> _databaseOptions;

        public CardRepository(
            ILogger<ClientRepository> logger,
            IOptions<DatabaseOptions> databaseOptions,
            CardStorageServiceDbContext context)
        {
            _logger = logger;
            _databaseOptions = databaseOptions;
            _context = context;
        }

        public string Create(Card data)
        {
            var client = _context.Clients.FirstOrDefault(client => client.ClientId == data.ClientId);
            if (client == null)
                throw new Exception("Client not found.");

            _context.Cards.Add(data);

            _context.SaveChanges();

            return data.CardId.ToString();
        }

        public IList<Card> GetByClientId(string id)
        {
            List<Card> cards = new List<Card>();
            using (SqlConnection sqlConnection = new SqlConnection(_databaseOptions.Value.ConnectionString))
            {
                sqlConnection.Open();
                using (var sqlCommand = new SqlCommand(String.Format("select * from cards where ClientId = {0}", id), sqlConnection))
                {
                    var reader = sqlCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        cards.Add(new Card
                        {
                            CardId = new Guid(reader["CardId"].ToString()),
                            CardNo = reader["CardNo"]?.ToString(),
                            Name = reader["Name"]?.ToString(),
                            CVV2 = reader["CVV2"]?.ToString(),
                            ExpDate = Convert.ToDateTime(reader["ExpDate"])
                        });
                    }
                }

            }
            return cards;

        }

        public void Delete(string id)
        {
            try
            {
                _context.Remove(id);
                _context.SaveChanges();
            }
            catch
            {
                return;
            }
        }

        public IList<Card> GetAll()
        {
            var cards = _context.Cards.ToList();
            return cards;
        }

        public void Update(Card data)
        {
            var cards = _context.Cards.FirstOrDefault(en => en.CardNo == data.CardNo);
            cards.Name = data.Name;
            cards.CVV2 = data.CVV2;
            cards.ExpDate = data.ExpDate;

            _context.SaveChanges();
        }
        public Card GetById(string id)
        {
            throw new NotImplementedException();
        }


    }
}
