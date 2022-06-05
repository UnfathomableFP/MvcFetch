using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MvcMovie.Data;
using System;
using System.Linq;

namespace MvcMovie.Models
{
    public static class SeedData
    {
        public static Movie Check(Movie fetch)
        {
            //this function adds a layer of security to our database, such that any negative points are removed,
            //as points do not go negative. ever.
            //if not negative, add!
            if (fetch.Price > 0)
            {
                return fetch;
            }
            else
            {
                return null;
            }
        }
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new MvcMovieContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<MvcMovieContext>>()))
            {
                // Look for any fetches. If there are some, kill them. kill them now.
                   /*Reason: for this test, we want to eliminate past examples and any errors that 
                     would pop up. Every new test is a new instance.
                   */
                if (context.Movie.Any())
                {
                    foreach(var movie in context.Movie){
                        context.Movie.Remove(movie);
                    }
                    context.SaveChanges();
                }
                //add each fetch individually to the context, using our own Add function.
                try
                {
                    context.Movie.Add(Check(new Movie
                    {
                        Title = "DANNON",
                        ReleaseDate = DateTime.Parse("2020-11-02T14:00:00"),
                        Genre = "",
                        Price = 1000
                    }));
                }catch(Exception e)
                {

                }
                try
                {
                    context.Movie.Add(Check(new Movie{
                    Title = "UNILEVER ",
                        ReleaseDate = DateTime.Parse("2020-10-31T11:00:00"),
                        Genre = "",
                        Price = 200
                }));
                }
                catch (Exception e)
                {

                }
                try
                {
                    context.Movie.Add(Check(new Movie{
                    Title = "DANNON",
                        ReleaseDate = DateTime.Parse("2020-10-31T15:00:00"),
                        Genre = "",
                        Price = -200
                }));
                }
                catch (Exception e)
                {

                }
                try
                {
                    context.Movie.Add(Check(new Movie{
                    Title = "MILLER COORS",
                        ReleaseDate = DateTime.Parse("2020-11-01T14:00:00"),
                        Genre = "",
                        Price = 10000
                }));
                }
                catch (Exception e)
                {

                }
                try
                {
                    context.Movie.Add(Check(new Movie{
                    Title = "DANNON",
                        ReleaseDate = DateTime.Parse("2020-10-31T10:00:00"),
                        Genre = "",
                        Price = 300
                }));
                }
                catch (Exception e)
                {

                }
                    //loop through the context and organize:
                    //context.Movie = context.Movie.OrderBy(x => x.ReleaseDate);

                    var list = context.Movie.ToList();  
                list.Sort();

                context.SaveChanges();
            }
        }
    }
}