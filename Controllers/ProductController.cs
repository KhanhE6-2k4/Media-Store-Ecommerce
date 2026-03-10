using MediaStore.Data;
using MediaStore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.View;
using System;

namespace MediaStore.Controllers
{
    public class ProductController : Controller
    {
        private readonly AimsContext db;
        private const int PageSize = 6;
        public ProductController(AimsContext context)
        {
            db = context;
        }

        private static string GetTypeName(Media media)
        {
            if (media.Book != null) return "Book";
            if (media.Dvd != null) return "DVD";
            if (media.CdAndLp != null) return media.CdAndLp.IsCd ? "CD" : "LP";
            return "Media";
        }
 
        [HttpGet]
        public IActionResult Index(string? query, string? TypeName, string sorted, int page = 1)
        {
            var products = db.Media
                .Include(m => m.Book)
                .Include(m => m.Dvd)
                .Include(m => m.CdAndLp)
                .AsQueryable();

            // Filter theo type
            if (!string.IsNullOrEmpty(TypeName) && TypeName != "Media")
            {
                products = TypeName switch
                {
                    "Book" => products.Where(p => p.Book != null),
                    "DVD" => products.Where(p => p.Dvd != null),
                    "Cd" => products.Where(p => p.CdAndLp != null && p.CdAndLp.IsCd),
                    "LP" => products.Where(p => p.CdAndLp != null && !p.CdAndLp.IsCd),
                    _ => products
                };
            }

            // Search sp theo title
            if (!string.IsNullOrEmpty(query))
            {
                products = products.Where(p => p.Title != null && p.Title.Contains(query));
            }
            // Sort sp
            products = sorted switch
            {
                "az" => products.OrderBy(p => p.Title),
                "za" => products.OrderByDescending(p => p.Title),
                "price_asc" => products.OrderBy(p => p.Price),
                "price_desc" => products.OrderByDescending(p => p.Price),
                _ => products
            };

            // Pagination
            var totalItems = products.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / PageSize);

            var pagedProducts = products
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            var result = pagedProducts.Select(p => new ProductViewModel
            {
                MediaId = p.MediaId,
                Title = p.Title,
                Price = p.Price,
                ImageUrl = p.ImageUrl,
                Description = p.Description,
                Type = GetTypeName(p),
                TotalQuantity = p.TotalQuantity
            }).ToList(); 

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.query = query;
            ViewBag.TypeName = TypeName;
            ViewBag.TotalProduct = totalItems;
            ViewBag.Sorted = sorted ?? "default";

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_productList", result);
            }
            return View(result);
            // var products = db.Media.ToList(); // ToList() trước để có thể dùng 'is Book'

            // // Lọc theo loại trong bộ nhớ
            // if (!string.IsNullOrEmpty(TypeName) && TypeName != "Media")
            // {
            //     products = products.Where(p =>
            //         (TypeName == "Book" && p.Book != null) ||
            //         (TypeName == "DVD" && p.Dvd != null) ||
            //         (TypeName == "CD" && p.CdAndLp != null && p.CdAndLp.IsCd) ||
            //         (TypeName == "LP" && p.CdAndLp != null && !p.CdAndLp.IsCd)
            //     ).ToList();
            // }

            // // Lọc theo tiêu đề
            // if (!string.IsNullOrEmpty(query))
            // {
            //     products = products.Where(p => p.Title.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
            // }

            // // Sap xe sp
            // if (!string.IsNullOrEmpty(sorted) && sorted != "Default")
            // {
            //     if (sorted == "az")
            //     {
            //         products.Sort((a, b) => string.Compare(a.Title, b.Title));
            //     }
            //     else if (sorted == "za")
            //     {
            //         products.Sort((a, b) => string.Compare(b.Title, a.Title));
            //     }
            //     else if (sorted == "price_asc")
            //     {
            //         products.Sort((a, b) => a.Price.CompareTo(b.Price));
            //     }
            //     else
            //     {
            //         products.Sort((a, b) => b.Price.CompareTo(a.Price));
            //     }
            // }

            // // Tổng số sản phẩm và phân trang
            // var totalItems = products.Count();
            // var totalPages = (int)Math.Ceiling((double)totalItems / PageSize);

            // var pagedProducts = products
            //     .Skip((page - 1) * PageSize)
            //     .Take(PageSize)
            //     .ToList();

            // var result = pagedProducts.Select(p => new ProductViewModel
            // {
            //     MediaId = p.MediaId,
            //     Title = p.Title,
            //     Price = p.Price,
            //     ImageUrl = p.ImageUrl,
            //     Description = p.Description,
            //     Type = GetTypeName(p),
            //     TotalQuantity = p.TotalQuantity
            // }).ToList();

            // ViewBag.CurrentPage = page;
            // ViewBag.TotalPages = totalPages;
            // ViewBag.Query = query;
            // ViewBag.TypeName = TypeName;
            // ViewBag.TotalProduct = products.Count();
            // ViewBag.Sorted = sorted ?? "default"; // mặc định nếu null

            // if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            // {
            //     return PartialView("_productList", result);
            // }

            // return View(result);
        }

        public IActionResult Detail(int id)
        {
            var media = db.Media
                .Include(m => m.Book)
                .Include(m => m.Dvd)
                .Include(m => m.CdAndLp)
                .FirstOrDefault(m => m.MediaId == id);

            if (media == null)
                return NotFound();

            if (media.Book != null)
            {
                var result = new BookViewModel(media.Book);
                return View("ProductDetail/bookDetail", result);
            }
            if (media.Dvd != null)
            {
                var result = new DvdViewModel(media.Dvd);
                return View("ProductDetail/dvdDetail", result);
            }
            if (media.CdAndLp != null)
            {
                if (media.CdAndLp.IsCd) 
                    return View("ProductDetail/cdDetail", new CdViewModel(media.CdAndLp));
                else
                    return View("ProductDetail/lpDetail", new LpViewModel(media.CdAndLp));
            }
            return NotFound();
        }
        // public IActionResult Detail(int id, string type)
        // {
        //     switch (type)
        //     {
        //         case "Book":
        //         {
        //             var book = db.Books.FirstOrDefault(b => b.MediaId == id);
        //             if (book == null)
        //                 return NotFound();
        //             var result = new BookViewModel(book);
        //             return View("ProductDetail/bookDetail", result);
        //         }
        //         case "DVD":
        //         {
        //             var dvd = db.Dvds.FirstOrDefault(b => b.MediaId == id);
        //             if (dvd == null)
        //                 return NotFound();
        //             var result = new DvdViewModel(dvd);
        //             return View("ProductDetail/dvdDetail", result);
        //         }
        //         case "CD":
        //         {
        //             var cd = db.CdAndLps.FirstOrDefault(b => b.MediaId == id && b.IsCd);
        //             if (cd == null)
        //                 return NotFound();
        //             var result = new CdViewModel(cd);
        //             return View("ProductDetail/cdDetail", result);
        //         }
        //         case "LP":
        //         {
        //             var lp = db.CdAndLps.FirstOrDefault(b => b.MediaId == id && !b.IsCd);
        //             if (lp == null)                        
        //                 return NotFound();
        //             var result = new LpViewModel(lp);
        //             return View("ProductDetail/lpDetail", result);
        //         }
        //         default: 
        //             return NotFound();
        //     }
        // }
    }
}
