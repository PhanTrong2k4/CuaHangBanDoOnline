using CuaHangBanDoOnline.Models;
using System.Collections.Generic;

namespace CuaHangBanDoOnline.Repository
{
    public interface ISlideRepository
    {
        List<Slide> GetSlides();
        Slide GetSlide(int id);
        void AddSlide(Slide slide);
        void UpdateSlide(Slide slide);
        void DeleteSlide(int id);
    }
}