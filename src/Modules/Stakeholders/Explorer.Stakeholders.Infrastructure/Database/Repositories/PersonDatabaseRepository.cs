﻿using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database.Repositories
{
    public class PersonDataBaseRepository : IPersonRepository
    {

        private readonly StakeholdersContext _dbContext;

        public PersonDataBaseRepository(StakeholdersContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Person? GetByUserId(long id)
        {
            var person = _dbContext.People.Include(x => x.User).FirstOrDefault(person => person.UserId == id);
            return person;
        }

        public Result<PagedResult<Person>> GetAll(int page, int pageSize)
        {
            var task = _dbContext.People.Include(x => x.User).GetPagedById(page, pageSize);
            task.Wait();
            return task.Result;
        }
    }
}
