using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAnChamSocSucKhoe.Data;
using System.Threading.Tasks;

namespace DoAnChamSocSucKhoe.ViewComponents
{
    public class FeedbackCountViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public FeedbackCountViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Count unresolved feedback (TrangThai = false means not yet resolved)
            var unresolvedCount = await _context.PhanHoiSucKhoes
                .CountAsync(f => !f.TrangThai);

            return View(unresolvedCount);
        }
    }
}
