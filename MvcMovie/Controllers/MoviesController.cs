using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcMovie.Data;
using MvcMovie.Models;

namespace MvcMovie.Controllers
{
    public class MoviesController : Controller
    {
        private readonly MvcMovieContext _context;

        public MoviesController(MvcMovieContext context)
        {
            _context = context;
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
              return _context.Movie != null ? 
                          View( 
                              (await _context.Movie.ToListAsync()).OrderBy(fetch => fetch.ReleaseDate)
                              ) :
                          Problem("Entity set 'MvcMovieContext.Movie'  is null.");
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Movie == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }
        public IActionResult Spend()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,ReleaseDate,Genre,Price")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Movie == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ReleaseDate,Genre,Price")] Movie movie)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        //POST: Movies/Spend/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Spend(int POINTS)
        {
            //go through the list of fetches until all points are spent, using a temporary array.
            //return that array when complete:
            var points = POINTS; // POINTS is a constant used to track how much we start with, points is the return value.
            if (points < 1 || _context.Movie == null)
            {
                return NotFound();
            }

            var fetch = (await _context.Movie.ToListAsync()).OrderBy(f => f.ReleaseDate).ToList();
            //begin looping
            for (var i=0; i< fetch.Count(); i++)
            {
                if ( points > fetch[i].Price)
                {
                    points -= (int) fetch[i].Price;
                    fetch[i].Price = 0; //we will need to delete this after, but continue to make
                                        //sure we have enough points to spend.


                }
                else //fetch[i].Price > points
                {
                    fetch[i].Price -= points;
                    points = 0;
                    break; //we spent the points from a fetch reward, so break
                }
                if (points < 1)
                {
                    break;
                }
                if (i == fetch.Count - 1)
                {
                    //if we get to this point and still have points, then we went over 
                    //the users total. We need to return an error.
                    return Problem("You cannot spend "+POINTS+" points. You are short by "+points+".");


                }
            }
            //update and return
            for (var e = 0; e< fetch.Count(); e++)
            {
                if (fetch[e].Price < 1) //just in case we somehow get a decimal or something odd.
                {
                    _context.Movie.Remove(fetch[e]);
                }
            }
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Movie == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Movie == null)
            {
                return Problem("Entity set 'MvcMovieContext.Movie'  is null.");
            }
            var movie = await _context.Movie.FindAsync(id);
            if (movie != null)
            {
                _context.Movie.Remove(movie);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
          return (_context.Movie?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
