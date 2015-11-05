using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using AirTNG.Web.Models;
using AirTNG.Web.Models.Repository;
using AirTNG.Web.ViewModels;
using Microsoft.AspNet.Identity;

namespace AirTNG.Web.Controllers
{
    [Authorize]
    public class VacationPropertiesController : Controller
    {
        private readonly IVacationPropertiesRepository _repository;

        public Func<string> UserId;

        public VacationPropertiesController() : this(new VacationPropertiesRepository()) { }

        public VacationPropertiesController(IVacationPropertiesRepository repository)
        {
            _repository = repository;
            UserId = () => User.Identity.GetUserId();
        }

        // GET: VacationProperties
        public async Task<ActionResult> Index()
        {
            var vacationProperties = await _repository.AllAsync();
            return View(vacationProperties);
        }

        // GET: VacationProperties/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: VacationProperties/Create
        [HttpPost]
        public async Task<ActionResult> Create(VacationPropertyViewModel model)
        {
            if (ModelState.IsValid)
            {
                var vacationProperty = new VacationProperty
                {
                    UserId = UserId(),
                    Description = model.Description,
                    ImageUrl = model.ImageUrl,
                    CreatedAt = DateTime.Now
                };

                await _repository.CreateAsync(vacationProperty);

                return RedirectToAction("Index");
            }

            return View();
        }

        // GET: VacationProperties/Edit/1
        public async Task<ActionResult> Edit(int id)
        {
            var vacationProperty = await _repository.FindAsync(id);
            return View(vacationProperty);
        }

        // POST: VacationProperties/Edit
        [HttpPost]
        public async Task<ActionResult> Edit(VacationPropertyViewModel model)
        {
            if (ModelState.IsValid)
            {
                var vacationProperty = await _repository.FindAsync(model.Id);
                vacationProperty.Description = model.Description;
                vacationProperty.ImageUrl = model.ImageUrl;

                await _repository.UpdateAsync(vacationProperty);

                return RedirectToAction("Index");
            }

            return View();
        }
    }
}