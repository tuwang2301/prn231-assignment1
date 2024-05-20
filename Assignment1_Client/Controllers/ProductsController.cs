﻿using Assignment1_Api.Models;
using Assignment1_Client.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Assignment1_Client.Controllers
{
    public class ProductsController : Controller
    {
        private readonly HttpClient client = null;
        private string ProductApiUrl = "";
        private string CategoryApiUrl = "";
        public ProductsController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ProductApiUrl = "http://localhost:5267/api/Products";
            CategoryApiUrl = "http://localhost:5267/api/Categories";

        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int? userId = HttpContext.Session.GetInt32("USERID");

            string Role = HttpContext.Session.GetString("ROLE");

            if (userId == null)
            {
                TempData["ErrorMessage"] = "You must login to access this page.";
                return RedirectToAction("Index", "Home");
            }
            else if (Role != "Admin")
            {
                TempData["ErrorMessage"] = "You don't have permission to access this page.";
                return RedirectToAction("Profile", "Staffs");
            }

            List<Product> listProducts = await ApiHandler.DeserializeApiResponse<List<Product>>(ProductApiUrl, HttpMethod.Get);

            ViewData["Categories"] = await ApiHandler.DeserializeApiResponse<List<Category>>(CategoryApiUrl, HttpMethod.Get);

            if (TempData != null)
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
            }

            return View(listProducts);
        }
        public async Task<IActionResult> Search(string keyword)
        {
            List<Product> listMembers = await ApiHandler.DeserializeApiResponse<List<Product>>(ProductApiUrl + "/Search/" + keyword, HttpMethod.Get);

            ViewData["keyword"] = keyword;

            return View("Index", listMembers);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            int? userId = HttpContext.Session.GetInt32("USERID");

            string Role = HttpContext.Session.GetString("ROLE");

            if (userId == null)
            {
                TempData["ErrorMessage"] = "You must login to access this page.";
                return RedirectToAction("Index", "Home");
            }
            else if (Role != "Admin")
            {
                TempData["ErrorMessage"] = "You don't have permission to access this page.";
                return RedirectToAction("Profile", "Staffs");
            }

            List<Category> listCategories = await ApiHandler.DeserializeApiResponse<List<Category>>(CategoryApiUrl, HttpMethod.Get);

            ViewData["Categories"] = listCategories;


            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(ProductApiUrl, product);
            response.EnsureSuccessStatusCode();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            int? userId = HttpContext.Session.GetInt32("USERID");

            string Role = HttpContext.Session.GetString("USERNAME");

            if (userId == null)
            {
                TempData["ErrorMessage"] = "You must login to access this page.";
                return RedirectToAction("Index", "Home");
            }
            else if (Role != "admin@estore.com")
            {
                TempData["ErrorMessage"] = "You don't have permission to access this page.";
                return RedirectToAction("Profile", "Member");
            }

            Product product = await ApiHandler.DeserializeApiResponse<Product>(ProductApiUrl + "/" + id, HttpMethod.Get);
            if (product == null)
            {
                TempData["ErrorMessage"] = "Prouduct not found";
                return RedirectToAction("Index");
            }


            List<Category> listCategories = await ApiHandler.DeserializeApiResponse<List<Category>>(CategoryApiUrl, HttpMethod.Get);

            ViewData["Categories"] = listCategories;


            if (TempData != null)
            {
                ViewData["SuccessMessage"] = TempData["SuccessMessage"];
                ViewData["ErrorMessage"] = TempData["ErrorMessage"];
            }

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Product product)
        {

            await ApiHandler.DeserializeApiResponse(ProductApiUrl + "/" + product.ProductId, HttpMethod.Put, product);
            TempData["SuccessMessage"] = "Product updated successfully";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            int? userId = HttpContext.Session.GetInt32("USERID");

            string Role = HttpContext.Session.GetString("USERNAME");

            if (userId == null)
            {
                TempData["ErrorMessage"] = "You must login to access this page.";
                return RedirectToAction("Index", "Home");
            }
            else if (Role != "admin@estore")
            {
                TempData["ErrorMessage"] = "You don't have permission to access this page.";
                return RedirectToAction("Profile", "Member");
            }

            Product product = await ApiHandler.DeserializeApiResponse<Product>(ProductApiUrl + "/" + id, HttpMethod.Get);
            if (product == null)
            {
                TempData["ErrorMessage"] = "Flower Bouquet not found";
                return RedirectToAction("Index");
            }
            //if (flowerBouquet.FlowerBouquetStatus == 0)
            //{
            //    TempData["ErrorMessage"] = "Flower Bouquet is not available";
            //    return RedirectToAction("Index");
            //}

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Product product)
        {
            await ApiHandler.DeserializeApiResponse<Product>(ProductApiUrl + "/" + product.ProductId, HttpMethod.Delete);
            TempData["SuccessMessage"] = "Flower Bouquet deleted successfully";
            return RedirectToAction("Index");
        }

    }
}