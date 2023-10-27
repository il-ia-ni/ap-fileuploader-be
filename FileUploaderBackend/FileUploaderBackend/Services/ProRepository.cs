using Microsoft.EntityFrameworkCore;
using ProLibrary.Models;

namespace FileUploaderBackend.Services
{
    public class ProRepository : IProRepository
    {
        private readonly PROContext _dbContext;
        public ProRepository(PROContext dbContext) 
        {
            _dbContext = dbContext;
        }
    }
}
