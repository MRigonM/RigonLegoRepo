using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity
{
    public class AppIdentityDbContextSeed
    {
        public static async Task SeedUsersAsync(UserManager<AppUser>userManager){

            if(!userManager.Users.Any())
            {
                var user = new AppUser{
                    DisplayName ="Bob",
                    Email = "bob@test.com",
                    UserName = "bob@test.com",
                    Address = new Address{
                        FirstName = "Bob",
                        LastName = "Bob",
                        Street = "Bill Clinton",
                        City = "Prishtine",
                        ZipCode = "10000"
                    }
                };
                await userManager.CreateAsync(user,"Pa$$wo0rd");
            }
        }
    }
}