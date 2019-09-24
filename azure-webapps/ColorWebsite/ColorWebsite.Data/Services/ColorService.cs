using ColorWebsite.Data.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorWebsite.Data.Services
{
    public interface IColorService
    {
        List<DemoColor> GetAllColors();
        void DeleteColors();
        void InsertRandomColors();
    }

    public class ColorService :IColorService
    {
        private readonly ApplicationDBContext _dbContext;

        public ColorService(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
            
        }
        private KnownColor[] _names = (KnownColor[])Enum.GetValues(typeof(KnownColor));


        public List<DemoColor> GetAllColors() =>
             _dbContext.Colors.ToList();

        public void DeleteColors()
        {
            foreach (var color in _dbContext.Colors.ToList())
                _dbContext.Colors.Remove(color);

            _dbContext.SaveChanges();
        }

        public void InsertRandomColors()
        {
            Random random = new Random();

            Color color1 = GetRandomColor(random.Next(_names.Length));
            Color color2 = GetRandomColor(random.Next(_names.Length));
            Color color3 = GetRandomColor(random.Next(_names.Length));

            _dbContext.Colors.Add(new DemoColor { Name = color1.Name });
            _dbContext.Colors.Add(new DemoColor { Name = color2.Name });
            _dbContext.Colors.Add(new DemoColor { Name = color3.Name });
            _dbContext.SaveChanges();
        }

        private Color GetRandomColor(int indexOfColor)
        {
            KnownColor randomColorName = _names[indexOfColor];
            Color randomColor = Color.FromKnownColor(randomColorName);

            return randomColor;
        }
    }
}
