
using Labb3Api2.Data;
using Labb3Api2.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;

namespace Labb3Api2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            // Add services to the container.
            builder.Services.AddAuthorization();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            ////////////////////////////////////////////////////
            ////           [PERSON SECTION]          ///////////
            ///////////////////////////////////////////////////


            //Return(Read) all Persons)
            app.MapGet("/persons", async (ApplicationDbContext context) =>
            {
                var persons = await context.Persons.ToListAsync();
                if (persons == null || !persons.Any())
                {
                    return Results.NotFound("Did not found any person");

                }
                return Results.Ok(persons);
            });

            //Create(Post) a Person)
            app.MapPost("/persons", async (Person person, ApplicationDbContext context) =>
            {
                //spara/ skapa till context (minnet)
                context.Persons.Add(person);
                //spara till databas
                await context.SaveChangesAsync();
                return Results.Created($"/persons/{person.PersonId}", person);
            });

            //Edit a Person
            app.MapPut("/persons/{id:int}", async (int id, Person updatedPerson, ApplicationDbContext context) =>
            {
                var person = await context.Persons.FindAsync(id);
                if (person == null)
                {
                    return Results.NotFound("Person not found");
                }
                person.Name = updatedPerson.Name;
                person.Phone = updatedPerson.Phone;
                await context.SaveChangesAsync();
                return Results.Ok(person);
            });

            //Delete a Person
            app.MapDelete("/persons/{id:int}", async (int id, ApplicationDbContext context) =>
            {
                var person = await context.Persons.FindAsync(id);
                if (person == null)
                {
                    return Results.NotFound("Person not found");
                }
                context.Persons.Remove(person);
                await context.SaveChangesAsync();
                return Results.Ok($"Person with ID {id} deleted!");
            });




            ////////////////////////////////////////////////////
            ////           [Interest]               ///////////
            ///////////////////////////////////////////////////

            //Return(Read) all Interests)
            app.MapGet("/interests", async (ApplicationDbContext context) =>
            {
                var interests = await context.Interests.ToListAsync();
                if (interests == null || !interests.Any())
                {
                    return Results.NotFound("Did not found any interest");

                }
                return Results.Ok(interests);
            });

            //Create(Post) a Interest)
            app.MapPost("/interests", async (Interest interest, ApplicationDbContext context) =>
            {
                //spara/ skapa till context (minnet)
                context.Interests.Add(interest);
                //spara till databas
                await context.SaveChangesAsync();
                return Results.Created($"/interests/{interest.InterestId}", interest);
            });

            //Edit interest
            app.MapPut("/interests/{id:int}", async (int id, Interest updatedInterest, ApplicationDbContext context) =>
            {
                var interest = await context.Interests.FindAsync(id);
                if (interest == null)
                {
                    return Results.NotFound("Person not found");
                }
                interest.Title = updatedInterest.Title;
                interest.Description = updatedInterest.Description;
                await context.SaveChangesAsync();
                return Results.Ok(interest);
            });

            //Delete interests
            app.MapDelete("/interests/{id:int}", async (int id, ApplicationDbContext context) =>
            {
                var interest = await context.Interests.FindAsync(id);
                if (interest == null)
                {
                    return Results.NotFound("Interest not found");
                }
                context.Interests.Remove(interest);
                await context.SaveChangesAsync();
                return Results.Ok($"Interest with ID {id} deleted!");
            });


            ////////////////////////////////////////////////////
            ////           [Person Add Interest]   ///////////
            ///////////////////////////////////////////////////

            // Get all PersonWithAddInterests
            app.MapGet("/personinterests/{personId:int}", async (int personId, ApplicationDbContext context) =>
            {
                // Retrieve person information
                var person = await context.Persons
                                            .Where(p => p.PersonId == personId)
                                            .Select(p => new
                                            {
                                                p.PersonId,
                                                p.Name,
                                                p.Phone
                                            })
                                            .FirstOrDefaultAsync();

                if (person == null)
                {
                    return Results.NotFound("Person not found");
                }

                // Retrieve interests for the person
                var interests = await context.PersonInterests
                                            .Include(pi => pi.Interest)
                                            .Where(pi => pi.FkPersonId == personId)
                                            .Select(pi => pi.Interest)
                                            .ToListAsync();

                if (interests == null || interests.Count == 0)
                {
                    return Results.NotFound("Interests not found");
                }

                return Results.Ok(new { Person = person, Interests = interests });
            });


            //AddInterestsToPerson:
            app.MapPost("/person/addinterests", async (AddInterestsToPersonDTO dto, ApplicationDbContext context) =>
            {
                // Retrieve the existing Person
                var existingPerson = await context.Persons.FindAsync(dto.FkPersonId);
                if (existingPerson == null)
                {
                    return Results.NotFound("Person not found");
                }

                if (dto.InterestIds != null && dto.InterestIds.Any())
                {
                    foreach (var interestId in dto.InterestIds)
                    {
                        // Check if the interest exists
                        var existingInterest = await context.Interests.FindAsync(interestId);
                        if (existingInterest == null)
                        {
                            return Results.NotFound($"Interest with ID {interestId} not found");
                        }

                        // Check if the person already has this interest
                        var existingPersonInterest = await context.PersonInterests
                            .Include(pi => pi.Person)
                            .Include(pi => pi.Interest)
                            .FirstOrDefaultAsync(pi => pi.FkPersonId == dto.FkPersonId && pi.FkInterestId == interestId);

                        if (existingPersonInterest == null)
                        {
                            // Add PersonInterest
                            var newPersonInterest = new PersonInterest
                            {
                                FkPersonId = dto.FkPersonId,
                                FkInterestId = interestId
                            };

                            context.PersonInterests.Add(newPersonInterest);
                        }
                    }
                }
                else
                {
                    return Results.BadRequest("No interests provided");
                }

                await context.SaveChangesAsync();

                // Retrieve person information
                var person = await context.Persons
                                            .Where(p => p.PersonId == dto.FkPersonId)
                                            .Select(p => new
                                            {
                                                p.PersonId,
                                                p.Name,
                                                p.Phone
                                            })
                                            .FirstOrDefaultAsync();

                if (person == null)
                {
                    return Results.NotFound("Person not found");
                }

                // Retrieve interests for the person
                var interests = await context.PersonInterests
                                            .Include(pi => pi.Interest)
                                            .Where(pi => pi.FkPersonId == dto.FkPersonId)
                                            .Select(pi => pi.Interest)
                                            .ToListAsync();

                if (interests == null || interests.Count == 0)
                {
                    return Results.NotFound("Interests not found after creation");
                }

                return Results.Created($"/personinterests/{dto.FkPersonId}", new { Person = person, Interests = interests });
            });

       
            // Edit a PersonAddInterest
            app.MapPut("/person/addinterests/{id:int}", async (int id, PersonInterest updatedPersonInterest, ApplicationDbContext context) =>
            {
                var existingPersonInterest = await context.PersonInterests.FindAsync(id);
                if (existingPersonInterest == null)
                {
                    return Results.NotFound("PersonInterest not found");
                }

                // Update the interest if provided
                if (updatedPersonInterest.FkInterestId != 0)
                {
                    var existingInterest = await context.Interests.FindAsync(updatedPersonInterest.FkInterestId);
                    if (existingInterest == null)
                    {
                        return Results.NotFound($"Interest with ID {updatedPersonInterest.FkInterestId} not found");
                    }
                    existingPersonInterest.FkInterestId = updatedPersonInterest.FkInterestId;
                }

                await context.SaveChangesAsync();

                return Results.Ok(existingPersonInterest);
            });

            // Delete a PersonInterest
            app.MapDelete("/person/addinterests/{id:int}", async (int id, ApplicationDbContext context) =>
            {
                var existingPersonInterest = await context.PersonInterests.FindAsync(id);
                if (existingPersonInterest == null)
                {
                    return Results.NotFound("PersonInterest not found");
                }

                context.PersonInterests.Remove(existingPersonInterest);
                await context.SaveChangesAsync();

                return Results.Ok($"PersonInterest with ID {id} deleted");
            });


            /////////////////////////////////
            ///      ADD LINK        ///////
            ///////////////////////////////
            ///


            // Get all links for a person
            // Get all links for a person
            app.MapGet("/links/person/{personId:int}", async (int personId, ApplicationDbContext context) =>
            {
                // Check if the person exists
                var existingPerson = await context.Persons.FindAsync(personId);
                if (existingPerson == null)
                {
                    return Results.NotFound("Person not found");
                }

                // Retrieve links for the person
                var links = await context.Links
                                        .Include(l => l.Interest)
                                        .Where(l => l.FkPersonId == personId)
                                        .Select(l => new
                                        {
                                            l.LinkId,
                                            l.FkPersonId,
                                            l.FkInterestId,
                                            l.URL,
                                            Interest = new
                                            {
                                                l.Interest.InterestId,
                                                l.Interest.Title,
                                                l.Interest.Description
                                            }
                                        })
                                        .ToListAsync();

                if (links == null || links.Count == 0)
                {
                    return Results.NotFound("Links not found for this person");
                }

                // Group links by person
                var groupedLinks = links.GroupBy(l => l.FkPersonId)
                                        .Select(group => new
                                        {
                                            Person = new
                                            {
                                                existingPerson.PersonId,
                                                existingPerson.Name,
                                                existingPerson.Phone
                                            },
                                            Links = group.Select(link => new
                                            {
                                                link.LinkId,
                                                link.URL,
                                                link.Interest
                                            }).ToList()
                                        }).ToList();

                return Results.Ok(groupedLinks);
            });


            //addLinK
            app.MapPost("/links/addlinks", async (AddLinkToPersonInterestDTO dto, ApplicationDbContext context) =>
            {
                // Check if the person exists
                var existingPerson = await context.Persons.FindAsync(dto.FkPersonId);
                if (existingPerson == null)
                {
                    return Results.NotFound("Person not found");
                }

                if (dto.Links != null && dto.Links.Any())
                {
                    foreach (var linkDto in dto.Links)
                    {
                        // Check if the interest exists
                        var existingInterest = await context.Interests.FindAsync(linkDto.FkInterestId);
                        if (existingInterest == null)
                        {
                            return Results.NotFound($"Interest with ID {linkDto.FkInterestId} not found");
                        }

                        // Create the link
                        var link = new Link
                        {
                            FkPersonId = dto.FkPersonId,
                            FkInterestId = linkDto.FkInterestId,
                            URL = linkDto.URL
                        };

                        context.Links.Add(link);
                    }
                }
                else
                {
                    return Results.BadRequest("No links provided");
                }

                await context.SaveChangesAsync();

                return Results.Created($"/links/person/{dto.FkPersonId}", "Links added successfully");
            });

            // Edit a link
            app.MapPut("/links/{id:int}", async (int id, LinkDTO updatedLink, ApplicationDbContext context) =>
            {
                var existingLink = await context.Links.FindAsync(id);
                if (existingLink == null)
                {
                    return Results.NotFound("Link not found");
                }

                // Update the URL if provided
                if (!string.IsNullOrEmpty(updatedLink.URL))
                {
                    existingLink.URL = updatedLink.URL;
                }

                await context.SaveChangesAsync();

                return Results.Ok(existingLink);
            });

            // Delete a link
            app.MapDelete("/links/{id:int}", async (int id, ApplicationDbContext context) =>
            {
                var existingLink = await context.Links.FindAsync(id);
                if (existingLink == null)
                {
                    return Results.NotFound("Link not found");
                }

                context.Links.Remove(existingLink);
                await context.SaveChangesAsync();

                return Results.Ok($"Link with ID {id} deleted");
            });

            app.Run();
        }
    }
}
