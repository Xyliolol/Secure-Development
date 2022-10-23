using CardStorageService.Data;
using CardStorageService.Models;
using Microsoft.Extensions.Options;

namespace CardStorageService.Services.Impl
{
    public class ClientRepository : IClientRepositoryService
    {

        private readonly CardStorageServiceDbContext _context;
        private readonly ILogger<ClientRepository> _logger;
        private readonly IOptions<DatabaseOptions> _databaseOptions;

        public ClientRepository(
            ILogger<ClientRepository> logger,
            IOptions<DatabaseOptions> databaseOptions,
            CardStorageServiceDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public int Create(Client data)
        {
            _context.Clients.Add(data);
            _context.SaveChanges();
            return data.ClientId;
        }

        public void Delete(int id)
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

        public IList<Client> GetAll()
        {
            var clients = _context.Clients.ToList();
            return clients;
        }
        public void Update(Client data)
        {
            var clients = _context.Clients.FirstOrDefault(cl => cl.ClientId == data.ClientId);
            clients.FirstName = data.FirstName;
            clients.Surname = data.Surname;
            clients.Patronymic = data.Patronymic;

            _context.SaveChanges();
        }
        public Client GetById(int id)
        {
            throw new NotImplementedException();
        }

    }
}
