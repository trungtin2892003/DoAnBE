using Microsoft.AspNetCore.Mvc;

namespace ShopCake.Models
{
    public class ContactController : Controller
    {
        private readonly CakeShopContext _context;

        public ContactController(CakeShopContext context)
        {
            _context = context;
        }

        // GET: /Contact
        public IActionResult Index()
        {
            return View();
        }

        // POST: /Contact
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitContactForm(ContactForm contactForm)
        {
            if (ModelState.IsValid)
            {
                // Lưu thông tin vào cơ sở dữ liệu
                _context.ContactForms.Add(contactForm);
                _context.SaveChanges();

                // Hiển thị thông báo thành công
                ViewBag.Message = "Thank you for contacting us!";
            }
            else
            {
                // Hiển thị lỗi nếu có
                ViewBag.Message = "There was an error. Please try again.";
            }

            return View("Index");
        }
    }
}