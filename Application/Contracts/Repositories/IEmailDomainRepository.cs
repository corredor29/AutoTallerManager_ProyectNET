using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Persons;

namespace Application.Contracts.Repositories
{
using Domain.Entities.Persons;
    public interface IEmailDomainRepository
    {
        Task<EmailDomain?>            GetByIdAsync(int id);
        Task<IEnumerable<EmailDomain>> GetAllAsync();
        Task<bool> ExistsByDomainAsync(string domain);
        Task AddAsync(EmailDomain emailDomain);
        void  Update(EmailDomain emailDomain);
        void  Remove(EmailDomain emailDomain);
    }
}