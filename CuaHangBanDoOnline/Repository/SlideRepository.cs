using CuaHangBanDoOnline.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace CuaHangBanDoOnline.Repository
{
    public class SlideRepository : ISlideRepository
    {
        private readonly CuaHangDbContext _context;

        public SlideRepository(CuaHangDbContext context)
        {
            _context = context;
        }

        public List<Slide> GetSlides()
        {
            return _context.Slides.ToList();
        }

        public Slide GetSlide(int id)
        {
            return _context.Slides.Find(id);
        }

        public void AddSlide(Slide slide)
        {
            _context.Slides.Add(slide);
            _context.SaveChanges();
        }

        public void UpdateSlide(Slide slide)
        {
            _context.Slides.Update(slide);
            _context.SaveChanges();
        }

        public void DeleteSlide(int id)
        {
            var slide = _context.Slides.Find(id);
            if (slide != null)
            {
                _context.Slides.Remove(slide);
                _context.SaveChanges();
            }
        }
    }
}