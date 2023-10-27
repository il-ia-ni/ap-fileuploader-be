using Microsoft.EntityFrameworkCore;
using PRORepository.Models;

namespace FileUploaderBackend.Services
{
    public class PRORepository
    {
        private readonly PROContext _dbContext;
        public PRORepository(PROContext dbContext) 
        {
            _dbContext = dbContext;
        }
    }
}
