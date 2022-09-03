﻿using DAL.Repositories.Abstract;
using DAL.Repositories.Abstract.Base;
using Entities;

namespace DAL.Repositories
{
    public class UserRepository : BaseRepository<UserEntity>, IUserRepository
    {
        public UserRepository(BlogDbContext db) : base(db)
        {

        }
    }
}
