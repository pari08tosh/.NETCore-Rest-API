using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MoviesAPI;
using MoviesAPI.Helpers;

namespace MoviesAPI.Tests
{
    public class BaseTests
    {
        private readonly string _testKey = "testkeykjdsnfkjindskjfsdijufn";
        protected ApplicationDBContext BuildContext(string databaseName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
            .UseInMemoryDatabase(databaseName).Options;

            var dbContext = new ApplicationDBContext(options);

            return dbContext;
        }

        protected IMapper BuildMapper()
        {
            var config = new MapperConfiguration(options =>
            {
                options.AddProfile(new AutoMapperProfiles());
            });

            return config.CreateMapper();
        }

        protected DefaultHttpContext BuildHttpContext(string method)
        {
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Scheme = "http";
            httpContext.Request.Host = new HostString("localhost");
            httpContext.Request.Path = "/test";
            httpContext.Request.Method = method;

            return httpContext;
        }

        protected WebApplicationFactory<Startup> BuildWebApplicationFactory(string databaseName)
        {
            var factory = new WebApplicationFactory<Startup>();

            factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var descriptorDbContext = services.SingleOrDefault(d =>
                        d.ServiceType == typeof(DbContextOptions<ApplicationDBContext>)
                    );

                    if (descriptorDbContext != null)
                    {
                        services.Remove(descriptorDbContext);
                    }

                    services.AddDbContext<ApplicationDBContext>(options =>
                    {
                        options.UseInMemoryDatabase(databaseName);
                    });
                });

                builder.ConfigureAppConfiguration((ContextCacheKey, configBuilder) =>
                {
                    configBuilder.AddInMemoryCollection(
                        new Dictionary<string, string>
                        {
                            ["jwt:secret"] = _testKey
                        });
                });
            });
            return factory;
        }

        protected string GenerateDummyToken(List<string> roles)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, "Test User"),
                new Claim(ClaimTypes.Email, "test@test.com")
            };


            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_testKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expirey = DateTime.UtcNow.AddHours(1);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: expirey,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}