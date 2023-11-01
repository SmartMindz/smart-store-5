using BizsolTech.Chatbot.Domain;
using BizsolTech.Chatbot.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Smartstore.Web.Controllers;
using Smartstore.Web.Models.DataGrid;

namespace BizsolTech.Chatbot.Controllers
{
    [Route("[area]/Business/{action=index}/{id?}")]
    public class BusinessController : AdminController
    {
        private readonly IBusinessService _businessService;

        public BusinessController(IBusinessService businessService)
        {
            this._businessService = businessService;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return RedirectToAction(nameof(List));
        }
        [HttpGet]
        public IActionResult List()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> List(GridCommand command)
        {
            var businessList = await _businessService.GetAllAsync();
            return Ok(new GridModel<BusinessPageEntity>
            {
                Rows=businessList,
                Total=businessList.Count
            });
        }
    }
}
