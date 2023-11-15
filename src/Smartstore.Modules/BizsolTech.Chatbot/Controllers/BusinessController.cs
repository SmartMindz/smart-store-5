using BizsolTech.Chatbot.Domain;
using BizsolTech.Chatbot.Models;
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
            var model = new BusinessListModel();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> BusinessList(GridCommand command, BusinessListModel model)
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
